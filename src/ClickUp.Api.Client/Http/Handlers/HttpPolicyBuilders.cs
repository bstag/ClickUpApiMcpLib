using System;
using System.Net;
using System.Net.Http;

using ClickUp.Api.Client.Models.Exceptions;

using Polly;
using Polly.Extensions.Http;

namespace ClickUp.Api.Client.Http.Handlers;

/// <summary>
/// Provides Polly policies for handling HTTP requests.
/// </summary>
public static class HttpPolicyBuilders
{
    /// <summary>
    /// Gets a policy that handles transient HTTP errors and retries with an exponential backoff.
    /// </summary>
    /// <param name="retries">The number of retries to attempt.</param>
    /// <returns>An <see cref="IAsyncPolicy{HttpResponseMessage}"/> for retrying requests.</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retries = 3)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles HttpRequestException, 5XX and 408
            .Or<ClickUpApiRateLimitException>() // Also retry on our custom rate limit exception
            .WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff: 2s, 4s, 8s
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Log or handle the retry attempt (optional)
                    // Example: context.GetLogger()?.LogWarning($"Delaying for {timespan.TotalSeconds}s, then making retry {retryAttempt}.");
                });
    }

    /// <summary>
    /// Gets a circuit breaker policy that breaks if too many failures occur.
    /// </summary>
    /// <param name="handledEventsAllowedBeforeBreaking">The number of handled events allowed before breaking the circuit.</param>
    /// <param name="durationOfBreak">The duration the circuit will stay open before attempting to reset.</param>
    /// <returns>An <see cref="IAsyncPolicy{HttpResponseMessage}"/> for the circuit breaker pattern.</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking = 5, TimeSpan? durationOfBreak = null)
    {
        durationOfBreak ??= TimeSpan.FromSeconds(30); // Default to 30 seconds

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<ClickUpApiRateLimitException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: handledEventsAllowedBeforeBreaking,
                durationOfBreak: durationOfBreak.Value,
                onBreak: (result, breakDelay, context) =>
                {
                    // Log or handle when the circuit breaks (optional)
                },
                onReset: context =>
                {
                    // Log or handle when the circuit resets (optional)
                },
                onHalfOpen: () =>
                {
                    // Log or handle when the circuit is half-open (optional)
                });
    }

    /// <summary>
    /// Gets a policy for handling cases where the server returns a 429 (Too Many Requests) status code with a Retry-After header.
    /// </summary>
    /// <returns>An <see cref="IAsyncPolicy{HttpResponseMessage}"/> for handling Retry-After responses.</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryAfterPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.TooManyRequests && response.Headers.RetryAfter != null)
            .WaitAndRetryAsync(
                retryCount: 1, // Only retry once based on the header
                sleepDurationProvider: (retryCount, response, context) =>
                {
                    var retryAfter = response.Result.Headers.RetryAfter;
                    if (retryAfter?.Delta.HasValue == true)
                    {
                        return retryAfter.Delta.Value;
                    }
                    if (retryAfter?.Date.HasValue == true)
                    {
                        return retryAfter.Date.Value - DateTimeOffset.UtcNow;
                    }
                    return TimeSpan.FromSeconds(5); // Default fallback if header is malformed
                },
                onRetryAsync: (response, timespan, retryAttempt, context) =>
                {
                    // Optional: Log the retry attempt
                    return Task.CompletedTask;
                });
    }
}
