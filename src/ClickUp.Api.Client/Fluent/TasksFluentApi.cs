using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TasksFluentApi
{
    private readonly ITasksService _tasksService;

    public TasksFluentApi(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }

    public TasksFluentQueryRequest Get(string listId)
    {
        return new TasksFluentQueryRequest(listId, _tasksService);
    }

    public TasksFluentGetFilteredTeamRequest GetFilteredTeamTasks(string workspaceId)
    {
        return new TasksFluentGetFilteredTeamRequest(workspaceId, _tasksService);
    }

    public TaskFluentCreateRequest CreateTask(string listId)
    {
        return new TaskFluentCreateRequest(listId, _tasksService);
    }

    public TaskFluentUpdateRequest UpdateTask(string taskId)
    {
        return new TaskFluentUpdateRequest(taskId, _tasksService);
    }

    public TaskFluentGetSingleRequest GetTask(string taskId)
    {
        return new TaskFluentGetSingleRequest(taskId, _tasksService);
    }

    public TaskFluentDeleteRequest DeleteTask(string taskId)
    {
        return new TaskFluentDeleteRequest(taskId, _tasksService);
    }

    public TasksFluentMergeRequest MergeTasks(string targetTaskId)
    {
        return new TasksFluentMergeRequest(targetTaskId, _tasksService);
    }

    public TaskTimeInStatusFluentGetRequest GetTaskTimeInStatus(string taskId)
    {
        return new TaskTimeInStatusFluentGetRequest(taskId, _tasksService);
    }

    public TimeInStatusFluentGetBulkTasksRequest GetBulkTasksTimeInStatus(IEnumerable<string> taskIds)
    {
        return new TimeInStatusFluentGetBulkTasksRequest(taskIds, _tasksService);
    }

    public TemplateFluentCreateTaskRequest CreateTaskFromTemplate(string listId, string templateId)
    {
        return new TemplateFluentCreateTaskRequest(listId, templateId, _tasksService);
    }

    public IAsyncEnumerable<CuTask> GetTasksAsyncEnumerableAsync(string listId, bool? archived = null, bool? includeMarkdownDescription = null, string? orderBy = null, bool? reverse = null, bool? subtasks = null, IEnumerable<string>? statuses = null, bool? includeClosed = null, IEnumerable<string>? assignees = null, IEnumerable<string>? watchers = null, IEnumerable<string>? tags = null, long? dueDateGreaterThan = null, long? dueDateLessThan = null, long? dateCreatedGreaterThan = null, long? dateCreatedLessThan = null, long? dateUpdatedGreaterThan = null, long? dateUpdatedLessThan = null, long? dateDoneGreaterThan = null, long? dateDoneLessThan = null, string? customFields = null, IEnumerable<long>? customItems = null, CancellationToken cancellationToken = default)
    {
        var requestModel = new GetTasksRequest
        {
            Archived = archived,
            IncludeMarkdownDescription = includeMarkdownDescription,
            OrderBy = orderBy,
            Reverse = reverse,
            Subtasks = subtasks,
            Statuses = statuses,
            IncludeClosed = includeClosed,
            Assignees = assignees,
            Watchers = watchers,
            Tags = tags,
            DueDateGreaterThan = dueDateGreaterThan,
            DueDateLessThan = dueDateLessThan,
            DateCreatedGreaterThan = dateCreatedGreaterThan,
            DateCreatedLessThan = dateCreatedLessThan,
            DateUpdatedGreaterThan = dateUpdatedGreaterThan,
            DateUpdatedLessThan = dateUpdatedLessThan,
            DateDoneGreaterThan = dateDoneGreaterThan,
            DateDoneLessThan = dateDoneLessThan,
            CustomFields = customFields,
            CustomItems = customItems
            // Page is intentionally omitted as it's handled by the streaming service method
        };
        return _tasksService.GetTasksAsyncEnumerableAsync(listId, requestModel, cancellationToken);
    }

    public IAsyncEnumerable<CuTask> GetFilteredTeamTasksAsyncEnumerableAsync(string workspaceId, GetFilteredTeamTasksRequest requestModel, CancellationToken cancellationToken = default)
    {
        return _tasksService.GetFilteredTeamTasksAsyncEnumerableAsync(workspaceId, requestModel, cancellationToken);
    }
}
