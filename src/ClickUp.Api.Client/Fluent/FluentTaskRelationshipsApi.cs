using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentTaskRelationshipsApi
{
    private readonly ITaskRelationshipsService _taskRelationshipsService;

    public FluentTaskRelationshipsApi(ITaskRelationshipsService taskRelationshipsService)
    {
        _taskRelationshipsService = taskRelationshipsService;
    }

    public FluentAddDependencyRequest AddDependency(string taskId)
    {
        return new FluentAddDependencyRequest(taskId, _taskRelationshipsService);
    }

    public async Task DeleteDependencyAsync(string taskId, string? dependsOnTaskId = null, string? dependencyOfTaskId = null, bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        await _taskRelationshipsService.DeleteDependencyAsync(taskId, dependsOnTaskId, dependencyOfTaskId, customTaskIds, teamId, cancellationToken);
    }

    public FluentDeleteDependencyRequest DeleteDependency(string taskId)
    {
        return new FluentDeleteDependencyRequest(taskId, _taskRelationshipsService);
    }

    public FluentAddTaskLinkRequest AddTaskLink(string taskId, string linksToTaskId)
    {
        return new FluentAddTaskLinkRequest(taskId, linksToTaskId, _taskRelationshipsService);
    }

    public FluentDeleteTaskLinkRequest DeleteTaskLink(string taskId, string linksToTaskId)
    {
        return new FluentDeleteTaskLinkRequest(taskId, linksToTaskId, _taskRelationshipsService);
    }
}
