using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.Parameters;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services.Tasks
{
    /// <summary>
    /// Handles complex queries, filtering, and pagination for ClickUp tasks.
    /// Implements the Single Responsibility Principle by focusing only on task querying and filtering operations.
    /// </summary>
    public class TaskQueryService : ITaskQueryService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<TaskQueryService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskQueryService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public TaskQueryService(IApiConnection apiConnection, ILogger<TaskQueryService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<TaskQueryService>.Instance;
        }

        /// <inheritdoc />
        public async Task<IPagedResult<CuTask>> GetTasksAsync(
            string listId,
            Action<GetTasksRequestParameters>? configureParameters = null,
            CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(listId, nameof(listId));

            var parameters = new GetTasksRequestParameters();
            configureParameters?.Invoke(parameters);

            int currentPage = parameters.Page ?? 0; // Default to page 0 if not set
            parameters.Page = currentPage; // Ensure Page is set for ToQueryParametersList

            _logger.LogInformation("Getting tasks for list ID: {ListId}, Parameters: {@Parameters}", listId, parameters);
            
            var sb = new StringBuilder($"list/{listId}/task");
            UrlBuilderHelper.AppendQueryString(sb, parameters.ToQueryParametersList());
            var endpoint = sb.ToString();

            var response = await _apiConnection.GetAsync<GetTasksResponse>(endpoint, cancellationToken);

            if (response == null)
            {
                _logger.LogWarning("API connection returned null response when getting tasks for list {ListId}. Returning empty paged result.", listId);
                return PagedResult<CuTask>.Empty(currentPage);
            }

            var itemsList = (response.Tasks ?? Enumerable.Empty<CuTask>()).ToList();
            var result = new PagedResult<CuTask>(
                itemsList,
                currentPage,
                itemsList.Count,
                response.LastPage == false
            );
            
            _logger.LogDebug("Retrieved {TaskCount} tasks for list {ListId} on page {Page}", itemsList.Count, listId, currentPage);
            return result;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<CuTask> GetTasksAsyncEnumerableAsync(
            string listId,
            GetTasksRequestParameters parameters,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(listId)) throw new ArgumentNullException(nameof(listId));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            _logger.LogInformation("Getting tasks as an async enumerable for list ID: {ListId}, Parameters: {@Parameters}", listId, parameters);

            int currentPage = parameters.Page ?? 0;
            bool lastPage;
            int totalYielded = 0;

            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Fetching page {PageNumber} for tasks in list ID {ListId} via async enumerable.", currentPage, listId);

                // Temporarily set the page for this iteration without modifying the original parameters
                var originalPage = parameters.Page;
                parameters.Page = currentPage;
                
                try
                {
                    var sb = new StringBuilder($"list/{listId}/task");
                    UrlBuilderHelper.AppendQueryString(sb, parameters.ToQueryParametersList());
                    var endpoint = sb.ToString();
                        
                    var response = await _apiConnection.GetAsync<GetTasksResponse>(endpoint, cancellationToken).ConfigureAwait(false);

                    if (response?.Tasks != null && response.Tasks.Any())
                    {
                        foreach (var task in response.Tasks)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            totalYielded++;
                            yield return task;
                        }
                        lastPage = response.LastPage == true; // ClickUp API indicates if it's the last page
                    }
                    else
                    {
                        lastPage = true;
                    }
                }
                finally
                {
                    // Restore the original page value
                    parameters.Page = originalPage;
                }

                if (!lastPage)
                {
                    currentPage++;
                }
            } while (!lastPage);
            
            _logger.LogInformation("Finished streaming {TotalTasks} tasks for list ID: {ListId}", totalYielded, listId);
        }

        /// <inheritdoc />
        public async Task<IPagedResult<CuTask>> GetFilteredTeamTasksAsync(
            string workspaceId,
            Action<GetTasksRequestParameters>? configureParameters = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workspaceId)) throw new ArgumentNullException(nameof(workspaceId));

            var parameters = new GetTasksRequestParameters();
            configureParameters?.Invoke(parameters);

            int currentPage = parameters.Page ?? 0;
            parameters.Page = currentPage; // Ensure Page is set for ToQueryParametersList

            _logger.LogInformation("Getting filtered team tasks for workspace ID: {WorkspaceId}, Parameters: {@Parameters}", workspaceId, parameters);
            
            var sb = new StringBuilder($"team/{workspaceId}/task");
            UrlBuilderHelper.AppendQueryString(sb, parameters.ToQueryParametersList());
            var endpoint = sb.ToString();

            var response = await _apiConnection.GetAsync<GetTasksResponse>(endpoint, cancellationToken);

            if (response == null)
            {
                _logger.LogWarning("API connection returned null response when getting filtered team tasks for workspace {WorkspaceId}. Returning empty paged result.", workspaceId);
                return PagedResult<CuTask>.Empty(currentPage);
            }

            var items = response.Tasks ?? Enumerable.Empty<CuTask>();
            var result = new PagedResult<CuTask>(
                items,
                currentPage,
                items.Count(),
                response.LastPage == false
            );
            
            _logger.LogDebug("Retrieved {TaskCount} filtered team tasks for workspace {WorkspaceId} on page {Page}", items.Count(), workspaceId, currentPage);
            return result;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<CuTask> GetFilteredTeamTasksAsyncEnumerableAsync(
            string workspaceId,
            GetTasksRequestParameters parameters,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workspaceId)) throw new ArgumentNullException(nameof(workspaceId));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            _logger.LogInformation("Getting filtered team tasks as an async enumerable for workspace ID: {WorkspaceId}, Parameters: {@Parameters}", workspaceId, parameters);

            int currentPage = parameters.Page ?? 0;
            bool lastPage;
            int totalYielded = 0;

            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Fetching page {PageNumber} for filtered team tasks in workspace ID {WorkspaceId} via async enumerable.", currentPage, workspaceId);

                // Temporarily set the page for this iteration without modifying the original parameters
                var originalPage = parameters.Page;
                parameters.Page = currentPage;

                try
                {
                    var sb = new StringBuilder($"team/{workspaceId}/task");
                    UrlBuilderHelper.AppendQueryString(sb, parameters.ToQueryParametersList());
                    var endpoint = sb.ToString();
                        
                    var response = await _apiConnection.GetAsync<GetTasksResponse>(endpoint, cancellationToken).ConfigureAwait(false);

                    if (response?.Tasks != null && response.Tasks.Any())
                    {
                        foreach (var task in response.Tasks)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            totalYielded++;
                            yield return task;
                        }
                        lastPage = response.LastPage == true;
                    }
                    else
                    {
                        lastPage = true;
                    }
                }
                finally
                {
                    // Restore the original page value
                    parameters.Page = originalPage;
                }

                if (!lastPage)
                {
                    currentPage++;
                }
            } while (!lastPage);
            
            _logger.LogInformation("Finished streaming {TotalTasks} filtered team tasks for workspace ID: {WorkspaceId}", totalYielded, workspaceId);
        }
    }
}