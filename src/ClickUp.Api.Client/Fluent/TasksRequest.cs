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

public class TasksRequest
{
    private readonly GetTasksRequestParameters _parameters = new();
    private readonly string _listId;
    private readonly ITasksService _tasksService;

    public TasksRequest(string listId, ITasksService tasksService)
    {
        _listId = listId;
        _tasksService = tasksService;
    }

    public TasksRequest WithArchived(bool archived)
    {
        _parameters.Archived = archived;
        return this;
    }

    // Assuming IncludeMarkdownDescription is not part of GetTasksRequestParameters,
    // but if it were, it would be:
    // public TasksRequest WithIncludeMarkdownDescription(bool includeMarkdownDescription)
    // {
    //     _parameters.IncludeMarkdownDescription = includeMarkdownDescription; // Example, if this property existed
    //     return this;
    // }

    public TasksRequest WithPage(int page)
    {
        _parameters.Page = page;
        return this;
    }

    public TasksRequest OrderBy(string fieldName, SortDirection direction = SortDirection.Ascending)
    {
        _parameters.SortBy = new SortOption(fieldName, direction);
        return this;
    }

    public TasksRequest WithSubtasks(bool subtasks)
    {
        _parameters.Subtasks = subtasks;
        return this;
    }

    public TasksRequest WithStatuses(IEnumerable<string> statuses)
    {
        _parameters.Statuses = statuses;
        return this;
    }

    public TasksRequest WithIncludeClosed(bool includeClosed)
    {
        _parameters.IncludeClosed = includeClosed;
        return this;
    }

    public TasksRequest WithAssignees(IEnumerable<long> assignees) // Changed from string to long to match GetTasksRequestParameters
    {
        _parameters.Assignees = assignees;
        return this;
    }

    // Watchers are not in GetTasksRequestParameters for typical GetTasks, often a GetSingleTask detail.
    // public TasksRequest WithWatchers(IEnumerable<string> watchers)
    // {
    //     // _parameters.Watchers = watchers; // Example
    //     return this;
    // }

    // Tags are not directly in GetTasksRequestParameters, might be part of a more complex filter or custom field.
    // public TasksRequest WithTags(IEnumerable<string> tags)
    // {
    //     // _parameters.Tags = tags; // Example
    //     return this;
    // }

    public TasksRequest WithDueDateBetween(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        _parameters.DueDateRange = new TimeRange(startDate, endDate);
        return this;
    }

    public TasksRequest WithDateCreatedBetween(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        _parameters.DateCreatedRange = new TimeRange(startDate, endDate);
        return this;
    }

    public TasksRequest WithDateUpdatedBetween(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        _parameters.DateUpdatedRange = new TimeRange(startDate, endDate);
        return this;
    }

    // DateDone range is not explicitly in GetTasksRequestParameters, but could be added if API supports it.

    public TasksRequest WithCustomField(string fieldId, string value)
    {
        var currentCustomFields = _parameters.CustomFields?.ToList() ?? new List<CustomFieldParameter>();
        currentCustomFields.Add(new CustomFieldParameter(fieldId, value));
        _parameters.CustomFields = currentCustomFields;
        return this;
    }

    // CustomItems (integer IDs) are not in GetTasksRequestParameters.
    // public TasksRequest WithCustomItems(IEnumerable<long> customItems)
    // {
    //     // _parameters.CustomItems = customItems; // Example
    //     return this;
    // }

    public async Task<IPagedResult<CuTask>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetTasksAsync(_listId, p =>
        {
            // Copy all configured _parameters to p
            p.Archived = _parameters.Archived;
            p.Page = _parameters.Page;
            p.SortBy = _parameters.SortBy;
            p.Subtasks = _parameters.Subtasks;
            p.Statuses = _parameters.Statuses;
            p.IncludeClosed = _parameters.IncludeClosed;
            p.Assignees = _parameters.Assignees;
            p.DueDateRange = _parameters.DueDateRange;
            p.DateCreatedRange = _parameters.DateCreatedRange;
            p.DateUpdatedRange = _parameters.DateUpdatedRange;
            p.CustomFields = _parameters.CustomFields;
            // Copy other relevant properties from _parameters to p as needed
        }, cancellationToken);
    }

    public IAsyncEnumerable<CuTask> GetAsyncEnumerableAsync(CancellationToken cancellationToken = default)
    {
        // The ITasksService.GetTasksAsyncEnumerableAsync now also takes Action<GetTasksRequestParameters>
        // However, the old TasksFluentApi.GetTasksAsyncEnumerableAsync directly constructs GetTasksRequest.
        // This fluent builder should ideally call the service method that accepts Action.
        // For now, let's assume TasksFluentApi will be updated, or we adapt here.
        // Simplest adaptation for now, though less ideal as it bypasses the service's direct use of the Action.
        // It's better if TasksFluentApi.GetTasksAsyncEnumerableAsync is updated to pass the action.

        // To correctly use the new service signature:
        return _tasksService.GetTasksAsyncEnumerableAsync(_listId, p =>
        {
            p.Archived = _parameters.Archived;
            // Page is handled by the enumerable internally, so don't set p.Page here from _parameters.Page
            p.SortBy = _parameters.SortBy;
            p.Subtasks = _parameters.Subtasks;
            p.Statuses = _parameters.Statuses;
            p.IncludeClosed = _parameters.IncludeClosed;
            p.Assignees = _parameters.Assignees;
            p.DueDateRange = _parameters.DueDateRange;
            p.DateCreatedRange = _parameters.DateCreatedRange;
            p.DateUpdatedRange = _parameters.DateUpdatedRange;
            p.CustomFields = _parameters.CustomFields;
        }, cancellationToken);
    }
}
