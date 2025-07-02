using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TasksRequest
{
    private readonly GetTasksRequest _request = new();
    private readonly string _listId;
    private readonly ITasksService _tasksService;

    public TasksRequest(string listId, ITasksService tasksService)
    {
        _listId = listId;
        _tasksService = tasksService;
    }

    public TasksRequest WithArchived(bool archived)
    {
        _request.Archived = archived;
        return this;
    }

    public TasksRequest WithIncludeMarkdownDescription(bool includeMarkdownDescription)
    {
        _request.IncludeMarkdownDescription = includeMarkdownDescription;
        return this;
    }

    public TasksRequest WithPage(int page)
    {
        _request.Page = page;
        return this;
    }

    public TasksRequest WithOrderBy(string orderBy)
    {
        _request.OrderBy = orderBy;
        return this;
    }

    public TasksRequest WithReverse(bool reverse)
    {
        _request.Reverse = reverse;
        return this;
    }

    public TasksRequest WithSubtasks(bool subtasks)
    {
        _request.Subtasks = subtasks;
        return this;
    }

    public TasksRequest WithStatuses(IEnumerable<string> statuses)
    {
        _request.Statuses = statuses;
        return this;
    }

    public TasksRequest WithIncludeClosed(bool includeClosed)
    {
        _request.IncludeClosed = includeClosed;
        return this;
    }

    public TasksRequest WithAssignees(IEnumerable<string> assignees)
    {
        _request.Assignees = assignees;
        return this;
    }

    public TasksRequest WithWatchers(IEnumerable<string> watchers)
    {
        _request.Watchers = watchers;
        return this;
    }

    public TasksRequest WithTags(IEnumerable<string> tags)
    {
        _request.Tags = tags;
        return this;
    }

    public TasksRequest WithDueDateGreaterThan(long dueDateGreaterThan)
    {
        _request.DueDateGreaterThan = dueDateGreaterThan;
        return this;
    }

    public TasksRequest WithDueDateLessThan(long dueDateLessThan)
    {
        _request.DueDateLessThan = dueDateLessThan;
        return this;
    }

    public TasksRequest WithDateCreatedGreaterThan(long dateCreatedGreaterThan)
    {
        _request.DateCreatedGreaterThan = dateCreatedGreaterThan;
        return this;
    }

    public TasksRequest WithDateCreatedLessThan(long dateCreatedLessThan)
    {
        _request.DateCreatedLessThan = dateCreatedLessThan;
        return this;
    }

    public TasksRequest WithDateUpdatedGreaterThan(long dateUpdatedGreaterThan)
    {
        _request.DateUpdatedGreaterThan = dateUpdatedGreaterThan;
        return this;
    }

    public TasksRequest WithDateUpdatedLessThan(long dateUpdatedLessThan)
    {
        _request.DateUpdatedLessThan = dateUpdatedLessThan;
        return this;
    }

    public TasksRequest WithDateDoneGreaterThan(long dateDoneGreaterThan)
    {
        _request.DateDoneGreaterThan = dateDoneGreaterThan;
        return this;
    }

    public TasksRequest WithDateDoneLessThan(long dateDoneLessThan)
    {
        _request.DateDoneLessThan = dateDoneLessThan;
        return this;
    }

    public TasksRequest WithCustomFields(string customFields)
    {
        _request.CustomFields = customFields;
        return this;
    }

    public TasksRequest WithCustomItems(IEnumerable<long> customItems)
    {
        _request.CustomItems = customItems;
        return this;
    }

    public async Task<Models.Common.Pagination.IPagedResult<Models.Entities.Tasks.CuTask>> GetAsync(CancellationToken cancellationToken = default)
    {
        // Page is set on _request by WithPage()
        return await _tasksService.GetTasksAsync(_listId, _request, _request.Page, cancellationToken);
    }

    public IAsyncEnumerable<Models.Entities.Tasks.CuTask> GetAsyncEnumerableAsync(CancellationToken cancellationToken = default)
    {
        // The _request DTO is already populated by the With... methods.
        // The Page property in _request will be ignored by the service layer's GetTasksAsyncEnumerableAsync,
        // as it handles its own pagination.
        return _tasksService.GetTasksAsyncEnumerableAsync(_listId, _request, cancellationToken);
    }
}
