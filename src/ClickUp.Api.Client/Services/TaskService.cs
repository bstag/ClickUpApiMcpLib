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
            bool? queryCustomTaskIds = null,
            string? teamIdForQueryCustomTaskIds = null,
            CancellationToken cancellationToken = default)
        {
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
            if (!string.IsNullOrEmpty(customFields)) queryParams["custom_fields"] = customFields;
            if (queryCustomTaskIds.HasValue) queryParams["custom_task_ids"] = queryCustomTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamIdForQueryCustomTaskIds)) queryParams["team_id"] = teamIdForQueryCustomTaskIds;

            var queryString = BuildQueryString(queryParams);

            var arrayParams = new List<string>();
            if (spaceIds != null && spaceIds.Any()) arrayParams.Add(BuildQueryStringFromArray("space_ids", spaceIds));
            if (projectIds != null && projectIds.Any()) arrayParams.Add(BuildQueryStringFromArray("project_ids", projectIds));
            if (listIds != null && listIds.Any()) arrayParams.Add(BuildQueryStringFromArray("list_ids", listIds));
            if (statuses != null && statuses.Any()) arrayParams.Add(BuildQueryStringFromArray("statuses", statuses));
            if (assignees != null && assignees.Any()) arrayParams.Add(BuildQueryStringFromArray("assignees", assignees));
            if (tags != null && tags.Any()) arrayParams.Add(BuildQueryStringFromArray("tags", tags));

            if (arrayParams.Any(p => !string.IsNullOrEmpty(p)))
            {
                 queryString += (string.IsNullOrEmpty(queryString) || queryString == "?" ? (queryString == "?" ? "" : "?") : "&") + string.Join("&", arrayParams.Where(p => !string.IsNullOrEmpty(p)));
            }
            if (queryString == "?") queryString = string.Empty;

            var response = await _apiConnection.GetAsync<GetTasksResponse>($"{endpoint}{queryString}", cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting filtered team tasks for workspace {workspaceId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<CuTask> MergeTasksAsync(
            string targetTaskId,
            MergeTasksRequest mergeTasksRequest,
            bool? customTaskIds = null, // Applies to source_task_ids and targetTaskId if used as query params
            string? teamId = null,      // Applies to source_task_ids and targetTaskId if used as query params
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Merging tasks into target task ID: {TargetTaskId}", targetTaskId);
            // This overload maps to POST /v2/task/{task_id}/merge where task_id is the targetTaskId param
            // and mergeTasksRequest.TargetTaskId is the actual target.
            // The API is POST /task/{task_id}/merge with body { "target_task_id": "string" }
            // The 'task_id' in the path is the source task.
            // The prompt's naming of 'targetTaskId' for the path and MergeTasksRequest for body is a bit confusing.
            // Let's assume targetTaskId is the source task_id for the path, and MergeTasksRequest contains the real target.
            // This would make mergeTasksRequest.SourceTaskId the path {task_id} and targetTaskId the body.
            // Re-interpreting based on common API patterns:
            // POST /task/{source_task_id}/merge with body { "target_task_id": "string" }
            // If targetTaskId is the one in the path, then mergeTasksRequest should contain the target.
            // The interface is: MergeTasksAsync(string targetTaskId, MergeTasksRequest mergeTasksRequest, ...)
            // Let's assume 'targetTaskId' in method signature is the SOURCE task to be merged from.
            // And 'mergeTasksRequest' contains the actual target task ID.
            // This means the endpoint should be /task/{targetTaskId}/merge (where targetTaskId is source)
            // And the body mergeTasksRequest contains { "target_task_id": "actual_target_id" }

            // The interface has `string targetTaskId` as the first param, and `MergeTasksRequest`
            // Let's assume the `targetTaskId` parameter is the `task_id` in the path (the task being merged FROM)
            // and `MergeTasksRequest` contains the actual target.
            // So endpoint is /task/{targetTaskId}/merge
            // Body is mergeTasksRequest (which should contain the *actual* target task id)
            // This is confusing. The prompt for ITasksService was:
            // MergeTasksAsync: Request MergeTasksRequest, response CuTask.
            // Parameters: string targetTaskId, MergeTasksRequest mergeTasksRequest
            // This implies `targetTaskId` is the task all sources from `mergeTasksRequest` are merged INTO.
            // If so, the API endpoint POST /v2/task/{task_id}/merge is problematic because task_id in path is source.
            // Let's assume the intent is that `targetTaskId` is the destination, and `mergeTasksRequest.TaskIds` are sources.
            // This doesn't map directly to a single ClickUp v2 endpoint easily.
            // ClickUp's "Merge Tasks" endpoint is POST /v2/task/{task_id}/merge where {task_id} is the source, and body contains target.
            // ClickUp's "Merge CuTask Into" endpoint is POST /v2/task/{task_id}/merge_into/{target_task_id}
            //
            // Given the interface: CuTask<CuTask> MergeTasksAsync(string targetTaskId, MergeTasksRequest mergeTasksRequest, ...)
            // The most logical mapping is that `targetTaskId` is the task where other tasks (specified in `mergeTasksRequest.TaskIds`) are merged INTO.
            // This would require multiple calls to POST /task/{source_task_id}/merge_into/{target_task_id} or similar.
            // Or, if there's a bulk merge endpoint, that would be it.
            //
            // Reverting to the refined interface's intent:
            // "Merges tasks into a target task."
            // - param name="targetTaskId">ID of the target task that other tasks will be merged into.
            // - param name="mergeTasksRequest">Contains the list of source task IDs to merge.
            // This implies `mergeTasksRequest.TaskIds` are merged into `targetTaskId`.
            // This likely requires multiple API calls, one for each task in `mergeTasksRequest.TaskIds`.
            // For a single source task in `mergeTasksRequest.TaskIds[0]`: POST /task/{mergeTasksRequest.TaskIds[0]}/merge_into/{targetTaskId}
            // The API call POST /task/{task_id}/merge with body { "target_task_id": "string" } can be used.
            // Here, {task_id} is the source, and target_task_id in body is destination.
            // So, for each task_id in mergeTasksRequest.TaskIds, we'd call:
            // POST /task/{task_id_from_request}/merge with body { "target_task_id": targetTaskId (from method param) }
            // This is complex for a single method stub. For now, I'll assume it's merging ONE task specified in MergeTasksRequest into targetTaskId.
            // Let's assume MergeTasksRequest has a SINGLE SourceTaskId.
            // Endpoint: task/{mergeTasksRequest.SourceTaskId}/merge_into/{targetTaskId} (if this is what MergeTasksRequest implies)
            // OR task/{mergeTasksRequest.SourceTaskId}/merge (with body {"target_task_id": targetTaskId})

            // For simplicity of stubbing, I'll assume mergeTasksRequest has one source task ID and targetTaskId is the destination.
            // This is a common pattern: POST /task/{source_id}/merge  Body: { "target_task_id": "dest_id" }
            // Let's say mergeTasksRequest contains the source_id, and targetTaskId is the destination.
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
            // The API documentation for "Merge Tasks" (POST /task/{task_id}/merge)
            // states that {task_id} in the path is the source task ID,
            // and the body contains { "target_task_id": "string" }.
            // The response is the updated target task.
            foreach (var sourceTaskId in mergeTasksRequest.SourceTaskIds)
            {
                if (string.IsNullOrWhiteSpace(sourceTaskId))
                {
                    // Optionally, log this or decide if the whole operation should fail.
                    // For now, skipping invalid source task IDs.
                    continue;
                }

                var endpoint = $"task/{sourceTaskId}/merge";
                var queryParams = new Dictionary<string, string?>();
                if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
                if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;

                string fullEndpoint = endpoint + BuildQueryString(queryParams);

                var payload = new { target_task_id = targetTaskId };

                // The API returns the updated TARGET task after a successful merge.
                var updatedTargetTask = await _apiConnection.PostAsync<object, CuTask>(fullEndpoint, payload, cancellationToken);

                if (updatedTargetTask == null)
                {
                    // If any merge operation fails to return the task, consider it an issue.
                    // Depending on desired atomicity, one might choose to throw here or collect errors.
                    // For now, throwing an exception indicating which merge failed.
                    throw new InvalidOperationException($"API connection returned null response when merging source task '{sourceTaskId}' into target task '{targetTaskId}'.");
                }
                lastMergedTargetTask = updatedTargetTask; // Keep track of the latest state of the target task.
            }

            if (lastMergedTargetTask == null)
            {
                // This would happen if SourceTaskIds was empty or contained only invalid IDs,
                // and no merge operations were attempted or all skipped.
                // Or if the loop completed but no task was returned from the last operation (should be caught above).
                // It might be better to fetch the target task if no merges happened but that adds complexity.
                // For now, if no merge was successfully performed and returned a task, throw.
                throw new InvalidOperationException($"No merge operations were successfully performed, or the target task '{targetTaskId}' could not be retrieved after merging.");
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
    }
}
