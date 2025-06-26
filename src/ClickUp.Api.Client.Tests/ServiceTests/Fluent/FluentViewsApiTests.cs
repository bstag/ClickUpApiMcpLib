using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Views;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentViewsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentViewsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentViewsApi_GetWorkspaceViewsAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new GetViewsResponse { Views = new List<Client.Models.Entities.Views.View>() };

        var mockViewsService = new Mock<IViewsService>();
        mockViewsService.Setup(x => x.GetWorkspaceViewsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentViewsApi = new ViewsFluentApi(mockViewsService.Object);

        // Act
        var result = await fluentViewsApi.GetWorkspaceViewsAsync(workspaceId);

        // Assert
        Assert.Equal(expectedResponse, result);
        mockViewsService.Verify(x => x.GetWorkspaceViewsAsync(
            workspaceId,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
