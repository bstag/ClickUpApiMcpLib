using ClickUp.Api.Client.Abstractions.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TaskFluentDeleteRequest
{
    private bool? _customTaskIds;
    private string? _teamId;

    private readonly string _taskId;
    private readonly ITasksService _tasksService;

    public TaskFluentDeleteRequest(string taskId, ITasksService tasksService)
    {
        _taskId = taskId;
        _tasksService = tasksService;
    }

    public TaskFluentDeleteRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public TaskFluentDeleteRequest WithTeamId(string teamId)
    {
        _teamId = teamId;
        return this;
    }

    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        await _tasksService.DeleteTaskAsync(
            _taskId,
            _customTaskIds,
            _teamId,
            cancellationToken
        );
    }
}