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
        var expectedTimeEntries = new List<TimeEntry>();

        var mockTimeTrackingService = new Mock<ITimeTrackingService>();
        mockTimeTrackingService.Setup(x => x.GetTimeEntriesAsync(
            It.IsAny<string>(),
            It.IsAny<Client.Models.RequestModels.TimeTracking.GetTimeEntriesRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTimeEntries);

        var fluentTimeTrackingApi = new TimeTrackingFluentApi(mockTimeTrackingService.Object);

        // Act
        var result = await fluentTimeTrackingApi.GetTimeEntries(workspaceId)
            .WithStartDate(1234567890L)
            .WithEndDate(9876543210L)
            .GetAsync();

        // Assert
        Assert.Equal(expectedTimeEntries, result);
        mockTimeTrackingService.Verify(x => x.GetTimeEntriesAsync(
            workspaceId,
            It.Is<Client.Models.RequestModels.TimeTracking.GetTimeEntriesRequest>(req =>
                req.StartDate == DateTimeOffset.FromUnixTimeMilliseconds(1234567890L) &&
                req.EndDate == DateTimeOffset.FromUnixTimeMilliseconds(9876543210L)),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
