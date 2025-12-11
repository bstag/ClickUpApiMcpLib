using System.Net.Http;

namespace ClickUp.Api.Client.Abstractions.Infrastructure
{
    /// <summary>
    /// Abstraction for HTTP client factory to support dependency inversion principle.
    /// This interface provides a testable abstraction over the .NET HttpClientFactory.
    /// </summary>
    /// <remarks>
    /// This abstraction allows for:
    /// - Easy unit testing by providing mock implementations
    /// - Flexibility in HTTP client configuration and management
    /// - Decoupling from specific HTTP client factory implementations
    /// - Support for different HTTP client strategies (pooled, named, typed clients)
    /// </remarks>
    public interface IHttpClientFactory
    {
        /// <summary>
        /// Creates an HttpClient instance.
        /// </summary>
        /// <returns>A configured HttpClient instance ready for use.</returns>
        /// <remarks>
        /// The returned HttpClient should be properly configured with:
        /// - Base address (if applicable)
        /// - Default headers
        /// - Timeout settings
        /// - Authentication handlers (if needed)
        /// </remarks>
        HttpClient CreateClient();

        /// <summary>
        /// Creates a named HttpClient instance with specific configuration.
        /// </summary>
        /// <param name="name">The logical name of the client to create.</param>
        /// <returns>A configured HttpClient instance for the specified name.</returns>
        /// <remarks>
        /// Named clients allow for different configurations for different purposes:
        /// - Different base URLs
        /// - Different timeout settings
        /// - Different authentication mechanisms
        /// - Different retry policies
        /// </remarks>
        HttpClient CreateClient(string name);
    }
}