namespace UserService.Domain.Common;

/// <summary>
/// Defines the base contract for all entities in the domain.
/// Every entity must have a unique identifier of type <see cref="Guid"/>.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    Guid Id { get; set; }
}