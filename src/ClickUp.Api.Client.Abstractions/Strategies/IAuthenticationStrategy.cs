using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Strategies
{
    /// <summary>
    /// Interface for authentication strategies that can be used to authenticate API requests.
    /// </summary>
    public interface IAuthenticationStrategy
    {
        /// <summary>
        /// Gets the name of the authentication strategy.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the authentication scheme (e.g., "Bearer", "Basic", "ApiKey").
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Gets a value indicating whether this authentication strategy is enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this strategy supports token refresh.
        /// </summary>
        bool SupportsTokenRefresh { get; }

        /// <summary>
        /// Applies authentication to an HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request to authenticate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ApplyAuthenticationAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates the current authentication credentials.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the credentials are valid; otherwise, false.</returns>
        Task<bool> ValidateCredentialsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes the authentication token if supported.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the token was successfully refreshed; otherwise, false.</returns>
        Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the authentication headers that should be added to requests.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A dictionary of header names and values.</returns>
        Task<Dictionary<string, string>> GetAuthenticationHeadersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles authentication failures and determines if retry is possible.
        /// </summary>
        /// <param name="response">The failed HTTP response.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the request should be retried after handling the failure; otherwise, false.</returns>
        Task<bool> HandleAuthenticationFailureAsync(HttpResponseMessage response, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the expiration time of the current authentication token (if applicable).
        /// </summary>
        /// <returns>The token expiration time, or null if not applicable or unknown.</returns>
        DateTimeOffset? GetTokenExpiration();

        /// <summary>
        /// Checks if the current authentication token is expired or about to expire.
        /// </summary>
        /// <param name="bufferTime">The buffer time before expiration to consider the token as expired.</param>
        /// <returns>True if the token is expired or about to expire; otherwise, false.</returns>
        bool IsTokenExpired(TimeSpan? bufferTime = null);

        /// <summary>
        /// Clears the current authentication credentials.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ClearCredentialsAsync(CancellationToken cancellationToken = default);
    }
}