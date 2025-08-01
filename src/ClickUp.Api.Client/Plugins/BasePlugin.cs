using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ClickUp.Api.Client.Abstractions.Plugins;

namespace ClickUp.Api.Client.Plugins
{
    /// <summary>
    /// Base implementation for plugins providing common functionality.
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        private bool _isInitialized;
        private bool _isEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePlugin"/> class.
        /// </summary>
        /// <param name="id">The plugin ID.</param>
        /// <param name="name">The plugin name.</param>
        /// <param name="version">The plugin version.</param>
        /// <param name="description">The plugin description.</param>
        /// <param name="logger">The logger instance.</param>
        protected BasePlugin(string id, string name, Version version, string description, ILogger? logger = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Plugin ID cannot be null or whitespace.", nameof(id));
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Plugin name cannot be null or whitespace.", nameof(name));

            Id = id;
            Name = name;
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Description = description ?? string.Empty;
            Logger = logger;
            _isEnabled = true; // Enabled by default
        }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Version Version { get; }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public bool IsEnabled => _isEnabled && _isInitialized;

        /// <summary>
        /// Gets the logger instance.
        /// </summary>
        protected ILogger? Logger { get; }

        /// <summary>
        /// Gets the plugin configuration.
        /// </summary>
        protected IPluginConfiguration? Configuration { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the plugin has been initialized.
        /// </summary>
        protected bool IsInitialized => _isInitialized;

        /// <inheritdoc />
        public virtual async Task InitializeAsync(IPluginConfiguration configuration, CancellationToken cancellationToken = default)
        {
            if (_isInitialized)
            {
                Logger?.LogWarning("Plugin '{PluginName}' (ID: {PluginId}) is already initialized.", Name, Id);
                return;
            }

            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            
            Logger?.LogInformation("Initializing plugin '{PluginName}' (ID: {PluginId}, Version: {PluginVersion})", Name, Id, Version);

            try
            {
                await OnInitializeAsync(configuration, cancellationToken).ConfigureAwait(false);
                _isInitialized = true;
                
                Logger?.LogInformation("Successfully initialized plugin '{PluginName}' (ID: {PluginId})", Name, Id);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to initialize plugin '{PluginName}' (ID: {PluginId})", Name, Id);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<IPluginResult> ExecuteAsync(IPluginContext context, CancellationToken cancellationToken = default)
        {
            if (!_isInitialized)
            {
                var errorMessage = $"Plugin '{Name}' (ID: {Id}) is not initialized.";
                Logger?.LogError(errorMessage);
                return PluginResult.Failure(errorMessage);
            }

            if (!_isEnabled)
            {
                Logger?.LogDebug("Plugin '{PluginName}' (ID: {PluginId}) is disabled. Skipping execution.", Name, Id);
                return PluginResult.Success();
            }

            if (context == null)
            {
                var errorMessage = "Plugin context cannot be null.";
                Logger?.LogError(errorMessage);
                return PluginResult.Failure(errorMessage);
            }

            Logger?.LogDebug("Executing plugin '{PluginName}' (ID: {PluginId}) for operation '{OperationType}' in service '{ServiceName}'", 
                Name, Id, context.OperationType, context.ServiceName);

            try
            {
                var result = await OnExecuteAsync(context, cancellationToken).ConfigureAwait(false);
                
                Logger?.LogDebug("Plugin '{PluginName}' execution completed. Success: {IsSuccess}", Name, result.IsSuccess);
                
                return result;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Plugin execution failed: {ex.Message}";
                Logger?.LogError(ex, "Error executing plugin '{PluginName}' (ID: {PluginId})", Name, Id);
                return PluginResult.Failure(errorMessage);
            }
        }

        /// <inheritdoc />
        public virtual async Task CleanupAsync(CancellationToken cancellationToken = default)
        {
            if (!_isInitialized)
                return;

            Logger?.LogInformation("Cleaning up plugin '{PluginName}' (ID: {PluginId})", Name, Id);

            try
            {
                await OnCleanupAsync(cancellationToken).ConfigureAwait(false);
                _isInitialized = false;
                
                Logger?.LogInformation("Successfully cleaned up plugin '{PluginName}' (ID: {PluginId})", Name, Id);
            }
            catch (Exception ex)
            {
                Logger?.LogWarning(ex, "Error during cleanup of plugin '{PluginName}' (ID: {PluginId})", Name, Id);
            }
        }

        /// <summary>
        /// Enables the plugin.
        /// </summary>
        public virtual void Enable()
        {
            _isEnabled = true;
            Logger?.LogInformation("Enabled plugin '{PluginName}' (ID: {PluginId})", Name, Id);
        }

        /// <summary>
        /// Disables the plugin.
        /// </summary>
        public virtual void Disable()
        {
            _isEnabled = false;
            Logger?.LogInformation("Disabled plugin '{PluginName}' (ID: {PluginId})", Name, Id);
        }

        /// <summary>
        /// Called during plugin initialization. Override to provide custom initialization logic.
        /// </summary>
        /// <param name="configuration">The plugin configuration.</param>
        /// <param name="cancellationToken">A token to cancel the initialization.</param>
        /// <returns>A task representing the asynchronous initialization operation.</returns>
        protected virtual Task OnInitializeAsync(IPluginConfiguration configuration, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called during plugin execution. Override to provide custom execution logic.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">A token to cancel the execution.</param>
        /// <returns>A task representing the asynchronous execution operation with the plugin result.</returns>
        protected abstract Task<IPluginResult> OnExecuteAsync(IPluginContext context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Called during plugin cleanup. Override to provide custom cleanup logic.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the cleanup.</param>
        /// <returns>A task representing the asynchronous cleanup operation.</returns>
        protected virtual Task OnCleanupAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}