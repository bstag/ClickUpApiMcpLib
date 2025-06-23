using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClickUp.Api.Client.Models.Entities.Tasks; // CuTask DTO
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;

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

        private string BuildQueryString(Dictionary<string, string?> queryParams)
        {
            if (queryParams == null || !queryParams.Any(kvp => kvp.Value != null))
            {
                return string.Empty;
            }

            var sb = new StringBuilder("?");
            foreach (var kvp in queryParams)
            {
                if (kvp.Value != null)
                {
                    if (sb.Length > 1) sb.Append('&');
                    sb.Append($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
                }
            }
            return sb.ToString();
        }

        private string BuildQueryStringFromArray<T>(string key, IEnumerable<T>? values)
        {
            if (values == null || !values.Any()) return string.Empty;
            // Format: key[]=value1&key[]=value2
            return string.Join("&", values.Select(v => $"{Uri.EscapeDataString(key)}[]={Uri.EscapeDataString(v?.ToString() ?? string.Empty)}"));
        }


        /// <inheritdoc />
        public async Task<GetTasksResponse> GetTasksAsync(
            string listId,
            bool? archived = null,
            bool? includeMarkdownDescription = null,
            int? page = null,
            string? orderBy = null,
            bool? reverse = null,
            bool? subtasks = null,
            IEnumerable<string>? statuses = null,
            bool? includeClosed = null,
            IEnumerable<string>? assignees = null,
            IEnumerable<string>? watchers = null,
            IEnumerable<string>? tags = null,
            long? dueDateGreaterThan = null,
            long? dueDateLessThan = null,
            long? dateCreatedGreaterThan = null,
            long? dateCreatedLessThan = null,
            long? dateUpdatedGreaterThan = null,
            long? dateUpdatedLessThan = null,
            long? dateDoneGreaterThan = null,
            long? dateDoneLessThan = null,
            string? customFields = null,
            IEnumerable<long>? customItems = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting tasks for list ID: {ListId}, Page: {Page}", listId, page);
            var endpoint = $"list/{listId}/task";
            var queryParams = new Dictionary<string, string?>();
            if (archived.HasValue) queryParams["archived"] = archived.Value.ToString().ToLower();
            if (includeMarkdownDescription.HasValue) queryParams["include_markdown_description"] = includeMarkdownDescription.Value.ToString().ToLower();
            if (page.HasValue) queryParams["page"] = page.Value.ToString();
            if (!string.IsNullOrEmpty(orderBy)) queryParams["order_by"] = orderBy;
            if (reverse.HasValue) queryParams["reverse"] = reverse.Value.ToString().ToLower();
            if (subtasks.HasValue) queryParams["subtasks"] = subtasks.Value.ToString().ToLower();
            if (includeClosed.HasValue) queryParams["include_closed"] = includeClosed.Value.ToString().ToLower();
            if (dueDateGreaterThan.HasValue) queryParams["due_date_gt"] = dueDateGreaterThan.Value.ToString();
            if (dueDateLessThan.HasValue) queryParams["due_date_lt"] = dueDateLessThan.Value.ToString();
            if (dateCreatedGreaterThan.HasValue) queryParams["date_created_gt"] = dateCreatedGreaterThan.Value.ToString();
            if (dateCreatedLessThan.HasValue) queryParams["date_created_lt"] = dateCreatedLessThan.Value.ToString();
            if (dateUpdatedGreaterThan.HasValue) queryParams["date_updated_gt"] = dateUpdatedGreaterThan.Value.ToString();
            if (dateUpdatedLessThan.HasValue) queryParams["date_updated_lt"] = dateUpdatedLessThan.Value.ToString();
            if (dateDoneGreaterThan.HasValue) queryParams["date_done_gt"] = dateDoneGreaterThan.Value.ToString();
            if (dateDoneLessThan.HasValue) queryParams["date_done_lt"] = dateDoneLessThan.Value.ToString();
            if (!string.IsNullOrEmpty(customFields)) queryParams["custom_fields"] = customFields;

            var queryString = BuildQueryString(queryParams);

            // Handle array parameters separately as they have a specific format (e.g., statuses[]=...&statuses[]=...)
            var arrayParams = new List<string>();
            if (statuses != null && statuses.Any()) arrayParams.Add(BuildQueryStringFromArray("statuses", statuses));
            if (assignees != null && assignees.Any()) arrayParams.Add(BuildQueryStringFromArray("assignees", assignees));
            if (watchers != null && watchers.Any()) arrayParams.Add(BuildQueryStringFromArray("watchers", watchers));
            if (tags != null && tags.Any()) arrayParams.Add(BuildQueryStringFromArray("tags", tags));
            if (customItems != null && customItems.Any()) arrayParams.Add(BuildQueryStringFromArray("custom_items", customItems.Select(ci => ci.ToString())));

            if (arrayParams.Any(p => !string.IsNullOrEmpty(p)))
            {
                queryString += (string.IsNullOrEmpty(queryString) || queryString == "?" ? (queryString == "?" ? "" : "?") : "&") + string.Join("&", arrayParams.Where(p => !string.IsNullOrEmpty(p)));
            }
            if (queryString == "?") queryString = string.Empty;

            var response = await _apiConnection.GetAsync<GetTasksResponse>($"{endpoint}{queryString}", cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting tasks for list {listId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<CuTask> CreateTaskAsync(
            string listId,
            CreateTaskRequest createTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating task in list ID: {ListId} with name: {TaskName}", listId, createTaskRequest.Name);
            var endpoint = $"list/{listId}/task";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

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
            bool? customTaskIds = null,
            string? teamId = null,
            bool? includeSubtasks = null,
            bool? includeMarkdownDescription = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting task with ID: {TaskId}", taskId);
            var endpoint = $"task/{taskId}";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            if (includeSubtasks.HasValue) queryParams["include_subtasks"] = includeSubtasks.Value.ToString().ToLower();
            if (includeMarkdownDescription.HasValue) queryParams["include_markdown_description"] = includeMarkdownDescription.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var task = await _apiConnection.GetAsync<CuTask>(endpoint, cancellationToken);
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
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

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
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting task with ID: {TaskId}", taskId);
            var endpoint = $"task/{taskId}";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

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
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting task time in status for task ID: {TaskId}", taskId);
            var endpoint = $"task/{taskId}/time_in_status";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<TaskTimeInStatusResponse>(endpoint, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting task time in status for task {taskId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<GetBulkTasksTimeInStatusResponse> GetBulkTasksTimeInStatusAsync(
            IEnumerable<string> taskIds,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting bulk tasks time in status for task IDs: {TaskIdsCount}", taskIds?.Count());
            var endpoint = $"task/bulk_time_in_status/task_ids"; // Path doesn't take task_ids directly
            var queryParams = new Dictionary<string, string?>();
            if (taskIds == null || !taskIds.Any())
            {
                throw new ArgumentException("Task IDs collection cannot be null or empty.", nameof(taskIds));
            }
            queryParams["task_ids"] = string.Join(",", taskIds); // Comma-separated list
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetBulkTasksTimeInStatusResponse>(endpoint, cancellationToken);
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
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating task in list ID: {ListId} from template ID: {TemplateId}", listId, templateId);
            var endpoint = $"list/{listId}/taskTemplate/{templateId}";
            var queryParams = new Dictionary<string, string?>();
            // Note: ClickUp API for this endpoint doesn't explicitly list custom_task_ids or team_id as query params.
            // Assuming they might be applicable to the created task itself if passed in body, or not supported here.
            // For now, not adding them to query string unless API spec confirms.
            // If they apply to the list or template context, that's different.
            // The `customTaskIds` and `teamId` parameters in the method signature might be a slight mismatch for this specific endpoint's query params.
            // However, the `createTaskFromTemplateRequest` DTO is the main payload.
            endpoint += BuildQueryString(queryParams);

            var task = await _apiConnection.PostAsync<CreateTaskFromTemplateRequest, CuTask>(endpoint, createTaskFromTemplateRequest, cancellationToken);
            if (task == null)
            {
                throw new InvalidOperationException($"API connection returned null response when creating task from template {templateId} in list {listId}.");
            }
            return task;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Models.Entities.Tasks.CuTask> GetTasksAsyncEnumerableAsync(
            string listId,
            bool? archived = null,
            bool? includeMarkdownDescription = null,
            string? orderBy = null,
            bool? reverse = null,
            bool? subtasks = null,
            IEnumerable<string>? statuses = null,
            bool? includeClosed = null,
            IEnumerable<string>? assignees = null,
            IEnumerable<string>? watchers = null,
            IEnumerable<string>? tags = null,
            long? dueDateGreaterThan = null,
            long? dueDateLessThan = null,
            long? dateCreatedGreaterThan = null,
            long? dateCreatedLessThan = null,
            long? dateUpdatedGreaterThan = null,
            long? dateUpdatedLessThan = null,
            long? dateDoneGreaterThan = null,
            long? dateDoneLessThan = null,
            string? customFields = null,
            IEnumerable<long>? customItems = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting tasks as an async enumerable for list ID: {ListId}", listId);
            int currentPage = 0;
            bool lastPage;

            do
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Cancellation requested while getting tasks for list ID {ListId} via async enumerable.", listId);
                    yield break;
                }

                _logger.LogDebug("Fetching page {PageNumber} for tasks in list ID {ListId} via async enumerable.", currentPage, listId);
                var response = await GetTasksAsync( // Call the existing paged method
                    listId: listId,
                    archived: archived,
                    includeMarkdownDescription: includeMarkdownDescription,
                    page: currentPage, // Current page for this iteration
                    orderBy: orderBy,
                    reverse: reverse,
                    subtasks: subtasks,
                    statuses: statuses,
                    includeClosed: includeClosed,
                    assignees: assignees,
                    watchers: watchers,
                    tags: tags,
                    dueDateGreaterThan: dueDateGreaterThan,
                    dueDateLessThan: dueDateLessThan,
                    dateCreatedGreaterThan: dateCreatedGreaterThan,
                    dateCreatedLessThan: dateCreatedLessThan,
                    dateUpdatedGreaterThan: dateUpdatedGreaterThan,
                    dateUpdatedLessThan: dateUpdatedLessThan,
                    dateDoneGreaterThan: dateDoneGreaterThan,
                    dateDoneLessThan: dateDoneLessThan,
                    customFields: customFields,
                    customItems: customItems,
                    cancellationToken: cancellationToken
                ).ConfigureAwait(false);

                if (response?.Tasks != null && response.Tasks.Any())
                {
                    foreach (var task in response.Tasks)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            yield break;
                        }
                        yield return task;
                    }
                    // If response.LastPage is true, it's the last page.
                    // If response.LastPage is false or null, it's not the last page.
                    lastPage = response.LastPage == true;
                }
                else
                {
                    lastPage = true; // No response or no tasks implies last page or an issue
                }

                if (!lastPage)
                {
                    currentPage++;
                }
            } while (!lastPage);
        }

        /// <inheritdoc />
        public async Task<GetTasksResponse> GetFilteredTeamTasksAsync(
            string workspaceId,
            int? page = null,
            string? orderBy = null,
            bool? reverse = null,
            bool? subtasks = null,
            IEnumerable<string>? spaceIds = null,
            IEnumerable<string>? projectIds = null,
            IEnumerable<string>? listIds = null,
            IEnumerable<string>? statuses = null,
            bool? includeClosed = null,
            IEnumerable<string>? assignees = null,
            IEnumerable<string>? tags = null,
            long? dueDateGreaterThan = null,
            long? dueDateLessThan = null,
            long? dateCreatedGreaterThan = null,
            long? dateCreatedLessThan = null,
            long? dateUpdatedGreaterThan = null,
            long? dateUpdatedLessThan = null,
            string? customFields = null,
            bool? customTaskIds = null, // Name in interface
            string? teamIdForCustomTaskIds = null, // Name in interface
            IEnumerable<long>? customItems = null,
            long? dateDoneGreaterThan = null,
            long? dateDoneLessThan = null,
            string? parentTaskId = null,
            bool? includeMarkdownDescription = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting filtered team tasks for workspace ID: {WorkspaceId}, Page: {Page}", workspaceId, page);
            if (string.IsNullOrWhiteSpace(workspaceId))
            {
                throw new ArgumentNullException(nameof(workspaceId));
            }

            var endpoint = $"team/{workspaceId}/task";
            var queryParams = new Dictionary<string, string?>();

            if (page.HasValue) queryParams["page"] = page.Value.ToString();
            if (!string.IsNullOrEmpty(orderBy)) queryParams["order_by"] = orderBy;
            if (reverse.HasValue) queryParams["reverse"] = reverse.Value.ToString().ToLower();
            if (subtasks.HasValue) queryParams["subtasks"] = subtasks.Value.ToString().ToLower();
            if (includeClosed.HasValue) queryParams["include_closed"] = includeClosed.Value.ToString().ToLower();

            if (dueDateGreaterThan.HasValue) queryParams["due_date_gt"] = dueDateGreaterThan.Value.ToString();
            if (dueDateLessThan.HasValue) queryParams["due_date_lt"] = dueDateLessThan.Value.ToString();
            if (dateCreatedGreaterThan.HasValue) queryParams["date_created_gt"] = dateCreatedGreaterThan.Value.ToString();
            if (dateCreatedLessThan.HasValue) queryParams["date_created_lt"] = dateCreatedLessThan.Value.ToString();
            if (dateUpdatedGreaterThan.HasValue) queryParams["date_updated_gt"] = dateUpdatedGreaterThan.Value.ToString();
            if (dateUpdatedLessThan.HasValue) queryParams["date_updated_lt"] = dateUpdatedLessThan.Value.ToString();
            if (dateDoneGreaterThan.HasValue) queryParams["date_done_gt"] = dateDoneGreaterThan.Value.ToString();
            if (dateDoneLessThan.HasValue) queryParams["date_done_lt"] = dateDoneLessThan.Value.ToString();
            if (!string.IsNullOrEmpty(parentTaskId)) queryParams["parent"] = parentTaskId;
            if (includeMarkdownDescription.HasValue) queryParams["include_markdown_description"] = includeMarkdownDescription.Value.ToString().ToLower();

            if (!string.IsNullOrEmpty(customFields)) queryParams["custom_fields"] = customFields;
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamIdForCustomTaskIds)) queryParams["team_id"] = teamIdForCustomTaskIds;

            var queryString = BuildQueryString(queryParams);

            var arrayParams = new List<string>();
            if (spaceIds != null && spaceIds.Any()) arrayParams.Add(BuildQueryStringFromArray("space_ids", spaceIds));
            if (projectIds != null && projectIds.Any()) arrayParams.Add(BuildQueryStringFromArray("project_ids", projectIds));
            if (listIds != null && listIds.Any()) arrayParams.Add(BuildQueryStringFromArray("list_ids", listIds));
            if (statuses != null && statuses.Any()) arrayParams.Add(BuildQueryStringFromArray("statuses", statuses));
            if (assignees != null && assignees.Any()) arrayParams.Add(BuildQueryStringFromArray("assignees", assignees));
            if (tags != null && tags.Any()) arrayParams.Add(BuildQueryStringFromArray("tags", tags));
            if (customItems != null && customItems.Any()) arrayParams.Add(BuildQueryStringFromArray("custom_items", customItems.Select(ci => ci.ToString())));

            if (arrayParams.Any(p => !string.IsNullOrEmpty(p)))
            {
                queryString += (string.IsNullOrEmpty(queryString) || queryString == "?" ? (queryString == "?" ? "" : "?") : "&") + string.Join("&", arrayParams.Where(p => !string.IsNullOrEmpty(p)));
            }
            if (queryString == "?") queryString = string.Empty;

            var response = await _apiConnection.GetAsync<GetTasksResponse>($"{endpoint}{queryString}", cancellationToken);
            if (response == null)
            {
                // Consider a more specific message or logging
                throw new InvalidOperationException($"API connection returned null response when getting filtered team tasks for workspace {workspaceId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<CuTask> GetFilteredTeamTasksAsyncEnumerableAsync(
            string workspaceId,
            string? orderBy = null,
            bool? reverse = null,
            bool? subtasks = null,
            IEnumerable<string>? spaceIds = null,
            IEnumerable<string>? projectIds = null,
            IEnumerable<string>? listIds = null,
            IEnumerable<string>? statuses = null,
            bool? includeClosed = null,
            IEnumerable<string>? assignees = null,
            IEnumerable<string>? tags = null,
            long? dueDateGreaterThan = null,
            long? dueDateLessThan = null,
            long? dateCreatedGreaterThan = null,
            long? dateCreatedLessThan = null,
            long? dateUpdatedGreaterThan = null,
            long? dateUpdatedLessThan = null,
            // Removed duplicate dateDoneGreaterThan and dateDoneLessThan
            string? customFields = null,
            bool? queryCustomTaskIds = null,
            string? teamIdForQueryCustomTaskIds = null,
            IEnumerable<long>? customItems = null,
            long? dateDoneGreaterThan = null, // Kept one set of these
            long? dateDoneLessThan = null,
            string? parentTaskId = null,
            bool? includeMarkdownDescription = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting filtered team tasks as an async enumerable for workspace ID: {WorkspaceId}", workspaceId);
            if (string.IsNullOrWhiteSpace(workspaceId))
            {
                throw new ArgumentNullException(nameof(workspaceId));
            }

            int currentPage = 0;
            bool lastPage;

            do
            {
                cancellationToken.ThrowIfCancellationRequested(); // Check before API call

                _logger.LogDebug("Fetching page {PageNumber} for filtered team tasks in workspace ID {WorkspaceId} via async enumerable.", currentPage, workspaceId);
                var response = await GetFilteredTeamTasksAsync(
                    workspaceId: workspaceId,
                    page: currentPage,
                    orderBy: orderBy,
                    reverse: reverse,
                    subtasks: subtasks,
                    spaceIds: spaceIds,
                    projectIds: projectIds,
                    listIds: listIds,
                    statuses: statuses,
                    includeClosed: includeClosed,
                    assignees: assignees,
                    tags: tags,
                    dueDateGreaterThan: dueDateGreaterThan,
                    dueDateLessThan: dueDateLessThan,
                    dateCreatedGreaterThan: dateCreatedGreaterThan,
                    dateCreatedLessThan: dateCreatedLessThan,
                    dateUpdatedGreaterThan: dateUpdatedGreaterThan,
                    dateUpdatedLessThan: dateUpdatedLessThan,
                    customFields: customFields,
                    customTaskIds: queryCustomTaskIds, // Ensure mapping to the correct parameter name
                    teamIdForCustomTaskIds: teamIdForQueryCustomTaskIds, // Ensure mapping
                    customItems: customItems,
                    dateDoneGreaterThan: dateDoneGreaterThan,
                    dateDoneLessThan: dateDoneLessThan,
                    parentTaskId: parentTaskId,
                    includeMarkdownDescription: includeMarkdownDescription,
                    cancellationToken: cancellationToken
                ).ConfigureAwait(false);

                if (response?.Tasks != null && response.Tasks.Any())
                {
                    foreach (var task in response.Tasks)
                    {
                        cancellationToken.ThrowIfCancellationRequested(); // Check before yielding each item
                        yield return task;
                    }
                    lastPage = response.LastPage == true;
                }
                else
                {
                    lastPage = true;
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
