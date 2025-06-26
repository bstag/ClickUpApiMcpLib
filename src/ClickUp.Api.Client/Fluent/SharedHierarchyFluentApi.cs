using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Sharing;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class SharedHierarchyFluentApi
{
    private readonly ISharedHierarchyService _sharedHierarchyService;

    public SharedHierarchyFluentApi(ISharedHierarchyService sharedHierarchyService)
    {
        _sharedHierarchyService = sharedHierarchyService;
    }

    public async Task<SharedHierarchyResponse> GetSharedHierarchyAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        return await _sharedHierarchyService.GetSharedHierarchyAsync(workspaceId, cancellationToken);
    }
}