using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using ClickUp.Api.Client.Models.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TimeEntryFluentUpdateRequest
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
    private string? _tagAction; // "add" or "remove"

    private readonly string _workspaceId;
    private readonly string _timerId;
    private readonly ITimeTrackingService _timeTrackingService;
    private readonly List<string> _validationErrors = new List<string>();

    public TimeEntryFluentUpdateRequest(string workspaceId, string timerId, ITimeTrackingService timeTrackingService)
    {
        _workspaceId = workspaceId;
        _timerId = timerId;
        _timeTrackingService = timeTrackingService;
    }

    public TimeEntryFluentUpdateRequest WithTaskId(string taskId)
    {
        _taskId = taskId;
        return this;
    }

    public TimeEntryFluentUpdateRequest WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TimeEntryFluentUpdateRequest WithTags(List<string> tags)
    {
        _tags = tags;
        return this;
    }

    public TimeEntryFluentUpdateRequest WithStart(long start)
    {
        _start = start;
        return this;
    }

    public TimeEntryFluentUpdateRequest WithEnd(long end)
    {
        _end = end;
        return this;
    }

    public TimeEntryFluentUpdateRequest WithDuration(int duration)
    {
        _duration = duration;
        return this;
    }

    public TimeEntryFluentUpdateRequest WithBillable(bool billable)
    {
        _billable = billable;
        return this;
    }

    public TimeEntryFluentUpdateRequest WithAssignee(int assignee)
    {
        _assignee = assignee;
        return this;
    }

    public TimeEntryFluentUpdateRequest WithIsLocked(bool isLocked)
    {
        _isLocked = isLocked;
        return this;
    }

    public TimeEntryFluentUpdateRequest WithTagAction(string tagAction)
    {
        _tagAction = tagAction;
        return this;
    }

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_workspaceId))
        {
            _validationErrors.Add("WorkspaceId (TeamId) is required.");
        }
        if (string.IsNullOrWhiteSpace(_timerId))
        {
            _validationErrors.Add("TimerId is required.");
        }
        if (_duration.HasValue && _duration.Value <= 0)
        {
            _validationErrors.Add("If Duration is provided, it must be positive.");
        }
        if (!string.IsNullOrWhiteSpace(_tagAction) && !(_tagAction == "add" || _tagAction == "remove"))
        {
            _validationErrors.Add("TagAction must be 'add' or 'remove' if provided.");
        }
        if ((_tagAction == "add" || _tagAction == "remove") && (_tags == null || !_tags.Any()))
        {
            _validationErrors.Add("Tags must be provided when TagAction is specified.");
        }
        // At least one field should be provided for an update.
        if (string.IsNullOrWhiteSpace(_taskId) && string.IsNullOrWhiteSpace(_description) &&
            (_tags == null || !_tags.Any()) && !_start.HasValue && !_end.HasValue &&
            !_duration.HasValue && !_billable.HasValue && !_assignee.HasValue && !_isLocked.HasValue)
        {
            _validationErrors.Add("At least one property must be set for updating a Time Entry.");
        }


        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<TimeEntry> UpdateAsync(bool? customTaskIds = null, string? teamIdForCustomTaskIds = null, CancellationToken cancellationToken = default)
    {
        Validate();
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