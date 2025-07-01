using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.RequestModels.TaskRelationships; // Added for AddTaskLinkRequest

public class TaskLinkFluentAddRequest
{
    private readonly AddTaskLinkRequest _requestDto = new AddTaskLinkRequest(); // Use the DTO
    private readonly string _taskId;
    private readonly string _linksToTaskId;
    private readonly ITaskRelationshipsService _taskRelationshipsService;

    public TaskLinkFluentAddRequest(string taskId, string linksToTaskId, ITaskRelationshipsService taskRelationshipsService)
    {
        _taskId = taskId;
        _linksToTaskId = linksToTaskId;
        _taskRelationshipsService = taskRelationshipsService;
    }

    public TaskLinkFluentAddRequest WithCustomTaskIds(bool customTaskIds)
    {
        _requestDto.CustomTaskIds = customTaskIds;
        return this;
    }

    public TaskLinkFluentAddRequest WithTeamId(string teamId)
    {
        _requestDto.TeamId = teamId;
        return this;
    }

    public async Task<CuTask?> AddAsync(CancellationToken cancellationToken = default)
    {
        return await _taskRelationshipsService.AddTaskLinkAsync(
            _taskId,
            _linksToTaskId,
            _requestDto, // Pass the DTO
            cancellationToken
        );
    }
}