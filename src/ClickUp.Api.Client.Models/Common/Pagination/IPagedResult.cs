using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Common.Pagination
{
    public interface IPagedResult<T>
    {
        IReadOnlyList<T> Items { get; }
        int Page { get; }
        int PageSize { get; } // Or PerPage
        int TotalPages { get; }
        long TotalCount { get; } // If available from API
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
    }
}
