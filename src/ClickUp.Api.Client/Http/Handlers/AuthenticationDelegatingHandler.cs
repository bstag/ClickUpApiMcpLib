using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ClickUp.Api.Client.Abstractions.Options;

namespace ClickUp.Api.Client.Http.Handlers
{
    /// <summary>
    /// A <see cref="DelegatingHandler"/> that adds the ClickUp API authentication header to requests,
    /// supporting either Personal Access Token or OAuth 2.0 Bearer Token.
    /// </summary>
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IOptions<ClickUpClientOptions> _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationDelegatingHandler"/> class.
        /// </summary>
        /// <param name="options">The ClickUp client options containing authentication details.</param>
        /// <exception cref="ArgumentNullException">Thrown if options is null.</exception>
        public AuthenticationDelegatingHandler(IOptions<ClickUpClientOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Sends an HTTP request with the appropriate authentication header.
        /// Prioritizes OAuth Access Token if available, otherwise uses Personal Access Token.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>The HTTP response message.</returns>
        /// <exception cref="InvalidOperationException">Thrown if neither OAuth nor Personal Access Token is configured.</exception>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var currentOptions = _options.Value;

            if (!string.IsNullOrWhiteSpace(currentOptions.OAuthAccessToken))
            {
                // OAuth 2.0 uses Bearer token scheme
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", currentOptions.OAuthAccessToken);
            }
            else if (!string.IsNullOrWhiteSpace(currentOptions.PersonalAccessToken))
            {
                // ClickUp Personal Access Token should be sent directly without Bearer prefix
                // See: https://clickup.com/api/developer-portal/authentication/
                // Format: Authorization: {personal_token}
                request.Headers.Authorization = AuthenticationHeaderValue.Parse(currentOptions.PersonalAccessToken);
            }
            else
            {
                // This case should ideally be prevented by checks in ServiceCollectionExtensions or by the application configuration
                throw new InvalidOperationException(
                    "Authentication token not configured. Please provide either an OAuthAccessToken or a PersonalAccessToken in ClickUpClientOptions.");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
