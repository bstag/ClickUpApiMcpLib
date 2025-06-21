# Detailed Plan: Resilience with Polly Integration

This document details the plan for integrating Polly policies into the `HttpClient` setup to provide resilience against transient HTTP errors and improve the robustness of the ClickUp API SDK.

**Source Documents:**
*   [`docs/plans/NEW_OVERALL_PLAN.md`](../NEW_OVERALL_PLAN.md) (Phase 2, Step 8)
*   [`docs/plans/updatedPlans/http/03-HttpClientAndHelpers.md`](./http/03-HttpClientAndHelpers.md) (HttpClient setup context)

**Location in Codebase:**
*   Polly policy configuration is part of the `IServiceCollection` extension method `AddClickUpClient` in `src/ClickUp.Api.Client/Extensions/ServiceCollectionExtensions.cs`.

## 1. Polly Policies to Implement

The following Polly policies are configured for the `HttpClient` used by the SDK:

- [x] **1. Retry Policy for Transient Errors:**
    - [x] **Purpose:** Automatically retry requests that fail due to transient network issues or temporary server-side problems.
    - [x] **Trigger Conditions:** Uses `AddTransientHttpErrorPolicy` which handles `HttpRequestException` and HTTP status codes 5xx and 408 by default.
    - [x] **Retry Strategy:** Exponential backoff with jitter.
        - [x] Number of retries: 3 (fixed in code).
        - [x] Sleep duration: `TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 500))`.
        - [x] Jitter: Random milliseconds between 0 and 500.
    - [x] **Implementation:**
        ```csharp
        // From src/ClickUp.Api.Client/Extensions/ServiceCollectionExtensions.cs
        .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                                 + TimeSpan.FromMilliseconds(jitterer.Next(0, 500)), // Jitter
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                // Using Console.WriteLine for now. In a real app, use ILogger.
                Console.WriteLine($"[PollyRetry] Request to {outcome.Result?.RequestMessage?.RequestUri} failed with {outcome.Result?.StatusCode}. Delaying for {timespan.TotalMilliseconds}ms, then making retry {retryAttempt}. CorrelationId: {context.CorrelationId}");
            }
        ))
        ```
    - [ ] *Refinement Note:* The current `.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(...))` will retry based on the default definition of transient errors (5xx, 408, HttpRequestException). The plan mentions `.Or<HttpRequestException>()` and specific status codes like 429. The current implementation is simpler by relying on the default `AddTransientHttpErrorPolicy` behavior. If 429 needs specific retry handling (especially considering `Retry-After` headers), it might require a more custom policy definition or ensuring the circuit breaker handles it appropriately.

- [x] **2. Circuit Breaker Policy:**
    - [x] **Purpose:** Prevent the application from repeatedly trying to call an endpoint that is consistently failing.
    - [x] **Trigger Conditions:** Uses `AddTransientHttpErrorPolicy` which handles `HttpRequestException` and HTTP status codes 5xx and 408 by default.
    - [x] **Configuration:**
        - [x] Exceptions allowed before breaking: 5 (fixed in code).
        - [x] Duration of break: 30 seconds (fixed in code).
        - [x] `onBreak`: Logs to console.
        - [x] `onReset`: Logs to console.
        - [x] `onHalfOpen`: Logs to console.
    - [x] **Implementation:**
        ```csharp
        // From src/ClickUp.Api.Client/Extensions/ServiceCollectionExtensions.cs
        .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (outcome, breakDelay, context) =>
            {
                Console.WriteLine($"[PollyCircuitBreaker] Circuit broken for {breakDelay.TotalSeconds}s for request to {outcome.Result?.RequestMessage?.RequestUri} due to {outcome.Result?.StatusCode}. CorrelationId: {context.CorrelationId}");
            },
            onReset: (context) =>
            {
                Console.WriteLine($"[PollyCircuitBreaker] Circuit reset. CorrelationId: {context.OperationKey}");
            },
            onHalfOpen: () =>
            {
                Console.WriteLine("[PollyCircuitBreaker] Circuit is now half-open; next request is a trial.");
            }
        ));
        ```
    - [ ] *Refinement Note:* Similar to the retry policy, this uses the default transient error definition. If specific handling for 429 (Too Many Requests) with `Retry-After` is desired for the circuit breaker, the policy definition might need to be more specific than the general `AddTransientHttpErrorPolicy`.

- [x] **3. (Optional) Timeout Policy:**
    - [x] **Status:** Not explicitly implemented with a Polly `TimeoutPolicy`.
    - [x] Relies on the default `HttpClient.Timeout` (which is 100 seconds). This is acceptable for now as per the plan.

## 2. Policy Configuration and Order

- [x] Policies are configured using `Microsoft.Extensions.Http.Polly` in `ServiceCollectionExtensions.cs`.
- [x] **Order:** The `WaitAndRetryAsync` policy is added first, then the `CircuitBreakerAsync` policy. This means the circuit breaker is the outer policy, and retries happen "inside" it. If retries fail, those failures count towards the circuit breaker. This matches the common recommended order.

## 3. Context Propagation

- [x] The `onRetry`, `onBreak`, `onReset` delegates in the current implementation use `Console.WriteLine` for logging.
- [ ] For proper logging, `ILogger` would need to be made available to these delegates, potentially via `Polly.Context` or by resolving `ILoggerFactory` from the `ServiceProvider` if the context is accessible. The current `Console.WriteLine` is a placeholder.
- [x] The context is used to get `RequestMessage.RequestUri` and `CorrelationId` in logs.

## 4. Making Policies Configurable

- [ ] Key parameters (retry count, delays, break duration, etc.) are currently hardcoded in `ServiceCollectionExtensions.cs`.
- [ ] The plan suggests a `ClickUpPollyOptions` class and using `IOptions<ClickUpPollyOptions>` to make these configurable. This is **not yet implemented**.

    ```csharp
    // Example Options class (Not yet implemented)
    // public class ClickUpPollyOptions
    // {
    //     public int RetryCount { get; set; } = 3;
    //     public double InitialDelaySeconds { get; set; } = 1.0;
    //     public double JitterMilliseconds { get; set; } = 100.0;
    //     public int CircuitBreakerAllowedExceptions { get; set; } = 5;
    //     public double CircuitBreakerDurationSeconds { get; set; } = 30.0;
    // }
    ```

## 5. Testing Resilience Policies

- [ ] This plan item refers to the strategy for testing. Actual tests are not part of this checklist for the plan document itself.

## 6. Documentation

- [ ] This plan item refers to SDK documentation. Not applicable to this checklist for the plan document itself.

## Plan Output

- [x] This document `05-ResilienceWithPolly.md` has been updated with checkboxes.
- [x] It reflects that basic Retry and Circuit Breaker policies are implemented using `AddTransientHttpErrorPolicy`.
- [x] It notes that advanced configuration (like specific handling for 429 with Retry-After, making parameters configurable via options, and robust logging) are areas for future enhancement.
```
```
