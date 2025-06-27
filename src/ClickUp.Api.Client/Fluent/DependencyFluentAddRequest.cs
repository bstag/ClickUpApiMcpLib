using ClickUp.Api.Client.Abstractions.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class DependencyFluentAddRequest
{
    private string? _dependsOnTaskId;
    private string? _dependencyOfTaskId;
    private bool? _customTaskIds;
    private string? _teamId;

    private readonly string _taskId;
    private readonly ITaskRelationshipsService _taskRelationshipsService;

    public DependencyFluentAddRequest(string taskId, ITaskRelationshipsService taskRelationshipsService)
    {
        _taskId = taskId;
        _taskRelationshipsService = taskRelationshipsService;
    }

    public DependencyFluentAddRequest WithDependsOnTaskId(string dependsOnTaskId)
    {
        _dependsOnTaskId = dependsOnTaskId;
        return this;
    }

    public DependencyFluentAddRequest WithDependencyOfTaskId(string dependencyOfTaskId)
    {
        _dependencyOfTaskId = dependencyOfTaskId;
        return this;
    }

    public DependencyFluentAddRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public DependencyFluentAddRequest WithTeamId(string teamId)
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