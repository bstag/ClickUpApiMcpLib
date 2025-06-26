using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.UserGroups;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentUserGroupsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentUserGroupsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentUserGroupsApi_GetUserGroupsAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedUserGroups = new List<UserGroup>();

        var mockUserGroupsService = new Mock<IUserGroupsService>();
        mockUserGroupsService.Setup(x => x.GetUserGroupsAsync(
            It.IsAny<string>(),
            It.IsAny<List<string>?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserGroups);

        var fluentUserGroupsApi = new FluentUserGroupsApi(mockUserGroupsService.Object);

        // Act
        var result = await fluentUserGroupsApi.GetUserGroupsAsync(workspaceId);

        // Assert
        Assert.Equal(expectedUserGroups, result);
        mockUserGroupsService.Verify(x => x.GetUserGroupsAsync(
            workspaceId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
