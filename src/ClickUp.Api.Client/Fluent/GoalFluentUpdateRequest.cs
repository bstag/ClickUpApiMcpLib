using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class GoalFluentUpdateRequest
{
    private string? _name;
    private System.DateTimeOffset? _dueDate;
    private string? _description;
    private List<int>? _removeOwners;
    private List<int>? _addOwners;
    private string? _color;
    private bool? _archived;

    private readonly string _goalId;
    private readonly IGoalsService _goalsService;

    public GoalFluentUpdateRequest(string goalId, IGoalsService goalsService)
    {
        _goalId = goalId;
        _goalsService = goalsService;
    }

    public GoalFluentUpdateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public GoalFluentUpdateRequest WithDueDate(System.DateTimeOffset dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public GoalFluentUpdateRequest WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public GoalFluentUpdateRequest WithRemoveOwners(List<int> removeOwners)
    {
        _removeOwners = removeOwners;
        return this;
    }

    public GoalFluentUpdateRequest WithAddOwners(List<int> addOwners)
    {
        _addOwners = addOwners;
        return this;
    }

    public GoalFluentUpdateRequest WithColor(string color)
    {
        _color = color;
        return this;
    }

    public GoalFluentUpdateRequest WithArchived(bool archived)
    {
        _archived = archived;
        return this;
    }

    public async Task<Goal> UpdateAsync(CancellationToken cancellationToken = default)
    {
        var updateGoalRequest = new UpdateGoalRequest(
            Name: _name,
            DueDate: _dueDate,
            Description: _description,
            RemoveOwners: _removeOwners,
            AddOwners: _addOwners,
            Color: _color,
            Archived: _archived
        );

        return await _goalsService.UpdateGoalAsync(
            _goalId,
            updateGoalRequest,
            cancellationToken
        );
    }
}