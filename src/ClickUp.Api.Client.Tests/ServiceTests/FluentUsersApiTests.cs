using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Users;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentUsersApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentUsersApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentUsersApi_GetUserFromWorkspaceAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var userId = "testUserId";
        var expectedUser = new User(1, "testuser", "test@example.com", "#000000", null, "TU", null);

        var mockUsersService = new Mock<IUsersService>();
        mockUsersService.Setup(x => x.GetUserFromWorkspaceAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        var fluentUsersApi = new FluentUsersApi(mockUsersService.Object);

        // Act
        var result = await fluentUsersApi.GetUserFromWorkspaceAsync(workspaceId, userId);

        // Assert
        Assert.Equal(expectedUser, result);
        mockUsersService.Verify(x => x.GetUserFromWorkspaceAsync(
            workspaceId,
            userId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
