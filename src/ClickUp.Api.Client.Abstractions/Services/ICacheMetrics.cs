using System;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Interface for cache metrics and monitoring.
    /// </summary>
    public interface ICacheMetrics
    {
        /// <summary>
        /// Gets the total number of cache hits.
        /// </summary>
        long HitCount { get; }

        /// <summary>
        /// Gets the total number of cache misses.
        /// </summary>
        long MissCount { get; }

        /// <summary>
        /// Gets the cache hit ratio (hits / (hits + misses)).
        /// </summary>
        double HitRatio { get; }

        /// <summary>
        /// Gets the total number of cache sets.
        /// </summary>
        long SetCount { get; }

        /// <summary>
        /// Gets the total number of cache evictions.
        /// </summary>
        long EvictionCount { get; }

        /// <summary>
        /// Gets the current number of items in the cache.
        /// </summary>
        long ItemCount { get; }

        /// <summary>
        /// Gets the estimated memory usage of the cache in bytes.
        /// </summary>
        long MemoryUsage { get; }

        /// <summary>
        /// Gets the average time taken for cache operations in milliseconds.
        /// </summary>
        double AverageOperationTime { get; }

        /// <summary>
        /// Records a cache hit.
        /// </summary>
        void RecordHit();

        /// <summary>
        /// Records a cache miss.
        /// </summary>
        void RecordMiss();

        /// <summary>
        /// Records a cache set operation.
        /// </summary>
        void RecordSet();

        /// <summary>
        /// Records a cache eviction.
        /// </summary>
        void RecordEviction();

        /// <summary>
        /// Records the time taken for a cache operation.
        /// </summary>
        /// <param name="operationTime">The operation time in milliseconds.</param>
        void RecordOperationTime(double operationTime);

        /// <summary>
        /// Resets all metrics to zero.
        /// </summary>
        void Reset();

        /// <summary>
        /// Updates the item count in the cache.
        /// </summary>
        /// <param name="itemCount">The current item count.</param>
        void UpdateItemCount(long itemCount);

        /// <summary>
        /// Updates the estimated memory usage.
        /// </summary>
        /// <param name="memoryUsage">The current memory usage in bytes.</param>
        void UpdateMemoryUsage(long memoryUsage);

        /// <summary>
        /// Gets detailed metrics as a dictionary.
        /// </summary>
        /// <returns>A dictionary containing all metric values.</returns>
        Dictionary<string, object> GetDetailedMetrics();
    }

    /// <summary>
    /// Interface for cache configuration settings.
    /// </summary>
    public interface ICacheConfiguration
    {
        /// <summary>
        /// Gets or sets the default cache expiration time.
        /// </summary>
        TimeSpan DefaultExpiration { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of items in the cache.
        /// </summary>
        int MaxCacheSize { get; set; }

        /// <summary>
        /// Gets or sets whether compression is enabled by default.
        /// </summary>
        bool EnableCompression { get; set; }

        /// <summary>
        /// Gets or sets the compression threshold in bytes.
        /// </summary>
        int CompressionThreshold { get; set; }

        /// <summary>
        /// Gets or sets the cache cleanup interval.
        /// </summary>
        TimeSpan CleanupInterval { get; set; }

        /// <summary>
        /// Gets or sets whether metrics collection is enabled.
        /// </summary>
        bool EnableMetrics { get; set; }

        /// <summary>
        /// Gets or sets the cache eviction policy.
        /// </summary>
        CacheEvictionPolicy EvictionPolicy { get; set; }

        /// <summary>
        /// Gets or sets the memory limit for the cache in bytes.
        /// </summary>
        long MemoryLimit { get; set; }

        /// <summary>
        /// Gets or sets whether distributed caching is enabled.
        /// </summary>
        bool EnableDistributedCache { get; set; }

        /// <summary>
        /// Gets or sets the distributed cache connection string.
        /// </summary>
        string? DistributedCacheConnectionString { get; set; }
    }

    /// <summary>
    /// Cache eviction policies.
    /// </summary>
    public enum CacheEvictionPolicy
    {
        /// <summary>
        /// Least Recently Used - evict the least recently accessed items.
        /// </summary>
        LRU,

        /// <summary>
        /// Least Frequently Used - evict the least frequently accessed items.
        /// </summary>
        LFU,

        /// <summary>
        /// First In, First Out - evict the oldest items.
        /// </summary>
        FIFO,

        /// <summary>
        /// Random eviction.
        /// </summary>
        Random
    }

    /// <summary>
    /// Interface for cache warming strategies.
    /// </summary>
    public interface ICacheWarmupStrategy
    {
        /// <summary>
        /// Gets the name of the warmup strategy.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the priority of this warmup strategy.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Executes the cache warmup strategy.
        /// </summary>
        /// <param name="cacheService">The cache service to warm up.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ExecuteAsync(ICacheService cacheService, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Cache statistics for monitoring and debugging.
    /// </summary>
    public class CacheStatistics
    {
        /// <summary>
        /// Gets or sets the total number of cache hits.
        /// </summary>
        public long HitCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of cache misses.
        /// </summary>
        public long MissCount { get; set; }

        /// <summary>
        /// Gets the cache hit ratio.
        /// </summary>
        public double HitRatio => HitCount + MissCount > 0 ? (double)HitCount / (HitCount + MissCount) : 0;

        /// <summary>
        /// Gets or sets the total number of cache evictions.
        /// </summary>
        public long EvictionCount { get; set; }

        /// <summary>
        /// Gets or sets the current number of items in the cache.
        /// </summary>
        public long ItemCount { get; set; }

        /// <summary>
        /// Gets or sets the estimated memory usage in bytes.
        /// </summary>
        public long MemoryUsage { get; set; }

        /// <summary>
        /// Gets or sets the average operation time in milliseconds.
        /// </summary>
        public double AverageOperationTime { get; set; }

        /// <summary>
        /// Gets or sets the cache uptime.
        /// </summary>
        public TimeSpan Uptime { get; set; }

        /// <summary>
        /// Gets or sets additional custom metrics.
        /// </summary>
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }
}