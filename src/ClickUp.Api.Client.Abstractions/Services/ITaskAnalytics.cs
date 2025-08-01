using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines operations for task analytics, reporting, and time tracking.
    /// </summary>
    public interface ITaskAnalytics
    {
        /// <summary>
        /// Retrieves the amount of time a specific Task has spent in each status.
        /// </summary>
        /// <param name="taskId">The unique identifier of the Task.</param>
        /// <param name="requestModel">An object containing options for custom task ID handling.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="TaskTimeInStatusResponse"/> object with the time-in-status data for the Task.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="requestModel"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="requestModel"/> specifies CustomTaskIds as true but TeamId is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Task with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information for the Task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<TaskTimeInStatusResponse> GetTaskTimeInStatusAsync(
            string taskId,
            GetTaskTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the time spent in each status for a bulk list of Tasks.
        /// </summary>
        /// <param name="requestModel">An object containing a collection of Task IDs and options for custom task ID handling. The API expects a comma-separated string of up to 100 task IDs.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetBulkTasksTimeInStatusResponse"/> object with time-in-status data for the requested Tasks.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="requestModel"/> or its TaskIds collection is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="requestModel"/> specifies CustomTaskIds as true but TeamId is not provided, or if the number of task IDs exceeds the API limit (e.g., 100).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<GetBulkTasksTimeInStatusResponse> GetBulkTasksTimeInStatusAsync(
            GetBulkTasksTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default);
    }
}