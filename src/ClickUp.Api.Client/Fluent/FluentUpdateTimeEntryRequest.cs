using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentUpdateTimeEntryRequest
{
    private string? _taskId;
    private string? _description;
    private List<string>? _tags;
    private long? _start;
    private long? _end;
    private int? _duration; // Duration in milliseconds
    private bool? _billable;
    private int? _assignee;
    private bool? _isLocked;
    private string? _tagAction;

    private readonly string _workspaceId;
    private readonly string _timerId;
    private readonly ITimeTrackingService _timeTrackingService;

    public FluentUpdateTimeEntryRequest(string workspaceId, string timerId, ITimeTrackingService timeTrackingService)
    {
        _workspaceId = workspaceId;
        _timerId = timerId;
        _timeTrackingService = timeTrackingService;
    }

    public FluentUpdateTimeEntryRequest WithTaskId(string taskId)
    {
        _taskId = taskId;
        return this;
    }

    public FluentUpdateTimeEntryRequest WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public FluentUpdateTimeEntryRequest WithTags(List<string> tags)
    {
        _tags = tags;
        return this;
    }

    public FluentUpdateTimeEntryRequest WithStart(long start)
    {
        _start = start;
        return this;
    }

    public FluentUpdateTimeEntryRequest WithEnd(long end)
    {
        _end = end;
        return this;
    }

    public FluentUpdateTimeEntryRequest WithDuration(int duration)
    {
        _duration = duration;
        return this;
    }

    public FluentUpdateTimeEntryRequest WithBillable(bool billable)
    {
        _billable = billable;
        return this;
    }

    public FluentUpdateTimeEntryRequest WithAssignee(int assignee)
    {
        _assignee = assignee;
        return this;
    }

    public FluentUpdateTimeEntryRequest WithIsLocked(bool isLocked)
    {
        _isLocked = isLocked;
        return this;
    }

    public FluentUpdateTimeEntryRequest WithTagAction(string tagAction)
    {
        _tagAction = tagAction;
        return this;
    }

    public async Task<TimeEntry> UpdateAsync(bool? customTaskIds = null, string? teamIdForCustomTaskIds = null, CancellationToken cancellationToken = default)
    {
        var updateTimeEntryRequest = new UpdateTimeEntryRequest(
            Description: _description,
            Tags: _tags?.Select(t => new TimeTrackingTagDefinition(Name: t, TagFg: null, TagBg: null)).ToList(),
            TagAction: _tagAction,
            Start: _start.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_start.Value) : (System.DateTimeOffset?)null,
            End: _end.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_end.Value) : (System.DateTimeOffset?)null,
            TaskId: _taskId,
            Billable: _billable,
            Duration: _duration,
            Assignee: _assignee,
            IsLocked: _isLocked
        );

        return await _timeTrackingService.UpdateTimeEntryAsync(
            _workspaceId,
            _timerId,
            updateTimeEntryRequest,
            customTaskIds,
            teamIdForCustomTaskIds,
            cancellationToken
        );
    }
}