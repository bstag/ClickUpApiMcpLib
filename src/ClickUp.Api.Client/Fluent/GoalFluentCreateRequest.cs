using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ClickUp.Api.Client.Fluent;

public class GoalFluentCreateRequest
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
    private readonly List<string> _validationErrors = new List<string>();

    public GoalFluentCreateRequest(string workspaceId, IGoalsService goalsService)
    {
        _workspaceId = workspaceId;
        _goalsService = goalsService;
    }

    public GoalFluentCreateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public GoalFluentCreateRequest WithDueDate(long dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public GoalFluentCreateRequest WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public GoalFluentCreateRequest WithMultipleOwners(bool multipleOwners)
    {
        _multipleOwners = multipleOwners;
        return this;
    }

    public GoalFluentCreateRequest WithOwners(List<int> owners)
    {
        _owners = owners;
        return this;
    }

    public GoalFluentCreateRequest WithColor(string color)
    {
        _color = color;
        return this;
    }

    public GoalFluentCreateRequest WithFolderId(string folderId)
    {
        _folderId = folderId;
        return this;
    }

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_workspaceId))
        {
            _validationErrors.Add("WorkspaceId (TeamId) is required.");
        }
        if (string.IsNullOrWhiteSpace(_name))
        {
            _validationErrors.Add("Goal name is required.");
        }
        if (!_dueDate.HasValue)
        {
            _validationErrors.Add("DueDate is required.");
        }
        if ((_owners == null || !_owners.Any()))
        {
            _validationErrors.Add("At least one owner is required for the Goal.");
        }
        // Add other validation rules as needed

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<Goal> CreateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
        var createGoalRequest = new CreateGoalRequest(
            Name: _name ?? string.Empty,
            DueDate: _dueDate ?? 0,
            Description: _description ?? string.Empty,
            MultipleOwners: _multipleOwners ?? false,
            Owners: _owners ?? new List<int>(),
            Color: _color,
            TeamId: _workspaceId,
            FolderId: _folderId
        );

        return await _goalsService.CreateGoalAsync(
            _workspaceId,
            createGoalRequest,
            cancellationToken
        );
    }
}