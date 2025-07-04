using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ClickUp.Api.Client.Fluent;

public class KeyResultFluentEditRequest
{
    private object? _stepsCurrent;
    private string? _note;
    private string? _name;
    private List<int>? _owners;
    private List<int>? _addOwners;
    private List<int>? _removeOwners;
    private List<string>? _taskIds;
    private List<string>? _listIds;
    private bool? _archived;

    private readonly string _keyResultId;
    private readonly IGoalsService _goalsService;
    private readonly List<string> _validationErrors = new List<string>();

    public KeyResultFluentEditRequest(string keyResultId, IGoalsService goalsService)
    {
        _keyResultId = keyResultId;
        _goalsService = goalsService;
    }

    public KeyResultFluentEditRequest WithStepsCurrent(object stepsCurrent)
    {
        _stepsCurrent = stepsCurrent;
        return this;
    }

    public KeyResultFluentEditRequest WithNote(string note)
    {
        _note = note;
        return this;
    }

    public KeyResultFluentEditRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public KeyResultFluentEditRequest WithOwners(List<int> owners)
    {
        _owners = owners;
        return this;
    }

    public KeyResultFluentEditRequest WithAddOwners(List<int> addOwners)
    {
        _addOwners = addOwners;
        return this;
    }

    public KeyResultFluentEditRequest WithRemoveOwners(List<int> removeOwners)
    {
        _removeOwners = removeOwners;
        return this;
    }

    public KeyResultFluentEditRequest WithTaskIds(List<string> taskIds)
    {
        _taskIds = taskIds;
        return this;
    }

    public KeyResultFluentEditRequest WithListIds(List<string> listIds)
    {
        _listIds = listIds;
        return this;
    }

    public KeyResultFluentEditRequest WithArchived(bool archived)
    {
        _archived = archived;
        return this;
    }

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_keyResultId))
        {
            _validationErrors.Add("KeyResultId is required.");
        }
        // For an update, at least one field should ideally be provided.
        // However, the API might allow an empty update request (noop).
        // For now, we'll assume the API handles this, but specific checks can be added if needed.
        // Example:
        // if (_stepsCurrent == null && string.IsNullOrWhiteSpace(_note) && string.IsNullOrWhiteSpace(_name) && ...)
        // {
        //     _validationErrors.Add("At least one property must be set for updating a Key Result.");
        // }

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<KeyResult> EditAsync(CancellationToken cancellationToken = default)
    {
        Validate();
        var editKeyResultRequest = new EditKeyResultRequest(
            StepsCurrent: _stepsCurrent,
            Note: _note,
            Name: _name,
            Owners: _owners,
            AddOwners: _addOwners,
            RemoveOwners: _removeOwners,
            TaskIds: _taskIds,
            ListIds: _listIds,
            Archived: _archived
        );

        return await _goalsService.EditKeyResultAsync(
            _keyResultId,
            editKeyResultRequest,
            cancellationToken
        );
    }
}