using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Authorization;
using ClickUp.Api.Client.Models.ResponseModels.Authorization;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class GetAccessTokenFluentRequest
{
    private readonly GetAccessTokenRequest _request = new();
    private readonly IAuthorizationService _authorizationService;

    public GetAccessTokenFluentRequest(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public GetAccessTokenFluentRequest WithClientId(string clientId)
    {
        _request.ClientId = clientId;
        return this;
    }

    public GetAccessTokenFluentRequest WithClientSecret(string clientSecret)
    {
        _request.ClientSecret = clientSecret;
        return this;
    }

    public GetAccessTokenFluentRequest WithCode(string code)
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