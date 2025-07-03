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

public class TasksFluentGetFilteredTeamRequest
{
    private readonly GetTasksRequestParameters _parameters = new();
    private readonly string _workspaceId;
    private readonly ITasksService _tasksService;

    public TasksFluentGetFilteredTeamRequest(string workspaceId, ITasksService tasksService)
    {
        _workspaceId = workspaceId;
        _tasksService = tasksService;
    }

    public TasksFluentGetFilteredTeamRequest WithPage(int page) { _parameters.Page = page; return this; }
    public TasksFluentGetFilteredTeamRequest OrderBy(string fieldName, SortDirection direction = SortDirection.Ascending) { _parameters.SortBy = new SortOption(fieldName, direction); return this; }
    public TasksFluentGetFilteredTeamRequest WithSubtasks(bool subtasks) { _parameters.Subtasks = subtasks; return this; }
    public TasksFluentGetFilteredTeamRequest WithSpaceIds(IEnumerable<long> spaceIds) { _parameters.SpaceIds = spaceIds.ToList(); return this; }
    public TasksFluentGetFilteredTeamRequest WithProjectIds(IEnumerable<long> projectIds) { _parameters.ProjectIds = projectIds.ToList(); return this; }
    public TasksFluentGetFilteredTeamRequest WithListIds(IEnumerable<string> listIds) { _parameters.ListIds = listIds.ToList(); return this; } // Changed to string
    public TasksFluentGetFilteredTeamRequest WithStatuses(IEnumerable<string> statuses) { _parameters.Statuses = statuses.ToList(); return this; }
    public TasksFluentGetFilteredTeamRequest WithIncludeClosed(bool includeClosed) { _parameters.IncludeClosed = includeClosed; return this; }
    public TasksFluentGetFilteredTeamRequest WithAssignees(IEnumerable<int> assigneeIds) { _parameters.AssigneeIds = assigneeIds.ToList(); return this; }
    public TasksFluentGetFilteredTeamRequest WithTags(IEnumerable<string> tags) { _parameters.Tags = tags.ToList(); return this; }
    public TasksFluentGetFilteredTeamRequest WithDueDateBetween(DateTimeOffset startDate, DateTimeOffset endDate) { _parameters.DueDateRange = new TimeRange(startDate, endDate); return this; }
    public TasksFluentGetFilteredTeamRequest WithDateCreatedBetween(DateTimeOffset startDate, DateTimeOffset endDate) { _parameters.DateCreatedRange = new TimeRange(startDate, endDate); return this; }
    public TasksFluentGetFilteredTeamRequest WithDateUpdatedBetween(DateTimeOffset startDate, DateTimeOffset endDate) { _parameters.DateUpdatedRange = new TimeRange(startDate, endDate); return this; }
    public TasksFluentGetFilteredTeamRequest WithIncludeMarkdownDescription(bool include) { _parameters.IncludeMarkdownDescription = include; return this; }
    public TasksFluentGetFilteredTeamRequest WithArchived(bool archived) { _parameters.Archived = archived; return this; }

    public TasksFluentGetFilteredTeamRequest WithCustomField(string fieldId, string @operator, object? value)
    {
        var currentCustomFields = _parameters.CustomFields?.ToList() ?? new List<CustomFieldFilter>();
        currentCustomFields.Add(new CustomFieldFilter { FieldId = fieldId, Operator = @operator, Value = value });
        _parameters.CustomFields = currentCustomFields;
        return this;
    }

    public TasksFluentGetFilteredTeamRequest WithCustomItems(IEnumerable<int> customItems) { _parameters.CustomItems = customItems.ToList(); return this; }

    public async Task<IPagedResult<CuTask>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetFilteredTeamTasksAsync(_workspaceId, CopyParametersFrom(_parameters), cancellationToken);
    }

    public IAsyncEnumerable<CuTask> GetAsyncEnumerableAsync(CancellationToken cancellationToken = default)
    {
        return _tasksService.GetFilteredTeamTasksAsyncEnumerableAsync(_workspaceId, _parameters, cancellationToken);
    }

    private static Action<GetTasksRequestParameters> CopyParametersFrom(GetTasksRequestParameters source)
    {
        return target =>
        {
            target.Page = source.Page;
            target.SortBy = source.SortBy;
            target.Subtasks = source.Subtasks;
            target.SpaceIds = source.SpaceIds;
            target.ProjectIds = source.ProjectIds;
            target.ListIds = source.ListIds;
            target.Statuses = source.Statuses;
            target.IncludeClosed = source.IncludeClosed;
            target.AssigneeIds = source.AssigneeIds;
            target.Tags = source.Tags;
            target.DueDateRange = source.DueDateRange;
            target.DateCreatedRange = source.DateCreatedRange;
            target.DateUpdatedRange = source.DateUpdatedRange;
            target.CustomFields = source.CustomFields;
            target.CustomItems = source.CustomItems;
            target.IncludeMarkdownDescription = source.IncludeMarkdownDescription;
            target.Archived = source.Archived;
        };
    }
}