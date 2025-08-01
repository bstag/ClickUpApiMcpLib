using System.Collections.Generic;

namespace ClickUp.Api.Client.Abstractions.Plugins
{
    /// <summary>
    /// Defines the contract for plugin configuration.
    /// </summary>
    public interface IPluginConfiguration
    {
        /// <summary>
        /// Gets the configuration settings as key-value pairs.
        /// </summary>
        IReadOnlyDictionary<string, object> Settings { get; }

        /// <summary>
        /// Gets a configuration value by key.
        /// </summary>
        /// <typeparam name="T">The type of the configuration value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value, or default if not found.</returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Gets a configuration value by key with a default value.
        /// </summary>
        /// <typeparam name="T">The type of the configuration value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value to return if key is not found.</param>
        /// <returns>The configuration value, or the default value if not found.</returns>
        T GetValue<T>(string key, T defaultValue);

        /// <summary>
        /// Checks if a configuration key exists.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        bool HasKey(string key);

        /// <summary>
        /// Sets a configuration value.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <param name="value">The configuration value.</param>
        void SetValue(string key, object value);
    }
}