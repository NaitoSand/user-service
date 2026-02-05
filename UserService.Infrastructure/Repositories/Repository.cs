using System.Linq.Expressions;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace UserService.Infrastructure.Repositories;

public class Repository<TEntity, TContext>(
    DbContextFactory<TContext> factory)
    : IRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    public async Task<TEntity?> GetByIdAsync(Guid id) =>
        await factory.ExecuteAsync(async db =>
        {
            var set = db.Set<TEntity>();
            return await set.FindAsync(id);
        });

    public async Task<IEnumerable<TEntity>> GetAllAsync() =>
        await factory.ExecuteAsync(async db => 
            await db.Set<TEntity>().AsNoTracking().ToListAsync());

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate) =>
        await factory.ExecuteAsync(async db => 
            await db.Set<TEntity>().AsNoTracking().Where(predicate).ToListAsync());

    public async Task AddAsync(TEntity entity) =>
        await factory.ExecuteTransactionAsync(async db =>
        {
            await db.Set<TEntity>().AddAsync(entity);
            await db.SaveChangesAsync();
        });

    public async Task UpdateAsync(TEntity entity) =>
        await factory.ExecuteTransactionAsync(async db =>
        {
            db.Set<TEntity>().Update(entity);
            await db.SaveChangesAsync();
        });

    public async Task DeleteAsync(TEntity entity) =>
        await factory.ExecuteTransactionAsync(async db =>
        {
            db.Set<TEntity>().Remove(entity);
            await db.SaveChangesAsync();
        });
}