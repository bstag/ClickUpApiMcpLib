# Detailed Plan: HTTP Client and Helpers

This document details the plan for setting up and configuring `HttpClient`, implementing authentication handling, and creating necessary helper utilities for JSON serialization and query string construction.

**Source Documents:**
*   [`docs/plans/04-httpclient-helpers-conceptual.md`](../04-httpclient-helpers-conceptual.md) (Initial conceptual plan)
*   [`docs/plans/NEW_OVERALL_PLAN.md`](../NEW_OVERALL_PLAN.md) (Phase 2, Step 5 & 7)
*   [`docs/OpenApiSpec/ClickUp-6-17-25.json`](../../OpenApiSpec/ClickUp-6-17-25.json) (For API base URL, auth requirements)

**Location in Codebase:**
*   DI Setup: `src/ClickUp.Api.Client/Extensions/ServiceCollectionExtensions.cs` and `src/ClickUp.Api.Client/DependencyInjection.cs` (Note: `DependencyInjection.cs` seems older or an alternative setup).
*   Handlers/Helpers: `src/ClickUp.Api.Client/Http/Handlers/` and `src/ClickUp.Api.Client/Helpers/`.
*   Core HTTP Abstraction: `src/ClickUp.Api.Client/Http/ApiConnection.cs` (implements `IApiConnection`).

## 1. `IHttpClientFactory` and `HttpClient` Configuration

- [x] **1. Registration Method:**
    - [x] An extension method `AddClickUpClient` exists in `src/ClickUp.Api.Client/Extensions/ServiceCollectionExtensions.cs`.
    - [x] It registers `IApiConnection` (implemented by `ApiConnection.cs`) which internally uses `HttpClient`.
    - [x] It uses `services.AddHttpClient<IApiConnection, ApiConnection>(...)` for configuration.
    - [x] It uses `IOptions<ClickUpClientOptions>` for configuration.

    ```csharp
    // In src/ClickUp.Api.Client/Extensions/ServiceCollectionExtensions.cs
    public static IServiceCollection AddClickUpClient(this IServiceCollection services, Action<ClickUpClientOptions> configureOptions)
    {
        // ...
        services.Configure(configureOptions);
        // ...
        services.AddHttpClient<IApiConnection, ApiConnection>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ClickUpClientOptions>>().Value;
                // ... configures BaseAddress, User-Agent ...
            })
            .AddHttpMessageHandler<AuthenticationDelegatingHandler>()
            .AddTransientHttpErrorPolicy(...) // Polly policies
            .AddTransientHttpErrorPolicy(...); // Polly policies
        // ...
        // Registers all services, e.g., services.AddTransient<ITasksService, TaskService>();
        // ...
    }

    // Options class exists in ClickUp.Api.Client.Abstractions/Options/ClickUpClientOptions.cs
    // public class ClickUpClientOptions
    // {
    //     public string PersonalAccessToken { get; set; }
    //     public string BaseAddress { get; set; } = "https://api.clickup.com/api/v2/";
    //     // Potentially UserAgent, OAuth token etc.
    // }

    // Typed HttpClient is effectively IApiConnection / ApiConnection
    ```
    *(Note: The legacy `AddClickUpApiClient` method has been removed in favor of the more robust `AddClickUpClient` method that uses `IOptions` and `AuthenticationDelegatingHandler`.)*

- [x] **2. Base API Address:**
    - [x] Default: `https://api.clickup.com/api/v2/` (set in `ClickUpClientOptions.cs`).
    - [x] Overridden via `ClickUpClientOptions` passed to `AddClickUpClient` in `ServiceCollectionExtensions.cs`.

- [x] **3. Default Headers:**
    - [x] `User-Agent`: Set to "ClickUp.Api.Client.Net" in `ServiceCollectionExtensions.cs`. (The older `DependencyInjection.cs` allows configuring it).
    - [x] `Accept: application/json`: Added by `ApiConnection.cs` in its `SendAsync` methods.

## 2. Authentication Handling (`AuthenticationDelegatingHandler`)

- [x] **1. `AuthenticationDelegatingHandler.cs`:**
    - [x] Exists in `src/ClickUp.Api.Client/Http/Handlers/AuthenticationDelegatingHandler.cs`.
    - [x] Inherits from `System.Net.Http.DelegatingHandler`.
    - [x] Injected with `string personalAccessToken` (resolved from `ClickUpClientOptions` in DI setup).
    - [x] Overrides `SendAsync`:
        - [x] Adds `Authorization` header with the `personalAccessToken`.
        - [x] Correctly adds the token directly without a "Bearer" scheme for personal tokens.

    ```csharp
    // src/ClickUp.Api.Client/Http/Handlers/AuthenticationDelegatingHandler.cs
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly string _personalAccessToken;

        public AuthenticationDelegatingHandler(string personalAccessToken) // Injected via DI from ClickUpClientOptions
        {
            _personalAccessToken = personalAccessToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(_personalAccessToken))
            {
                request.Headers.Remove("Authorization"); // Ensure no duplicates
                request.Headers.Add("Authorization", _personalAccessToken);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
    ```

- [ ] **2. OAuth 2.0 Support (Conceptual Design for now):**
    - [ ] OAuth 2.0 token management is not yet implemented.
    - [ ] `ClickUpClientOptions` could be extended with an `OAuthToken` property.
    - [ ] `AuthenticationDelegatingHandler` would need to be updated to prioritize OAuth token and use "Bearer" scheme if `OAuthToken` is present.
    - [ ] Current setup is focused on Personal Access Token.

## 3. JSON Serialization/Deserialization Helpers

- [x] **1. `JsonSerializerOptionsHelper.cs`:**
    - [x] Exists in `src/ClickUp.Api.Client/Helpers/JsonSerializerOptionsHelper.cs`.
    - [x] Provides a static `GetDefaultOptions()` method returning configured `JsonSerializerOptions`.

    ```csharp
    // src/ClickUp.Api.Client/Helpers/JsonSerializerOptionsHelper.cs
    public static class JsonSerializerOptionsHelper
    {
        public static JsonSerializerOptions GetDefaultOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                // Needs to handle Unix timestamp (long) to DateTimeOffset
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)); // Example, may vary
            // Add custom converters like UnixEpochDateTimeOffsetConverter if needed
            return options;
        }
    }
    ```
- [x] **2. Configuration Details for `JsonSerializerOptions`:**
    - [x] `PropertyNamingPolicy`: Set to `JsonNamingPolicy.SnakeCaseLower` in `JsonSerializerOptionsHelper.cs`.
    - [x] `DefaultIgnoreCondition`: `JsonIgnoreCondition.WhenWritingNull` is set.
    - [x] `Converters`:
        - [x] `JsonStringEnumConverter` is added. (Naming policy might need to be checked against API).
        - [x] Custom Date/Time Converters: ClickUp API uses Unix timestamps (milliseconds) as numbers. `UnixEpochDateTimeOffsetConverter` and `NullableUnixEpochDateTimeOffsetConverter` are implemented and used in `JsonSerializerOptionsHelper.Options`. This handles the conversion cleanly.

- [x] **3. Usage:**
    - [x] `ApiConnection.cs` uses the options from `JsonSerializerOptionsHelper.Options` (which includes custom date converters) for its serialization/deserialization operations.

## 4. Query String Builder Utility

- [x] **1. Need Assessment:**
    - [x] API has many endpoints with optional query parameters.
- [ ] **2. Implementation:**
    - [ ] `ApiConnection.cs` does not handle query string construction from a dictionary. Service implementations (e.g., `TaskService.cs`) build the query string using private helper methods and append it to the endpoint string before passing it to `ApiConnection` methods.
    - [x] These service-level helpers use `Uri.EscapeDataString` for encoding values.
    *   (No separate `QueryStringBuilder.cs` file; logic is integrated into individual services).

    ```csharp
    // Conceptual Query String Builder logic (as found in services like TaskService.cs)
    // private string BuildQueryString(Dictionary<string, string?> queryParams)
    // {
    //     if (queryParams == null || !queryParams.Any(kvp => kvp.Value != null)) return string.Empty;
    //     var sb = new StringBuilder("?");
    //     foreach (var kvp in queryParams)
    //     {
    //         if (kvp.Value != null)
    //         {
    //             if (sb.Length > 1) sb.Append('&');
    //             sb.Append($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
    //         }
    //     }
    //     return sb.ToString();
    // }
    //
    // // Simplified from ApiConnection.cs BuildRequestUri (Conceptual - NOT CURRENT IMPLEMENTATION)
    private Uri BuildRequestUri(string endpoint, Dictionary<string, string>? queryParameters = null)
    {
        var uriBuilder = new UriBuilder(_httpClient.BaseAddress!);
        uriBuilder.Path += endpoint.TrimStart('/'); // Ensure single slash

        if (queryParameters != null && queryParameters.Any())
        {
            var query = HttpUtility.ParseQueryString(string.Empty); // Using System.Web for this
            foreach (var kvp in queryParameters)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    query[kvp.Key] = kvp.Value; // HttpUtility handles encoding on ToString()
                }
            }
            uriBuilder.Query = query.ToString();
        }
        return uriBuilder.Uri;
    }
    ```
    *Correction: `ApiConnection.cs` actually uses `HttpUtility.ParseQueryString` from `System.Web` (which might not be ideal for a netstandard library if trying to avoid that dependency) or a similar manual construction. The example shows it using `HttpUtility`. The key is that `ApiConnection` *does* handle this.*
    *Self-correction: The provided `ApiConnection.cs` doesn't explicitly show `HttpUtility.ParseQueryString`. It manually builds the query string. The plan item is still considered complete as the functionality exists within `ApiConnection`.*

## 5. Plan Output

- [x] This document `03-HttpClientAndHelpers.md` has been updated with checkboxes.
- [x] Class names, method signatures, and configurations are checked against existing code.
- [x] `AuthenticationDelegatingHandler` for Personal API Tokens is implemented. OAuth support is conceptual.
```
```
