using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentDeleteDependencyRequest
{
    private string? _dependsOnTaskId;
    private string? _dependencyOfTaskId;
    private bool? _customTaskIds;
    private string? _teamId;

    private readonly string _taskId;
    private readonly ITaskRelationshipsService _taskRelationshipsService;

    public FluentDeleteDependencyRequest(string taskId, ITaskRelationshipsService taskRelationshipsService)
    {
        _taskId = taskId;
        _taskRelationshipsService = taskRelationshipsService;
    }

    public FluentDeleteDependencyRequest WithDependsOnTaskId(string dependsOnTaskId)
    {
        _dependsOnTaskId = dependsOnTaskId;
        return this;
    }

    public FluentDeleteDependencyRequest WithDependencyOfTaskId(string dependencyOfTaskId)
    {
        _dependencyOfTaskId = dependencyOfTaskId;
        return this;
    }

    public FluentDeleteDependencyRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public FluentDeleteDependencyRequest WithTeamId(string teamId)
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