using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Strategies
{
    /// <summary>
    /// Interface for retry strategies that can be used to handle failed API requests.
    /// </summary>
    public interface IRetryStrategy
    {
        /// <summary>
        /// Gets the name of the retry strategy.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this retry strategy is enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets the maximum number of retry attempts.
        /// </summary>
        int MaxRetryAttempts { get; }

        /// <summary>
        /// Determines whether the specified exception should trigger a retry.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="attemptNumber">The current attempt number (1-based).</param>
        /// <returns>True if the request should be retried; otherwise, false.</returns>
        bool ShouldRetry(Exception exception, int attemptNumber);

        /// <summary>
        /// Determines whether the specified HTTP response should trigger a retry.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="attemptNumber">The current attempt number (1-based).</param>
        /// <returns>True if the request should be retried; otherwise, false.</returns>
        bool ShouldRetry(HttpResponseMessage response, int attemptNumber);

        /// <summary>
        /// Calculates the delay before the next retry attempt.
        /// </summary>
        /// <param name="attemptNumber">The current attempt number (1-based).</param>
        /// <param name="exception">The exception that occurred (if any).</param>
        /// <param name="response">The HTTP response (if any).</param>
        /// <returns>The delay before the next retry attempt.</returns>
        TimeSpan CalculateDelay(int attemptNumber, Exception? exception = null, HttpResponseMessage? response = null);

        /// <summary>
        /// Executes an operation with retry logic.
        /// </summary>
        /// <typeparam name="T">The return type of the operation.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of the operation.</returns>
        Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes an HTTP request with retry logic.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use.</param>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The HTTP response message.</returns>
        Task<HttpResponseMessage> ExecuteHttpAsync(HttpClient httpClient, HttpRequestMessage request, CancellationToken cancellationToken = default);
    }
}