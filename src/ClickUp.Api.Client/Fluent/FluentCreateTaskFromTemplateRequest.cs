using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentCreateTaskFromTemplateRequest
{
    private string? _name;
    private bool? _customTaskIds;
    private string? _teamId;

    private readonly string _listId;
    private readonly string _templateId;
    private readonly ITasksService _tasksService;

    public FluentCreateTaskFromTemplateRequest(string listId, string templateId, ITasksService tasksService)
    {
        _listId = listId;
        _templateId = templateId;
        _tasksService = tasksService;
    }

    public FluentCreateTaskFromTemplateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public FluentCreateTaskFromTemplateRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public FluentCreateTaskFromTemplateRequest WithTeamId(string teamId)
    {
        _teamId = teamId;
        return this;
    }

    public async Task<CuTask> CreateAsync(CancellationToken cancellationToken = default)
    {
        var createTaskFromTemplateRequest = new CreateTaskFromTemplateRequest(
            Name: _name ?? string.Empty
        );

        return await _tasksService.CreateTaskFromTemplateAsync(
            _listId,
            _templateId,
            createTaskFromTemplateRequest,
            _customTaskIds,
            _teamId,
            cancellationToken
        );
    }
}