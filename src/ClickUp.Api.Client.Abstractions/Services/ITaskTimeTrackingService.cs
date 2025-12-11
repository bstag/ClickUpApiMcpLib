using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines the contract for time tracking and time-related operations on ClickUp tasks.
    /// Follows the Single Responsibility Principle by focusing only on task time tracking functionality.
    /// </summary>
    public interface ITaskTimeTrackingService
    {
        /// <summary>
        /// Gets the time a task has spent in each status.
        /// </summary>
        /// <param name="taskId">The ID of the task to get time in status for.</param>
        /// <param name="requestModel">The request parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task time in status response.</returns>
        Task<TaskTimeInStatusResponse> GetTaskTimeInStatusAsync(
            string taskId,
            GetTaskTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the time multiple tasks have spent in each status in bulk.
        /// </summary>
        /// <param name="requestModel">The bulk request parameters containing task IDs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The bulk tasks time in status response.</returns>
        Task<GetBulkTasksTimeInStatusResponse> GetBulkTasksTimeInStatusAsync(
            GetBulkTasksTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default);
    }
}