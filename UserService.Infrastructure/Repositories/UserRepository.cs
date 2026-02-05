using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace UserService.Infrastructure.Repositories;

public class UserRepository(DbContextFactory<UserDbContext> factory)
    : Repository<User, UserDbContext>(factory), IUserRepository
{
    private readonly DbContextFactory<UserDbContext> _factory = factory;

    public Task<User?> GetByEmailAsync(string email) =>
        _factory.ExecuteAsync(async db =>
            await db.Users.FirstOrDefaultAsync(x => x.Email == email));
}