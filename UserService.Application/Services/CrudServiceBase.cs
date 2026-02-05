using UserService.Application.Interfaces;
using UserService.Domain.Common;
using UserService.Domain.Errors;
using Microsoft.Extensions.Logging;

namespace UserService.Application.Services;

/// <summary>
/// Provides a minimal generic base for CRUD operations with unified error handling
/// and optional exception logging. Intended for application-layer services.
/// </summary>
public abstract class CrudServiceBase<TEntity, TRepository>(
    TRepository repository,
    ILogger logger)
    where TEntity : class, IEntity
    where TRepository : IRepository<TEntity>
{
    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// Returns a <see cref="Result{TEntity}"/> containing the entity if found,
    /// or a failure result if it does not exist or an exception occurs.
    /// </summary>
    public virtual async Task<Result<TEntity>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await repository.GetByIdAsync(id);
            return entity is not null
                ? Result<TEntity>.Success(entity)
                : Result<TEntity>.Failure(Errors.Entity.NotFound<TEntity>(id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while fetching {Entity} with ID {Id}", typeof(TEntity).Name, id);
            return Result<TEntity>.Failure(Errors.Entity.Unexpected<TEntity>());
        }
    }

    /// <summary>
    /// Retrieves all entities from the repository.
    /// Returns a success result or a failure result if an exception occurs.
    /// </summary>
    public virtual async Task<Result<IEnumerable<TEntity>>> GetAllAsync()
    {
        try
        {
            var all = await repository.GetAllAsync();
            return Result<IEnumerable<TEntity>>.Success(all);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while fetching all {Entity} records", typeof(TEntity).Name);
            return Result<IEnumerable<TEntity>>.Failure(Errors.Entity.Unexpected<TEntity>());
        }
    }

    /// <summary>
    /// Creates a new entity in the repository.
    /// Must be implemented in derived classes to apply entity-specific rules.
    /// </summary>
    public abstract Task<Result<TEntity>> CreateAsync(TEntity entity);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// Must be implemented in derived classes to apply entity-specific rules.
    /// </summary>
    public abstract Task<Result<TEntity>> UpdateAsync(TEntity entity);

    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// Returns <see cref="Result.Success"/> if deleted successfully,
    /// or <see cref="Result.Failure"/> if not found or an exception occurs.
    /// </summary>
    public virtual async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await repository.GetByIdAsync(id);
            if (entity is null)
                return Result.Failure(Errors.Entity.NotFound<TEntity>(id));

            await repository.DeleteAsync(entity);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while deleting {Entity} with ID {Id}", typeof(TEntity).Name, id);
            return Result.Failure(Errors.Entity.Unexpected<TEntity>());
        }
    }
}
