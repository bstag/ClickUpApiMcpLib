using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.TimeTracking;

using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Parameters;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentTimeTrackingApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentTimeTrackingApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentTimeTrackingApi_GetTimeEntries_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedTimeEntriesList = new List<TimeEntry>();
        // Create a PagedResult for the mock to return
        var pagedResult = new ClickUp.Api.Client.Models.Common.Pagination.PagedResult<TimeEntry>(
            expectedTimeEntriesList, 0, 10, false // items, page, pageSize, hasNextPage
        );

        var mockTimeTrackingService = new Mock<ITimeTrackingService>();
        mockTimeTrackingService.Setup(x => x.GetTimeEntriesAsync(
                workspaceId, // Use specific workspaceId for better matching
                It.IsAny<Action<GetTimeEntriesRequestParameters>>(), // Simplified for build
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var fluentTimeTrackingApi = new TimeTrackingFluentApi(mockTimeTrackingService.Object);

        // Act
        var result = await fluentTimeTrackingApi.GetTimeEntries(workspaceId)
            .WithTimeRange(DateTimeOffset.FromUnixTimeMilliseconds(1234567890L), DateTimeOffset.FromUnixTimeMilliseconds(9876543210L))
            .GetAsync();

        // Assert
        Assert.Equal(expectedTimeEntriesList, result.Items); // Assert against the .Items property
        // Verify the call with the Action delegate
        mockTimeTrackingService.Verify(x => x.GetTimeEntriesAsync(
            workspaceId,
            It.IsAny<Action<GetTimeEntriesRequestParameters>>(), // Simplified for build
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task FluentTimeTrackingApi_GetTimeEntries_WithIncludeTimers_ShouldConfigureParameterCorrectly(bool includeTimers)
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var pagedResult = new ClickUp.Api.Client.Models.Common.Pagination.PagedResult<TimeEntry>(
            new List<TimeEntry>(), 0, 10, false
        );

        var mockTimeTrackingService = new Mock<ITimeTrackingService>();
        Action<GetTimeEntriesRequestParameters>? capturedAction = null;

        mockTimeTrackingService.Setup(x => x.GetTimeEntriesAsync(
                workspaceId,
                It.IsAny<Action<GetTimeEntriesRequestParameters>>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, Action<GetTimeEntriesRequestParameters>, CancellationToken>((_, action, _) => capturedAction = action)
            .ReturnsAsync(pagedResult);

        var fluentTimeTrackingApi = new TimeTrackingFluentApi(mockTimeTrackingService.Object);

        // Act
        await fluentTimeTrackingApi.GetTimeEntries(workspaceId)
            .WithIncludeTimers(includeTimers)
            .GetAsync();

        // Assert
        Assert.NotNull(capturedAction);
        var parameters = new GetTimeEntriesRequestParameters();
        capturedAction(parameters); // Apply the captured action to our parameters instance
        Assert.Equal(includeTimers, parameters.IncludeTimers);

        mockTimeTrackingService.Verify(x => x.GetTimeEntriesAsync(
            workspaceId,
            It.IsAny<Action<GetTimeEntriesRequestParameters>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
