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
    public void ToQueryParametersList_WithAllParametersSet_ShouldProduceCorrectList()
    {
        // Arrange
        var startDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var endDate = new DateTimeOffset(2023, 1, 10, 0, 0, 0, TimeSpan.Zero);
        var parameters = new GetTasksRequestParameters
        {
            SpaceIds = new List<long> { 123, 456 },
            ProjectIds = new List<long> { 789 }, // Folders
            ListIds = new List<string> { "101", "102" }, // Changed to string
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
        var kvpList = parameters.ToQueryParametersList(); // Already changed in a previous step, this is just for context

        // Assert
        Assert.Contains(new KeyValuePair<string, string>("space_ids", "123"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("space_ids", "456"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("project_ids", "789"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("list_ids", "101"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("list_ids", "102"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("assignees", "201"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("assignees", "202"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("statuses", Uri.EscapeDataString("Open")), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("statuses", Uri.EscapeDataString("In Progress")), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("tags", Uri.EscapeDataString("frontend")), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("tags", Uri.EscapeDataString("api task")), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("include_closed", "true"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("subtasks", "true"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("due_date_gt", startDate.ToUnixTimeMilliseconds().ToString()), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("due_date_lt", endDate.ToUnixTimeMilliseconds().ToString()), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("date_created_gt", startDate.AddDays(-5).ToUnixTimeMilliseconds().ToString()), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("date_created_lt", endDate.AddDays(-5).ToUnixTimeMilliseconds().ToString()), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("date_updated_gt", startDate.AddDays(-2).ToUnixTimeMilliseconds().ToString()), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("date_updated_lt", endDate.AddDays(-2).ToUnixTimeMilliseconds().ToString()), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("order_by", "due_date"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("reverse", "true"), kvpList); // Descending
        Assert.Contains(new KeyValuePair<string, string>("page", "1"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("include_markdown_description", "true"), kvpList);

        var expectedCustomFieldsJson = JsonSerializer.Serialize(parameters.CustomFields);
        Assert.Contains(new KeyValuePair<string, string>("custom_fields", expectedCustomFieldsJson), kvpList);

        Assert.Contains(new KeyValuePair<string, string>("custom_items", "0"), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("custom_items", "1"), kvpList);
    }

    [Fact]
    public void ToQueryParametersList_WithMinimalParameters_ShouldProduceCorrectList() // Method name already changed
    {
        // Arrange
        var parameters = new GetTasksRequestParameters
        {
            ListIds = new List<string> { "999" } // Changed to string
        };

        // Act
        var kvpList = parameters.ToQueryParametersList();

        // Assert
        Assert.Single(kvpList);
        Assert.Equal(new KeyValuePair<string,string>("list_ids", "999"), kvpList.First());
    }

    [Fact]
    public void ToQueryParametersList_ArrayParameters_ShouldBeMultipleKeyValuePairsAndEscaped() // Method name already changed
    {
        // Arrange
        var parameters = new GetTasksRequestParameters
        {
            Statuses = new List<string> { "To Do", "Another Status With Space" },
            Tags = new List<string> { "tag1", "tag with space" }
        };

        // Act
        var kvpList = parameters.ToQueryParametersList();

        // Assert
        Assert.Contains(new KeyValuePair<string, string>("statuses", Uri.EscapeDataString("To Do")), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("statuses", Uri.EscapeDataString("Another Status With Space")), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("tags", Uri.EscapeDataString("tag1")), kvpList);
        Assert.Contains(new KeyValuePair<string, string>("tags", Uri.EscapeDataString("tag with space")), kvpList);
        Assert.Equal(4, kvpList.Count); // Ensure only these parameters are present
    }

    [Fact]
    public void ToQueryParametersList_CustomFields_ShouldBeJsonSerialized() // Method name already changed
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
        var kvpList = parameters.ToQueryParametersList();

        // Assert
        Assert.Contains(new KeyValuePair<string, string>("custom_fields", expectedJson), kvpList);
        Assert.Single(kvpList); // Ensure only this parameter is present
    }

    [Fact]
    public void ToQueryParametersList_WhenNoParametersSet_ShouldReturnEmptyList() // Method name already changed
    {
        // Arrange
        var parameters = new GetTasksRequestParameters();

        // Act
        var kvpList = parameters.ToQueryParametersList();

        // Assert
        Assert.Empty(kvpList);
    }
}
