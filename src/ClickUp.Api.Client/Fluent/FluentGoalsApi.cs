using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.ResponseModels.Goals;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.Entities.Goals;

public class FluentGoalsApi
{
    private readonly IGoalsService _goalsService;

    public FluentGoalsApi(IGoalsService goalsService)
    {
        _goalsService = goalsService;
    }

    public FluentGetGoalsRequest GetGoals(string workspaceId)
    {
        return new FluentGetGoalsRequest(workspaceId, _goalsService);
    }

    public FluentCreateGoalRequest CreateGoal(string workspaceId)
    {
        return new FluentCreateGoalRequest(workspaceId, _goalsService);
    }

    public async Task<Goal> GetGoalAsync(string goalId, CancellationToken cancellationToken = default)
    {
        return await _goalsService.GetGoalAsync(goalId, cancellationToken);
    }

    public FluentUpdateGoalRequest UpdateGoal(string goalId)
    {
        return new FluentUpdateGoalRequest(goalId, _goalsService);
    }

    public async Task DeleteGoalAsync(string goalId, CancellationToken cancellationToken = default)
    {
        await _goalsService.DeleteGoalAsync(goalId, cancellationToken);
    }

    public FluentCreateKeyResultRequest CreateKeyResult(string goalId)
    {
        return new FluentCreateKeyResultRequest(goalId, _goalsService);
    }

    public FluentEditKeyResultRequest EditKeyResult(string keyResultId)
    {
        return new FluentEditKeyResultRequest(keyResultId, _goalsService);
    }

    public async Task DeleteKeyResultAsync(string keyResultId, CancellationToken cancellationToken = default)
    {
        await _goalsService.DeleteKeyResultAsync(keyResultId, cancellationToken);
    }
}
