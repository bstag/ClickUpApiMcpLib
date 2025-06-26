using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentCreateChecklistItemRequest
{
    private string? _name;
    private string? _assignee;

    private readonly string _checklistId;
    private readonly ITaskChecklistsService _taskChecklistsService;

    public FluentCreateChecklistItemRequest(string checklistId, ITaskChecklistsService taskChecklistsService)
    {
        _checklistId = checklistId;
        _taskChecklistsService = taskChecklistsService;
    }

    public FluentCreateChecklistItemRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public FluentCreateChecklistItemRequest WithAssignee(string assignee)
    {
        _assignee = assignee;
        return this;
    }

    public async Task<CreateChecklistItemResponse> CreateAsync(CancellationToken cancellationToken = default)
    {
        var createChecklistItemRequest = new CreateChecklistItemRequest(
            Name: _name ?? string.Empty,
            Assignee: int.TryParse(_assignee, out int assigneeId) ? assigneeId : (int?)null
        );

        return await _taskChecklistsService.CreateChecklistItemAsync(
            _checklistId,
            createChecklistItemRequest,
            cancellationToken
        );
    }
}