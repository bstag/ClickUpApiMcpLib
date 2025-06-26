using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentCreateTimeEntryRequest
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

    public FluentCreateTimeEntryRequest(string workspaceId, ITimeTrackingService timeTrackingService)
    {
        _workspaceId = workspaceId;
        _timeTrackingService = timeTrackingService;
    }

    public FluentCreateTimeEntryRequest WithTaskId(string tid)
    {
        _tid = tid;
        return this;
    }

    public FluentCreateTimeEntryRequest WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public FluentCreateTimeEntryRequest WithTags(List<string> tags)
    {
        _tags = tags;
        return this;
    }

    public FluentCreateTimeEntryRequest WithStart(long start)
    {
        _start = start;
        return this;
    }

    public FluentCreateTimeEntryRequest WithDuration(int duration)
    {
        _duration = duration;
        return this;
    }

    public FluentCreateTimeEntryRequest WithBillable(bool billable)
    {
        _billable = billable;
        return this;
    }

    public FluentCreateTimeEntryRequest WithAssignee(int assignee)
    {
        _assignee = assignee;
        return this;
    }

    public async Task<TimeEntry> CreateAsync(bool? customTaskIds = null, string? teamIdForCustomTaskIds = null, CancellationToken cancellationToken = default)
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