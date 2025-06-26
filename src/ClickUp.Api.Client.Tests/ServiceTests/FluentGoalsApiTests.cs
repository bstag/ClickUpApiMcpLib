using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Goals;
using ClickUp.Api.Client.Models.Entities.Goals;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentGoalsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentGoalsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentGoalsApi_GetGoals_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new GetGoalsResponse(new List<Goal>(), new List<GoalFolder>());

        var mockGoalsService = new Mock<IGoalsService>();
        mockGoalsService.Setup(x => x.GetGoalsAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentGoalsApi = new FluentGoalsApi(mockGoalsService.Object);

        // Act
        var result = await fluentGoalsApi.GetGoals(workspaceId)
            .WithIncludeCompleted(true)
            .GetAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockGoalsService.Verify(x => x.GetGoalsAsync(
            workspaceId,
            true,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
