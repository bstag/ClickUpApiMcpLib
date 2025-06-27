using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TaskFluentGetRequest
{
    private bool? _customTaskIds;
    private string? _teamId;
    private bool? _includeSubtasks;
    private bool? _includeMarkdownDescription;

    private readonly string _taskId;
    private readonly ITasksService _tasksService;

    public TaskFluentGetRequest(string taskId, ITasksService tasksService)
    {
        _taskId = taskId;
        _tasksService = tasksService;
    }

    public TaskFluentGetRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public TaskFluentGetRequest WithTeamId(string teamId)
    {
        _teamId = teamId;
        return this;
    }

    public TaskFluentGetRequest WithIncludeSubtasks(bool includeSubtasks)
    {
        _includeSubtasks = includeSubtasks;
        return this;
    }

    public TaskFluentGetRequest WithIncludeMarkdownDescription(bool includeMarkdownDescription)
    {
        _includeMarkdownDescription = includeMarkdownDescription;
        return this;
    }

    public async Task<CuTask> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetTaskAsync(
            _taskId,
            _customTaskIds,
            _teamId,
            _includeSubtasks,
            _includeMarkdownDescription,
            cancellationToken
        );
    }
}