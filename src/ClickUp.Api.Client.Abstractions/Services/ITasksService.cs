using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities; // Assuming CuTask DTO is here
using ClickUp.Api.Client.Models.RequestModels.Tasks; // Assuming Request DTOs are here
using ClickUp.Api.Client.Models.ResponseModels.Tasks; // Assuming Response DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Tasks operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/list/{list_id}/task
    /// - POST /v2/list/{list_id}/task
    /// - GET /v2/task/{task_id}
    /// - PUT /v2/task/{task_id}
    /// - DELETE /v2/task/{task_id}
    /// - GET /v2/team/{team_Id}/task
    /// - POST /v2/task/{task_id}/merge
    /// - GET /v2/task/{task_id}/time_in_status
    /// - GET /v2/task/bulk_time_in_status/task_ids
    /// - POST /v2/list/{list_id}/taskTemplate/{template_id}
    /// </remarks>
    public interface ITasksService
    {
        /// <summary>
        /// Retrieves tasks in a specific List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="archived">Optional. Whether to include archived tasks.</param>
        /// <param name="includeMarkdownDescription">Optional. Whether to return task descriptions in Markdown format.</param>
        /// <param name="page">Optional. Page to fetch (starts at 0).</param>
        /// <param name="orderBy">Optional. Field to order by (e.g., "created", "updated", "due_date").</param>
        /// <param name="reverse">Optional. Whether to reverse the order.</param>
        /// <param name="subtasks">Optional. Whether to include subtasks.</param>
        /// <param name="statuses">Optional. Filter by statuses.</param>
        /// <param name="includeClosed">Optional. Whether to include closed tasks.</param>
        /// <param name="assignees">Optional. Filter by assignees (user IDs).</param>
        /// <param name="watchers">Optional. Filter by watchers (user IDs).</param>
        /// <param name="tags">Optional. Filter by tags.</param>
        /// <param name="dueDateGreaterThan">Optional. Filter by due date greater than (Unix time ms).</param>
        /// <param name="dueDateLessThan">Optional. Filter by due date less than (Unix time ms).</param>
        /// <param name="dateCreatedGreaterThan">Optional. Filter by date created greater than (Unix time ms).</param>
        /// <param name="dateCreatedLessThan">Optional. Filter by date created less than (Unix time ms).</param>
        /// <param name="dateUpdatedGreaterThan">Optional. Filter by date updated greater than (Unix time ms).</param>
        /// <param name="dateUpdatedLessThan">Optional. Filter by date updated less than (Unix time ms).</param>
        /// <param name="dateDoneGreaterThan">Optional. Filter by date done greater than (Unix time ms).</param>
        /// <param name="dateDoneLessThan">Optional. Filter by date done less than (Unix time ms).</param>
        /// <param name="customFields">Optional. Filter by custom fields (JSON string representation of an array of objects).</param>
        /// <param name="customItems">Optional. Filter by custom task types (integer IDs).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="GetTasksResponse"/> object containing a list of tasks and pagination details.</returns>
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
            IEnumerable<long>? customItems = null, // Changed from IEnumerable<double> to IEnumerable<long> as custom item IDs are usually integers
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new task in a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="createTaskRequest">Details of the task to create.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="Task"/> object.</returns>
        Task<Task> CreateTaskAsync(
            string listId,
            CreateTaskRequest createTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves details of a specific task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="includeSubtasks">Optional. Whether to include subtasks.</param>
        /// <param name="includeMarkdownDescription">Optional. Whether to return task description in Markdown format.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="Task"/>.</returns>
        Task<Task> GetTaskAsync(
            string taskId,
            bool? customTaskIds = null,
            string? teamId = null,
            bool? includeSubtasks = null,
            bool? includeMarkdownDescription = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="updateTaskRequest">Details for updating the task.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="Task"/>.</returns>
        Task<Task> UpdateTaskAsync(
            string taskId,
            UpdateTaskRequest updateTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a task.
        /// </summary>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task DeleteTaskAsync(
            string taskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves tasks from a Workspace (Team) based on specified filters.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="page">Optional. Page to fetch (starts at 0).</param>
        /// <param name="orderBy">Optional. Field to order by.</param>
        /// <param name="reverse">Optional. Whether to reverse the order.</param>
        /// <param name="subtasks">Optional. Whether to include subtasks.</param>
        /// <param name="spaceIds">Optional. Filter by Space IDs.</param>
        /// <param name="projectIds">Optional. Filter by Project IDs (Lists).</param>
        /// <param name="listIds">Optional. Filter by List IDs.</param>
        /// <param name="statuses">Optional. Filter by statuses.</param>
        /// <param name="includeClosed">Optional. Whether to include closed tasks.</param>
        /// <param name="assignees">Optional. Filter by assignees (user IDs).</param>
        /// <param name="tags">Optional. Filter by tags.</param>
        /// <param name="dueDateGreaterThan">Optional. Filter by due date greater than (Unix time ms).</param>
        /// <param name="dueDateLessThan">Optional. Filter by due date less than (Unix time ms).</param>
        /// <param name="dateCreatedGreaterThan">Optional. Filter by date created greater than (Unix time ms).</param>
        /// <param name="dateCreatedLessThan">Optional. Filter by date created less than (Unix time ms).</param>
        /// <param name="dateUpdatedGreaterThan">Optional. Filter by date updated greater than (Unix time ms).</param>
        /// <param name="dateUpdatedLessThan">Optional. Filter by date updated less than (Unix time ms).</param>
        /// <param name="customFields">Optional. Filter by custom fields (JSON string).</param>
        /// <param name="customTaskIds">Optional. Export tasks with Custom CuTask IDs.</param>
        /// <param name="teamIdForCustomTaskIds">Optional. Team ID, required if custom_task_ids is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="GetTasksResponse"/> object containing a list of tasks and pagination details.</returns>
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
            bool? customTaskIds = null, // This is for the query param "custom_task_ids" not the path param modifier
            string? teamIdForCustomTaskIds = null, // This is for the query param "team_id" related to "custom_task_ids"
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Merges a task into another task.
        /// </summary>
        /// <param name="taskId">The ID of the task to merge from.</param>
        /// <param name="targetTaskId">The ID of the task to merge into.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true for either task.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task MergeTasksAsync(
            string taskId,
            string targetTaskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
        // Note: The original ClickUp API POST /v2/task/{task_id}/merge takes the target in the body.
        // This method signature might need adjustment if it's for merging task_id INTO target_task_id.
        // The prompt said "MergeTasksAsync: Request MergeTasksRequest, response CuTask".
        // Let's assume for now it means merge ONE task (taskId) into another (targetTaskId).
        // If it's for merging MULTIPLE tasks, the signature and DTO would be different (e.g. MergeTasksRequest DTO in body).
        // Given the current method name and params, it seems like merging one specific task into another specific task.
        // The API doc for "Merge Tasks" (POST /v2/task/{task_id}/merge) implies task_id is the source, and target is in body { "target_task_id": "string" }
        // This is different from "Merge CuTask Into" (POST /v2/task/{task_id}/merge_into/{target_task_id})
        // The prompt's "MergeTasksRequest" suggests a DTO. I will adjust this method to take a DTO.

        /// <summary>
        /// Merges tasks into a target task.
        /// </summary>
        /// <param name="targetTaskId">ID of the target task that other tasks will be merged into.</param>
        /// <param name="mergeTasksRequest">Contains the list of source task IDs to merge.</param>
        /// <param name="customTaskIds">Optional. If true, references tasks by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated target <see cref="Task"/>.</returns>
        Task<Task> MergeTasksAsync(
            string targetTaskId,
            MergeTasksRequest mergeTasksRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Retrieves the time a task has spent in each status.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="TaskTimeInStatusResponse"/> object for the task.</returns>
        Task<TaskTimeInStatusResponse> GetTaskTimeInStatusAsync(
            string taskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the time in status for multiple tasks.
        /// </summary>
        /// <param name="taskIds">A comma-separated list of task IDs. Max 100.</param>
        /// <param name="customTaskIds">Optional. If true, references tasks by their custom task ids.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="GetBulkTasksTimeInStatusResponse"/> object containing time in status for multiple tasks.</returns>
        Task<GetBulkTasksTimeInStatusResponse> GetBulkTasksTimeInStatusAsync(
            IEnumerable<string> taskIds, // API expects comma-separated string, but IEnumerable is more C#-friendly for input
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new task from a task template.
        /// </summary>
        /// <param name="listId">The ID of the List where the task will be created.</param>
        /// <param name="templateId">The ID of the task template.</param>
        /// <param name="createTaskFromTemplateRequest">Details for creating the task from a template (e.g., new task name).</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="Task"/>.</returns>
        Task<Task> CreateTaskFromTemplateAsync(
            string listId,
            string templateId,
            CreateTaskFromTemplateRequest createTaskFromTemplateRequest,
            bool? customTaskIds = null,
            string? teamId = null, // team_id is mentioned for custom_task_ids in other places, assuming it applies here too
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all tasks in a specific List, automatically handling pagination.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="archived">Optional. Whether to include archived tasks.</param>
        /// <param name="includeMarkdownDescription">Optional. Whether to return task descriptions in Markdown format.</param>
        /// <param name="orderBy">Optional. Field to order by (e.g., "created", "updated", "due_date").</param>
        /// <param name="reverse">Optional. Whether to reverse the order.</param>
        /// <param name="subtasks">Optional. Whether to include subtasks.</param>
        /// <param name="statuses">Optional. Filter by statuses.</param>
        /// <param name="includeClosed">Optional. Whether to include closed tasks.</param>
        /// <param name="assignees">Optional. Filter by assignees (user IDs).</param>
        /// <param name="watchers">Optional. Filter by watchers (user IDs).</param>
        /// <param name="tags">Optional. Filter by tags.</param>
        /// <param name="dueDateGreaterThan">Optional. Filter by due date greater than (Unix time ms).</param>
        /// <param name="dueDateLessThan">Optional. Filter by due date less than (Unix time ms).</param>
        /// <param name="dateCreatedGreaterThan">Optional. Filter by date created greater than (Unix time ms).</param>
        /// <param name="dateCreatedLessThan">Optional. Filter by date created less than (Unix time ms).</param>
        /// <param name="dateUpdatedGreaterThan">Optional. Filter by date updated greater than (Unix time ms).</param>
        /// <param name="dateUpdatedLessThan">Optional. Filter by date updated less than (Unix time ms).</param>
        /// <param name="dateDoneGreaterThan">Optional. Filter by date done greater than (Unix time ms).</param>
        /// <param name="dateDoneLessThan">Optional. Filter by date done less than (Unix time ms).</param>
        /// <param name="customFields">Optional. Filter by custom fields (JSON string representation of an array of objects).</param>
        /// <param name="customItems">Optional. Filter by custom task types (integer IDs).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An asynchronous stream of tasks.</returns>
        IAsyncEnumerable<Task> GetTasksAsyncEnumerableAsync(
            string listId,
            bool? archived = null,
            bool? includeMarkdownDescription = null,
            // No 'page' parameter here
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
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default
        );
    }
}
