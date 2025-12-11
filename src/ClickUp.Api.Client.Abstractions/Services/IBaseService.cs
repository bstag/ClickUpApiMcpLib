using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Common.Pagination;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines the contract for a generic base service that provides common CRUD operations.
    /// This interface follows the DRY principle by centralizing common service patterns.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this service manages.</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
    public interface IBaseService<TEntity, TId>
        where TEntity : class
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Gets an entity by its identifier.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all entities with optional pagination.
        /// </summary>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A paginated result containing the entities.</returns>
        Task<PagedResult<TEntity>> GetAllAsync(int page = 0, int pageSize = 50, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created entity.</returns>
        Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="entity">The updated entity data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated entity.</returns>
        Task<TEntity> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the entity was deleted; otherwise, false.</returns>
        Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an entity exists by its identifier.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the entity exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets entities by a collection of identifiers.
        /// </summary>
        /// <param name="ids">The collection of entity identifiers.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of entities that match the provided identifiers.</returns>
        Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Simplified base service interface for entities with string identifiers.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this service manages.</typeparam>
    public interface IBaseService<TEntity> : IBaseService<TEntity, string>
        where TEntity : class
    {
    }
}