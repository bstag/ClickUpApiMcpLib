using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.TaskRelationships;

namespace ClickUp.Api.Client.Abstractions.Services.TaskRelationships
{
    /// <summary>
    /// Service interface for ClickUp Task Link operations.
    /// Handles non-dependent link relationships between tasks (related but not blocking relationships).
    /// </summary>
    public interface ITaskLinkManager
    {
        /// <summary>
        /// Creates a non-dependent link between two tasks, indicating they are related.
        /// </summary>
        /// <param name="taskId">The unique identifier of the first task in the link.</param>
        /// <param name="linksToTaskId">The unique identifier of the second task to be linked with the first task.</param>
        /// <param name="requestModel">An object containing options for custom task ID handling ('customTaskIds', 'teamId').</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="CuTask"/> object, typically for the <paramref name="taskId"/>, reflecting the new link. The return can be null if the API does not return content on success.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/>, <paramref name="linksToTaskId"/>, or <paramref name="requestModel"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="requestModel"/> specifies 'customTaskIds' as true but 'teamId' is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if either of the tasks with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to link these tasks.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CuTask?> AddTaskLinkAsync(
            string taskId,
            string linksToTaskId,
            AddTaskLinkRequest requestModel,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an existing non-dependent link between two tasks.
        /// </summary>
        /// <param name="taskId">The unique identifier of the first task in the link.</param>
        /// <param name="linksToTaskId">The unique identifier of the second task that is currently linked to the first task.</param>
        /// <param name="requestModel">An object containing options for custom task ID handling ('customTaskIds', 'teamId').</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="CuTask"/> object, typically for the <paramref name="taskId"/>, reflecting the removed link. The return can be null if the API does not return content on success.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/>, <paramref name="linksToTaskId"/>, or <paramref name="requestModel"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="requestModel"/> specifies 'customTaskIds' as true but 'teamId' is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if either of the tasks or the link between them does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove the link between these tasks.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CuTask?> DeleteTaskLinkAsync(
            string taskId,
            string linksToTaskId,
            DeleteTaskLinkRequest requestModel,
            CancellationToken cancellationToken = default);
    }
}