using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentAddTaskLinkRequest
{
    private bool? _customTaskIds;
    private string? _teamId;

    private readonly string _taskId;
    private readonly string _linksToTaskId;
    private readonly ITaskRelationshipsService _taskRelationshipsService;

    public FluentAddTaskLinkRequest(string taskId, string linksToTaskId, ITaskRelationshipsService taskRelationshipsService)
    {
        _taskId = taskId;
        _linksToTaskId = linksToTaskId;
        _taskRelationshipsService = taskRelationshipsService;
    }

    public FluentAddTaskLinkRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public FluentAddTaskLinkRequest WithTeamId(string teamId)
    {
        _teamId = teamId;
        return this;
    }

    public async Task<CuTask?> AddAsync(CancellationToken cancellationToken = default)
    {
        return await _taskRelationshipsService.AddTaskLinkAsync(
            _taskId,
            _linksToTaskId,
            _customTaskIds,
            _teamId,
            cancellationToken
        );
    }
}