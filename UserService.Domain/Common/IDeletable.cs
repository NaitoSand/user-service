namespace UserService.Domain.Common;

/// <summary>
/// Defines a contract for entities that support soft deletion,
/// allowing logical removal without physically deleting the record from the database.
/// </summary>
public interface IDeletable
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is active.
    /// When set to <c>false</c>, the entity is considered soft-deleted.
    /// </summary>
    bool IsActive { get; set; }
}