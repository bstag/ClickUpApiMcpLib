using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

    public async Task<KeyResult> EditAsync(CancellationToken cancellationToken = default)
    {
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