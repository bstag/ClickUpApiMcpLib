using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Assuming DTOs for Task, CreateTaskRequest, UpdateTaskRequest, TaskTimeInStatus, etc.
    // public record TaskDto(...); // Define based on GetTask response
    // public record CreateTaskInListRequest(...); // Define based on POST /v2/list/{list_id}/task
    // public record UpdateTaskRequestDto(...); // Define based on PUT /v2/task/{task_id}
    // public record FilteredTasksRequestParams(...); // To encapsulate query params for GetFilteredTeamTasks
    // public record TaskTimeInStatusDto(...);
    // public record BulkTaskTimeInStatusDto(Dictionary<string, TaskTimeInStatusDto> Tasks); // Assuming task IDs are keys
    // public record MergeTasksRequest(IEnumerable<string> SourceTaskIds);
    // public record CreateTaskFromTemplateRequestDto(string Name);


    namespace ClickUp.Abstract;

    // Represents the Tasks operations in the ClickUp API
    // Based on endpoints like:
    // - GET /v2/list/{list_id}/task
    // - POST /v2/list/{list_id}/task
    // - GET /v2/task/{task_id}
    // - PUT /v2/task/{task_id}
    // - DELETE /v2/task/{task_id}
    // - GET /v2/team/{team_Id}/task
    // - POST /v2/task/{task_id}/merge
    // - GET /v2/task/{task_id}/time_in_status
    // - GET /v2/task/bulk_time_in_status/task_ids
    // - POST /v2/list/{list_id}/taskTemplate/{template_id}

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
        /// <param name="customFields">Optional. Filter by custom fields (complex JSON string).</param>
        /// <param name="customItems">Optional. Filter by custom task types.</param>
        /// <returns>A list of tasks.</returns>
        Task<IEnumerable<object>> GetTasksAsync(double listId, bool? archived = null, bool? includeMarkdownDescription = null, int? page = null, string? orderBy = null, bool? reverse = null, bool? subtasks = null, IEnumerable<string>? statuses = null, bool? includeClosed = null, IEnumerable<string>? assignees = null, IEnumerable<string>? watchers = null, IEnumerable<string>? tags = null, long? dueDateGreaterThan = null, long? dueDateLessThan = null, long? dateCreatedGreaterThan = null, long? dateCreatedLessThan = null, long? dateUpdatedGreaterThan = null, long? dateUpdatedLessThan = null, long? dateDoneGreaterThan = null, long? dateDoneLessThan = null, string? customFields = null, IEnumerable<double>? customItems = null);
        // Note: Return type should be a DTO that includes 'tasks' and 'last_page' (e.g. GetTasksResponseDto). Individual task objects should be TaskDto.

        /// <summary>
        /// Creates a new task in a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="createTaskRequest">Details of the task to create.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>The created task.</returns>
        Task<object> CreateTaskAsync(double listId, object createTaskRequest, bool? customTaskIds = null, double? teamId = null);
        // Note: createTaskRequest should be CreateTaskInListRequest, return type should be TaskDto.

        /// <summary>
        /// Retrieves details of a specific task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <param name="includeSubtasks">Optional. Whether to include subtasks.</param>
        /// <param name="includeMarkdownDescription">Optional. Whether to return task description in Markdown format.</param>
        /// <returns>Details of the task.</returns>
        Task<object> GetTaskAsync(string taskId, bool? customTaskIds = null, double? teamId = null, bool? includeSubtasks = null, bool? includeMarkdownDescription = null);
        // Note: Return type should be TaskDto.

        /// <summary>
        /// Updates a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="updateTaskRequest">Details for updating the task.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>The updated task.</returns>
        Task<object> UpdateTaskAsync(string taskId, object updateTaskRequest, bool? customTaskIds = null, double? teamId = null);
        // Note: updateTaskRequest should be UpdateTaskRequestDto, return type should be TaskDto.

        /// <summary>
        /// Deletes a task.
        /// </summary>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteTaskAsync(string taskId, bool? customTaskIds = null, double? teamId = null);
        // Note: API returns 204 No Content.

        /// <summary>
        /// Retrieves tasks from a Workspace based on specified filters.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_Id).</param>
        /// <param name="filterParams">Parameters for filtering tasks.</param>
        /// <returns>A list of filtered tasks.</returns>
        Task<IEnumerable<object>> GetFilteredTeamTasksAsync(double workspaceId, object filterParams);
        // Note: filterParams should be a DTO (e.g. FilteredTasksRequestParams) encapsulating all query parameters. Return type should be a DTO like GetTasksResponseDto.

        /// <summary>
        /// Merges multiple source tasks into a target task.
        /// </summary>
        /// <param name="targetTaskId">ID of the target task that other tasks will be merged into.</param>
        /// <param name="mergeTasksRequest">Contains the list of source task IDs to merge.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task MergeTasksAsync(string targetTaskId, object mergeTasksRequest);
        // Note: mergeTasksRequest should be MergeTasksRequest. API returns 200 OK.

        /// <summary>
        /// Retrieves the time a task has spent in each status.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>Time in status information for the task.</returns>
        Task<object> GetTaskTimeInStatusAsync(string taskId, bool? customTaskIds = null, double? teamId = null);
        // Note: Return type should be TaskTimeInStatusDto.

        /// <summary>
        /// Retrieves the time in status for multiple tasks.
        /// </summary>
        /// <param name="taskIds">A list of task IDs.</param>
        /// <param name="customTaskIds">Optional. If true, references tasks by their custom task ids.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>A dictionary mapping task IDs to their time in status information.</returns>
        Task<object> GetBulkTasksTimeInStatusAsync(IEnumerable<string> taskIds, bool? customTaskIds = null, double? teamId = null);
        // Note: Return type should be BulkTaskTimeInStatusDto or Dictionary<string, TaskTimeInStatusDto>.

        /// <summary>
        /// Creates a new task from a task template.
        /// </summary>
        /// <param name="listId">The ID of the List where the task will be created.</param>
        /// <param name="templateId">The ID of the task template.</param>
        /// <param name="createTaskFromTemplateRequest">Details for creating the task from a template (e.g., new task name).</param>
        /// <returns>The created task.</returns>
        Task<object> CreateTaskFromTemplateAsync(double listId, string templateId, object createTaskFromTemplateRequest);
        // Note: createTaskFromTemplateRequest should be CreateTaskFromTemplateRequestDto. Return type is not clearly defined in spec for this, might be TaskDto or an empty object.
    }
}
