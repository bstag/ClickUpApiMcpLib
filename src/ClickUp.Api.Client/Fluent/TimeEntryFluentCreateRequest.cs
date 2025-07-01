using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TimeEntryFluentCreateRequest
{
    private string? _tid;
    private string? _description;
    private List<string>? _tags;
    private long? _start;
    private int? _duration; // Duration in milliseconds
    private bool? _billable;
    private int? _assignee;

    private readonly string _workspaceId;
    private readonly ITimeTrackingService _timeTrackingService;

    public TimeEntryFluentCreateRequest(string workspaceId, ITimeTrackingService timeTrackingService)
    {
        _workspaceId = workspaceId;
        _timeTrackingService = timeTrackingService;
    }

    public TimeEntryFluentCreateRequest WithTaskId(string tid)
    {
        _tid = tid;
        return this;
    }

    public TimeEntryFluentCreateRequest WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TimeEntryFluentCreateRequest WithTags(List<string> tags)
    {
        _tags = tags;
        return this;
    }

    public TimeEntryFluentCreateRequest WithStart(long start)
    {
        _start = start;
        return this;
    }

    public TimeEntryFluentCreateRequest WithDuration(int duration)
    {
        _duration = duration;
        return this;
    }

    public TimeEntryFluentCreateRequest WithBillable(bool billable)
    {
        _billable = billable;
        return this;
    }

    public TimeEntryFluentCreateRequest WithAssignee(int assignee)
    {
        _assignee = assignee;
        return this;
    }

    public async Task<TimeEntry> ExecuteAsync(bool? customTaskIds = null, string? teamIdForCustomTaskIds = null, CancellationToken cancellationToken = default)
    {
        var createTimeEntryRequest = new CreateTimeEntryRequest(
            Description: _description,
            Tags: _tags?.Select(t => new TimeTrackingTagDefinition(Name: t, TagFg: null, TagBg: null)).ToList(),
            Start: _start ?? 0, // Assuming 0 is a valid default or will be handled by validation
            Duration: _duration ?? 0, // Assuming 0 is a valid default or will be handled by validation
            Billable: _billable,
            Assignee: _assignee,
            TaskId: _tid,
            WorkspaceId: _workspaceId
        );

        return await _timeTrackingService.CreateTimeEntryAsync(
            _workspaceId,
            createTimeEntryRequest,
            customTaskIds,
            teamIdForCustomTaskIds,
            cancellationToken
        );
    }
}