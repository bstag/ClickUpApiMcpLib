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
        return await _tasksService.GetFilteredTeamTasksAsync(_workspaceId, p =>
        {
            p.Page = _parameters.Page;
            p.SortBy = _parameters.SortBy;
            p.Subtasks = _parameters.Subtasks;
            p.SpaceIds = _parameters.SpaceIds;
            p.ProjectIds = _parameters.ProjectIds;
            p.ListIds = _parameters.ListIds;
            p.Statuses = _parameters.Statuses;
            p.IncludeClosed = _parameters.IncludeClosed;
            p.AssigneeIds = _parameters.AssigneeIds;
            p.Tags = _parameters.Tags;
            p.DueDateRange = _parameters.DueDateRange;
            p.DateCreatedRange = _parameters.DateCreatedRange;
            p.DateUpdatedRange = _parameters.DateUpdatedRange;
            p.CustomFields = _parameters.CustomFields;
            p.CustomItems = _parameters.CustomItems;
            p.IncludeMarkdownDescription = _parameters.IncludeMarkdownDescription;
            p.Archived = _parameters.Archived;
        }, cancellationToken);
    }

    public IAsyncEnumerable<CuTask> GetAsyncEnumerableAsync(CancellationToken cancellationToken = default)
    {
        // Create a new GetTasksRequestParameters instance to pass to the service method,
        // copying configured values. Page will be handled by the enumerable.
        var serviceParameters = new GetTasksRequestParameters
        {
            Page = _parameters.Page, // Though enumerable handles pages, pass initial if set
            SortBy = _parameters.SortBy,
            Subtasks = _parameters.Subtasks,
            SpaceIds = _parameters.SpaceIds,
            ProjectIds = _parameters.ProjectIds,
            ListIds = _parameters.ListIds,
            Statuses = _parameters.Statuses,
            IncludeClosed = _parameters.IncludeClosed,
            AssigneeIds = _parameters.AssigneeIds,
            Tags = _parameters.Tags,
            DueDateRange = _parameters.DueDateRange,
            DateCreatedRange = _parameters.DateCreatedRange,
            DateUpdatedRange = _parameters.DateUpdatedRange,
            CustomFields = _parameters.CustomFields,
            CustomItems = _parameters.CustomItems,
            IncludeMarkdownDescription = _parameters.IncludeMarkdownDescription,
            Archived = _parameters.Archived
        };
        return _tasksService.GetFilteredTeamTasksAsyncEnumerableAsync(_workspaceId, serviceParameters, cancellationToken);
    }
}