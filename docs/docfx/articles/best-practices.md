# Best Practices Guide

This guide provides recommendations for optimal usage of the ClickUp .NET SDK, covering performance, security, error handling, and maintainability best practices.

## Table of Contents

- [Configuration Best Practices](#configuration-best-practices)
- [Fluent API Usage](#fluent-api-usage)
- [Caching Strategies](#caching-strategies)
- [Error Handling](#error-handling)
- [Authentication & Security](#authentication--security)
- [Performance Optimization](#performance-optimization)
- [Testing Recommendations](#testing-recommendations)
- [Logging and Monitoring](#logging-and-monitoring)

## Configuration Best Practices

### Dependency Injection Setup

**✅ Recommended**
```csharp
// Program.cs or Startup.cs
services.AddClickUpSdk(options => {
    // Use configuration binding
    configuration.GetSection("ClickUp").Bind(options);
    
    // Configure caching strategy based on your needs
    options.Caching = new MemoryCachingStrategy(
        defaultExpiration: TimeSpan.FromMinutes(5),
        maxSize: 1000
    );
    
    // Configure retry strategy for resilience
    options.RetryStrategy = new ExponentialBackoffRetryStrategy(
        maxRetries: 3,
        baseDelay: TimeSpan.FromSeconds(1)
    );
    
    // Enable request/response logging in development
    options.EnableRequestLogging = hostEnvironment.IsDevelopment();
});
```

**❌ Avoid**
```csharp
// Don't hardcode sensitive values
services.AddClickUpSdk(options => {
    options.ApiKey = "pk_12345_hardcoded_key"; // Never do this!
    options.BaseUrl = "https://api.clickup.com/api/v2";
});
```

### Configuration File Structure

**✅ Recommended**
```json
{
  "ClickUp": {
    "ApiKey": "", // Set via environment variables or user secrets
    "BaseUrl": "https://api.clickup.com/api/v2",
    "Timeout": "00:00:30",
    "MaxRetries": 3,
    "CacheExpiration": "00:05:00",
    "EnableRequestLogging": false
  },
  "Logging": {
    "LogLevel": {
      "ClickUp.Api.Client": "Information"
    }
  }
}
```

## Fluent API Usage

### Task Operations

**✅ Recommended - Chain operations logically**
```csharp
// Build queries step by step with clear intent
var tasks = await _taskService
    .GetTasks()
    .FromList(listId)
    .WithStatus("open", "in progress")
    .AssignedTo(currentUserId)
    .DueBefore(DateTime.UtcNow.AddDays(7))
    .OrderBy("due_date")
    .Ascending()
    .Take(50)
    .ExecuteAsync();
```

**✅ Recommended - Use conditional chaining**
```csharp
var queryBuilder = _taskService
    .GetTasks()
    .FromList(listId);

// Conditionally add filters
if (!string.IsNullOrEmpty(assigneeId))
    queryBuilder = queryBuilder.AssignedTo(assigneeId);

if (includeSubtasks)
    queryBuilder = queryBuilder.IncludeSubtasks();

var tasks = await queryBuilder.ExecuteAsync();
```

**❌ Avoid - Overly complex single chains**
```csharp
// Too complex, hard to read and debug
var tasks = await _taskService.GetTasks().FromList(listId).WithStatus("open").AssignedTo(userId).DueBefore(DateTime.Now).OrderBy("created").Descending().IncludeSubtasks().WithCustomFields().IncludeAttachments().Take(100).Skip(50).ExecuteAsync();
```

### URL Building

**✅ Recommended**
```csharp
// Use the fluent URL builder for complex URLs
var url = _urlBuilder
    .SetPath("/team/{teamId}/space")
    .AddPathParameter("teamId", teamId)
    .AddQueryParameter("archived", "false")
    .AddQueryParameter("include_members", "true")
    .Build();
```

## Caching Strategies

### Memory Caching

**✅ Recommended for single-instance applications**
```csharp
services.AddClickUpSdk(options => {
    options.Caching = new MemoryCachingStrategy(
        defaultExpiration: TimeSpan.FromMinutes(5),
        maxSize: 1000 // Prevent memory bloat
    );
});
```

### Distributed Caching

**✅ Recommended for multi-instance applications**
```csharp
// Configure distributed cache first
services.AddStackExchangeRedisCache(options => {
    options.Configuration = connectionString;
});

// Then configure ClickUp SDK to use it
services.AddClickUpSdk(options => {
    options.Caching = new DistributedCachingStrategy(
        serviceProvider.GetRequiredService<IDistributedCache>(),
        defaultExpiration: TimeSpan.FromMinutes(10)
    );
});
```

### Cache Key Strategies

**✅ Recommended - Use descriptive, hierarchical keys**
```csharp
// Good cache key patterns
"clickup:task:{taskId}"
"clickup:list:{listId}:tasks"
"clickup:user:{userId}:spaces"
"clickup:team:{teamId}:members"
```

### Cache Invalidation

**✅ Recommended - Use tag-based invalidation**
```csharp
// Tag cache entries for easy invalidation
await _cacheService.SetAsync(
    key: $"task:{taskId}",
    value: task,
    expiration: TimeSpan.FromMinutes(5),
    tags: new[] { $"list:{task.ListId}", $"user:{task.AssigneeId}" }
);

// Invalidate all tasks for a list when list is updated
await _cacheService.RemoveByTagAsync($"list:{listId}");
```

## Error Handling

### Result Pattern Usage

**✅ Recommended - Always check IsSuccess**
```csharp
public async Task<TaskModel> GetTaskSafelyAsync(string taskId)
{
    var result = await _taskService.GetTaskAsync(taskId);
    
    if (result.IsSuccess)
    {
        return result.Data;
    }
    
    // Log errors for debugging
    foreach (var error in result.Errors)
    {
        _logger.LogWarning("Task retrieval failed: {ErrorCode} - {ErrorMessage}", 
            error.Code, error.Message);
    }
    
    // Return null or throw appropriate exception
    return null;
}
```

### Exception Handling

**✅ Recommended - Handle specific exception types**
```csharp
try
{
    var result = await _taskService.CreateTaskAsync(taskData);
    return result;
}
catch (ValidationException ex)
{
    // Handle validation errors - usually client-side issues
    _logger.LogWarning(ex, "Task validation failed");
    throw new BusinessLogicException("Invalid task data provided", ex);
}
catch (ClickUpApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
{
    // Handle authentication issues
    _logger.LogError(ex, "Authentication failed");
    throw new AuthenticationException("ClickUp API authentication failed", ex);
}
catch (ClickUpApiException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
{
    // Handle rate limiting - retry strategy should handle this automatically
    _logger.LogWarning(ex, "Rate limit exceeded");
    throw new RateLimitException("API rate limit exceeded", ex);
}
catch (ClickUpApiException ex)
{
    // Handle other API errors
    _logger.LogError(ex, "ClickUp API error occurred");
    throw;
}
```

### Retry Strategy Configuration

**✅ Recommended - Configure appropriate retry strategies**
```csharp
// For transient errors (network issues, temporary server errors)
services.AddClickUpSdk(options => {
    options.RetryStrategy = new ExponentialBackoffRetryStrategy(
        maxRetries: 3,
        baseDelay: TimeSpan.FromSeconds(1),
        maxDelay: TimeSpan.FromSeconds(30),
        retryableStatusCodes: new[] { 
            HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout,
            HttpStatusCode.TooManyRequests
        }
    );
});
```

## Authentication & Security

### API Key Management

**✅ Recommended - Use secure configuration**
```csharp
// Use user secrets in development
// dotnet user-secrets set "ClickUp:ApiKey" "your-api-key"

// Use environment variables in production
// CLICKUP__APIKEY=your-api-key

// Configuration binding
services.AddClickUpSdk(options => {
    options.ApiKey = configuration["ClickUp:ApiKey"];
});
```

**❌ Avoid - Hardcoded secrets**
```csharp
// Never commit API keys to source control
services.AddClickUpSdk(options => {
    options.ApiKey = "pk_12345_secret_key"; // DON'T DO THIS!
});
```

### OAuth Implementation

**✅ Recommended - Secure OAuth flow**
```csharp
public class OAuthService
{
    private readonly ITokenStorage _tokenStorage;
    private readonly ILogger<OAuthService> _logger;
    
    public async Task<string> GetValidTokenAsync(string userId)
    {
        var token = await _tokenStorage.GetTokenAsync(userId);
        
        if (IsTokenExpired(token))
        {
            token = await RefreshTokenAsync(token.RefreshToken);
            await _tokenStorage.SaveTokenAsync(userId, token);
        }
        
        return token.AccessToken;
    }
    
    private async Task<OAuthToken> RefreshTokenAsync(string refreshToken)
    {
        // Implement secure token refresh logic
        // Store tokens securely (encrypted)
        // Handle refresh failures appropriately
    }
}
```

### Request Security

**✅ Recommended - Validate and sanitize inputs**
```csharp
public async Task<Result<TaskModel>> CreateTaskAsync(CreateTaskRequest request)
{
    // Validate input
    var validationResult = await _validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Result<TaskModel>.Failure(validationResult.Errors);
    }
    
    // Sanitize input data
    var sanitizedRequest = _sanitizer.Sanitize(request);
    
    // Proceed with API call
    return await _taskService.CreateTaskAsync(sanitizedRequest);
}
```

## Performance Optimization

### Batch Operations

**✅ Recommended - Use batch operations when available**
```csharp
// Instead of multiple individual calls
// ❌ Avoid
foreach (var taskId in taskIds)
{
    var task = await _taskService.GetTaskAsync(taskId);
    tasks.Add(task.Data);
}

// ✅ Use batch operations
var tasks = await _taskService.GetTasksBatchAsync(taskIds);
```

### Async/Await Best Practices

**✅ Recommended - Use ConfigureAwait(false) in libraries**
```csharp
public async Task<Result<TaskModel>> GetTaskAsync(string taskId)
{
    var cacheKey = $"task:{taskId}";
    
    // Check cache first
    var cachedTask = await _cache.GetAsync<TaskModel>(cacheKey)
        .ConfigureAwait(false);
    
    if (cachedTask != null)
        return Result<TaskModel>.Success(cachedTask);
    
    // Fetch from API
    var result = await _httpClient.GetAsync($"/task/{taskId}")
        .ConfigureAwait(false);
    
    // Process and cache result
    var task = await ProcessResponseAsync(result)
        .ConfigureAwait(false);
    
    await _cache.SetAsync(cacheKey, task, TimeSpan.FromMinutes(5))
        .ConfigureAwait(false);
    
    return Result<TaskModel>.Success(task);
}
```

### Memory Management

**✅ Recommended - Dispose resources properly**
```csharp
public class TaskProcessor : IDisposable
{
    private readonly ITaskService _taskService;
    private readonly Timer _timer;
    private bool _disposed = false;
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _timer?.Dispose();
            // Dispose other resources
        }
        _disposed = true;
    }
}
```

## Testing Recommendations

### Unit Testing

**✅ Recommended - Mock external dependencies**
```csharp
[Test]
public async Task GetTask_WithValidId_ReturnsTask()
{
    // Arrange
    var taskId = "123";
    var expectedTask = new TaskModel { Id = taskId, Name = "Test Task" };
    
    var mockTaskService = new Mock<ITaskService>();
    mockTaskService
        .Setup(x => x.GetTaskAsync(taskId))
        .ReturnsAsync(Result<TaskModel>.Success(expectedTask));
    
    var service = new BusinessService(mockTaskService.Object);
    
    // Act
    var result = await service.ProcessTaskAsync(taskId);
    
    // Assert
    Assert.IsTrue(result.IsSuccess);
    Assert.AreEqual(expectedTask.Name, result.Data.Name);
    mockTaskService.Verify(x => x.GetTaskAsync(taskId), Times.Once);
}
```

### Integration Testing

**✅ Recommended - Test with real API (carefully)**
```csharp
[Test]
[Category("Integration")]
public async Task GetTask_IntegrationTest()
{
    // Use test API key and test data
    var services = new ServiceCollection();
    services.AddClickUpSdk(options => {
        options.ApiKey = TestConfiguration.TestApiKey;
        options.BaseUrl = TestConfiguration.TestBaseUrl;
    });
    
    var serviceProvider = services.BuildServiceProvider();
    var taskService = serviceProvider.GetRequiredService<ITaskService>();
    
    // Test with known test task ID
    var result = await taskService.GetTaskAsync(TestConfiguration.TestTaskId);
    
    Assert.IsTrue(result.IsSuccess);
    Assert.IsNotNull(result.Data);
}
```

### Test Data Management

**✅ Recommended - Use builders for test data**
```csharp
public class TaskModelBuilder
{
    private TaskModel _task = new TaskModel();
    
    public TaskModelBuilder WithId(string id)
    {
        _task.Id = id;
        return this;
    }
    
    public TaskModelBuilder WithName(string name)
    {
        _task.Name = name;
        return this;
    }
    
    public TaskModelBuilder WithStatus(string status)
    {
        _task.Status = status;
        return this;
    }
    
    public TaskModel Build() => _task;
}

// Usage in tests
var task = new TaskModelBuilder()
    .WithId("123")
    .WithName("Test Task")
    .WithStatus("open")
    .Build();
```

## Logging and Monitoring

### Structured Logging

**✅ Recommended - Use structured logging**
```csharp
public class TaskService
{
    private readonly ILogger<TaskService> _logger;
    
    public async Task<Result<TaskModel>> GetTaskAsync(string taskId)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["TaskId"] = taskId,
            ["Operation"] = "GetTask"
        });
        
        _logger.LogInformation("Starting task retrieval for {TaskId}", taskId);
        
        try
        {
            var result = await FetchTaskFromApiAsync(taskId);
            
            _logger.LogInformation("Successfully retrieved task {TaskId}", taskId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve task {TaskId}", taskId);
            throw;
        }
    }
}
```

### Performance Monitoring

**✅ Recommended - Monitor key metrics**
```csharp
public class MetricsService
{
    private readonly IMetrics _metrics;
    
    public async Task<T> TrackOperationAsync<T>(string operationName, Func<Task<T>> operation)
    {
        using var timer = _metrics.Measure.Timer.Time("clickup_operation_duration", 
            new MetricTags("operation", operationName));
        
        try
        {
            var result = await operation();
            _metrics.Measure.Counter.Increment("clickup_operation_success", 
                new MetricTags("operation", operationName));
            return result;
        }
        catch (Exception)
        {
            _metrics.Measure.Counter.Increment("clickup_operation_failure", 
                new MetricTags("operation", operationName));
            throw;
        }
    }
}
```

### Health Checks

**✅ Recommended - Implement health checks**
```csharp
public class ClickUpHealthCheck : IHealthCheck
{
    private readonly ITaskService _taskService;
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Perform a lightweight API call to check connectivity
            var result = await _taskService.GetUserAsync();
            
            if (result.IsSuccess)
            {
                return HealthCheckResult.Healthy("ClickUp API is accessible");
            }
            
            return HealthCheckResult.Degraded("ClickUp API returned errors", 
                data: new Dictionary<string, object> { ["errors"] = result.Errors });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("ClickUp API is not accessible", ex);
        }
    }
}

// Register health check
services.AddHealthChecks()
    .AddCheck<ClickUpHealthCheck>("clickup");
```

## Summary

Following these best practices will help you:

- **Build maintainable applications** with proper separation of concerns
- **Achieve optimal performance** through effective caching and async patterns
- **Handle errors gracefully** with comprehensive error handling strategies
- **Maintain security** through proper authentication and input validation
- **Monitor effectively** with structured logging and health checks
- **Test thoroughly** with comprehensive unit and integration tests

Remember to regularly review and update your implementation as the SDK evolves and your requirements change.