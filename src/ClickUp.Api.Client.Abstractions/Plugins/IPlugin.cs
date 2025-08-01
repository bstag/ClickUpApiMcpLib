using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Plugins
{
    /// <summary>
    /// Defines the contract for ClickUp API client plugins.
    /// Implements the Open/Closed Principle by allowing extension without modification.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets the unique identifier for this plugin.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the display name of the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the version of the plugin.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets the description of what this plugin does.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets a value indicating whether this plugin is enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Initializes the plugin with the provided configuration.
        /// </summary>
        /// <param name="configuration">The plugin configuration.</param>
        /// <param name="cancellationToken">A token to cancel the initialization.</param>
        /// <returns>A task representing the asynchronous initialization operation.</returns>
        Task InitializeAsync(IPluginConfiguration configuration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the plugin's main functionality.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">A token to cancel the execution.</param>
        /// <returns>A task representing the asynchronous execution operation.</returns>
        Task<IPluginResult> ExecuteAsync(IPluginContext context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs cleanup when the plugin is being disposed.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the cleanup.</param>
        /// <returns>A task representing the asynchronous cleanup operation.</returns>
        Task CleanupAsync(CancellationToken cancellationToken = default);
    }
}