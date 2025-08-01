using System;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Strategies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ClickUp.Api.Client.Strategies.Caching
{
    /// <summary>
    /// Distributed caching strategy implementation using IDistributedCache.
    /// </summary>
    public class DistributedCachingStrategy : ICachingStrategy
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<DistributedCachingStrategy>? _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly TimeSpan _defaultExpiration;
        private readonly string _keyPrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedCachingStrategy"/> class.
        /// </summary>
        /// <param name="distributedCache">The distributed cache implementation.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="defaultExpiration">The default cache expiration time.</param>
        /// <param name="keyPrefix">The prefix to add to all cache keys.</param>
        public DistributedCachingStrategy(
            IDistributedCache distributedCache,
            ILogger<DistributedCachingStrategy>? logger = null,
            TimeSpan? defaultExpiration = null,
            string keyPrefix = "clickup:")
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _logger = logger;
            _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(15);
            _keyPrefix = keyPrefix ?? string.Empty;
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        /// <inheritdoc />
        public string Name => "Distributed";

        /// <inheritdoc />
        public bool IsEnabled => true;

        /// <inheritdoc />
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            if (string.IsNullOrEmpty(key))
                return null;

            try
            {
                var fullKey = GetFullKey(key);
                var cachedBytes = await _distributedCache.GetAsync(fullKey, cancellationToken).ConfigureAwait(false);
                
                if (cachedBytes == null || cachedBytes.Length == 0)
                {
                    _logger?.LogDebug("Cache miss: {Key}", key);
                    return null;
                }

                var cachedJson = Encoding.UTF8.GetString(cachedBytes);
                var result = JsonSerializer.Deserialize<T>(cachedJson, _jsonOptions);
                
                _logger?.LogDebug("Cache hit: {Key}", key);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving from distributed cache: {Key}", key);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return;

            try
            {
                var fullKey = GetFullKey(key);
                var json = JsonSerializer.Serialize(value, _jsonOptions);
                var bytes = Encoding.UTF8.GetBytes(json);
                
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
                };

                await _distributedCache.SetAsync(fullKey, bytes, options, cancellationToken).ConfigureAwait(false);
                _logger?.LogDebug("Cache entry set: {Key}, Expires in: {Expiration}", key, options.AbsoluteExpirationRelativeToNow);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error setting distributed cache entry: {Key}", key);
            }
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key))
                return;

            try
            {
                var fullKey = GetFullKey(key);
                await _distributedCache.RemoveAsync(fullKey, cancellationToken).ConfigureAwait(false);
                _logger?.LogDebug("Cache entry removed: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error removing distributed cache entry: {Key}", key);
            }
        }

        /// <inheritdoc />
        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(pattern))
                return;

            // Note: Pattern-based removal is not directly supported by IDistributedCache
            // This is a limitation of the interface. In a real implementation, you might:
            // 1. Use Redis-specific commands if using Redis
            // 2. Maintain a separate index of keys
            // 3. Use a different caching abstraction that supports pattern operations
            
            _logger?.LogWarning("Pattern-based cache removal is not supported by IDistributedCache. Pattern: {Pattern}", pattern);
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task ClearAsync(CancellationToken cancellationToken = default)
        {
            // Note: Clear all is not directly supported by IDistributedCache
            // This is a limitation of the interface. In a real implementation, you might:
            // 1. Use Redis FLUSHDB command if using Redis
            // 2. Maintain a separate index of keys
            // 3. Use a different caching abstraction that supports clear operations
            
            _logger?.LogWarning("Clear all cache entries is not supported by IDistributedCache");
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            try
            {
                var fullKey = GetFullKey(key);
                var cachedBytes = await _distributedCache.GetAsync(fullKey, cancellationToken).ConfigureAwait(false);
                return cachedBytes != null && cachedBytes.Length > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking if distributed cache entry exists: {Key}", key);
                return false;
            }
        }

        private string GetFullKey(string key)
        {
            return $"{_keyPrefix}{key}";
        }
    }
}