using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services.Tags
{
    /// <summary>
    /// Service interface for managing ClickUp Tag assignments to Tasks.
    /// This interface handles adding and removing tags from tasks.
    /// </summary>
    /// <remarks>
    /// This service provides methods for applying or removing Tags from Tasks.
    /// Covered API Endpoints:
    /// - Task Tags: `POST /task/{task_id}/tag/{tag_name}`, `DELETE /task/{task_id}/tag/{tag_name}`
    /// </remarks>
    public interface ITaskTagManager
    {
        /// <summary>
        /// Adds an existing Tag to a specific task. The Tag must already exist within the Space of the task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task to which the Tag will be added.</param>
        /// <param name="tagName">The name of the Tag to add. The Tag must be predefined in the task's Space.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="tagName"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or the Tag (within the task's Space) with the specified ID/name does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to add Tags to this task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task AddTagToTaskAsync(
            string taskId,
            string tagName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an existing Tag from a specific task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task from which the Tag will be removed.</param>
        /// <param name="tagName">The name of the Tag to remove from the task.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="tagName"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task does not exist, or if the Tag is not currently applied to the task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove Tags from this task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task RemoveTagFromTaskAsync(
            string taskId,
            string tagName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
    }
}