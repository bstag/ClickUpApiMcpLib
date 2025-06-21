using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.TimeTracking; // Assuming TimeEntry, TimeEntryHistory, TimeEntryTag are here
using ClickUp.Api.Client.Models.RequestModels.TimeTracking; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Time Tracking operations in the ClickUp API.
    /// This service provides methods for creating, retrieving, updating, deleting, and managing time entries and their associated tags.
    /// </summary>
    /// <remarks>
    /// Based on ClickUp API endpoints for time tracking, including:
    /// <list type="bullet">
    /// <item><description>GET /v2/team/{team_id}/time_entries</description></item>
    /// <item><description>POST /v2/team/{team_id}/time_entries</description></item>
    /// <item><description>GET /v2/team/{team_id}/time_entries/{timer_id}</description></item>
    /// <item><description>PUT /v2/team/{team_id}/time_entries/{timer_id}</description></item>
    /// <item><description>DELETE /v2/team/{team_id}/time_entries/{timer_id}</description></item>
    /// <item><description>GET /v2/team/{team_id}/time_entries/{timer_id}/history</description></item>
    /// <item><description>GET /v2/team/{team_id}/time_entries/current</description></item>
    /// <item><description>POST /v2/team/{team_id}/time_entries/start</description></item>
    /// <item><description>POST /v2/team/{team_id}/time_entries/stop</description></item>
    /// <item><description>GET /v2/team/{team_id}/time_entries/tags</description></item>
    /// <item><description>POST /v2/team/{team_id}/time_entries/tags</description></item>
    /// <item><description>DELETE /v2/team/{team_id}/time_entries/tags</description></item>
    /// <item><description>PUT /v2/team/{team_id}/time_entries/tags</description></item>
    /// </list>
    /// </remarks>
    public interface ITimeTrackingService
    {
        /// <summary>
        /// Retrieves time entries for a Workspace, filtered by the provided request parameters.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="request">A <see cref="GetTimeEntriesRequest"/> object containing filter criteria such as date ranges, assignees, and task IDs.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="TimeEntry"/> objects matching the criteria.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="request"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<IEnumerable<TimeEntry>> GetTimeEntriesAsync(
            string workspaceId,
            GetTimeEntriesRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new time entry in the specified Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="createTimeEntryRequest">A <see cref="CreateTimeEntryRequest"/> object containing details for the new time entry, such as duration, start date, and associated task ID.</param>
        /// <param name="customTaskIds">Optional. If true and a task ID (tid) is provided in the request, it's treated as a custom task ID. Requires <paramref name="teamIdForCustomTaskIds"/>.</param>
        /// <param name="teamIdForCustomTaskIds">Optional. The Workspace ID, required if <paramref name="customTaskIds"/> is true and a task ID is in the request.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="TimeEntry"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createTimeEntryRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<TimeEntry> CreateTimeEntryAsync(
            string workspaceId,
            CreateTimeEntryRequest createTimeEntryRequest,
            bool? customTaskIds = null,
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a specific time entry by its ID from a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="timerId">The ID of the time entry (timer_id).</param>
        /// <param name="includeTaskTags">Optional. If true, includes tags associated with the task in the response.</param>
        /// <param name="includeLocationNames">Optional. If true, includes names of the List, Folder, and Space.</param>
        /// <param name="includeApprovalHistory">Optional. If true, includes the approval history for the time entry.</param>
        /// <param name="includeApprovalDetails">Optional. If true, includes detailed approval information.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the requested <see cref="TimeEntry"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="timerId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as time entry not found.</exception>
        Task<TimeEntry> GetTimeEntryAsync(
            string workspaceId,
            string timerId,
            bool? includeTaskTags = null,
            bool? includeLocationNames = null,
            bool? includeApprovalHistory = null,
            bool? includeApprovalDetails = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing time entry in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="timerId">The ID of the time entry to update.</param>
        /// <param name="updateTimeEntryRequest">A <see cref="UpdateTimeEntryRequest"/> object containing the updated details for the time entry.</param>
        /// <param name="customTaskIds">Optional. If true and a task ID (tid) is provided in the request, it's treated as a custom task ID. Requires <paramref name="teamIdForCustomTaskIds"/>.</param>
        /// <param name="teamIdForCustomTaskIds">Optional. The Workspace ID, required if <paramref name="customTaskIds"/> is true and a task ID is in the request.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="TimeEntry"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="timerId"/>, or <paramref name="updateTimeEntryRequest"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<TimeEntry> UpdateTimeEntryAsync(
            string workspaceId,
            string timerId,
            UpdateTimeEntryRequest updateTimeEntryRequest,
            bool? customTaskIds = null,
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a time entry from a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="timerId">The ID of the time entry to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="timerId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        System.Threading.Tasks.Task DeleteTimeEntryAsync(
            string workspaceId,
            string timerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the history of changes for a specific time entry in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="timerId">The ID of the time entry.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="TimeEntryHistory"/> records.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="timerId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<IEnumerable<TimeEntryHistory>> GetTimeEntryHistoryAsync(
            string workspaceId,
            string timerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the currently running time entry for the authenticated user or a specified assignee within a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="assigneeUserId">Optional. The user ID of the assignee whose running timer is to be retrieved. Only available to Workspace Owners/Admins.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the currently running <see cref="TimeEntry"/>, or null if no timer is running for the user.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<TimeEntry?> GetRunningTimeEntryAsync(
            string workspaceId,
            string? assigneeUserId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts a new time entry (timer) in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="startTimeEntryRequest">A <see cref="StartTimeEntryRequest"/> object containing details for the timer, such as an optional task ID (tid), description, and tags.</param>
        /// <param name="customTaskIds">Optional. If true and a task ID (tid) is provided in the request, it's treated as a custom task ID. Requires <paramref name="teamIdForCustomTaskIds"/>.</param>
        /// <param name="teamIdForCustomTaskIds">Optional. The Workspace ID, required if <paramref name="customTaskIds"/> is true and a task ID is in the request.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the started <see cref="TimeEntry"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="startTimeEntryRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as when another timer is already running.</exception>
        Task<TimeEntry> StartTimeEntryAsync(
            string workspaceId,
            StartTimeEntryRequest startTimeEntryRequest,
            bool? customTaskIds = null,
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops the currently running time entry for the authenticated user in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the stopped <see cref="TimeEntry"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as no timer currently running.</exception>
        Task<TimeEntry> StopTimeEntryAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all unique tags used in time entries for a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="TaskTag"/> objects representing the unique time entry tags.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<IEnumerable<TaskTag>> GetAllTimeEntryTagsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds specified tags to one or more time entries in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="addTagsRequest">An <see cref="AddTagsFromTimeEntriesRequest"/> object containing a list of time entry IDs and the tags to add.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="addTagsRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        System.Threading.Tasks.Task AddTagsToTimeEntriesAsync(
            string workspaceId,
            AddTagsFromTimeEntriesRequest addTagsRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes specified tags from one or more time entries in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="removeTagsRequest">An <see cref="RemoveTagsFromTimeEntriesRequest"/> object containing a list of time entry IDs and the tags to remove.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="removeTagsRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        System.Threading.Tasks.Task RemoveTagsFromTimeEntriesAsync(
            string workspaceId,
            RemoveTagsFromTimeEntriesRequest removeTagsRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the name and colors of an existing time entry tag across an entire Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="changeTagNamesRequest">A <see cref="ChangeTagNamesFromTimeEntriesRequest"/> object specifying the current tag name and the new name and colors.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="changeTagNamesRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        System.Threading.Tasks.Task ChangeTimeEntryTagNameAsync(
            string workspaceId,
            ChangeTagNamesFromTimeEntriesRequest changeTagNamesRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all time entries for a Workspace, filtered by the provided request parameters, automatically handling pagination.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="request">A <see cref="GetTimeEntriesRequest"/> object containing filter criteria (excluding pagination parameters like 'page').</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation, also used for <see cref="System.Runtime.CompilerServices.EnumeratorCancellationAttribute"/>.</param>
        /// <returns>An asynchronous stream (<see cref="IAsyncEnumerable{TimeEntry}"/>) of <see cref="TimeEntry"/> objects matching the criteria.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="request"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors during pagination.</exception>
        IAsyncEnumerable<TimeEntry> GetTimeEntriesAsyncEnumerableAsync(
            string workspaceId,
            GetTimeEntriesRequest request, // This DTO should not contain 'page'
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default
        );
    }
}
