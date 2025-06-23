using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Task operations.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive methods for managing Tasks, including CRUD operations,
    /// filtering, merging, retrieving time in status, and creating Tasks from templates.
    /// It also supports pagination for retrieving lists of Tasks.
    /// Covered API Endpoints (non-exhaustive list):
    /// - List Tasks: `GET /list/{list_id}/task`
    /// - Create Task: `POST /list/{list_id}/task`
    /// - Get Task: `GET /task/{task_id}`
    /// - Update Task: `PUT /task/{task_id}`
    /// - Delete Task: `DELETE /task/{task_id}`
    /// - Get Filtered Workspace Tasks: `GET /team/{team_id}/task`
    /// - Merge Tasks: `POST /task/{task_id}/merge` (Note: API endpoint might be different, this is based on typical patterns)
    /// - Task Time in Status: `GET /task/{task_id}/time_in_status`
    /// - Bulk Task Time in Status: `GET /task/bulk_time_in_status/task_ids`
    /// - Create Task from Template: `POST /list/{list_id}/taskTemplate/{template_id}`
    /// </remarks>
    public interface ITasksService
    {
        /// <summary>
        /// Retrieves a paginated list of Tasks within a specific List, with various filtering and sorting options.
        /// </summary>
        /// <param name="listId">The unique identifier of the List from which to retrieve Tasks.</param>
        /// <param name="archived">Optional. If set to <c>true</c>, includes archived Tasks in the results. Defaults to <c>false</c>.</param>
        /// <param name="includeMarkdownDescription">Optional. If set to <c>true</c>, returns Task descriptions in Markdown format. Defaults to <c>false</c>.</param>
        /// <param name="page">Optional. The page number to fetch for pagination (0-indexed).</param>
        /// <param name="orderBy">Optional. The field by which to order the Tasks (e.g., "created", "updated", "due_date").</param>
        /// <param name="reverse">Optional. If set to <c>true</c>, reverses the sort order. Defaults to <c>false</c>.</param>
        /// <param name="subtasks">Optional. If set to <c>true</c>, includes subtasks in the results. Defaults to <c>false</c>.</param>
        /// <param name="statuses">Optional. An enumerable of status names or IDs to filter Tasks by.</param>
        /// <param name="includeClosed">Optional. If set to <c>true</c>, includes closed Tasks in the results. Defaults to <c>false</c>.</param>
        /// <param name="assignees">Optional. An enumerable of user IDs to filter Tasks by assignees.</param>
        /// <param name="watchers">Optional. An enumerable of user IDs to filter Tasks by watchers.</param>
        /// <param name="tags">Optional. An enumerable of tag names to filter Tasks by.</param>
        /// <param name="dueDateGreaterThan">Optional. Filters for Tasks with a due date after this Unix timestamp (in milliseconds).</param>
        /// <param name="dueDateLessThan">Optional. Filters for Tasks with a due date before this Unix timestamp (in milliseconds).</param>
        /// <param name="dateCreatedGreaterThan">Optional. Filters for Tasks created after this Unix timestamp (in milliseconds).</param>
        /// <param name="dateCreatedLessThan">Optional. Filters for Tasks created before this Unix timestamp (in milliseconds).</param>
        /// <param name="dateUpdatedGreaterThan">Optional. Filters for Tasks updated after this Unix timestamp (in milliseconds).</param>
        /// <param name="dateUpdatedLessThan">Optional. Filters for Tasks updated before this Unix timestamp (in milliseconds).</param>
        /// <param name="dateDoneGreaterThan">Optional. Filters for Tasks completed after this Unix timestamp (in milliseconds).</param>
        /// <param name="dateDoneLessThan">Optional. Filters for Tasks completed before this Unix timestamp (in milliseconds).</param>
        /// <param name="customFields">Optional. A JSON string representing an array of custom field filters.</param>
        /// <param name="customItems">Optional. An enumerable of custom item type IDs to filter Tasks by.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetTasksResponse"/> object with the list of Tasks and pagination details.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Tasks in this List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<GetTasksResponse> GetTasksAsync(
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
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Task within a specified List.
        /// </summary>
        /// <param name="listId">The unique identifier of the List where the new Task will be created.</param>
        /// <param name="createTaskRequest">An object containing the details for the new Task, such as its name, description, assignees, and status.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the Task ID in the response and for related operations might be treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This may be required if <paramref name="customTaskIds"/> is <c>true</c> in certain contexts.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="CuTask"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="createTaskRequest"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided when required by the API for this combination.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create Tasks in this List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CuTask> CreateTaskAsync(
            string listId,
            CreateTaskRequest createTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the details of a specific Task by its ID.
        /// </summary>
        /// <param name="taskId">The unique identifier of the Task to retrieve.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="includeSubtasks">Optional. If set to <c>true</c>, includes subtasks in the response. Defaults to <c>false</c>.</param>
        /// <param name="includeMarkdownDescription">Optional. If set to <c>true</c>, returns the Task description in Markdown format. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the details of the requested <see cref="CuTask"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Task with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this Task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CuTask> GetTaskAsync(
            string taskId,
            bool? customTaskIds = null,
            string? teamId = null,
            bool? includeSubtasks = null,
            bool? includeMarkdownDescription = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the properties of an existing Task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the Task to update.</param>
        /// <param name="updateTaskRequest">An object containing the properties to update for the Task.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="CuTask"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="updateTaskRequest"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Task with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this Task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CuTask> UpdateTaskAsync(
            string taskId,
            UpdateTaskRequest updateTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specified Task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the Task to delete.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Task with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this Task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteTaskAsync(
            string taskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves Tasks from a Workspace (Team) based on a comprehensive set of filters.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="page">Optional. The page number to fetch for pagination (0-indexed).</param>
        /// <param name="orderBy">Optional. Field to order Tasks by.</param>
        /// <param name="reverse">Optional. If true, reverses the sort order.</param>
        /// <param name="subtasks">Optional. If true, includes subtasks in the results.</param>
        /// <param name="spaceIds">Optional. An enumerable of Space IDs to filter by.</param>
        /// <param name="projectIds">Optional. An enumerable of Project IDs (Folders) to filter by.</param>
        /// <param name="listIds">Optional. An enumerable of List IDs to filter by.</param>
        /// <param name="statuses">Optional. An enumerable of status names or IDs to filter by.</param>
        /// <param name="includeClosed">Optional. If true, includes closed Tasks.</param>
        /// <param name="assignees">Optional. An enumerable of user IDs to filter by assignees.</param>
        /// <param name="tags">Optional. An enumerable of tag names to filter by.</param>
        /// <param name="dueDateGreaterThan">Optional. Filters for Tasks with a due date after this Unix timestamp (ms).</param>
        /// <param name="dueDateLessThan">Optional. Filters for Tasks with a due date before this Unix timestamp (ms).</param>
        /// <param name="dateCreatedGreaterThan">Optional. Filters for Tasks created after this Unix timestamp (ms).</param>
        /// <param name="dateCreatedLessThan">Optional. Filters for Tasks created before this Unix timestamp (ms).</param>
        /// <param name="dateUpdatedGreaterThan">Optional. Filters for Tasks updated after this Unix timestamp (ms).</param>
        /// <param name="dateUpdatedLessThan">Optional. Filters for Tasks updated before this Unix timestamp (ms).</param>
        /// <param name="customFields">Optional. A JSON string representing an array of custom field filters.</param>
        /// <param name="customTaskIds">Optional. If true, indicates that task IDs used in other filters (like projectIds, listIds if they can contain task IDs) are custom task IDs. Also affects how task IDs are exported/returned if applicable.</param>
        /// <param name="teamIdForCustomTaskIds">Optional. The Workspace ID (team_id), required if <paramref name="customTaskIds"/> is true and custom task IDs are used in filters.</param>
        /// <param name="customItems">Optional. An enumerable of custom item type IDs (long integers) to filter Tasks by.</param>
        /// <param name="dateDoneGreaterThan">Optional. Filters for Tasks completed after this Unix timestamp (in milliseconds).</param>
        /// <param name="dateDoneLessThan">Optional. Filters for Tasks completed before this Unix timestamp (in milliseconds).</param>
        /// <param name="parentTaskId">Optional. Filters for subtasks of a specific parent Task ID.</param>
        /// <param name="includeMarkdownDescription">Optional. If set to <c>true</c>, returns Task descriptions in Markdown format. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetTasksResponse"/> object with the list of matching Tasks and pagination details.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Tasks in this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<GetTasksResponse> GetFilteredTeamTasksAsync(
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
            bool? customTaskIds = null,
            string? teamIdForCustomTaskIds = null,
            IEnumerable<long>? customItems = null,
            long? dateDoneGreaterThan = null,
            long? dateDoneLessThan = null,
            string? parentTaskId = null,
            bool? includeMarkdownDescription = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Merges a list of source Tasks into a single target Task. The source tasks will typically be deleted or closed after merging.
        /// </summary>
        /// <param name="targetTaskId">The unique identifier of the target Task into which other Tasks will be merged.</param>
        /// <param name="mergeTasksRequest">An object containing a list of source Task IDs to be merged into the target Task.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated target <see cref="CuTask"/>, reflecting the merge.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="targetTaskId"/> or <paramref name="mergeTasksRequest"/> is null, or if the list of source task IDs in the request is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the target Task or any of the source Tasks do not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to merge these Tasks.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        /// <remarks>The ClickUp API endpoint used for this operation (POST /task/{task_id}/merge) does not support custom task IDs.</remarks>
        Task<CuTask> MergeTasksAsync(
            string targetTaskId,
            MergeTasksRequest mergeTasksRequest,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Retrieves the amount of time a specific Task has spent in each status.
        /// </summary>
        /// <param name="taskId">The unique identifier of the Task.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="TaskTimeInStatusResponse"/> object with the time-in-status data for the Task.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Task with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information for the Task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<TaskTimeInStatusResponse> GetTaskTimeInStatusAsync(
            string taskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the time spent in each status for a bulk list of Tasks.
        /// </summary>
        /// <param name="taskIds">An enumerable collection of Task IDs for which to retrieve time-in-status data. The API expects a comma-separated string of up to 100 task IDs.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the task IDs in <paramref name="taskIds"/> are treated as custom task IDs. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetBulkTasksTimeInStatusResponse"/> object with time-in-status data for the requested Tasks.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskIds"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided, or if the number of task IDs exceeds the API limit (e.g., 100).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<GetBulkTasksTimeInStatusResponse> GetBulkTasksTimeInStatusAsync(
            IEnumerable<string> taskIds,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Task from a specified Task Template within a List.
        /// </summary>
        /// <param name="listId">The unique identifier of the List where the new Task will be created.</param>
        /// <param name="templateId">The unique identifier of the Task Template to use.</param>
        /// <param name="createTaskFromTemplateRequest">An object containing details for creating the Task from the template, such as the new Task's name.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, affects how the new task's ID might be handled or if the template ID itself is custom. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This may be required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="CuTask"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/>, <paramref name="templateId"/>, or <paramref name="createTaskFromTemplateRequest"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided when required.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List or Task Template with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create Tasks in this List or use the specified template.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CuTask> CreateTaskFromTemplateAsync(
            string listId,
            string templateId,
            CreateTaskFromTemplateRequest createTaskFromTemplateRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all Tasks within a specific List, automatically handling pagination using <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="listId">The unique identifier of the List.</param>
        /// <param name="archived">Optional. If set to <c>true</c>, includes archived Tasks. Defaults to <c>false</c>.</param>
        /// <param name="includeMarkdownDescription">Optional. If set to <c>true</c>, returns Task descriptions in Markdown format. Defaults to <c>false</c>.</param>
        /// <param name="orderBy">Optional. Field to order Tasks by.</param>
        /// <param name="reverse">Optional. If true, reverses the sort order.</param>
        /// <param name="subtasks">Optional. If true, includes subtasks.</param>
        /// <param name="statuses">Optional. An enumerable of status names or IDs to filter by.</param>
        /// <param name="includeClosed">Optional. If true, includes closed Tasks.</param>
        /// <param name="assignees">Optional. An enumerable of user IDs to filter by assignees.</param>
        /// <param name="watchers">Optional. An enumerable of user IDs to filter by watchers.</param>
        /// <param name="tags">Optional. An enumerable of tag names to filter by.</param>
        /// <param name="dueDateGreaterThan">Optional. Filters for Tasks with a due date after this Unix timestamp (ms).</param>
        /// <param name="dueDateLessThan">Optional. Filters for Tasks with a due date before this Unix timestamp (ms).</param>
        /// <param name="dateCreatedGreaterThan">Optional. Filters for Tasks created after this Unix timestamp (ms).</param>
        /// <param name="dateCreatedLessThan">Optional. Filters for Tasks created before this Unix timestamp (ms).</param>
        /// <param name="dateUpdatedGreaterThan">Optional. Filters for Tasks updated after this Unix timestamp (ms).</param>
        /// <param name="dateUpdatedLessThan">Optional. Filters for Tasks updated before this Unix timestamp (ms).</param>
        /// <param name="dateDoneGreaterThan">Optional. Filters for Tasks completed after this Unix timestamp (ms).</param>
        /// <param name="dateDoneLessThan">Optional. Filters for Tasks completed before this Unix timestamp (ms).</param>
        /// <param name="customFields">Optional. A JSON string representing an array of custom field filters.</param>
        /// <param name="customItems">Optional. An enumerable of custom item type IDs to filter by.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>An asynchronous stream of <see cref="CuTask"/> objects from the specified List.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Tasks in this List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown if an API call fails during the pagination process.</exception>
        IAsyncEnumerable<CuTask> GetTasksAsyncEnumerableAsync(
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
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Retrieves all Tasks from a Workspace (Team) based on a comprehensive set of filters, automatically handling pagination using <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="orderBy">Optional. Field to order Tasks by.</param>
        /// <param name="reverse">Optional. If true, reverses the sort order.</param>
        /// <param name="subtasks">Optional. If true, includes subtasks in the results.</param>
        /// <param name="spaceIds">Optional. An enumerable of Space IDs to filter by.</param>
        /// <param name="projectIds">Optional. An enumerable of Project IDs (Folders) to filter by.</param>
        /// <param name="listIds">Optional. An enumerable of List IDs to filter by.</param>
        /// <param name="statuses">Optional. An enumerable of status names or IDs to filter by.</param>
        /// <param name="includeClosed">Optional. If true, includes closed Tasks.</param>
        /// <param name="assignees">Optional. An enumerable of user IDs to filter by assignees.</param>
        /// <param name="tags">Optional. An enumerable of tag names to filter by.</param>
        /// <param name="dueDateGreaterThan">Optional. Filters for Tasks with a due date after this Unix timestamp (ms).</param>
        /// <param name="dueDateLessThan">Optional. Filters for Tasks with a due date before this Unix timestamp (ms).</param>
        /// <param name="dateCreatedGreaterThan">Optional. Filters for Tasks created after this Unix timestamp (ms).</param>
        /// <param name="dateCreatedLessThan">Optional. Filters for Tasks created before this Unix timestamp (ms).</param>
        /// <param name="dateUpdatedGreaterThan">Optional. Filters for Tasks updated after this Unix timestamp (ms).</param>
        /// <param name="dateUpdatedLessThan">Optional. Filters for Tasks updated before this Unix timestamp (ms).</param>
        /// <param name="customFields">Optional. A JSON string representing an array of custom field filters.</param>
        /// <param name="customTaskIds">Optional. If true, indicates that task IDs used in other filters are custom task IDs.</param>
        /// <param name="teamIdForCustomTaskIds">Optional. The Workspace ID (team_id), required if <paramref name="customTaskIds"/> is true.</param>
        /// <param name="customItems">Optional. An enumerable of custom item type IDs (long integers) to filter Tasks by.</param>
        /// <param name="dateDoneGreaterThan">Optional. Filters for Tasks completed after this Unix timestamp (in milliseconds).</param>
        /// <param name="dateDoneLessThan">Optional. Filters for Tasks completed before this Unix timestamp (in milliseconds).</param>
        /// <param name="parentTaskId">Optional. Filters for subtasks of a specific parent Task ID.</param>
        /// <param name="includeMarkdownDescription">Optional. If set to <c>true</c>, returns Task descriptions in Markdown format. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>An asynchronous stream of <see cref="CuTask"/> objects from the specified Workspace matching the filters.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Tasks in this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown if an API call fails during the pagination process.</exception>
        IAsyncEnumerable<CuTask> GetFilteredTeamTasksAsyncEnumerableAsync(
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
            string? customFields = null,
            bool? customTaskIds = null,
            string? teamIdForCustomTaskIds = null,
            IEnumerable<long>? customItems = null,
            long? dateDoneGreaterThan = null,
            long? dateDoneLessThan = null,
            string? parentTaskId = null,
            bool? includeMarkdownDescription = null,
            CancellationToken cancellationToken = default
        );
    }
}
