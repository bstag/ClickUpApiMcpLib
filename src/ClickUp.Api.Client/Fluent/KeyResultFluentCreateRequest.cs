using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
    private readonly List<string> _validationErrors = new List<string>();

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

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_goalId))
        {
            _validationErrors.Add("GoalId is required.");
        }
        if (string.IsNullOrWhiteSpace(_name))
        {
            _validationErrors.Add("Key Result name is required.");
        }
        if (string.IsNullOrWhiteSpace(_type))
        {
            _validationErrors.Add("Key Result type is required.");
        }
        if ((_owners == null || !_owners.Any()))
        {
            _validationErrors.Add("At least one owner is required for the Key Result.");
        }
        if (_stepsEnd == null) // Assuming stepsEnd is mandatory for creation
        {
            _validationErrors.Add("StepsEnd is required for the Key Result.");
        }
        // Add other specific validation rules for KeyResult types if necessary

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<KeyResult> CreateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
        var createKeyResultRequest = new CreateKeyResultRequest(
            Name: _name ?? string.Empty,
            Owners: _owners ?? new List<int>(),
            Type: _type ?? string.Empty,
            StepsStart: _stepsStart,
            StepsEnd: _stepsEnd ?? new object(),
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