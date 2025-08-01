using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Strategies
{
    /// <summary>
    /// Interface for caching strategies that can be used to cache API responses.
    /// </summary>
    public interface ICachingStrategy
    {
        /// <summary>
        /// Gets the name of the caching strategy.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this caching strategy is enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets a cached value by key.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The cached value if found; otherwise, null.</returns>
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Sets a value in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiration">The cache expiration time.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Removes a value from the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes all values from the cache that match the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern to match cache keys.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

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
    }
}