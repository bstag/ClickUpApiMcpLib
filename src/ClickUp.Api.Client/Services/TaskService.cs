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
using ClickUp.Api.Client.Models.Common.Pagination; // For IPagedResult

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
        public async Task<IPagedResult<CuTask>> GetTasksAsync(
            string listId,
            GetTasksRequest requestModel,
            int? page = null, // Added to match interface, but requestModel.Page is primary
            CancellationToken cancellationToken = default)
        {
            // Prioritize page from requestModel if available, else use the direct parameter.
            // The interface change added 'page' for flexibility, but DTO should be source of truth.
            if (page.HasValue && !requestModel.Page.HasValue)
            {
                requestModel.Page = page.Value;
            }
            int currentPage = requestModel.Page ?? 0; // Default to page 0 if not set

            _logger.LogInformation("Getting tasks for list ID: {ListId}, Page: {Page}", listId, currentPage);
            var endpoint = $"list/{listId}/task";
            var queryParams = new Dictionary<string, string?>();

            if (requestModel.Archived.HasValue) queryParams["archived"] = requestModel.Archived.Value.ToString().ToLower();
            if (requestModel.IncludeMarkdownDescription.HasValue) queryParams["include_markdown_description"] = requestModel.IncludeMarkdownDescription.Value.ToString().ToLower();

            queryParams["page"] = currentPage.ToString(); // Always include page

            if (!string.IsNullOrEmpty(requestModel.OrderBy)) queryParams["order_by"] = requestModel.OrderBy;
            if (requestModel.Reverse.HasValue) queryParams["reverse"] = requestModel.Reverse.Value.ToString().ToLower();
            if (requestModel.Subtasks.HasValue) queryParams["subtasks"] = requestModel.Subtasks.Value.ToString().ToLower();
            if (requestModel.IncludeClosed.HasValue) queryParams["include_closed"] = requestModel.IncludeClosed.Value.ToString().ToLower();
            if (requestModel.DueDateGreaterThan.HasValue) queryParams["due_date_gt"] = requestModel.DueDateGreaterThan.Value.ToString();
            if (requestModel.DueDateLessThan.HasValue) queryParams["due_date_lt"] = requestModel.DueDateLessThan.Value.ToString();
            if (requestModel.DateCreatedGreaterThan.HasValue) queryParams["date_created_gt"] = requestModel.DateCreatedGreaterThan.Value.ToString();
            if (requestModel.DateCreatedLessThan.HasValue) queryParams["date_created_lt"] = requestModel.DateCreatedLessThan.Value.ToString();
            if (requestModel.DateUpdatedGreaterThan.HasValue) queryParams["date_updated_gt"] = requestModel.DateUpdatedGreaterThan.Value.ToString();
            if (requestModel.DateUpdatedLessThan.HasValue) queryParams["date_updated_lt"] = requestModel.DateUpdatedLessThan.Value.ToString();
            if (requestModel.DateDoneGreaterThan.HasValue) queryParams["date_done_gt"] = requestModel.DateDoneGreaterThan.Value.ToString();
            if (requestModel.DateDoneLessThan.HasValue) queryParams["date_done_lt"] = requestModel.DateDoneLessThan.Value.ToString();
            if (!string.IsNullOrEmpty(requestModel.CustomFields)) queryParams["custom_fields"] = requestModel.CustomFields;

            var queryString = BuildQueryString(queryParams);

            var arrayParams = new List<string>();
            if (requestModel.Statuses != null && requestModel.Statuses.Any()) arrayParams.Add(BuildQueryStringFromArray("statuses", requestModel.Statuses));
            if (requestModel.Assignees != null && requestModel.Assignees.Any()) arrayParams.Add(BuildQueryStringFromArray("assignees", requestModel.Assignees));
            if (requestModel.Watchers != null && requestModel.Watchers.Any()) arrayParams.Add(BuildQueryStringFromArray("watchers", requestModel.Watchers));
            if (requestModel.Tags != null && requestModel.Tags.Any()) arrayParams.Add(BuildQueryStringFromArray("tags", requestModel.Tags));
            if (requestModel.CustomItems != null && requestModel.CustomItems.Any()) arrayParams.Add(BuildQueryStringFromArray("custom_items", requestModel.CustomItems.Select(ci => ci.ToString())));

            if (arrayParams.Any(p => !string.IsNullOrEmpty(p)))
            {
                queryString += (string.IsNullOrEmpty(queryString) || queryString == "?" ? (queryString == "?" ? "" : "?") : "&") + string.Join("&", arrayParams.Where(p => !string.IsNullOrEmpty(p)));
            }
            if (queryString == "?") queryString = string.Empty;

            var response = await _apiConnection.GetAsync<GetTasksResponse>($"{endpoint}{queryString}", cancellationToken);

            if (response == null)
            {
                _logger.LogWarning("API connection returned null response when getting tasks for list {ListId}. Returning empty paged result.", listId);
                return PagedResult<CuTask>.Empty(currentPage);
            }

            var items = response.Tasks ?? Enumerable.Empty<CuTask>();
            return new PagedResult<CuTask>(
                items,
                currentPage,
                items.Count(), // PageSize for this specific page
                response.LastPage == false // HasNextPage
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
            GetTaskRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting task with ID: {TaskId}", taskId);
            var endpoint = $"task/{taskId}";
            var queryParams = new Dictionary<string, string?>();
            if (requestModel.CustomTaskIds.HasValue) queryParams["custom_task_ids"] = requestModel.CustomTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(requestModel.TeamId)) queryParams["team_id"] = requestModel.TeamId;
            if (requestModel.IncludeSubtasks.HasValue) queryParams["include_subtasks"] = requestModel.IncludeSubtasks.Value.ToString().ToLower();
            if (requestModel.IncludeMarkdownDescription.HasValue) queryParams["include_markdown_description"] = requestModel.IncludeMarkdownDescription.Value.ToString().ToLower();
            if (requestModel.Page.HasValue) queryParams["page"] = requestModel.Page.Value.ToString();
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
            DeleteTaskRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting task with ID: {TaskId}", taskId);
            var endpoint = $"task/{taskId}";
            var queryParams = new Dictionary<string, string?>();
            if (requestModel.CustomTaskIds.HasValue) queryParams["custom_task_ids"] = requestModel.CustomTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(requestModel.TeamId)) queryParams["team_id"] = requestModel.TeamId;
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
            GetTaskTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting task time in status for task ID: {TaskId}", taskId);
            var endpoint = $"task/{taskId}/time_in_status";
            var queryParams = new Dictionary<string, string?>();
            if (requestModel.CustomTaskIds.HasValue) queryParams["custom_task_ids"] = requestModel.CustomTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(requestModel.TeamId)) queryParams["team_id"] = requestModel.TeamId;
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
            GetBulkTasksTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting bulk tasks time in status for task IDs: {TaskIdsCount}", requestModel.TaskIds?.Count());
            var endpoint = $"task/bulk_time_in_status/task_ids"; // Path doesn't take task_ids directly
            var queryParams = new Dictionary<string, string?>();
            if (requestModel.TaskIds == null || !requestModel.TaskIds.Any())
            {
                throw new ArgumentException("Task IDs collection cannot be null or empty.", nameof(requestModel.TaskIds));
            }
            queryParams["task_ids"] = string.Join(",", requestModel.TaskIds); // Comma-separated list
            if (requestModel.CustomTaskIds.HasValue) queryParams["custom_task_ids"] = requestModel.CustomTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(requestModel.TeamId)) queryParams["team_id"] = requestModel.TeamId;
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
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating task in list ID: {ListId} from template ID: {TemplateId}", listId, templateId);
            var endpoint = $"list/{listId}/taskTemplate/{templateId}";
            // Query parameters customTaskIds and teamId are removed as they are not used for this endpoint.
            // var queryParams = new Dictionary<string, string?>();
            // endpoint += BuildQueryString(queryParams);

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
            GetTasksRequest requestModel,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting tasks as an async enumerable for list ID: {ListId}", listId);

            // Clone the requestModel to safely modify the Page property for internal pagination
            var pagedRequestModel = new GetTasksRequest
            {
                Archived = requestModel.Archived,
                IncludeMarkdownDescription = requestModel.IncludeMarkdownDescription,
                // Page will be set in the loop
                OrderBy = requestModel.OrderBy,
                Reverse = requestModel.Reverse,
                Subtasks = requestModel.Subtasks,
                Statuses = requestModel.Statuses,
                IncludeClosed = requestModel.IncludeClosed,
                Assignees = requestModel.Assignees,
                Watchers = requestModel.Watchers,
                Tags = requestModel.Tags,
                DueDateGreaterThan = requestModel.DueDateGreaterThan,
                DueDateLessThan = requestModel.DueDateLessThan,
                DateCreatedGreaterThan = requestModel.DateCreatedGreaterThan,
                DateCreatedLessThan = requestModel.DateCreatedLessThan,
                DateUpdatedGreaterThan = requestModel.DateUpdatedGreaterThan,
                DateUpdatedLessThan = requestModel.DateUpdatedLessThan,
                DateDoneGreaterThan = requestModel.DateDoneGreaterThan,
                DateDoneLessThan = requestModel.DateDoneLessThan,
                CustomFields = requestModel.CustomFields,
                CustomItems = requestModel.CustomItems
            };

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
                pagedRequestModel.Page = currentPage; // Set current page for this iteration

                // GetTasksAsync now returns IPagedResult<CuTask>
                var pagedResult = await GetTasksAsync(listId, pagedRequestModel, cancellationToken: cancellationToken).ConfigureAwait(false);

                if (pagedResult?.Items != null && pagedResult.Items.Any())
                {
                    foreach (var task in pagedResult.Items)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            yield break;
                        }
                        yield return task;
                    }
                    lastPage = !pagedResult.HasNextPage;
                }
                else
                {
                    lastPage = true; // No items implies last page or an issue
                }

                if (!lastPage)
                {
                    currentPage++;
                }
            } while (!lastPage);
        }

        /// <inheritdoc />
        public async Task<IPagedResult<CuTask>> GetFilteredTeamTasksAsync(
            string workspaceId,
            GetFilteredTeamTasksRequest requestModel,
            int? page = null, // Added to match interface, but requestModel.Page is primary
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workspaceId))
            {
                throw new ArgumentNullException(nameof(workspaceId));
            }
            if (requestModel == null)
            {
                throw new ArgumentNullException(nameof(requestModel));
            }

            // Prioritize page from requestModel if available, else use the direct parameter.
            if (page.HasValue && !requestModel.Page.HasValue)
            {
                requestModel.Page = page.Value;
            }
            int currentPage = requestModel.Page ?? 0; // Default to page 0 if not set

            _logger.LogInformation("Getting filtered team tasks for workspace ID: {WorkspaceId}, Page: {Page}", workspaceId, currentPage);

            var endpoint = $"team/{workspaceId}/task";
            var queryParams = new Dictionary<string, string?>();

            queryParams["page"] = currentPage.ToString(); // Always include page
            if (!string.IsNullOrEmpty(requestModel.OrderBy)) queryParams["order_by"] = requestModel.OrderBy;
            if (requestModel.Reverse.HasValue) queryParams["reverse"] = requestModel.Reverse.Value.ToString().ToLower();
            if (requestModel.Subtasks.HasValue) queryParams["subtasks"] = requestModel.Subtasks.Value.ToString().ToLower();
            if (requestModel.IncludeClosed.HasValue) queryParams["include_closed"] = requestModel.IncludeClosed.Value.ToString().ToLower();

            if (requestModel.DueDateGreaterThan.HasValue) queryParams["due_date_gt"] = requestModel.DueDateGreaterThan.Value.ToString();
            if (requestModel.DueDateLessThan.HasValue) queryParams["due_date_lt"] = requestModel.DueDateLessThan.Value.ToString();
            if (requestModel.DateCreatedGreaterThan.HasValue) queryParams["date_created_gt"] = requestModel.DateCreatedGreaterThan.Value.ToString();
            if (requestModel.DateCreatedLessThan.HasValue) queryParams["date_created_lt"] = requestModel.DateCreatedLessThan.Value.ToString();
            if (requestModel.DateUpdatedGreaterThan.HasValue) queryParams["date_updated_gt"] = requestModel.DateUpdatedGreaterThan.Value.ToString();
            if (requestModel.DateUpdatedLessThan.HasValue) queryParams["date_updated_lt"] = requestModel.DateUpdatedLessThan.Value.ToString();
            if (requestModel.DateDoneGreaterThan.HasValue) queryParams["date_done_gt"] = requestModel.DateDoneGreaterThan.Value.ToString();
            if (requestModel.DateDoneLessThan.HasValue) queryParams["date_done_lt"] = requestModel.DateDoneLessThan.Value.ToString();
            if (!string.IsNullOrEmpty(requestModel.ParentTaskId)) queryParams["parent"] = requestModel.ParentTaskId;
            if (requestModel.IncludeMarkdownDescription.HasValue) queryParams["include_markdown_description"] = requestModel.IncludeMarkdownDescription.Value.ToString().ToLower();

            if (!string.IsNullOrEmpty(requestModel.CustomFields)) queryParams["custom_fields"] = requestModel.CustomFields;
            if (requestModel.CustomTaskIds.HasValue) queryParams["custom_task_ids"] = requestModel.CustomTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(requestModel.TeamIdForCustomTaskIds)) queryParams["team_id"] = requestModel.TeamIdForCustomTaskIds; // Note: This is 'team_id' in query, not 'workspace_id'

            var queryString = BuildQueryString(queryParams);

            var arrayParams = new List<string>();
            if (requestModel.SpaceIds != null && requestModel.SpaceIds.Any()) arrayParams.Add(BuildQueryStringFromArray("space_ids", requestModel.SpaceIds));
            if (requestModel.ProjectIds != null && requestModel.ProjectIds.Any()) arrayParams.Add(BuildQueryStringFromArray("project_ids", requestModel.ProjectIds)); // project_ids is likely folder_ids
            if (requestModel.ListIds != null && requestModel.ListIds.Any()) arrayParams.Add(BuildQueryStringFromArray("list_ids", requestModel.ListIds));
            if (requestModel.Statuses != null && requestModel.Statuses.Any()) arrayParams.Add(BuildQueryStringFromArray("statuses", requestModel.Statuses));
            if (requestModel.Assignees != null && requestModel.Assignees.Any()) arrayParams.Add(BuildQueryStringFromArray("assignees", requestModel.Assignees));
            if (requestModel.Tags != null && requestModel.Tags.Any()) arrayParams.Add(BuildQueryStringFromArray("tags", requestModel.Tags));
            if (requestModel.CustomItems != null && requestModel.CustomItems.Any()) arrayParams.Add(BuildQueryStringFromArray("custom_items", requestModel.CustomItems.Select(ci => ci.ToString())));

            if (arrayParams.Any(p => !string.IsNullOrEmpty(p)))
            {
                queryString += (string.IsNullOrEmpty(queryString) || queryString == "?" ? (queryString == "?" ? "" : "?") : "&") + string.Join("&", arrayParams.Where(p => !string.IsNullOrEmpty(p)));
            }
            if (queryString == "?") queryString = string.Empty;

            var response = await _apiConnection.GetAsync<GetTasksResponse>($"{endpoint}{queryString}", cancellationToken);

            if (response == null)
            {
                _logger.LogWarning("API connection returned null response when getting filtered team tasks for workspace {WorkspaceId}. Returning empty paged result.", workspaceId);
                return PagedResult<CuTask>.Empty(currentPage);
            }

            var items = response.Tasks ?? Enumerable.Empty<CuTask>();
            return new PagedResult<CuTask>(
                items,
                currentPage,
                items.Count(), // PageSize for this specific page
                response.LastPage == false // HasNextPage
            );
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<CuTask> GetFilteredTeamTasksAsyncEnumerableAsync(
            string workspaceId,
            GetFilteredTeamTasksRequest requestModel,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting filtered team tasks as an async enumerable for workspace ID: {WorkspaceId}", workspaceId);
            if (string.IsNullOrWhiteSpace(workspaceId))
            {
                throw new ArgumentNullException(nameof(workspaceId));
            }
            if (requestModel == null)
            {
                throw new ArgumentNullException(nameof(requestModel));
            }

            // Clone the request model to modify the Page property without affecting the original
            var pagedRequestModel = new GetFilteredTeamTasksRequest
            {
                OrderBy = requestModel.OrderBy,
                Reverse = requestModel.Reverse,
                Subtasks = requestModel.Subtasks,
                SpaceIds = requestModel.SpaceIds,
                ProjectIds = requestModel.ProjectIds,
                ListIds = requestModel.ListIds,
                Statuses = requestModel.Statuses,
                IncludeClosed = requestModel.IncludeClosed,
                Assignees = requestModel.Assignees,
                Tags = requestModel.Tags,
                DueDateGreaterThan = requestModel.DueDateGreaterThan,
                DueDateLessThan = requestModel.DueDateLessThan,
                DateCreatedGreaterThan = requestModel.DateCreatedGreaterThan,
                DateCreatedLessThan = requestModel.DateCreatedLessThan,
                DateUpdatedGreaterThan = requestModel.DateUpdatedGreaterThan,
                DateUpdatedLessThan = requestModel.DateUpdatedLessThan,
                CustomFields = requestModel.CustomFields,
                CustomTaskIds = requestModel.CustomTaskIds,
                TeamIdForCustomTaskIds = requestModel.TeamIdForCustomTaskIds,
                CustomItems = requestModel.CustomItems,
                DateDoneGreaterThan = requestModel.DateDoneGreaterThan,
                DateDoneLessThan = requestModel.DateDoneLessThan,
                ParentTaskId = requestModel.ParentTaskId,
                IncludeMarkdownDescription = requestModel.IncludeMarkdownDescription
                // Page will be set in the loop
            };

            int currentPage = 0;
            bool lastPage;

            do
            {
                cancellationToken.ThrowIfCancellationRequested(); // Check before API call

                _logger.LogDebug("Fetching page {PageNumber} for filtered team tasks in workspace ID {WorkspaceId} via async enumerable.", currentPage, workspaceId);
                pagedRequestModel.Page = currentPage; // Set the current page for this request

                // GetFilteredTeamTasksAsync now returns IPagedResult<CuTask>
                var pagedResult = await GetFilteredTeamTasksAsync(
                    workspaceId: workspaceId,
                    requestModel: pagedRequestModel, // Pass the modified request model
                    cancellationToken: cancellationToken
                ).ConfigureAwait(false);

                if (pagedResult?.Items != null && pagedResult.Items.Any())
                {
                    foreach (var task in pagedResult.Items)
                    {
                        cancellationToken.ThrowIfCancellationRequested(); // Check before yielding each item
                        yield return task;
                    }
                    lastPage = !pagedResult.HasNextPage;
                }
                else
                {
                    lastPage = true; // No items implies last page or an issue
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
