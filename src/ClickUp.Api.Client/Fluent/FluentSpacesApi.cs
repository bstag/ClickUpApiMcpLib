using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Spaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentSpacesApi
{
    private readonly ISpacesService _spacesService;

    public FluentSpacesApi(ISpacesService spacesService)
    {
        _spacesService = spacesService;
    }

    public async Task<IEnumerable<Space>> GetSpacesAsync(string workspaceId, bool? archived = null, CancellationToken cancellationToken = default)
    {
        return await _spacesService.GetSpacesAsync(workspaceId, archived, cancellationToken);
    }

    public async Task<Space> GetSpaceAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        return await _spacesService.GetSpaceAsync(spaceId, cancellationToken);
    }

    public async Task DeleteSpaceAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        await _spacesService.DeleteSpaceAsync(spaceId, cancellationToken);
    }

    public FluentCreateSpaceRequest CreateSpace(string workspaceId)
    {
        return new FluentCreateSpaceRequest(workspaceId, _spacesService);
    }

    public FluentUpdateSpaceRequest UpdateSpace(string spaceId)
    {
        return new FluentUpdateSpaceRequest(spaceId, _spacesService);
    }
}
