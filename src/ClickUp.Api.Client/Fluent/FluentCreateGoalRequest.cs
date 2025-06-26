using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentCreateGoalRequest
{
    private string? _name;
    private long? _dueDate;
    private string? _description;
    private bool? _multipleOwners;
    private List<int>? _owners;
    private string? _color;
    private string? _folderId;

    private readonly string _workspaceId;
    private readonly IGoalsService _goalsService;

    public FluentCreateGoalRequest(string workspaceId, IGoalsService goalsService)
    {
        _workspaceId = workspaceId;
        _goalsService = goalsService;
    }

    public FluentCreateGoalRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public FluentCreateGoalRequest WithDueDate(long dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public FluentCreateGoalRequest WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public FluentCreateGoalRequest WithMultipleOwners(bool multipleOwners)
    {
        _multipleOwners = multipleOwners;
        return this;
    }

    public FluentCreateGoalRequest WithOwners(List<int> owners)
    {
        _owners = owners;
        return this;
    }

    public FluentCreateGoalRequest WithColor(string color)
    {
        _color = color;
        return this;
    }

    public FluentCreateGoalRequest WithFolderId(string folderId)
    {
        _folderId = folderId;
        return this;
    }

    public async Task<Goal> CreateAsync(CancellationToken cancellationToken = default)
    {
        var createGoalRequest = new CreateGoalRequest(
            Name: _name ?? string.Empty,
            DueDate: _dueDate ?? 0, // Assuming 0 is a valid default or will be handled by validation
            Description: _description ?? string.Empty,
            MultipleOwners: _multipleOwners ?? false, // Assuming false is a valid default
            Owners: _owners ?? new List<int>(), // Assuming empty list is a valid default
            Color: _color,
            TeamId: _workspaceId, // Passed from constructor
            FolderId: _folderId
        );

        return await _goalsService.CreateGoalAsync(
            _workspaceId,
            createGoalRequest,
            cancellationToken
        );
    }
}