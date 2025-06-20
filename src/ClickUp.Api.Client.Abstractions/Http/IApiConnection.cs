using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Http
{
    /// <summary>
    /// Defines the contract for a connection to the ClickUp API, handling HTTP requests and responses.
    /// </summary>
    public interface IApiConnection
    {
        /// <summary>
        /// Sends a GET request to the specified endpoint and returns the deserialized response.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response object.</typeparam>
        /// <param name="endpoint">The API endpoint (relative to the base URL).</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The deserialized response from the API, or null if the response content is empty or cannot be deserialized.</returns>
        Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a POST request with a JSON payload to the specified endpoint and returns the deserialized response.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request payload.</typeparam>
        /// <typeparam name="TResponse">The type of the response object.</typeparam>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="payload">The request payload to serialize as JSON.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized response from the API.</returns>
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a POST request with a JSON payload to the specified endpoint without expecting a deserializable response body.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request payload.</typeparam>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="payload">The request payload to serialize as JSON.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task PostAsync<TRequest>(string endpoint, TRequest payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a PUT request with a JSON payload to the specified endpoint and returns the deserialized response.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request payload.</typeparam>
        /// <typeparam name="TResponse">The type of the response object.</typeparam>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="payload">The request payload to serialize as JSON.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized response from the API.</returns>
        Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a PUT request with a JSON payload to the specified endpoint without expecting a deserializable response body.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request payload.</typeparam>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="payload">The request payload to serialize as JSON.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task PutAsync<TRequest>(string endpoint, TRequest payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DELETE request to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a POST request with multipart/form-data content to the specified endpoint and returns the deserialized response.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response object.</typeparam>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="content">The multipart/form-data content.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized response from the API.</returns>
        Task<TResponse?> PostMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent content, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DELETE request to the specified endpoint and returns the deserialized response.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response object.</typeparam>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized response from the API.</returns>
        Task<TResponse?> DeleteAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DELETE request with a JSON payload to the specified endpoint.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request payload.</typeparam>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="payload">The request payload to serialize as JSON.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync<TRequest>(string endpoint, TRequest payload, CancellationToken cancellationToken = default);
    }
}
