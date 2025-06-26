using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentGetTaskTimeInStatusRequest
{
    private bool? _customTaskIds;
    private string? _teamId;

    private readonly string _taskId;
    private readonly ITasksService _tasksService;

    public FluentGetTaskTimeInStatusRequest(string taskId, ITasksService tasksService)
    {
        _taskId = taskId;
        _tasksService = tasksService;
    }

    public FluentGetTaskTimeInStatusRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public FluentGetTaskTimeInStatusRequest WithTeamId(string teamId)
    {
        _teamId = teamId;
        return this;
    }

    public async Task<TaskTimeInStatusResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetTaskTimeInStatusAsync(
            _taskId,
            _customTaskIds,
            _teamId,
            cancellationToken
        );
    }
}