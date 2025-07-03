using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ClickUp.Api.Client.Models.Common.ValueObjects;
using ClickUp.Api.Client.Models.Parameters;
using Xunit;

namespace ClickUp.Api.Client.Tests.Parameters;

public class GetTasksRequestParametersTests
{
    [Fact]
    public void ToDictionary_WithAllParametersSet_ShouldProduceCorrectDictionary()
    {
        // Arrange
        var startDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var endDate = new DateTimeOffset(2023, 1, 10, 0, 0, 0, TimeSpan.Zero);
        var parameters = new GetTasksRequestParameters
        {
            SpaceIds = new List<long> { 123, 456 },
            ProjectIds = new List<long> { 789 }, // Folders
            ListIds = new List<long> { 101, 102 },
            AssigneeIds = new List<int> { 201, 202 },
            Statuses = new List<string> { "Open", "In Progress" },
            Tags = new List<string> { "frontend", "api task" },
            IncludeClosed = true,
            Subtasks = true,
            DueDateRange = new TimeRange(startDate, endDate),
            DateCreatedRange = new TimeRange(startDate.AddDays(-5), endDate.AddDays(-5)),
            DateUpdatedRange = new TimeRange(startDate.AddDays(-2), endDate.AddDays(-2)),
            SortBy = new SortOption("due_date", SortDirection.Descending),
            Page = 1,
            IncludeMarkdownDescription = true,
            CustomFields = new List<CustomFieldFilter>
            {
                new CustomFieldFilter { FieldId = "cf_1", Operator = "=", Value = "test_value" },
                new CustomFieldFilter { FieldId = "cf_2", Operator = ">", Value = 100 }
            },
            CustomItems = new List<int> { 0, 1 } // 0 for task, 1 for milestone
        };

        // Act
        var dictionary = parameters.ToDictionary();

        // Assert
        Assert.Equal("123,456", dictionary["space_ids"]);
        Assert.Equal("789", dictionary["project_ids"]);
        Assert.Equal("101,102", dictionary["list_ids"]);
        Assert.Equal("201,202", dictionary["assignees"]);
        Assert.Equal($"Open,{Uri.EscapeDataString("In Progress")}", dictionary["statuses"]);
        Assert.Equal($"frontend,{Uri.EscapeDataString("api task")}", dictionary["tags"]);
        Assert.Equal("true", dictionary["include_closed"]);
        Assert.Equal("true", dictionary["subtasks"]);
        Assert.Equal(startDate.ToUnixTimeMilliseconds().ToString(), dictionary["due_date_gt"]);
        Assert.Equal(endDate.ToUnixTimeMilliseconds().ToString(), dictionary["due_date_lt"]);
        Assert.Equal(startDate.AddDays(-5).ToUnixTimeMilliseconds().ToString(), dictionary["date_created_gt"]);
        Assert.Equal(endDate.AddDays(-5).ToUnixTimeMilliseconds().ToString(), dictionary["date_created_lt"]);
        Assert.Equal(startDate.AddDays(-2).ToUnixTimeMilliseconds().ToString(), dictionary["date_updated_gt"]);
        Assert.Equal(endDate.AddDays(-2).ToUnixTimeMilliseconds().ToString(), dictionary["date_updated_lt"]);
        Assert.Equal("due_date", dictionary["order_by"]);
        Assert.Equal("true", dictionary["reverse"]); // Descending
        Assert.Equal("1", dictionary["page"]);
        Assert.Equal("true", dictionary["include_markdown_description"]);

        var expectedCustomFieldsJson = JsonSerializer.Serialize(parameters.CustomFields);
        Assert.Equal(expectedCustomFieldsJson, dictionary["custom_fields"]);

        Assert.Equal("0,1", dictionary["custom_items"]);
    }

    [Fact]
    public void ToDictionary_WithMinimalParameters_ShouldProduceCorrectDictionary()
    {
        // Arrange
        var parameters = new GetTasksRequestParameters
        {
            ListIds = new List<long> { 999 }
        };

        // Act
        var dictionary = parameters.ToDictionary();

        // Assert
        Assert.Single(dictionary);
        Assert.Equal("999", dictionary["list_ids"]);
    }

    [Fact]
    public void ToDictionary_ArrayParameters_ShouldBeCommaSeparatedAndEscaped()
    {
        // Arrange
        var parameters = new GetTasksRequestParameters
        {
            Statuses = new List<string> { "To Do", "Another Status With Space" },
            Tags = new List<string> { "tag1", "tag with space" }
        };

        // Act
        var dictionary = parameters.ToDictionary();

        // Assert
        Assert.Equal($"To%20Do,{Uri.EscapeDataString("Another Status With Space")}", dictionary["statuses"]);
        Assert.Equal($"tag1,{Uri.EscapeDataString("tag with space")}", dictionary["tags"]);
    }

    [Fact]
    public void ToDictionary_CustomFields_ShouldBeJsonSerialized()
    {
        // Arrange
        var customFieldFilters = new List<CustomFieldFilter>
        {
            new CustomFieldFilter { FieldId = "field_abc", Operator = "=", Value = "value123" },
            new CustomFieldFilter { FieldId = "field_def", Operator = "IS NOT NULL", Value = null }
        };
        var parameters = new GetTasksRequestParameters
        {
            CustomFields = customFieldFilters
        };
        var expectedJson = JsonSerializer.Serialize(customFieldFilters);

        // Act
        var dictionary = parameters.ToDictionary();

        // Assert
        Assert.True(dictionary.ContainsKey("custom_fields"));
        Assert.Equal(expectedJson, dictionary["custom_fields"]);
    }

    [Fact]
    public void ToDictionary_WhenNoParametersSet_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var parameters = new GetTasksRequestParameters();

        // Act
        var dictionary = parameters.ToDictionary();

        // Assert
        Assert.Empty(dictionary);
    }
}
