using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Authorization;
using ClickUp.Api.Client.Models.ResponseModels.Authorization;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class AccessTokenFluentGetSingleRequest
{
    private readonly GetAccessTokenRequest _request = new();
    private readonly IAuthorizationService _authorizationService;

    public AccessTokenFluentGetSingleRequest(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public AccessTokenFluentGetSingleRequest WithClientId(string clientId)
    {
        _request.ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("Client ID cannot be empty or whitespace.", nameof(clientId));
        }
        return this;
    }

    public AccessTokenFluentGetSingleRequest WithClientSecret(string clientSecret)
    {
        _request.ClientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
        if (string.IsNullOrWhiteSpace(clientSecret))
        {
            throw new ArgumentException("Client secret cannot be empty or whitespace.", nameof(clientSecret));
        }
        return this;
    }

    public AccessTokenFluentGetSingleRequest WithCode(string code)
    {
        _request.Code = code ?? throw new ArgumentNullException(nameof(code));
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code cannot be empty or whitespace.", nameof(code));
        }
        return this;
    }

    public async Task<GetAccessTokenResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_request.ClientId))
            throw new InvalidOperationException("Client ID must be set before calling GetAsync.");
            
        if (string.IsNullOrWhiteSpace(_request.ClientSecret))
            throw new InvalidOperationException("Client secret must be set before calling GetAsync.");
            
        if (string.IsNullOrWhiteSpace(_request.Code))
            throw new InvalidOperationException("Code must be set before calling GetAsync.");

        return await _authorizationService.GetAccessTokenAsync(
            _request.ClientId,
            _request.ClientSecret,
            _request.Code,
            cancellationToken
        );
    }
}