namespace UserService.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Data;

/// <summary>
/// Generic factory for controlled EF Core context and raw SQL access.
/// </summary>
public class DbContextFactory<TContext>(
    IDbContextFactory<TContext> contextFactory,
    IConfiguration configuration)
    where TContext : DbContext
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
                                                ?? throw new InvalidOperationException("Connection string not configured.");

    // --- CONTEXT (no transaction)
    public async Task ExecuteAsync(Func<TContext, Task> action)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        await action(db);
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<TContext, Task<TResult>> func)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        return await func(db);
    }

    // --- CONTEXT + TRANSACTION
    public async Task ExecuteTransactionAsync(Func<TContext, Task> action)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            await action(db);
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<TResult> ExecuteTransactionAsync<TResult>(Func<TContext, Task<TResult>> func)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            var result = await func(db);
            await tx.CommitAsync();
            return result;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    // --- RAW CONNECTION (for Dapper)
    public async Task UseRawConnectionAsync(Func<IDbConnection, Task> action)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();
        await action(conn);
    }

    public async Task<TResult> UseRawConnectionAsync<TResult>(Func<IDbConnection, Task<TResult>> func)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();
        return await func(conn);
    }

    private DbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}
