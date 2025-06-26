using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentStartTimeEntryRequest
{
    private string? _taskId;
    private string? _description;
    private List<string>? _tags;
    private bool? _billable;
    private string? _workspaceIdForRequest;
    private int? _projectIdLegacy;
    private string? _createdWith;

    private readonly string _workspaceId;
    private readonly ITimeTrackingService _timeTrackingService;

    public FluentStartTimeEntryRequest(string workspaceId, ITimeTrackingService timeTrackingService)
    {
        _workspaceId = workspaceId;
        _timeTrackingService = timeTrackingService;
    }

    public FluentStartTimeEntryRequest WithTaskId(string taskId)
    {
        _taskId = taskId;
        return this;
    }

    public FluentStartTimeEntryRequest WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public FluentStartTimeEntryRequest WithTags(List<string> tags)
    {
        _tags = tags;
        return this;
    }

    public FluentStartTimeEntryRequest WithBillable(bool billable)
    {
        _billable = billable;
        return this;
    }

    public FluentStartTimeEntryRequest WithWorkspaceIdForRequest(string workspaceIdForRequest)
    {
        _workspaceIdForRequest = workspaceIdForRequest;
        return this;
    }

    public FluentStartTimeEntryRequest WithProjectIdLegacy(int projectIdLegacy)
    {
        _projectIdLegacy = projectIdLegacy;
        return this;
    }

    public FluentStartTimeEntryRequest WithCreatedWith(string createdWith)
    {
        _createdWith = createdWith;
        return this;
    }

    public async Task<TimeEntry> StartAsync(bool? customTaskIds = null, string? teamIdForCustomTaskIds = null, CancellationToken cancellationToken = default)
    {
        var startTimeEntryRequest = new StartTimeEntryRequest(
            Description: _description,
            Tags: _tags?.Select(t => new TimeTrackingTagDefinition(Name: t, TagFg: null, TagBg: null)).ToList(),
            TaskId: _taskId,
            Billable: _billable,
            WorkspaceId: _workspaceIdForRequest,
            ProjectId_Legacy: _projectIdLegacy,
            CreatedWith: _createdWith
        );

        return await _timeTrackingService.StartTimeEntryAsync(
            _workspaceId,
            startTimeEntryRequest,
            customTaskIds,
            teamIdForCustomTaskIds,
            cancellationToken
        );
    }
}