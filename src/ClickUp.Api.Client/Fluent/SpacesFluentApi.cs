using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Spaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class SpacesFluentApi
{
    private readonly ISpacesService _spacesService;

    public SpacesFluentApi(ISpacesService spacesService)
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

    public SpaceFluentCreateRequest CreateSpace(string workspaceId)
    {
        return new SpaceFluentCreateRequest(workspaceId, _spacesService);
    }

    public SpaceFluentUpdateRequest UpdateSpace(string spaceId)
    {
        return new SpaceFluentUpdateRequest(spaceId, _spacesService);
    }
}
