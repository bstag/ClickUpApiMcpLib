using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Spaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentSpacesApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentSpacesApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentSpacesApi_GetSpacesAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedSpaces = new List<ClickUp.Api.Client.Models.Entities.Spaces.Space>();

        var mockSpacesService = new Mock<ISpacesService>();
        mockSpacesService.Setup(x => x.GetSpacesAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpaces);

        var fluentSpacesApi = new FluentSpacesApi(mockSpacesService.Object);

        // Act
        var result = await fluentSpacesApi.GetSpacesAsync(workspaceId);

        // Assert
        Assert.Equal(expectedSpaces, result);
        mockSpacesService.Verify(x => x.GetSpacesAsync(
            workspaceId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
