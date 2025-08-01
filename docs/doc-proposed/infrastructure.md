# Infrastructure Abstraction Layer

This document describes the infrastructure abstraction layer implemented in the ClickUp API Client SDK, which provides testable abstractions for file system operations, date/time providers, HTTP client factory, and configuration management.

## Overview

The infrastructure abstraction layer follows the Dependency Inversion Principle by providing interfaces for external dependencies, making the SDK more testable, maintainable, and flexible. This approach allows for easy mocking in unit tests and enables different implementations for different environments.

## Core Infrastructure Interfaces

### IFileSystemProvider

Abstracts file system operations to enable testability and cross-platform compatibility:

```csharp
public interface IFileSystemProvider
{
    // File operations
    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);
    Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default);
    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);
    Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);
    Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);
    Task<FileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken = default);
    
    // Directory operations
    Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default);
    Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
    Task DeleteDirectoryAsync(string path, bool recursive = false, CancellationToken cancellationToken = default);
    Task<string[]> GetFilesAsync(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default);
    Task<string[]> GetDirectoriesAsync(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default);
    
    // Path operations
    string GetTempPath();
    string GetTempFileName();
    string CombinePath(params string[] paths);
    string GetDirectoryName(string path);
    string GetFileName(string path);
    string GetFileNameWithoutExtension(string path);
    string GetExtension(string path);
    bool IsPathRooted(string path);
    string GetFullPath(string path);
}
```

### IDateTimeProvider

Abstracts date and time operations for testability:

```csharp
public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTime Today { get; }
    DateTimeOffset OffsetNow { get; }
    DateTimeOffset OffsetUtcNow { get; }
    
    DateTime Parse(string dateTime);
    DateTime ParseExact(string dateTime, string format, IFormatProvider provider);
    bool TryParse(string dateTime, out DateTime result);
    bool TryParseExact(string dateTime, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result);
    
    DateTimeOffset ParseOffset(string dateTime);
    DateTimeOffset ParseOffsetExact(string dateTime, string format, IFormatProvider provider);
    bool TryParseOffset(string dateTime, out DateTimeOffset result);
    bool TryParseOffsetExact(string dateTime, string format, IFormatProvider provider, DateTimeStyles style, out DateTimeOffset result);
    
    long ToUnixTimeSeconds(DateTimeOffset dateTimeOffset);
    long ToUnixTimeMilliseconds(DateTimeOffset dateTimeOffset);
    DateTimeOffset FromUnixTimeSeconds(long seconds);
    DateTimeOffset FromUnixTimeMilliseconds(long milliseconds);
}
```

### IHttpClientFactoryProvider

Abstracts HTTP client creation and management:

```csharp
public interface IHttpClientFactoryProvider
{
    HttpClient CreateClient();
    HttpClient CreateClient(string name);
    HttpClient CreateClient(HttpClientOptions options);
    
    void ConfigureClient(string name, Action<HttpClient> configureClient);
    void ConfigureClient(string name, Action<HttpClientOptions> configureOptions);
    
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> SendAsync(string clientName, HttpRequestMessage request, CancellationToken cancellationToken = default);
    
    void Dispose();
}

public class HttpClientOptions
{
    public string BaseAddress { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public Dictionary<string, string> DefaultHeaders { get; set; } = new();
    public bool FollowRedirects { get; set; } = true;
    public int MaxRedirects { get; set; } = 10;
    public DecompressionMethods AutomaticDecompression { get; set; } = DecompressionMethods.GZip | DecompressionMethods.Deflate;
    public Version HttpVersion { get; set; } = HttpVersion.Version11;
    public HttpVersionPolicy VersionPolicy { get; set; } = HttpVersionPolicy.RequestVersionOrLower;
}
```

### IConfigurationProvider

Abstracts configuration management with multiple sources:

```csharp
public interface IConfigurationProvider
{
    T GetValue<T>(string key);
    T GetValue<T>(string key, T defaultValue);
    string GetConnectionString(string name);
    
    IConfigurationSection GetSection(string key);
    IEnumerable<IConfigurationSection> GetChildren();
    
    bool TryGetValue<T>(string key, out T value);
    
    void SetValue<T>(string key, T value);
    void RemoveValue(string key);
    
    void Reload();
    
    event Action<string, object> ValueChanged;
}
```

## Default Implementations

### FileSystemProvider

Default implementation using System.IO:

```csharp
public class FileSystemProvider : IFileSystemProvider
{
    public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
    {
        return await File.ReadAllTextAsync(path, cancellationToken);
    }

    public async Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        await File.WriteAllTextAsync(path, content, cancellationToken);
    }

    public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
        return await File.ReadAllBytesAsync(path, cancellationToken);
    }

    public async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        await File.WriteAllBytesAsync(path, bytes, cancellationToken);
    }

    public Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(File.Exists(path));
    }

    public Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        return Task.CompletedTask;
    }

    public Task<FileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken = default)
    {
        var fileInfo = new FileInfo(path);
        return Task.FromResult(fileInfo);
    }

    public Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Directory.Exists(path));
    }

    public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return Task.CompletedTask;
    }

    public Task DeleteDirectoryAsync(string path, bool recursive = false, CancellationToken cancellationToken = default)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive);
        }
        return Task.CompletedTask;
    }

    public Task<string[]> GetFilesAsync(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default)
    {
        var files = Directory.GetFiles(path, searchPattern, searchOption);
        return Task.FromResult(files);
    }

    public Task<string[]> GetDirectoriesAsync(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default)
    {
        var directories = Directory.GetDirectories(path, searchPattern, searchOption);
        return Task.FromResult(directories);
    }

    public string GetTempPath() => Path.GetTempPath();
    public string GetTempFileName() => Path.GetTempFileName();
    public string CombinePath(params string[] paths) => Path.Combine(paths);
    public string GetDirectoryName(string path) => Path.GetDirectoryName(path);
    public string GetFileName(string path) => Path.GetFileName(path);
    public string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);
    public string GetExtension(string path) => Path.GetExtension(path);
    public bool IsPathRooted(string path) => Path.IsPathRooted(path);
    public string GetFullPath(string path) => Path.GetFullPath(path);
}
```

### DateTimeProvider

Default implementation using System.DateTime:

```csharp
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Today => DateTime.Today;
    public DateTimeOffset OffsetNow => DateTimeOffset.Now;
    public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;

    public DateTime Parse(string dateTime) => DateTime.Parse(dateTime);
    public DateTime ParseExact(string dateTime, string format, IFormatProvider provider) => DateTime.ParseExact(dateTime, format, provider);
    public bool TryParse(string dateTime, out DateTime result) => DateTime.TryParse(dateTime, out result);
    public bool TryParseExact(string dateTime, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result) => DateTime.TryParseExact(dateTime, format, provider, style, out result);

    public DateTimeOffset ParseOffset(string dateTime) => DateTimeOffset.Parse(dateTime);
    public DateTimeOffset ParseOffsetExact(string dateTime, string format, IFormatProvider provider) => DateTimeOffset.ParseExact(dateTime, format, provider);
    public bool TryParseOffset(string dateTime, out DateTimeOffset result) => DateTimeOffset.TryParse(dateTime, out result);
    public bool TryParseOffsetExact(string dateTime, string format, IFormatProvider provider, DateTimeStyles style, out DateTimeOffset result) => DateTimeOffset.TryParseExact(dateTime, format, provider, style, out result);

    public long ToUnixTimeSeconds(DateTimeOffset dateTimeOffset) => dateTimeOffset.ToUnixTimeSeconds();
    public long ToUnixTimeMilliseconds(DateTimeOffset dateTimeOffset) => dateTimeOffset.ToUnixTimeMilliseconds();
    public DateTimeOffset FromUnixTimeSeconds(long seconds) => DateTimeOffset.FromUnixTimeSeconds(seconds);
    public DateTimeOffset FromUnixTimeMilliseconds(long milliseconds) => DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
}
```

### HttpClientFactoryProvider

Default implementation using IHttpClientFactory:

```csharp
public class HttpClientFactoryProvider : IHttpClientFactoryProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpClientFactoryProvider> _logger;
    private readonly ConcurrentDictionary<string, HttpClientOptions> _clientOptions;

    public HttpClientFactoryProvider(IHttpClientFactory httpClientFactory, ILogger<HttpClientFactoryProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _clientOptions = new ConcurrentDictionary<string, HttpClientOptions>();
    }

    public HttpClient CreateClient()
    {
        return _httpClientFactory.CreateClient();
    }

    public HttpClient CreateClient(string name)
    {
        var client = _httpClientFactory.CreateClient(name);
        
        if (_clientOptions.TryGetValue(name, out var options))
        {
            ConfigureClientWithOptions(client, options);
        }
        
        return client;
    }

    public HttpClient CreateClient(HttpClientOptions options)
    {
        var client = _httpClientFactory.CreateClient();
        ConfigureClientWithOptions(client, options);
        return client;
    }

    public void ConfigureClient(string name, Action<HttpClient> configureClient)
    {
        // This would typically be done during service registration
        // For runtime configuration, we store the options
        var client = CreateClient(name);
        configureClient(client);
    }

    public void ConfigureClient(string name, Action<HttpClientOptions> configureOptions)
    {
        var options = new HttpClientOptions();
        configureOptions(options);
        _clientOptions.AddOrUpdate(name, options, (key, existing) => options);
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();
        return await client.SendAsync(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> SendAsync(string clientName, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        using var client = CreateClient(clientName);
        return await client.SendAsync(request, cancellationToken);
    }

    private void ConfigureClientWithOptions(HttpClient client, HttpClientOptions options)
    {
        if (!string.IsNullOrEmpty(options.BaseAddress))
        {
            client.BaseAddress = new Uri(options.BaseAddress);
        }

        client.Timeout = options.Timeout;

        foreach (var header in options.DefaultHeaders)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }
    }

    public void Dispose()
    {
        // HttpClientFactory manages the lifecycle of HttpClient instances
        // No explicit disposal needed
    }
}
```

### ConfigurationProvider

Default implementation using Microsoft.Extensions.Configuration:

```csharp
public class ConfigurationProvider : IConfigurationProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationProvider> _logger;
    private readonly ConcurrentDictionary<string, object> _cache;

    public ConfigurationProvider(IConfiguration configuration, ILogger<ConfigurationProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _cache = new ConcurrentDictionary<string, object>();
        
        // Subscribe to configuration changes if supported
        if (_configuration is IConfigurationRoot configRoot)
        {
            ChangeToken.OnChange(configRoot.GetReloadToken, OnConfigurationChanged);
        }
    }

    public T GetValue<T>(string key)
    {
        try
        {
            return _configuration.GetValue<T>(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get configuration value for key: {Key}", key);
            throw;
        }
    }

    public T GetValue<T>(string key, T defaultValue)
    {
        try
        {
            return _configuration.GetValue(key, defaultValue);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get configuration value for key: {Key}, returning default value", key);
            return defaultValue;
        }
    }

    public string GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name);
    }

    public IConfigurationSection GetSection(string key)
    {
        return _configuration.GetSection(key);
    }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        return _configuration.GetChildren();
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        try
        {
            value = GetValue<T>(key);
            return value != null;
        }
        catch
        {
            value = default(T);
            return false;
        }
    }

    public void SetValue<T>(string key, T value)
    {
        // Note: IConfiguration is typically read-only
        // This would require a writable configuration provider
        _cache.AddOrUpdate(key, value, (k, v) => value);
        ValueChanged?.Invoke(key, value);
    }

    public void RemoveValue(string key)
    {
        _cache.TryRemove(key, out _);
        ValueChanged?.Invoke(key, null);
    }

    public void Reload()
    {
        if (_configuration is IConfigurationRoot configRoot)
        {
            configRoot.Reload();
            _cache.Clear();
        }
    }

    public event Action<string, object> ValueChanged;

    private void OnConfigurationChanged()
    {
        _logger.LogInformation("Configuration changed, clearing cache");
        _cache.Clear();
    }
}
```

## Test Implementations

### InMemoryFileSystemProvider

In-memory implementation for testing:

```csharp
public class InMemoryFileSystemProvider : IFileSystemProvider
{
    private readonly ConcurrentDictionary<string, byte[]> _files;
    private readonly ConcurrentDictionary<string, DateTime> _fileTimestamps;
    private readonly HashSet<string> _directories;

    public InMemoryFileSystemProvider()
    {
        _files = new ConcurrentDictionary<string, byte[]>();
        _fileTimestamps = new ConcurrentDictionary<string, DateTime>();
        _directories = new HashSet<string>();
    }

    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!_files.TryGetValue(NormalizePath(path), out var bytes))
        {
            throw new FileNotFoundException($"File not found: {path}");
        }
        
        return Task.FromResult(Encoding.UTF8.GetString(bytes));
    }

    public Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        var normalizedPath = NormalizePath(path);
        var bytes = Encoding.UTF8.GetBytes(content);
        
        _files.AddOrUpdate(normalizedPath, bytes, (key, existing) => bytes);
        _fileTimestamps.AddOrUpdate(normalizedPath, DateTime.UtcNow, (key, existing) => DateTime.UtcNow);
        
        // Ensure directory exists
        var directory = GetDirectoryName(normalizedPath);
        if (!string.IsNullOrEmpty(directory))
        {
            _directories.Add(NormalizePath(directory));
        }
        
        return Task.CompletedTask;
    }

    public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!_files.TryGetValue(NormalizePath(path), out var bytes))
        {
            throw new FileNotFoundException($"File not found: {path}");
        }
        
        return Task.FromResult(bytes);
    }

    public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
        var normalizedPath = NormalizePath(path);
        
        _files.AddOrUpdate(normalizedPath, bytes, (key, existing) => bytes);
        _fileTimestamps.AddOrUpdate(normalizedPath, DateTime.UtcNow, (key, existing) => DateTime.UtcNow);
        
        // Ensure directory exists
        var directory = GetDirectoryName(normalizedPath);
        if (!string.IsNullOrEmpty(directory))
        {
            _directories.Add(NormalizePath(directory));
        }
        
        return Task.CompletedTask;
    }

    public Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_files.ContainsKey(NormalizePath(path)));
    }

    public Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        var normalizedPath = NormalizePath(path);
        _files.TryRemove(normalizedPath, out _);
        _fileTimestamps.TryRemove(normalizedPath, out _);
        return Task.CompletedTask;
    }

    public Task<FileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken = default)
    {
        var normalizedPath = NormalizePath(path);
        
        if (!_files.TryGetValue(normalizedPath, out var bytes))
        {
            throw new FileNotFoundException($"File not found: {path}");
        }
        
        var timestamp = _fileTimestamps.GetValueOrDefault(normalizedPath, DateTime.UtcNow);
        
        // Create a mock FileInfo (this is a simplified version)
        var fileInfo = new FileInfo(path);
        return Task.FromResult(fileInfo);
    }

    // Additional methods implementation...
    
    private string NormalizePath(string path)
    {
        return path.Replace('\\', '/').ToLowerInvariant();
    }
}
```

### FixedDateTimeProvider

Fixed date/time implementation for testing:

```csharp
public class FixedDateTimeProvider : IDateTimeProvider
{
    private DateTime _fixedDateTime;
    private DateTimeOffset _fixedDateTimeOffset;

    public FixedDateTimeProvider(DateTime fixedDateTime)
    {
        _fixedDateTime = fixedDateTime;
        _fixedDateTimeOffset = new DateTimeOffset(fixedDateTime);
    }

    public DateTime Now => _fixedDateTime;
    public DateTime UtcNow => _fixedDateTime.ToUniversalTime();
    public DateTime Today => _fixedDateTime.Date;
    public DateTimeOffset OffsetNow => _fixedDateTimeOffset;
    public DateTimeOffset OffsetUtcNow => _fixedDateTimeOffset.ToUniversalTime();

    public void SetFixedDateTime(DateTime dateTime)
    {
        _fixedDateTime = dateTime;
        _fixedDateTimeOffset = new DateTimeOffset(dateTime);
    }

    public void SetFixedDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        _fixedDateTimeOffset = dateTimeOffset;
        _fixedDateTime = dateTimeOffset.DateTime;
    }

    // Delegate parsing methods to DateTime/DateTimeOffset
    public DateTime Parse(string dateTime) => DateTime.Parse(dateTime);
    public DateTime ParseExact(string dateTime, string format, IFormatProvider provider) => DateTime.ParseExact(dateTime, format, provider);
    public bool TryParse(string dateTime, out DateTime result) => DateTime.TryParse(dateTime, out result);
    public bool TryParseExact(string dateTime, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result) => DateTime.TryParseExact(dateTime, format, provider, style, out result);

    public DateTimeOffset ParseOffset(string dateTime) => DateTimeOffset.Parse(dateTime);
    public DateTimeOffset ParseOffsetExact(string dateTime, string format, IFormatProvider provider) => DateTimeOffset.ParseExact(dateTime, format, provider);
    public bool TryParseOffset(string dateTime, out DateTimeOffset result) => DateTimeOffset.TryParse(dateTime, out result);
    public bool TryParseOffsetExact(string dateTime, string format, IFormatProvider provider, DateTimeStyles style, out DateTimeOffset result) => DateTimeOffset.TryParseExact(dateTime, format, provider, style, out result);

    public long ToUnixTimeSeconds(DateTimeOffset dateTimeOffset) => dateTimeOffset.ToUnixTimeSeconds();
    public long ToUnixTimeMilliseconds(DateTimeOffset dateTimeOffset) => dateTimeOffset.ToUnixTimeMilliseconds();
    public DateTimeOffset FromUnixTimeSeconds(long seconds) => DateTimeOffset.FromUnixTimeSeconds(seconds);
    public DateTimeOffset FromUnixTimeMilliseconds(long milliseconds) => DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
}
```

## Service Registration

### Dependency Injection Setup

```csharp
// Register infrastructure services
services.AddSingleton<IFileSystemProvider, FileSystemProvider>();
services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
services.AddSingleton<IHttpClientFactoryProvider, HttpClientFactoryProvider>();
services.AddSingleton<IConfigurationProvider, ConfigurationProvider>();

// Register with ClickUp client
services.AddClickUpClient()
    .WithInfrastructure(infrastructure => infrastructure
        .UseFileSystem<FileSystemProvider>()
        .UseDateTime<DateTimeProvider>()
        .UseHttpClientFactory<HttpClientFactoryProvider>()
        .UseConfiguration<ConfigurationProvider>());
```

### Test Setup

```csharp
// Register test implementations
services.AddSingleton<IFileSystemProvider, InMemoryFileSystemProvider>();
services.AddSingleton<IDateTimeProvider>(provider => new FixedDateTimeProvider(new DateTime(2024, 1, 1)));
services.AddSingleton<IHttpClientFactoryProvider, MockHttpClientFactoryProvider>();
services.AddSingleton<IConfigurationProvider, InMemoryConfigurationProvider>();

// Register with ClickUp client for testing
services.AddClickUpClient()
    .WithInfrastructure(infrastructure => infrastructure
        .UseFileSystem<InMemoryFileSystemProvider>()
        .UseDateTime<FixedDateTimeProvider>()
        .UseHttpClientFactory<MockHttpClientFactoryProvider>()
        .UseConfiguration<InMemoryConfigurationProvider>());
```

## Usage Examples

### File System Operations

```csharp
public class ConfigurationService
{
    private readonly IFileSystemProvider _fileSystem;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IFileSystemProvider fileSystem, ILogger<ConfigurationService> logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
    }

    public async Task<AppConfig> LoadConfigurationAsync(string configPath)
    {
        try
        {
            if (!await _fileSystem.FileExistsAsync(configPath))
            {
                _logger.LogWarning("Configuration file not found: {ConfigPath}", configPath);
                return new AppConfig(); // Return default configuration
            }

            var configJson = await _fileSystem.ReadAllTextAsync(configPath);
            return JsonSerializer.Deserialize<AppConfig>(configJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load configuration from {ConfigPath}", configPath);
            throw;
        }
    }

    public async Task SaveConfigurationAsync(string configPath, AppConfig config)
    {
        try
        {
            var configJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            await _fileSystem.WriteAllTextAsync(configPath, configJson);
            _logger.LogInformation("Configuration saved to {ConfigPath}", configPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save configuration to {ConfigPath}", configPath);
            throw;
        }
    }
}
```

### Date/Time Operations

```csharp
public class TaskSchedulingService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<TaskSchedulingService> _logger;

    public TaskSchedulingService(IDateTimeProvider dateTimeProvider, ILogger<TaskSchedulingService> logger)
    {
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<CuTask> ScheduleTaskAsync(string listId, TaskCreateRequest request, TimeSpan delay)
    {
        var scheduledTime = _dateTimeProvider.UtcNow.Add(delay);
        
        request.DueDate = scheduledTime.DateTime;
        request.StartDate = _dateTimeProvider.UtcNow.DateTime;
        
        _logger.LogInformation("Scheduling task for {ScheduledTime} (delay: {Delay})", scheduledTime, delay);
        
        // Create the task with scheduled dates
        // Implementation would use ITasksService
        return null; // Placeholder
    }

    public bool IsTaskOverdue(CuTask task)
    {
        if (task.DueDate == null) return false;
        
        var dueDate = _dateTimeProvider.Parse(task.DueDate);
        return _dateTimeProvider.UtcNow > dueDate;
    }
}
```

### HTTP Client Operations

```csharp
public class CustomApiService
{
    private readonly IHttpClientFactoryProvider _httpClientFactory;
    private readonly ILogger<CustomApiService> _logger;

    public CustomApiService(IHttpClientFactoryProvider httpClientFactory, ILogger<CustomApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<T> SendCustomRequestAsync<T>(string endpoint, object data)
    {
        using var client = _httpClientFactory.CreateClient("ClickUpApi");
        
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseJson);
    }
}
```

### Configuration Operations

```csharp
public class ClickUpClientConfiguration
{
    private readonly IConfigurationProvider _configurationProvider;
    private readonly ILogger<ClickUpClientConfiguration> _logger;

    public ClickUpClientConfiguration(IConfigurationProvider configurationProvider, ILogger<ClickUpClientConfiguration> logger)
    {
        _configurationProvider = configurationProvider;
        _logger = logger;
    }

    public string GetApiToken()
    {
        var token = _configurationProvider.GetValue<string>("ClickUp:ApiToken");
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("ClickUp API token is not configured");
        }
        return token;
    }

    public string GetBaseUrl()
    {
        return _configurationProvider.GetValue("ClickUp:BaseUrl", "https://api.clickup.com/api/v2");
    }

    public TimeSpan GetTimeout()
    {
        var timeoutSeconds = _configurationProvider.GetValue("ClickUp:TimeoutSeconds", 30);
        return TimeSpan.FromSeconds(timeoutSeconds);
    }

    public void UpdateConfiguration(string key, object value)
    {
        _configurationProvider.SetValue(key, value);
        _logger.LogInformation("Configuration updated: {Key} = {Value}", key, value);
    }
}
```

## Benefits of Infrastructure Abstraction

### Testability
- Easy to mock infrastructure dependencies in unit tests
- Deterministic behavior with fixed implementations
- Isolated testing without external dependencies

### Flexibility
- Different implementations for different environments
- Easy to swap implementations without changing business logic
- Support for cloud-native scenarios (e.g., Azure Blob Storage for file operations)

### Maintainability
- Clear separation of concerns
- Centralized infrastructure management
- Consistent patterns across the codebase

### Cross-Platform Compatibility
- Abstract away platform-specific implementations
- Support for different operating systems and environments
- Consistent behavior across platforms

## Best Practices

### Interface Design
- Keep interfaces focused and cohesive
- Use async/await patterns for I/O operations
- Include cancellation token support
- Provide both sync and async versions where appropriate

### Implementation Guidelines
- Handle exceptions gracefully
- Log important operations and errors
- Use dependency injection for configuration
- Follow the single responsibility principle

### Testing Strategies
- Create dedicated test implementations
- Use builder patterns for test data setup
- Verify behavior through interface contracts
- Test error conditions and edge cases

### Performance Considerations
- Use caching where appropriate
- Implement connection pooling for HTTP clients
- Consider memory usage for file operations
- Monitor and log performance metrics