using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentTasksApi
{
    private readonly ITasksService _tasksService;

    public FluentTasksApi(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }

    public FluentGetTasksRequest Get(string listId)
    {
        return new FluentGetTasksRequest(listId, _tasksService);
    }

    public FluentGetFilteredTeamTasksRequest GetFilteredTeamTasks(string workspaceId)
    {
        return new FluentGetFilteredTeamTasksRequest(workspaceId, _tasksService);
    }

    public FluentCreateTaskRequest CreateTask(string listId)
    {
        return new FluentCreateTaskRequest(listId, _tasksService);
    }

    public FluentUpdateTaskRequest UpdateTask(string taskId)
    {
        return new FluentUpdateTaskRequest(taskId, _tasksService);
    }

    public FluentGetTaskRequest GetTask(string taskId)
    {
        return new FluentGetTaskRequest(taskId, _tasksService);
    }

    public FluentDeleteTaskRequest DeleteTask(string taskId)
    {
        return new FluentDeleteTaskRequest(taskId, _tasksService);
    }

    public FluentMergeTasksRequest MergeTasks(string targetTaskId)
    {
        return new FluentMergeTasksRequest(targetTaskId, _tasksService);
    }

    public FluentGetTaskTimeInStatusRequest GetTaskTimeInStatus(string taskId)
    {
        return new FluentGetTaskTimeInStatusRequest(taskId, _tasksService);
    }

    public FluentGetBulkTasksTimeInStatusRequest GetBulkTasksTimeInStatus(IEnumerable<string> taskIds)
    {
        return new FluentGetBulkTasksTimeInStatusRequest(taskIds, _tasksService);
    }

    public FluentCreateTaskFromTemplateRequest CreateTaskFromTemplate(string listId, string templateId)
    {
        return new FluentCreateTaskFromTemplateRequest(listId, templateId, _tasksService);
    }

    public IAsyncEnumerable<CuTask> GetTasksAsyncEnumerableAsync(string listId, bool? archived = null, bool? includeMarkdownDescription = null, string? orderBy = null, bool? reverse = null, bool? subtasks = null, IEnumerable<string>? statuses = null, bool? includeClosed = null, IEnumerable<string>? assignees = null, IEnumerable<string>? watchers = null, IEnumerable<string>? tags = null, long? dueDateGreaterThan = null, long? dueDateLessThan = null, long? dateCreatedGreaterThan = null, long? dateCreatedLessThan = null, long? dateUpdatedGreaterThan = null, long? dateUpdatedLessThan = null, long? dateDoneGreaterThan = null, long? dateDoneLessThan = null, string? customFields = null, IEnumerable<long>? customItems = null, CancellationToken cancellationToken = default)
    {
        return _tasksService.GetTasksAsyncEnumerableAsync(listId, archived, includeMarkdownDescription, orderBy, reverse, subtasks, statuses, includeClosed, assignees, watchers, tags, dueDateGreaterThan, dueDateLessThan, dateCreatedGreaterThan, dateCreatedLessThan, dateUpdatedGreaterThan, dateUpdatedLessThan, dateDoneGreaterThan, dateDoneLessThan, customFields, customItems, cancellationToken);
    }

    public IAsyncEnumerable<CuTask> GetFilteredTeamTasksAsyncEnumerableAsync(string workspaceId, string? orderBy = null, bool? reverse = null, bool? subtasks = null, IEnumerable<string>? spaceIds = null, IEnumerable<string>? projectIds = null, IEnumerable<string>? listIds = null, IEnumerable<string>? statuses = null, bool? includeClosed = null, IEnumerable<string>? assignees = null, IEnumerable<string>? tags = null, long? dueDateGreaterThan = null, long? dueDateLessThan = null, long? dateCreatedGreaterThan = null, long? dateCreatedLessThan = null, long? dateUpdatedGreaterThan = null, long? dateUpdatedLessThan = null, string? customFields = null, bool? customTaskIds = null, string? teamIdForCustomTaskIds = null, IEnumerable<long>? customItems = null, long? dateDoneGreaterThan = null, long? dateDoneLessThan = null, string? parentTaskId = null, bool? includeMarkdownDescription = null, CancellationToken cancellationToken = default)
    {
        return _tasksService.GetFilteredTeamTasksAsyncEnumerableAsync(workspaceId, orderBy, reverse, subtasks, spaceIds, projectIds, listIds, statuses, includeClosed, assignees, tags, dueDateGreaterThan, dueDateLessThan, dateCreatedGreaterThan, dateCreatedLessThan, dateUpdatedGreaterThan, dateUpdatedLessThan, customFields, customTaskIds, teamIdForCustomTaskIds, customItems, dateDoneGreaterThan, dateDoneLessThan, parentTaskId, includeMarkdownDescription, cancellationToken);
    }
}
