using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Strategies;
using Microsoft.Extensions.Logging;

namespace ClickUp.Api.Client.Strategies.Authentication
{
    /// <summary>
    /// API key authentication strategy implementation for ClickUp API.
    /// </summary>
    public class ApiKeyAuthenticationStrategy : IAuthenticationStrategy
    {
        private readonly ILogger<ApiKeyAuthenticationStrategy>? _logger;
        private string? _apiKey;
        private readonly string _headerName;
        private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyAuthenticationStrategy"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="headerName">The header name for the API key (default: "Authorization").</param>
        public ApiKeyAuthenticationStrategy(
            ILogger<ApiKeyAuthenticationStrategy>? logger = null,
            string? apiKey = null,
            string headerName = "Authorization")
        {
            _logger = logger;
            _apiKey = apiKey;
            _headerName = headerName ?? throw new ArgumentNullException(nameof(headerName));
        }

        /// <inheritdoc />
        public string Name => "API Key";

        /// <inheritdoc />
        public string Scheme => "ApiKey";

        /// <inheritdoc />
        public bool IsEnabled => !string.IsNullOrEmpty(_apiKey);

        /// <inheritdoc />
        public bool RequiresCredentials => true;

        /// <inheritdoc />
        public bool SupportsTokenRefresh => false;

        /// <summary>
        /// Sets the API key for authentication.
        /// </summary>
        /// <param name="apiKey">The API key to set.</param>
        public void SetApiKey(string apiKey)
        {
            lock (_lock)
            {
                _apiKey = apiKey;
                _logger?.LogDebug("API key updated");
            }
        }

        /// <inheritdoc />
        public Task ApplyAuthenticationAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            string? currentApiKey;
            lock (_lock)
            {
                currentApiKey = _apiKey;
            }

            if (string.IsNullOrEmpty(currentApiKey))
            {
                _logger?.LogWarning("API key is not set, skipping authentication");
                return Task.CompletedTask;
            }

            try
            {
                // Remove existing authorization header if present
                if (request.Headers.Contains(_headerName))
                {
                    request.Headers.Remove(_headerName);
                }

                // Add the API key to the request headers
                request.Headers.Add(_headerName, currentApiKey);
                
                _logger?.LogDebug("Applied API key authentication to request: {Method} {Uri}", 
                    request.Method, request.RequestUri);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to apply API key authentication");
                throw new InvalidOperationException("Failed to apply API key authentication", ex);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<bool> ValidateCredentialsAsync(CancellationToken cancellationToken = default)
        {
            string? currentApiKey;
            lock (_lock)
            {
                currentApiKey = _apiKey;
            }

            var isValid = !string.IsNullOrEmpty(currentApiKey) && currentApiKey.Trim().Length > 0;
            
            _logger?.LogDebug("API key validation result: {IsValid}", isValid);
            return Task.FromResult(isValid);
        }

        /// <inheritdoc />
        public Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            // API keys don't support refresh
            _logger?.LogDebug("Token refresh not supported for API key authentication");
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<Dictionary<string, string>> GetAuthenticationHeadersAsync(CancellationToken cancellationToken = default)
        {
            var headers = new Dictionary<string, string>();

            string? currentApiKey;
            lock (_lock)
            {
                currentApiKey = _apiKey;
            }

            if (!string.IsNullOrEmpty(currentApiKey))
            {
                headers[_headerName] = currentApiKey;
                _logger?.LogDebug("Generated authentication headers with API key");
            }
            else
            {
                _logger?.LogWarning("API key is not set, returning empty headers");
            }

            return Task.FromResult(headers);
        }

        /// <inheritdoc />
        public Task<bool> HandleAuthenticationFailureAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            if (response == null)
                return Task.FromResult(false);

            // For API key authentication, we typically can't recover from auth failures
            // The user needs to provide a valid API key
            var canRecover = false;

            _logger?.LogWarning("Authentication failure with API key strategy. Status: {StatusCode}, Can recover: {CanRecover}", 
                response.StatusCode, canRecover);

            return Task.FromResult(canRecover);
        }

        /// <inheritdoc />
        public DateTimeOffset? GetTokenExpiration()
        {
            // API keys typically don't have expiration dates
            return null;
        }

        /// <inheritdoc />
        public bool IsTokenExpired(TimeSpan? bufferTime = null)
        {
            // API keys typically don't expire
            return false;
        }

        /// <inheritdoc />
        public Task ClearCredentialsAsync(CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                _apiKey = null;
                _logger?.LogDebug("API key credentials cleared");
            }

            return Task.CompletedTask;
        }
    }
}