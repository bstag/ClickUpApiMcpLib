// src/ClickUp.Api.Client.Models/Common/Pagination/IPagedResult.cs
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Common.Pagination
{
    public interface IPagedResult<T>
    {
        IReadOnlyList<T> Items { get; }
        int Page { get; }
        int PageSize { get; }
        int? TotalPages { get; } // Can be null if not determinable from a single response
        long? TotalCount { get; } // Can be null if not provided by the API
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
    }
}
