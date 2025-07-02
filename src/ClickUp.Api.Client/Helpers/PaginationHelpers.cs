// src/ClickUp.Api.Client/Helpers/PaginationHelpers.cs
using ClickUp.Api.Client.Models.Common.Pagination;
using System.Collections.Generic;
using System.Linq; // Required for .ToList() and .AsReadOnly()

namespace ClickUp.Api.Client.Helpers
{
    public static class PaginationHelpers
    {
        /// <summary>
        /// Creates a PagedResult for page-based pagination scenarios where the API response indicates if it's the last page.
        /// </summary>
        /// <typeparam name="T">The type of items in the page.</typeparam>
        /// <param name="items">The items on the current page.</param>
        /// <param name="currentPage">The current page number (0-indexed).</param>
        /// <param name="pageSize">The number of items requested per page.</param>
        /// <param name="isLastPage">A boolean indicating if this is the last page of results.</param>
        /// <param name="totalCount">Optional. The total count of items across all pages, if known.</param>
        /// <returns>A PagedResult instance.</returns>
        public static PagedResult<T> CreatePagedResult<T>(
            IEnumerable<T> items,
            int currentPage,
            int pageSize,
            bool isLastPage,
            long? totalCount = null)
        {
            var itemsList = items?.ToList() ?? new List<T>();
            // TotalPages can only be definitively calculated if totalCount and pageSize are positive.
            // If isLastPage is true, and we don't have totalCount, we can infer totalPages if itemsList.Count < pageSize or itemsList.Count == 0.
            // However, the most robust way without totalCount is to simply not set TotalPages unless all pages are iterated.
            // For now, we pass null for TotalPages if totalCount is not provided.
            int? totalPages = null;
            if (totalCount.HasValue && pageSize > 0)
            {
                totalPages = (int)System.Math.Ceiling((double)totalCount.Value / pageSize);
            }
            else if (isLastPage)
            {
                // If it's the last page, then the current page number + 1 is the total number of pages.
                // (assuming currentPage is 0-indexed)
                totalPages = currentPage + 1;
                // If it's the last page and totalCount wasn't provided, we can only sum up if we assume previous pages were full.
                // This is not reliable for totalCount. So, we'll leave totalCount as potentially null.
            }


            return new PagedResult<T>(
                items: itemsList.AsReadOnly(),
                page: currentPage,
                pageSize: pageSize,
                hasNextPage: !isLastPage,
                totalPages: totalPages,
                totalCount: totalCount
            );
        }
    }
}
