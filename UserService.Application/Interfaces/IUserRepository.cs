namespace UserService.Application.Interfaces;

using Domain.Entities;

public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Gets user by email.
    /// </summary>
    Task<User?> GetByEmailAsync(string email);
}
