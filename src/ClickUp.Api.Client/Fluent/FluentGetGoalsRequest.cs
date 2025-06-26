using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.ResponseModels.Goals;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentGetGoalsRequest
{
    private readonly GetGoalsRequest _request = new();
    private readonly string _workspaceId;
    private readonly IGoalsService _goalsService;

    public FluentGetGoalsRequest(string workspaceId, IGoalsService goalsService)
    {
        _workspaceId = workspaceId;
        _goalsService = goalsService;
    }

    public FluentGetGoalsRequest WithIncludeCompleted(bool includeCompleted)
    {
        _request.IncludeCompleted = includeCompleted;
        return this;
    }

    public async Task<GetGoalsResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _goalsService.GetGoalsAsync(
            _workspaceId,
            _request.IncludeCompleted,
            cancellationToken
        );
    }
}