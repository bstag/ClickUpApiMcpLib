using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Time Tracking operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for creating, retrieving, updating, deleting, and managing time entries
    /// and their associated tags within a Workspace. It also includes functionality for starting and stopping timers.
    /// Covered API Endpoints (non-exhaustive list):
    /// - Get Time Entries: `GET /team/{team_id}/time_entries`
    /// - Create Time Entry: `POST /team/{team_id}/time_entries`
    /// - Get Single Time Entry: `GET /team/{team_id}/time_entries/{timer_id}`
    /// - Update Time Entry: `PUT /team/{team_id}/time_entries/{timer_id}`
    /// - Delete Time Entry: `DELETE /team/{team_id}/time_entries/{timer_id}`
    /// - Get Time Entry History: `GET /team/{team_id}/time_entries/{timer_id}/history`
    /// - Get Running Time Entry: `GET /team/{team_id}/time_entries/current`
    /// - Start Timer: `POST /team/{team_id}/time_entries/start`
    /// - Stop Timer: `POST /team/{team_id}/time_entries/stop`
    /// - Time Entry Tags: `GET /team/{team_id}/time_entries/tags`, `POST /team/{team_id}/time_entries/tags`, `DELETE /team/{team_id}/time_entries/tags`, `PUT /team/{team_id}/time_entries/tags`
    /// </remarks>
    public interface ITimeTrackingService
    {
        /// <summary>
        /// Retrieves a list of time entries for a specified Workspace (Team), with various filtering options.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="request">A <see cref="GetTimeEntriesRequest"/> object containing filter criteria such as date ranges, assignees, and task IDs.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="TimeEntry"/> objects matching the filter criteria.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="request"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access time entries for this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<Models.Common.Pagination.IPagedResult<TimeEntry>> GetTimeEntriesAsync(
            string workspaceId,
            GetTimeEntriesRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new time entry within the specified Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="createTimeEntryRequest">A <see cref="CreateTimeEntryRequest"/> object containing details for the new time entry, such as duration, start date, associated task ID, and description.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c> and a task ID (tid) is provided in the request, it is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamIdForCustomTaskIds">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c> and a task ID is provided in the request.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="TimeEntry"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createTimeEntryRequest"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamIdForCustomTaskIds"/> is not provided when a task ID is present in the request.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create time entries.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<TimeEntry> CreateTimeEntryAsync(
            string workspaceId,
            CreateTimeEntryRequest createTimeEntryRequest,
            bool? customTaskIds = null,
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a specific time entry by its ID from a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="timerId">The unique identifier of the time entry (timer_id) to retrieve.</param>
        /// <param name="includeTaskTags">Optional. If <c>true</c>, includes tags associated with the task in the response. Defaults to <c>false</c>.</param>
        /// <param name="includeLocationNames">Optional. If <c>true</c>, includes the names of the List, Folder, and Space associated with the task. Defaults to <c>false</c>.</param>
        /// <param name="includeApprovalHistory">Optional. If <c>true</c>, includes the approval history for the time entry. Defaults to <c>false</c>.</param>
        /// <param name="includeApprovalDetails">Optional. If <c>true</c>, includes detailed approval information. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the requested <see cref="TimeEntry"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="timerId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the time entry with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this time entry.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<TimeEntry> GetTimeEntryAsync(
            string workspaceId,
            string timerId,
            bool? includeTaskTags = null,
            bool? includeLocationNames = null,
            bool? includeApprovalHistory = null,
            bool? includeApprovalDetails = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing time entry in a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="timerId">The unique identifier of the time entry to update.</param>
        /// <param name="updateTimeEntryRequest">A <see cref="UpdateTimeEntryRequest"/> object containing the updated details for the time entry.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c> and a task ID (tid) is provided in the request, it is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamIdForCustomTaskIds">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c> and a task ID is provided in the request.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="TimeEntry"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="timerId"/>, or <paramref name="updateTimeEntryRequest"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamIdForCustomTaskIds"/> is not provided when a task ID is present in the request.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the time entry with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this time entry.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<TimeEntry> UpdateTimeEntryAsync(
            string workspaceId,
            string timerId,
            UpdateTimeEntryRequest updateTimeEntryRequest,
            bool? customTaskIds = null,
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a time entry from a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="timerId">The unique identifier of the time entry to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="timerId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the time entry with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this time entry.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteTimeEntryAsync(
            string workspaceId,
            string timerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the history of changes for a specific time entry in a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="timerId">The unique identifier of the time entry for which to retrieve history.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="TimeEntryHistory"/> records.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="timerId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the time entry does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<TimeEntryHistory>> GetTimeEntryHistoryAsync(
            string workspaceId,
            string timerId,
            CancellationToken cancellationToken = default);

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

        /// <summary>
        /// Retrieves all unique tags that have been used in time entries for a specific Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="TaskTag"/> objects representing the unique time entry tags.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access time entry tags.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<TaskTag>> GetAllTimeEntryTagsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds one or more specified tags to one or more time entries within a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="addTagsRequest">An <see cref="AddTagsFromTimeEntriesRequest"/> object containing a list of time entry IDs and the tags to be added to them.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="addTagsRequest"/> is null, or if the request contains no time entry IDs or tags.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify tags on time entries.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task AddTagsToTimeEntriesAsync(
            string workspaceId,
            AddTagsFromTimeEntriesRequest addTagsRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes one or more specified tags from one or more time entries within a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="removeTagsRequest">An <see cref="RemoveTagsFromTimeEntriesRequest"/> object containing a list of time entry IDs and the tags to be removed from them.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="removeTagsRequest"/> is null, or if the request contains no time entry IDs or tags.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify tags on time entries.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task RemoveTagsFromTimeEntriesAsync(
            string workspaceId,
            RemoveTagsFromTimeEntriesRequest removeTagsRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the name and color attributes of an existing time entry tag across an entire Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="changeTagNamesRequest">A <see cref="ChangeTagNamesFromTimeEntriesRequest"/> object specifying the current tag name and the new name and color attributes to apply.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="changeTagNamesRequest"/> is null, or if the tag name details are invalid.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the tag to be changed does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify time entry tags.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task ChangeTimeEntryTagNameAsync(
            string workspaceId,
            ChangeTagNamesFromTimeEntriesRequest changeTagNamesRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all time entries for a Workspace (Team), filtered by the provided request parameters, and automatically handles pagination using <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="request">A <see cref="GetTimeEntriesRequest"/> object containing filter criteria (excluding pagination parameters like 'page').</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>An asynchronous stream (<see cref="IAsyncEnumerable{TimeEntry}"/>) of <see cref="TimeEntry"/> objects matching the criteria.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="request"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access time entries.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures during pagination.</exception>
        IAsyncEnumerable<TimeEntry> GetTimeEntriesAsyncEnumerableAsync(
            string workspaceId,
            GetTimeEntriesRequest request,
            CancellationToken cancellationToken = default
        );
    }
}
