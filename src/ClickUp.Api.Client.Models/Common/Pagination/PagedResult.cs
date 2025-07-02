using System.Collections.Generic;
using System.Linq;

namespace ClickUp.Api.Client.Models.Common.Pagination
{
    public class PagedResult<T> : IPagedResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public long TotalCount { get; }
        public bool HasNextPage { get; }
        public bool HasPreviousPage { get; }

        public PagedResult(IEnumerable<T> items, int page, int pageSize, long totalCount)
        {
            Items = items?.ToList() ?? new List<T>();
            Page = page; // Assuming 0-indexed page from API response or request
            PageSize = pageSize;
            TotalCount = totalCount;

            if (pageSize > 0 && totalCount >= 0)
            {
                TotalPages = (int)System.Math.Ceiling((double)totalCount / pageSize);
            }
            else
            {
                TotalPages = (totalCount == 0 && page == 0 && !Items.Any()) ? 0 : -1; // -1 indicates unknown total pages
            }

            HasPreviousPage = page > 0; // Page is 0-indexed
            HasNextPage = totalCount >= 0 ? (page + 1) * pageSize < totalCount : false; // If totalCount is unknown, can't determine HasNextPage this way
                                                                                      // This will be overridden by the other constructor if LastPage is used.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{T}"/> class
        /// when total count is not known but next page availability is.
        /// Page is assumed to be 0-indexed.
        /// </summary>
        public PagedResult(IEnumerable<T> items, int page, int pageSize, bool hasNextPage)
        {
            Items = items?.ToList() ?? new List<T>();
            Page = page; // 0-indexed
            PageSize = pageSize;
            HasNextPage = hasNextPage;
            HasPreviousPage = page > 0;
            TotalCount = -1; // Unknown
            TotalPages = -1; // Unknown
        }

        public static PagedResult<T> Empty(int page = 0, int pageSize = 0) // Default page to 0 for 0-indexed consistency
        {
            return new PagedResult<T>(new List<T>(), page, pageSize, 0);
        }
    }
}
