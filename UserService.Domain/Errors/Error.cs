using UserService.Domain.Enums;

namespace UserService.Domain.Errors;

using Common;

/// <summary>
/// Provides strongly typed, reusable domain and application error definitions.
/// Includes factory helpers for entity-specific and user-related error cases.
/// </summary>
public static class Errors
{
    /// <summary>
    /// Contains generic errors related to entities, such as "not found" or "unexpected".
    /// </summary>
    public static class Entity
    {
        /// <summary>
        /// Creates a standardized "not found" error for a given entity type, including its unique identifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity related to the error.</typeparam>
        /// <param name="id">The unique identifier of the missing entity.</param>
        /// <returns>An <see cref="Error"/> representing a "not found" condition.</returns>
        public static Error NotFound<TEntity>(Guid id) =>
            Error.Create(
                $"{typeof(TEntity).Name}.NotFound",
                $"{typeof(TEntity).Name} with id '{id}' was not found.",
                ErrorType.NotFound
            );

        /// <summary>
        /// Creates a standardized "unexpected" error for a specific entity type.
        /// Used when an unhandled exception occurs without exposing internal details.
        /// </summary>
        /// <typeparam name="TEntity">The entity type related to the error.</typeparam>
        /// <returns>An <see cref="Error"/> representing an unexpected error for the given entity type.</returns>
        public static Error Unexpected<TEntity>() =>
            Error.Create(
                $"{typeof(TEntity).Name}.Unexpected",
                $"An unexpected error occurred while processing {typeof(TEntity).Name}.",
                ErrorType.Unexpected
            );
    }

    /// <summary>
    /// Contains errors specific to the <c>User</c> entity and user-related operations.
    /// </summary>
    public static class User
    {
        /// <summary>
        /// Indicates that the email address exceeds the maximum allowed length.
        /// </summary>
        public static readonly Error EmailTooLong =
            Error.Create("User.EmailTooLong", "Email exceeds the maximum allowed length.", ErrorType.Validation);

        /// <summary>
        /// Indicates that the full name exceeds the maximum allowed length.
        /// </summary>
        public static readonly Error FullNameTooLong =
            Error.Create("User.FullNameTooLong", "Full name exceeds the maximum allowed length.", ErrorType.Validation);

        /// <summary>
        /// Indicates that a user with the specified email already exists in the system.
        /// </summary>
        public static readonly Error EmailConflict =
            Error.Create("User.EmailConflict", "Email is already registered.", ErrorType.Conflict);

        /// <summary>
        /// Indicates that the email field is required but was not provided.
        /// </summary>
        public static readonly Error MissingEmail =
            Error.Create("User.MissingEmail", "Email is required.", ErrorType.Validation);

        /// <summary>
        /// Indicates that the full name field is required but was not provided.
        /// </summary>
        public static readonly Error MissingFullName =
            Error.Create("User.MissingFullName", "Full name is required.", ErrorType.Validation);
    }
}
