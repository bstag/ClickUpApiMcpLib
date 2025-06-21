using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities; // Assuming CuTask DTO is here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Task Relationships operations in the ClickUp API, such as managing dependencies and links between tasks.
    /// </summary>
    /// <remarks>
    /// This service allows for creating and removing dependencies (tasks blocking or being blocked by others)
    /// and linking tasks that are related but not strictly dependent.
    /// Based on ClickUp API endpoints like:
    /// <list type="bullet">
    /// <item><description>POST /v2/task/{task_id}/dependency</description></item>
    /// <item><description>DELETE /v2/task/{task_id}/dependency</description></item>
    /// <item><description>POST /v2/task/{task_id}/link/{links_to_task_id}</description></item>
    /// <item><description>DELETE /v2/task/{task_id}/link/{links_to_task_id}</description></item>
    /// </list>
    /// </remarks>
    public interface ITaskRelationshipsService
    {
        /// <summary>
        /// Adds a dependency between two tasks.
        /// The <paramref name="taskId"/> will either depend on <paramref name="dependsOnTaskId"/>,
        /// or <paramref name="dependencyOfTaskId"/> will depend on <paramref name="taskId"/>.
        /// Only one of <paramref name="dependsOnTaskId"/> or <paramref name="dependencyOfTaskId"/> should be provided.
        /// </summary>
        /// <param name="taskId">The primary task's ID for the dependency relationship.</param>
        /// <param name="dependsOnTaskId">Optional. The ID of the task that <paramref name="taskId"/> will depend on (i.e., <paramref name="taskId"/> is blocked by <paramref name="dependsOnTaskId"/>).</param>
        /// <param name="dependencyOfTaskId">Optional. The ID of the task that will depend on <paramref name="taskId"/> (i.e., <paramref name="dependencyOfTaskId"/> is blocked by <paramref name="taskId"/>).</param>
        /// <param name="customTaskIds">Optional. If true, all task IDs are treated as custom task IDs. Requires <paramref name="teamId"/>.</param>
        /// <param name="teamId">Optional. The Workspace ID, required if <paramref name="customTaskIds"/> is true.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown if both or neither <paramref name="dependsOnTaskId"/> and <paramref name="dependencyOfTaskId"/> are provided.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid task IDs or authentication issues.</exception>
        /// <remarks>The ClickUp API for adding a dependency requires a JSON body specifying `depends_on` or `dependency_of`.</remarks>
        System.Threading.Tasks.Task AddDependencyAsync(
            string taskId,
            string? dependsOnTaskId = null,
            string? dependencyOfTaskId = null,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a dependency relationship from a task.
        /// You must specify which relationship to remove by providing either <paramref name="dependsOnTaskId"/> (the task <paramref name="taskId"/> depends on)
        /// or <paramref name="dependencyOfTaskId"/> (the task that depends on <paramref name="taskId"/>).
        /// </summary>
        /// <param name="taskId">The primary task's ID from which the dependency is being removed.</param>
        /// <param name="dependsOnTaskId">The ID of the task that <paramref name="taskId"/> currently depends on. Provide this to remove this specific "depends on" relationship.</param>
        /// <param name="dependencyOfTaskId">The ID of the task that currently depends on <paramref name="taskId"/>. Provide this to remove this specific "dependency of" relationship.</param>
        /// <param name="customTaskIds">Optional. If true, all task IDs are treated as custom task IDs. Requires <paramref name="teamId"/>.</param>
        /// <param name="teamId">Optional. The Workspace ID, required if <paramref name="customTaskIds"/> is true.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="taskId"/> is null or empty, or if both <paramref name="dependsOnTaskId"/> and <paramref name="dependencyOfTaskId"/> are null or empty (as one is required by the API).</exception>
        /// <exception cref="ArgumentException">Thrown if both <paramref name="dependsOnTaskId"/> and <paramref name="dependencyOfTaskId"/> are provided, as the API expects only one for deletion.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid task IDs or relationship not found.</exception>
        /// <remarks>The ClickUp API uses query parameters for deleting a dependency: `depends_on={task_id}` or `dependency_of={task_id}`. This method's parameters reflect that one of these must be chosen to identify the specific dependency link to remove.</remarks>
        System.Threading.Tasks.Task DeleteDependencyAsync(
            string taskId,
            string? dependsOnTaskId, // Changed to nullable, API requires one or the other.
            string? dependencyOfTaskId, // Changed to nullable, API requires one or the other.
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Links two tasks together. This is a non-dependent relationship, indicating the tasks are related.
        /// </summary>
        /// <param name="taskId">The ID of the first task in the link.</param>
        /// <param name="linksToTaskId">The ID of the second task to link with the first task.</param>
        /// <param name="customTaskIds">Optional. If true, all task IDs are treated as custom task IDs. Requires <paramref name="teamId"/>.</param>
        /// <param name="teamId">Optional. The Workspace ID, required if <paramref name="customTaskIds"/> is true.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="ClickUp.Api.Client.Models.Entities.Tasks.CuTask"/> object, typically for the <paramref name="taskId"/>, reflecting the new link.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="linksToTaskId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid task IDs or authentication issues.</exception>
        Task<ClickUp.Api.Client.Models.Entities.Tasks.CuTask?> AddTaskLinkAsync(
            string taskId,
            string linksToTaskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an existing link between two tasks.
        /// </summary>
        /// <param name="taskId">The ID of the first task in the link.</param>
        /// <param name="linksToTaskId">The ID of the second task that is currently linked to the first task.</param>
        /// <param name="customTaskIds">Optional. If true, all task IDs are treated as custom task IDs. Requires <paramref name="teamId"/>.</param>
        /// <param name="teamId">Optional. The Workspace ID, required if <paramref name="customTaskIds"/> is true.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="ClickUp.Api.Client.Models.Entities.Tasks.CuTask"/> object, typically for the <paramref name="taskId"/>, reflecting the removed link.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="linksToTaskId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid task IDs, link not found, or authentication issues.</exception>
        Task<ClickUp.Api.Client.Models.Entities.Tasks.CuTask?> DeleteTaskLinkAsync(
            string taskId,
            string linksToTaskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
    }
}
