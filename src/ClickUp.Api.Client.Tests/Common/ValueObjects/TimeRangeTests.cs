using System;
using System.Collections.Generic;
using ClickUp.Api.Client.Models.Common.ValueObjects;
using Xunit;

namespace ClickUp.Api.Client.Tests.Common.ValueObjects;

public class TimeRangeTests
{
    [Fact]
    public void Constructor_WhenStartDateIsAfterEndDate_ShouldThrowArgumentException()
    {
        // Arrange
        var startDate = new DateTimeOffset(2023, 1, 2, 0, 0, 0, TimeSpan.Zero);
        var endDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new TimeRange(startDate, endDate));
        Assert.Equal("StartDate cannot be after EndDate. (Parameter 'StartDate')", exception.Message);
    }

    [Fact]
    public void Constructor_WhenStartDateIsEqualToEndDate_ShouldNotThrow()
    {
        // Arrange
        var date = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        var timeRange = new TimeRange(date, date);

        // Assert
        Assert.Equal(date, timeRange.StartDate);
        Assert.Equal(date, timeRange.EndDate);
    }

    [Fact]
    public void Constructor_WhenStartDateIsBeforeEndDate_ShouldNotThrow()
    {
        // Arrange
        var startDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var endDate = new DateTimeOffset(2023, 1, 2, 0, 0, 0, TimeSpan.Zero);

        // Act
        var timeRange = new TimeRange(startDate, endDate);

        // Assert
        Assert.Equal(startDate, timeRange.StartDate);
        Assert.Equal(endDate, timeRange.EndDate);
    }

    [Fact]
    public void ToQueryParameters_ShouldReturnCorrectDictionary_WithDefaultNames()
    {
        // Arrange
        var startDate = new DateTimeOffset(2023, 1, 1, 10, 0, 0, TimeSpan.FromHours(2)); // UTC: 2023-01-01 08:00:00
        var endDate = new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.FromHours(2));   // UTC: 2023-01-01 10:00:00
        var timeRange = new TimeRange(startDate, endDate);

        var expectedStartDateUnixMs = startDate.ToUnixTimeMilliseconds().ToString();
        var expectedEndDateUnixMs = endDate.ToUnixTimeMilliseconds().ToString();

        // Act
        var queryParams = timeRange.ToQueryParameters();

        // Assert
        Assert.Equal(2, queryParams.Count);
        Assert.Equal(expectedStartDateUnixMs, queryParams["start_date"]);
        Assert.Equal(expectedEndDateUnixMs, queryParams["end_date"]);
    }

    [Fact]
    public void ToQueryParameters_ShouldReturnCorrectDictionary_WithCustomNames()
    {
        // Arrange
        var startDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var endDate = new DateTimeOffset(2023, 1, 2, 0, 0, 0, TimeSpan.Zero);
        var timeRange = new TimeRange(startDate, endDate);

        var expectedStartDateUnixMs = startDate.ToUnixTimeMilliseconds().ToString();
        var expectedEndDateUnixMs = endDate.ToUnixTimeMilliseconds().ToString();

        const string customStartName = "beginDate";
        const string customEndName = "finishDate";

        // Act
        var queryParams = timeRange.ToQueryParameters(customStartName, customEndName);

        // Assert
        Assert.Equal(2, queryParams.Count);
        Assert.Equal(expectedStartDateUnixMs, queryParams[customStartName]);
        Assert.Equal(expectedEndDateUnixMs, queryParams[customEndName]);
    }

    [Fact]
    public void ToQueryParameters_ShouldHandleDifferentOffsetsCorrectly_ConvertingToUtcEquivalentMilliseconds()
    {
        // Arrange
        // StartDate: 2023-01-01 10:00:00 +02:00 (UTC 08:00:00)
        var startDate = new DateTimeOffset(2023, 1, 1, 10, 0, 0, TimeSpan.FromHours(2));
        // EndDate:   2023-01-01 15:00:00 +05:00 (UTC 10:00:00)
        // This is the same instant as 2023-01-01 12:00:00 +02:00
        var endDate = new DateTimeOffset(2023, 1, 1, 15, 0, 0, TimeSpan.FromHours(5));
        var timeRange = new TimeRange(startDate, endDate);

        // Expected Unix milliseconds are based on the UTC instants
        var expectedStartDateUnixMs = startDate.ToUnixTimeMilliseconds().ToString();
        var expectedEndDateUnixMs = endDate.ToUnixTimeMilliseconds().ToString();

        // Act
        var queryParams = timeRange.ToQueryParameters();

        // Assert
        Assert.Equal(expectedStartDateUnixMs, queryParams["start_date"]);
        Assert.Equal(expectedEndDateUnixMs, queryParams["end_date"]);
    }
}
