using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Strategies;
using Microsoft.Extensions.Logging;

namespace ClickUp.Api.Client.Strategies.Retry
{
    /// <summary>
    /// Linear retry strategy implementation with fixed delays between retries.
    /// </summary>
    public class LinearRetryStrategy : IRetryStrategy
    {
        private readonly ILogger<LinearRetryStrategy>? _logger;
        private readonly int _maxRetries;
        private readonly TimeSpan _fixedDelay;
        private readonly bool _useJitter;
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearRetryStrategy"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="maxRetries">The maximum number of retry attempts.</param>
        /// <param name="fixedDelay">The fixed delay between retries.</param>
        /// <param name="useJitter">Whether to add jitter to prevent thundering herd.</param>
        public LinearRetryStrategy(
            ILogger<LinearRetryStrategy>? logger = null,
            int maxRetries = 3,
            TimeSpan? fixedDelay = null,
            bool useJitter = false)
        {
            _logger = logger;
            _maxRetries = Math.Max(0, maxRetries);
            _fixedDelay = fixedDelay ?? TimeSpan.FromSeconds(2);
            _useJitter = useJitter;
            _random = new Random();
        }

        /// <inheritdoc />
        public string Name => "Linear";

        /// <inheritdoc />
        public bool IsEnabled => true;

        /// <inheritdoc />
        public int MaxRetryAttempts => _maxRetries;

        /// <inheritdoc />
        public bool ShouldRetry(Exception exception, int attemptNumber)
        {
            if (attemptNumber >= _maxRetries)
            {
                _logger?.LogDebug("Max retries ({MaxRetries}) reached for exception: {Exception}", _maxRetries, exception.GetType().Name);
                return false;
            }

            // Retry on specific exceptions
            var shouldRetry = exception switch
            {
                HttpRequestException => true,
                TaskCanceledException when !exception.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) => false, // Don't retry on cancellation
                TaskCanceledException => true, // Retry on timeout
                TimeoutException => true,
                SocketException => true,
                _ => false
            };

            _logger?.LogDebug("Retry decision for {Exception} (attempt {Attempt}): {ShouldRetry}", 
                exception.GetType().Name, attemptNumber, shouldRetry);

            return shouldRetry;
        }

        /// <inheritdoc />
        public bool ShouldRetry(HttpResponseMessage response, int attemptNumber)
        {
            if (attemptNumber >= _maxRetries)
            {
                _logger?.LogDebug("Max retries ({MaxRetries}) reached for HTTP {StatusCode}", _maxRetries, response.StatusCode);
                return false;
            }

            // Retry on specific HTTP status codes
            var shouldRetry = response.StatusCode switch
            {
                HttpStatusCode.InternalServerError => true,
                HttpStatusCode.BadGateway => true,
                HttpStatusCode.ServiceUnavailable => true,
                HttpStatusCode.GatewayTimeout => true,
                HttpStatusCode.TooManyRequests => true,
                HttpStatusCode.RequestTimeout => true,
                _ => false
            };

            _logger?.LogDebug("Retry decision for HTTP {StatusCode} (attempt {Attempt}): {ShouldRetry}", 
                response.StatusCode, attemptNumber, shouldRetry);

            return shouldRetry;
        }

        /// <inheritdoc />
        public TimeSpan CalculateDelay(int attemptNumber, Exception? exception = null, HttpResponseMessage? response = null)
        {
            if (attemptNumber <= 0)
                return TimeSpan.Zero;

            // Check for Retry-After header in HTTP responses (takes precedence)
            if (response?.Headers.RetryAfter != null)
            {
                var retryAfter = response.Headers.RetryAfter;
                if (retryAfter.Delta.HasValue)
                {
                    var headerDelay = retryAfter.Delta.Value;
                    _logger?.LogDebug("Using Retry-After header delay: {Delay}", headerDelay);
                    return headerDelay;
                }
                else if (retryAfter.Date.HasValue)
                {
                    var headerDelay = retryAfter.Date.Value - DateTimeOffset.UtcNow;
                    if (headerDelay > TimeSpan.Zero)
                    {
                        _logger?.LogDebug("Using Retry-After header date delay: {Delay}", headerDelay);
                        return headerDelay;
                    }
                }
            }

            var delay = _fixedDelay;

            // Add jitter to prevent thundering herd
            if (_useJitter)
            {
                var jitterRange = delay.TotalMilliseconds * 0.1; // 10% jitter
                var jitter = (_random.NextDouble() - 0.5) * 2 * jitterRange;
                delay = TimeSpan.FromMilliseconds(Math.Max(0, delay.TotalMilliseconds + jitter));
            }

            _logger?.LogDebug("Calculated linear delay for attempt {Attempt}: {Delay}", attemptNumber, delay);
            return delay;
        }

        /// <inheritdoc />
        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            Exception? lastException = null;
            
            for (int attempt = 1; attempt <= _maxRetries + 1; attempt++)
            {
                try
                {
                    _logger?.LogDebug("Executing operation, attempt {Attempt}", attempt);
                    return await operation(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex) when (attempt <= _maxRetries && ShouldRetry(ex, attempt))
                {
                    lastException = ex;
                    var delay = CalculateDelay(attempt, ex);
                    
                    _logger?.LogWarning("Operation failed on attempt {Attempt}, retrying after {Delay}: {Exception}", 
                        attempt, delay, ex.Message);
                    
                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            // If we get here, all retries have been exhausted
            _logger?.LogError("Operation failed after {MaxRetries} retries: {Exception}", 
                _maxRetries, lastException?.Message);
            
            throw lastException ?? new InvalidOperationException("Operation failed after all retry attempts");
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> ExecuteHttpAsync(HttpClient httpClient, HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Exception? lastException = null;
            HttpResponseMessage? lastResponse = null;
            
            for (int attempt = 1; attempt <= _maxRetries + 1; attempt++)
            {
                try
                {
                    _logger?.LogDebug("Executing HTTP request, attempt {Attempt}", attempt);
                    
                    // Clone the request for retry attempts (except the first one)
                    var requestToSend = attempt == 1 ? request : await CloneHttpRequestMessageAsync(request).ConfigureAwait(false);
                    var response = await httpClient.SendAsync(requestToSend, cancellationToken).ConfigureAwait(false);
                    
                    if (attempt <= _maxRetries && ShouldRetry(response, attempt))
                    {
                        lastResponse?.Dispose();
                        lastResponse = response;
                        
                        var delay = CalculateDelay(attempt, response: response);
                        
                        _logger?.LogWarning("HTTP operation returned {StatusCode} on attempt {Attempt}, retrying after {Delay}", 
                            response.StatusCode, attempt, delay);
                        
                        if (delay > TimeSpan.Zero)
                        {
                            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                        }
                        continue;
                    }
                    
                    return response;
                }
                catch (Exception ex) when (attempt <= _maxRetries && ShouldRetry(ex, attempt))
                {
                    lastException = ex;
                    var delay = CalculateDelay(attempt, ex);
                    
                    _logger?.LogWarning("HTTP operation failed on attempt {Attempt}, retrying after {Delay}: {Exception}", 
                        attempt, delay, ex.Message);
                    
                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            // If we get here, all retries have been exhausted
            lastResponse?.Dispose();
            
            if (lastException != null)
            {
                _logger?.LogError("HTTP operation failed after {MaxRetries} retries: {Exception}", 
                    _maxRetries, lastException.Message);
                throw lastException;
            }
            
            throw new InvalidOperationException("HTTP operation failed after all retry attempts");
        }

        /// <summary>
        /// Clones an HTTP request message for retry attempts.
        /// </summary>
        /// <param name="original">The original HTTP request message.</param>
        /// <returns>A cloned HTTP request message.</returns>
        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage original)
        {
            var clone = new HttpRequestMessage(original.Method, original.RequestUri)
            {
                Version = original.Version
            };

            // Copy headers
            foreach (var header in original.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Copy content if present
            if (original.Content != null)
            {
                var contentBytes = await original.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                clone.Content = new ByteArrayContent(contentBytes);

                // Copy content headers
                foreach (var header in original.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return clone;
        }
    }
}