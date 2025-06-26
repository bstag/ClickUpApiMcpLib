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

    public TimeEntriesFluentGetRequest GetTimeEntries(string workspaceId)
    {
        return new TimeEntriesFluentGetRequest(workspaceId, _timeTrackingService);
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
