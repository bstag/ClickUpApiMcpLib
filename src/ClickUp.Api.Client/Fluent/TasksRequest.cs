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
        // The configureParameters action in ITasksService.GetTasksAsync will effectively use the state of _parameters
        return await _tasksService.GetTasksAsync(_listId, p =>
        {
            p.Archived = _parameters.Archived;
            p.IncludeMarkdownDescription = _parameters.IncludeMarkdownDescription;
            p.Page = _parameters.Page;
            p.SortBy = _parameters.SortBy;
            p.Subtasks = _parameters.Subtasks;
            p.Statuses = _parameters.Statuses;
            p.IncludeClosed = _parameters.IncludeClosed;
            p.AssigneeIds = _parameters.AssigneeIds;
            p.Tags = _parameters.Tags;
            p.DueDateRange = _parameters.DueDateRange;
            p.DateCreatedRange = _parameters.DateCreatedRange;
            p.DateUpdatedRange = _parameters.DateUpdatedRange;
            p.CustomFields = _parameters.CustomFields;
            p.CustomItems = _parameters.CustomItems;
            p.SpaceIds = _parameters.SpaceIds;
            p.ProjectIds = _parameters.ProjectIds;
            // ListIds is typically implicitly set by the _listId path parameter for this specific service call,
            // but GetTasksRequestParameters can hold it if needed for other contexts (like GetFilteredTeamTasksAsync).
            // For GetTasksAsync(listId, ...), _parameters.ListIds will be ignored by the service method implementation,
            // as the primary listId is taken from the path.
        }, cancellationToken);
    }

    public IAsyncEnumerable<CuTask> GetAsyncEnumerableAsync(CancellationToken cancellationToken = default)
    {
        // Create a new GetTasksRequestParameters instance to pass to the service method,
        // copying configured values. Page will be handled by the enumerable.
        var serviceParameters = new GetTasksRequestParameters
        {
            Archived = _parameters.Archived,
            IncludeMarkdownDescription = _parameters.IncludeMarkdownDescription,
            SortBy = _parameters.SortBy,
            Subtasks = _parameters.Subtasks,
            Statuses = _parameters.Statuses,
            IncludeClosed = _parameters.IncludeClosed,
            AssigneeIds = _parameters.AssigneeIds,
            Tags = _parameters.Tags,
            DueDateRange = _parameters.DueDateRange,
            DateCreatedRange = _parameters.DateCreatedRange,
            DateUpdatedRange = _parameters.DateUpdatedRange,
            CustomFields = _parameters.CustomFields,
            CustomItems = _parameters.CustomItems,
            SpaceIds = _parameters.SpaceIds,
            ProjectIds = _parameters.ProjectIds,
            ListIds = _parameters.ListIds // Include ListIds from parameters if set, though primary listId is path param
        };
        // Page is omitted as the service's AsyncEnumerable method handles pagination.
        return _tasksService.GetTasksAsyncEnumerableAsync(_listId, serviceParameters, cancellationToken);
    }
}
