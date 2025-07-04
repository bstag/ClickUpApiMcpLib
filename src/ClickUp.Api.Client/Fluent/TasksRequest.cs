using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Common.ValueObjects;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.Parameters;
using ClickUp.Api.Client.Models.Common.Pagination;


namespace ClickUp.Api.Client.Fluent;

public class TasksRequest
{
    private readonly GetTasksRequestParameters _parameters = new();
    private readonly string _listId;
    private readonly ITasksService _tasksService;

    public TasksRequest(string listId, ITasksService tasksService)
    {
        _listId = listId;
        _tasksService = tasksService;
        _parameters.ListIds = new List<string> { listId }; // Pre-set ListId for this request type, now as string
    }

    public TasksRequest WithArchived(bool archived) { _parameters.Archived = archived; return this; }
    public TasksRequest WithIncludeMarkdownDescription(bool include) { _parameters.IncludeMarkdownDescription = include; return this; }
    public TasksRequest WithPage(int page) { _parameters.Page = page; return this; }
    public TasksRequest OrderBy(string fieldName, SortDirection direction = SortDirection.Ascending) { _parameters.SortBy = new SortOption(fieldName, direction); return this; }
    public TasksRequest WithSubtasks(bool subtasks) { _parameters.Subtasks = subtasks; return this; }
    public TasksRequest WithStatuses(IEnumerable<string> statuses) { _parameters.Statuses = statuses.ToList(); return this; }
    public TasksRequest WithIncludeClosed(bool includeClosed) { _parameters.IncludeClosed = includeClosed; return this; }
    public TasksRequest WithAssignees(IEnumerable<int> assigneeIds) { _parameters.AssigneeIds = assigneeIds.ToList(); return this; }
    public TasksRequest WithTags(IEnumerable<string> tags) { _parameters.Tags = tags.ToList(); return this; }
    public TasksRequest WithDueDateBetween(DateTimeOffset startDate, DateTimeOffset endDate) { _parameters.DueDateRange = new TimeRange(startDate, endDate); return this; }
    public TasksRequest WithDateCreatedBetween(DateTimeOffset startDate, DateTimeOffset endDate) { _parameters.DateCreatedRange = new TimeRange(startDate, endDate); return this; }
    public TasksRequest WithDateUpdatedBetween(DateTimeOffset startDate, DateTimeOffset endDate) { _parameters.DateUpdatedRange = new TimeRange(startDate, endDate); return this; }

    public TasksRequest WithCustomField(string fieldId, string @operator, object? value)
    {
        var currentCustomFields = _parameters.CustomFields?.ToList() ?? new List<CustomFieldFilter>();
        currentCustomFields.Add(new CustomFieldFilter { FieldId = fieldId, Operator = @operator, Value = value });
        _parameters.CustomFields = currentCustomFields;
        return this;
    }

    public TasksRequest WithCustomItems(IEnumerable<int> customItems) { _parameters.CustomItems = customItems.ToList(); return this; }

    // Methods specific to GetTasks (by listId) that might not be in GetFilteredTeamTasks
    public TasksRequest WithSpaceIds(IEnumerable<long> spaceIds) { _parameters.SpaceIds = spaceIds.ToList(); return this; }
    public TasksRequest WithProjectIds(IEnumerable<long> projectIds) { _parameters.ProjectIds = projectIds.ToList(); return this; }


    public async Task<IPagedResult<CuTask>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetTasksAsync(_listId, CopyParametersFrom(_parameters), cancellationToken);
    }

    public IAsyncEnumerable<CuTask> GetAsyncEnumerableAsync(CancellationToken cancellationToken = default)
    {
        return _tasksService.GetTasksAsyncEnumerableAsync(_listId, _parameters, cancellationToken);
    }

    private static Action<GetTasksRequestParameters> CopyParametersFrom(GetTasksRequestParameters source)
    {
        return target =>
        {
            target.Archived = source.Archived;
            target.IncludeMarkdownDescription = source.IncludeMarkdownDescription;
            target.Page = source.Page;
            target.SortBy = source.SortBy;
            target.Subtasks = source.Subtasks;
            target.Statuses = source.Statuses;
            target.IncludeClosed = source.IncludeClosed;
            target.AssigneeIds = source.AssigneeIds;
            target.Tags = source.Tags;
            target.DueDateRange = source.DueDateRange;
            target.DateCreatedRange = source.DateCreatedRange;
            target.DateUpdatedRange = source.DateUpdatedRange;
            target.CustomFields = source.CustomFields;
            target.CustomItems = source.CustomItems;
            target.SpaceIds = source.SpaceIds;
            target.ProjectIds = source.ProjectIds;
            target.ListIds = source.ListIds;
        };
    }
}
