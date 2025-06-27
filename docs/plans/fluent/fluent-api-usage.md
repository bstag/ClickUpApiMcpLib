# Using the Fluent API

This document provides an overview and examples of how to use the new fluent API for interacting with the ClickUp API.

## Introduction

The fluent API provides a more intuitive and readable way to make requests to the ClickUp API. It complements the existing service-based approach and is the recommended way for new projects to interact with this library.

## Getting Started

To get started, you first need to create an instance of the `ClickUpClient`. This is the main entry point for the fluent API.

```csharp
using ClickUp.Api.Client.Fluent;
using Microsoft.Extensions.Logging.Abstractions; // For a null logger

// You'll need a logger factory. For simple cases, you can use NullLoggerFactory.Instance.
var loggerFactory = NullLoggerFactory.Instance;

// Create a client with your API token
var client = ClickUpClient.Create("YOUR_API_TOKEN", loggerFactory);
```

## Basic Usage

Once you have a `ClickUpClient` instance, you can access the different API services through its properties. For example, to get information about workspaces, you can use the `Workspaces` property:

```csharp
// Get all workspaces
var workspaces = await client.Workspaces.GetAsync();
```

## Method Chaining and Parameter Objects

For more complex requests that involve multiple parameters, the fluent API uses method chaining and parameter objects to improve readability.

### Example: Getting Tasks with Filters

Here's how you can get a list of tasks from a specific list and apply various filters:

```csharp
var tasksResponse = await client.Tasks.Get("YOUR_LIST_ID")
    .WithArchived(false)
    .WithPage(0)
    .WithOrderBy("created")
    .WithReverse(true)
    .WithSubtasks(true)
    .WithStatuses(new[] { "open", "in progress" })
    .GetAsync();

foreach (var task in tasksResponse.Tasks)
{
    Console.WriteLine($"Task Name: {task.Name}");
}
```

### Example: Getting Filtered Tasks for a Workspace

Similarly, you can get filtered tasks for an entire workspace:

```csharp
var teamTasksResponse = await client.Tasks.GetFilteredTeamTasksAsync("YOUR_WORKSPACE_ID", request =>
    request.WithStatuses(new[] { "in progress" })
           .WithAssignees(new[] { "USER_ID_1", "USER_ID_2" })
);

foreach (var task in teamTasksResponse.Tasks)
{
    Console.WriteLine($"Task Name: {task.Name}");
}
```

This approach makes the code more self-documenting and easier to read and maintain compared to traditional methods with many optional parameters.
