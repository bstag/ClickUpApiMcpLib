using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.RequestModels.Tasks; // Added for GetTaskTimeInStatusRequest

public class TaskTimeInStatusFluentGetRequest
{
    private readonly GetTaskTimeInStatusRequest _requestDto = new(); // Use the DTO
    private readonly string _taskId;
    private readonly ITasksService _tasksService;

    public TaskTimeInStatusFluentGetRequest(string taskId, ITasksService tasksService)
    {
        _taskId = taskId;
        _tasksService = tasksService;
    }

    public TaskTimeInStatusFluentGetRequest WithCustomTaskIds(bool customTaskIds)
    {
        _requestDto.CustomTaskIds = customTaskIds;
        return this;
    }

    public TaskTimeInStatusFluentGetRequest WithTeamId(string teamId)
    {
        _requestDto.TeamId = teamId;
        return this;
    }

    public async Task<TaskTimeInStatusResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetTaskTimeInStatusAsync(
            _taskId,
            _requestDto, // Pass the DTO
            cancellationToken
        );
    }
}