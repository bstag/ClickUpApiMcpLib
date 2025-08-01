using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Common.Pagination;

namespace ClickUp.Api.Client.Patterns.Repository
{
    /// <summary>
    /// Defines the contract for a generic repository that provides data access operations.
    /// This interface follows the Repository pattern to abstract data access logic.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this repository manages.</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
    public interface IRepository<TEntity, TId>
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
        /// Finds entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of entities that match the predicate.</returns>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds entities that match the specified predicate with pagination.
        /// </summary>
        /// <param name="predicate">The predicate to filter entities.</param>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A paginated result containing the entities that match the predicate.</returns>
        Task<PagedResult<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the first entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The first entity that matches the predicate; otherwise, null.</returns>
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

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
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated entity.</returns>
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the entity was deleted; otherwise, false.</returns>
        Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the entity was deleted; otherwise, false.</returns>
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an entity exists by its identifier.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the entity exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if any entity matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to check.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if any entity matches the predicate; otherwise, false.</returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter entities. If null, counts all entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The count of entities.</returns>
        Task<long> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets entities by a collection of identifiers.
        /// </summary>
        /// <param name="ids">The collection of entity identifiers.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of entities that match the provided identifiers.</returns>
        Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Simplified repository interface for entities with string identifiers.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this repository manages.</typeparam>
    public interface IRepository<TEntity> : IRepository<TEntity, string>
        where TEntity : class
    {
    }
}