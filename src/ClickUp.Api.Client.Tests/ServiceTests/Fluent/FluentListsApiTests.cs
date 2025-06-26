using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentListsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentListsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentListsApi_GetListsInFolderAsync_ShouldCallService()
    {
        // Arrange
        var folderId = "testFolderId";
        var expectedLists = new List<ClickUpList>();

        var mockListsService = new Mock<IListsService>();
        mockListsService.Setup(x => x.GetListsInFolderAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLists);

        var fluentListsApi = new ListsFluentApi(mockListsService.Object);

        // Act
        var result = await fluentListsApi.GetListsInFolderAsync(folderId);

        // Assert
        Assert.Equal(expectedLists, result);
        mockListsService.Verify(x => x.GetListsInFolderAsync(
            folderId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
