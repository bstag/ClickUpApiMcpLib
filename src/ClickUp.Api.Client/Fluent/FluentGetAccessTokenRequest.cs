using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Authorization;
using ClickUp.Api.Client.Models.ResponseModels.Authorization;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentGetAccessTokenRequest
{
    private readonly GetAccessTokenRequest _request = new();
    private readonly IAuthorizationService _authorizationService;

    public FluentGetAccessTokenRequest(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public FluentGetAccessTokenRequest WithClientId(string clientId)
    {
        _request.ClientId = clientId;
        return this;
    }

    public FluentGetAccessTokenRequest WithClientSecret(string clientSecret)
    {
        _request.ClientSecret = clientSecret;
        return this;
    }

    public FluentGetAccessTokenRequest WithCode(string code)
    {
        _request.Code = code;
        return this;
    }

    public async Task<GetAccessTokenResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _authorizationService.GetAccessTokenAsync(
            _request.ClientId,
            _request.ClientSecret,
            _request.Code,
            cancellationToken
        );
    }
}