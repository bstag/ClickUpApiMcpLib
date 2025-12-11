using System.Collections.Generic;
using ClickUp.Api.Client.Abstractions.Plugins;

namespace ClickUp.Api.Client.Plugins
{
    /// <summary>
    /// Concrete implementation of plugin execution result.
    /// </summary>
    public class PluginResult : IPluginResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginResult"/> class.
        /// </summary>
        /// <param name="isSuccess">Whether the execution was successful.</param>
        /// <param name="errorMessage">The error message if execution failed.</param>
        /// <param name="data">The result data.</param>
        /// <param name="modifications">Any modifications to apply.</param>
        /// <param name="continueExecution">Whether to continue execution to next plugins.</param>
        /// <param name="metadata">Additional metadata.</param>
        public PluginResult(
            bool isSuccess = true,
            string? errorMessage = null,
            IDictionary<string, object>? data = null,
            IDictionary<string, object>? modifications = null,
            bool continueExecution = true,
            IDictionary<string, object>? metadata = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Data = data != null ? new Dictionary<string, object>(data) : new Dictionary<string, object>();
            Modifications = modifications != null ? new Dictionary<string, object>(modifications) : new Dictionary<string, object>();
            ContinueExecution = continueExecution;
            Metadata = metadata != null ? new Dictionary<string, object>(metadata) : new Dictionary<string, object>();
        }

        /// <inheritdoc />
        public bool IsSuccess { get; }

        /// <inheritdoc />
        public string? ErrorMessage { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, object> Data { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, object> Modifications { get; }

        /// <inheritdoc />
        public bool ContinueExecution { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, object> Metadata { get; }

        /// <summary>
        /// Creates a successful plugin result.
        /// </summary>
        /// <param name="data">The result data.</param>
        /// <param name="modifications">Any modifications to apply.</param>
        /// <param name="continueExecution">Whether to continue execution to next plugins.</param>
        /// <param name="metadata">Additional metadata.</param>
        /// <returns>A successful plugin result.</returns>
        public static PluginResult Success(
            IDictionary<string, object>? data = null,
            IDictionary<string, object>? modifications = null,
            bool continueExecution = true,
            IDictionary<string, object>? metadata = null)
        {
            return new PluginResult(
                isSuccess: true,
                data: data,
                modifications: modifications,
                continueExecution: continueExecution,
                metadata: metadata);
        }

        /// <summary>
        /// Creates a failed plugin result.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="continueExecution">Whether to continue execution to next plugins.</param>
        /// <param name="metadata">Additional metadata.</param>
        /// <returns>A failed plugin result.</returns>
        public static PluginResult Failure(
            string errorMessage,
            bool continueExecution = true,
            IDictionary<string, object>? metadata = null)
        {
            return new PluginResult(
                isSuccess: false,
                errorMessage: errorMessage,
                continueExecution: continueExecution,
                metadata: metadata);
        }
    }
}