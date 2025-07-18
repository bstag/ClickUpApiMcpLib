using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
    private readonly List<string> _validationErrors = new List<string>();

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

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_goalId))
        {
            _validationErrors.Add("GoalId is required.");
        }
        // Similar to KeyResultEdit, an update should ideally change something.
        // For now, assume API handles no-op updates gracefully.
        // Example check:
        // if (string.IsNullOrWhiteSpace(_name) && !_dueDate.HasValue && string.IsNullOrWhiteSpace(_description) &&
        //     (_removeOwners == null || !_removeOwners.Any()) && (_addOwners == null || !_addOwners.Any()) &&
        //     string.IsNullOrWhiteSpace(_color) && !_archived.HasValue)
        // {
        //     _validationErrors.Add("At least one property must be set for updating a Goal.");
        // }

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<Goal> UpdateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
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