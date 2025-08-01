using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ClickUp.Api.Client.Abstractions.Plugins;

namespace ClickUp.Api.Client.Plugins.Samples
{
    /// <summary>
    /// Sample plugin that implements rate limiting for API operations.
    /// Demonstrates the Open/Closed Principle by adding rate limiting functionality without modifying existing services.
    /// </summary>
    public class RateLimitingPlugin : BasePlugin
    {
        private readonly ConcurrentDictionary<string, Queue<DateTime>> _requestHistory;
        private readonly SemaphoreSlim _semaphore;
        private int _maxRequestsPerMinute;
        private TimeSpan _timeWindow;
        private bool _blockExcessRequests;

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitingPlugin"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public RateLimitingPlugin(ILogger<RateLimitingPlugin>? logger = null)
            : base(
                id: "clickup.ratelimiting",
                name: "Rate Limiting Plugin",
                version: new Version(1, 0, 0),
                description: "Implements rate limiting for API operations to prevent exceeding API quotas.",
                logger: logger)
        {
            _requestHistory = new ConcurrentDictionary<string, Queue<DateTime>>();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        /// <inheritdoc />
        protected override Task OnInitializeAsync(IPluginConfiguration configuration, CancellationToken cancellationToken = default)
        {
            // Read configuration settings
            _maxRequestsPerMinute = configuration.GetValue("MaxRequestsPerMinute", 100);
            var timeWindowMinutes = configuration.GetValue("TimeWindowMinutes", 1);
            _timeWindow = TimeSpan.FromMinutes(timeWindowMinutes);
            _blockExcessRequests = configuration.GetValue("BlockExcessRequests", true);

            Logger?.LogInformation("RateLimitingPlugin initialized with settings: MaxRequestsPerMinute={MaxRequestsPerMinute}, TimeWindow={TimeWindow}, BlockExcessRequests={BlockExcessRequests}",
                _maxRequestsPerMinute, _timeWindow, _blockExcessRequests);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override async Task<IPluginResult> OnExecuteAsync(IPluginContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var operationKey = $"{context.ServiceName}:{context.OperationType}";
                var currentTime = DateTime.UtcNow;

                await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    // Get or create request history for this operation
                    var requestQueue = _requestHistory.GetOrAdd(operationKey, _ => new Queue<DateTime>());

                    // Remove old requests outside the time window
                    while (requestQueue.Count > 0 && currentTime - requestQueue.Peek() > _timeWindow)
                    {
                        requestQueue.Dequeue();
                    }

                    // Check if we're within the rate limit
                    if (requestQueue.Count >= _maxRequestsPerMinute)
                    {
                        var oldestRequest = requestQueue.Peek();
                        var waitTime = _timeWindow - (currentTime - oldestRequest);

                        Logger?.LogWarning("Rate limit exceeded for operation '{OperationKey}'. Current requests: {CurrentRequests}, Limit: {MaxRequests}",
                            operationKey, requestQueue.Count, _maxRequestsPerMinute);

                        if (_blockExcessRequests)
                        {
                            return PluginResult.Failure(
                                $"Rate limit exceeded for {operationKey}. Please wait {waitTime.TotalSeconds:F1} seconds.",
                                continueExecution: false,
                                metadata: new Dictionary<string, object>
                                {
                                    ["RateLimitExceeded"] = true,
                                    ["WaitTimeSeconds"] = waitTime.TotalSeconds,
                                    ["CurrentRequests"] = requestQueue.Count,
                                    ["MaxRequests"] = _maxRequestsPerMinute
                                });
                        }
                        else
                        {
                            Logger?.LogWarning("Rate limit exceeded but allowing request to continue for operation '{OperationKey}'", operationKey);
                        }
                    }

                    // Add current request to history
                    requestQueue.Enqueue(currentTime);

                    Logger?.LogDebug("Rate limit check passed for operation '{OperationKey}'. Current requests: {CurrentRequests}/{MaxRequests}",
                        operationKey, requestQueue.Count, _maxRequestsPerMinute);

                    return PluginResult.Success(
                        data: new Dictionary<string, object>
                        {
                            ["OperationKey"] = operationKey,
                            ["CurrentRequests"] = requestQueue.Count,
                            ["MaxRequests"] = _maxRequestsPerMinute,
                            ["RemainingRequests"] = Math.Max(0, _maxRequestsPerMinute - requestQueue.Count)
                        },
                        metadata: new Dictionary<string, object>
                        {
                            ["PluginType"] = "RateLimiting",
                            ["TimeWindow"] = _timeWindow.ToString()
                        });
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error in RateLimitingPlugin execution");
                return PluginResult.Failure($"Rate limiting plugin error: {ex.Message}");
            }
        }

        /// <inheritdoc />
        protected override async Task OnCleanupAsync(CancellationToken cancellationToken = default)
        {
            _requestHistory.Clear();
            _semaphore?.Dispose();
            
            Logger?.LogInformation("RateLimitingPlugin cleanup completed");
            await Task.CompletedTask;
        }
    }
}