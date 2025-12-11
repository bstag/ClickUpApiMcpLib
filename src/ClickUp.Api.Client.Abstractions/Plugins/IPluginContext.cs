using System;
using System.Collections.Generic;
using ClickUp.Api.Client.Abstractions.Http;

namespace ClickUp.Api.Client.Abstractions.Plugins
{
    /// <summary>
    /// Defines the execution context for plugins.
    /// </summary>
    public interface IPluginContext
    {
        /// <summary>
        /// Gets the API connection instance.
        /// </summary>
        IApiConnection ApiConnection { get; }

        /// <summary>
        /// Gets the request data associated with the current operation.
        /// </summary>
        IReadOnlyDictionary<string, object> RequestData { get; }

        /// <summary>
        /// Gets the response data from the current operation (if available).
        /// </summary>
        IReadOnlyDictionary<string, object> ResponseData { get; }

        /// <summary>
        /// Gets the operation type being performed.
        /// </summary>
        string OperationType { get; }

        /// <summary>
        /// Gets the service name that initiated the plugin execution.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Gets additional context data.
        /// </summary>
        IReadOnlyDictionary<string, object> AdditionalData { get; }

        /// <summary>
        /// Gets a value from the context data.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The value, or default if not found.</returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Gets a value from the context data with a default value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="key">The key to retrieve.</param>
        /// <param name="defaultValue">The default value if key is not found.</param>
        /// <returns>The value, or the default value if not found.</returns>
        T GetValue<T>(string key, T defaultValue);
    }
}