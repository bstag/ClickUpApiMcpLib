using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.RequestModels.TaskRelationships; // Required for DeleteDependencyRequest

namespace ClickUp.Api.Client.Fluent;

public class TaskRelationshipsFluentApi
{
    private readonly ITaskRelationshipsService _taskRelationshipsService;

    public TaskRelationshipsFluentApi(ITaskRelationshipsService taskRelationshipsService)
    {
        _taskRelationshipsService = taskRelationshipsService;
    }

    public DependencyFluentAddRequest AddDependency(string taskId)
    {
        return new DependencyFluentAddRequest(taskId, _taskRelationshipsService);
    }

    public async Task DeleteDependencyAsync(string taskId, string? dependsOnTaskId = null, string? dependencyOfTaskId = null, bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        var requestModel = new DeleteDependencyRequest(dependsOnTaskId, dependencyOfTaskId, customTaskIds, teamId);
        await _taskRelationshipsService.DeleteDependencyAsync(taskId, requestModel, cancellationToken);
    }

    public DependencyFluentDeleteRequest DeleteDependency(string taskId)
    {
        return new DependencyFluentDeleteRequest(taskId, _taskRelationshipsService);
    }

    public TaskLinkFluentAddRequest AddTaskLink(string taskId, string linksToTaskId)
    {
        return new TaskLinkFluentAddRequest(taskId, linksToTaskId, _taskRelationshipsService);
    }

    public TaskLinkFluentDeleteRequest DeleteTaskLink(string taskId, string linksToTaskId)
    {
        return new TaskLinkFluentDeleteRequest(taskId, linksToTaskId, _taskRelationshipsService);
    }
}
