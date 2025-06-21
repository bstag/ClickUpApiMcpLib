# Detailed Plan: Resilience with Polly Integration

This document details the plan for integrating Polly policies into the `HttpClient` setup to provide resilience against transient HTTP errors and improve the robustness of the ClickUp API SDK.

**Source Documents:**
*   `docs/plans/NEW_OVERALL_PLAN.md` (Phase 2, Step 8)
*   `docs/plans/updatedPlans/http/03-HttpClientAndHelpers.md` (HttpClient setup context)

**Location in Codebase:**
*   Polly policy configuration will be part of the `IServiceCollection` extension method (e.g., `AddClickUpApiClient`) in the `ClickUp.Api.Client` project, where `IHttpClientFactory` is configured.

## 1. Polly Policies to Implement

The following Polly policies will be configured for the `HttpClient` used by the SDK:

1.  **Retry Policy for Transient Errors:**
    *   **Purpose:** Automatically retry requests that fail due to transient network issues or temporary server-side problems.
    *   **Trigger Conditions:**
        *   `HttpRequestException` (indicating network-level failures).
        *   HTTP status codes:
            *   `5xx` (Server errors: 500, 502, 503, 504).
            *   `408` (Request Timeout).
            *   Possibly `429` (Too Many Requests) if the API doesn't provide a `Retry-After` header that the Circuit Breaker handles more gracefully. If `Retry-After` is present, the Circuit Breaker or a dedicated rate limit policy might be better. For now, include 429 here for basic retry, but this may be refined.
    *   **Retry Strategy:** Exponential backoff with jitter.
        *   Number of retries: e.g., 3 to 5.
        *   Initial delay: e.g., 1-2 seconds.
        *   Backoff factor: 2 (delay doubles with each retry).
        *   Jitter: Add a small random component to the delay to prevent thundering herd scenarios.
    *   **Implementation:**
        ```csharp
        // Within services.AddHttpClient<ClickUpHttpClient>(...)
        .AddTransientHttpErrorPolicy(policyBuilder =>
            policyBuilder.Or<HttpRequestException>() // Also handle network errors
                       .WaitAndRetryAsync(
                           retryCount: 3, // Configurable
                           sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // Exponential backoff
                               + TimeSpan.FromMilliseconds(new Random().Next(0, 1000)), // Jitter
                           onRetry: (outcome, timespan, retryAttempt, context) =>
                           {
                               // Log the retry attempt
                               var logger = context.GetLogger(); // Requires context and helper to get logger
                               logger?.LogWarning(outcome.Exception,
                                   "Delaying for {Timespan}ms, then making retry {RetryAttempt} of request to {RequestUri}",
                                   timespan.TotalMilliseconds, retryAttempt, context.GetRequestUri()); // Helper to get URI
                           }
                       )
        )
        ```
        *Note: Accessing `ILogger` and `HttpRequestMessage` within `onRetry` requires passing them via `Polly.Context`. The `IHttpClientFactory` integration with Polly provides some of this automatically.*

2.  **Circuit Breaker Policy:**
    *   **Purpose:** Prevent the application from repeatedly trying to call an endpoint that is consistently failing, allowing the downstream service time to recover.
    *   **Trigger Conditions:** Same as the retry policy (transient HTTP errors and `HttpRequestException`).
    *   **Configuration:**
        *   Exceptions allowed before breaking: e.g., 5 consecutive failures.
        *   Duration of break: e.g., 30 seconds to 1 minute.
        *   `onBreak`: Log that the circuit is breaking.
        *   `onReset`: Log that the circuit is resetting.
        *   `onHalfOpen`: Log that the circuit is attempting a trial request.
    *   **Implementation:**
        ```csharp
        // Chained after the retry policy
        .AddPolicyHandler(Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(response => (int)response.StatusCode >= 500 || response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.TooManyRequests)
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5, // Configurable
                durationOfBreak: TimeSpan.FromSeconds(30), // Configurable
                onBreak: (result, timespan, context) =>
                {
                    var logger = context.GetLogger();
                    logger?.LogWarning(result.Exception ?? new Exception($"Breaking circuit for {timespan.TotalSeconds}s due to {result.Result?.StatusCode} from {context.GetRequestUri()}"));
                },
                onReset: (context) =>
                {
                    var logger = context.GetLogger();
                    logger?.LogInformation($"Circuit closed for {context.GetRequestUri()}");
                },
                onHalfOpen: (context) =>
                {
                    var logger = context.GetLogger();
                    logger?.LogInformation($"Circuit half-open for {context.GetRequestUri()}, next call is a trial.");
                }
            )
        )
        ```

3.  **(Optional) Timeout Policy:**
    *   **Purpose:** Ensure that individual HTTP requests do not hang indefinitely.
    *   **Configuration:** Define a reasonable timeout per request (e.g., 30-60 seconds). `HttpClient.Timeout` provides a total timeout for a request including retries. Polly's TimeoutPolicy can apply per try.
    *   **Note:** `HttpClient.Timeout` (default 100 seconds) might be sufficient. If more granular per-try timeouts are needed, a Polly `TimeoutPolicy` can be added. For now, rely on `HttpClient.Timeout`.

## 2. Policy Configuration and Order

*   Policies will be configured using the `Microsoft.Extensions.Http.Polly` integration with `IHttpClientFactory`.
*   **Order Matters:** Policies are typically wrapped from outside-in. A common order is:
    1.  Retry Policy (inner)
    2.  Circuit Breaker Policy (outer)
    *   This means a request attempt goes through the Circuit Breaker first. If closed/half-open, it proceeds. If it fails and is a transient error, the Retry Policy handles it. If retries are exhausted and still failing, these failures count towards the Circuit Breaker's threshold.

## 3. Context Propagation

*   For logging within policy delegates (`onRetry`, `onBreak`, etc.), `Polly.Context` can be used to pass information like the `ILogger` instance or `HttpRequestMessage`.
*   Helper extension methods for `Context` like `GetLogger()` and `GetRequestUri()` might be created for convenience.

## 4. Making Policies Configurable

*   Key parameters of the policies (number of retries, break duration, exceptions allowed before breaking) should be configurable.
*   This can be achieved by accepting an `Action<PollyPolicyOptions>` in the `AddClickUpApiClient` extension method, where `PollyPolicyOptions` is a custom class holding these values. These options can then be read from the application's configuration.

    ```csharp
    // Example Options class
    public class ClickUpPollyOptions
    {
        public int RetryCount { get; set; } = 3;
        public double InitialDelaySeconds { get; set; } = 1.0;
        public double JitterMilliseconds { get; set; } = 100.0;
        public int CircuitBreakerAllowedExceptions { get; set; } = 5;
        public double CircuitBreakerDurationSeconds { get; set; } = 30.0;
    }

    // In AddClickUpApiClient, use IOptions<ClickUpPollyOptions>
    // var pollyOptions = services.BuildServiceProvider().GetRequiredService<IOptions<ClickUpPollyOptions>>().Value;
    // ... then use pollyOptions.RetryCount etc. in policy configuration.
    ```

## 5. Testing Resilience Policies

*   Testing Polly policies often involves setting up a mock `HttpMessageHandler` that can simulate transient failures, specific HTTP status codes, or delays.
*   Assert that retries occur as expected and that the circuit breaker opens and closes correctly under defined conditions.
*   Unit tests can verify that the policies are added to the `HttpClient` pipeline.

## 6. Documentation

*   The SDK's documentation should mention the resilience policies in place and any configurable aspects.
*   Guidance on how consumers can further customize Polly policies if they take direct control of `HttpClient` configuration (though the default setup should be robust for most).

## Plan Output

*   This document `05-ResilienceWithPolly.md` will serve as the detailed plan.
*   It will specify the exact policies, their configurations (with default values), and the order of application.
*   It will include code snippets for how these policies are added in the `IServiceCollection` extension method.
*   It will outline how policy parameters can be made configurable.
```
