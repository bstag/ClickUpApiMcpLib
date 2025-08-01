using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.RequestModels.TaskRelationships;

namespace ClickUp.Api.Client.Abstractions.Services.TaskRelationships
{
    /// <summary>
    /// Service interface for ClickUp Task Dependency operations.
    /// Handles dependency relationships between tasks (blocking/blocked by relationships).
    /// </summary>
    public interface ITaskDependencyManager
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
        /// The details of which dependency to remove and options for custom task ID handling are specified in the <paramref name="requestModel"/>.
        /// </summary>
        /// <param name="taskId">The unique identifier of the primary task from which the dependency is being removed.</param>
        /// <param name="requestModel">An object containing parameters like 'dependsOnTaskId' or 'dependencyOfTaskId' to specify the relationship, and options for 'customTaskIds' and 'teamId'.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="requestModel"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="requestModel"/> is invalid (e.g., specifies both or neither of 'dependsOnTaskId' and 'dependencyOfTaskId', or 'customTaskIds' is true but 'teamId' is not provided).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if any of the specified tasks or the dependency relationship does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify dependencies for these tasks.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteDependencyAsync(
            string taskId,
            DeleteDependencyRequest requestModel,
            CancellationToken cancellationToken = default);
    }
}