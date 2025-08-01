# ClickUp API Client Plugin Architecture

This document describes the enhanced plugin architecture implemented in the ClickUp API Client library, which follows the Open/Closed Principle by allowing functionality extension without modifying existing code. The plugin system has been significantly enhanced with improved interfaces, better lifecycle management, and comprehensive sample implementations.

## Overview

The plugin architecture provides a flexible and extensible way to extend the ClickUp API Client with custom functionality such as logging, caching, rate limiting, authentication, metrics collection, and more. The enhanced system supports both request/response pipeline plugins and service decorator plugins, allowing for comprehensive monitoring and modification of API interactions.

## Architectural Improvements

### Enhanced Plugin System Features
- **Dual Plugin Types**: Support for both request/response pipeline plugins and service decorator plugins
- **Priority-based Execution**: Plugins execute in configurable priority order
- **Conditional Execution**: Plugins can determine if they should handle specific contexts
- **Comprehensive Context**: Rich context information available to all plugins
- **Fluent Configuration**: Builder pattern for easy plugin registration and configuration
- **Infrastructure Integration**: Seamless integration with the infrastructure abstraction layer

## Core Components

### Enhanced Interfaces

#### IClickUpPlugin
The base contract for all plugins:
```csharp
public interface IClickUpPlugin
{
    string Name { get; }
    int Priority { get; }
    Task InitializeAsync(IServiceProvider serviceProvider);
    Task<bool> CanHandleAsync(PluginContext context);
}
```

#### IRequestResponsePlugin
For plugins that operate on the HTTP request/response pipeline:
```csharp
public interface IRequestResponsePlugin : IClickUpPlugin
{
    Task<HttpRequestMessage> ProcessRequestAsync(HttpRequestMessage request, PluginContext context);
    Task<HttpResponseMessage> ProcessResponseAsync(HttpResponseMessage response, PluginContext context);
}
```

#### IServiceDecoratorPlugin
For plugins that decorate service implementations:
```csharp
public interface IServiceDecoratorPlugin : IClickUpPlugin
{
    TService DecorateService<TService>(TService service, IServiceProvider serviceProvider) where TService : class;
}
```
```

#### IPluginManager
Manages plugin lifecycle and execution:
```csharp
public interface IPluginManager
{
    Task RegisterPluginAsync(IPlugin plugin, IPluginConfiguration? configuration = null, CancellationToken cancellationToken = default);
    Task UnregisterPluginAsync(string pluginId, CancellationToken cancellationToken = default);
    Task<IEnumerable<IPlugin>> GetPluginsAsync(CancellationToken cancellationToken = default);
    Task ExecutePluginsAsync(IPluginContext context, CancellationToken cancellationToken = default);
    // ... other methods
}
```

#### PluginContext
Provides rich execution context for plugins:
```csharp
public class PluginContext
{
    public string OperationName { get; set; }
    public Dictionary<string, object> Properties { get; set; }
    public IServiceProvider ServiceProvider { get; set; }
    public CancellationToken CancellationToken { get; set; }
    public TimeSpan Elapsed { get; set; }
    public Exception Exception { get; set; }
}
```

## Enhanced Built-in Plugins

### LoggingPlugin
Comprehensive logging for API operations with configurable options:
- **Request/Response Logging**: Detailed HTTP request and response information
- **Header Logging**: Optional logging of request/response headers
- **Body Logging**: Optional logging of request/response bodies
- **Error Logging**: Automatic error detection and logging
- **Performance Metrics**: Request duration and timing information
- **Configurable Log Levels**: Control verbosity through LoggingOptions

### RateLimitingPlugin
Intelligent rate limiting with API-aware features:
- **Dynamic Rate Limits**: Updates limits based on API response headers
- **Burst Handling**: Configurable burst limits for short-term spikes
- **Auto-retry Logic**: Automatic retry with exponential backoff
- **Per-endpoint Limiting**: Different limits for different API endpoints
- **Rate Limit Monitoring**: Real-time rate limit status tracking

### CachingPlugin
Advanced response caching with intelligent strategies:
- **HTTP Cache Headers**: Respects Cache-Control and Expires headers
- **Content-Type Filtering**: Configurable caching based on response content types
- **Size Limits**: Configurable maximum cache size and per-item limits
- **TTL Management**: Flexible expiration policies
- **Cache Key Generation**: Intelligent cache key creation including relevant headers
- **Memory Management**: Automatic cleanup and size management

## Enhanced Usage Patterns

### Fluent Client Builder

```csharp
// Enhanced client configuration with plugins
var client = ClickUpClientBuilder.Create()
    .WithApiToken(apiToken)
    .WithPlugin<LoggingPlugin>(options =>
    {
        options.LogRequests = true;
        options.LogResponses = true;
        options.LogErrors = true;
    })
    .WithPlugin<RateLimitingPlugin>(options =>
    {
        options.RequestsPerMinute = 100;
        options.BurstLimit = 10;
        options.AutoRetryOnRateLimit = true;
    })
    .WithPlugin<CachingPlugin>(options =>
    {
        options.DefaultExpiration = TimeSpan.FromMinutes(5);
        options.MaxCacheSize = 50 * 1024 * 1024; // 50MB
    })
    .Build();
```

### Dependency Injection Registration

```csharp
// Enhanced DI registration with plugin configuration
services.AddClickUpClient()
    .WithApiToken(configuration["ClickUp:ApiToken"])
    .WithPlugins(plugins => plugins
        .Add<LoggingPlugin>()
        .Add<RateLimitingPlugin>()
        .Add<CachingPlugin>()
        .Add<CustomMetricsPlugin>()
        .Add<CustomAuthenticationPlugin>());

// Configure plugin options separately
services.Configure<LoggingOptions>(configuration.GetSection("ClickUp:Logging"));
services.Configure<RateLimitingOptions>(configuration.GetSection("ClickUp:RateLimit"));
services.Configure<CachingOptions>(configuration.GetSection("ClickUp:Cache"));
```

### Advanced Configuration

```csharp
services.ConfigureClickUpPlugins(builder => {
    builder.AddPlugin<CustomPlugin>()
           .WithConfiguration(config => {
               config.SetValue("CustomSetting", "Value");
           });
});
```

### Creating Enhanced Custom Plugins

#### Request/Response Pipeline Plugin
```csharp
public class CustomMetricsPlugin : IRequestResponsePlugin
{
    private readonly IMetricsCollector _metricsCollector;
    private readonly ILogger<CustomMetricsPlugin> _logger;

    public string Name => "CustomMetrics";
    public int Priority => 400;

    public CustomMetricsPlugin(IMetricsCollector metricsCollector, ILogger<CustomMetricsPlugin> logger)
    {
        _metricsCollector = metricsCollector;
        _logger = logger;
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        _logger.LogInformation("Custom metrics plugin initialized");
    }

    public async Task<bool> CanHandleAsync(PluginContext context)
    {
        return true; // Handle all requests
    }

    public async Task<HttpRequestMessage> ProcessRequestAsync(HttpRequestMessage request, PluginContext context)
    {
        // Record request metrics
        _metricsCollector.IncrementCounter("clickup_requests_total", new Dictionary<string, string>
        {
            ["method"] = request.Method.Method,
            ["endpoint"] = GetEndpointFromUri(request.RequestUri)
        });

        context.Properties["RequestStartTime"] = DateTimeOffset.UtcNow;
        return request;
    }

    public async Task<HttpResponseMessage> ProcessResponseAsync(HttpResponseMessage response, PluginContext context)
    {
        // Record response metrics with timing
        if (context.Properties.TryGetValue("RequestStartTime", out var startTimeObj) && 
            startTimeObj is DateTimeOffset startTime)
        {
            var duration = DateTimeOffset.UtcNow - startTime;
            
            _metricsCollector.RecordHistogram("clickup_request_duration_seconds", duration.TotalSeconds, new Dictionary<string, string>
            {
                ["method"] = response.RequestMessage?.Method.Method ?? "unknown",
                ["status_code"] = ((int)response.StatusCode).ToString(),
                ["endpoint"] = GetEndpointFromUri(response.RequestMessage?.RequestUri)
            });
        }

        return response;
    }

    private string GetEndpointFromUri(Uri uri)
    {
        // Extract endpoint pattern logic
        return uri?.AbsolutePath ?? "unknown";
    }
}
```

#### Service Decorator Plugin
```csharp
public class AuditingPlugin : IServiceDecoratorPlugin
{
    private readonly IAuditLogger _auditLogger;

    public string Name => "Auditing";
    public int Priority => 500;

    public AuditingPlugin(IAuditLogger auditLogger)
    {
        _auditLogger = auditLogger;
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        // Initialize auditing plugin
    }

    public async Task<bool> CanHandleAsync(PluginContext context)
    {
        return context.Properties.ContainsKey("RequiresAuditing");
    }

    public TService DecorateService<TService>(TService service, IServiceProvider serviceProvider) where TService : class
    {
        if (service is ITasksService tasksService)
        {
            return (TService)(object)new AuditedTasksService(tasksService, _auditLogger);
        }

        return service;
    }
}
```

## Plugin Execution Flow

1. **Before API Operation**: Plugins are executed in registration order
2. **API Operation**: The actual API call is made
3. **After API Operation**: Plugins are executed again with response data
4. **Error Handling**: Plugin errors are logged but don't interrupt the API flow

## Configuration

Plugins can be configured using the `IPluginConfiguration` interface:

```csharp
public class MyPluginConfiguration : IPluginConfiguration
{
    public string GetSetting(string key, string defaultValue = "")
    {
        // Implementation
    }
    
    public T GetValue<T>(string key, T defaultValue = default)
    {
        // Implementation
    }
    
    // ... other methods
}
```

## Best Practices

1. **Keep plugins lightweight**: Avoid heavy operations that could slow down API calls
2. **Handle errors gracefully**: Plugin errors should not break the API flow
3. **Use async/await properly**: Ensure proper async patterns in plugin code
4. **Log appropriately**: Use structured logging for better observability
5. **Test thoroughly**: Write unit tests for your custom plugins
6. **Follow naming conventions**: Use descriptive names for plugin IDs and configurations

## Integration Points

The plugin system is integrated into:
- **BaseService**: All services inheriting from BaseService automatically support plugins
- **TaskCrudService**: Demonstrates plugin integration in specialized services
- **Dependency Injection**: Full DI container integration for easy configuration

## Performance Considerations

- Plugins are executed asynchronously but sequentially
- Plugin errors are caught and logged to prevent API disruption
- Caching plugins can improve performance for repeated operations
- Rate limiting plugins help prevent API quota exhaustion

## Extensibility

The architecture supports:
- Custom plugin types beyond the built-in samples
- Plugin-to-plugin communication through the context
- Dynamic plugin loading and unloading
- Configuration hot-reloading (implementation dependent)

This plugin architecture enables the ClickUp API Client to be extended with new functionality while maintaining the Open/Closed Principle - open for extension, closed for modification.