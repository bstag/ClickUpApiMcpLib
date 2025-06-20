using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Http.Handlers
{
    /// <summary>
    /// A <see cref="DelegatingHandler"/> that adds the ClickUp Personal API Token to outgoing requests.
    /// </summary>
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationDelegatingHandler"/> class.
        /// </summary>
        /// <param name="apiKey">The ClickUp Personal API Token.</param>
        /// <exception cref="ArgumentNullException">Thrown if the API key is null or whitespace.</exception>
        public AuthenticationDelegatingHandler(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey), "API key cannot be null or whitespace.");
            }
            _apiKey = apiKey;
        }

        /// <summary>
        /// Sends an HTTP request with the API key added to the Authorization header.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // As per ClickUp API documentation for Personal API Token, the token itself is the header value.
            // No "Bearer" or other scheme is prefixed.
            request.Headers.Authorization = new AuthenticationHeaderValue(_apiKey);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
