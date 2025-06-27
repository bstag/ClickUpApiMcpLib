using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class DependencyFluentDeleteRequest
{
    private string? _dependsOnTaskId;
    private string? _dependencyOfTaskId;
    private bool? _customTaskIds;
    private string? _teamId;

    private readonly string _taskId;
    private readonly ITaskRelationshipsService _taskRelationshipsService;

    public DependencyFluentDeleteRequest(string taskId, ITaskRelationshipsService taskRelationshipsService)
    {
        _taskId = taskId;
        _taskRelationshipsService = taskRelationshipsService;
    }

    public DependencyFluentDeleteRequest WithDependsOnTaskId(string dependsOnTaskId)
    {
        _dependsOnTaskId = dependsOnTaskId;
        return this;
    }

    public DependencyFluentDeleteRequest WithDependencyOfTaskId(string dependencyOfTaskId)
    {
        _dependencyOfTaskId = dependencyOfTaskId;
        return this;
    }

    public DependencyFluentDeleteRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public DependencyFluentDeleteRequest WithTeamId(string teamId)
    {
        _teamId = teamId;
        return this;
    }

    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        await _taskRelationshipsService.DeleteDependencyAsync(
            _taskId,
            _dependsOnTaskId,
            _dependencyOfTaskId,
            _customTaskIds,
            _teamId,
            cancellationToken
        );
    }
}