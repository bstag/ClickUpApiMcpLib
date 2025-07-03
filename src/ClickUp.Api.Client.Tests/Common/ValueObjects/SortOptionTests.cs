using System.Collections.Generic;
using ClickUp.Api.Client.Models.Common.ValueObjects;
using Xunit;

namespace ClickUp.Api.Client.Tests.Common.ValueObjects;

public class SortOptionTests
{
    [Fact]
    public void ToQueryParameters_DefaultNames_Ascending_ShouldReturnCorrectDictionary()
    {
        // Arrange
        var sortOption = new SortOption("name", SortDirection.Ascending);

        // Act
        var queryParams = sortOption.ToQueryParameters(orderByParamName: "order_by"); // Explicitly choose overload

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "order_by", "name" },
            { "reverse", "false" }
        };
        Assert.Equal(expected, queryParams);
    }

    [Fact]
    public void ToQueryParameters_DefaultNames_Descending_ShouldReturnCorrectDictionary()
    {
        // Arrange
        var sortOption = new SortOption("created_date", SortDirection.Descending);

        // Act
        var queryParams = sortOption.ToQueryParameters(orderByParamName: "order_by"); // Explicitly choose overload

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "order_by", "created_date" },
            { "reverse", "true" }
        };
        Assert.Equal(expected, queryParams);
    }

    [Fact]
    public void ToQueryParameters_CustomNames_Ascending_ShouldReturnCorrectDictionary()
    {
        // Arrange
        var sortOption = new SortOption("priority", SortDirection.Ascending);

        // Act
        var queryParams = sortOption.ToQueryParameters("sortField", "isDesc");

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "sortField", "priority" },
            { "isDesc", "false" }
        };
        Assert.Equal(expected, queryParams);
    }

    [Fact]
    public void ToQueryParameters_SortOrderConvention_Ascending_ShouldReturnCorrectDictionary()
    {
        // Arrange
        var sortOption = new SortOption("due_date", SortDirection.Ascending);

        // Act
        var queryParams = sortOption.ToQueryParameters("sortByField", "orderDirection", useSortOrderConvention: true);

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "sortByField", "due_date" },
            { "orderDirection", "asc" }
        };
        Assert.Equal(expected, queryParams);
    }

    [Fact]
    public void ToQueryParameters_SortOrderConvention_Descending_ShouldReturnCorrectDictionary()
    {
        // Arrange
        var sortOption = new SortOption("status", SortDirection.Descending);

        // Act
        var queryParams = sortOption.ToQueryParameters(useSortOrderConvention: true); // Using default names for sort_by and sort_order

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "sort_by", "status" },
            { "sort_order", "desc" }
        };
        Assert.Equal(expected, queryParams);
    }

    [Fact]
    public void ToQueryParameters_SortOrderConventionFalse_ShouldFallBackToReverseConvention()
    {
        // Arrange
        var sortOption = new SortOption("name", SortDirection.Descending);

        // Act
        // This will call the ToQueryParameters(orderByParamName, reverseParamName) overload
        // with sortByParamName as orderByParamName and sortOrderParamName as reverseParamName.
        var queryParams = sortOption.ToQueryParameters(sortByParamName: "custom_sort_field", sortOrderParamName: "custom_reverse_flag", useSortOrderConvention: false);

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "custom_sort_field", "name" },
            { "custom_reverse_flag", "true" } // "true" because Direction is Descending
        };
        Assert.Equal(expected, queryParams);
    }
}
