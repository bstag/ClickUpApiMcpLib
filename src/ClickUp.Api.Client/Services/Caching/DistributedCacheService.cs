using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ClickUp.Api.Client.Services.Caching
{
    /// <summary>
    /// Distributed cache service implementation with advanced features including
    /// compression, metrics, cache warming, and tag-based invalidation.
    /// </summary>
    public class DistributedCacheService : ICacheService, IDisposable
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<DistributedCacheService>? _logger;
        private readonly ConcurrentDictionary<string, HashSet<string>> _tagIndex;
        private readonly Timer _cleanupTimer;
        private readonly CacheMetrics _metrics;
        private readonly object _lockObject = new();
        private volatile bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedCacheService"/> class.
        /// </summary>
        /// <param name="distributedCache">The distributed cache implementation.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="configuration">The cache configuration.</param>
        public DistributedCacheService(
            IDistributedCache distributedCache,
            ILogger<DistributedCacheService>? logger = null,
            ICacheConfiguration? configuration = null)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
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
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var cachedData = await _distributedCache.GetAsync(key, cancellationToken).ConfigureAwait(false);
                if (cachedData == null)
                {
                    _metrics.RecordMiss();
                    _logger?.LogDebug("Cache miss for key: {Key}", key);
                    return null;
                }

                var wrapper = JsonSerializer.Deserialize<CacheWrapper<T>>(cachedData);
                if (wrapper == null)
                {
                    _metrics.RecordMiss();
                    return null;
                }

                var result = wrapper.IsCompressed 
                    ? DecompressData<T>(wrapper.Data) 
                    : JsonSerializer.Deserialize<T>(wrapper.Data);

                _metrics.RecordHit();
                _logger?.LogDebug("Cache hit for key: {Key}, Compressed: {IsCompressed}", key, wrapper.IsCompressed);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving cache entry for key: {Key}", key);
                _metrics.RecordMiss();
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
            if (value != null)
            {
                var options = new CacheOptions
                {
                    Expiration = expiration ?? Configuration.DefaultExpiration
                };
                await SetAsync(key, value, options, cancellationToken).ConfigureAwait(false);
            }

            return value!;
        }

        /// <inheritdoc />
        public async Task SetAsync<T>(string key, T value, CacheOptions? options = null, CancellationToken cancellationToken = default) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            options ??= new CacheOptions();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var serializedData = JsonSerializer.SerializeToUtf8Bytes(value);
                var isCompressed = false;

                // Apply compression if enabled and data size exceeds threshold
                if (options.EnableCompression && serializedData.Length > Configuration.CompressionThreshold)
                {
                    serializedData = CompressData(serializedData);
                    isCompressed = true;
                }

                var wrapper = new CacheWrapper<T>
                {
                    Data = serializedData,
                    IsCompressed = isCompressed,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Tags = options.Tags?.ToList() ?? new List<string>()
                };

                var wrapperData = JsonSerializer.SerializeToUtf8Bytes(wrapper);
                var distributedCacheOptions = new DistributedCacheEntryOptions();

                if (options.Expiration.HasValue)
                {
                    distributedCacheOptions.SetAbsoluteExpiration(options.Expiration.Value);
                }
                else if (Configuration.DefaultExpiration != TimeSpan.Zero)
                {
                    distributedCacheOptions.SetAbsoluteExpiration(Configuration.DefaultExpiration);
                }

                await _distributedCache.SetAsync(key, wrapperData, distributedCacheOptions, cancellationToken).ConfigureAwait(false);

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
            if (string.IsNullOrEmpty(key))
                return;

            await _distributedCache.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
            RemoveFromTagIndex(key);
            _metrics.RecordEviction();
            _logger?.LogDebug("Cache entry removed for key: {Key}", key);
        }

        /// <inheritdoc />
        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            // Note: IDistributedCache doesn't support pattern-based removal natively
            // This would require a custom implementation or Redis-specific commands
            _logger?.LogWarning("Pattern-based removal not supported in distributed cache: {Pattern}", pattern);
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
        {
            if (_tagIndex.TryGetValue(tag, out var keys))
            {
                var tasks = keys.Select(key => RemoveAsync(key, cancellationToken));
                await Task.WhenAll(tasks).ConfigureAwait(false);
                _tagIndex.TryRemove(tag, out _);
            }
        }

        /// <inheritdoc />
        public async Task RemoveByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
        {
            var tasks = tags.Select(tag => RemoveByTagAsync(tag, cancellationToken));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task ClearAsync(CancellationToken cancellationToken = default)
        {
            // Note: IDistributedCache doesn't support clearing all entries natively
            // This would require a custom implementation or Redis-specific commands
            _tagIndex.Clear();
            _logger?.LogWarning("Clear operation not fully supported in distributed cache");
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            var data = await _distributedCache.GetAsync(key, cancellationToken).ConfigureAwait(false);
            return data != null;
        }

        /// <inheritdoc />
        public async Task WarmupAsync(IEnumerable<ICacheWarmupStrategy> warmupStrategies, CancellationToken cancellationToken = default)
        {
            var tasks = warmupStrategies.Select(strategy => strategy.ExecuteAsync(this, cancellationToken));
            await Task.WhenAll(tasks).ConfigureAwait(false);
            _logger?.LogInformation("Cache warmup completed with {Count} strategies", warmupStrategies.Count());
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

        private static byte[] CompressData(byte[] data)
        {
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        private static T? DecompressData<T>(byte[] compressedData) where T : class
        {
            using var input = new MemoryStream(compressedData);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            gzip.CopyTo(output);
            var decompressedData = output.ToArray();
            return JsonSerializer.Deserialize<T>(decompressedData);
        }

        private void PerformCleanup(object? state)
        {
            if (_disposed)
                return;

            try
            {
                // Distributed cache cleanup is typically handled by the underlying store
                // We only clean up our local tag index of expired entries
                _logger?.LogDebug("Performing distributed cache cleanup");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error during cache cleanup");
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="DistributedCacheService"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _cleanupTimer?.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// Cache wrapper for storing metadata with cached data.
        /// </summary>
        private class CacheWrapper<T>
        {
            public byte[] Data { get; set; } = Array.Empty<byte>();
            public bool IsCompressed { get; set; }
            public DateTimeOffset CreatedAt { get; set; }
            public List<string> Tags { get; set; } = new();
        }
    }
}