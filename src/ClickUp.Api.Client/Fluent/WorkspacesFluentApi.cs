using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class WorkspacesFluentApi
{
    private readonly IWorkspacesService _workspacesService;

    public WorkspacesFluentApi(IWorkspacesService workspacesService)
    {
        _workspacesService = workspacesService;
    }

    public async Task<GetWorkspaceSeatsResponse> GetSeatsAsync(string workspaceId)
    {
        return await _workspacesService.GetWorkspaceSeatsAsync(workspaceId);
    }

    public async Task<GetWorkspacePlanResponse> GetPlanAsync(string workspaceId)
    {
        return await _workspacesService.GetWorkspacePlanAsync(workspaceId);
    }
}
