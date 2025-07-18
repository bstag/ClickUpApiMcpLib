using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Parameters; // Updated to use the new Parameters namespace
using ClickUp.Api.Client.Helpers;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements the <see cref="ITasksService"/> interface for interacting with ClickUp Tasks.
    /// </summary>
    public class TaskService : ITasksService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<TaskService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public TaskService(IApiConnection apiConnection, ILogger<TaskService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<TaskService>.Instance;
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
            var endpoint = $"list/{listId}/task";

            var queryParamsList = parameters.ToQueryParametersList();
            var fullEndpoint = endpoint + UrlBuilderHelper.BuildQueryString(queryParamsList);
            var response = await _apiConnection.GetAsync<GetTasksResponse>(fullEndpoint, cancellationToken);

            if (response == null)
            {
                _logger.LogWarning("API connection returned null response when getting tasks for list {ListId}. Returning empty paged result.", listId);
                return PagedResult<CuTask>.Empty(currentPage);
            }

            var itemsList = (response.Tasks ?? Enumerable.Empty<CuTask>()).ToList();
            return new PagedResult<CuTask>(
                itemsList,
                currentPage,
                itemsList.Count,
                response.LastPage == false
            );
        }

        /// <inheritdoc />
        public async Task<CuTask> CreateTaskAsync(
            string listId,
            CreateTaskRequest createTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(listId, nameof(listId));
            if (createTaskRequest == null) throw new ArgumentNullException(nameof(createTaskRequest));
            ValidationHelper.ValidateRequiredString(createTaskRequest.Name, nameof(createTaskRequest.Name));
            if (!string.IsNullOrEmpty(teamId)) ValidationHelper.ValidateId(teamId, nameof(teamId));

            _logger.LogInformation("Creating task in list ID: {ListId} with name: {TaskName}", listId, createTaskRequest.Name);
            var endpoint = $"list/{listId}/task";
            var queryParamsList = new List<KeyValuePair<string, string>>();
            if (customTaskIds.HasValue) queryParamsList.Add(new KeyValuePair<string, string>("custom_task_ids", customTaskIds.Value.ToString().ToLower()));
            if (!string.IsNullOrEmpty(teamId)) queryParamsList.Add(new KeyValuePair<string, string>("team_id", teamId));
            endpoint += UrlBuilderHelper.BuildQueryString(queryParamsList);

            var task = await _apiConnection.PostAsync<CreateTaskRequest, CuTask>(endpoint, createTaskRequest, cancellationToken);
            if (task == null)
            {
                throw new InvalidOperationException($"API connection returned null response when creating task in list {listId}.");
            }
            return task;
        }

        /// <inheritdoc />
        public async Task<CuTask> GetTaskAsync(
            string taskId,
            GetTaskRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting task with ID: {TaskId}", taskId);
            var endpoint = $"task/{taskId}";
            var queryParamsList = new List<KeyValuePair<string, string>>();
            if (requestModel.CustomTaskIds.HasValue) queryParamsList.Add(new KeyValuePair<string, string>("custom_task_ids", requestModel.CustomTaskIds.Value.ToString().ToLower()));
            if (!string.IsNullOrEmpty(requestModel.TeamId)) queryParamsList.Add(new KeyValuePair<string, string>("team_id", requestModel.TeamId));
            if (requestModel.IncludeSubtasks.HasValue) queryParamsList.Add(new KeyValuePair<string, string>("include_subtasks", requestModel.IncludeSubtasks.Value.ToString().ToLower()));
            if (requestModel.IncludeMarkdownDescription.HasValue) queryParamsList.Add(new KeyValuePair<string, string>("include_markdown_description", requestModel.IncludeMarkdownDescription.Value.ToString().ToLower()));
            if (requestModel.Page.HasValue) queryParamsList.Add(new KeyValuePair<string, string>("page", requestModel.Page.Value.ToString()));
            var fullEndpoint = endpoint + UrlBuilderHelper.BuildQueryString(queryParamsList);

            var task = await _apiConnection.GetAsync<CuTask>(fullEndpoint, cancellationToken);
            if (task == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting task {taskId}.");
            }
            return task;
        }

        /// <inheritdoc />
        public async Task<CuTask> UpdateTaskAsync(
            string taskId,
            UpdateTaskRequest updateTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating task with ID: {TaskId}", taskId);
            var endpoint = $"task/{taskId}";
            var queryParamsList = new List<KeyValuePair<string, string>>();
            if (customTaskIds.HasValue) queryParamsList.Add(new KeyValuePair<string, string>("custom_task_ids", customTaskIds.Value.ToString().ToLower()));
            if (!string.IsNullOrEmpty(teamId)) queryParamsList.Add(new KeyValuePair<string, string>("team_id", teamId));
            endpoint += UrlBuilderHelper.BuildQueryString(queryParamsList);

            var task = await _apiConnection.PutAsync<UpdateTaskRequest, CuTask>(endpoint, updateTaskRequest, cancellationToken);
            if (task == null)
            {
                throw new InvalidOperationException($"API connection returned null response when updating task {taskId}.");
            }
            return task;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteTaskAsync(
            string taskId,
            DeleteTaskRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting task with ID: {TaskId}", taskId);
            var endpoint = $"task/{taskId}";
            var queryParamsList = new List<KeyValuePair<string, string>>();
            if (requestModel.CustomTaskIds.HasValue) queryParamsList.Add(new KeyValuePair<string, string>("custom_task_ids", requestModel.CustomTaskIds.Value.ToString().ToLower()));
            if (!string.IsNullOrEmpty(requestModel.TeamId)) queryParamsList.Add(new KeyValuePair<string, string>("team_id", requestModel.TeamId));
            endpoint += UrlBuilderHelper.BuildQueryString(queryParamsList);

            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CuTask> MergeTasksAsync(
            string targetTaskId,
            MergeTasksRequest mergeTasksRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Merging tasks into target task ID: {TargetTaskId}", targetTaskId);

            if (string.IsNullOrWhiteSpace(targetTaskId))
            {
                throw new ArgumentNullException(nameof(targetTaskId));
            }
            if (mergeTasksRequest == null)
            {
                throw new ArgumentNullException(nameof(mergeTasksRequest));
            }
            if (mergeTasksRequest.SourceTaskIds == null || !mergeTasksRequest.SourceTaskIds.Any())
            {
                throw new ArgumentException("MergeTasksRequest must contain at least one source task ID.", nameof(mergeTasksRequest.SourceTaskIds));
            }

            CuTask? lastMergedTargetTask = null;

            // Iterate through each source task and merge it into the target task.
            // The ClickUp API endpoint for merging is POST /task/{source_task_id}/merge
            // with a body like { "target_task_id": "the_actual_target_task_id" }.
            // The response is the updated target task.
            // API documentation for this endpoint states "Custom Task IDs are not supported".
            foreach (var sourceTaskId in mergeTasksRequest.SourceTaskIds)
            {
                if (string.IsNullOrWhiteSpace(sourceTaskId))
                {
                    _logger.LogWarning("Skipping empty or whitespace source task ID during merge operation into target {TargetTaskId}.", targetTaskId);
                    continue;
                }

                var endpoint = $"task/{sourceTaskId}/merge";
                // Query parameters for customTaskIds and teamId are removed as they are not supported by this API endpoint.

                var payload = new { target_task_id = targetTaskId };

                _logger.LogDebug("Attempting to merge source task {SourceTaskId} into target task {TargetTaskId} via endpoint {Endpoint}", sourceTaskId, targetTaskId, endpoint);
                var updatedTargetTask = await _apiConnection.PostAsync<object, CuTask>(endpoint, payload, cancellationToken);

                if (updatedTargetTask == null)
                {
                    // If any merge operation fails to return the task, consider it an issue.
                    _logger.LogError("API connection returned null response when merging source task '{SourceTaskId}' into target task '{TargetTaskId}'.", sourceTaskId, targetTaskId);
                    throw new InvalidOperationException($"API connection returned null response when merging source task '{sourceTaskId}' into target task '{targetTaskId}'.");
                }
                lastMergedTargetTask = updatedTargetTask; // Keep track of the latest state of the target task.
                _logger.LogInformation("Successfully merged source task {SourceTaskId} into target task {TargetTaskId}. Updated target task ID: {UpdatedTargetTaskId}", sourceTaskId, targetTaskId, updatedTargetTask.Id);
            }

            if (lastMergedTargetTask == null)
            {
                // This would happen if SourceTaskIds was empty or contained only invalid (empty/whitespace) IDs,
                // and no merge operations were attempted or all skipped.
                _logger.LogWarning("No merge operations were successfully performed for target task '{TargetTaskId}'. This could be due to an empty or invalid list of source task IDs.", targetTaskId);
                // Depending on strictness, we might throw or return a fetched version of targetTaskId.
                // For now, if no merge actually happened and returned a task, it's an issue.
                throw new InvalidOperationException($"No merge operations were successfully performed for target task '{targetTaskId}', or the target task could not be retrieved after attempting merges. Ensure SourceTaskIds are valid.");
            }

            return lastMergedTargetTask;
        }

        /// <inheritdoc />
        public async Task<TaskTimeInStatusResponse> GetTaskTimeInStatusAsync(
            string taskId,
            GetTaskTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting task time in status for task ID: {TaskId}", taskId);
            var endpoint = $"task/{taskId}/time_in_status";
            var queryParamsList = new List<KeyValuePair<string, string>>();
            if (requestModel.CustomTaskIds.HasValue) queryParamsList.Add(new KeyValuePair<string, string>("custom_task_ids", requestModel.CustomTaskIds.Value.ToString().ToLower()));
            if (!string.IsNullOrEmpty(requestModel.TeamId)) queryParamsList.Add(new KeyValuePair<string, string>("team_id", requestModel.TeamId));
            var fullEndpoint = endpoint + UrlBuilderHelper.BuildQueryString(queryParamsList);

            var response = await _apiConnection.GetAsync<TaskTimeInStatusResponse>(fullEndpoint, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting task time in status for task {taskId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<GetBulkTasksTimeInStatusResponse> GetBulkTasksTimeInStatusAsync(
            GetBulkTasksTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting bulk tasks time in status for task IDs: {TaskIdsCount}", requestModel.TaskIds?.Count());
            var endpoint = $"task/bulk_time_in_status/task_ids"; // Path doesn't take task_ids directly
            var queryParamsList = new List<KeyValuePair<string, string>>();
            if (requestModel.TaskIds == null || !requestModel.TaskIds.Any())
            {
                throw new ArgumentException("Task IDs collection cannot be null or empty.", nameof(requestModel.TaskIds));
            }
            // The API for this endpoint expects task_ids as a comma-separated string.
            queryParamsList.Add(new KeyValuePair<string, string>("task_ids", string.Join(",", requestModel.TaskIds)));
            if (requestModel.CustomTaskIds.HasValue) queryParamsList.Add(new KeyValuePair<string, string>("custom_task_ids", requestModel.CustomTaskIds.Value.ToString().ToLower()));
            if (!string.IsNullOrEmpty(requestModel.TeamId)) queryParamsList.Add(new KeyValuePair<string, string>("team_id", requestModel.TeamId));
            var fullEndpoint = endpoint + UrlBuilderHelper.BuildQueryString(queryParamsList);

            var response = await _apiConnection.GetAsync<GetBulkTasksTimeInStatusResponse>(fullEndpoint, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting bulk tasks time in status.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<CuTask> CreateTaskFromTemplateAsync(
            string listId,
            string templateId,
            CreateTaskFromTemplateRequest createTaskFromTemplateRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating task in list ID: {ListId} from template ID: {TemplateId}", listId, templateId);
            var endpoint = $"list/{listId}/taskTemplate/{templateId}";
            // Query parameters customTaskIds and teamId are removed as they are not used for this endpoint.
            // var queryParams = new Dictionary<string, string?>();
            // endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

            var task = await _apiConnection.PostAsync<CreateTaskFromTemplateRequest, CuTask>(endpoint, createTaskFromTemplateRequest, cancellationToken);
            if (task == null)
            {
                throw new InvalidOperationException($"API connection returned null response when creating task from template {templateId} in list {listId}.");
            }
            return task;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<CuTask> GetTasksAsyncEnumerableAsync(
            string listId,
            GetTasksRequestParameters parameters,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(listId)) throw new ArgumentNullException(nameof(listId));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            _logger.LogInformation("Getting tasks as an async enumerable for list ID: {ListId}, Parameters: {@Parameters}", listId, parameters);

            int currentPage = parameters.Page ?? 0;
            bool lastPage;

            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Fetching page {PageNumber} for tasks in list ID {ListId} via async enumerable.", currentPage, listId);

                // Temporarily set the page for this iteration without modifying the original parameters
                var originalPage = parameters.Page;
                parameters.Page = currentPage;
                
                try
                {
                    var queryParamsList = parameters.ToQueryParametersList();
                    var fullEndpoint = $"list/{listId}/task" + UrlBuilderHelper.BuildQueryString(queryParamsList);
                    var response = await _apiConnection.GetAsync<GetTasksResponse>(fullEndpoint, cancellationToken).ConfigureAwait(false);

                    if (response?.Tasks != null && response.Tasks.Any())
                    {
                        foreach (var task in response.Tasks)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
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
            _logger.LogInformation("Finished streaming tasks for list ID: {ListId}", listId);
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
            var endpoint = $"team/{workspaceId}/task";

            var queryParamsList = parameters.ToQueryParametersList();
            var fullEndpoint = endpoint + UrlBuilderHelper.BuildQueryString(queryParamsList);
            var response = await _apiConnection.GetAsync<GetTasksResponse>(fullEndpoint, cancellationToken);

            if (response == null)
            {
                _logger.LogWarning("API connection returned null response when getting filtered team tasks for workspace {WorkspaceId}. Returning empty paged result.", workspaceId);
                return PagedResult<CuTask>.Empty(currentPage);
            }

            var items = response.Tasks ?? Enumerable.Empty<CuTask>();
            return new PagedResult<CuTask>(
                items,
                currentPage,
                items.Count(),
                response.LastPage == false
            );
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<CuTask> GetFilteredTeamTasksAsyncEnumerableAsync(
            string workspaceId,
            GetTasksRequestParameters parameters,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workspaceId)) throw new ArgumentNullException(nameof(workspaceId));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            _logger.LogInformation("Getting filtered team tasks as an async enumerable for workspace ID: {WorkspaceId}, Parameters: {@Parameters}", workspaceId, parameters);

            int currentPage = parameters.Page ?? 0;
            bool lastPage;

            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Fetching page {PageNumber} for filtered team tasks in workspace ID {WorkspaceId} via async enumerable.", currentPage, workspaceId);

                // Temporarily set the page for this iteration without modifying the original parameters
                var originalPage = parameters.Page;
                parameters.Page = currentPage;

                try
                {
                    var queryParamsList = parameters.ToQueryParametersList();
                    var fullEndpoint = $"team/{workspaceId}/task" + UrlBuilderHelper.BuildQueryString(queryParamsList);
                    var response = await _apiConnection.GetAsync<GetTasksResponse>(fullEndpoint, cancellationToken).ConfigureAwait(false);

                    if (response?.Tasks != null && response.Tasks.Any())
                    {
                        foreach (var task in response.Tasks)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
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
            _logger.LogInformation("Finished streaming filtered team tasks for workspace ID: {WorkspaceId}", workspaceId);
        }
    }
}
