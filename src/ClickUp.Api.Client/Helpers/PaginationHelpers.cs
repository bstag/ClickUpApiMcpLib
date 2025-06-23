using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Helpers;

/// <summary>
/// Provides helper methods for handling paginated API responses.
/// </summary>
public static class PaginationHelpers
{
    /// <summary>
    /// Asynchronously retrieves all pages of data from a cursor-paginated endpoint.
    /// </summary>
    /// <typeparam name="TItem">The type of the items in the paginated response.</typeparam>
    /// <typeparam name="TResponse">The type of the API response, which must contain the items and a next cursor.</typeparam>
    /// <param name="apiCallFunc">
    /// A function that takes a cursor string (nullable for the first call) and a CancellationToken,
    /// and returns a Task of TResponse. TResponse must have a property named "NextCursor" (string)
    /// and a property named "Data" or "Items" (IEnumerable&lt;TItem&gt;).
    /// This will be dynamically checked. A more robust implementation might use an interface for TResponse.
    /// </param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>An IAsyncEnumerable of TItem, allowing asynchronous iteration over all items from all pages.</returns>
    /// <exception cref="ArgumentException">Thrown if TResponse does not conform to the expected structure (missing NextCursor or data property).</exception>
    public static async IAsyncEnumerable<TItem> GetAllPaginatedDataAsync<TItem, TResponse>(
        Func<string?, CancellationToken, Task<TResponse?>> apiCallFunc,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default) where TResponse : class
    {
        string? currentCursor = null;

        // Validate TResponse structure once using reflection (or use an interface)
        var responseType = typeof(TResponse);
        var nextCursorProp = responseType.GetProperty("NextCursor") ?? responseType.GetProperty("NextPageId"); // Added NextPageId
        var dataProp = responseType.GetProperty("Data") ?? responseType.GetProperty("Items") ?? responseType.GetProperty(typeof(TItem).Name + "s"); // Common naming conventions

        if (nextCursorProp == null || nextCursorProp.PropertyType != typeof(string))
        {
            throw new ArgumentException($"Type {responseType.Name} must have a public string property named 'NextCursor' or 'NextPageId'.");
        }

        if (dataProp == null || !typeof(IEnumerable<TItem>).IsAssignableFrom(dataProp.PropertyType))
        {
            throw new ArgumentException($"Type {responseType.Name} must have a public property assignable to IEnumerable<{typeof(TItem).Name}> (e.g., 'Data', 'Items', or '{typeof(TItem).Name}s').");
        }

        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            TResponse? response = await apiCallFunc(currentCursor, cancellationToken).ConfigureAwait(false);

            if (response == null)
            {
                yield break; // No response, stop pagination
            }

            var items = dataProp.GetValue(response) as IEnumerable<TItem>;
            if (items != null)
            {
                foreach (var item in items)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return item;
                }
            }

            currentCursor = nextCursorProp.GetValue(response) as string;

        } while (!string.IsNullOrEmpty(currentCursor));
    }
}
