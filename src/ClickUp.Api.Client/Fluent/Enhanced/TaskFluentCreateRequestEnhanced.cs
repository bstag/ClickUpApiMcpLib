using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent.Builders;
using ClickUp.Api.Client.Fluent.Validation;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent.Enhanced;

/// <summary>
/// Enhanced fluent builder for creating tasks with improved validation and chaining.
/// </summary>
public class TaskFluentCreateRequestEnhanced : FluentBuilderBase<TaskFluentCreateRequestEnhanced, CuTask>
{
    private readonly string _listId;
    private readonly ITasksService _tasksService;

    // State keys for type-safe state management
    private const string NameKey = "name";
    private const string DescriptionKey = "description";
    private const string AssigneesKey = "assignees";
    private const string GroupAssigneesKey = "groupAssignees";
    private const string TagsKey = "tags";
    private const string StatusKey = "status";
    private const string PriorityKey = "priority";
    private const string DueDateKey = "dueDate";
    private const string DueDateTimeKey = "dueDateTime";
    private const string TimeEstimateKey = "timeEstimate";
    private const string StartDateKey = "startDate";
    private const string StartDateTimeKey = "startDateTime";
    private const string ParentKey = "parent";
    private const string NotifyAllKey = "notifyAll";
    private const string CustomFieldsKey = "customFields";
    private const string LinksToKey = "linksTo";
    private const string CheckRequiredCustomFieldsKey = "checkRequiredCustomFields";
    private const string CustomItemIdKey = "customItemId";

    public TaskFluentCreateRequestEnhanced(string listId, ITasksService tasksService)
    {
        _listId = listId ?? throw new ArgumentNullException(nameof(listId));
        _tasksService = tasksService ?? throw new ArgumentNullException(nameof(tasksService));
    }

    /// <summary>
    /// Sets the task name.
    /// </summary>
    /// <param name="name">The task name (required)</param>
    /// <returns>The builder for chaining</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or empty</exception>
    public TaskFluentCreateRequestEnhanced WithName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Task name cannot be null or empty.", nameof(name));
        
        GetOrSetState(NameKey, name);
        return this;
    }

    /// <summary>
    /// Sets the task description.
    /// </summary>
    /// <param name="description">The task description</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithDescription(string? description)
    {
        GetOrSetState(DescriptionKey, description);
        return this;
    }

    /// <summary>
    /// Sets the task assignees.
    /// </summary>
    /// <param name="assignees">The list of assignee user IDs</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithAssignees(params int[] assignees)
    {
        return WithAssignees(assignees?.ToList());
    }

    /// <summary>
    /// Sets the task assignees.
    /// </summary>
    /// <param name="assignees">The list of assignee user IDs</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithAssignees(List<int>? assignees)
    {
        GetOrSetState(AssigneesKey, assignees);
        return this;
    }

    /// <summary>
    /// Adds a single assignee to the task.
    /// </summary>
    /// <param name="assigneeId">The assignee user ID</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced AddAssignee(int assigneeId)
    {
        var assignees = GetOrSetState<List<int>>(AssigneesKey) ?? new List<int>();
        if (!assignees.Contains(assigneeId))
        {
            assignees.Add(assigneeId);
            GetOrSetState(AssigneesKey, assignees);
        }
        return this;
    }

    /// <summary>
    /// Sets the task group assignees.
    /// </summary>
    /// <param name="groupAssignees">The list of group assignee IDs</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithGroupAssignees(params string[] groupAssignees)
    {
        return WithGroupAssignees(groupAssignees?.ToList());
    }

    /// <summary>
    /// Sets the task group assignees.
    /// </summary>
    /// <param name="groupAssignees">The list of group assignee IDs</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithGroupAssignees(List<string>? groupAssignees)
    {
        GetOrSetState(GroupAssigneesKey, groupAssignees);
        return this;
    }

    /// <summary>
    /// Sets the task tags.
    /// </summary>
    /// <param name="tags">The list of tags</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithTags(params string[] tags)
    {
        return WithTags(tags?.ToList());
    }

    /// <summary>
    /// Sets the task tags.
    /// </summary>
    /// <param name="tags">The list of tags</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithTags(List<string>? tags)
    {
        GetOrSetState(TagsKey, tags);
        return this;
    }

    /// <summary>
    /// Adds a single tag to the task.
    /// </summary>
    /// <param name="tag">The tag to add</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return this;
        
        var tags = GetOrSetState<List<string>>(TagsKey) ?? new List<string>();
        if (!tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            tags.Add(tag);
            GetOrSetState(TagsKey, tags);
        }
        return this;
    }

    /// <summary>
    /// Sets the task status.
    /// </summary>
    /// <param name="status">The status ID</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithStatus(long status)
    {
        GetOrSetState(StatusKey, status);
        return this;
    }

    /// <summary>
    /// Sets the task priority.
    /// </summary>
    /// <param name="priority">The priority level (1-4, where 1 is urgent)</param>
    /// <returns>The builder for chaining</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when priority is not between 1 and 4</exception>
    public TaskFluentCreateRequestEnhanced WithPriority(int priority)
    {
        if (priority < 1 || priority > 4)
            throw new ArgumentOutOfRangeException(nameof(priority), "Priority must be between 1 and 4.");
        
        GetOrSetState(PriorityKey, priority);
        return this;
    }

    /// <summary>
    /// Sets the task as urgent (priority 1).
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced AsUrgent() => WithPriority(1);

    /// <summary>
    /// Sets the task as high priority (priority 2).
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced AsHighPriority() => WithPriority(2);

    /// <summary>
    /// Sets the task as normal priority (priority 3).
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced AsNormalPriority() => WithPriority(3);

    /// <summary>
    /// Sets the task as low priority (priority 4).
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced AsLowPriority() => WithPriority(4);

    /// <summary>
    /// Sets the task due date.
    /// </summary>
    /// <param name="dueDate">The due date</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithDueDate(DateTimeOffset dueDate)
    {
        GetOrSetState(DueDateKey, dueDate.ToUnixTimeMilliseconds());
        return this;
    }

    /// <summary>
    /// Sets the task due date from a DateTime.
    /// </summary>
    /// <param name="dueDate">The due date</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithDueDate(DateTime dueDate)
    {
        return WithDueDate(new DateTimeOffset(dueDate));
    }

    /// <summary>
    /// Sets the task to be due in a specified number of days.
    /// </summary>
    /// <param name="days">The number of days from now</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced DueInDays(int days)
    {
        return WithDueDate(DateTimeOffset.Now.AddDays(days));
    }

    /// <summary>
    /// Sets whether the due date includes time.
    /// </summary>
    /// <param name="includeTime">True to include time in due date</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithDueDateTime(bool includeTime)
    {
        GetOrSetState(DueDateTimeKey, includeTime);
        return this;
    }

    /// <summary>
    /// Sets the task time estimate in milliseconds.
    /// </summary>
    /// <param name="timeEstimateMs">The time estimate in milliseconds</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithTimeEstimate(long timeEstimateMs)
    {
        GetOrSetState(TimeEstimateKey, timeEstimateMs);
        return this;
    }

    /// <summary>
    /// Sets the task time estimate from a TimeSpan.
    /// </summary>
    /// <param name="timeEstimate">The time estimate</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithTimeEstimate(TimeSpan timeEstimate)
    {
        return WithTimeEstimate((long)timeEstimate.TotalMilliseconds);
    }

    /// <summary>
    /// Sets the task time estimate in hours.
    /// </summary>
    /// <param name="hours">The time estimate in hours</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithTimeEstimateHours(double hours)
    {
        return WithTimeEstimate(TimeSpan.FromHours(hours));
    }

    /// <summary>
    /// Sets the task start date.
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithStartDate(DateTimeOffset startDate)
    {
        GetOrSetState(StartDateKey, startDate.ToUnixTimeMilliseconds());
        return this;
    }

    /// <summary>
    /// Sets whether the start date includes time.
    /// </summary>
    /// <param name="includeTime">True to include time in start date</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithStartDateTime(bool includeTime)
    {
        GetOrSetState(StartDateTimeKey, includeTime);
        return this;
    }

    /// <summary>
    /// Sets the parent task ID.
    /// </summary>
    /// <param name="parentTaskId">The parent task ID</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithParent(string parentTaskId)
    {
        GetOrSetState(ParentKey, parentTaskId);
        return this;
    }

    /// <summary>
    /// Sets whether to notify all assignees.
    /// </summary>
    /// <param name="notifyAll">True to notify all assignees</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithNotifyAll(bool notifyAll = true)
    {
        GetOrSetState(NotifyAllKey, notifyAll);
        return this;
    }

    /// <summary>
    /// Sets the custom fields for the task.
    /// </summary>
    /// <param name="customFields">The custom fields to set</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithCustomFields(List<CustomTaskFieldToSet>? customFields)
    {
        GetOrSetState(CustomFieldsKey, customFields);
        return this;
    }

    /// <summary>
    /// Sets the task to link to another task.
    /// </summary>
    /// <param name="taskId">The task ID to link to</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithLinksTo(string taskId)
    {
        GetOrSetState(LinksToKey, taskId);
        return this;
    }

    /// <summary>
    /// Sets whether to check required custom fields.
    /// </summary>
    /// <param name="check">True to check required custom fields</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithCheckRequiredCustomFields(bool check = true)
    {
        GetOrSetState(CheckRequiredCustomFieldsKey, check);
        return this;
    }

    /// <summary>
    /// Sets the custom item ID.
    /// </summary>
    /// <param name="customItemId">The custom item ID</param>
    /// <returns>The builder for chaining</returns>
    public TaskFluentCreateRequestEnhanced WithCustomItemId(long customItemId)
    {
        GetOrSetState(CustomItemIdKey, customItemId);
        return this;
    }

    /// <summary>
    /// Creates the task asynchronously.
    /// </summary>
    /// <param name="customTaskIds">Whether to use custom task IDs</param>
    /// <param name="teamId">The team ID</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created task</returns>
    public async Task<CuTask> CreateAsync(bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(cancellationToken);
    }

    protected override void ValidateCore()
    {
        var pipeline = new FluentValidationPipeline()
            .RequiredField(_listId, "ListId")
            .RequiredField(GetOrSetState<string>(NameKey), "Task Name")
            .MaxLengthField(GetOrSetState<string>(NameKey), 1000, "Task Name")
            .MaxLengthField(GetOrSetState<string>(DescriptionKey), 5000, "Description")
            .RangeField(GetOrSetState<int?>(PriorityKey), 1, 4, "Priority")
            .NotEmptyCollection(GetOrSetState<List<int>>(AssigneesKey), "Assignees")
            .NotEmptyCollection(GetOrSetState<List<string>>(GroupAssigneesKey), "Group Assignees")
            .NotEmptyCollection(GetOrSetState<List<string>>(TagsKey), "Tags");

        pipeline.ValidateAndThrow();
    }

    protected override CuTask BuildCore()
    {
        // This method is not used for async operations
        throw new NotSupportedException("Use ExecuteAsync for task creation.");
    }

    public override async Task<CuTask> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Validate();

        var createTaskRequest = new CreateTaskRequest(
            Name: GetOrSetState<string>(NameKey) ?? string.Empty,
            Description: GetOrSetState<string>(DescriptionKey),
            Assignees: GetOrSetState<List<int>>(AssigneesKey),
            GroupAssignees: GetOrSetState<List<string>>(GroupAssigneesKey),
            Tags: GetOrSetState<List<string>>(TagsKey),
            Status: GetOrSetState<long?>(StatusKey)?.ToString(),
            Priority: GetOrSetState<int?>(PriorityKey),
            DueDate: GetOrSetState<long?>(DueDateKey).HasValue 
                ? DateTimeOffset.FromUnixTimeMilliseconds(GetOrSetState<long?>(DueDateKey)!.Value) 
                : null,
            DueDateTime: GetOrSetState<bool?>(DueDateTimeKey),
            TimeEstimate: GetOrSetState<long?>(TimeEstimateKey).HasValue 
                ? (int?)GetOrSetState<long?>(TimeEstimateKey)!.Value 
                : null,
            StartDate: GetOrSetState<long?>(StartDateKey).HasValue 
                ? DateTimeOffset.FromUnixTimeMilliseconds(GetOrSetState<long?>(StartDateKey)!.Value) 
                : null,
            StartDateTime: GetOrSetState<bool?>(StartDateTimeKey),
            NotifyAll: GetOrSetState<bool?>(NotifyAllKey),
            Parent: GetOrSetState<string>(ParentKey),
            LinksTo: GetOrSetState<string>(LinksToKey),
            CheckRequiredCustomFields: GetOrSetState<bool?>(CheckRequiredCustomFieldsKey),
            CustomFields: GetOrSetState<List<CustomTaskFieldToSet>>(CustomFieldsKey),
            CustomItemId: GetOrSetState<long?>(CustomItemIdKey),
            ListId: _listId
        );

        return await _tasksService.CreateTaskAsync(
            _listId,
            createTaskRequest,
            null, // customTaskIds - will be added as parameter if needed
            null, // teamId - will be added as parameter if needed
            cancellationToken
        );
    }

    protected override TaskFluentCreateRequestEnhanced CreateInstance()
    {
        return new TaskFluentCreateRequestEnhanced(_listId, _tasksService);
    }
}