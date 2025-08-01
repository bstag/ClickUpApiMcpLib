using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines operations for creating, updating, and deleting tasks.
    /// </summary>
    public interface ITaskWriter
    {
        /// <summary>
        /// Creates a new Task within a specified List.
        /// </summary>
        /// <param name="listId">The unique identifier of the List where the new Task will be created.</param>
        /// <param name="createTaskRequest">An object containing the details for the new Task, such as its name, description, assignees, and status.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the Task ID in the response and for related operations might be treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This may be required if <paramref name="customTaskIds"/> is <c>true</c> in certain contexts.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="CuTask"/> object.</returns>
        Task<CuTask> CreateTaskAsync(
            string listId,
            CreateTaskRequest createTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
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
        /// <param name="requestModel">An object containing options for custom task ID handling.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        System.Threading.Tasks.Task DeleteTaskAsync(
            string taskId,
            DeleteTaskRequest requestModel,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Task from a specified Task Template within a List.
        /// </summary>
        /// <param name="listId">The unique identifier of the List where the new Task will be created.</param>
        /// <param name="templateId">The unique identifier of the Task Template to use.</param>
        /// <param name="createTaskFromTemplateRequest">An object containing details for creating the Task from the template, such as the new Task's name.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="CuTask"/> object.</returns>
        Task<CuTask> CreateTaskFromTemplateAsync(
            string listId,
            string templateId,
            CreateTaskFromTemplateRequest createTaskFromTemplateRequest,
            CancellationToken cancellationToken = default);
    }
}