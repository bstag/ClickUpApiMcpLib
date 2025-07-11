// Copyright (c) AI General. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace ClickUp.Api.Client.Abstractions.Options;

/// <summary>
/// Configuration options for Polly resilience policies used by the ClickUp API client.
/// </summary>
public class ClickUpPollyOptions
{
    /// <summary>
    /// Gets or sets the number of retries for transient failures.
    /// </summary>
    /// <remarks>
    /// Default is 3.
    /// </remarks>
    public int RetryCount { get; set; } = ClickUpDefaults.DefaultRetryCount;

    /// <summary>
    /// Gets or sets the base delay for the first retry. Subsequent retries will use exponential backoff.
    /// </summary>
    /// <remarks>
    /// Default is 1 second.
    /// </remarks>
    public TimeSpan RetryBaseDelay { get; set; } = TimeSpan.FromSeconds(ClickUpDefaults.DefaultRetryDelaySeconds);

    /// <summary>
    /// Gets or sets the number of consecutive failures before the circuit breaker opens.
    /// </summary>
    /// <remarks>
    /// Default is 5.
    /// </remarks>
    public int CircuitBreakerFailureThreshold { get; set; } = ClickUpDefaults.DefaultCircuitBreakerThreshold;

    /// <summary>
    /// Gets or sets the duration the circuit will stay open before transitioning to half-open.
    /// </summary>
    /// <remarks>
    /// Default is 30 seconds.
    /// </remarks>
    public TimeSpan CircuitBreakerBreakDuration { get; set; } = TimeSpan.FromSeconds(ClickUpDefaults.DefaultCircuitBreakerBreakDurationSeconds);

    /// <summary>
    /// Gets or sets the duration the circuit will stay in the half-open state, allowing a single test request.
    /// </summary>
    /// <remarks>
    /// Default is 10 seconds. (Note: This is not directly used by Polly's built-in half-open state,
    /// but Polly manages the transition from half-open back to closed or open based on the test request's success.)
    /// For more fine-grained control, custom logic or advanced circuit breaker patterns might be needed.
    /// Polly's `durationOfBreak` is the primary configuration for how long it stays open.
    /// The half-open state allows one request after `durationOfBreak`. If it succeeds, the circuit closes. If it fails, it re-opens for `durationOfBreak` again.
    /// This property is kept for conceptual clarity if more advanced scenarios are built later.
    /// </remarks>
    public TimeSpan CircuitBreakerHalfOpenDuration { get; set; } = TimeSpan.FromSeconds(ClickUpDefaults.DefaultCircuitBreakerHalfOpenDurationSeconds);
}
