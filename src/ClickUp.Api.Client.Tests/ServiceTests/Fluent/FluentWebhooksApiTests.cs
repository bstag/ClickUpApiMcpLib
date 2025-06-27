using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Webhooks;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentWebhooksApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentWebhooksApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentWebhooksApi_GetWebhooksAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedWebhooks = new List<Webhook>();

        var mockWebhooksService = new Mock<IWebhooksService>();
        mockWebhooksService.Setup(x => x.GetWebhooksAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedWebhooks);

        var fluentWebhooksApi = new WebhooksFluentApi(mockWebhooksService.Object);

        // Act
        var result = await fluentWebhooksApi.GetWebhooksAsync(workspaceId);

        // Assert
        Assert.Equal(expectedWebhooks, result);
        mockWebhooksService.Verify(x => x.GetWebhooksAsync(
            workspaceId,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
