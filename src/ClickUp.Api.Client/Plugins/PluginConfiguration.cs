using System;
using System.Collections.Generic;
using ClickUp.Api.Client.Abstractions.Plugins;

namespace ClickUp.Api.Client.Plugins
{
    /// <summary>
    /// Concrete implementation of plugin configuration.
    /// </summary>
    public class PluginConfiguration : IPluginConfiguration
    {
        private readonly Dictionary<string, object> _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        /// <param name="settings">The configuration settings.</param>
        public PluginConfiguration(IDictionary<string, object> settings = null)
        {
            _settings = new Dictionary<string, object>(settings ?? new Dictionary<string, object>());
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, object> Settings => _settings;

        /// <inheritdoc />
        public T GetValue<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            if (_settings.TryGetValue(key, out var value))
            {
                if (value is T directValue)
                    return directValue;

                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException($"Cannot convert value '{value}' to type '{typeof(T).Name}' for key '{key}'.", ex);
                }
            }

            return default(T);
        }

        /// <inheritdoc />
        public T GetValue<T>(string key, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key))
                return defaultValue;

            if (_settings.TryGetValue(key, out var value))
            {
                if (value is T directValue)
                    return directValue;

                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <inheritdoc />
        public bool HasKey(string key)
        {
            return !string.IsNullOrWhiteSpace(key) && _settings.ContainsKey(key);
        }

        /// <summary>
        /// Sets a configuration value.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <param name="value">The configuration value.</param>
        public void SetValue(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            _settings[key] = value;
        }

        /// <summary>
        /// Removes a configuration value.
        /// </summary>
        /// <param name="key">The configuration key to remove.</param>
        /// <returns>True if the key was removed, false if it didn't exist.</returns>
        public bool RemoveValue(string key)
        {
            return !string.IsNullOrWhiteSpace(key) && _settings.Remove(key);
        }
    }
}