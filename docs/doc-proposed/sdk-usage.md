# SDK Usage

This document provides a guide on how to use the ClickUp API Client SDK in your .NET applications. It covers installation, configuration, and basic usage patterns.

## Installation

First, you need to add the SDK to your project. You can do this using the .NET CLI:

```bash
dotnet add package ClickUp.Api.Client
```

## Configuration

The SDK is designed to be used with the `Microsoft.Extensions.DependencyInjection` framework. You can configure the `ClickUpApiClient` in your `Program.cs` or `Startup.cs` file.

### Using `appsettings.json`

It's recommended to store your ClickUp API token and other settings in `appsettings.json`:

```json
{
  "ClickUpApiOptions": {
    "PersonalAccessToken": "YOUR_PERSONAL_ACCESS_TOKEN"
  }
}
```

### Dependency Injection

In your application's startup code, use the `AddClickUpClient` extension method to register the SDK services:

```csharp
using Microsoft.Extensions.DependencyInjection;
using ClickUp.Api.Client.Extensions;

public void ConfigureServices(IServiceCollection services)
{
    // ... other services

    services.AddClickUpClient(options =>
    {
        Configuration.GetSection("ClickUpApiOptions").Bind(options);
    });
}
```

## Basic Usage

Once the SDK is configured, you can inject any of the service interfaces into your classes and start making API calls.

### Example: Fetching Tasks

Here's an example of how to fetch tasks from a list:

```csharp
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Tasks;

public class MyService
{
    private readonly ITasksService _tasksService;

    public MyService(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }

    public async Task<List<CuTask>> GetTasksAsync(string listId)
    {
        var response = await _tasksService.GetTasksAsync(listId, new GetTasksRequest());
        return response.Tasks;
    }
}
```

### Example: Creating a Task

Here's how you can create a new task:

```csharp
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Tasks;

public class MyTaskCreator
{
    private readonly ITasksService _tasksService;

    public MyTaskCreator(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }

    public async Task<CuTask> CreateNewTaskAsync(string listId, string taskName)
    {
        var request = new CreateTaskRequest(Name: taskName);
        var createdTask = await _tasksService.CreateTaskAsync(listId, request);
        return createdTask;
    }
}
```

## Error Handling

The SDK uses custom exceptions to indicate problems with API requests. You should wrap your API calls in a `try-catch` block to handle these exceptions gracefully.

```csharp
try
{
    // Make API call
}
catch (ClickUpApiException ex)
{
    // Handle API error
    Console.WriteLine($"API Error: {ex.Message}");
}
```