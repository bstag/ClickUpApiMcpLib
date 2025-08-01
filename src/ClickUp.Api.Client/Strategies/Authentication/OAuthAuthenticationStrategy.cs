using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Strategies;
using Microsoft.Extensions.Logging;

namespace ClickUp.Api.Client.Strategies.Authentication
{
    /// <summary>
    /// OAuth authentication strategy implementation for ClickUp API.
    /// </summary>
    public class OAuthAuthenticationStrategy : IAuthenticationStrategy
    {
        private readonly ILogger<OAuthAuthenticationStrategy>? _logger;
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tokenEndpoint;
        private readonly object _lock = new object();
        
        private string? _accessToken;
        private string? _refreshToken;
        private DateTimeOffset? _tokenExpiration;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthAuthenticationStrategy"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for token requests.</param>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <param name="clientSecret">The OAuth client secret.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenEndpoint">The OAuth token endpoint URL.</param>
        public OAuthAuthenticationStrategy(
            HttpClient httpClient,
            string clientId,
            string clientSecret,
            ILogger<OAuthAuthenticationStrategy>? logger = null,
            string tokenEndpoint = "https://api.clickup.com/api/v2/oauth/token")
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
            _logger = logger;
            _tokenEndpoint = tokenEndpoint ?? throw new ArgumentNullException(nameof(tokenEndpoint));
        }

        /// <inheritdoc />
        public string Name => "OAuth 2.0";

        /// <inheritdoc />
        public string Scheme => "Bearer";

        /// <inheritdoc />
        public bool IsEnabled => !string.IsNullOrEmpty(_accessToken);

        /// <inheritdoc />
        public bool RequiresCredentials => true;

        /// <inheritdoc />
        public bool SupportsTokenRefresh => true;

        /// <summary>
        /// Sets the OAuth tokens.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="expiresIn">The token expiration time in seconds.</param>
        public void SetTokens(string accessToken, string? refreshToken = null, int? expiresIn = null)
        {
            lock (_lock)
            {
                _accessToken = accessToken;
                _refreshToken = refreshToken;
                _tokenExpiration = expiresIn.HasValue 
                    ? DateTimeOffset.UtcNow.AddSeconds(expiresIn.Value)
                    : null;
                
                _logger?.LogDebug("OAuth tokens updated. Expires: {Expiration}", _tokenExpiration);
            }
        }

        /// <inheritdoc />
        public Task ApplyAuthenticationAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            string? currentAccessToken;
            lock (_lock)
            {
                currentAccessToken = _accessToken;
            }

            if (string.IsNullOrEmpty(currentAccessToken))
            {
                _logger?.LogWarning("Access token is not set, skipping authentication");
                return Task.CompletedTask;
            }

            try
            {
                // Remove existing authorization header if present
                if (request.Headers.Authorization != null)
                {
                    request.Headers.Authorization = null;
                }

                // Add the Bearer token to the request headers
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", currentAccessToken);
                
                _logger?.LogDebug("Applied OAuth Bearer token authentication to request: {Method} {Uri}", 
                    request.Method, request.RequestUri);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to apply OAuth authentication");
                throw new InvalidOperationException("Failed to apply OAuth authentication", ex);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<bool> ValidateCredentialsAsync(CancellationToken cancellationToken = default)
        {
            string? currentAccessToken;
            lock (_lock)
            {
                currentAccessToken = _accessToken;
            }

            var isValid = !string.IsNullOrEmpty(currentAccessToken) && !IsTokenExpired();
            
            _logger?.LogDebug("OAuth credentials validation result: {IsValid}", isValid);
            return Task.FromResult(isValid);
        }

        /// <inheritdoc />
        public async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            string? currentRefreshToken;
            lock (_lock)
            {
                currentRefreshToken = _refreshToken;
            }

            if (string.IsNullOrEmpty(currentRefreshToken))
            {
                _logger?.LogWarning("Refresh token is not available");
                return false;
            }

            try
            {
                _logger?.LogDebug("Attempting to refresh OAuth token");

                var requestBody = new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["client_id"] = _clientId,
                    ["client_secret"] = _clientSecret,
                    ["refresh_token"] = currentRefreshToken
                };

                var request = new HttpRequestMessage(HttpMethod.Post, _tokenEndpoint)
                {
                    Content = new FormUrlEncodedContent(requestBody)
                };

                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                    if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.AccessToken))
                    {
                        SetTokens(
                            tokenResponse.AccessToken,
                            tokenResponse.RefreshToken ?? currentRefreshToken,
                            tokenResponse.ExpiresIn);

                        _logger?.LogDebug("OAuth token refreshed successfully");
                        return true;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    _logger?.LogWarning("Token refresh failed with status {StatusCode}: {Error}", 
                        response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while refreshing OAuth token");
            }

            return false;
        }

        /// <inheritdoc />
        public Task<Dictionary<string, string>> GetAuthenticationHeadersAsync(CancellationToken cancellationToken = default)
        {
            var headers = new Dictionary<string, string>();

            string? currentAccessToken;
            lock (_lock)
            {
                currentAccessToken = _accessToken;
            }

            if (!string.IsNullOrEmpty(currentAccessToken))
            {
                headers["Authorization"] = $"Bearer {currentAccessToken}";
                _logger?.LogDebug("Generated OAuth authentication headers");
            }
            else
            {
                _logger?.LogWarning("Access token is not set, returning empty headers");
            }

            return Task.FromResult(headers);
        }

        /// <inheritdoc />
        public async Task<bool> HandleAuthenticationFailureAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            if (response == null)
                return false;

            // Try to refresh the token if we get an unauthorized response
            if (response.StatusCode == HttpStatusCode.Unauthorized && SupportsTokenRefresh)
            {
                _logger?.LogDebug("Received 401 Unauthorized, attempting token refresh");
                return await RefreshTokenAsync(cancellationToken).ConfigureAwait(false);
            }

            _logger?.LogWarning("Authentication failure with OAuth strategy. Status: {StatusCode}", response.StatusCode);
            return false;
        }

        /// <inheritdoc />
        public DateTimeOffset? GetTokenExpiration()
        {
            lock (_lock)
            {
                return _tokenExpiration;
            }
        }

        /// <inheritdoc />
        public bool IsTokenExpired(TimeSpan? bufferTime = null)
        {
            var expiration = GetTokenExpiration();
            if (!expiration.HasValue)
                return false;

            var buffer = bufferTime ?? TimeSpan.FromMinutes(5);
            return expiration.Value <= DateTimeOffset.UtcNow.Add(buffer);
        }

        /// <inheritdoc />
        public Task ClearCredentialsAsync(CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                _accessToken = null;
                _refreshToken = null;
                _tokenExpiration = null;
                _logger?.LogDebug("OAuth credentials cleared");
            }

            return Task.CompletedTask;
        }

        private class TokenResponse
        {
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
            public int? ExpiresIn { get; set; }
            public string? TokenType { get; set; }
        }
    }
}