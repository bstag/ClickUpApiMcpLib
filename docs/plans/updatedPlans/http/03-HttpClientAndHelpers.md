# Detailed Plan: HTTP Client and Helpers

This document details the plan for setting up and configuring `HttpClient`, implementing authentication handling, and creating necessary helper utilities for JSON serialization and query string construction.

**Source Documents:**
*   `docs/plans/04-httpclient-helpers-conceptual.md` (Initial conceptual plan)
*   `docs/plans/NEW_OVERALL_PLAN.md` (Phase 2, Step 5 & 7)
*   `docs/OpenApiSpec/ClickUp-6-17-25.json` (For API base URL, auth requirements)

**Location in Codebase:**
*   DI Setup: In the consuming application (e.g., example projects) or a dedicated DI extension method in `ClickUp.Api.Client`.
*   Handlers/Helpers: `src/ClickUp.Api.Client/Http/` (new folder) or `src/ClickUp.Api.Client/Helpers/`.

## 1. `IHttpClientFactory` and `HttpClient` Configuration

1.  **Registration Method:**
    *   Create an extension method for `IServiceCollection` (e.g., `AddClickUpApiClient`) in the `ClickUp.Api.Client` project. This method will encapsulate the DI setup for the SDK.
    *   This method will register a typed client (e.g., `ClickUpHttpClient`) or a named client ("ClickUpApiClient"). A typed client is generally preferred.

    ```csharp
    // In ClickUp.Api.Client project (e.g., ServiceCollectionExtensions.cs)
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClickUpApiClient(this IServiceCollection services, Action<ClickUpApiClientOptions> configureOptions)
        {
            // Configure options pattern for API key, base URL etc.
            services.Configure(configureOptions);
            var provider = services.BuildServiceProvider(); // Temporary provider to get options
            var options = provider.GetRequiredService<IOptions<ClickUpApiClientOptions>>().Value;

            services.AddHttpClient<ClickUpHttpClient>(client =>
            {
                client.BaseAddress = new Uri(options.BaseApiUrl ?? "https://api.clickup.com/api/v2/");
                client.DefaultRequestHeaders.Add("User-Agent", options.UserAgent ?? "DefaultClickUpSdkUserAgent/1.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<AuthenticationDelegatingHandler>()
            // Polly policies will be added here (see Resilience plan)
            ;

            // Register services that depend on ClickUpHttpClient
            // services.AddScoped<ITasksService, TasksService>(); // Example
            // ... other services

            services.AddScoped<AuthenticationDelegatingHandler>(); // Register handler
            services.AddSingleton<ClickUpJsonSerializerSettings>(); // For shared JsonSerializerOptions

            return services;
        }
    }

    // Options class
    public class ClickUpApiClientOptions
    {
        public string? PersonalApiKey { get; set; }
        public string? BaseApiUrl { get; set; }
        public string? UserAgent { get; set; }
    }

    // Typed HttpClient (can be simple wrapper or empty if handlers do all the work)
    public class ClickUpHttpClient
    {
        public HttpClient Client { get; }
        public ClickUpHttpClient(HttpClient client)
        {
            Client = client;
        }
    }
    ```

2.  **Base API Address:**
    *   Default: `https://api.clickup.com/api/v2/`
    *   Allow override via `ClickUpApiClientOptions`.

3.  **Default Headers:**
    *   `User-Agent`: Configurable via `ClickUpApiClientOptions`, with a sensible default.
    *   `Accept: application/json`: Added by default.

## 2. Authentication Handling (`AuthenticationDelegatingHandler`)

1.  **Create `AuthenticationDelegatingHandler.cs`:**
    *   Location: `src/ClickUp.Api.Client/Http/`
    *   Inherit from `System.Net.Http.DelegatingHandler`.
    *   Inject `IOptions<ClickUpApiClientOptions>` to access the Personal API Token.
    *   Override `SendAsync`:
        *   Before calling `base.SendAsync`, check if `options.Value.PersonalApiKey` is available.
        *   If available, add/update the `Authorization` header: `request.Headers.Authorization = new AuthenticationHeaderValue(options.Value.PersonalApiKey);` (Note: The scheme "Bearer" is usually for OAuth tokens. For ClickUp personal tokens, it's just the token itself, e.g., "pk_xxxxxx". The `AuthenticationHeaderValue` constructor might need adjustment or just use `request.Headers.TryAddWithoutValidation("Authorization", options.Value.PersonalApiKey);` if it's a simple token without a scheme).
            *   **Correction based on ClickUp Spec:** Personal API token is typically prefixed (e.g. `pk_...`) and sent directly as the `Authorization` header value.
            *   Corrected usage: `request.Headers.Authorization = new AuthenticationHeaderValue("pk_your_token");` - No, the scheme is *not* "Bearer". The ClickUp documentation states: "Add token to requests with header: `Authorization: pk_...`". So, it is `request.Headers.TryAddWithoutValidation("Authorization", options.Value.PersonalApiKey);` or ensure `AuthenticationHeaderValue` handles scheme-less tokens correctly if one exists (it usually expects a scheme). Simplest is `request.Headers.Add("Authorization", options.Value.PersonalApiKey);`
        *   If no API key is provided and the request requires authentication (most will), the API will return a 401. The handler could optionally throw an exception here if an API key is expected but missing for most calls.

    ```csharp
    // src/ClickUp.Api.Client/Http/AuthenticationDelegatingHandler.cs
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IOptions<ClickUpApiClientOptions> _apiOptions;

        public AuthenticationDelegatingHandler(IOptions<ClickUpApiClientOptions> apiOptions)
        {
            _apiOptions = apiOptions;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(_apiOptions.Value.PersonalApiKey))
            {
                // Ensure no existing Authorization header is duplicated
                request.Headers.Remove("Authorization");
                request.Headers.Add("Authorization", _apiOptions.Value.PersonalApiKey);
            }
            // else: proceed without Authorization header; API will reject if needed.

            return await base.SendAsync(request, cancellationToken);
        }
    }
    ```

2.  **OAuth 2.0 Support (Conceptual Design for now):**
    *   OAuth 2.0 token management is complex and typically handled by the consuming application.
    *   The SDK will **not** implement the full OAuth flow (redirects, token exchange).
    *   Instead, the SDK will allow the consumer to provide a valid OAuth access token.
    *   The `AuthenticationDelegatingHandler` can be adapted or a new handler created:
        *   If an OAuth token is provided in `ClickUpApiClientOptions` (e.g., `options.OAuthToken`), it takes precedence over the Personal API Key.
        *   The header would be `request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.OAuthToken);`.
    *   This document will note that for OAuth, the consuming application is responsible for acquiring and refreshing the token and configuring the SDK with the active token.

## 3. JSON Serialization/Deserialization Helpers

1.  **Create `ClickUpJsonSerializerSettings.cs` (or similar):**
    *   Location: `src/ClickUp.Api.Client/Helpers/` or `src/ClickUp.Api.Client/Http/`
    *   This class will provide a static or singleton instance of `JsonSerializerOptions`.

    ```csharp
    // src/ClickUp.Api.Client/Helpers/ClickUpJsonSerializerSettings.cs
    public class ClickUpJsonSerializerSettings
    {
        public JsonSerializerOptions Options { get; }

        public ClickUpJsonSerializerSettings() // Could be singleton registered in DI
        {
            Options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, // ClickUp API typically uses snake_case
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } // Or SnakeCaseLower depending on API
            };
            // Add any other custom converters needed after reviewing API spec details
        }
    }
    ```
2.  **Configuration Details for `JsonSerializerOptions`:**
    *   `PropertyNamingPolicy`: Set to `JsonNamingPolicy.SnakeCaseLower` as this is common for APIs like ClickUp. This means C# PascalCase properties will map to json_snake_case. Models should still use `[JsonPropertyName]` for clarity or if specific properties deviate.
    *   `DefaultIgnoreCondition`: `JsonIgnoreCondition.WhenWritingNull` is a good default to avoid sending null values in request bodies unless explicitly required.
    *   `Converters`:
        *   `JsonStringEnumConverter`: Essential for serializing C# enums as their string representations. The naming policy for the enum converter (e.g., `JsonNamingPolicy.CamelCase` or `JsonNamingPolicy.SnakeCaseLower`) should match how the API expects enum string values.
        *   Custom Date/Time Converters: If the API uses non-standard date/time formats not handled by default `DateTimeOffset` parsing (e.g., Unix timestamps as strings/numbers where `DateTimeOffsetConverter` might not cover all cases), custom converters will be needed. ClickUp API seems to use Unix timestamps (milliseconds) as numbers for many date fields, which `System.Text.Json` can often handle for `DateTimeOffset` or `long` with appropriate model property types. This needs verification against actual API usage.

3.  **Usage:**
    *   The `System.Net.Http.Json` extension methods (`ReadFromJsonAsync`, `PostAsJsonAsync`, etc.) can accept `JsonSerializerOptions`. If the `HttpClient` is obtained via `IHttpClientFactory`, these options can sometimes be configured globally for the client, or passed explicitly.
    *   Services will use these shared options when manually serializing/deserializing if not using the built-in extensions or if custom logic is wrapped around it.

## 4. Query String Builder Utility

1.  **Need Assessment:**
    *   Review API endpoints in `ClickUp-6-17-25.json` that take multiple optional query parameters (especially for GET requests like `GetTasks`).
    *   If many such endpoints exist, a utility can simplify query string construction and ensure correct encoding.

2.  **Implementation (`QueryStringBuilder.cs` or similar):**
    *   Location: `src/ClickUp.Api.Client/Helpers/`
    *   Method: `public static string BuildQueryString(Dictionary<string, string?> parameters)`
        *   Iterate through the dictionary.
        *   Skip entries where the value is null or empty.
        *   URL-encode both keys and values using `HttpUtility.UrlEncode` (if `System.Web` is acceptable) or `Uri.EscapeDataString`.
        *   Join key-value pairs with `&`.
    *   Alternative: Use `Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString`. This is a good option if a dependency on this small package is acceptable. It handles encoding correctly.
        *   Example usage:
            ```csharp
            var queryParams = new Dictionary<string, string?>();
            if (param1 != null) queryParams["param_1_name"] = param1.ToString();
            if (!string.IsNullOrEmpty(param2)) queryParams["param_2_name"] = param2;
            // ...
            string queryString = QueryHelpers.AddQueryString(baseUrl, queryParams);
            ```

## 5. Plan Output

*   This document `03-HttpClientAndHelpers.md` will contain the finalized plan for these components.
*   It will specify the exact class names, method signatures for helpers, and configuration approaches for `HttpClient` and JSON settings.
*   It will detail the `AuthenticationDelegatingHandler` logic for Personal API Tokens and the conceptual approach for OAuth token integration.
```
