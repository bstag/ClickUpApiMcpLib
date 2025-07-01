using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.RequestModels.Tasks; // Added for GetTaskRequest

public class TaskFluentGetRequest
{
    private readonly GetTaskRequest _requestDto = new(); // Use the DTO
    private readonly string _taskId;
    private readonly ITasksService _tasksService;

    public TaskFluentGetRequest(string taskId, ITasksService tasksService)
    {
        _taskId = taskId;
        _tasksService = tasksService;
    }

    public TaskFluentGetRequest WithCustomTaskIds(bool customTaskIds)
    {
        _requestDto.CustomTaskIds = customTaskIds;
        return this;
    }

    public TaskFluentGetRequest WithTeamId(string teamId)
    {
        _requestDto.TeamId = teamId;
        return this;
    }

    public TaskFluentGetRequest WithIncludeSubtasks(bool includeSubtasks)
    {
        _requestDto.IncludeSubtasks = includeSubtasks;
        return this;
    }

    public TaskFluentGetRequest WithIncludeMarkdownDescription(bool includeMarkdownDescription)
    {
        _requestDto.IncludeMarkdownDescription = includeMarkdownDescription;
        return this;
    }

    public TaskFluentGetRequest WithCommentPage(int page)
    {
        _requestDto.Page = page;
        return this;
    }

    public async Task<CuTask> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetTaskAsync(
            _taskId,
            _requestDto, // Pass the DTO
            cancellationToken
        );
    }
}