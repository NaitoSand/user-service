using UserService.Application.Interfaces;
using UserService.Domain.Common;
using UserService.Domain.Entities;
using UserService.Domain.Errors;
using Microsoft.Extensions.Logging;

namespace UserService.Application.Services;

/// <summary>
/// Application-level service that handles business logic for <see cref="User"/> entities.
/// Performs validation and ensures consistency before delegating work to the repository.
/// </summary>
public class UserService(IUserRepository repository, ILogger<UserService> logger)
    : CrudServiceBase<User, IUserRepository>(repository, logger), IUserService
{
    private readonly IUserRepository _repository = repository;
    private const int MaxFieldLength = 200;

    /// <summary>
    /// Creates a new user after validation and duplicate email checks.
    /// </summary>
    /// <param name="user">User entity to create.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> representing success or validation/conflict failure.
    /// </returns>
    public override async Task<Result<User>> CreateAsync(User user)
    {
        // --- Validation ---
        if (string.IsNullOrWhiteSpace(user.Email))
            return Result<User>.Failure(Errors.User.MissingEmail);

        if (user.Email.Length > MaxFieldLength)
            return Result<User>.Failure(Errors.User.EmailTooLong);

        if (string.IsNullOrWhiteSpace(user.FullName))
            return Result<User>.Failure(Errors.User.MissingFullName);

        if (user.FullName.Length > MaxFieldLength)
            return Result<User>.Failure(Errors.User.FullNameTooLong);

        // --- Business rule: email must be unique ---
        if (await _repository.GetByEmailAsync(user.Email) is not null)
            return Result<User>.Failure(Errors.User.EmailConflict);

        // --- Assign domain defaults ---
        user.Id = Guid.NewGuid();
        user.IsActive = true;

        await _repository.AddAsync(user);
        return Result<User>.Success(user);
    }

    /// <summary>
    /// Updates an existing user after validation and conflict checks.
    /// </summary>
    /// <param name="user">User entity with updated data.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> representing success or validation/conflict failure.
    /// </returns>
    public override async Task<Result<User>> UpdateAsync(User user)
    {
        // --- Validation ---
        if (string.IsNullOrWhiteSpace(user.Email))
            return Result<User>.Failure(Errors.User.MissingEmail);

        if (user.Email.Length > MaxFieldLength)
            return Result<User>.Failure(Errors.User.EmailTooLong);

        if (string.IsNullOrWhiteSpace(user.FullName))
            return Result<User>.Failure(Errors.User.MissingFullName);

        if (user.FullName.Length > MaxFieldLength)
            return Result<User>.Failure(Errors.User.FullNameTooLong);

        // --- Existence check ---
        var existing = await _repository.GetByIdAsync(user.Id);
        if (existing is null)
            return Result<User>.Failure(Errors.Entity.NotFound<User>(user.Id));

        // --- Business rule: email must be unique ---
        var emailOwner = await _repository.GetByEmailAsync(user.Email);
        if (emailOwner != null && emailOwner.Id != user.Id)
            return Result<User>.Failure(Errors.User.EmailConflict);

        // --- Apply updates ---
        existing.Email = user.Email;
        existing.FullName = user.FullName;
        existing.IsActive = user.IsActive;

        await _repository.UpdateAsync(existing);
        return Result<User>.Success(existing);
    }
}
