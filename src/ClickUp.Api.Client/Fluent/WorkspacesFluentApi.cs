using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.WorkSpaces;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class WorkspacesFluentApi
{
    private readonly IWorkspacesService _workspacesService;

    public WorkspacesFluentApi(IWorkspacesService workspacesService)
    {
        _workspacesService = workspacesService;
    }

    public async Task<GetWorkspaceSeatsResponse> GetSeatsAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        return await _workspacesService.GetWorkspaceSeatsAsync(workspaceId, cancellationToken);
    }

    public async Task<GetWorkspacePlanResponse> GetPlanAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        return await _workspacesService.GetWorkspacePlanAsync(workspaceId, cancellationToken);
    }

    public async IAsyncEnumerable<ClickUpWorkspace> GetAuthorizedWorkspacesAsyncEnumerableAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _workspacesService.GetAuthorizedWorkspacesAsync(cancellationToken).ConfigureAwait(false);
        if (response?.Workspaces == null)
        {
            yield break;
        }

        foreach (var workspace in response.Workspaces)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return workspace;
        }
    }
}
