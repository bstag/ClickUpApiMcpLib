using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.ResponseModels.Authorization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentAuthorizationApi
{
    private readonly IAuthorizationService _authorizationService;

    public FluentAuthorizationApi(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public FluentGetAccessTokenRequest GetAccessToken()
    {
        return new FluentGetAccessTokenRequest(_authorizationService);
    }

    public async Task<User> GetAuthorizedUserAsync(CancellationToken cancellationToken = default)
    {
        return await _authorizationService.GetAuthorizedUserAsync(cancellationToken);
    }

    public async Task<IEnumerable<ClickUpWorkspace>> GetAuthorizedWorkspacesAsync(CancellationToken cancellationToken = default)
    {
        return await _authorizationService.GetAuthorizedWorkspacesAsync(cancellationToken);
    }
}
