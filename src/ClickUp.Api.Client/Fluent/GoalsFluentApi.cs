using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.ResponseModels.Goals;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.Entities.Goals;

public class GoalsFluentApi
{
    private readonly IGoalsService _goalsService;

    public GoalsFluentApi(IGoalsService goalsService)
    {
        _goalsService = goalsService;
    }

    public GoalsFluentGetRequest GetGoals(string workspaceId)
    {
        return new GoalsFluentGetRequest(workspaceId, _goalsService);
    }

    public GoalFluentCreateRequest CreateGoal(string workspaceId)
    {
        return new GoalFluentCreateRequest(workspaceId, _goalsService);
    }

    public async Task<Goal> GetGoalAsync(string goalId, CancellationToken cancellationToken = default)
    {
        return await _goalsService.GetGoalAsync(goalId, cancellationToken);
    }

    public GoalFluentUpdateRequest UpdateGoal(string goalId)
    {
        return new GoalFluentUpdateRequest(goalId, _goalsService);
    }

    public async Task DeleteGoalAsync(string goalId, CancellationToken cancellationToken = default)
    {
        await _goalsService.DeleteGoalAsync(goalId, cancellationToken);
    }

    public KeyResultFluentCreateRequest CreateKeyResult(string goalId)
    {
        return new KeyResultFluentCreateRequest(goalId, _goalsService);
    }

    public KeyResultFluentEditRequest EditKeyResult(string keyResultId)
    {
        return new KeyResultFluentEditRequest(keyResultId, _goalsService);
    }

    public async Task DeleteKeyResultAsync(string keyResultId, CancellationToken cancellationToken = default)
    {
        await _goalsService.DeleteKeyResultAsync(keyResultId, cancellationToken);
    }
}
