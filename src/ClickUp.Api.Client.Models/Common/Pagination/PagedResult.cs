// src/ClickUp.Api.Client.Models/Common/Pagination/PagedResult.cs
using System.Collections.Generic;
using System.Linq;

namespace ClickUp.Api.Client.Models.Common.Pagination
{
    public class PagedResult<T> : IPagedResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int? TotalPages { get; }
        public long? TotalCount { get; }
        public bool HasNextPage { get; }
        public bool HasPreviousPage => Page > 0; // Page is 0-indexed, so previous is possible if Page > 0

        // Constructor for when total pages/count are known
        public PagedResult(IEnumerable<T> items, int page, int pageSize, bool hasNextPage, int? totalPages = null, long? totalCount = null)
        {
            Items = items?.ToList().AsReadOnly() ?? new List<T>().AsReadOnly();
            Page = page;
            PageSize = pageSize;
            TotalPages = totalPages;
            TotalCount = totalCount;
            HasNextPage = hasNextPage;
        }
    }
}
