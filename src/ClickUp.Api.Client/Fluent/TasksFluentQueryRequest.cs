using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TasksFluentQueryRequest
{
    private readonly GetTasksRequest _request = new();
    private readonly string _listId;
    private readonly ITasksService _tasksService;

    public TasksFluentQueryRequest(string listId, ITasksService tasksService)
    {
        _listId = listId;
        _tasksService = tasksService;
    }

    public TasksFluentQueryRequest WithArchived(bool archived)
    {
        _request.Archived = archived;
        return this;
    }

    public TasksFluentQueryRequest WithIncludeMarkdownDescription(bool includeMarkdownDescription)
    {
        _request.IncludeMarkdownDescription = includeMarkdownDescription;
        return this;
    }

    public TasksFluentQueryRequest WithPage(int page)
    {
        _request.Page = page;
        return this;
    }

    public TasksFluentQueryRequest WithOrderBy(string orderBy)
    {
        _request.OrderBy = orderBy;
        return this;
    }

    public TasksFluentQueryRequest WithReverse(bool reverse)
    {
        _request.Reverse = reverse;
        return this;
    }

    public TasksFluentQueryRequest WithSubtasks(bool subtasks)
    {
        _request.Subtasks = subtasks;
        return this;
    }

    public TasksFluentQueryRequest WithStatuses(IEnumerable<string> statuses)
    {
        _request.Statuses = statuses;
        return this;
    }

    public TasksFluentQueryRequest WithIncludeClosed(bool includeClosed)
    {
        _request.IncludeClosed = includeClosed;
        return this;
    }

    public TasksFluentQueryRequest WithAssignees(IEnumerable<string> assignees)
    {
        _request.Assignees = assignees;
        return this;
    }

    public TasksFluentQueryRequest WithWatchers(IEnumerable<string> watchers)
    {
        _request.Watchers = watchers;
        return this;
    }

    public TasksFluentQueryRequest WithTags(IEnumerable<string> tags)
    {
        _request.Tags = tags;
        return this;
    }

    public TasksFluentQueryRequest WithDueDateGreaterThan(long dueDateGreaterThan)
    {
        _request.DueDateGreaterThan = dueDateGreaterThan;
        return this;
    }

    public TasksFluentQueryRequest WithDueDateLessThan(long dueDateLessThan)
    {
        _request.DueDateLessThan = dueDateLessThan;
        return this;
    }

    public TasksFluentQueryRequest WithDateCreatedGreaterThan(long dateCreatedGreaterThan)
    {
        _request.DateCreatedGreaterThan = dateCreatedGreaterThan;
        return this;
    }

    public TasksFluentQueryRequest WithDateCreatedLessThan(long dateCreatedLessThan)
    {
        _request.DateCreatedLessThan = dateCreatedLessThan;
        return this;
    }

    public TasksFluentQueryRequest WithDateUpdatedGreaterThan(long dateUpdatedGreaterThan)
    {
        _request.DateUpdatedGreaterThan = dateUpdatedGreaterThan;
        return this;
    }

    public TasksFluentQueryRequest WithDateUpdatedLessThan(long dateUpdatedLessThan)
    {
        _request.DateUpdatedLessThan = dateUpdatedLessThan;
        return this;
    }

    public TasksFluentQueryRequest WithDateDoneGreaterThan(long dateDoneGreaterThan)
    {
        _request.DateDoneGreaterThan = dateDoneGreaterThan;
        return this;
    }

    public TasksFluentQueryRequest WithDateDoneLessThan(long dateDoneLessThan)
    {
        _request.DateDoneLessThan = dateDoneLessThan;
        return this;
    }

    public TasksFluentQueryRequest WithCustomFields(string customFields)
    {
        _request.CustomFields = customFields;
        return this;
    }

    public TasksFluentQueryRequest WithCustomItems(IEnumerable<long> customItems)
    {
        _request.CustomItems = customItems;
        return this;
    }

    public async Task<GetTasksResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetTasksAsync(_listId, _request, cancellationToken);
    }

    public IAsyncEnumerable<Models.Entities.Tasks.CuTask> GetAsyncEnumerableAsync(CancellationToken cancellationToken = default)
    {
        // The _request DTO is already populated by the With... methods.
        // The Page property in _request will be ignored by the service layer's GetTasksAsyncEnumerableAsync,
        // as it handles its own pagination.
        return _tasksService.GetTasksAsyncEnumerableAsync(_listId, _request, cancellationToken);
    }
}
