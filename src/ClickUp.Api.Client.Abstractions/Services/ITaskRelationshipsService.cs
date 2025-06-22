using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Task Relationships operations.
    /// </summary>
    /// <remarks>
    /// This service allows for managing dependencies (tasks blocking or being blocked by others)
    /// and linking tasks that are related but not strictly dependent.
    /// Covered API Endpoints:
    /// - Add Dependency: `POST /task/{task_id}/dependency`
    /// - Remove Dependency: `DELETE /task/{task_id}/dependency`
    /// - Add Task Link: `POST /task/{task_id}/link/{links_to_task_id}`
    /// - Remove Task Link: `DELETE /task/{task_id}/link/{links_to_task_id}`
    /// </remarks>
    public interface ITaskRelationshipsService
    {
        /// <summary>
        /// Adds a dependency relationship between two tasks.
        /// The relationship is defined by specifying either that <paramref name="taskId"/> depends on <paramref name="dependsOnTaskId"/>,
        /// or that <paramref name="dependencyOfTaskId"/> depends on <paramref name="taskId"/>.
        /// Only one of <paramref name="dependsOnTaskId"/> or <paramref name="dependencyOfTaskId"/> should be provided.
        /// </summary>
        /// <param name="taskId">The unique identifier of the primary task involved in the dependency.</param>
        /// <param name="dependsOnTaskId">Optional. The unique identifier of the task that <paramref name="taskId"/> will depend on (i.e., <paramref name="taskId"/> is blocked by <paramref name="dependsOnTaskId"/>).</param>
        /// <param name="dependencyOfTaskId">Optional. The unique identifier of the task that will depend on <paramref name="taskId"/> (i.e., <paramref name="dependencyOfTaskId"/> is blocked by <paramref name="taskId"/>).</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, all task IDs provided are treated as custom task IDs. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if both or neither of <paramref name="dependsOnTaskId"/> and <paramref name="dependencyOfTaskId"/> are provided, or if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if any of the specified tasks do not exist or are not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify dependencies for these tasks.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        System.Threading.Tasks.Task AddDependencyAsync(
            string taskId,
            string? dependsOnTaskId = null,
            string? dependencyOfTaskId = null,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an existing dependency relationship from a task.
        /// To specify which relationship to remove, provide either <paramref name="dependsOnTaskId"/> (the task <paramref name="taskId"/> depends on)
        /// or <paramref name="dependencyOfTaskId"/> (the task that depends on <paramref name="taskId"/>). Only one should be provided.
        /// </summary>
        /// <param name="taskId">The unique identifier of the primary task from which the dependency is being removed.</param>
        /// <param name="dependsOnTaskId">Optional. The unique identifier of the task that <paramref name="taskId"/> currently depends on. Provide this to remove this specific "depends on" relationship.</param>
        /// <param name="dependencyOfTaskId">Optional. The unique identifier of the task that currently depends on <paramref name="taskId"/>. Provide this to remove this specific "dependency of" relationship.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, all task IDs provided are treated as custom task IDs. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if both or neither of <paramref name="dependsOnTaskId"/> and <paramref name="dependencyOfTaskId"/> are provided (as one is required), or if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if any of the specified tasks or the dependency relationship does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify dependencies for these tasks.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteDependencyAsync(
            string taskId,
            string? dependsOnTaskId,
            string? dependencyOfTaskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a non-dependent link between two tasks, indicating they are related.
        /// </summary>
        /// <param name="taskId">The unique identifier of the first task in the link.</param>
        /// <param name="linksToTaskId">The unique identifier of the second task to be linked with the first task.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, all task IDs provided are treated as custom task IDs. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="CuTask"/> object, typically for the <paramref name="taskId"/>, reflecting the new link. The return can be null if the API does not return content on success.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="linksToTaskId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if either of the tasks with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to link these tasks.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CuTask?> AddTaskLinkAsync(
            string taskId,
            string linksToTaskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an existing non-dependent link between two tasks.
        /// </summary>
        /// <param name="taskId">The unique identifier of the first task in the link.</param>
        /// <param name="linksToTaskId">The unique identifier of the second task that is currently linked to the first task.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, all task IDs provided are treated as custom task IDs. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="CuTask"/> object, typically for the <paramref name="taskId"/>, reflecting the removed link. The return can be null if the API does not return content on success.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="linksToTaskId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if either of the tasks or the link between them does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove the link between these tasks.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CuTask?> DeleteTaskLinkAsync(
            string taskId,
            string linksToTaskId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
    }
}
