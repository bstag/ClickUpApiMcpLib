using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentTaskChecklistsApi
{
    private readonly ITaskChecklistsService _taskChecklistsService;

    public FluentTaskChecklistsApi(ITaskChecklistsService taskChecklistsService)
    {
        _taskChecklistsService = taskChecklistsService;
    }

    public FluentCreateChecklistRequest CreateChecklist(string taskId)
    {
        return new FluentCreateChecklistRequest(taskId, _taskChecklistsService);
    }

    public FluentEditChecklistRequest EditChecklist(string checklistId)
    {
        return new FluentEditChecklistRequest(checklistId, _taskChecklistsService);
    }

    public async Task DeleteChecklistAsync(string checklistId, CancellationToken cancellationToken = default)
    {
        await _taskChecklistsService.DeleteChecklistAsync(checklistId, cancellationToken);
    }

    public FluentCreateChecklistItemRequest CreateChecklistItem(string checklistId)
    {
        return new FluentCreateChecklistItemRequest(checklistId, _taskChecklistsService);
    }

    public FluentEditChecklistItemRequest EditChecklistItem(string checklistId, string checklistItemId)
    {
        return new FluentEditChecklistItemRequest(checklistId, checklistItemId, _taskChecklistsService);
    }

    public async Task DeleteChecklistItemAsync(string checklistId, string checklistItemId, CancellationToken cancellationToken = default)
    {
        await _taskChecklistsService.DeleteChecklistItemAsync(checklistId, checklistItemId, cancellationToken);
    }
}
