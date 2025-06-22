using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models; // For ClickUpWorkspace
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClickUp.Api.Client.Models.Entities.Users; // For User
using ClickUp.Api.Client.Models.RequestModels.Authorization;
using ClickUp.Api.Client.Models.ResponseModels.Authorization;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces; // For GetAuthorizedWorkspacesResponse

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IAuthorizationService"/> for ClickUp API authorization operations.
    /// </summary>
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<AuthorizationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public AuthorizationService(IApiConnection apiConnection, ILogger<AuthorizationService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<AuthorizationService>.Instance;
        }

        /// <inheritdoc />
        public async Task<GetAccessTokenResponse?> GetAccessTokenAsync(
            string clientId,
            string clientSecret,
            string code,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting access token for Client ID: {ClientId}", clientId);
            var endpoint = "oauth/token";
            var payload = new GetAccessTokenRequest(clientId, clientSecret, code);
            return await _apiConnection.PostAsync<GetAccessTokenRequest, GetAccessTokenResponse>(endpoint, payload, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User?> GetAuthorizedUserAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting authorized user");
            var endpoint = "user";
            var response = await _apiConnection.GetAsync<GetAuthorizedUserResponse>(endpoint, cancellationToken);
            return response?.User;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ClickUpWorkspace>?> GetAuthorizedWorkspacesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting authorized workspaces");
            var endpoint = "team";
            var response = await _apiConnection.GetAsync<GetAuthorizedWorkspacesResponse>(endpoint, cancellationToken);
            return response?.Workspaces; // Changed from Teams to Workspaces to match DTO property name
        }
    }
}
