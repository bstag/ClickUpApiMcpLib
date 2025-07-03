using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Common.ValueObjects
{
    public record SortOption(string FieldName, SortDirection Direction)
    {
        public Dictionary<string, string> ToQueryParameters(string orderByParamName = "order_by", string reverseParamName = "reverse")
        {
            return new Dictionary<string, string>
            {
                { orderByParamName, FieldName },
                { reverseParamName, Direction == SortDirection.Descending ? "true" : "false" }
            };
        }

        // Overload for APIs that use sort_order (asc/desc) instead of reverse (true/false)
        public Dictionary<string, string> ToQueryParameters(string sortByParamName = "sort_by", string sortOrderParamName = "sort_order", bool useSortOrder = true)
        {
            if (!useSortOrder) return ToQueryParameters(sortByParamName, reverseParamName: sortOrderParamName); // Fallback to order_by/reverse if needed, though names might be confusing

            return new Dictionary<string, string>
            {
                { sortByParamName, FieldName },
                { sortOrderParamName, Direction == SortDirection.Descending ? "desc" : "asc" }
            };
        }
    }
}
