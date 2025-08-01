using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// High-level cache service interface that provides comprehensive caching capabilities
    /// including cache warming, metrics, invalidation strategies, and configuration management.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets cache statistics and metrics.
        /// </summary>
        ICacheMetrics Metrics { get; }

        /// <summary>
        /// Gets or sets the cache configuration.
        /// </summary>
        ICacheConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets a cached value by key with automatic deserialization.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The cached value if found; otherwise, null.</returns>
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Gets a cached value by key, or creates and caches it using the provided factory function.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="factory">The factory function to create the value if not cached.</param>
        /// <param name="expiration">Optional cache expiration time.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The cached or newly created value.</returns>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Sets a value in the cache with optional compression and serialization.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="options">Cache options including expiration, compression, and tags.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetAsync<T>(string key, T value, CacheOptions? options = null, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Removes a value from the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes all cached values that match the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern to match cache keys (supports wildcards).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes all cached values associated with the specified tags.
        /// </summary>
        /// <param name="tags">The tags to match for cache invalidation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default);

        /// <summary>
        /// Clears all cached values.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ClearAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a key exists in the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the key exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Warms the cache by pre-loading frequently accessed data.
        /// </summary>
        /// <param name="warmupStrategies">The cache warming strategies to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WarmupAsync(IEnumerable<ICacheWarmupStrategy> warmupStrategies, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets cache statistics for monitoring and debugging.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Cache statistics including hit/miss ratios, memory usage, etc.</returns>
        Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Cache options for controlling caching behavior.
    /// </summary>
    public class CacheOptions
    {
        /// <summary>
        /// Gets or sets the cache expiration time.
        /// </summary>
        public TimeSpan? Expiration { get; set; }

        /// <summary>
        /// Gets or sets whether to compress the cached value.
        /// </summary>
        public bool EnableCompression { get; set; } = false;

        /// <summary>
        /// Gets or sets the cache priority.
        /// </summary>
        public CachePriority Priority { get; set; } = CachePriority.Normal;

        /// <summary>
        /// Gets or sets the tags associated with this cache entry for invalidation.
        /// </summary>
        public IEnumerable<string>? Tags { get; set; }

        /// <summary>
        /// Gets or sets the sliding expiration time.
        /// </summary>
        public TimeSpan? SlidingExpiration { get; set; }
    }

    /// <summary>
    /// Cache priority levels.
    /// </summary>
    public enum CachePriority
    {
        /// <summary>
        /// Low priority - first to be evicted.
        /// </summary>
        Low,

        /// <summary>
        /// Normal priority.
        /// </summary>
        Normal,

        /// <summary>
        /// High priority - last to be evicted.
        /// </summary>
        High,

        /// <summary>
        /// Never remove from cache unless explicitly removed.
        /// </summary>
        NeverRemove
    }
}