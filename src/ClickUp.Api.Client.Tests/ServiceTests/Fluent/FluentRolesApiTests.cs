using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Roles;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentRolesApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentRolesApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentRolesApi_GetCustomRolesAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedRoles = new List<CustomRole>();

        var mockRolesService = new Mock<IRolesService>();
        mockRolesService.Setup(x => x.GetCustomRolesAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRoles);

        var fluentRolesApi = new RolesFluentApi(mockRolesService.Object);

        // Act
        var result = await fluentRolesApi.GetCustomRolesAsync(workspaceId);

        // Assert
        Assert.Equal(expectedRoles, result);
        mockRolesService.Verify(x => x.GetCustomRolesAsync(
            workspaceId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
