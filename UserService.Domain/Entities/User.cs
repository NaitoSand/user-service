using UserService.Domain.Common;

namespace UserService.Domain.Entities;

/// <summary>
/// Represents an application user stored in the system.
/// </summary>
public class User : IEntity, IDeletable, IAuditable
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User's email address (must be unique, required, max length 200).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the user (required, max length 200).
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the user record is active (soft delete support).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date and time when the record was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the record was last updated (UTC).
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}