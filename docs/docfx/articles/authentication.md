# Authentication

The `ClickUp.Api.Client` SDK supports two primary methods for authenticating with the ClickUp API: Personal Access Tokens and OAuth 2.0 Access Tokens.

## Configuration

Authentication is configured via `ClickUpClientOptions` when you set up the SDK services.

```csharp
// In Program.cs or Startup.cs
builder.Services.Configure<ClickUpClientOptions>(
    builder.Configuration.GetSection("ClickUpClient"));

builder.Services.AddClickUpClient();
```

The options can be populated from `appsettings.json` or any other .NET configuration provider.

### `ClickUpClientOptions`

The relevant properties in `ClickUpClientOptions` are:

-   `PersonalAccessToken` (string): Your ClickUp Personal Access Token.
-   `OAuthAccessToken` (string): A pre-acquired OAuth 2.0 Access Token.

The SDK will use `OAuthAccessToken` if provided; otherwise, it will fall back to `PersonalAccessToken`. If neither is provided, API calls requiring authentication will fail.

## 1. Personal Access Token (PAT)

A Personal Access Token is the simplest way to authenticate for individual use or for server-to-server integrations where you manage the token directly.

**How to get a PAT:**
1.  Go to your ClickUp settings (click your avatar in the bottom left).
2.  Navigate to "My Settings" > "Apps".
3.  Generate an API token.

**Configuration (`appsettings.json`):**
```json
{
  "ClickUpClient": {
    "PersonalAccessToken": "pk_YOUR_PERSONAL_ACCESS_TOKEN_HERE"
  }
}
```

Ensure this token is kept secure and is not exposed in client-side applications or public repositories.

## 2. OAuth 2.0 Access Token

OAuth 2.0 is a more secure and flexible authentication method, especially for applications used by multiple users, as it allows users to grant your application access to their ClickUp data without sharing their personal credentials.

**Important Note:** This SDK **does not handle the OAuth 2.0 authorization flow** (i.e., redirecting users to ClickUp, handling callbacks, exchanging authorization codes for tokens, or refreshing tokens). You are responsible for implementing the OAuth 2.0 flow in your application to obtain an Access Token.

Once you have obtained an OAuth 2.0 Access Token for a user, you can configure the SDK to use it.

**Configuration (`appsettings.json`):**
```json
{
  "ClickUpClient": {
    "OAuthAccessToken": "cu_YOUR_OAUTH_ACCESS_TOKEN_HERE"
    // "PersonalAccessToken": "..." // Can be omitted or empty if using OAuth
  }
}
```

**Dynamic Token Configuration:**
For applications serving multiple users, you'll likely configure the `OAuthAccessToken` dynamically per request or per user session, rather than from a static `appsettings.json`. The SDK's `AuthenticationDelegatingHandler` uses `IOptionsSnapshot<ClickUpClientOptions>` by default, which means if you can update the options for a given scope (like a web request), the handler should pick up the correct token. For more complex scenarios, you might need to customize the DI setup or the handler.

## Security Considerations

-   **Token Storage:** Always store tokens securely. Avoid hardcoding them directly in your source code. Use environment variables, .NET Secret Manager for development, or a secure vault service (like Azure Key Vault, HashiCorp Vault) for production.
-   **Least Privilege:** When using OAuth 2.0, request only the scopes necessary for your application's functionality.
-   **Token Expiration and Refresh (OAuth 2.0):** Remember that OAuth 2.0 access tokens expire. Your application needs to handle token refresh using the refresh token obtained during the OAuth flow. The SDK itself does not manage token refresh.

## Verifying Authentication

You can verify that your authentication is set up correctly by making a call to an authenticated endpoint, such as fetching the authorized user:

```csharp
using ClickUp.Api.Client.Abstractions.Services;

// ... inject IAuthorizationService ...

try
{
    var userResponse = await _authorizationService.GetAuthorizedUserAsync();
    Console.WriteLine($"Authenticated as: {userResponse.User.Username}");
}
catch (ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException authEx)
{
    Console.WriteLine($"Authentication failed: {authEx.Message}");
}
catch (ClickUp.Api.Client.Models.Exceptions.ClickUpApiException apiEx)
{
    Console.WriteLine($"API error: {apiEx.Message}");
}
```

If authentication fails, a `ClickUpApiAuthenticationException` (or a more general `ClickUpApiException` with a 401/403 status) will typically be thrown.
