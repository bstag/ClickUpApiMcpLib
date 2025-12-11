using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Parameters;

namespace ClickUp.Api.Client.Abstractions.Services.TimeTracking
{
    /// <summary>
    /// Interface for managing ClickUp Time Entry operations.
    /// Handles CRUD operations for time entries within a Workspace.
    /// </summary>
    /// <remarks>
    /// This interface focuses on time entry management operations including:
    /// - Retrieving time entries with filtering
    /// - Creating new time entries
    /// - Getting individual time entries
    /// - Updating existing time entries
    /// - Deleting time entries
    /// - Getting time entry history
    /// - Async enumerable support for paginated results
    /// </remarks>
    public interface ITimeEntryManager
    {
        /// <summary>
        /// Retrieves a list of time entries for a specified Workspace (Team), with various filtering options.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="configureParameters">An action to configure the <see cref="GetTimeEntriesRequestParameters"/> for filtering.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IPagedResult{TimeEntry}"/> object with time entries matching the filter criteria.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access time entries for this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<IPagedResult<TimeEntry>> GetTimeEntriesAsync(
            string workspaceId,
            Action<GetTimeEntriesRequestParameters>? configureParameters = null,
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
        /// Retrieves all time entries for a Workspace (Team), filtered by the provided request parameters, and automatically handles pagination using <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="parameters">A <see cref="GetTimeEntriesRequestParameters"/> object containing filter criteria. The <see cref="GetTimeEntriesRequestParameters.Page"/> property will be ignored and managed internally for pagination.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>An asynchronous stream (<see cref="IAsyncEnumerable{TimeEntry}"/>) of <see cref="TimeEntry"/> objects matching the criteria.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="parameters"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access time entries.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures during pagination.</exception>
        IAsyncEnumerable<TimeEntry> GetTimeEntriesAsyncEnumerableAsync(
            string workspaceId,
            GetTimeEntriesRequestParameters parameters,
            CancellationToken cancellationToken = default);
    }
}