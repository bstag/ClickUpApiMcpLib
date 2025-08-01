# ClickUp API Client Plugin Architecture

This document describes the plugin architecture implemented in the ClickUp API Client library, which follows the Open/Closed Principle by allowing functionality extension without modifying existing code.

## Overview

The plugin architecture provides a flexible way to extend the ClickUp API Client with custom functionality such as logging, caching, rate limiting, authentication, and more. Plugins can be executed before and after API operations, allowing for comprehensive monitoring and modification of API interactions.

## Core Components

### Interfaces

#### IPlugin
The main contract for all plugins:
```csharp
public interface IPlugin
{
    string Id { get; }
    string Name { get; }
    string Version { get; }
    string Description { get; }
    bool IsEnabled { get; set; }
    
    Task InitializeAsync(IPluginConfiguration configuration, CancellationToken cancellationToken = default);
    Task<IPluginResult> ExecuteAsync(IPluginContext context, CancellationToken cancellationToken = default);
    Task CleanupAsync(CancellationToken cancellationToken = default);
}
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

#### IPluginContext
Provides execution context for plugins:
```csharp
public interface IPluginContext
{
    IApiConnection ApiConnection { get; set; }
    object? RequestData { get; set; }
    object? ResponseData { get; set; }
    string OperationType { get; set; }
    string ServiceName { get; set; }
    Dictionary<string, object> AdditionalData { get; set; }
    
    T? GetValue<T>(string key);
    void SetValue<T>(string key, T value);
}
```

## Built-in Sample Plugins

### LoggingPlugin
Logs API operations with detailed information:
- Operation type and service name
- Request and response data
- Execution timing
- Error handling

### RateLimitingPlugin
Implements rate limiting for API operations:
- Configurable requests per minute
- Per-service rate limiting
- Automatic delay when limits are exceeded

### CachingPlugin
Provides response caching capabilities:
- Configurable cache duration
- Memory-based caching
- Cache key generation based on operation context

## Usage

### Basic Setup

1. **Register the plugin system** in your DI container:
```csharp
services.AddClickUpClient(options => {
    // Configure your ClickUp client
});
```

2. **Add plugins** using extension methods:
```csharp
services.AddClickUpLoggingPlugin();
services.AddClickUpRateLimitingPlugin(options => {
    options.RequestsPerMinute = 100;
});
services.AddClickUpCachingPlugin(options => {
    options.DefaultCacheDurationMinutes = 5;
});
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

### Creating Custom Plugins

1. **Inherit from BasePlugin**:
```csharp
public class CustomPlugin : BasePlugin
{
    public override string Id => "custom-plugin";
    public override string Name => "Custom Plugin";
    public override string Version => "1.0.0";
    public override string Description => "A custom plugin example";

    public override async Task<IPluginResult> ExecuteAsync(IPluginContext context, CancellationToken cancellationToken = default)
    {
        // Your custom logic here
        Logger?.LogInformation("Executing custom plugin for {OperationType} in {ServiceName}", 
            context.OperationType, context.ServiceName);
        
        // Return success result
        return PluginResult.Success();
    }
}
```

2. **Register your custom plugin**:
```csharp
services.AddClickUpPlugin<CustomPlugin>();
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