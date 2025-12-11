using System.Net.Http;
using ClickUp.Api.Client.Abstractions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ClickUp.Api.Client.Infrastructure
{
    /// <summary>
    /// Concrete implementation of IHttpClientFactory that wraps the .NET HttpClientFactory.
    /// This implementation provides dependency inversion while leveraging the built-in HttpClientFactory.
    /// </summary>
    /// <remarks>
    /// This wrapper allows the ClickUp SDK to:
    /// - Use the standard .NET HttpClientFactory for production scenarios
    /// - Support dependency injection and testability
    /// - Maintain proper HttpClient lifecycle management
    /// - Support named and typed clients
    /// </remarks>
    public class HttpClientFactoryWrapper : Abstractions.Infrastructure.IHttpClientFactory
    {
        private readonly System.Net.Http.IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the HttpClientFactoryWrapper class.
        /// </summary>
        /// <param name="httpClientFactory">The .NET HttpClientFactory to wrap.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when httpClientFactory is null.</exception>
        public HttpClientFactoryWrapper(System.Net.Http.IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Creates an HttpClient instance using the default configuration.
        /// </summary>
        /// <returns>A configured HttpClient instance ready for use.</returns>
        /// <remarks>
        /// This method delegates to the underlying .NET HttpClientFactory to create
        /// a default HttpClient with standard configuration.
        /// </remarks>
        public HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Creates a named HttpClient instance with specific configuration.
        /// </summary>
        /// <param name="name">The logical name of the client to create.</param>
        /// <returns>A configured HttpClient instance for the specified name.</returns>
        /// <remarks>
        /// This method delegates to the underlying .NET HttpClientFactory to create
        /// a named HttpClient with configuration specific to the provided name.
        /// Named clients should be configured during service registration.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Thrown when name is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when name is empty or whitespace.</exception>
        public HttpClient CreateClient(string name)
        {
            if (name == null)
                throw new System.ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException("Client name cannot be empty or whitespace.", nameof(name));

            return _httpClientFactory.CreateClient(name);
        }
    }
}