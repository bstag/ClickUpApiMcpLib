using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using ClickUp.Api.Client.Models.ResponseModels.TimeTracking;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentTimeTrackingApi
{
    private readonly ITimeTrackingService _timeTrackingService;

    public FluentTimeTrackingApi(ITimeTrackingService timeTrackingService)
    {
        _timeTrackingService = timeTrackingService;
    }

    public FluentGetTimeEntriesRequest GetTimeEntries(string workspaceId)
    {
        return new FluentGetTimeEntriesRequest(workspaceId, _timeTrackingService);
    }

    public FluentCreateTimeEntryRequest CreateTimeEntry(string workspaceId)
    {
        return new FluentCreateTimeEntryRequest(workspaceId, _timeTrackingService);
    }

    public async Task<TimeEntry> GetTimeEntryAsync(string workspaceId, string timerId, bool? includeTaskTags = null, bool? includeLocationNames = null, bool? includeApprovalHistory = null, bool? includeApprovalDetails = null, CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.GetTimeEntryAsync(workspaceId, timerId, includeTaskTags, includeLocationNames, includeApprovalHistory, includeApprovalDetails, cancellationToken);
    }

    public FluentUpdateTimeEntryRequest UpdateTimeEntry(string workspaceId, string timerId)
    {
        return new FluentUpdateTimeEntryRequest(workspaceId, timerId, _timeTrackingService);
    }

    public async Task DeleteTimeEntryAsync(string workspaceId, string timerId, CancellationToken cancellationToken = default)
    {
        await _timeTrackingService.DeleteTimeEntryAsync(workspaceId, timerId, cancellationToken);
    }

    public async Task<IEnumerable<TimeEntryHistory>> GetTimeEntryHistoryAsync(string workspaceId, string timerId, CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.GetTimeEntryHistoryAsync(workspaceId, timerId, cancellationToken);
    }

    public async Task<TimeEntry?> GetRunningTimeEntryAsync(string workspaceId, string? assigneeUserId = null, CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.GetRunningTimeEntryAsync(workspaceId, assigneeUserId, cancellationToken);
    }

    public FluentStartTimeEntryRequest StartTimeEntry(string workspaceId)
    {
        return new FluentStartTimeEntryRequest(workspaceId, _timeTrackingService);
    }

    public async Task<TimeEntry> StopTimeEntryAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.StopTimeEntryAsync(workspaceId, cancellationToken);
    }

    public async Task<IEnumerable<TaskTag>> GetAllTimeEntryTagsAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.GetAllTimeEntryTagsAsync(workspaceId, cancellationToken);
    }

    public FluentAddTagsFromTimeEntriesRequest AddTagsToTimeEntries(string workspaceId)
    {
        return new FluentAddTagsFromTimeEntriesRequest(workspaceId, _timeTrackingService);
    }

    public FluentRemoveTagsFromTimeEntriesRequest RemoveTagsFromTimeEntries(string workspaceId)
    {
        return new FluentRemoveTagsFromTimeEntriesRequest(workspaceId, _timeTrackingService);
    }

    public FluentChangeTagNamesFromTimeEntriesRequest ChangeTimeEntryTagName(string workspaceId)
    {
        return new FluentChangeTagNamesFromTimeEntriesRequest(workspaceId, _timeTrackingService);
    }
}
