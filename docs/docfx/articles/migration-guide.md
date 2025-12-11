# Migration Guide

This guide helps you migrate from previous versions of the ClickUp .NET SDK to the latest refactored version.

## Overview

The ClickUp SDK has undergone a major refactoring to implement SOLID principles, enhance performance, and improve developer experience. This guide covers breaking changes and provides step-by-step migration instructions.

## Breaking Changes

### 1. Service Architecture Changes

#### Before (Old Version)
```csharp
// Old monolithic service approach
var client = new ClickUpClient("your-api-key");
var tasks = await client.GetTasksAsync(listId);
```

#### After (New Version)
```csharp
// New service-oriented architecture with dependency injection
var services = new ServiceCollection();
services.AddClickUpSdk(options => {
    options.ApiKey = "your-api-key";
    options.BaseUrl = "https://api.clickup.com/api/v2";
});

var serviceProvider = services.BuildServiceProvider();
var taskService = serviceProvider.GetRequiredService<ITaskService>();
var tasks = await taskService.GetTasksAsync(listId);
```

### 2. Fluent API Changes

#### Before (Old Version)
```csharp
// Old parameter-based approach
var request = new GetTasksRequest
{
    ListId = listId,
    Archived = false,
    IncludeSubtasks = true,
    OrderBy = "created",
    Reverse = true
};
var tasks = await client.GetTasksAsync(request);
```

#### After (New Version)
```csharp
// New fluent API approach
var tasks = await taskService
    .GetTasks()
    .FromList(listId)
    .ExcludeArchived()
    .IncludeSubtasks()
    .OrderBy("created")
    .Descending()
    .ExecuteAsync();
```

### 3. Error Handling Changes

#### Before (Old Version)
```csharp
try
{
    var task = await client.GetTaskAsync(taskId);
}
catch (HttpRequestException ex)
{
    // Generic HTTP exception handling
    Console.WriteLine($"Error: {ex.Message}");
}
```

#### After (New Version)
```csharp
try
{
    var result = await taskService.GetTaskAsync(taskId);
    if (result.IsSuccess)
    {
        var task = result.Data;
        // Process task
    }
    else
    {
        // Handle specific error types
        foreach (var error in result.Errors)
        {
            Console.WriteLine($"Error: {error.Message} (Code: {error.Code})");
        }
    }
}
catch (ClickUpApiException ex)
{
    // Specific ClickUp API exceptions
    Console.WriteLine($"ClickUp API Error: {ex.Message}");
}
catch (ValidationException ex)
{
    // Validation errors
    Console.WriteLine($"Validation Error: {ex.Message}");
}
```

### 4. Authentication Changes

#### Before (Old Version)
```csharp
var client = new ClickUpClient("your-api-key");
```

#### After (New Version)
```csharp
// API Key Authentication
services.AddClickUpSdk(options => {
    options.Authentication = new ApiKeyAuthenticationStrategy("your-api-key");
});

// OAuth Authentication
services.AddClickUpSdk(options => {
    options.Authentication = new OAuthAuthenticationStrategy("your-oauth-token");
});
```

### 5. Caching Configuration

#### Before (Old Version)
```csharp
// No built-in caching support
var client = new ClickUpClient("your-api-key");
```

#### After (New Version)
```csharp
// Memory caching
services.AddClickUpSdk(options => {
    options.Caching = new MemoryCachingStrategy(TimeSpan.FromMinutes(5));
});

// Distributed caching
services.AddClickUpSdk(options => {
    options.Caching = new DistributedCachingStrategy(distributedCache, TimeSpan.FromMinutes(10));
});
```

## Step-by-Step Migration

### Step 1: Update Package References

```xml
<!-- Remove old package -->
<!-- <PackageReference Include="ClickUp.Api.Client" Version="1.x.x" /> -->

<!-- Add new package -->
<PackageReference Include="ClickUp.Api.Client" Version="2.0.0" />
```

### Step 2: Update Dependency Injection Setup

```csharp
// In Program.cs or Startup.cs
services.AddClickUpSdk(options => {
    options.ApiKey = configuration["ClickUp:ApiKey"];
    options.BaseUrl = "https://api.clickup.com/api/v2";
    
    // Optional: Configure caching
    options.Caching = new MemoryCachingStrategy(TimeSpan.FromMinutes(5));
    
    // Optional: Configure retry policy
    options.RetryStrategy = new ExponentialBackoffRetryStrategy(
        maxRetries: 3,
        baseDelay: TimeSpan.FromSeconds(1)
    );
});
```

### Step 3: Update Service Injection

```csharp
// Old approach - remove
// private readonly ClickUpClient _client;

// New approach - inject specific services
private readonly ITaskService _taskService;
private readonly ISpaceService _spaceService;
private readonly IListService _listService;

public MyService(
    ITaskService taskService,
    ISpaceService spaceService,
    IListService listService)
{
    _taskService = taskService;
    _spaceService = spaceService;
    _listService = listService;
}
```

### Step 4: Update API Calls

```csharp
// Replace old API calls with new fluent syntax

// Tasks
var tasks = await _taskService
    .GetTasks()
    .FromList(listId)
    .WithStatus("open")
    .AssignedTo(userId)
    .ExecuteAsync();

// Spaces
var spaces = await _spaceService
    .GetSpaces()
    .FromTeam(teamId)
    .IncludeArchived(false)
    .ExecuteAsync();

// Lists
var lists = await _listService
    .GetLists()
    .FromSpace(spaceId)
    .WithFolders(true)
    .ExecuteAsync();
```

### Step 5: Update Error Handling

```csharp
// Wrap API calls in proper error handling
try
{
    var result = await _taskService.GetTaskAsync(taskId);
    
    if (result.IsSuccess)
    {
        var task = result.Data;
        // Process successful result
    }
    else
    {
        // Handle API errors
        LogErrors(result.Errors);
    }
}
catch (ClickUpApiException ex)
{
    // Handle specific ClickUp API exceptions
    _logger.LogError(ex, "ClickUp API error occurred");
}
catch (ValidationException ex)
{
    // Handle validation errors
    _logger.LogWarning(ex, "Validation error occurred");
}
```

## Common Migration Issues

### Issue 1: Missing Service Registration

**Problem**: `InvalidOperationException: Unable to resolve service for type 'ITaskService'`

**Solution**: Ensure you've called `AddClickUpSdk()` in your service registration:

```csharp
services.AddClickUpSdk(options => {
    options.ApiKey = "your-api-key";
});
```

### Issue 2: Configuration Not Found

**Problem**: API key or configuration values are null

**Solution**: Verify your configuration setup:

```csharp
// appsettings.json
{
  "ClickUp": {
    "ApiKey": "your-api-key",
    "BaseUrl": "https://api.clickup.com/api/v2"
  }
}

// Configuration binding
services.AddClickUpSdk(options => {
    configuration.GetSection("ClickUp").Bind(options);
});
```

### Issue 3: Async/Await Pattern Changes

**Problem**: Synchronous methods no longer available

**Solution**: All API methods are now async. Update your code:

```csharp
// Old synchronous call - no longer supported
// var task = client.GetTask(taskId);

// New async call
var task = await _taskService.GetTaskAsync(taskId);
```

### Issue 4: Response Type Changes

**Problem**: Direct object returns vs. Result<T> pattern

**Solution**: Handle the new Result<T> pattern:

```csharp
// Old direct return
// var task = await client.GetTaskAsync(taskId);

// New Result<T> pattern
var result = await _taskService.GetTaskAsync(taskId);
if (result.IsSuccess)
{
    var task = result.Data;
    // Use task
}
```

## Testing Migration

### Unit Testing Changes

```csharp
// Old testing approach
[Test]
public async Task GetTask_ReturnsTask()
{
    var mockClient = new Mock<IClickUpClient>();
    mockClient.Setup(x => x.GetTaskAsync(It.IsAny<string>()))
           .ReturnsAsync(new TaskModel());
    
    var service = new MyService(mockClient.Object);
    var result = await service.GetTaskAsync("123");
    
    Assert.IsNotNull(result);
}

// New testing approach
[Test]
public async Task GetTask_ReturnsTask()
{
    var mockTaskService = new Mock<ITaskService>();
    mockTaskService.Setup(x => x.GetTaskAsync(It.IsAny<string>()))
                  .ReturnsAsync(Result<TaskModel>.Success(new TaskModel()));
    
    var service = new MyService(mockTaskService.Object);
    var result = await service.GetTaskAsync("123");
    
    Assert.IsTrue(result.IsSuccess);
    Assert.IsNotNull(result.Data);
}
```

## Performance Considerations

### Caching Benefits

The new version includes built-in caching that can significantly improve performance:

```csharp
// Configure appropriate cache duration based on data volatility
services.AddClickUpSdk(options => {
    options.Caching = new MemoryCachingStrategy(
        defaultExpiration: TimeSpan.FromMinutes(5),
        maxSize: 1000
    );
});
```

### Retry Strategy Benefits

Built-in retry strategies improve reliability:

```csharp
services.AddClickUpSdk(options => {
    options.RetryStrategy = new ExponentialBackoffRetryStrategy(
        maxRetries: 3,
        baseDelay: TimeSpan.FromSeconds(1),
        maxDelay: TimeSpan.FromSeconds(30)
    );
});
```

## Validation

After migration, verify your implementation:

1. **Compile successfully** - No compilation errors
2. **Run tests** - All unit and integration tests pass
3. **Test API calls** - Verify actual API calls work as expected
4. **Monitor performance** - Check that caching and retry strategies work
5. **Error handling** - Verify proper error handling and logging

## Getting Help

If you encounter issues during migration:

1. Check the [API Documentation](../api/index.md)
2. Review [Best Practices](best-practices.md)
3. Check [GitHub Issues](https://github.com/your-repo/issues)
4. Contact support or create a new issue

## Next Steps

After successful migration:

1. Review [Best Practices](best-practices.md) for optimal usage
2. Explore new features like advanced caching and retry strategies
3. Consider implementing additional error handling patterns
4. Update your documentation and team knowledge