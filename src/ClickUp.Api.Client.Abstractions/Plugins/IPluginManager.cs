using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Plugins
{
    /// <summary>
    /// Defines the contract for managing plugins in the ClickUp API client.
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// Gets all registered plugins.
        /// </summary>
        IReadOnlyCollection<IPlugin> Plugins { get; }

        /// <summary>
        /// Gets all enabled plugins.
        /// </summary>
        IReadOnlyCollection<IPlugin> EnabledPlugins { get; }

        /// <summary>
        /// Registers a plugin with the manager.
        /// </summary>
        /// <param name="plugin">The plugin to register.</param>
        /// <param name="configuration">The plugin configuration.</param>
        /// <param name="cancellationToken">A token to cancel the registration.</param>
        /// <returns>A task representing the asynchronous registration operation.</returns>
        Task RegisterPluginAsync(IPlugin plugin, IPluginConfiguration configuration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unregisters a plugin from the manager.
        /// </summary>
        /// <param name="pluginId">The ID of the plugin to unregister.</param>
        /// <param name="cancellationToken">A token to cancel the unregistration.</param>
        /// <returns>A task representing the asynchronous unregistration operation.</returns>
        Task UnregisterPluginAsync(string pluginId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a plugin by its ID.
        /// </summary>
        /// <param name="pluginId">The plugin ID.</param>
        /// <returns>The plugin if found, null otherwise.</returns>
        IPlugin GetPlugin(string pluginId);

        /// <summary>
        /// Executes all enabled plugins for a specific operation.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">A token to cancel the execution.</param>
        /// <returns>A task representing the asynchronous execution operation with results from all plugins.</returns>
        Task<IEnumerable<IPluginResult>> ExecutePluginsAsync(IPluginContext context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes plugins that match a specific filter.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="filter">A function to filter which plugins to execute.</param>
        /// <param name="cancellationToken">A token to cancel the execution.</param>
        /// <returns>A task representing the asynchronous execution operation with results from filtered plugins.</returns>
        Task<IEnumerable<IPluginResult>> ExecutePluginsAsync(IPluginContext context, Func<IPlugin, bool> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables a plugin.
        /// </summary>
        /// <param name="pluginId">The plugin ID.</param>
        /// <returns>True if the plugin was enabled, false if not found.</returns>
        bool EnablePlugin(string pluginId);

        /// <summary>
        /// Disables a plugin.
        /// </summary>
        /// <param name="pluginId">The plugin ID.</param>
        /// <returns>True if the plugin was disabled, false if not found.</returns>
        bool DisablePlugin(string pluginId);

        /// <summary>
        /// Disposes all plugins and cleans up resources.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the cleanup.</param>
        /// <returns>A task representing the asynchronous cleanup operation.</returns>
        Task DisposeAsync(CancellationToken cancellationToken = default);
    }
}