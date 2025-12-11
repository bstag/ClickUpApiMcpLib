using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Abstractions.Strategies;
using Microsoft.Extensions.Logging;

namespace ClickUp.Api.Client.Services.Caching
{
    /// <summary>
    /// Memory-based cache service implementation with advanced features including
    /// compression, metrics, cache warming, and tag-based invalidation.
    /// </summary>
    public class MemoryCacheService : ICacheService, IDisposable
    {
        private readonly ICachingStrategy _cachingStrategy;
        private readonly ILogger<MemoryCacheService>? _logger;
        private readonly ConcurrentDictionary<string, HashSet<string>> _tagIndex;
        private readonly Timer _cleanupTimer;
        private readonly CacheMetrics _metrics;
        private readonly object _lockObject = new();
        private volatile bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheService"/> class.
        /// </summary>
        /// <param name="cachingStrategy">The underlying caching strategy.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="configuration">The cache configuration.</param>
        public MemoryCacheService(
            ICachingStrategy cachingStrategy,
            ILogger<MemoryCacheService>? logger = null,
            ICacheConfiguration? configuration = null)
        {
            _cachingStrategy = cachingStrategy ?? throw new ArgumentNullException(nameof(cachingStrategy));
            _logger = logger;
            _tagIndex = new ConcurrentDictionary<string, HashSet<string>>();
            _metrics = new CacheMetrics();
            Configuration = configuration ?? new CacheConfiguration();

            // Setup cleanup timer
            _cleanupTimer = new Timer(PerformCleanup, null, 
                Configuration.CleanupInterval, Configuration.CleanupInterval);
        }

        /// <inheritdoc />
        public ICacheMetrics Metrics => _metrics;

        /// <inheritdoc />
        public ICacheConfiguration Configuration { get; set; }

        /// <inheritdoc />
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            if (_disposed || string.IsNullOrEmpty(key))
                return null;

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = await _cachingStrategy.GetAsync<CacheWrapper<T>>(key, cancellationToken).ConfigureAwait(false);
                
                if (result != null)
                {
                    _metrics.RecordHit();
                    _logger?.LogDebug("Cache hit for key: {Key}", key);
                    
                    return result.IsCompressed 
                        ? DecompressValue<T>(result.Data) 
                        : JsonSerializer.Deserialize<T>(result.Data);
                }

                _metrics.RecordMiss();
                _logger?.LogDebug("Cache miss for key: {Key}", key);
                return null;
            }
            finally
            {
                _metrics.RecordOperationTime(stopwatch.Elapsed.TotalMilliseconds);
            }
        }

        /// <inheritdoc />
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            var cached = await GetAsync<T>(key, cancellationToken).ConfigureAwait(false);
            if (cached != null)
                return cached;

            var value = await factory().ConfigureAwait(false);
            await SetAsync(key, value, new CacheOptions { Expiration = expiration }, cancellationToken).ConfigureAwait(false);
            return value;
        }

        /// <inheritdoc />
        public async Task SetAsync<T>(string key, T value, CacheOptions? options = null, CancellationToken cancellationToken = default) where T : class
        {
            if (_disposed || string.IsNullOrEmpty(key) || value == null)
                return;

            var stopwatch = Stopwatch.StartNew();
            try
            {
                options ??= new CacheOptions();
                var expiration = options.Expiration ?? Configuration.DefaultExpiration;

                // Serialize the value
                var serializedData = JsonSerializer.Serialize(value);
                var isCompressed = false;

                // Apply compression if enabled and data exceeds threshold
                if ((options.EnableCompression || Configuration.EnableCompression) && 
                    serializedData.Length > Configuration.CompressionThreshold)
                {
                    serializedData = CompressValue(serializedData);
                    isCompressed = true;
                }

                var wrapper = new CacheWrapper<T>
                {
                    Data = serializedData,
                    IsCompressed = isCompressed,
                    Priority = options.Priority,
                    Tags = options.Tags?.ToList() ?? new List<string>(),
                    CreatedAt = DateTime.UtcNow
                };

                await _cachingStrategy.SetAsync(key, wrapper, expiration, cancellationToken).ConfigureAwait(false);

                // Update tag index
                if (options.Tags != null)
                {
                    UpdateTagIndex(key, options.Tags);
                }

                _metrics.RecordSet();
                _logger?.LogDebug("Cache entry set for key: {Key}, Compressed: {IsCompressed}", key, isCompressed);
            }
            finally
            {
                _metrics.RecordOperationTime(stopwatch.Elapsed.TotalMilliseconds);
            }
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            if (_disposed || string.IsNullOrEmpty(key))
                return;

            await _cachingStrategy.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
            RemoveFromTagIndex(key);
            _metrics.RecordEviction();
            _logger?.LogDebug("Cache entry removed for key: {Key}", key);
        }

        /// <inheritdoc />
        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            if (_disposed || string.IsNullOrEmpty(pattern))
                return;

            await _cachingStrategy.RemoveByPatternAsync(pattern, cancellationToken).ConfigureAwait(false);
            _metrics.RecordEviction();
            _logger?.LogDebug("Cache entries removed by pattern: {Pattern}", pattern);
        }

        /// <inheritdoc />
        public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
        {
            if (_disposed || string.IsNullOrEmpty(tag))
                return;

            if (_tagIndex.TryGetValue(tag, out var taggedKeys))
            {
                var keysToRemove = taggedKeys.ToList();
                foreach (var key in keysToRemove)
                {
                    await RemoveAsync(key, cancellationToken).ConfigureAwait(false);
                    _metrics.RecordEviction();
                }
                _tagIndex.TryRemove(tag, out _);
                _logger?.LogDebug("Removed {Count} cache entries by tag: {Tag}", keysToRemove.Count, tag);
            }
        }

        /// <inheritdoc />
        public async Task RemoveByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
        {
            if (_disposed || tags == null)
                return;

            var keysToRemove = new HashSet<string>();
            
            foreach (var tag in tags)
            {
                if (_tagIndex.TryGetValue(tag, out var taggedKeys))
                {
                    foreach (var key in taggedKeys)
                    {
                        keysToRemove.Add(key);
                    }
                }
            }

            foreach (var key in keysToRemove)
            {
                await RemoveAsync(key, cancellationToken).ConfigureAwait(false);
            }

            _logger?.LogDebug("Removed {Count} cache entries by tags: {Tags}", keysToRemove.Count, string.Join(", ", tags));
        }

        /// <inheritdoc />
        public async Task ClearAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                return;

            await _cachingStrategy.ClearAsync(cancellationToken).ConfigureAwait(false);
            _tagIndex.Clear();
            _metrics.Reset();
            _logger?.LogDebug("Cache cleared");
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            if (_disposed || string.IsNullOrEmpty(key))
                return false;

            return await _cachingStrategy.ExistsAsync(key, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task WarmupAsync(IEnumerable<ICacheWarmupStrategy> warmupStrategies, CancellationToken cancellationToken = default)
        {
            if (_disposed || warmupStrategies == null)
                return;

            var strategies = warmupStrategies.OrderBy(s => s.Priority).ToList();
            _logger?.LogInformation("Starting cache warmup with {Count} strategies", strategies.Count);

            foreach (var strategy in strategies)
            {
                try
                {
                    _logger?.LogDebug("Executing warmup strategy: {StrategyName}", strategy.Name);
                    await strategy.ExecuteAsync(this, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error executing warmup strategy: {StrategyName}", strategy.Name);
                }
            }

            _logger?.LogInformation("Cache warmup completed");
        }

        /// <inheritdoc />
        public Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
        {
            var statistics = new CacheStatistics
            {
                HitCount = _metrics.HitCount,
                MissCount = _metrics.MissCount,
                EvictionCount = _metrics.EvictionCount,
                ItemCount = _metrics.ItemCount,
                MemoryUsage = _metrics.MemoryUsage,
                AverageOperationTime = _metrics.AverageOperationTime,
                Uptime = DateTime.UtcNow - _metrics.StartTime,
                CustomMetrics = _metrics.GetDetailedMetrics()
            };

            return Task.FromResult(statistics);
        }

        private void UpdateTagIndex(string key, IEnumerable<string> tags)
        {
            lock (_lockObject)
            {
                foreach (var tag in tags)
                {
                    _tagIndex.AddOrUpdate(tag, 
                        new HashSet<string> { key },
                        (_, existing) => { existing.Add(key); return existing; });
                }
            }
        }

        private void RemoveFromTagIndex(string key)
        {
            lock (_lockObject)
            {
                var tagsToRemove = new List<string>();
                foreach (var kvp in _tagIndex)
                {
                    kvp.Value.Remove(key);
                    if (kvp.Value.Count == 0)
                    {
                        tagsToRemove.Add(kvp.Key);
                    }
                }

                foreach (var tag in tagsToRemove)
                {
                    _tagIndex.TryRemove(tag, out _);
                }
            }
        }

        private static string CompressValue(string value)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(value);
            using var output = new System.IO.MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(bytes, 0, bytes.Length);
            }
            return Convert.ToBase64String(output.ToArray());
        }

        private static T? DecompressValue<T>(string compressedValue) where T : class
        {
            var compressedBytes = Convert.FromBase64String(compressedValue);
            using var input = new System.IO.MemoryStream(compressedBytes);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new System.IO.MemoryStream();
            gzip.CopyTo(output);
            var decompressedValue = System.Text.Encoding.UTF8.GetString(output.ToArray());
            return JsonSerializer.Deserialize<T>(decompressedValue);
        }

        private void PerformCleanup(object? state)
        {
            if (_disposed)
                return;

            try
            {
                // Cleanup logic would go here
                // For now, just log that cleanup is running
                _logger?.LogDebug("Performing cache cleanup");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error during cache cleanup");
            }
        }

        /// <summary>
        /// Disposes the cache service and releases resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _cleanupTimer?.Dispose();
            _tagIndex.Clear();
            
            if (_cachingStrategy is IDisposable disposableStrategy)
            {
                disposableStrategy.Dispose();
            }
        }
    }

    /// <summary>
    /// Wrapper class for cached values with metadata.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    public class CacheWrapper<T>
    {
        public string Data { get; set; } = string.Empty;
        public bool IsCompressed { get; set; }
        public CachePriority Priority { get; set; }
        public List<string> Tags { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}