using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Common.ValueObjects;

/// <summary>
/// Represents a sorting option defined by a field name and a sort direction.
/// </summary>
/// <param name="FieldName">The name of the field to sort by.</param>
/// <param name="Direction">The direction of the sort (Ascending or Descending).</param>
public record SortOption(string FieldName, SortDirection Direction)
{
    /// <summary>
    /// Converts the SortOption to a dictionary of query parameters using "order_by" and "reverse" (boolean).
    /// This method is for APIs that use the order_by + reverse convention where reverse=true means descending.
    /// </summary>
    /// <param name="orderByParamName">The name for the order by parameter (defaults to "order_by").</param>
    /// <param name="reverseParamName">The name for the reverse parameter (defaults to "reverse").</param>
    /// <returns>A dictionary containing the order_by field and reverse flag (true/false).</returns>
    public Dictionary<string, string> ToOrderByReverseParameters(string orderByParamName = "order_by", string reverseParamName = "reverse")
    {
        return new Dictionary<string, string>
        {
            { orderByParamName, FieldName },
            { reverseParamName, (Direction == SortDirection.Descending).ToString().ToLowerInvariant() }
        };
    }

    /// <summary>
    /// Converts the SortOption to a dictionary of query parameters using "sort_by" and "sort_order" (asc/desc).
    /// This method is for APIs that use the sort_by + sort_order convention with explicit asc/desc values.
    /// </summary>
    /// <param name="sortByParamName">The name for the sort by parameter (defaults to "sort_by").</param>
    /// <param name="sortOrderParamName">The name for the sort order parameter (defaults to "sort_order").</param>
    /// <returns>A dictionary containing the sort_by field and sort_order value (asc/desc).</returns>
    public Dictionary<string, string> ToSortByOrderParameters(string sortByParamName = "sort_by", string sortOrderParamName = "sort_order")
    {
        return new Dictionary<string, string>
        {
            { sortByParamName, FieldName },
            { sortOrderParamName, Direction == SortDirection.Descending ? "desc" : "asc" }
        };
    }

    /// <summary>
    /// Converts the SortOption to a dictionary of query parameters using "order_by" and "reverse" (boolean).
    /// </summary>
    /// <param name="orderByParamName">The name for the order by parameter (defaults to "order_by").</param>
    /// <param name="reverseParamName">The name for the reverse parameter (defaults to "reverse").</param>
    /// <returns>A dictionary containing the order_by field and reverse flag (true/false).</returns>
    [System.Obsolete("Use ToOrderByReverseParameters() for clarity. This method will be removed in a future version.")]
    public Dictionary<string, string> ToQueryParameters(string orderByParamName = "order_by", string reverseParamName = "reverse")
    {
        return ToOrderByReverseParameters(orderByParamName, reverseParamName);
    }

    /// <summary>
    /// Converts the SortOption to a dictionary of query parameters using "sort_by" and "sort_order" (asc/desc).
    /// </summary>
    /// <param name="sortByParamName">The name for the sort by parameter (defaults to "sort_by").</param>
    /// <param name="sortOrderParamName">The name for the sort order parameter (defaults to "sort_order").</param>
    /// <param name="useSortOrderConvention">If true (default), uses 'asc'/'desc'. If false, it attempts to call the other overload, which might lead to unexpected behavior if parameter names aren't adjusted by the caller.</param>
    /// <returns>A dictionary containing the sort_by field and sort_order value (asc/desc).</returns>
    [System.Obsolete("Use ToSortByOrderParameters() or ToOrderByReverseParameters() for clarity. This confusing boolean flag will be removed in a future version.")]
    public Dictionary<string, string> ToQueryParameters(string sortByParamName = "sort_by", string sortOrderParamName = "sort_order", bool useSortOrderConvention = true)
    {
        if (!useSortOrderConvention)
        {
            return ToOrderByReverseParameters(sortByParamName, reverseParamName: sortOrderParamName);
        }

        return ToSortByOrderParameters(sortByParamName, sortOrderParamName);
    }
}
