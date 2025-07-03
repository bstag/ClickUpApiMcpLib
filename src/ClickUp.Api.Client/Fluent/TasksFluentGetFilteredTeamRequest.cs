using System; // For DateTimeOffset
using System.Collections.Generic;
using System.Linq;
using System.Threading; // For CancellationToken
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Common.ValueObjects; // For TimeRange, SortOption, SortDirection
using ClickUp.Api.Client.Models.Entities.Tasks; // For CuTask
using ClickUp.Api.Client.Models.RequestModels.Parameters; // For GetTasksRequestParameters
using ClickUp.Api.Client.Models.Common.Pagination; // For IPagedResult

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
    public TasksFluentGetFilteredTeamRequest WithSpaceIds(IEnumerable<long> spaceIds) { _parameters.SpaceIds = spaceIds; return this; } // Changed to long
    public TasksFluentGetFilteredTeamRequest WithProjectIds(IEnumerable<long> projectIds) { _parameters.ProjectIds = projectIds; return this; } // Changed to long
    public TasksFluentGetFilteredTeamRequest WithListIds(IEnumerable<long> listIds) { _parameters.ListIds = listIds; return this; } // Changed to long
    public TasksFluentGetFilteredTeamRequest WithStatuses(IEnumerable<string> statuses) { _parameters.Statuses = statuses; return this; }
    public TasksFluentGetFilteredTeamRequest WithIncludeClosed(bool includeClosed) { _parameters.IncludeClosed = includeClosed; return this; }
    public TasksFluentGetFilteredTeamRequest WithAssignees(IEnumerable<long> assignees) { _parameters.Assignees = assignees; return this; } // Changed to long
    // Tags are not part of GetTasksRequestParameters directly
    // public TasksFluentGetFilteredTeamRequest WithTags(IEnumerable<string> tags) { _parameters.Tags = tags; return this; }

    public TasksFluentGetFilteredTeamRequest WithDueDateBetween(DateTimeOffset startDate, DateTimeOffset endDate) { _parameters.DueDateRange = new TimeRange(startDate, endDate); return this; }
    public TasksFluentGetFilteredTeamRequest WithDateCreatedBetween(DateTimeOffset startDate, DateTimeOffset endDate) { _parameters.DateCreatedRange = new TimeRange(startDate, endDate); return this; }
    public TasksFluentGetFilteredTeamRequest WithDateUpdatedBetween(DateTimeOffset startDate, DateTimeOffset endDate) { _parameters.DateUpdatedRange = new TimeRange(startDate, endDate); return this; }

    public TasksFluentGetFilteredTeamRequest WithCustomField(string fieldId, string value)
    {
        var currentCustomFields = _parameters.CustomFields?.ToList() ?? new List<CustomFieldParameter>();
        currentCustomFields.Add(new CustomFieldParameter(fieldId, value));
        _parameters.CustomFields = currentCustomFields;
        return this;
    }

    // CustomTaskIds and TeamIdForCustomTaskIds are not part of GetTasksRequestParameters general filtering.
    // They are usually specific to single task operations or special query modes.
    // public TasksFluentGetFilteredTeamRequest WithCustomTaskIds(bool customTaskIds) { _parameters.CustomTaskIds = customTaskIds; return this; }
    // public TasksFluentGetFilteredTeamRequest WithTeamIdForCustomTaskIds(string teamIdForCustomTaskIds) { _parameters.TeamIdForCustomTaskIds = teamIdForCustomTaskIds; return this; }

    // CustomItems not in GetTasksRequestParameters
    // public TasksFluentGetFilteredTeamRequest WithCustomItems(IEnumerable<long> customItems) { _parameters.CustomItems = customItems; return this; }

    // DateDone range not in GetTasksRequestParameters
    // public TasksFluentGetFilteredTeamRequest WithDateDoneGreaterThan(long dateDoneGreaterThan) { _parameters.DateDoneGreaterThan = dateDoneGreaterThan; return this; }
    // public TasksFluentGetFilteredTeamRequest WithDateDoneLessThan(long dateDoneLessThan) { _parameters.DateDoneLessThan = dateDoneLessThan; return this; }

    // ParentTaskId not in GetTasksRequestParameters general filtering for multiple tasks.
    // public TasksFluentGetFilteredTeamRequest WithParentTaskId(string parentTaskId) { _parameters.ParentTaskId = parentTaskId; return this; }

    // IncludeMarkdownDescription not in GetTasksRequestParameters
    // public TasksFluentGetFilteredTeamRequest WithIncludeMarkdownDescription(bool includeMarkdownDescription) { _parameters.IncludeMarkdownDescription = includeMarkdownDescription; return this; }
    public TasksFluentGetFilteredTeamRequest WithArchived(bool archived) { _parameters.Archived = archived; return this; }


    public async Task<IPagedResult<CuTask>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetFilteredTeamTasksAsync(_workspaceId, p =>
        {
            // Copy all configured _parameters to p
            p.Page = _parameters.Page;
            p.SortBy = _parameters.SortBy;
            p.Subtasks = _parameters.Subtasks;
            p.SpaceIds = _parameters.SpaceIds;
            p.ProjectIds = _parameters.ProjectIds;
            p.ListIds = _parameters.ListIds;
            p.Statuses = _parameters.Statuses;
            p.IncludeClosed = _parameters.IncludeClosed;
            p.Assignees = _parameters.Assignees;
            p.DueDateRange = _parameters.DueDateRange;
            p.DateCreatedRange = _parameters.DateCreatedRange;
            p.DateUpdatedRange = _parameters.DateUpdatedRange;
            p.CustomFields = _parameters.CustomFields;
            p.Archived = _parameters.Archived;
            // Copy other relevant properties from _parameters to p as needed
        }, cancellationToken);
    }

    // Consider adding GetAsyncEnumerableAsync here as well, similar to TasksRequest
    public IAsyncEnumerable<CuTask> GetAsyncEnumerableAsync(CancellationToken cancellationToken = default)
    {
        return _tasksService.GetFilteredTeamTasksAsyncEnumerableAsync(_workspaceId, p =>
        {
            // Copy all configured _parameters to p, except Page
            p.SortBy = _parameters.SortBy;
            p.Subtasks = _parameters.Subtasks;
            p.SpaceIds = _parameters.SpaceIds;
            p.ProjectIds = _parameters.ProjectIds;
            p.ListIds = _parameters.ListIds;
            p.Statuses = _parameters.Statuses;
            p.IncludeClosed = _parameters.IncludeClosed;
            p.Assignees = _parameters.Assignees;
            p.DueDateRange = _parameters.DueDateRange;
            p.DateCreatedRange = _parameters.DateCreatedRange;
            p.DateUpdatedRange = _parameters.DateUpdatedRange;
            p.CustomFields = _parameters.CustomFields;
            p.Archived = _parameters.Archived;
        }, cancellationToken);
    }
}