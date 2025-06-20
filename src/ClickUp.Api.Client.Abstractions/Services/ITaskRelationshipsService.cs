using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities; // Assuming CuTask DTO is here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the CuTask Relationships operations in the ClickUp API, such as dependencies and links.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - POST /v2/task/{task_id}/dependency
    /// - DELETE /v2/task/{task_id}/dependency
    /// - POST /v2/task/{task_id}/link/{links_to}
    /// - DELETE /v2/task/{task_id}/link/{links_to}
    /// </remarks>
    public interface ITaskRelationshipsService
    {
        /// <summary>
        /// Sets a task as waiting on another task (depends_on) or making another task wait for it (dependency_of).
        /// Only one of 'dependsOnTaskId' or 'dependencyOfTaskId' should be provided.
        /// </summary>
        /// <param name="taskId">The ID of the task to which the dependency is being added.</param>
        /// <param name="dependsOnTaskId">Optional. The ID of the task that <paramref name="taskId"/> will depend on.</param>
        /// <param name="dependencyOfTaskId">Optional. The ID of the task that will depend on <paramref name="taskId"/>.</param>
        /// <param name="customTaskIds">Optional. If true, references tasks by their custom task IDs.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <remarks>Consider using a request DTO for future clarity if API evolves to take more complex dependency structures.</remarks>
        System.Threading.Tasks.Task AddDependencyAsync(
            string taskId,
            string? dependsOnTaskId = null,
            string? dependencyOfTaskId = null,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a dependency relationship from a task.
        /// Provide either 'dependsOnTaskId' or 'dependencyOfTaskId' to specify the relationship to remove.
        /// </summary>
        /// <param name="taskId">The ID of the task from which the dependency is being removed.</param>
        /// <param name="dependsOnTaskId">The ID of the task that <paramref name="taskId"/> depends on, to remove this specific dependency.</param>
        /// <param name="dependencyOfTaskId">The ID of the task that is a dependency of <paramref name="taskId"/>, to remove this specific dependency.</param>
        /// <param name="customTaskIds">Optional. If true, references tasks by their custom task IDs.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <remarks>The ClickUp API uses query parameters for DELETE dependency: depends_on and dependency_of. Ensure implementation reflects this.</remarks>
        System.Threading.Tasks.Task DeleteDependencyAsync(
            string taskId,
            string dependsOnTaskId, // Kept as non-optional based on original, but API implies one is chosen.
            string dependencyOfTaskId, // Kept as non-optional based on original.
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Links two tasks together.
        /// </summary>
        /// <param name="taskId">The ID of the task to initiate the link from.</param>
        /// <param name="linksToTaskId">The ID of the task to link to.</param>
        /// <param name="customTaskIds">Optional. If true, references tasks by their custom task IDs.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="ClickUp.Api.Client.Models.Entities.Tasks.CuTask"/> (usually the source task with relationship info).</returns>
        Task<ClickUp.Api.Client.Models.Entities.Tasks.CuTask?> AddTaskLinkAsync(
            string taskId,
            string linksToTaskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the link between two tasks.
        /// </summary>
        /// <param name="taskId">The ID of the task from which the link is initiated.</param>
        /// <param name="linksToTaskId">The ID of the task linked to.</param>
        /// <param name="customTaskIds">Optional. If true, references tasks by their custom task IDs.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="ClickUp.Api.Client.Models.Entities.Tasks.CuTask"/>.</returns>
        Task<ClickUp.Api.Client.Models.Entities.Tasks.CuTask?> DeleteTaskLinkAsync(
            string taskId,
            string linksToTaskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
    }
}
