using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TasksFluentGetFilteredTeamRequest
{
    private readonly GetFilteredTeamTasksRequest _request = new();
    private readonly string _workspaceId;
    private readonly ITasksService _tasksService;

    public TasksFluentGetFilteredTeamRequest(string workspaceId, ITasksService tasksService)
    {
        _workspaceId = workspaceId;
        _tasksService = tasksService;
    }

    public TasksFluentGetFilteredTeamRequest WithPage(int page) { _request.Page = page; return this; }
    public TasksFluentGetFilteredTeamRequest WithOrderBy(string orderBy) { _request.OrderBy = orderBy; return this; }
    public TasksFluentGetFilteredTeamRequest WithReverse(bool reverse) { _request.Reverse = reverse; return this; }
    public TasksFluentGetFilteredTeamRequest WithSubtasks(bool subtasks) { _request.Subtasks = subtasks; return this; }
    public TasksFluentGetFilteredTeamRequest WithSpaceIds(IEnumerable<string> spaceIds) { _request.SpaceIds = spaceIds; return this; }
    public TasksFluentGetFilteredTeamRequest WithProjectIds(IEnumerable<string> projectIds) { _request.ProjectIds = projectIds; return this; }
    public TasksFluentGetFilteredTeamRequest WithListIds(IEnumerable<string> listIds) { _request.ListIds = listIds; return this; }
    public TasksFluentGetFilteredTeamRequest WithStatuses(IEnumerable<string> statuses) { _request.Statuses = statuses; return this; }
    public TasksFluentGetFilteredTeamRequest WithIncludeClosed(bool includeClosed) { _request.IncludeClosed = includeClosed; return this; }
    public TasksFluentGetFilteredTeamRequest WithAssignees(IEnumerable<string> assignees) { _request.Assignees = assignees; return this; }
    public TasksFluentGetFilteredTeamRequest WithTags(IEnumerable<string> tags) { _request.Tags = tags; return this; }
    public TasksFluentGetFilteredTeamRequest WithDueDateGreaterThan(long dueDateGreaterThan) { _request.DueDateGreaterThan = dueDateGreaterThan; return this; }
    public TasksFluentGetFilteredTeamRequest WithDueDateLessThan(long dueDateLessThan) { _request.DueDateLessThan = dueDateLessThan; return this; }
    public TasksFluentGetFilteredTeamRequest WithDateCreatedGreaterThan(long dateCreatedGreaterThan) { _request.DateCreatedGreaterThan = dateCreatedGreaterThan; return this; }
    public TasksFluentGetFilteredTeamRequest WithDateCreatedLessThan(long dateCreatedLessThan) { _request.DateCreatedLessThan = dateCreatedLessThan; return this; }
    public TasksFluentGetFilteredTeamRequest WithDateUpdatedGreaterThan(long dateUpdatedGreaterThan) { _request.DateUpdatedGreaterThan = dateUpdatedGreaterThan; return this; }
    public TasksFluentGetFilteredTeamRequest WithDateUpdatedLessThan(long dateUpdatedLessThan) { _request.DateUpdatedLessThan = dateUpdatedLessThan; return this; }
    public TasksFluentGetFilteredTeamRequest WithCustomFields(string customFields) { _request.CustomFields = customFields; return this; }
    public TasksFluentGetFilteredTeamRequest WithCustomTaskIds(bool customTaskIds) { _request.CustomTaskIds = customTaskIds; return this; }
    public TasksFluentGetFilteredTeamRequest WithTeamIdForCustomTaskIds(string teamIdForCustomTaskIds) { _request.TeamIdForCustomTaskIds = teamIdForCustomTaskIds; return this; }
    public TasksFluentGetFilteredTeamRequest WithCustomItems(IEnumerable<long> customItems) { _request.CustomItems = customItems; return this; }
    public TasksFluentGetFilteredTeamRequest WithDateDoneGreaterThan(long dateDoneGreaterThan) { _request.DateDoneGreaterThan = dateDoneGreaterThan; return this; }
    public TasksFluentGetFilteredTeamRequest WithDateDoneLessThan(long dateDoneLessThan) { _request.DateDoneLessThan = dateDoneLessThan; return this; }
    public TasksFluentGetFilteredTeamRequest WithParentTaskId(string parentTaskId) { _request.ParentTaskId = parentTaskId; return this; }
    public TasksFluentGetFilteredTeamRequest WithIncludeMarkdownDescription(bool includeMarkdownDescription) { _request.IncludeMarkdownDescription = includeMarkdownDescription; return this; }

    public async Task<GetTasksResponse> GetAsync(System.Threading.CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetFilteredTeamTasksAsync(
            _workspaceId,
            _request,
            cancellationToken
        );
    }
}