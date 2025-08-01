using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Helpers;

namespace ClickUp.Api.Client.Patterns.Repository
{
    /// <summary>
    /// Provides a base implementation for the repository pattern with ClickUp API integration.
    /// This class abstracts common data access patterns and provides a foundation for specific repositories.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this repository manages.</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
    public abstract class BaseRepository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class
        where TId : IEquatable<TId>
    {
        protected readonly IApiConnection ApiConnection;
        protected readonly ILogger Logger;

        /// <summary>
        /// Gets the base endpoint for this repository's API operations.
        /// </summary>
        protected abstract string BaseEndpoint { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{TEntity, TId}"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this repository.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        protected BaseRepository(IApiConnection apiConnection, ILogger logger)
        {
            ApiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            ValidateId(id);

            try
            {
                var endpoint = BuildEntityEndpoint(id);
                Logger.LogDebug("Repository: Getting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                
                var response = await ApiConnection.GetAsync<TEntity>(endpoint, cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error getting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<PagedResult<TEntity>> GetAllAsync(int page = 0, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            ValidatePagination(page, pageSize);

            try
            {
                var endpoint = BuildListEndpoint(page, pageSize);
                Logger.LogDebug("Repository: Getting {EntityType} list - Page: {Page}, PageSize: {PageSize}", typeof(TEntity).Name, page, pageSize);
                
                var response = await ApiConnection.GetAsync<IEnumerable<TEntity>>(endpoint, cancellationToken);
                var items = response?.ToList() ?? new List<TEntity>();
                
                return new PagedResult<TEntity>(items, page, pageSize, items.Count);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error getting {EntityType} list", typeof(TEntity).Name);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            try
            {
                // Note: This is a simplified implementation. In practice, you would need to translate
                // the expression to API query parameters or handle filtering on the client side.
                Logger.LogDebug("Repository: Finding {EntityType} with predicate", typeof(TEntity).Name);
                
                var allItems = await GetAllAsync(0, 1000, cancellationToken);
                var compiledPredicate = predicate.Compile();
                
                return allItems.Items.Where(compiledPredicate);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error finding {EntityType} with predicate", typeof(TEntity).Name);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<PagedResult<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            
            ValidatePagination(page, pageSize);

            try
            {
                Logger.LogDebug("Repository: Finding {EntityType} with predicate - Page: {Page}, PageSize: {PageSize}", typeof(TEntity).Name, page, pageSize);
                
                var allMatching = await FindAsync(predicate, cancellationToken);
                var items = allMatching.Skip(page * pageSize).Take(pageSize).ToList();
                var totalCount = allMatching.Count();
                
                return new PagedResult<TEntity>(items, page, pageSize, totalCount);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error finding {EntityType} with predicate and pagination", typeof(TEntity).Name);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            try
            {
                Logger.LogDebug("Repository: Getting first {EntityType} with predicate", typeof(TEntity).Name);
                
                var results = await FindAsync(predicate, cancellationToken);
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error getting first {EntityType} with predicate", typeof(TEntity).Name);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            ValidateEntity(entity);

            try
            {
                var endpoint = BaseEndpoint;
                Logger.LogDebug("Repository: Creating {EntityType}", typeof(TEntity).Name);
                
                var response = await ApiConnection.PostAsync<TEntity, TEntity>(endpoint, entity, cancellationToken);
                return response ?? throw new InvalidOperationException("API returned null response for create operation.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error creating {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            ValidateEntity(entity);

            try
            {
                // Note: This assumes the entity has an ID property. In practice, you might need
                // to extract the ID using reflection or require entities to implement an interface.
                var id = ExtractEntityId(entity);
                var endpoint = BuildEntityEndpoint(id);
                Logger.LogDebug("Repository: Updating {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                
                var response = await ApiConnection.PutAsync<TEntity, TEntity>(endpoint, entity, cancellationToken);
                return response ?? throw new InvalidOperationException("API returned null response for update operation.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error updating {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
        {
            ValidateId(id);

            try
            {
                var endpoint = BuildEntityEndpoint(id);
                Logger.LogDebug("Repository: Deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                
                await ApiConnection.DeleteAsync(endpoint, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            ValidateEntity(entity);

            try
            {
                var id = ExtractEntityId(entity);
                return await DeleteAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error deleting {EntityType} entity", typeof(TEntity).Name);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await GetByIdAsync(id, cancellationToken);
                return entity != null;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            try
            {
                var result = await FirstOrDefaultAsync(predicate, cancellationToken);
                return result != null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error checking if any {EntityType} matches predicate", typeof(TEntity).Name);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                Logger.LogDebug("Repository: Counting {EntityType}", typeof(TEntity).Name);
                
                if (predicate == null)
                {
                    var allItems = await GetAllAsync(0, int.MaxValue, cancellationToken);
                    return allItems.Items.Count;
                }
                else
                {
                    var matchingItems = await FindAsync(predicate, cancellationToken);
                    return matchingItems.Count();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error counting {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var idList = ids.ToList();
            if (!idList.Any())
                return Enumerable.Empty<TEntity>();

            try
            {
                Logger.LogDebug("Repository: Getting {EntityType} by IDs: {Count} items", typeof(TEntity).Name, idList.Count);
                
                var tasks = idList.Select(id => GetByIdAsync(id, cancellationToken));
                var results = await Task.WhenAll(tasks);
                
                return results.Where(r => r != null).Cast<TEntity>();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error getting {EntityType} by IDs", typeof(TEntity).Name);
                throw;
            }
        }

        /// <summary>
        /// Builds the endpoint for a specific entity by ID.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <returns>The complete endpoint URL.</returns>
        protected virtual string BuildEntityEndpoint(TId id)
        {
            return UrlBuilderHelper.CombinePath(BaseEndpoint, id?.ToString() ?? string.Empty);
        }

        /// <summary>
        /// Builds the endpoint for listing entities with pagination.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The complete endpoint URL with pagination parameters.</returns>
        protected virtual string BuildListEndpoint(int page, int pageSize)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["page"] = page.ToString(),
                ["limit"] = pageSize.ToString()
            };
            var queryString = UrlBuilderHelper.BuildQueryString(queryParams);
            return BaseEndpoint + queryString;
        }

        /// <summary>
        /// Extracts the ID from an entity. This method should be overridden in derived classes
        /// to provide entity-specific ID extraction logic.
        /// </summary>
        /// <param name="entity">The entity to extract the ID from.</param>
        /// <returns>The entity's identifier.</returns>
        /// <exception cref="NotImplementedException">Thrown if not implemented in derived class.</exception>
        protected virtual TId ExtractEntityId(TEntity entity)
        {
            throw new NotImplementedException("ExtractEntityId must be implemented in derived repository classes.");
        }

        /// <summary>
        /// Validates an entity identifier.
        /// </summary>
        /// <param name="id">The identifier to validate.</param>
        /// <exception cref="ArgumentException">Thrown if the ID is invalid.</exception>
        protected virtual void ValidateId(TId id)
        {
            if (id == null || id.Equals(default(TId)))
                throw new ArgumentException("ID cannot be null or default value.", nameof(id));
        }

        /// <summary>
        /// Validates an entity.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown if the entity is null.</exception>
        protected virtual void ValidateEntity(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
        }

        /// <summary>
        /// Validates pagination parameters.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if pagination parameters are invalid.</exception>
        protected virtual void ValidatePagination(int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentOutOfRangeException(nameof(page), "Page number cannot be negative.");
            
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            
            if (pageSize > 1000)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size cannot exceed 1000.");
        }
    }

    /// <summary>
    /// Simplified base repository for entities with string identifiers.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this repository manages.</typeparam>
    public abstract class BaseRepository<TEntity> : BaseRepository<TEntity, string>
        where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this repository.</param>
        protected BaseRepository(IApiConnection apiConnection, ILogger logger)
            : base(apiConnection, logger)
        {
        }
    }
}