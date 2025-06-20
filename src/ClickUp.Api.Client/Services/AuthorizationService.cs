using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Authorization; // Assuming GetAccessTokenRequest DTO might exist or params are sent in a specific way
using ClickUp.Api.Client.Models.ResponseModels.Authorization;
using ClickUp.Api.Client.Models.ResponseModels; // For potential wrapper DTOs if GetAuthorizedWorkspacesAsync returns one

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IAuthorizationService"/> for ClickUp API authorization operations.
    /// </summary>
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public AuthorizationService(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        }

        /// <inheritdoc />
        public async Task<AccessTokenResponse?> GetAccessTokenAsync(
            string clientId,
            string clientSecret,
            string code,
            CancellationToken cancellationToken = default)
        {
            // The GetAccessToken endpoint is typically a POST request.
            // The parameters clientId, clientSecret, and code are usually sent in the request body.
            var endpoint = "oauth/token"; // Standard OAuth token endpoint, adjust if ClickUp uses a different one.
            var payload = new GetAccessTokenRequest(clientId, clientSecret, code); // Assuming a DTO for this

            // ClickUp API for Get Access Token: POST https://api.clickup.com/api/v2/oauth/token
            // Parameters: client_id, client_secret, code
            // These are sent as form parameters or JSON, depending on the API.
            // Assuming JSON payload for consistency with ApiConnection.

            return await _apiConnection.PostAsync<GetAccessTokenRequest, AccessTokenResponse>(endpoint, payload, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User?> GetAuthorizedUserAsync(CancellationToken cancellationToken = default)
        {
            var endpoint = "user"; // API endpoint: /user
            var response = await _apiConnection.GetAsync<GetAuthorizedUserResponse>(endpoint, cancellationToken); // API returns {"user": {...}}
            return response?.User;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Workspace>?> GetAuthorizedWorkspacesAsync(CancellationToken cancellationToken = default)
        {
            var endpoint = "team"; // API endpoint for workspaces/teams: /team
            var response = await _apiConnection.GetAsync<GetAuthorizedWorkspacesResponse>(endpoint, cancellationToken); // API returns {"teams": [...]}
            return response?.Teams;
        }
    }
}
