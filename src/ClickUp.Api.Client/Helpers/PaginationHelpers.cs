using ClickUp.Api.Client.Models.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Helpers
{
    public static class PaginationHelpers
    {
        /// <summary>
        /// Asynchronously fetches all items from a paginated API endpoint that uses cursor-based pagination.
        /// </summary>
        /// <typeparam name="TItem">The type of the items being paginated.</typeparam>
        /// <typeparam name="TResponse">The type of the API response for a single page.
        /// This type must contain a property that is an IEnumerable&lt;TItem&gt; (e.g., "Items", "Data", "Docs")
        /// and a string property named "NextCursor" (case-insensitive).</typeparam>
        /// <param name="fetchPageAsync">A function that fetches a single page of data.
        /// It takes the current cursor string (null for the first page) and a CancellationToken,
        /// and returns a Task&lt;TResponse&gt;.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An IAsyncEnumerable&lt;TItem&gt; that yields items from all pages.</returns>
        /// <exception cref="ArgumentNullException">Thrown if fetchPageAsync is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if TResponse does not have a "NextCursor" property
        /// or a suitable IEnumerable&lt;TItem&gt; property.</exception>
        public static async IAsyncEnumerable<TItem> GetAllPaginatedDataAsync<TItem, TResponse>(
            Func<string?, CancellationToken, Task<TResponse?>> fetchPageAsync,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
            where TResponse : class
        {
            if (fetchPageAsync == null)
            {
                throw new ArgumentNullException(nameof(fetchPageAsync));
            }

            string? currentCursor = null;
            bool hasMore = true;

            PropertyInfo? itemsProperty = null;
            PropertyInfo? nextCursorProperty = null;

            // Reflect to find Items/Data/Docs and NextCursor/NextPageId properties once
            if (typeof(TResponse).GetProperties().Any()) // Ensure there are properties to check
            {
                // Try to find a property that is IEnumerable<TItem>
                // Common names: "Items", "Data", or pluralized TItem name e.g. "Docs" for Doc
                itemsProperty = typeof(TResponse).GetProperty(typeof(TItem).Name + "s", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                             ?? typeof(TResponse).GetProperty("Items", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                             ?? typeof(TResponse).GetProperty("Data", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (itemsProperty == null || !typeof(IEnumerable<TItem>).IsAssignableFrom(itemsProperty.PropertyType))
                {
                    throw new InvalidOperationException($"Type '{typeof(TResponse).Name}' must have a public instance property that returns IEnumerable<{typeof(TItem).Name}> (e.g., 'Items', 'Data', or '{typeof(TItem).Name}s').");
                }

                // Prefer "NextCursor", fallback to "NextPageId"
                nextCursorProperty = typeof(TResponse).GetProperty("NextCursor", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                                  ?? typeof(TResponse).GetProperty("NextPageId", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (nextCursorProperty != null && nextCursorProperty.PropertyType != typeof(string))
                {
                     // If a cursor-like property is found but it's not a string, it's an issue.
                    throw new InvalidOperationException($"Type '{typeof(TResponse).Name}' has a pagination cursor property ('{nextCursorProperty.Name}') that is not of type string.");
                }
            }


            while (hasMore)
            {
                cancellationToken.ThrowIfCancellationRequested();
                TResponse? response = await fetchPageAsync(currentCursor, cancellationToken).ConfigureAwait(false);

                if (response == null)
                {
                    hasMore = false; // No response means no more data
                    continue;
                }

                if (itemsProperty == null)
                {
                     throw new InvalidOperationException($"Items property could not be determined for type '{typeof(TResponse).Name}'. This should not happen if initial checks passed.");
                }

                var items = itemsProperty.GetValue(response) as IEnumerable<TItem>;

                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        yield return item;
                    }
                }
                else
                {
                    hasMore = false; // No items means no more data
                }

                if (hasMore)
                {
                    if (nextCursorProperty != null)
                    {
                        currentCursor = nextCursorProperty.GetValue(response) as string;
                        if (string.IsNullOrEmpty(currentCursor))
                        {
                            hasMore = false;
                        }
                    }
                    else
                    {
                        // No "NextCursor" or "NextPageId" property found on TResponse.
                        // If we received items, we must assume this is the last page as there's no way to get the next.
                        // If items were empty, hasMore is already false.
                        if (items != null && items.Any())
                        {
                            hasMore = false;
                        }
                    }
                }
            }
        }
    }
}
