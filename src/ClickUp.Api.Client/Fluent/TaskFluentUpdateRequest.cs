using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ClickUp.Api.Client.Fluent;

public class TaskFluentUpdateRequest
{
    private string? _name;
    private string? _description;
    private List<int>? _assignees; // Note: UpdateTaskRequest takes AssigneeModification<int>
    private long? _status;
    private int? _priority;
    private long? _dueDate;
    private bool? _dueDateTime;
    private long? _timeEstimate;
    private long? _startDate;
    private bool? _startDateTime;
    private string? _parent;
    private bool? _notifyAll; // Not directly in UpdateTaskRequest, usually a query param or implicit
    private bool? _archived;
    // Unset properties are handled by passing null to nullable fields in UpdateTaskRequest
    // or by specific boolean flags if the API requires them (not typical for this model structure).
    // For example, to unset status, you might pass Status = null if the property is nullable string.
    // The current UpdateTaskRequest model seems to rely on nulls for unsetting.

    private readonly string _taskId;
    private readonly ITasksService _tasksService;
    private readonly List<string> _validationErrors = new List<string>();

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

    // To simplify, this method will overwrite assignees.
    // For add/remove, separate methods or a more complex WithAssignees taking an action would be needed.
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
        // This property is not directly part of UpdateTaskRequest body.
        // It's often a query parameter on the request.
        // For this example, we'll acknowledge it but it won't map to UpdateTaskRequest.
        _notifyAll = notifyAll;
        return this;
    }

    public TaskFluentUpdateRequest WithArchived(bool archived)
    {
        _archived = archived;
        return this;
    }

    // Methods for unsetting specific fields by setting them to null or a specific marker
    // will be handled by the user not calling the 'With<Property>' method for those,
    // or by explicitly passing null if the 'With' method allowed nullable types for value types.
    // The UpdateTaskRequest model uses nullable properties, so not setting them in the fluent builder
    // means they will be null in the request DTO.

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_taskId))
        {
            _validationErrors.Add("TaskId is required.");
        }
        // For an update, at least one modifiable field should ideally be provided.
        // However, the API might allow an empty update request (noop).
        // For now, we'll assume the API handles this.
        // A more strict validation could check if any property was actually changed from its default.
        // e.g. if (_name == null && _description == null && _assignees == null && ... && !_archived.HasValue)

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<CuTask> UpdateAsync(bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        Validate();
        var updateTaskRequest = new UpdateTaskRequest(
            Name: _name,
            Description: _description,
            Status: _status?.ToString(), // Null if _status is null
            Priority: _priority, // Null if _priority is null
            DueDate: _dueDate.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_dueDate.Value) : (System.DateTimeOffset?)null,
            DueDateTime: _dueDateTime, // Null if _dueDateTime is null
            Parent: _parent, // Null if _parent is null
            TimeEstimate: _timeEstimate.HasValue ? (int?)_timeEstimate.Value : (int?)null,
            StartDate: _startDate.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_startDate.Value) : (System.DateTimeOffset?)null,
            StartDateTime: _startDateTime, // Null if _startDateTime is null
            Assignees: _assignees != null ? new AssigneeModification<int>(Add: _assignees, Remove: null) : null, // Overwrites assignees
            GroupAssignees: null, // Not handled by this fluent builder for simplicity
            Archived: _archived, // Null if _archived is null
            CustomFields: null // Not handled by this fluent builder for simplicity
        );
        // Note on Unset: Passing null for nullable properties in UpdateTaskRequest is the standard way to indicate
        // "no change" or "unset" for those fields, depending on API behavior.
        // If API requires explicit "unset_property: true", the DTO and fluent builder would need adjustment.

        return await _tasksService.UpdateTaskAsync(
            _taskId,
            updateTaskRequest,
            customTaskIds,
            teamId,
            cancellationToken
        );
    }
}