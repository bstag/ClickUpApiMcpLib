using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ClickUp.Api.Client.Abstractions.Plugins;

namespace ClickUp.Api.Client.Plugins.Samples
{
    /// <summary>
    /// Sample plugin that logs API operations for monitoring and debugging purposes.
    /// Demonstrates the Open/Closed Principle by extending functionality without modifying existing code.
    /// </summary>
    public class LoggingPlugin : BasePlugin
    {
        private bool _logRequestData;
        private bool _logResponseData;
        private LogLevel _logLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingPlugin"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public LoggingPlugin(ILogger<LoggingPlugin> logger = null)
            : base(
                id: "clickup.logging",
                name: "API Logging Plugin",
                version: new Version(1, 0, 0),
                description: "Logs API operations for monitoring and debugging purposes.",
                logger: logger)
        {
        }

        /// <inheritdoc />
        protected override Task OnInitializeAsync(IPluginConfiguration configuration, CancellationToken cancellationToken = default)
        {
            // Read configuration settings
            _logRequestData = configuration.GetValue("LogRequestData", true);
            _logResponseData = configuration.GetValue("LogResponseData", true);
            _logLevel = Enum.TryParse<LogLevel>(configuration.GetValue("LogLevel", "Information"), out var level) 
                ? level 
                : LogLevel.Information;

            Logger?.LogInformation("LoggingPlugin initialized with settings: LogRequestData={LogRequestData}, LogResponseData={LogResponseData}, LogLevel={LogLevel}",
                _logRequestData, _logResponseData, _logLevel);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task<IPluginResult> OnExecuteAsync(IPluginContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var logData = new Dictionary<string, object>
                {
                    ["Timestamp"] = DateTime.UtcNow,
                    ["OperationType"] = context.OperationType,
                    ["ServiceName"] = context.ServiceName
                };

                // Log request data if enabled
                if (_logRequestData && context.RequestData.Count > 0)
                {
                    Logger?.Log(_logLevel, "API Request - Service: {ServiceName}, Operation: {OperationType}, Data: {@RequestData}",
                        context.ServiceName, context.OperationType, context.RequestData);
                    
                    logData["RequestDataCount"] = context.RequestData.Count;
                }

                // Log response data if enabled
                if (_logResponseData && context.ResponseData.Count > 0)
                {
                    Logger?.Log(_logLevel, "API Response - Service: {ServiceName}, Operation: {OperationType}, Data: {@ResponseData}",
                        context.ServiceName, context.OperationType, context.ResponseData);
                    
                    logData["ResponseDataCount"] = context.ResponseData.Count;
                }

                // Log basic operation info
                Logger?.Log(_logLevel, "API Operation - Service: {ServiceName}, Operation: {OperationType}",
                    context.ServiceName, context.OperationType);

                return Task.FromResult<IPluginResult>(PluginResult.Success(
                    data: logData,
                    metadata: new Dictionary<string, object>
                    {
                        ["PluginType"] = "Logging",
                        ["LogLevel"] = _logLevel.ToString()
                    }));
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error in LoggingPlugin execution");
                return Task.FromResult<IPluginResult>(PluginResult.Failure($"Logging plugin error: {ex.Message}"));
            }
        }

        /// <inheritdoc />
        protected override Task OnCleanupAsync(CancellationToken cancellationToken = default)
        {
            Logger?.LogInformation("LoggingPlugin cleanup completed");
            return Task.CompletedTask;
        }
    }
}