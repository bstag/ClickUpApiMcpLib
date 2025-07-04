using System.Collections.Generic;
using ClickUp.Api.Client.Models.Common.ValueObjects;
using Xunit;

namespace ClickUp.Api.Client.Tests.Common.ValueObjects;

public class SortOptionTests
{
    [Fact]
    public void ToOrderByReverseParameters_DefaultNames_Ascending_ShouldReturnCorrectDictionary()
    {
        // Arrange
        var sortOption = new SortOption("name", SortDirection.Ascending);

        // Act
        var queryParams = sortOption.ToOrderByReverseParameters();

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "order_by", "name" },
            { "reverse", "false" }
        };
        Assert.Equal(expected, queryParams);
    }

    [Fact]
    public void ToOrderByReverseParameters_DefaultNames_Descending_ShouldReturnCorrectDictionary()
    {
        // Arrange
        var sortOption = new SortOption("created_date", SortDirection.Descending);

        // Act
        var queryParams = sortOption.ToOrderByReverseParameters();

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "order_by", "created_date" },
            { "reverse", "true" }
        };
        Assert.Equal(expected, queryParams);
    }

    [Fact]
    public void ToOrderByReverseParameters_CustomNames_Ascending_ShouldReturnCorrectDictionary()
    {
        // Arrange
        var sortOption = new SortOption("priority", SortDirection.Ascending);

        // Act
        var queryParams = sortOption.ToOrderByReverseParameters("sortField", "isDesc");

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "sortField", "priority" },
            { "isDesc", "false" }
        };
        Assert.Equal(expected, queryParams);
    }

    [Fact]
    public void ToSortByOrderParameters_DefaultNames_Ascending_ShouldReturnCorrectDictionary()
    {
        // Arrange
        var sortOption = new SortOption("due_date", SortDirection.Ascending);

        // Act
        var queryParams = sortOption.ToSortByOrderParameters();

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "sort_by", "due_date" },
            { "sort_order", "asc" }
        };
        Assert.Equal(expected, queryParams);
    }

    [Fact]
    public void ToSortByOrderParameters_DefaultNames_Descending_ShouldReturnCorrectDictionary()
    {
        // Arrange
        var sortOption = new SortOption("status", SortDirection.Descending);

        // Act
        var queryParams = sortOption.ToSortByOrderParameters();

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "sort_by", "status" },
            { "sort_order", "desc" }
        };
        Assert.Equal(expected, queryParams);
    }

    [Fact]
    public void ToSortByOrderParameters_CustomNames_Ascending_ShouldReturnCorrectDictionary()
    {
        // Arrange
        var sortOption = new SortOption("due_date", SortDirection.Ascending);

        // Act
        var queryParams = sortOption.ToSortByOrderParameters("sortByField", "orderDirection");

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "sortByField", "due_date" },
            { "orderDirection", "asc" }
        };
        Assert.Equal(expected, queryParams);
    }

    // Tests for backward compatibility with obsolete methods
    [Fact]
    public void ToQueryParameters_SortOrderConventionFalse_ShouldFallBackToReverseConvention()
    {
        // Arrange
        var sortOption = new SortOption("name", SortDirection.Descending);

        // Act
        // Testing obsolete method for backward compatibility
        #pragma warning disable CS0618 // Type or member is obsolete
        var queryParams = sortOption.ToQueryParameters(sortByParamName: "custom_sort_field", sortOrderParamName: "custom_reverse_flag", useSortOrderConvention: false);
        #pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        var expected = new Dictionary<string, string>
        {
            { "custom_sort_field", "name" },
            { "custom_reverse_flag", "true" } // "true" because Direction is Descending
        };
        Assert.Equal(expected, queryParams);
    }
}
