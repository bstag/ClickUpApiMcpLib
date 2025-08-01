using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines the contract for task relationship operations such as merging tasks.
    /// Follows the Single Responsibility Principle by focusing only on task relationship management.
    /// </summary>
    public interface ITaskRelationshipService
    {
        /// <summary>
        /// Merges multiple source tasks into a target task.
        /// </summary>
        /// <param name="targetTaskId">The ID of the target task to merge into.</param>
        /// <param name="mergeTasksRequest">The merge request containing source task IDs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated target task after merging.</returns>
        /// <remarks>
        /// The ClickUp API merges tasks one by one. Each source task is merged into the target task
        /// using the POST /task/{source_task_id}/merge endpoint. Custom Task IDs are not supported
        /// for this operation.
        /// </remarks>
        Task<CuTask> MergeTasksAsync(
            string targetTaskId,
            MergeTasksRequest mergeTasksRequest,
            CancellationToken cancellationToken = default);
    }
}