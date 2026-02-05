using UserService.Domain.Common;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

public interface IUserService
{
    Task<Result<User>> CreateAsync(User user);
    Task<Result<User>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<User>>> GetAllAsync();
    Task<Result<User>> UpdateAsync(User updatedUser);
    Task<Result> DeleteAsync(Guid id);
}
