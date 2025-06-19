using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Task Relationships operations in the ClickUp API
    // Based on endpoints like:
    // - POST /v2/task/{task_id}/dependency
    // - DELETE /v2/task/{task_id}/dependency
    // - POST /v2/task/{task_id}/link/{links_to}
    // - DELETE /v2/task/{task_id}/link/{links_to}

    public interface ITaskRelationshipsService
    {
        /// <summary>
        /// Sets a task as waiting on or blocking another task.
        /// </summary>
        /// <param name="taskId">The ID of the task which is waiting on or blocking another task.</param>
        /// <param name="dependsOnTaskId">The ID of the task that must be completed before the task identified by taskId.</param>
        /// <param name="dependencyOfTaskId">The ID of the task that's waiting for the task identified by taskId to be completed.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task AddDependencyAsync(string taskId, string? dependsOnTaskId = null, string? dependencyOfTaskId = null, bool? customTaskIds = null, double? teamId = null);

        /// <summary>
        /// Removes a dependency relationship between two tasks.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="dependsOnTaskId">The ID of the task that the primary task depends on.</param>
        /// <param name="dependencyOfTaskId">The ID of the task that is a dependency of the primary task.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteDependencyAsync(string taskId, string dependsOnTaskId, string dependencyOfTaskId, bool? customTaskIds = null, double? teamId = null);

        /// <summary>
        /// Links two tasks together.
        /// </summary>
        /// <param name="taskId">The ID of the task to initiate the link from.</param>
        /// <param name="linksToTaskId">The ID of the task to link to.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the linked task details.</returns>
        Task<object> AddTaskLinkAsync(string taskId, string linksToTaskId, bool? customTaskIds = null, double? teamId = null);
        // Note: The return type 'object' should be replaced with a specific DTO representing the linked task structure from the API response.

        /// <summary>
        /// Removes the link between two tasks.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="linksToTaskId">The ID of the task linked to.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated task details.</returns>
        Task<object> DeleteTaskLinkAsync(string taskId, string linksToTaskId, bool? customTaskIds = null, double? teamId = null);
        // Note: The return type 'object' should be replaced with a specific DTO representing the task structure from the API response.
    }
}
