using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class ChecklistFluentCreateRequest
{
    private string? _name;

    private readonly string _taskId;
    private readonly ITaskChecklistsService _taskChecklistsService;

    public ChecklistFluentCreateRequest(string taskId, ITaskChecklistsService taskChecklistsService)
    {
        _taskId = taskId;
        _taskChecklistsService = taskChecklistsService;
    }

    public ChecklistFluentCreateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public async Task<CreateChecklistResponse> CreateAsync(bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        var createChecklistRequest = new CreateChecklistRequest(
            Name: _name ?? string.Empty
        );

        return await _taskChecklistsService.CreateChecklistAsync(
            _taskId,
            createChecklistRequest,
            customTaskIds,
            teamId,
            cancellationToken
        );
    }
}