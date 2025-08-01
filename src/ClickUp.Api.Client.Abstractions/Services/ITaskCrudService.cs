using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines the contract for basic CRUD operations on ClickUp tasks.
    /// Follows the Single Responsibility Principle by focusing only on task creation, reading, updating, and deletion.
    /// </summary>
    public interface ITaskCrudService
    {
        /// <summary>
        /// Creates a new task in the specified list.
        /// </summary>
        /// <param name="listId">The ID of the list to create the task in.</param>
        /// <param name="createTaskRequest">The task creation request.</param>
        /// <param name="customTaskIds">Whether to use custom task IDs.</param>
        /// <param name="teamId">The team ID for the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created task.</returns>
        Task<CuTask> CreateTaskAsync(
            string listId,
            CreateTaskRequest createTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a task by its ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to retrieve.</param>
        /// <param name="requestModel">The request parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The requested task.</returns>
        Task<CuTask> GetTaskAsync(
            string taskId,
            GetTaskRequest requestModel,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing task.
        /// </summary>
        /// <param name="taskId">The ID of the task to update.</param>
        /// <param name="updateTaskRequest">The task update request.</param>
        /// <param name="customTaskIds">Whether to use custom task IDs.</param>
        /// <param name="teamId">The team ID for the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated task.</returns>
        Task<CuTask> UpdateTaskAsync(
            string taskId,
            UpdateTaskRequest updateTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a task by its ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <param name="requestModel">The delete request parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        System.Threading.Tasks.Task DeleteTaskAsync(
            string taskId,
            DeleteTaskRequest requestModel,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new task from a template.
        /// </summary>
        /// <param name="listId">The ID of the list to create the task in.</param>
        /// <param name="templateId">The ID of the template to use.</param>
        /// <param name="createTaskFromTemplateRequest">The task creation request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created task.</returns>
        Task<CuTask> CreateTaskFromTemplateAsync(
            string listId,
            string templateId,
            CreateTaskFromTemplateRequest createTaskFromTemplateRequest,
            CancellationToken cancellationToken = default);
    }
}