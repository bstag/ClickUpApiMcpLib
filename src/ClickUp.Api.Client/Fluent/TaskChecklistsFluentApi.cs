using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TaskChecklistsFluentApi
{
    private readonly ITaskChecklistsService _taskChecklistsService;

    public TaskChecklistsFluentApi(ITaskChecklistsService taskChecklistsService)
    {
        _taskChecklistsService = taskChecklistsService;
    }

    public ChecklistFluentCreateRequest CreateChecklist(string taskId)
    {
        return new ChecklistFluentCreateRequest(taskId, _taskChecklistsService);
    }

    public ChecklistFluentUpdateRequest EditChecklist(string checklistId)
    {
        return new ChecklistFluentUpdateRequest(checklistId, _taskChecklistsService);
    }

    public async Task DeleteChecklistAsync(string checklistId, CancellationToken cancellationToken = default)
    {
        await _taskChecklistsService.DeleteChecklistAsync(checklistId, cancellationToken);
    }

    public ChecklistItemFluentCreateRequest CreateChecklistItem(string checklistId)
    {
        return new ChecklistItemFluentCreateRequest(checklistId, _taskChecklistsService);
    }

    public ChecklistItemFluentUpdateRequest EditChecklistItem(string checklistId, string checklistItemId)
    {
        return new ChecklistItemFluentUpdateRequest(checklistId, checklistItemId, _taskChecklistsService);
    }

    public async Task DeleteChecklistItemAsync(string checklistId, string checklistItemId, CancellationToken cancellationToken = default)
    {
        await _taskChecklistsService.DeleteChecklistItemAsync(checklistId, checklistItemId, cancellationToken);
    }
}
