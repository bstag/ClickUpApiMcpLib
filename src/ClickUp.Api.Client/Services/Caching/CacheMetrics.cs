using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using ClickUp.Api.Client.Abstractions.Services;

namespace ClickUp.Api.Client.Services.Caching
{
    /// <summary>
    /// Implementation of cache metrics for monitoring and performance analysis.
    /// </summary>
    public class CacheMetrics : ICacheMetrics
    {
        private long _hitCount;
        private long _missCount;
        private long _setCount;
        private long _evictionCount;
        private long _itemCount;
        private long _memoryUsage;
        private readonly ConcurrentQueue<double> _operationTimes;
        private readonly object _lockObject = new();
        private const int MaxOperationTimesSamples = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheMetrics"/> class.
        /// </summary>
        public CacheMetrics()
        {
            _operationTimes = new ConcurrentQueue<double>();
            StartTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the time when metrics collection started.
        /// </summary>
        public DateTime StartTime { get; }

        /// <inheritdoc />
        public long HitCount => _hitCount;

        /// <inheritdoc />
        public long MissCount => _missCount;

        /// <summary>
        /// Gets the total number of cache set operations.
        /// </summary>
        public long SetCount => _setCount;

        /// <inheritdoc />
        public double HitRatio
        {
            get
            {
                var total = _hitCount + _missCount;
                return total > 0 ? (double)_hitCount / total : 0.0;
            }
        }

        /// <inheritdoc />
        public long EvictionCount => _evictionCount;

        /// <inheritdoc />
        public long ItemCount => _itemCount;

        /// <inheritdoc />
        public long MemoryUsage => _memoryUsage;

        /// <inheritdoc />
        public double AverageOperationTime
        {
            get
            {
                if (_operationTimes.IsEmpty)
                    return 0.0;

                var sum = 0.0;
                var count = 0;
                
                foreach (var time in _operationTimes)
                {
                    sum += time;
                    count++;
                }

                return count > 0 ? sum / count : 0.0;
            }
        }

        /// <inheritdoc />
        public void RecordHit()
        {
            Interlocked.Increment(ref _hitCount);
        }

        /// <inheritdoc />
        public void RecordMiss()
        {
            Interlocked.Increment(ref _missCount);
        }

        /// <summary>
        /// Records a cache set operation.
        /// </summary>
        public void RecordSet()
        {
            Interlocked.Increment(ref _setCount);
        }

        /// <inheritdoc />
        public void RecordEviction()
        {
            Interlocked.Increment(ref _evictionCount);
            Interlocked.Decrement(ref _itemCount);
        }

        /// <inheritdoc />
        public void RecordOperationTime(double operationTime)
        {
            // Only record non-negative operation times
            if (operationTime >= 0)
            {
                _operationTimes.Enqueue(operationTime);
                
                // Keep only the most recent samples to prevent memory growth
                while (_operationTimes.Count > MaxOperationTimesSamples)
                {
                    _operationTimes.TryDequeue(out _);
                }
            }
        }

        /// <summary>
        /// Records an item addition to the cache.
        /// </summary>
        public void RecordItemAdded()
        {
            Interlocked.Increment(ref _itemCount);
        }

        /// <summary>
        /// Records an item removal from the cache.
        /// </summary>
        public void RecordItemRemoved()
        {
            Interlocked.Decrement(ref _itemCount);
        }

        /// <summary>
        /// Updates the estimated memory usage.
        /// </summary>
        /// <param name="memoryUsage">The current memory usage in bytes.</param>
        public void UpdateMemoryUsage(long memoryUsage)
        {
            Interlocked.Exchange(ref _memoryUsage, Math.Max(0, memoryUsage));
        }

        /// <summary>
        /// Updates the item count in the cache.
        /// </summary>
        /// <param name="itemCount">The current item count.</param>
        public void UpdateItemCount(long itemCount)
        {
            Interlocked.Exchange(ref _itemCount, Math.Max(0, itemCount));
        }

        /// <inheritdoc />
        public void Reset()
        {
            lock (_lockObject)
            {
                Interlocked.Exchange(ref _hitCount, 0);
                Interlocked.Exchange(ref _missCount, 0);
                Interlocked.Exchange(ref _setCount, 0);
                Interlocked.Exchange(ref _evictionCount, 0);
                Interlocked.Exchange(ref _itemCount, 0);
                Interlocked.Exchange(ref _memoryUsage, 0);
                
                // Clear operation times
                while (_operationTimes.TryDequeue(out _))
                {
                    // Continue dequeuing until empty
                }
            }
        }

        /// <inheritdoc />
        public Dictionary<string, object> GetDetailedMetrics()
        {
            var uptime = DateTime.UtcNow - StartTime;
            return new Dictionary<string, object>
            {
                ["HitCount"] = HitCount,
                ["MissCount"] = MissCount,
                ["SetCount"] = SetCount,
                ["HitRatio"] = HitRatio,
                ["EvictionCount"] = EvictionCount,
                ["ItemCount"] = ItemCount,
                ["MemoryUsage"] = MemoryUsage,
                ["AverageOperationTime"] = AverageOperationTime,
                ["StartTime"] = StartTime,
                ["Uptime"] = uptime,
                ["TotalRequests"] = HitCount + MissCount,
                ["OperationTimesSampleCount"] = _operationTimes.Count
            };
        }

        /// <summary>
        /// Gets detailed metrics as a dictionary (alias for GetDetailedMetrics for backward compatibility).
        /// </summary>
        /// <returns>A dictionary containing all metric values.</returns>
        public Dictionary<string, object> GetDetailedMetricsDictionary()
        {
            var uptime = DateTime.UtcNow - StartTime;
            return new Dictionary<string, object>
            {
                ["HitCount"] = HitCount,
                ["MissCount"] = MissCount,
                ["SetCount"] = SetCount,
                ["HitRatio"] = HitRatio,
                ["EvictionCount"] = EvictionCount,
                ["ItemCount"] = ItemCount,
                ["MemoryUsage"] = MemoryUsage,
                ["AverageOperationTime"] = AverageOperationTime,
                ["StartTime"] = StartTime,
                ["Uptime"] = uptime,
                ["TotalRequests"] = HitCount + MissCount,
                ["OperationTimesSampleCount"] = _operationTimes.Count
            };
        }
    }

    /// <summary>
    /// Implementation of cache configuration settings.
    /// </summary>
    public class CacheConfiguration : ICacheConfiguration
    {
        /// <inheritdoc />
        public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(15);

        /// <inheritdoc />
        public int MaxCacheSize { get; set; } = 1000;

        /// <inheritdoc />
        public bool EnableCompression { get; set; } = false;

        /// <inheritdoc />
        public int CompressionThreshold { get; set; } = 1024; // 1KB

        /// <inheritdoc />
        public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(5);

        /// <inheritdoc />
        public bool EnableMetrics { get; set; } = true;

        /// <inheritdoc />
        public CacheEvictionPolicy EvictionPolicy { get; set; } = CacheEvictionPolicy.LRU;

        /// <inheritdoc />
        public long MemoryLimit { get; set; } = 100 * 1024 * 1024; // 100MB

        /// <inheritdoc />
        public bool EnableDistributedCache { get; set; } = false;

        /// <inheritdoc />
        public string? DistributedCacheConnectionString { get; set; }
    }
}