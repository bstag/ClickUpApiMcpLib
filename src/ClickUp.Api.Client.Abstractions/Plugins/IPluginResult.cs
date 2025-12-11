using System.Collections.Generic;

namespace ClickUp.Api.Client.Abstractions.Plugins
{
    /// <summary>
    /// Defines the result of plugin execution.
    /// </summary>
    public interface IPluginResult
    {
        /// <summary>
        /// Gets a value indicating whether the plugin execution was successful.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets the error message if the execution failed.
        /// </summary>
        string? ErrorMessage { get; }

        /// <summary>
        /// Gets the result data from the plugin execution.
        /// </summary>
        IReadOnlyDictionary<string, object> Data { get; }

        /// <summary>
        /// Gets any modifications that should be applied to the original request/response.
        /// </summary>
        IReadOnlyDictionary<string, object> Modifications { get; }

        /// <summary>
        /// Gets a value indicating whether the plugin execution should continue to the next plugin.
        /// </summary>
        bool ContinueExecution { get; }

        /// <summary>
        /// Gets additional metadata about the execution.
        /// </summary>
        IReadOnlyDictionary<string, object> Metadata { get; }
    }
}