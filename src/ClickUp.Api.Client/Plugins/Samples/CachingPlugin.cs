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
    /// Sample plugin that implements caching for API responses.
    /// Demonstrates the Open/Closed Principle by adding caching functionality without modifying existing services.
    /// </summary>
    public class CachingPlugin : BasePlugin
    {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache;
        private readonly Timer _cleanupTimer;
        private TimeSpan _defaultCacheDuration;
        private int _maxCacheSize;
        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingPlugin"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public CachingPlugin(ILogger<CachingPlugin> logger = null)
            : base(
                id: "clickup.caching",
                name: "Response Caching Plugin",
                version: new Version(1, 0, 0),
                description: "Implements response caching to improve performance and reduce API calls.",
                logger: logger)
        {
            _cache = new ConcurrentDictionary<string, CacheEntry>();
            _semaphore = new SemaphoreSlim(1, 1);
            
            // Setup cleanup timer to run every 5 minutes
            _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        /// <inheritdoc />
        protected override Task OnInitializeAsync(IPluginConfiguration configuration, CancellationToken cancellationToken = default)
        {
            // Read configuration settings
            var cacheDurationMinutes = configuration.GetValue("CacheDurationMinutes", 15);
            _defaultCacheDuration = TimeSpan.FromMinutes(cacheDurationMinutes);
            _maxCacheSize = configuration.GetValue("MaxCacheSize", 1000);

            Logger?.LogInformation("CachingPlugin initialized with settings: CacheDuration={CacheDuration}, MaxCacheSize={MaxCacheSize}",
                _defaultCacheDuration, _maxCacheSize);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override async Task<IPluginResult> OnExecuteAsync(IPluginContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Only cache GET operations (read operations)
                if (!IsReadOperation(context.OperationType))
                {
                    Logger?.LogDebug("Skipping cache for non-read operation: {OperationType}", context.OperationType);
                    return PluginResult.Success();
                }

                var cacheKey = GenerateCacheKey(context);
                var currentTime = DateTime.UtcNow;

                await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    // Check if we have a valid cached entry
                    if (_cache.TryGetValue(cacheKey, out var cachedEntry) && !cachedEntry.IsExpired(currentTime))
                    {
                        Logger?.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                        
                        return PluginResult.Success(
                            data: new Dictionary<string, object>
                            {
                                ["CacheHit"] = true,
                                ["CacheKey"] = cacheKey,
                                ["CachedData"] = cachedEntry.Data,
                                ["CachedAt"] = cachedEntry.CreatedAt,
                                ["ExpiresAt"] = cachedEntry.ExpiresAt
                            },
                            modifications: new Dictionary<string, object>
                            {
                                ["UseCache"] = true,
                                ["CachedResponse"] = cachedEntry.Data
                            },
                            metadata: new Dictionary<string, object>
                            {
                                ["PluginType"] = "Caching",
                                ["CacheStatus"] = "Hit"
                            });
                    }

                    Logger?.LogDebug("Cache miss for key: {CacheKey}", cacheKey);

                    // Cache miss - we'll need to cache the response after the API call
                    // For now, just indicate that we should cache the response
                    return PluginResult.Success(
                        data: new Dictionary<string, object>
                        {
                            ["CacheHit"] = false,
                            ["CacheKey"] = cacheKey,
                            ["ShouldCache"] = true
                        },
                        modifications: new Dictionary<string, object>
                        {
                            ["CacheKey"] = cacheKey,
                            ["CacheAfterResponse"] = true
                        },
                        metadata: new Dictionary<string, object>
                        {
                            ["PluginType"] = "Caching",
                            ["CacheStatus"] = "Miss"
                        });
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error in CachingPlugin execution");
                return PluginResult.Failure($"Caching plugin error: {ex.Message}");
            }
        }

        /// <summary>
        /// Caches a response for future use.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="responseData">The response data to cache.</param>
        /// <param name="cacheDuration">Optional custom cache duration.</param>
        public async Task CacheResponseAsync(string cacheKey, object responseData, TimeSpan? cacheDuration = null)
        {
            if (string.IsNullOrWhiteSpace(cacheKey) || responseData == null)
                return;

            var duration = cacheDuration ?? _defaultCacheDuration;
            var entry = new CacheEntry(responseData, DateTime.UtcNow, duration);

            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                // Check cache size and remove oldest entries if necessary
                if (_cache.Count >= _maxCacheSize)
                {
                    RemoveOldestEntries(_cache.Count - _maxCacheSize + 1);
                }

                _cache[cacheKey] = entry;
                Logger?.LogDebug("Cached response for key: {CacheKey}, expires at: {ExpiresAt}", cacheKey, entry.ExpiresAt);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <inheritdoc />
        protected override async Task OnCleanupAsync(CancellationToken cancellationToken = default)
        {
            _cleanupTimer?.Dispose();
            _cache.Clear();
            _semaphore?.Dispose();
            
            Logger?.LogInformation("CachingPlugin cleanup completed");
            await Task.CompletedTask;
        }

        private static bool IsReadOperation(string operationType)
        {
            return operationType?.StartsWith("Get", StringComparison.OrdinalIgnoreCase) == true ||
                   operationType?.StartsWith("List", StringComparison.OrdinalIgnoreCase) == true ||
                   operationType?.StartsWith("Retrieve", StringComparison.OrdinalIgnoreCase) == true ||
                   operationType?.StartsWith("Fetch", StringComparison.OrdinalIgnoreCase) == true;
        }

        private static string GenerateCacheKey(IPluginContext context)
        {
            var keyParts = new List<string>
            {
                context.ServiceName,
                context.OperationType
            };

            // Add request parameters to the cache key
            foreach (var kvp in context.RequestData)
            {
                keyParts.Add($"{kvp.Key}:{kvp.Value}");
            }

            return string.Join("|", keyParts);
        }

        private void CleanupExpiredEntries(object state)
        {
            var currentTime = DateTime.UtcNow;
            var expiredKeys = new List<string>();

            foreach (var kvp in _cache)
            {
                if (kvp.Value.IsExpired(currentTime))
                {
                    expiredKeys.Add(kvp.Key);
                }
            }

            foreach (var key in expiredKeys)
            {
                _cache.TryRemove(key, out _);
            }

            if (expiredKeys.Count > 0)
            {
                Logger?.LogDebug("Cleaned up {ExpiredCount} expired cache entries", expiredKeys.Count);
            }
        }

        private void RemoveOldestEntries(int count)
        {
            var entries = new List<KeyValuePair<string, CacheEntry>>();
            foreach (var kvp in _cache)
            {
                entries.Add(kvp);
            }

            entries.Sort((x, y) => x.Value.CreatedAt.CompareTo(y.Value.CreatedAt));

            for (int i = 0; i < Math.Min(count, entries.Count); i++)
            {
                _cache.TryRemove(entries[i].Key, out _);
            }

            Logger?.LogDebug("Removed {RemovedCount} oldest cache entries to maintain size limit", Math.Min(count, entries.Count));
        }

        private class CacheEntry
        {
            public CacheEntry(object data, DateTime createdAt, TimeSpan duration)
            {
                Data = data;
                CreatedAt = createdAt;
                ExpiresAt = createdAt.Add(duration);
            }

            public object Data { get; }
            public DateTime CreatedAt { get; }
            public DateTime ExpiresAt { get; }

            public bool IsExpired(DateTime currentTime) => currentTime >= ExpiresAt;
        }
    }
}