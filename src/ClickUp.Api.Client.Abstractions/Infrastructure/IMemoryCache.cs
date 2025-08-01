using System;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Infrastructure
{
    /// <summary>
    /// Abstraction for memory caching operations to support dependency inversion principle.
    /// This interface allows for easier testing and different caching implementations.
    /// </summary>
    public interface IMemoryCache
    {
        /// <summary>
        /// Gets the value associated with this key if present.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The located value or null.</param>
        /// <returns>True if the key was found.</returns>
        bool TryGetValue(object key, out object value);

        /// <summary>
        /// Gets the value associated with this key if present.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The located value or default.</param>
        /// <returns>True if the key was found.</returns>
        bool TryGetValue<T>(object key, out T value);

        /// <summary>
        /// Gets the value associated with this key if it exists, or creates and caches the value.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="factory">The factory function to create the value if it doesn't exist.</param>
        /// <returns>The cached or newly created value.</returns>
        T GetOrCreate<T>(object key, Func<ICacheEntry, T> factory);

        /// <summary>
        /// Gets the value associated with this key if it exists, or creates and caches the value asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="factory">The factory function to create the value if it doesn't exist.</param>
        /// <returns>The cached or newly created value.</returns>
        Task<T> GetOrCreateAsync<T>(object key, Func<ICacheEntry, Task<T>> factory);

        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <returns>The newly created cache entry.</returns>
        ICacheEntry CreateEntry(object key);

        /// <summary>
        /// Sets a cache entry with the given key and value.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="absoluteExpiration">The absolute expiration time.</param>
        void Set(object key, object value, DateTimeOffset absoluteExpiration);

        /// <summary>
        /// Sets a cache entry with the given key and value.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="slidingExpiration">The sliding expiration time.</param>
        void Set(object key, object value, TimeSpan slidingExpiration);

        /// <summary>
        /// Sets a cache entry with the given key and value.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="options">The cache entry options.</param>
        void Set(object key, object value, MemoryCacheEntryOptions options);

        /// <summary>
        /// Sets a cache entry with the given key and value.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        void Set(object key, object value);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        void Remove(object key);

        /// <summary>
        /// Gets the value associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <returns>The located value or null.</returns>
        object Get(object key);

        /// <summary>
        /// Gets the value associated with the given key.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <returns>The located value or default.</returns>
        T Get<T>(object key);

        /// <summary>
        /// Clears all entries from the cache.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Represents an entry in the memory cache.
    /// </summary>
    public interface ICacheEntry : IDisposable
    {
        /// <summary>
        /// Gets the key of the cache entry.
        /// </summary>
        object Key { get; }

        /// <summary>
        /// Gets or sets the value of the cache entry.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Gets or sets an absolute expiration date for the cache entry.
        /// </summary>
        DateTimeOffset? AbsoluteExpiration { get; set; }

        /// <summary>
        /// Gets or sets an absolute expiration time, relative to now.
        /// </summary>
        TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

        /// <summary>
        /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before it will be removed.
        /// This will not extend the entry lifetime beyond the absolute expiration (if set).
        /// </summary>
        TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// Gets or sets the priority for keeping the cache entry in the cache during a memory pressure triggered cleanup.
        /// </summary>
        CacheItemPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the size of the cache entry value.
        /// </summary>
        long? Size { get; set; }
    }

    /// <summary>
    /// Options for configuring memory cache entries.
    /// </summary>
    public class MemoryCacheEntryOptions
    {
        /// <summary>
        /// Gets or sets an absolute expiration date for the cache entry.
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration { get; set; }

        /// <summary>
        /// Gets or sets an absolute expiration time, relative to now.
        /// </summary>
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

        /// <summary>
        /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before it will be removed.
        /// This will not extend the entry lifetime beyond the absolute expiration (if set).
        /// </summary>
        public TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// Gets or sets the priority for keeping the cache entry in the cache during a memory pressure triggered cleanup.
        /// </summary>
        public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;

        /// <summary>
        /// Gets or sets the size of the cache entry value.
        /// </summary>
        public long? Size { get; set; }
    }

    /// <summary>
    /// Specifies how items are prioritized for preservation during a memory pressure triggered cleanup.
    /// </summary>
    public enum CacheItemPriority
    {
        /// <summary>
        /// Items with this priority level will be the first to be removed from the cache.
        /// </summary>
        Low,

        /// <summary>
        /// Items with this priority level will be removed from the cache after those with Low priority.
        /// This is the default.
        /// </summary>
        Normal,

        /// <summary>
        /// Items with this priority level will be removed from the cache after those with Normal priority.
        /// </summary>
        High,

        /// <summary>
        /// Items with this priority level will not be removed from the cache.
        /// </summary>
        NeverRemove
    }
}