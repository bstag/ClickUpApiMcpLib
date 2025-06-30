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

    public TasksRequest Get(string listId)
    {
        return new TasksRequest(listId, _tasksService);
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

    public TaskFluentGetRequest GetTask(string taskId)
    {
        return new TaskFluentGetRequest(taskId, _tasksService);
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

    public IAsyncEnumerable<CuTask> GetTasksAsyncEnumerableAsync(string listId, GetTasksRequest requestModel, CancellationToken cancellationToken = default)
    {
        return _tasksService.GetTasksAsyncEnumerableAsync(listId, requestModel, cancellationToken);
    }

    public IAsyncEnumerable<CuTask> GetFilteredTeamTasksAsyncEnumerableAsync(string workspaceId, GetFilteredTeamTasksRequest requestModel, CancellationToken cancellationToken = default)
    {
        return _tasksService.GetFilteredTeamTasksAsyncEnumerableAsync(workspaceId, requestModel, cancellationToken);
    }
}
