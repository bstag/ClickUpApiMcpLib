using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentGetTasksRequest
{
    private readonly GetTasksRequest _request = new();
    private readonly string _listId;
    private readonly ITasksService _tasksService;

    public FluentGetTasksRequest(string listId, ITasksService tasksService)
    {
        _listId = listId;
        _tasksService = tasksService;
    }

    public FluentGetTasksRequest WithArchived(bool archived)
    {
        _request.Archived = archived;
        return this;
    }

    public FluentGetTasksRequest WithIncludeMarkdownDescription(bool includeMarkdownDescription)
    {
        _request.IncludeMarkdownDescription = includeMarkdownDescription;
        return this;
    }

    public FluentGetTasksRequest WithPage(int page)
    {
        _request.Page = page;
        return this;
    }

    public FluentGetTasksRequest WithOrderBy(string orderBy)
    {
        _request.OrderBy = orderBy;
        return this;
    }

    public FluentGetTasksRequest WithReverse(bool reverse)
    {
        _request.Reverse = reverse;
        return this;
    }

    public FluentGetTasksRequest WithSubtasks(bool subtasks)
    {
        _request.Subtasks = subtasks;
        return this;
    }

    public FluentGetTasksRequest WithStatuses(IEnumerable<string> statuses)
    {
        _request.Statuses = statuses;
        return this;
    }

    public FluentGetTasksRequest WithIncludeClosed(bool includeClosed)
    {
        _request.IncludeClosed = includeClosed;
        return this;
    }

    public FluentGetTasksRequest WithAssignees(IEnumerable<string> assignees)
    {
        _request.Assignees = assignees;
        return this;
    }

    public FluentGetTasksRequest WithWatchers(IEnumerable<string> watchers)
    {
        _request.Watchers = watchers;
        return this;
    }

    public FluentGetTasksRequest WithTags(IEnumerable<string> tags)
    {
        _request.Tags = tags;
        return this;
    }

    public FluentGetTasksRequest WithDueDateGreaterThan(long dueDateGreaterThan)
    {
        _request.DueDateGreaterThan = dueDateGreaterThan;
        return this;
    }

    public FluentGetTasksRequest WithDueDateLessThan(long dueDateLessThan)
    {
        _request.DueDateLessThan = dueDateLessThan;
        return this;
    }

    public FluentGetTasksRequest WithDateCreatedGreaterThan(long dateCreatedGreaterThan)
    {
        _request.DateCreatedGreaterThan = dateCreatedGreaterThan;
        return this;
    }

    public FluentGetTasksRequest WithDateCreatedLessThan(long dateCreatedLessThan)
    {
        _request.DateCreatedLessThan = dateCreatedLessThan;
        return this;
    }

    public FluentGetTasksRequest WithDateUpdatedGreaterThan(long dateUpdatedGreaterThan)
    {
        _request.DateUpdatedGreaterThan = dateUpdatedGreaterThan;
        return this;
    }

    public FluentGetTasksRequest WithDateUpdatedLessThan(long dateUpdatedLessThan)
    {
        _request.DateUpdatedLessThan = dateUpdatedLessThan;
        return this;
    }

    public FluentGetTasksRequest WithDateDoneGreaterThan(long dateDoneGreaterThan)
    {
        _request.DateDoneGreaterThan = dateDoneGreaterThan;
        return this;
    }

    public FluentGetTasksRequest WithDateDoneLessThan(long dateDoneLessThan)
    {
        _request.DateDoneLessThan = dateDoneLessThan;
        return this;
    }

    public FluentGetTasksRequest WithCustomFields(string customFields)
    {
        _request.CustomFields = customFields;
        return this;
    }

    public FluentGetTasksRequest WithCustomItems(IEnumerable<long> customItems)
    {
        _request.CustomItems = customItems;
        return this;
    }

    public async Task<GetTasksResponse> GetAsync()
    {
        return await _tasksService.GetTasksAsync(_listId, _request);
    }
}
