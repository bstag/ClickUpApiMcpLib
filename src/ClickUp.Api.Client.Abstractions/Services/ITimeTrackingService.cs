using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities; // Assuming TimeEntry, TimeEntryHistory, TimeEntryTag are here
using ClickUp.Api.Client.Models.RequestModels.TimeTracking; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Time Tracking operations in the ClickUp API.
    /// </summary>
    public interface ITimeTrackingService
    {
        /// <summary>
        /// Retrieves time entries within a specified date range for a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="request">Request DTO containing all filtering options like date range, assignee, task filters, etc.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="TimeEntry"/> objects.</returns>
        Task<IEnumerable<TimeEntry>> GetTimeEntriesAsync(
            string workspaceId,
            GetTimeEntriesRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new time entry.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="createTimeEntryRequest">Details of the time entry to create.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id (if tid is provided in request).</param>
        /// <param name="teamIdForCustomTaskIds">Optional. Workspace ID, required if customTaskIds is true for tid in request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="TimeEntry"/>.</returns>
        Task<TimeEntry> CreateTimeEntryAsync(
            string workspaceId,
            CreateTimeEntryRequest createTimeEntryRequest,
            bool? customTaskIds = null,
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a specific time entry.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="timerId">The ID of the time entry.</param>
        /// <param name="includeTaskTags">Optional. Whether to include task tags.</param>
        /// <param name="includeLocationNames">Optional. Whether to include List, Folder, and Space names.</param>
        /// <param name="includeApprovalHistory">Optional. Whether to include approval history.</param>
        /// <param name="includeApprovalDetails">Optional. Whether to include approval details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="TimeEntry"/>.</returns>
        Task<TimeEntry> GetTimeEntryAsync(
            string workspaceId,
            string timerId,
            bool? includeTaskTags = null,
            bool? includeLocationNames = null,
            bool? includeApprovalHistory = null,
            bool? includeApprovalDetails = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a time entry.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="timerId">The ID of the time entry.</param>
        /// <param name="updateTimeEntryRequest">Details for updating the time entry.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id (if tid is provided in request).</param>
        /// <param name="teamIdForCustomTaskIds">Optional. Workspace ID, required if customTaskIds is true for tid in request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="TimeEntry"/>.</returns>
        Task<TimeEntry> UpdateTimeEntryAsync(
            string workspaceId,
            string timerId,
            UpdateTimeEntryRequest updateTimeEntryRequest,
            bool? customTaskIds = null,
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a time entry.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="timerId">The ID of the time entry to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task DeleteTimeEntryAsync(
            string workspaceId,
            string timerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the history of changes for a specific time entry.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="timerId">The ID of the time entry.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="TimeEntryHistory"/> records for the time entry.</returns>
        Task<IEnumerable<TimeEntryHistory>> GetTimeEntryHistoryAsync(
            string workspaceId,
            string timerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the currently running time entry for the authenticated user (or specified assignee).
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="assigneeUserId">Optional. User ID to get running timer for. Only for Workspace Owners/Admins.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The currently running <see cref="TimeEntry"/>, or null if none.</returns>
        Task<TimeEntry?> GetRunningTimeEntryAsync(
            string workspaceId,
            string? assigneeUserId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts a new time entry (timer).
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="startTimeEntryRequest">Details for starting the timer, such as task ID (tid), description, tags.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id (if tid is provided in request).</param>
        /// <param name="teamIdForCustomTaskIds">Optional. Workspace ID, required if customTaskIds is true for tid in request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the started <see cref="TimeEntry"/>.</returns>
        Task<TimeEntry> StartTimeEntryAsync(
            string workspaceId,
            StartTimeEntryRequest startTimeEntryRequest,
            bool? customTaskIds = null,
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops the currently running time entry for the authenticated user.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the stopped <see cref="TimeEntry"/>.</returns>
        Task<TimeEntry> StopTimeEntryAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all tags used in time entries for a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="TimeEntryTag"/> objects.</returns>
        Task<IEnumerable<TimeEntryTag>> GetAllTimeEntryTagsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds tags to specified time entries.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="addTagsRequest">Request containing time entry IDs and tags to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task AddTagsToTimeEntriesAsync(
            string workspaceId,
            TimeEntryTagsRequest addTagsRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes tags from specified time entries.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="removeTagsRequest">Request containing time entry IDs and tags to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task RemoveTagsFromTimeEntriesAsync(
            string workspaceId,
            TimeEntryTagsRequest removeTagsRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the name and colors of a time entry tag across a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="changeTagNameRequest">Details for changing the tag name and colors.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task ChangeTimeEntryTagNameAsync(
            string workspaceId,
            ChangeTimeEntryTagNameRequest changeTagNameRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all time entries within a specified date range for a Workspace, automatically handling pagination.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="request">Request DTO containing all filtering options like date range, assignee, task filters, etc. (excluding page).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An asynchronous stream of <see cref="TimeEntry"/> objects.</returns>
        IAsyncEnumerable<TimeEntry> GetTimeEntriesAsyncEnumerableAsync(
            string workspaceId,
            GetTimeEntriesRequest request, // This DTO should not contain 'page'
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default
        );
    }
}
