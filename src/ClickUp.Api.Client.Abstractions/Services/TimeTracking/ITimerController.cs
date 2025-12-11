using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;

namespace ClickUp.Api.Client.Abstractions.Services.TimeTracking
{
    /// <summary>
    /// Interface for managing ClickUp Timer operations.
    /// Handles timer control operations including starting, stopping, and retrieving running timers.
    /// </summary>
    /// <remarks>
    /// This interface focuses on timer control operations including:
    /// - Getting currently running time entries
    /// - Starting new timers
    /// - Stopping running timers
    /// </remarks>
    public interface ITimerController
    {
        /// <summary>
        /// Retrieves the currently running time entry for the authenticated user or, if authorized, for a specified assignee within a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="assigneeUserId">Optional. The user ID of the assignee whose running timer is to be retrieved. This is typically only available to Workspace Owners/Admins.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the currently running <see cref="TimeEntry"/>, or <c>null</c> if no timer is currently running for the specified user.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information (e.g., requesting another user's timer without admin rights).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<TimeEntry?> GetRunningTimeEntryAsync(
            string workspaceId,
            string? assigneeUserId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts a new time entry (timer) in a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="startTimeEntryRequest">A <see cref="StartTimeEntryRequest"/> object containing details for the timer, such as an optional task ID (tid), description, and tags.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c> and a task ID (tid) is provided in the request, it is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamIdForCustomTaskIds">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c> and a task ID is provided in the request.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the started <see cref="TimeEntry"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="startTimeEntryRequest"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamIdForCustomTaskIds"/> is not provided when a task ID is present in the request.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to start time entries.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as when another timer is already running for the user.</exception>
        Task<TimeEntry> StartTimeEntryAsync(
            string workspaceId,
            StartTimeEntryRequest startTimeEntryRequest,
            bool? customTaskIds = null,
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops the currently running time entry for the authenticated user in a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the stopped <see cref="TimeEntry"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to stop time entries.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as if no timer is currently running for the user.</exception>
        Task<TimeEntry> StopTimeEntryAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);
    }
}