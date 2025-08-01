using System.Collections.Generic;

namespace ClickUp.Api.Client.Abstractions.Infrastructure;

/// <summary>
/// Abstraction for configuration operations to support dependency inversion principle.
/// This interface provides a testable abstraction over configuration functionality,
/// allowing for easy mocking and testing of components that require configuration access.
/// </summary>
public interface IConfiguration
{
    /// <summary>
    /// Gets or sets a configuration value for the specified key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration value.</returns>
    string? this[string key] { get; set; }

    /// <summary>
    /// Gets a configuration sub-section with the specified key.
    /// </summary>
    /// <param name="key">The key of the configuration section.</param>
    /// <returns>The <see cref="IConfigurationSection"/>.</returns>
    IConfigurationSection GetSection(string key);

    /// <summary>
    /// Gets the immediate descendant configuration sub-sections.
    /// </summary>
    /// <returns>The configuration sub-sections.</returns>
    IEnumerable<IConfigurationSection> GetChildren();

    /// <summary>
    /// Returns a <see cref="IChangeToken"/> that can be used to observe when this configuration is reloaded.
    /// </summary>
    /// <returns>The <see cref="IChangeToken"/>.</returns>
    IChangeToken GetReloadToken();

    /// <summary>
    /// Gets a configuration value with the specified key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration value.</returns>
    string? GetValue(string key);

    /// <summary>
    /// Gets a configuration value with the specified key and converts it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <returns>The converted configuration value.</returns>
    T? GetValue<T>(string key);

    /// <summary>
    /// Gets a configuration value with the specified key and converts it to the specified type.
    /// If the key is not found, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The converted configuration value or the default value.</returns>
    T GetValue<T>(string key, T defaultValue);

    /// <summary>
    /// Gets a configuration value with the specified key.
    /// If the key is not found, returns the default value.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The configuration value or the default value.</returns>
    string GetValue(string key, string defaultValue);

    /// <summary>
    /// Gets the connection string with the specified name.
    /// </summary>
    /// <param name="name">The connection string name.</param>
    /// <returns>The connection string.</returns>
    string? GetConnectionString(string name);
}

/// <summary>
/// Represents a section of application configuration values.
/// </summary>
public interface IConfigurationSection : IConfiguration
{
    /// <summary>
    /// Gets the key this section occupies in its parent.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets the full path to this section within the <see cref="IConfiguration"/>.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Gets or sets the section value.
    /// </summary>
    string? Value { get; set; }

    /// <summary>
    /// Determines whether the section has a value or has children.
    /// </summary>
    /// <returns>True if the section has a value or has children; otherwise, false.</returns>
    bool Exists();
}

/// <summary>
/// Propagates notifications that a change has occurred.
/// </summary>
public interface IChangeToken
{
    /// <summary>
    /// Gets a value that indicates if a change has occurred.
    /// </summary>
    bool HasChanged { get; }

    /// <summary>
    /// Indicates if this token will pro-actively raise callbacks. If false, the token consumer must
    /// poll <see cref="HasChanged" /> to detect changes.
    /// </summary>
    bool ActiveChangeCallbacks { get; }

    /// <summary>
    /// Registers for a callback that will be invoked when the entry has changed. <see cref="HasChanged"/>
    /// MUST be set before the callback is invoked.
    /// </summary>
    /// <param name="callback">The callback to invoke.</param>
    /// <param name="state">State to be passed into the callback.</param>
    /// <returns>The <see cref="IDisposable"/> used to unregister the callback.</returns>
    System.IDisposable RegisterChangeCallback(System.Action<object?> callback, object? state);
}