using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TemplateFluentCreateTaskRequest
{
    private string? _name;
    // private bool? _customTaskIds; // Removed
    // private string? _teamId; // Removed

    private readonly string _listId;
    private readonly string _templateId;
    private readonly ITasksService _tasksService;

    public TemplateFluentCreateTaskRequest(string listId, string templateId, ITasksService tasksService)
    {
        _listId = listId;
        _templateId = templateId;
        _tasksService = tasksService;
    }

    public TemplateFluentCreateTaskRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    // public TemplateFluentCreateTaskRequest WithCustomTaskIds(bool customTaskIds) // Removed
    // {
    //     _customTaskIds = customTaskIds;
    //     return this;
    // }

    // public TemplateFluentCreateTaskRequest WithTeamId(string teamId) // Removed
    // {
    //     _teamId = teamId;
    //     return this;
    // }

    public async Task<CuTask> CreateAsync(CancellationToken cancellationToken = default)
    {
        var createTaskFromTemplateRequest = new CreateTaskFromTemplateRequest(
            Name: _name ?? string.Empty // Ensure Name is provided, matching DTO constructor if it's non-nullable for name
        );

        return await _tasksService.CreateTaskFromTemplateAsync(
            _listId,
            _templateId,
            createTaskFromTemplateRequest,
            // _customTaskIds, // Removed
            // _teamId, // Removed
            cancellationToken
        );
    }
}