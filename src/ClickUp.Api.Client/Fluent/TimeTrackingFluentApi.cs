using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using ClickUp.Api.Client.Models.ResponseModels.TimeTracking;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TimeTrackingFluentApi
{
    private readonly ITimeTrackingService _timeTrackingService;

    public TimeTrackingFluentApi(ITimeTrackingService timeTrackingService)
    {
        _timeTrackingService = timeTrackingService;
    }

    public TimeEntriesFluentQueryRequest GetTimeEntries(string workspaceId)
    {
        return new TimeEntriesFluentQueryRequest(workspaceId, _timeTrackingService);
    }

    public TimeEntryFluentCreateRequest CreateTimeEntry(string workspaceId)
    {
        return new TimeEntryFluentCreateRequest(workspaceId, _timeTrackingService);
    }

    public async Task<TimeEntry> GetTimeEntryAsync(string workspaceId, string timerId, bool? includeTaskTags = null, bool? includeLocationNames = null, bool? includeApprovalHistory = null, bool? includeApprovalDetails = null, CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.GetTimeEntryAsync(workspaceId, timerId, includeTaskTags, includeLocationNames, includeApprovalHistory, includeApprovalDetails, cancellationToken);
    }

    public TimeEntryFluentUpdateRequest UpdateTimeEntry(string workspaceId, string timerId)
    {
        return new TimeEntryFluentUpdateRequest(workspaceId, timerId, _timeTrackingService);
    }

    public async Task DeleteTimeEntryAsync(string workspaceId, string timerId, CancellationToken cancellationToken = default)
    {
        await _timeTrackingService.DeleteTimeEntryAsync(workspaceId, timerId, cancellationToken);
    }

    public async Task<IEnumerable<TimeEntryHistory>> GetTimeEntryHistoryAsync(string workspaceId, string timerId, CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.GetTimeEntryHistoryAsync(workspaceId, timerId, cancellationToken);
    }

    /// <summary>
    /// Retrieves the history for a specific time entry asynchronously.
    /// While this method returns an IAsyncEnumerable, the underlying ClickUp API for time entry history
    /// does not appear to be paginated, so all history records are typically fetched in a single call by the service.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace (team).</param>
    /// <param name="timerId">The ID of the time entry (timer).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="TimeEntryHistory"/>.</returns>
    public async IAsyncEnumerable<TimeEntryHistory> GetTimeEntryHistoryAsyncEnumerableAsync(
        string workspaceId,
        string timerId,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var historyEntries = await _timeTrackingService.GetTimeEntryHistoryAsync(workspaceId, timerId, cancellationToken).ConfigureAwait(false);
        if (historyEntries != null)
        {
            foreach (var entry in historyEntries)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return entry;
            }
        }
    }

    public async Task<TimeEntry?> GetRunningTimeEntryAsync(string workspaceId, string? assigneeUserId = null, CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.GetRunningTimeEntryAsync(workspaceId, assigneeUserId, cancellationToken);
    }

    public TimeEntryFluentStartRequest StartTimeEntry(string workspaceId)
    {
        return new TimeEntryFluentStartRequest(workspaceId, _timeTrackingService);
    }

    public async Task<TimeEntry> StopTimeEntryAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.StopTimeEntryAsync(workspaceId, cancellationToken);
    }

    public async Task<IEnumerable<TaskTag>> GetAllTimeEntryTagsAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.GetAllTimeEntryTagsAsync(workspaceId, cancellationToken);
    }

    public TimeEntriesFluentAddTagsRequest AddTagsToTimeEntries(string workspaceId)
    {
        return new TimeEntriesFluentAddTagsRequest(workspaceId, _timeTrackingService);
    }

    public TimeEntriesFluentRemoveTagsRequest RemoveTagsFromTimeEntries(string workspaceId)
    {
        return new TimeEntriesFluentRemoveTagsRequest(workspaceId, _timeTrackingService);
    }

    public TimeEntriesFluentChangeTagNamesRequest ChangeTimeEntryTagName(string workspaceId)
    {
        return new TimeEntriesFluentChangeTagNamesRequest(workspaceId, _timeTrackingService);
    }
}
