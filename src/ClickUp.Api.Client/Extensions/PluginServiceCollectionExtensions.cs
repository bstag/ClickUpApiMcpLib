using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ClickUp.Api.Client.Abstractions.Plugins;
using ClickUp.Api.Client.Plugins;
using ClickUp.Api.Client.Plugins.Samples;

namespace ClickUp.Api.Client.Extensions
{
    /// <summary>
    /// Extension methods for configuring ClickUp API client plugins.
    /// </summary>
    public static class PluginServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures a plugin to the ClickUp API client.
        /// </summary>
        /// <typeparam name="TPlugin">The type of plugin to add.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="configurePlugin">Optional action to configure the plugin.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddClickUpPlugin<TPlugin>(
            this IServiceCollection services,
            Action<IPluginConfiguration> configurePlugin = null)
            where TPlugin : class, IPlugin
        {
            services.AddTransient<TPlugin>();
            
            if (configurePlugin != null)
            {
                services.Configure<Dictionary<string, IPluginConfiguration>>(options =>
                {
                    var config = new PluginConfiguration();
                    configurePlugin(config);
                    options[typeof(TPlugin).Name] = config;
                });
            }
            
            return services;
        }

        /// <summary>
        /// Adds the logging plugin with configuration.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="logRequestData">Whether to log request data.</param>
        /// <param name="logResponseData">Whether to log response data.</param>
        /// <param name="logLevel">The log level to use.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddClickUpLoggingPlugin(
            this IServiceCollection services,
            bool logRequestData = true,
            bool logResponseData = true,
            LogLevel logLevel = LogLevel.Information)
        {
            return services.AddClickUpPlugin<LoggingPlugin>(config =>
            {
                config.SetValue("LogRequestData", logRequestData);
                config.SetValue("LogResponseData", logResponseData);
                config.SetValue("LogLevel", logLevel.ToString());
            });
        }

        /// <summary>
        /// Adds the rate limiting plugin with configuration.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="maxRequestsPerMinute">Maximum requests per minute.</param>
        /// <param name="timeWindowMinutes">Time window in minutes.</param>
        /// <param name="blockExcessRequests">Whether to block excess requests.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddClickUpRateLimitingPlugin(
            this IServiceCollection services,
            int maxRequestsPerMinute = 100,
            int timeWindowMinutes = 1,
            bool blockExcessRequests = true)
        {
            return services.AddClickUpPlugin<RateLimitingPlugin>(config =>
            {
                config.SetValue("MaxRequestsPerMinute", maxRequestsPerMinute);
                config.SetValue("TimeWindowMinutes", timeWindowMinutes);
                config.SetValue("BlockExcessRequests", blockExcessRequests);
            });
        }

        /// <summary>
        /// Adds the caching plugin with configuration.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="cacheDurationMinutes">Cache duration in minutes.</param>
        /// <param name="maxCacheSize">Maximum number of cached items.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddClickUpCachingPlugin(
            this IServiceCollection services,
            int cacheDurationMinutes = 15,
            int maxCacheSize = 1000)
        {
            return services.AddClickUpPlugin<CachingPlugin>(config =>
            {
                config.SetValue("CacheDurationMinutes", cacheDurationMinutes);
                config.SetValue("MaxCacheSize", maxCacheSize);
            });
        }

        /// <summary>
        /// Configures the plugin manager with initial plugins.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configurePlugins">Action to configure plugins.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection ConfigureClickUpPlugins(
            this IServiceCollection services,
            Action<IPluginManagerBuilder> configurePlugins)
        {
            if (configurePlugins == null)
                throw new ArgumentNullException(nameof(configurePlugins));

            var builder = new PluginManagerBuilder(services);
            configurePlugins(builder);
            
            return services;
        }
    }

    /// <summary>
    /// Builder for configuring plugin manager.
    /// </summary>
    public interface IPluginManagerBuilder
    {
        /// <summary>
        /// Adds a plugin to the manager.
        /// </summary>
        /// <typeparam name="TPlugin">The plugin type.</typeparam>
        /// <param name="configurePlugin">Optional plugin configuration.</param>
        /// <returns>The builder for chaining.</returns>
        IPluginManagerBuilder AddPlugin<TPlugin>(Action<IPluginConfiguration> configurePlugin = null)
            where TPlugin : class, IPlugin;

        /// <summary>
        /// Adds the logging plugin.
        /// </summary>
        /// <param name="logRequestData">Whether to log request data.</param>
        /// <param name="logResponseData">Whether to log response data.</param>
        /// <param name="logLevel">The log level to use.</param>
        /// <returns>The builder for chaining.</returns>
        IPluginManagerBuilder AddLoggingPlugin(
            bool logRequestData = true,
            bool logResponseData = true,
            LogLevel logLevel = LogLevel.Information);

        /// <summary>
        /// Adds the rate limiting plugin.
        /// </summary>
        /// <param name="maxRequestsPerMinute">Maximum requests per minute.</param>
        /// <param name="timeWindowMinutes">Time window in minutes.</param>
        /// <param name="blockExcessRequests">Whether to block excess requests.</param>
        /// <returns>The builder for chaining.</returns>
        IPluginManagerBuilder AddRateLimitingPlugin(
            int maxRequestsPerMinute = 100,
            int timeWindowMinutes = 1,
            bool blockExcessRequests = true);

        /// <summary>
        /// Adds the caching plugin.
        /// </summary>
        /// <param name="cacheDurationMinutes">Cache duration in minutes.</param>
        /// <param name="maxCacheSize">Maximum number of cached items.</param>
        /// <returns>The builder for chaining.</returns>
        IPluginManagerBuilder AddCachingPlugin(
            int cacheDurationMinutes = 15,
            int maxCacheSize = 1000);
    }

    /// <summary>
    /// Implementation of plugin manager builder.
    /// </summary>
    internal class PluginManagerBuilder : IPluginManagerBuilder
    {
        private readonly IServiceCollection _services;

        public PluginManagerBuilder(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IPluginManagerBuilder AddPlugin<TPlugin>(Action<IPluginConfiguration> configurePlugin = null)
            where TPlugin : class, IPlugin
        {
            _services.AddClickUpPlugin<TPlugin>(configurePlugin);
            return this;
        }

        public IPluginManagerBuilder AddLoggingPlugin(
            bool logRequestData = true,
            bool logResponseData = true,
            LogLevel logLevel = LogLevel.Information)
        {
            _services.AddClickUpLoggingPlugin(logRequestData, logResponseData, logLevel);
            return this;
        }

        public IPluginManagerBuilder AddRateLimitingPlugin(
            int maxRequestsPerMinute = 100,
            int timeWindowMinutes = 1,
            bool blockExcessRequests = true)
        {
            _services.AddClickUpRateLimitingPlugin(maxRequestsPerMinute, timeWindowMinutes, blockExcessRequests);
            return this;
        }

        public IPluginManagerBuilder AddCachingPlugin(
            int cacheDurationMinutes = 15,
            int maxCacheSize = 1000)
        {
            _services.AddClickUpCachingPlugin(cacheDurationMinutes, maxCacheSize);
            return this;
        }
    }
}