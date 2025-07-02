using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.ResponseModels.Authorization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.WorkSpaces;

namespace ClickUp.Api.Client.Fluent;

public class AuthorizationFluentApi
{
    private readonly IAuthorizationService _authorizationService;

    public AuthorizationFluentApi(IAuthorizationService authorizationService)
    {
        if (authorizationService == null)
            throw new ArgumentNullException(nameof(authorizationService));
        _authorizationService = authorizationService;
    }

    public AccessTokenFluentGetSingleRequest GetAccessToken()
    {
        return new AccessTokenFluentGetSingleRequest(_authorizationService);
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
