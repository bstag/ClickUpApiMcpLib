using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Sharing;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.Entities.Folders;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentSharedHierarchyApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentSharedHierarchyApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentSharedHierarchyApi_GetSharedHierarchyAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new SharedHierarchyResponse(new SharedHierarchyDetailsResponse(
            new List<string>(),
            new List<SharedHierarchyListItem>(),
            new List<SharedHierarchyFolderItem>()));

        var mockSharedHierarchyService = new Mock<ISharedHierarchyService>();
        mockSharedHierarchyService.Setup(x => x.GetSharedHierarchyAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentSharedHierarchyApi = new SharedHierarchyFluentApi(mockSharedHierarchyService.Object);

        // Act
        var result = await fluentSharedHierarchyApi.GetSharedHierarchyAsync(workspaceId);

        // Assert
        Assert.Equal(expectedResponse, result);
        mockSharedHierarchyService.Verify(x => x.GetSharedHierarchyAsync(
            workspaceId,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
