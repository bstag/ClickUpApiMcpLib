using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines operations for managing task relationships and interactions.
    /// </summary>
    public interface ITaskRelations
    {
        /// <summary>
        /// Merges a list of source Tasks into a single target Task. The source tasks will typically be deleted or closed after merging.
        /// </summary>
        /// <param name="targetTaskId">The unique identifier of the target Task into which other Tasks will be merged.</param>
        /// <param name="mergeTasksRequest">An object containing a list of source Task IDs to be merged into the target Task.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated target <see cref="CuTask"/>, reflecting the merge.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="targetTaskId"/> or <paramref name="mergeTasksRequest"/> is null, or if the list of source task IDs in the request is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the target Task or any of the source Tasks do not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to merge these Tasks.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        /// <remarks>The ClickUp API endpoint used for this operation (POST /task/{task_id}/merge) does not support custom task IDs.</remarks>
        Task<CuTask> MergeTasksAsync(
            string targetTaskId,
            MergeTasksRequest mergeTasksRequest,
            CancellationToken cancellationToken = default);
    }
}