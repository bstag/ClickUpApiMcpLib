using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.ResponseModels.Goals;
using System.Collections.Generic; // Added for IAsyncEnumerable
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

// Removed redundant using ClickUp.Api.Client.Models.Entities.Goals;

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

    /// <summary>
    /// Retrieves all goals for a specific workspace asynchronously.
    /// While this method returns an IAsyncEnumerable, the underlying ClickUp API for getting goals
    /// does not appear to be paginated, so all goals are typically fetched in a single call by the service.
    /// The response from the service includes both goals and goal folders; this method yields only the goals.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace (team).</param>
    /// <param name="includeCompleted">Optional. Whether to include completed goals. Defaults to false.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Goal"/>.</returns>
    public async IAsyncEnumerable<Goal> GetGoalsAsyncEnumerableAsync(
        string workspaceId,
        bool? includeCompleted = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _goalsService.GetGoalsAsync(workspaceId, includeCompleted, cancellationToken).ConfigureAwait(false);
        if (response?.Goals != null)
        {
            foreach (var goal in response.Goals)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return goal;
            }
        }
    }
}
