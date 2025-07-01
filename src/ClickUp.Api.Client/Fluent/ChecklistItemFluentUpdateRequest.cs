using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class ChecklistItemFluentUpdateRequest
{
    private string? _name;
    private bool? _resolved;
    private string? _assignee;
    private string? _parent;

    private readonly string _checklistId;
    private readonly string _checklistItemId;
    private readonly ITaskChecklistsService _taskChecklistsService;

    public ChecklistItemFluentUpdateRequest(string checklistId, string checklistItemId, ITaskChecklistsService taskChecklistsService)
    {
        _checklistId = checklistId;
        _checklistItemId = checklistItemId;
        _taskChecklistsService = taskChecklistsService;
    }

    public ChecklistItemFluentUpdateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public ChecklistItemFluentUpdateRequest WithResolved(bool resolved)
    {
        _resolved = resolved;
        return this;
    }

    public ChecklistItemFluentUpdateRequest WithAssignee(string assignee)
    {
        _assignee = assignee;
        return this;
    }

    public ChecklistItemFluentUpdateRequest WithParent(string parent)
    {
        _parent = parent;
        return this;
    }

    public async Task<EditChecklistItemResponse> EditAsync(CancellationToken cancellationToken = default)
    {
        var editChecklistItemRequest = new EditChecklistItemRequest(
            Name: _name,
            Resolved: _resolved,
            Assignee: int.TryParse(_assignee, out int assigneeId) ? assigneeId : (int?)null,
            Parent: _parent
        );

        return await _taskChecklistsService.EditChecklistItemAsync(
            _checklistId,
            _checklistItemId,
            editChecklistItemRequest,
            cancellationToken
        );
    }
}