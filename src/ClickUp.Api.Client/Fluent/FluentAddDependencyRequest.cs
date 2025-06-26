using ClickUp.Api.Client.Abstractions.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentAddDependencyRequest
{
    private string? _dependsOnTaskId;
    private string? _dependencyOfTaskId;
    private bool? _customTaskIds;
    private string? _teamId;

    private readonly string _taskId;
    private readonly ITaskRelationshipsService _taskRelationshipsService;

    public FluentAddDependencyRequest(string taskId, ITaskRelationshipsService taskRelationshipsService)
    {
        _taskId = taskId;
        _taskRelationshipsService = taskRelationshipsService;
    }

    public FluentAddDependencyRequest WithDependsOnTaskId(string dependsOnTaskId)
    {
        _dependsOnTaskId = dependsOnTaskId;
        return this;
    }

    public FluentAddDependencyRequest WithDependencyOfTaskId(string dependencyOfTaskId)
    {
        _dependencyOfTaskId = dependencyOfTaskId;
        return this;
    }

    public FluentAddDependencyRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public FluentAddDependencyRequest WithTeamId(string teamId)
    {
        _teamId = teamId;
        return this;
    }

    public async Task AddAsync(CancellationToken cancellationToken = default)
    {
        await _taskRelationshipsService.AddDependencyAsync(
            _taskId,
            _dependsOnTaskId,
            _dependencyOfTaskId,
            _customTaskIds,
            _teamId,
            cancellationToken
        );
    }
}