# Phase 1, Step 4: Design HttpClient Configuration and Helpers (Conceptual)

This document outlines the conceptual approach for configuring and managing `HttpClient` instances, along with helper utilities for JSON processing and other common tasks supporting the service implementations. These components are crucial for a robust and maintainable API client library.

## `HttpClient` Management and Configuration

- **`IHttpClientFactory`:** The primary mechanism for creating and managing `HttpClient` instances will be `IHttpClientFactory`. This approach offers several benefits:
    - **Manages `HttpClient` lifetime:** Prevents issues related to `HttpClient` instantiation and disposal (e.g., socket exhaustion).
    - **Centralized Configuration:** Allows for named or typed client configurations.
    - **Integration with Polly:** Simplifies adding resilience and transient fault handling policies (retries, circuit breakers).
    - **Dependency Injection:** Integrates seamlessly with DI containers.

- **Named or Typed Clients:**
    - A **typed client** (e.g., `ClickUpHttpClient`) will be registered in the DI container. This typed client can encapsulate `HttpClient` and provide a convenient way to inject a pre-configured client into services.
    - Alternatively, a **named client** (e.g., "ClickUpApiClient") could be configured, and services would request this named client via `IHttpClientFactory.CreateClient("ClickUpApiClient")`. Typed clients are generally preferred for better type safety and ease of use.

- **Configuration in DI Startup:**
    - In the main application setup (e.g., `Startup.cs` or `Program.cs` in a .NET Core application using the library), `HttpClient` will be configured:
        ```csharp
        // Example for Program.cs with .NET 6+
        builder.Services.AddHttpClient<IClickUpAuthenticator, ClickUpAuthenticator>(); // For auth token management
        builder.Services.AddHttpClient<ITaskService, TaskService>((serviceProvider, client) =>
        {
            client.BaseAddress = new Uri("https_//api.clickup.com/api/v2/"); // Base URL from config
            client.DefaultRequestHeaders.Add("User-Agent", "ClickUp.Net Client Library"); // Example User-Agent

            // Potentially add an HttpMessageHandler for authentication here,
            // or configure it on a more global ClickUpHttpClient.
            // var authenticator = serviceProvider.GetRequiredService<IClickUpAuthenticator>();
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticator.GetToken());
        })
        // .AddHttpMessageHandler<AuthenticationDelegatingHandler>() // For adding auth token
        .AddTransientHttpErrorPolicy(policyBuilder =>
            policyBuilder.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
        ); // Example retry policy
        // ... register other services (ICommentService, etc.) similarly
        ```
    - **Note:** A more robust approach for authentication would involve a `DelegatingHandler` (see below).

- **Base URL:** The ClickUp API base URL (e.g., `https://api.clickup.com/api/v2/`) will be configured on the `HttpClient`. This will be sourced from a configuration file (e.g., `appsettings.json`).

- **Default Headers:**
    - **`User-Agent`:** A default `User-Agent` header should be set for all requests.
    - **`Authorization`:** For API key authentication, an `Authorization` header (e.g., `Authorization: <API_KEY>`) needs to be added to every request. This is best handled using a `DelegatingHandler` (see below) to avoid manual addition in every service call and to allow dynamic token retrieval/refresh if needed.
    - **`Accept`:** `Accept: application/json` will be standard. `HttpClient` often sets this by default for JSON-related extension methods.

- **`DelegatingHandler` for Authentication:**
    - A custom `DelegatingHandler` (e.g., `AuthenticationDelegatingHandler`) will be created to manage the `Authorization` header.
    - This handler will:
        - Retrieve the API key from a secure configuration source (e.g., options pattern, environment variable).
        - Add the `Authorization` header to each outgoing request.
        - This decouples authentication logic from the service implementations and `HttpClient` configuration.
    - This handler will be added to the `HttpClient` pipeline using `.AddHttpMessageHandler<AuthenticationDelegatingHandler>()`.

## JSON Serialization/Deserialization Helpers

- **`System.Text.Json` as Standard:** The library will standardize on `System.Text.Json` for all JSON operations due to its performance and integration with modern .NET.
- **Centralized `JsonSerializerOptions`:**
    - A static class or a singleton service will provide a pre-configured instance of `JsonSerializerOptions`.
    - Configuration will include:
        - `PropertyNamingPolicy`: To match the API's naming convention (e.g., `JsonNamingPolicy.SnakeCaseLower` if ClickUp uses snake_case, or rely on `[JsonPropertyName]` attributes on models).
        - `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull` (usually a good default).
        - `Converters`: Registration of any custom converters needed (e.g., `JsonStringEnumConverter`, custom date/time converters if the API format is non-standard).
    - Example:
        ```csharp
        // Shared/Helpers project
        public static class ClickUpJsonSerializerOptions
        {
            public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, // Example
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } // Example
            };
        }
        ```
- **Extension Methods for `HttpContent` and `HttpResponseMessage`:**
    - While `System.Net.Http.Json` provides `ReadFromJsonAsync` and `PostAsJsonAsync`, etc., we might need custom wrappers if the globally configured `JsonSerializerOptions` are not easily passed or if additional logic before/after serialization/deserialization is needed.
    - For example, a helper to deserialize an error response into a specific error model.
    ```csharp
    // Conceptual helper
    public static class HttpContentExtensions
    {
        public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            options ??= ClickUpJsonSerializerOptions.Options;
            return await JsonSerializer.DeserializeAsync<T>(
                await content.ReadAsStreamAsync(cancellationToken), options, cancellationToken);
        }
    }
    ```

## Other Utility Functions

- **Query Parameter Builder:**
    - A utility to construct query strings robustly, handling URL encoding and optional parameters.
    - Example signature: `string BuildQueryString(Dictionary<string, string?> parameters)`
    - This helper would iterate through parameters, skip null/empty ones, URL-encode keys and values, and join them.
    - While `HttpClient` can often take query params in `GetAsync` or via `HttpRequestMessage`, a dedicated builder can be useful for complex scenarios or logging. `System.Web.HttpUtility.ParseQueryString()` and `ToString()` can be a basis, but it's part of `System.Web` which might not be desirable in a netstandard library without careful consideration of dependencies. A lightweight custom builder is often preferred.
- **URL Path Combiner:**
    - While `HttpClient.BaseAddress` and relative URLs usually handle this, a helper to safely combine URL segments (e.g., ensuring correct slash handling) can sometimes be useful, though often not strictly necessary. `Path.Combine` is for file paths, not URLs. `new Uri(baseUri, relativeUri)` is the standard way.
- **Request Body Content Factory:**
    - Helper to create `HttpContent` for request bodies, particularly `StringContent` for JSON, ensuring correct `ContentType` header (`application/json`).
    - Example:
    ```csharp
    public static class JsonContent
    {
        public static StringContent Create<T>(T value, JsonSerializerOptions? options = null)
        {
            options ??= ClickUpJsonSerializerOptions.Options;
            return new StringContent(JsonSerializer.Serialize(value, options), System.Text.Encoding.UTF8, "application/json");
        }
    }
    // Usage: _httpClient.PostAsync(url, JsonContent.Create(requestModel));
    ```
    (Note: `HttpClientJsonExtensions.PostAsJsonAsync` already does this.)

## Error Handling Helpers (Conceptual Link)

- As mentioned in "Service Implementations (Conceptual)", helper methods will exist to process `HttpResponseMessage` objects, check for non-success status codes, and attempt to deserialize API error responses into a structured error model, then throw the appropriate custom exception. These helpers would utilize the JSON deserialization utilities.

## Location of Helpers

- These helpers and configurations would typically reside in a shared infrastructure or core component within the `Client` project, or if extensive, in a dedicated `ClickUp.Net.Core` or `ClickUp.Net.Infrastructure` project. For this project structure, they'd likely be internal classes within the `Client` project.

## Next Steps

- Define the global exception system and custom exception types.
- Begin implementing the `DelegatingHandler` for authentication.
- Set up the `IHttpClientFactory` configuration in the example projects (Console, Worker) once those are being developed.
- Implement the specific `JsonSerializerOptions` based on detailed review of the ClickUp API's JSON format.
- Develop utility functions as the need arises during service implementation.
```
