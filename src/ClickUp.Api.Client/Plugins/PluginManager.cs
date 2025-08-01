using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ClickUp.Api.Client.Abstractions.Plugins;

namespace ClickUp.Api.Client.Plugins
{
    /// <summary>
    /// Concrete implementation of plugin manager.
    /// </summary>
    public class PluginManager : IPluginManager
    {
        private readonly ConcurrentDictionary<string, IPlugin> _plugins;
        private readonly ConcurrentDictionary<string, IPluginConfiguration> _configurations;
        private readonly ConcurrentDictionary<string, bool> _enabledStates;
        private readonly ILogger<PluginManager> _logger;
        private readonly SemaphoreSlim _semaphore;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManager"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public PluginManager(ILogger<PluginManager> logger = null)
        {
            _plugins = new ConcurrentDictionary<string, IPlugin>();
            _configurations = new ConcurrentDictionary<string, IPluginConfiguration>();
            _enabledStates = new ConcurrentDictionary<string, bool>();
            _logger = logger;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IPlugin> Plugins => _plugins.Values.ToList();

        /// <inheritdoc />
        public IReadOnlyCollection<IPlugin> EnabledPlugins => 
            _plugins.Values.Where(p => p.IsEnabled && _enabledStates.GetValueOrDefault(p.Id, true)).ToList();

        /// <inheritdoc />
        public async Task RegisterPluginAsync(IPlugin plugin, IPluginConfiguration configuration, CancellationToken cancellationToken = default)
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            if (string.IsNullOrWhiteSpace(plugin.Id))
                throw new ArgumentException("Plugin ID cannot be null or whitespace.", nameof(plugin));

            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_plugins.ContainsKey(plugin.Id))
                {
                    _logger?.LogWarning("Plugin with ID '{PluginId}' is already registered. Skipping registration.", plugin.Id);
                    return;
                }

                _logger?.LogInformation("Registering plugin '{PluginName}' (ID: {PluginId}, Version: {PluginVersion})", 
                    plugin.Name, plugin.Id, plugin.Version);

                // Initialize the plugin
                await plugin.InitializeAsync(configuration ?? new PluginConfiguration(), cancellationToken).ConfigureAwait(false);

                // Register the plugin
                _plugins[plugin.Id] = plugin;
                _configurations[plugin.Id] = configuration ?? new PluginConfiguration();
                _enabledStates[plugin.Id] = plugin.IsEnabled;

                _logger?.LogInformation("Successfully registered plugin '{PluginName}' (ID: {PluginId})", plugin.Name, plugin.Id);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to register plugin '{PluginName}' (ID: {PluginId})", plugin.Name, plugin.Id);
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <inheritdoc />
        public async Task UnregisterPluginAsync(string pluginId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                throw new ArgumentException("Plugin ID cannot be null or whitespace.", nameof(pluginId));

            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_plugins.TryRemove(pluginId, out var plugin))
                {
                    _logger?.LogInformation("Unregistering plugin '{PluginName}' (ID: {PluginId})", plugin.Name, pluginId);

                    try
                    {
                        await plugin.CleanupAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Error during cleanup of plugin '{PluginName}' (ID: {PluginId})", plugin.Name, pluginId);
                    }

                    _configurations.TryRemove(pluginId, out _);
                    _enabledStates.TryRemove(pluginId, out _);

                    _logger?.LogInformation("Successfully unregistered plugin '{PluginName}' (ID: {PluginId})", plugin.Name, pluginId);
                }
                else
                {
                    _logger?.LogWarning("Plugin with ID '{PluginId}' not found for unregistration.", pluginId);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <inheritdoc />
        public IPlugin GetPlugin(string pluginId)
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                return null;

            return _plugins.TryGetValue(pluginId, out var plugin) ? plugin : null;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IPluginResult>> ExecutePluginsAsync(IPluginContext context, CancellationToken cancellationToken = default)
        {
            return await ExecutePluginsAsync(context, _ => true, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IPluginResult>> ExecutePluginsAsync(IPluginContext context, Func<IPlugin, bool> filter, CancellationToken cancellationToken = default)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var results = new List<IPluginResult>();
            var enabledPlugins = EnabledPlugins.Where(filter).ToList();

            _logger?.LogDebug("Executing {PluginCount} plugins for operation '{OperationType}' in service '{ServiceName}'", 
                enabledPlugins.Count, context.OperationType, context.ServiceName);

            foreach (var plugin in enabledPlugins)
            {
                try
                {
                    _logger?.LogDebug("Executing plugin '{PluginName}' (ID: {PluginId})", plugin.Name, plugin.Id);

                    var result = await plugin.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);
                    results.Add(result);

                    _logger?.LogDebug("Plugin '{PluginName}' execution completed. Success: {IsSuccess}, Continue: {ContinueExecution}", 
                        plugin.Name, result.IsSuccess, result.ContinueExecution);

                    if (!result.ContinueExecution)
                    {
                        _logger?.LogInformation("Plugin '{PluginName}' requested to stop execution chain.", plugin.Name);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error executing plugin '{PluginName}' (ID: {PluginId})", plugin.Name, plugin.Id);
                    results.Add(PluginResult.Failure($"Plugin execution failed: {ex.Message}"));
                }
            }

            return results;
        }

        /// <inheritdoc />
        public bool EnablePlugin(string pluginId)
        {
            if (string.IsNullOrWhiteSpace(pluginId) || !_plugins.ContainsKey(pluginId))
                return false;

            _enabledStates[pluginId] = true;
            _logger?.LogInformation("Enabled plugin with ID '{PluginId}'", pluginId);
            return true;
        }

        /// <inheritdoc />
        public bool DisablePlugin(string pluginId)
        {
            if (string.IsNullOrWhiteSpace(pluginId) || !_plugins.ContainsKey(pluginId))
                return false;

            _enabledStates[pluginId] = false;
            _logger?.LogInformation("Disabled plugin with ID '{PluginId}'", pluginId);
            return true;
        }

        /// <inheritdoc />
        public async Task DisposeAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                return;

            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                _logger?.LogInformation("Disposing plugin manager and cleaning up {PluginCount} plugins", _plugins.Count);

                var cleanupTasks = _plugins.Values.Select(async plugin =>
                {
                    try
                    {
                        await plugin.CleanupAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Error during cleanup of plugin '{PluginName}' (ID: {PluginId})", plugin.Name, plugin.Id);
                    }
                });

                await Task.WhenAll(cleanupTasks).ConfigureAwait(false);

                _plugins.Clear();
                _configurations.Clear();
                _enabledStates.Clear();

                _disposed = true;
                _logger?.LogInformation("Plugin manager disposed successfully");
            }
            finally
            {
                _semaphore.Release();
                _semaphore.Dispose();
            }
        }
    }
}