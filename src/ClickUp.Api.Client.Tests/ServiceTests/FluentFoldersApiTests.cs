using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Folders;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentFoldersApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentFoldersApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentFoldersApi_GetFoldersAsync_ShouldCallService()
    {
        // Arrange
        var spaceId = "testSpaceId";
        var expectedFolders = new List<Folder>();

        var mockFoldersService = new Mock<IFoldersService>();
        mockFoldersService.Setup(x => x.GetFoldersAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFolders);

        var fluentFoldersApi = new FluentFoldersApi(mockFoldersService.Object);

        // Act
        var result = await fluentFoldersApi.GetFoldersAsync(spaceId);

        // Assert
        Assert.Equal(expectedFolders, result);
        mockFoldersService.Verify(x => x.GetFoldersAsync(
            spaceId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
