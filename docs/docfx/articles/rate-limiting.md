# Rate Limiting and Resilience

When interacting with any web API, including ClickUp's, it's important to consider rate limits and the possibility of transient network errors. The `ClickUp.Api.Client` SDK incorporates resilience patterns using [Polly](https://github.com/App-vNext/Polly) to help manage these situations gracefully.

## ClickUp API Rate Limits

ClickUp, like most APIs, enforces rate limits to ensure fair usage and stability for all users. These limits restrict the number of requests an application can make within a certain time window.
-   **Per Token, Per Workspace:** Rate limits are typically applied per API token, per workspace.
-   **Dynamic Limits:** The exact limits can vary and may be adjusted by ClickUp.
-   **`X-RateLimit` Headers:** ClickUp API responses include headers like `X-RateLimit-Limit` (total requests allowed in the current window), `X-RateLimit-Remaining` (requests remaining), and `X-RateLimit-Reset` (time when the limit resets). The SDK does not currently use these proactively but reacts to 429 responses.
-   **HTTP 429 Response:** If you exceed the rate limit, the API will respond with an HTTP 429 "Too Many Requests" status code. This response may also include a `Retry-After` header indicating how long your application should wait before making another request.

## SDK Resilience with Polly

The `ClickUp.Api.Client` SDK uses Polly policies configured during dependency injection (`AddClickUpClient()`) to automatically handle common transient issues and rate limiting:

### 1. Retry Policy

-   **Handles Transient Errors:** Automatically retries requests that fail due to transient network issues (e.g., `HttpRequestException`) or specific HTTP status codes indicating temporary server problems (e.g., 5xx errors).
-   **Exponential Backoff:** Uses an exponential backoff strategy, waiting longer between retries to avoid overwhelming the server.
-   **Configurable:** The number of retries and the backoff parameters can be configured through `ClickUpPollyOptions`.

### 2. Circuit Breaker Policy

-   **Prevents Repeated Failures:** If an API endpoint consistently fails (e.g., returning many 5xx errors or timeouts), the circuit breaker will "open."
-   **Fail Fast:** While the circuit is open, requests to that endpoint will fail immediately without attempting to hit the network. This prevents your application from wasting resources on requests that are likely to fail and reduces load on the failing service.
-   **Automatic Recovery:** After a configured duration (break duration), the circuit moves to a "half-open" state, allowing a test request through. If it succeeds, the circuit closes and normal operation resumes. If it fails, the circuit opens again.
-   **Configurable:** The failure threshold, break duration, and other parameters are configurable via `ClickUpPollyOptions`.

### 3. HTTP 429 (Rate Limit) Handling

-   **Specific Policy for 429s:** The SDK includes a Polly policy specifically for handling HTTP 429 responses.
-   **Respects `Retry-After`:** If the API response includes a `Retry-After` header (indicating either a number of seconds or a specific date/time to retry), the SDK will honor this delay before retrying the request.
-   **Default Backoff:** If `Retry-After` is not present, it falls back to a configurable backoff strategy for 429s.
-   **Integration with Retry/Circuit Breaker:** This 429 handling is part of the overall retry strategy. Repeated 429s can still contribute to opening the circuit breaker if not resolved by retries.

## Configuring Resilience (`ClickUpPollyOptions`)

You can customize the behavior of these Polly policies by configuring `ClickUpPollyOptions` in your application's setup.

**`appsettings.json` Example:**
```json
{
  "ClickUpClient": {
    "PersonalAccessToken": "YOUR_TOKEN",
    // ...
  },
  "ClickUpPolly": { // Section for ClickUpPollyOptions
    "MaxRetryAttempts": 3,
    "InitialDelaySeconds": 1,
    "MaxDelaySeconds": 5, // For general retries, not specifically 429 with Retry-After
    "CircuitBreakerFailureThreshold": 0.5, // 50% failure rate
    "CircuitBreakerSamplingDurationSeconds": 60,
    "CircuitBreakerMinimumThroughput": 7,
    "CircuitBreakerBreakDurationSeconds": 30,
    "TimeoutSeconds": 100 // Overall request timeout
  }
}
```

**`Program.cs` / `Startup.cs`:**
```csharp
using ClickUp.Api.Client.Abstractions.Options;

// ...
builder.Services.Configure<ClickUpPollyOptions>(
    builder.Configuration.GetSection("ClickUpPolly"));
// ...
builder.Services.AddClickUpClient(); // This will use the configured ClickUpPollyOptions
```

**Available `ClickUpPollyOptions`:**
(Refer to `ClickUpPollyOptions.cs` for the definitive list and default values)
-   `MaxRetryAttempts`: Maximum number of times a request will be retried.
-   `InitialDelaySeconds`: Base delay for the first retry (exponential backoff).
-   `MaxDelaySeconds`: Maximum delay for retries.
-   `RetryHttpStatusCodesToConsider`: Array of `HttpStatusCode` values that should trigger a retry (e.g., 500, 502, 503, 504).
-   `CircuitBreakerFailureThreshold`: Percentage of failures (0.0 to 1.0) within the sampling duration that will open the circuit.
-   `CircuitBreakerSamplingDurationSeconds`: Duration over which failures are monitored for the threshold.
-   `CircuitBreakerMinimumThroughput`: Minimum number of requests within the sampling duration before the circuit breaker considers opening.
-   `CircuitBreakerBreakDurationSeconds`: Duration the circuit stays open before transitioning to half-open.
-   `TimeoutSeconds`: Per-request timeout. If a request (including retries) exceeds this, a `TimeoutRejectedException` is thrown.

## Logging

The Polly policies are configured to log events such as retries, circuit breaks, and resets using the standard `ILogger` infrastructure. Ensure your logging is configured to see these messages, which can be very helpful for diagnosing issues.

Example log messages:
-   `Retrying HTTP request... Delay: 00:00:02, Retry Attempt: 1/3, Endpoint: GET https://api.clickup.com/api/v2/user, Reason: InternalServerError`
-   `Circuit breaker opened for endpoint: GET https://api.clickup.com/api/v2/task/task_id. Breaking for 00:00:30.`
-   `Circuit breaker reset for endpoint: GET https://api.clickup.com/api/v2/task/task_id.`

## When Policies Are Exhausted

If all retries are exhausted, or if the circuit breaker is open and remains open after a half-open attempt, the original exception (e.g., `ClickUpApiRateLimitException`, `ClickUpApiServerException`, `HttpRequestException`, or `TimeoutRejectedException`) will be propagated to your application code. Your application should then handle this appropriately (e.g., log the error, inform the user, queue the operation for later).

By understanding and leveraging these built-in resilience features, you can build more robust and reliable integrations with the ClickUp API.
