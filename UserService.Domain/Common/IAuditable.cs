namespace UserService.Domain.Common;

/// <summary>
/// Defines common auditing metadata for entities that require tracking
/// of creation and modification timestamps.
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    DateTime UpdatedAt { get; set; }
}