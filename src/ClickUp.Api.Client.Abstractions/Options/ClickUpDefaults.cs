// Copyright (c) AI General. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace ClickUp.Api.Client.Abstractions.Options;

/// <summary>
/// Default values and constants for ClickUp API client configuration.
/// </summary>
public static class ClickUpDefaults
{
    /// <summary>
    /// Default number of retries for transient failures.
    /// </summary>
    public const int DefaultRetryCount = 3;

    /// <summary>
    /// Default base delay for the first retry in seconds.
    /// </summary>
    public const int DefaultRetryDelaySeconds = 1;

    /// <summary>
    /// Default number of consecutive failures before the circuit breaker opens.
    /// </summary>
    public const int DefaultCircuitBreakerThreshold = 5;

    /// <summary>
    /// Default duration the circuit will stay open before transitioning to half-open in seconds.
    /// </summary>
    public const int DefaultCircuitBreakerBreakDurationSeconds = 30;

    /// <summary>
    /// Default duration the circuit will stay in the half-open state in seconds.
    /// </summary>
    public const int DefaultCircuitBreakerHalfOpenDurationSeconds = 10;

    /// <summary>
    /// Default base address for the ClickUp API.
    /// </summary>
    public const string DefaultBaseAddress = "https://api.clickup.com/api/v2/";

    /// <summary>
    /// Default user agent string for HTTP requests.
    /// </summary>
    public const string DefaultUserAgent = "ClickUp.Api.Client.Net";

    /// <summary>
    /// Maximum reasonable length for ClickUp entity IDs.
    /// </summary>
    public const int MaxIdLength = 50;

    /// <summary>
    /// Default jitter range for retry delays in milliseconds.
    /// </summary>
    public const int DefaultJitterRangeMs = 500;
}