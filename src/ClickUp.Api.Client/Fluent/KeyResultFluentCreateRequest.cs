using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class KeyResultFluentCreateRequest
{
    private string? _name;
    private List<int>? _owners;
    private string? _type;
    private object? _stepsStart;
    private object? _stepsEnd;
    private string? _unit;
    private List<string>? _taskIds;
    private List<string>? _listIds;

    private readonly string _goalId;
    private readonly IGoalsService _goalsService;

    public KeyResultFluentCreateRequest(string goalId, IGoalsService goalsService)
    {
        _goalId = goalId;
        _goalsService = goalsService;
    }

    public KeyResultFluentCreateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public KeyResultFluentCreateRequest WithOwners(List<int> owners)
    {
        _owners = owners;
        return this;
    }

    public KeyResultFluentCreateRequest WithType(string type)
    {
        _type = type;
        return this;
    }

    public KeyResultFluentCreateRequest WithStepsStart(object stepsStart)
    {
        _stepsStart = stepsStart;
        return this;
    }

    public KeyResultFluentCreateRequest WithStepsEnd(object stepsEnd)
    {
        _stepsEnd = stepsEnd;
        return this;
    }

    public KeyResultFluentCreateRequest WithUnit(string unit)
    {
        _unit = unit;
        return this;
    }

    public KeyResultFluentCreateRequest WithTaskIds(List<string> taskIds)
    {
        _taskIds = taskIds;
        return this;
    }

    public KeyResultFluentCreateRequest WithListIds(List<string> listIds)
    {
        _listIds = listIds;
        return this;
    }

    public async Task<KeyResult> CreateAsync(CancellationToken cancellationToken = default)
    {
        var createKeyResultRequest = new CreateKeyResultRequest(
            Name: _name ?? string.Empty,
            Owners: _owners ?? new List<int>(),
            Type: _type ?? string.Empty,
            StepsStart: _stepsStart,
            StepsEnd: _stepsEnd ?? new object(), // Assuming a default empty object is acceptable if null
            Unit: _unit,
            TaskIds: _taskIds,
            ListIds: _listIds,
            GoalId: _goalId
        );

        return await _goalsService.CreateKeyResultAsync(
            _goalId,
            createKeyResultRequest,
            cancellationToken
        );
    }
}