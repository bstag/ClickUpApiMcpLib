using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Strategies;
using Microsoft.Extensions.Logging;

namespace ClickUp.Api.Client.Strategies.Caching
{
    /// <summary>
    /// In-memory caching strategy implementation.
    /// </summary>
    public class MemoryCachingStrategy : ICachingStrategy
    {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache;
        private readonly ILogger<MemoryCachingStrategy>? _logger;
        private readonly Timer _cleanupTimer;
        private readonly TimeSpan _defaultExpiration;
        private readonly int _maxCacheSize;
        private volatile bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCachingStrategy"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="defaultExpiration">The default cache expiration time.</param>
        /// <param name="maxCacheSize">The maximum number of items to cache.</param>
        public MemoryCachingStrategy(
            ILogger<MemoryCachingStrategy>? logger = null,
            TimeSpan? defaultExpiration = null,
            int maxCacheSize = 1000)
        {
            _cache = new ConcurrentDictionary<string, CacheEntry>();
            _logger = logger;
            _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(15);
            _maxCacheSize = maxCacheSize;
            
            // Setup cleanup timer to run every 5 minutes
            _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        /// <inheritdoc />
        public string Name => "Memory";

        /// <inheritdoc />
        public bool IsEnabled => !_disposed;

        /// <inheritdoc />
        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            if (_disposed || string.IsNullOrEmpty(key))
                return Task.FromResult<T?>(null);

            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.IsExpired)
                {
                    _cache.TryRemove(key, out _);
                    _logger?.LogDebug("Cache entry expired and removed: {Key}", key);
                    return Task.FromResult<T?>(null);
                }

                _logger?.LogDebug("Cache hit: {Key}", key);
                return Task.FromResult(entry.Value as T);
            }

            _logger?.LogDebug("Cache miss: {Key}", key);
            return Task.FromResult<T?>(null);
        }

        /// <inheritdoc />
        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            if (_disposed || string.IsNullOrEmpty(key) || value == null)
                return Task.CompletedTask;

            // Enforce cache size limit
            if (_cache.Count >= _maxCacheSize)
            {
                CleanupExpiredEntries(null);
                
                // If still at capacity, remove oldest entries
                if (_cache.Count >= _maxCacheSize)
                {
                    RemoveOldestEntries(_maxCacheSize / 4); // Remove 25% of entries
                }
            }

            var expirationTime = expiration ?? _defaultExpiration;
            var entry = new CacheEntry(value, DateTime.UtcNow.Add(expirationTime));
            
            _cache.AddOrUpdate(key, entry, (k, v) => entry);
            _logger?.LogDebug("Cache entry set: {Key}, Expires: {Expiration}", key, entry.ExpiresAt);
            
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            if (_disposed || string.IsNullOrEmpty(key))
                return Task.CompletedTask;

            if (_cache.TryRemove(key, out _))
            {
                _logger?.LogDebug("Cache entry removed: {Key}", key);
            }
            
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            if (_disposed || string.IsNullOrEmpty(pattern))
                return Task.CompletedTask;

            try
            {
                var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var keysToRemove = new List<string>();
                
                foreach (var key in _cache.Keys)
                {
                    if (regex.IsMatch(key))
                    {
                        keysToRemove.Add(key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    _cache.TryRemove(key, out _);
                }
                
                _logger?.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}", keysToRemove.Count, pattern);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error removing cache entries by pattern: {Pattern}", pattern);
            }
            
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ClearAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                return Task.CompletedTask;

            var count = _cache.Count;
            _cache.Clear();
            _logger?.LogDebug("Cache cleared, removed {Count} entries", count);
            
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            if (_disposed || string.IsNullOrEmpty(key))
                return Task.FromResult(false);

            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.IsExpired)
                {
                    _cache.TryRemove(key, out _);
                    return Task.FromResult(false);
                }
                return Task.FromResult(true);
            }
            
            return Task.FromResult(false);
        }

        private void CleanupExpiredEntries(object? state)
        {
            if (_disposed)
                return;

            var expiredKeys = new List<string>();
            var now = DateTime.UtcNow;
            
            foreach (var kvp in _cache)
            {
                if (kvp.Value.ExpiresAt <= now)
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
                _logger?.LogDebug("Cleaned up {Count} expired cache entries", expiredKeys.Count);
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
            
            _logger?.LogDebug("Removed {Count} oldest cache entries to maintain size limit", Math.Min(count, entries.Count));
        }

        /// <summary>
        /// Disposes the caching strategy and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;
                
            _disposed = true;
            _cleanupTimer?.Dispose();
            _cache.Clear();
        }

        private class CacheEntry
        {
            public CacheEntry(object value, DateTime expiresAt)
            {
                Value = value;
                ExpiresAt = expiresAt;
                CreatedAt = DateTime.UtcNow;
            }

            public object Value { get; }
            public DateTime ExpiresAt { get; }
            public DateTime CreatedAt { get; }
            public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        }
    }
}