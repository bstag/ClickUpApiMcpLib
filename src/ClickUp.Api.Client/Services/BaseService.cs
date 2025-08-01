using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Plugins;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Helpers;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Plugins;
using ClickUp.Api.Client.Validation;
using Microsoft.Extensions.Logging;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Base service providing common CRUD operations for ClickUp API entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this service manages.</typeparam>
    /// <typeparam name="TId">The type of the entity identifier.</typeparam>
    public abstract class BaseService<TEntity, TId> : IBaseService<TEntity, TId>
        where TEntity : class
        where TId : IEquatable<TId>
    {
        protected readonly IApiConnection ApiConnection;
        protected readonly ILogger Logger;
        protected readonly IPluginManager? PluginManager;

        /// <summary>
        /// Gets the base endpoint for this entity type.
        /// </summary>
        protected abstract string BaseEndpoint { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{TEntity, TId}"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <param name="pluginManager">The plugin manager for executing plugins (optional).</param>
        protected BaseService(IApiConnection apiConnection, ILogger logger, IPluginManager? pluginManager = null)
        {
            ApiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            PluginManager = pluginManager;
        }

        /// <inheritdoc />
        public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            ValidateId(id);

            try
            {
                await ExecuteBeforePluginsAsync("GetById", new { Id = id }, cancellationToken);
                
                var endpoint = BuildEntityEndpoint(id);
                Logger.LogDebug("Getting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                
                var response = await ApiConnection.GetAsync<TEntity>(endpoint, cancellationToken);
                
                await ExecuteAfterPluginsAsync("GetById", new { Id = id }, response, cancellationToken);
                
                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<PagedResult<TEntity>> GetAllAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            ValidatePagination(page, pageSize);

            try
            {
                var endpoint = BuildListEndpoint(page, pageSize);
                Logger.LogDebug("Getting {EntityType} list - Page: {Page}, PageSize: {PageSize}", typeof(TEntity).Name, page, pageSize);
                
                var response = await ApiConnection.GetAsync<IEnumerable<TEntity>>(endpoint, cancellationToken);
                var items = response?.ToList() ?? new List<TEntity>();
                
                // Note: This is a simplified implementation. In practice, you might need to handle
                // different response formats depending on the API endpoint.
                return new PagedResult<TEntity>(items, page, pageSize, items.Count);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting {EntityType} list", typeof(TEntity).Name);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            ValidateEntity(entity);

            try
            {
                await ExecuteBeforePluginsAsync("Create", entity, cancellationToken);
                
                var endpoint = BaseEndpoint;
                Logger.LogDebug("Creating {EntityType}", typeof(TEntity).Name);
                
                var response = await ApiConnection.PostAsync<TEntity, TEntity>(endpoint, entity, cancellationToken);
                var result = response ?? throw new InvalidOperationException("API returned null response for create operation.");
                
                await ExecuteAfterPluginsAsync("Create", entity, result, cancellationToken);
                
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default)
        {
            ValidateId(id);
            ValidateEntity(entity);

            try
            {
                await ExecuteBeforePluginsAsync("Update", new { Id = id, Entity = entity }, cancellationToken);
                
                var endpoint = BuildEntityEndpoint(id);
                Logger.LogDebug("Updating {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                
                var response = await ApiConnection.PutAsync<TEntity, TEntity>(endpoint, entity, cancellationToken);
                var result = response ?? throw new InvalidOperationException("API returned null response for update operation.");
                
                await ExecuteAfterPluginsAsync("Update", new { Id = id, Entity = entity }, result, cancellationToken);
                
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
        {
            ValidateId(id);

            try
            {
                await ExecuteBeforePluginsAsync("Delete", new { Id = id }, cancellationToken);
                
                var endpoint = BuildEntityEndpoint(id);
                Logger.LogDebug("Deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                
                await ApiConnection.DeleteAsync(endpoint, cancellationToken);
                
                await ExecuteAfterPluginsAsync("Delete", new { Id = id }, true, cancellationToken);
                
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
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
        public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var idList = ids.ToList();
            if (!idList.Any())
                return Enumerable.Empty<TEntity>();

            try
            {
                Logger.LogDebug("Getting {EntityType} by IDs: {Count} items", typeof(TEntity).Name, idList.Count);
                
                var tasks = idList.Select(id => GetByIdAsync(id, cancellationToken));
                var results = await Task.WhenAll(tasks);
                
                return results.Where(r => r != null).Cast<TEntity>();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting {EntityType} by IDs", typeof(TEntity).Name);
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
            return UrlBuilderHelper.CombinePath(BaseEndpoint, id.ToString()!);
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
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than zero.");
            
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            
            if (pageSize > 1000)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size cannot exceed 1000.");
        }

        /// <summary>
        /// Executes plugins before an API operation.
        /// </summary>
        /// <param name="operationType">The type of operation being performed.</param>
        /// <param name="requestData">The request data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected virtual async Task ExecuteBeforePluginsAsync(string operationType, object? requestData = null, CancellationToken cancellationToken = default)
        {
            if (PluginManager == null) return;

            try
            {
                var requestDict = requestData != null ? 
                    new Dictionary<string, object> { ["data"] = requestData } : 
                    new Dictionary<string, object>();
                    
                var context = new PluginContext(
                    ApiConnection,
                    operationType,
                    GetType().Name,
                    requestDict);

                await PluginManager.ExecutePluginsAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error executing before plugins for operation: {OperationType}", operationType);
            }
        }

        /// <summary>
        /// Executes plugins after an API operation.
        /// </summary>
        /// <param name="operationType">The type of operation being performed.</param>
        /// <param name="requestData">The request data.</param>
        /// <param name="responseData">The response data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected virtual async Task ExecuteAfterPluginsAsync(string operationType, object? requestData = null, object? responseData = null, CancellationToken cancellationToken = default)
        {
            if (PluginManager == null) return;

            try
            {
                var requestDict = requestData != null ? 
                    new Dictionary<string, object> { ["data"] = requestData } : 
                    new Dictionary<string, object>();
                    
                var responseDict = responseData != null ? 
                    new Dictionary<string, object> { ["data"] = responseData } : 
                    new Dictionary<string, object>();
                    
                var context = new PluginContext(
                    ApiConnection,
                    operationType,
                    GetType().Name,
                    requestDict,
                    responseDict);

                await PluginManager.ExecutePluginsAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error executing after plugins for operation: {OperationType}", operationType);
            }
        }
    }

    /// <summary>
    /// Simplified base service for entities with string identifiers.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this service manages.</typeparam>
    public abstract class BaseService<TEntity> : BaseService<TEntity, string>
        where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{TEntity}"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <param name="pluginManager">The plugin manager for executing plugins (optional).</param>
        protected BaseService(IApiConnection apiConnection, ILogger logger, IPluginManager? pluginManager = null)
            : base(apiConnection, logger, pluginManager)
        {
        }
    }
}