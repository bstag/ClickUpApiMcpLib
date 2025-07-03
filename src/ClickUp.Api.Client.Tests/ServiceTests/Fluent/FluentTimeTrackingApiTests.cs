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
                It.IsAny<string>(),
                It.Is<Action<GetTimeEntriesRequestParameters>>(configureParameters =>
                {
                    var parameters = new GetTimeEntriesRequestParameters();
                    configureParameters(parameters);
                    return parameters.TimeRange != null &&
                           parameters.TimeRange.StartDate == DateTimeOffset.FromUnixTimeMilliseconds(1234567890L) &&
                           parameters.TimeRange.EndDate == DateTimeOffset.FromUnixTimeMilliseconds(9876543210L);
                }),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var fluentTimeTrackingApi = new TimeTrackingFluentApi(mockTimeTrackingService.Object);

        // Act
        var result = await fluentTimeTrackingApi.GetTimeEntries(workspaceId)
            .WithStartDate(1234567890L)
            .WithEndDate(9876543210L)
            .GetAsync();

        // Assert
        Assert.Equal(expectedTimeEntriesList, result.Items); // Assert against the .Items property
        mockTimeTrackingService.Verify(x => x.GetTimeEntriesAsync(
            workspaceId,
            It.Is<GetTimeEntriesRequest>(req =>
                req.StartDate == DateTimeOffset.FromUnixTimeMilliseconds(1234567890L) &&
                req.EndDate == DateTimeOffset.FromUnixTimeMilliseconds(9876543210L)),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
