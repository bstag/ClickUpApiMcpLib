using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TaskFluentUpdateRequest
{
    private string? _name;
    private string? _description;
    private List<int>? _assignees;
    private long? _status;
    private int? _priority;
    private long? _dueDate;
    private bool? _dueDateTime;
    private long? _timeEstimate;
    private long? _startDate;
    private bool? _startDateTime;
    private string? _parent;
    private bool? _notifyAll;
    private bool? _archived;
    private bool? _unsetStatus;
    private bool? _unsetAssignee;
    private bool? _unsetDueDate;
    private bool? _unsetStartDate;
    private bool? _unsetTimeEstimate;
    private bool? _unsetParent;
    private bool? _unsetPriority;

    private readonly string _taskId;
    private readonly ITasksService _tasksService;

    public TaskFluentUpdateRequest(string taskId, ITasksService tasksService)
    {
        _taskId = taskId;
        _tasksService = tasksService;
    }

    public TaskFluentUpdateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public TaskFluentUpdateRequest WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TaskFluentUpdateRequest WithAssignees(List<int> assignees)
    {
        _assignees = assignees;
        return this;
    }

    public TaskFluentUpdateRequest WithStatus(long status)
    {
        _status = status;
        return this;
    }

    public TaskFluentUpdateRequest WithPriority(int priority)
    {
        _priority = priority;
        return this;
    }

    public TaskFluentUpdateRequest WithDueDate(long dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public TaskFluentUpdateRequest WithDueDateTime(bool dueDateTime)
    {
        _dueDateTime = dueDateTime;
        return this;
    }

    public TaskFluentUpdateRequest WithTimeEstimate(long timeEstimate)
    {
        _timeEstimate = timeEstimate;
        return this;
    }

    public TaskFluentUpdateRequest WithStartDate(long startDate)
    {
        _startDate = startDate;
        return this;
    }

    public TaskFluentUpdateRequest WithStartDateTime(bool startDateTime)
    {
        _startDateTime = startDateTime;
        return this;
    }

    public TaskFluentUpdateRequest WithParent(string parent)
    {
        _parent = parent;
        return this;
    }

    public TaskFluentUpdateRequest WithNotifyAll(bool notifyAll)
    {
        _notifyAll = notifyAll;
        return this;
    }

    public TaskFluentUpdateRequest WithArchived(bool archived)
    {
        _archived = archived;
        return this;
    }

    public TaskFluentUpdateRequest WithUnsetStatus(bool unsetStatus)
    {
        _unsetStatus = unsetStatus;
        return this;
    }

    public TaskFluentUpdateRequest WithUnsetAssignee(bool unsetAssignee)
    {
        _unsetAssignee = unsetAssignee;
        return this;
    }

    public TaskFluentUpdateRequest WithUnsetDueDate(bool unsetDueDate)
    {
        _unsetDueDate = unsetDueDate;
        return this;
    }

    public TaskFluentUpdateRequest WithUnsetStartDate(bool unsetStartDate)
    {
        _unsetStartDate = unsetStartDate;
        return this;
    }

    public TaskFluentUpdateRequest WithUnsetTimeEstimate(bool unsetTimeEstimate)
    {
        _unsetTimeEstimate = unsetTimeEstimate;
        return this;
    }

    public TaskFluentUpdateRequest WithUnsetParent(bool unsetParent)
    {
        _unsetParent = unsetParent;
        return this;
    }

    public TaskFluentUpdateRequest WithUnsetPriority(bool unsetPriority)
    {
        _unsetPriority = unsetPriority;
        return this;
    }

    public async Task<CuTask> ExecuteAsync(bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        var updateTaskRequest = new UpdateTaskRequest(
            Name: _name,
            Description: _description,
            Status: _status?.ToString(),
            Priority: _priority,
            DueDate: _dueDate.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_dueDate.Value) : (System.DateTimeOffset?)null,
            DueDateTime: _dueDateTime,
            Parent: _parent,
            TimeEstimate: _timeEstimate.HasValue ? (int?)_timeEstimate.Value : (int?)null,
            StartDate: _startDate.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_startDate.Value) : (System.DateTimeOffset?)null,
            StartDateTime: _startDateTime,
            Assignees: _assignees != null ? new AssigneeModification<int>(Add: _assignees, Remove: null) : null,
            GroupAssignees: null, // Not handled by this fluent builder yet
            Archived: _archived,
            CustomFields: null // Not handled by this fluent builder yet
        );

        return await _tasksService.UpdateTaskAsync(
            _taskId,
            updateTaskRequest,
            customTaskIds,
            teamId,
            cancellationToken
        );
    }
}