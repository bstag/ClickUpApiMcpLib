# Getting Started

This guide will walk you through installing the `ClickUp.Api.Client` SDK, setting it up in your .NET project, and making your first API call.

## 1. Installation

The SDK is distributed as a NuGet package. You can install it using your preferred method:

**.NET CLI:**
```bash
dotnet add package ClickUp.Api.Client
```

**Package Manager Console (Visual Studio):**
```powershell
Install-Package ClickUp.Api.Client
```

**PackageReference in `.csproj` file:**
```xml
<ItemGroup>
  <PackageReference Include="ClickUp.Api.Client" Version="LATEST_VERSION" /> <!-- Replace LATEST_VERSION with the actual version -->
</ItemGroup>
```

## 2. Configuration

The SDK is designed to be used with .NET's dependency injection system and `IHttpClientFactory`.

**Using `IServiceCollection` (e.g., in `Program.cs` or `Startup.cs`):**

```csharp
using ClickUp.Api.Client.Extensions; // For AddClickUpClient
using ClickUp.Api.Client.Abstractions.Options; // For ClickUpClientOptions

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args); // Or Host.CreateDefaultBuilder for console/worker apps

        // Configure ClickUpClientOptions - typically from appsettings.json
        builder.Services.Configure<ClickUpClientOptions>(
            builder.Configuration.GetSection("ClickUpClient"));

        // Add the ClickUpClient services
        builder.Services.AddClickUpClient();

        // ... other services

        var app = builder.Build();

        // ... use your services

        app.Run(); // Or host.Run()
    }
}
```

**Example `appsettings.json` for Personal Access Token:**
```json
{
  "ClickUpClient": {
    "PersonalAccessToken": "YOUR_PERSONAL_ACCESS_TOKEN"
    // Or for OAuth 2.0:
    // "OAuthAccessToken": "YOUR_OAUTH_ACCESS_TOKEN"
  },
  "Logging": {
    // ...
  }
}
```
Refer to the [Authentication](authentication.md) guide for more details on token types.

## 3. Making Your First API Call

Once configured, you can inject and use the ClickUp services in your classes.

**Example: Fetching User Information**

Let's say you have a service where you want to fetch the authorized user's details:

```csharp
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels;
using Microsoft.Extensions.Logging;

public class MyUserService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<MyUserService> _logger;

    public MyUserService(IAuthorizationService authorizationService, ILogger<MyUserService> logger)
    {
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<UserResponse> GetMyUserAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var userResponse = await _authorizationService.GetAuthorizedUserAsync(cancellationToken);
            _logger.LogInformation("Successfully fetched user: {UserName}", userResponse.User.Username);
            return userResponse.User;
        }
        catch (ClickUp.Api.Client.Models.Exceptions.ClickUpApiException ex)
        {
            _logger.LogError(ex, "API Error fetching user: {ErrorMessage}", ex.Message);
            // Handle specific exceptions (e.g., ClickUpApiAuthenticationException) as needed
            throw;
        }
    }
}
```

**To use `MyUserService` (assuming it's also registered in DI):**

```csharp
// In another service or controller
public class AnotherService
{
    private readonly MyUserService _myUserService;

    public AnotherService(MyUserService myUserService)
    {
        _myUserService = myUserService;
    }

    public async Task DoSomethingWithUser()
    {
        var user = await _myUserService.GetMyUserAsync();
        if (user != null)
        {
            Console.WriteLine($"User ID: {user.Id}, Email: {user.Email}");
        }
    }
}
```

## What's Next?

-   Explore the **[API Reference](../api/index.md)** to discover all available services and methods.
-   Read through other conceptual articles to understand features like [Authentication](authentication.md), [Error Handling](error-handling.md), and [Pagination](pagination.md).
