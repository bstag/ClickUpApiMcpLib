using ClickUp.Api.Client.Abstractions.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.RequestModels.Tasks; // Added for DeleteTaskRequest

public class TaskFluentDeleteRequest
{
    private readonly DeleteTaskRequest _requestDto = new(); // Use the DTO
    private readonly string _taskId;
    private readonly ITasksService _tasksService;

    public TaskFluentDeleteRequest(string taskId, ITasksService tasksService)
    {
        _taskId = taskId;
        _tasksService = tasksService;
    }

    public TaskFluentDeleteRequest WithCustomTaskIds(bool customTaskIds)
    {
        _requestDto.CustomTaskIds = customTaskIds;
        return this;
    }

    public TaskFluentDeleteRequest WithTeamId(string teamId)
    {
        _requestDto.TeamId = teamId;
        return this;
    }

    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        // The method in ITasksService is now DeleteTaskAsync(string taskId, DeleteTaskRequest requestModel, CancellationToken token)
        await _tasksService.DeleteTaskAsync(
            _taskId,
            _requestDto, // Pass the DTO
            cancellationToken
        );
    }
}