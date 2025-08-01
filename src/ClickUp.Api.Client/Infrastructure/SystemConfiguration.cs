using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using ClickUp.Api.Client.Abstractions.Infrastructure;
using IConfiguration = ClickUp.Api.Client.Abstractions.Infrastructure.IConfiguration;
using IConfigurationSection = ClickUp.Api.Client.Abstractions.Infrastructure.IConfigurationSection;
using IChangeToken = ClickUp.Api.Client.Abstractions.Infrastructure.IChangeToken;
using MsConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using MsConfigurationSection = Microsoft.Extensions.Configuration.IConfigurationSection;
using MsChangeToken = Microsoft.Extensions.Primitives.IChangeToken;

namespace ClickUp.Api.Client.Infrastructure;

/// <summary>
/// Concrete implementation of IConfiguration that wraps Microsoft.Extensions.Configuration.IConfiguration.
/// This implementation provides dependency inversion by wrapping the .NET configuration infrastructure,
/// supporting DI and testability while maintaining compatibility with the standard configuration framework.
/// </summary>
public class SystemConfiguration : IConfiguration
{
    private readonly MsConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemConfiguration"/> class.
    /// </summary>
    /// <param name="configuration">The Microsoft.Extensions.Configuration.IConfiguration instance to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
    public SystemConfiguration(MsConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc />
    public string? this[string key]
    {
        get => _configuration[key];
        set => _configuration[key] = value;
    }

    /// <inheritdoc />
    public IConfigurationSection GetSection(string key)
    {
        var msSection = _configuration.GetSection(key);
        return new SystemConfigurationSection(msSection);
    }

    /// <inheritdoc />
    public IEnumerable<IConfigurationSection> GetChildren()
    {
        return _configuration.GetChildren().Select(section => new SystemConfigurationSection(section));
    }

    /// <inheritdoc />
    public IChangeToken GetReloadToken()
    {
        var msChangeToken = _configuration.GetReloadToken();
        return new SystemChangeToken(msChangeToken);
    }

    /// <inheritdoc />
    public string? GetValue(string key)
    {
        return _configuration.GetValue<string>(key);
    }

    /// <inheritdoc />
    public T? GetValue<T>(string key)
    {
        return _configuration.GetValue<T>(key);
    }

    /// <inheritdoc />
    public T GetValue<T>(string key, T defaultValue)
    {
        return _configuration.GetValue(key, defaultValue) ?? defaultValue;
    }

    /// <inheritdoc />
    public string GetValue(string key, string defaultValue)
    {
        return _configuration.GetValue(key, defaultValue);
    }

    /// <inheritdoc />
    public string? GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name);
    }
}

/// <summary>
/// Concrete implementation of IConfigurationSection that wraps Microsoft.Extensions.Configuration.IConfigurationSection.
/// </summary>
public class SystemConfigurationSection : SystemConfiguration, IConfigurationSection
{
    private readonly MsConfigurationSection _configurationSection;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemConfigurationSection"/> class.
    /// </summary>
    /// <param name="configurationSection">The Microsoft.Extensions.Configuration.IConfigurationSection instance to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when configurationSection is null.</exception>
    public SystemConfigurationSection(MsConfigurationSection configurationSection)
        : base(configurationSection)
    {
        _configurationSection = configurationSection ?? throw new ArgumentNullException(nameof(configurationSection));
    }

    /// <inheritdoc />
    public string Key => _configurationSection.Key;

    /// <inheritdoc />
    public string Path => _configurationSection.Path;

    /// <inheritdoc />
    public string? Value
    {
        get => _configurationSection.Value;
        set => _configurationSection.Value = value;
    }

    /// <inheritdoc />
    public bool Exists()
    {
        return _configurationSection.Exists();
    }
}

/// <summary>
/// Concrete implementation of IChangeToken that wraps Microsoft.Extensions.Primitives.IChangeToken.
/// </summary>
public class SystemChangeToken : IChangeToken
{
    private readonly MsChangeToken _changeToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemChangeToken"/> class.
    /// </summary>
    /// <param name="changeToken">The Microsoft.Extensions.Primitives.IChangeToken instance to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when changeToken is null.</exception>
    public SystemChangeToken(MsChangeToken changeToken)
    {
        _changeToken = changeToken ?? throw new ArgumentNullException(nameof(changeToken));
    }

    /// <inheritdoc />
    public bool HasChanged => _changeToken.HasChanged;

    /// <inheritdoc />
    public bool ActiveChangeCallbacks => _changeToken.ActiveChangeCallbacks;

    /// <inheritdoc />
    public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
    {
        return _changeToken.RegisterChangeCallback(callback, state);
    }
}