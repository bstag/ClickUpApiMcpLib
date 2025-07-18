using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ClickUp.Api.Client.Fluent;

public class TaskFluentCreateRequest
{
    private string? _name;
    private string? _description;
    private List<int>? _assignees;
    private List<string>? _groupAssignees;
    private List<string>? _tags;
    private long? _status;
    private int? _priority;
    private long? _dueDate;
    private bool? _dueDateTime;
    private long? _timeEstimate;
    private long? _startDate;
    private bool? _startDateTime;
    private string? _parent;
    private bool? _notifyAll;
    private List<CustomTaskFieldToSet>? _customFields;
    private string? _linksTo;
    private bool? _checkRequiredCustomFields;
    private long? _customItemId;

    private readonly string _listId;
    private readonly ITasksService _tasksService;
    private readonly List<string> _validationErrors = new List<string>();

    public TaskFluentCreateRequest(string listId, ITasksService tasksService)
    {
        _listId = listId;
        _tasksService = tasksService;
    }

    public TaskFluentCreateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public TaskFluentCreateRequest WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TaskFluentCreateRequest WithAssignees(List<int> assignees)
    {
        _assignees = assignees;
        return this;
    }

    public TaskFluentCreateRequest WithGroupAssignees(List<string> groupAssignees)
    {
        _groupAssignees = groupAssignees;
        return this;
    }

    public TaskFluentCreateRequest WithTags(List<string> tags)
    {
        _tags = tags;
        return this;
    }

    public TaskFluentCreateRequest WithStatus(long status)
    {
        _status = status;
        return this;
    }

    public TaskFluentCreateRequest WithPriority(int priority)
    {
        _priority = priority;
        return this;
    }

    public TaskFluentCreateRequest WithDueDate(long dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public TaskFluentCreateRequest WithDueDateTime(bool dueDateTime)
    {
        _dueDateTime = dueDateTime;
        return this;
    }

    public TaskFluentCreateRequest WithTimeEstimate(long timeEstimate)
    {
        _timeEstimate = timeEstimate;
        return this;
    }

    public TaskFluentCreateRequest WithStartDate(long startDate)
    {
        _startDate = startDate;
        return this;
    }

    public TaskFluentCreateRequest WithStartDateTime(bool startDateTime)
    {
        _startDateTime = startDateTime;
        return this;
    }

    public TaskFluentCreateRequest WithParent(string parent)
    {
        _parent = parent;
        return this;
    }

    public TaskFluentCreateRequest WithNotifyAll(bool notifyAll)
    {
        _notifyAll = notifyAll;
        return this;
    }

    public TaskFluentCreateRequest WithCustomFields(List<CustomTaskFieldToSet> customFields)
    {
        _customFields = customFields;
        return this;
    }

    public TaskFluentCreateRequest WithLinksTo(string linksTo)
    {
        _linksTo = linksTo;
        return this;
    }

    public TaskFluentCreateRequest WithCheckRequiredCustomFields(bool checkRequiredCustomFields)
    {
        _checkRequiredCustomFields = checkRequiredCustomFields;
        return this;
    }

    public TaskFluentCreateRequest WithCustomItemId(long customItemId)
    {
        _customItemId = customItemId;
        return this;
    }

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_listId))
        {
            _validationErrors.Add("ListId is required.");
        }
        if (string.IsNullOrWhiteSpace(_name))
        {
            _validationErrors.Add("Task name is required.");
        }
        // Add other validation rules as needed (e.g. for custom fields if some are mandatory based on other settings)

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<CuTask> CreateAsync(bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        Validate();
        var createTaskRequest = new CreateTaskRequest(
            Name: _name ?? string.Empty,
            Description: _description,
            Assignees: _assignees,
            GroupAssignees: _groupAssignees,
            Tags: _tags,
            Status: _status?.ToString(),
            Priority: _priority,
            DueDate: _dueDate.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_dueDate.Value) : (System.DateTimeOffset?)null,
            DueDateTime: _dueDateTime,
            TimeEstimate: _timeEstimate.HasValue ? (int?)_timeEstimate.Value : (int?)null,
            StartDate: _startDate.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_startDate.Value) : (System.DateTimeOffset?)null,
            StartDateTime: _startDateTime,
            NotifyAll: _notifyAll,
            Parent: _parent,
            LinksTo: _linksTo,
            CheckRequiredCustomFields: _checkRequiredCustomFields,
            CustomFields: _customFields,
            CustomItemId: _customItemId,
            ListId: _listId
        );

        return await _tasksService.CreateTaskAsync(
            _listId,
            createTaskRequest,
            customTaskIds,
            teamId,
            cancellationToken
        );
    }
}