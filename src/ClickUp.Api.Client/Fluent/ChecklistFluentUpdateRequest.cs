using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class ChecklistFluentUpdateRequest
{
    private string? _name;
    private int? _position;

    private readonly string _checklistId;
    private readonly ITaskChecklistsService _taskChecklistsService;

    public ChecklistFluentUpdateRequest(string checklistId, ITaskChecklistsService taskChecklistsService)
    {
        _checklistId = checklistId;
        _taskChecklistsService = taskChecklistsService;
    }

    public ChecklistFluentUpdateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public ChecklistFluentUpdateRequest WithPosition(int position)
    {
        _position = position;
        return this;
    }

    public async Task EditAsync(CancellationToken cancellationToken = default)
    {
        var editChecklistRequest = new EditChecklistRequest(
            Name: _name,
            Position: _position
        );

        await _taskChecklistsService.EditChecklistAsync(
            _checklistId,
            editChecklistRequest,
            cancellationToken
        );
    }
}