using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{

    // Represents the Time Tracking operations in the ClickUp API
    // Based on endpoints under the "Time Tracking" tag

    public interface ITimeTrackingService
    {
        /// <summary>
        /// Retrieves time entries within a specified date range for a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_Id).</param>
        /// <param name="startDate">Optional. Unix time in milliseconds for the start of the range.</param>
        /// <param name="endDate">Optional. Unix time in milliseconds for the end of the range.</param>
        /// <param name="assignee">Optional. Filter by user ID(s).</param>
        /// <param name="includeTaskTags">Optional. Whether to include task tags.</param>
        /// <param name="includeLocationNames">Optional. Whether to include List, Folder, and Space names.</param>
        /// <param name="spaceId">Optional. Filter by Space ID.</param>
        /// <param name="folderId">Optional. Filter by Folder ID.</param>
        /// <param name="listId">Optional. Filter by List ID.</param>
        /// <param name="taskId">Optional. Filter by Task ID.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamIdForCustomTaskIds">Optional. Workspace ID, required if customTaskIds is true for task_id.</param>
        /// <param name="isBillable">Optional. Filter by billable status.</param>
        /// <returns>A list of time entries.</returns>
        Task<IEnumerable<object>> GetTimeEntriesAsync(double workspaceId, double? startDate = null, double? endDate = null, string? assignee = null, bool? includeTaskTags = null, bool? includeLocationNames = null, bool? includeApprovalHistory = null, bool? includeApprovalDetails = null, double? spaceId = null, double? folderId = null, double? listId = null, string? taskId = null, bool? customTaskIds = null, double? teamIdForCustomTaskIds = null, bool? isBillable = null);
        // Note: Return type should be IEnumerable<TimeEntryDto>. Consider a request DTO for all optional parameters.

        /// <summary>
        /// Creates a new time entry.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_Id).</param>
        /// <param name="createTimeEntryRequest">Details of the time entry to create.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id (if tid is provided).</param>
        /// <param name="teamIdForCustomTaskIds">Optional. Workspace ID, required if customTaskIds is true for tid.</param>
        /// <returns>The created time entry.</returns>
        Task<object> CreateTimeEntryAsync(double workspaceId, object createTimeEntryRequest, bool? customTaskIds = null, double? teamIdForCustomTaskIds = null);
        // Note: createTimeEntryRequest should be CreateTimeEntryRequest, return type should be TimeEntryDto (or a specific create response DTO).

        /// <summary>
        /// Retrieves a specific time entry.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="timerId">The ID of the time entry (timer_id).</param>
        /// <param name="includeTaskTags">Optional. Whether to include task tags.</param>
        /// <param name="includeLocationNames">Optional. Whether to include List, Folder, and Space names.</param>
        /// <returns>Details of the time entry.</returns>
        Task<object> GetTimeEntryAsync(double workspaceId, string timerId, bool? includeTaskTags = null, bool? includeLocationNames = null, bool? includeApprovalHistory = null, bool? includeApprovalDetails = null);
        // Note: Return type should be TimeEntryDto.

        /// <summary>
        /// Updates a time entry.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="timerId">The ID of the time entry (timer_id).</param>
        /// <param name="updateTimeEntryRequest">Details for updating the time entry.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id (if tid is provided).</param>
        /// <param name="teamIdForCustomTaskIds">Optional. Workspace ID, required if customTaskIds is true for tid.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The API returns 200 with an empty object or updated time entry details.</returns>
        Task<object> UpdateTimeEntryAsync(double workspaceId, string timerId, object updateTimeEntryRequest, bool? customTaskIds = null, double? teamIdForCustomTaskIds = null);
        // Note: updateTimeEntryRequest should be UpdateTimeEntryRequest. The response seems to be the updated time entry.

        /// <summary>
        /// Deletes a time entry.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="timerId">The ID of the time entry (timer_id) to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteTimeEntryAsync(double workspaceId, string timerId);
        // Note: API returns 200 with an empty object in some cases, or the deleted object. Check spec for specifics. For simplicity, Task if empty.

        /// <summary>
        /// Retrieves the history of changes for a specific time entry.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="timerId">The ID of the time entry (timer_id).</param>
        /// <returns>A list of history records for the time entry.</returns>
        Task<IEnumerable<object>> GetTimeEntryHistoryAsync(double workspaceId, string timerId);
        // Note: Return type should be IEnumerable<TimeEntryHistoryDto>.

        /// <summary>
        /// Retrieves the currently running time entry for the authenticated user (or specified assignee).
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="assignee">Optional. User ID to get running timer for. Only for Workspace Owners/Admins.</param>
        /// <returns>The currently running time entry, or null if none.</returns>
        Task<object?> GetRunningTimeEntryAsync(double workspaceId, double? assignee = null);
        // Note: Return type should be RunningTimeEntryDto or a similar DTO. Nullable if no timer is running.

        /// <summary>
        /// Starts a new time entry (timer).
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_Id).</param>
        /// <param name="startTimeEntryRequest">Details for starting the timer, such as task ID (tid), description, tags.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id (if tid is provided).</param>
        /// <param name="teamIdForCustomTaskIds">Optional. Workspace ID, required if customTaskIds is true for tid.</param>
        /// <returns>Details of the started time entry.</returns>
        Task<object> StartTimeEntryAsync(double workspaceId, object startTimeEntryRequest, bool? customTaskIds = null, double? teamIdForCustomTaskIds = null);
        // Note: startTimeEntryRequest should be StartTimeEntryRequest. Return type should be a DTO for the running timer.

        /// <summary>
        /// Stops the currently running time entry for the authenticated user.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <returns>Details of the stopped time entry.</returns>
        Task<object> StopTimeEntryAsync(double workspaceId);
        // Note: Return type should be a DTO representing the stopped time entry.

        /// <summary>
        /// Retrieves all tags used in time entries for a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <returns>A list of time entry tags.</returns>
        Task<IEnumerable<object>> GetAllTimeEntryTagsAsync(double workspaceId);
        // Note: Return type should be IEnumerable<TimeEntryTagDto>.

        /// <summary>
        /// Adds tags to specified time entries.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="tagsRequest">Request containing time entry IDs and tags to add.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task AddTagsToTimeEntriesAsync(double workspaceId, object tagsRequest);
        // Note: tagsRequest should be TimeEntryTagsRequest. API returns 200 with an empty object.

        /// <summary>
        /// Removes tags from specified time entries.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="tagsRequest">Request containing time entry IDs and tags to remove.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task RemoveTagsFromTimeEntriesAsync(double workspaceId, object tagsRequest);
        // Note: tagsRequest should be TimeEntryTagsRequest. API returns 200 with an empty object.

        /// <summary>
        /// Changes the name and colors of a time entry tag across a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="changeTagNameRequest">Details for changing the tag name and colors.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task ChangeTimeEntryTagNameAsync(double workspaceId, object changeTagNameRequest);
        // Note: changeTagNameRequest should be ChangeTagNameRequest. API returns 200 with an empty object.
    }
}
