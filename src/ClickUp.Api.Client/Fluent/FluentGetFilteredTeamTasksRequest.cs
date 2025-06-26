using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentGetFilteredTeamTasksRequest
{
    private readonly GetFilteredTeamTasksRequest _request = new();
    private readonly string _workspaceId;
    private readonly ITasksService _tasksService;

    public FluentGetFilteredTeamTasksRequest(string workspaceId, ITasksService tasksService)
    {
        _workspaceId = workspaceId;
        _tasksService = tasksService;
    }

    public FluentGetFilteredTeamTasksRequest WithPage(int page) { _request.Page = page; return this; }
    public FluentGetFilteredTeamTasksRequest WithOrderBy(string orderBy) { _request.OrderBy = orderBy; return this; }
    public FluentGetFilteredTeamTasksRequest WithReverse(bool reverse) { _request.Reverse = reverse; return this; }
    public FluentGetFilteredTeamTasksRequest WithSubtasks(bool subtasks) { _request.Subtasks = subtasks; return this; }
    public FluentGetFilteredTeamTasksRequest WithSpaceIds(IEnumerable<string> spaceIds) { _request.SpaceIds = spaceIds; return this; }
    public FluentGetFilteredTeamTasksRequest WithProjectIds(IEnumerable<string> projectIds) { _request.ProjectIds = projectIds; return this; }
    public FluentGetFilteredTeamTasksRequest WithListIds(IEnumerable<string> listIds) { _request.ListIds = listIds; return this; }
    public FluentGetFilteredTeamTasksRequest WithStatuses(IEnumerable<string> statuses) { _request.Statuses = statuses; return this; }
    public FluentGetFilteredTeamTasksRequest WithIncludeClosed(bool includeClosed) { _request.IncludeClosed = includeClosed; return this; }
    public FluentGetFilteredTeamTasksRequest WithAssignees(IEnumerable<string> assignees) { _request.Assignees = assignees; return this; }
    public FluentGetFilteredTeamTasksRequest WithTags(IEnumerable<string> tags) { _request.Tags = tags; return this; }
    public FluentGetFilteredTeamTasksRequest WithDueDateGreaterThan(long dueDateGreaterThan) { _request.DueDateGreaterThan = dueDateGreaterThan; return this; }
    public FluentGetFilteredTeamTasksRequest WithDueDateLessThan(long dueDateLessThan) { _request.DueDateLessThan = dueDateLessThan; return this; }
    public FluentGetFilteredTeamTasksRequest WithDateCreatedGreaterThan(long dateCreatedGreaterThan) { _request.DateCreatedGreaterThan = dateCreatedGreaterThan; return this; }
    public FluentGetFilteredTeamTasksRequest WithDateCreatedLessThan(long dateCreatedLessThan) { _request.DateCreatedLessThan = dateCreatedLessThan; return this; }
    public FluentGetFilteredTeamTasksRequest WithDateUpdatedGreaterThan(long dateUpdatedGreaterThan) { _request.DateUpdatedGreaterThan = dateUpdatedGreaterThan; return this; }
    public FluentGetFilteredTeamTasksRequest WithDateUpdatedLessThan(long dateUpdatedLessThan) { _request.DateUpdatedLessThan = dateUpdatedLessThan; return this; }
    public FluentGetFilteredTeamTasksRequest WithCustomFields(string customFields) { _request.CustomFields = customFields; return this; }
    public FluentGetFilteredTeamTasksRequest WithCustomTaskIds(bool customTaskIds) { _request.CustomTaskIds = customTaskIds; return this; }
    public FluentGetFilteredTeamTasksRequest WithTeamIdForCustomTaskIds(string teamIdForCustomTaskIds) { _request.TeamIdForCustomTaskIds = teamIdForCustomTaskIds; return this; }
    public FluentGetFilteredTeamTasksRequest WithCustomItems(IEnumerable<long> customItems) { _request.CustomItems = customItems; return this; }
    public FluentGetFilteredTeamTasksRequest WithDateDoneGreaterThan(long dateDoneGreaterThan) { _request.DateDoneGreaterThan = dateDoneGreaterThan; return this; }
    public FluentGetFilteredTeamTasksRequest WithDateDoneLessThan(long dateDoneLessThan) { _request.DateDoneLessThan = dateDoneLessThan; return this; }
    public FluentGetFilteredTeamTasksRequest WithParentTaskId(string parentTaskId) { _request.ParentTaskId = parentTaskId; return this; }
    public FluentGetFilteredTeamTasksRequest WithIncludeMarkdownDescription(bool includeMarkdownDescription) { _request.IncludeMarkdownDescription = includeMarkdownDescription; return this; }

    public async Task<GetTasksResponse> GetAsync()
    {
        return await _tasksService.GetFilteredTeamTasksAsync(
            _workspaceId,
            _request.Page,
            _request.OrderBy,
            _request.Reverse,
            _request.Subtasks,
            _request.SpaceIds,
            _request.ProjectIds,
            _request.ListIds,
            _request.Statuses,
            _request.IncludeClosed,
            _request.Assignees,
            _request.Tags,
            _request.DueDateGreaterThan,
            _request.DueDateLessThan,
            _request.DateCreatedGreaterThan,
            _request.DateCreatedLessThan,
            _request.DateUpdatedGreaterThan,
            _request.DateUpdatedLessThan,
            _request.CustomFields,
            _request.CustomTaskIds,
            _request.TeamIdForCustomTaskIds,
            _request.CustomItems,
            _request.DateDoneGreaterThan,
            _request.DateDoneLessThan,
            _request.ParentTaskId,
            _request.IncludeMarkdownDescription
        );
    }
}