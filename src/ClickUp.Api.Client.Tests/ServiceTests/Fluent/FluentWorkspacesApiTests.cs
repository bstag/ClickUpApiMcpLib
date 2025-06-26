using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentWorkspacesApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentWorkspacesApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentWorkspacesApi_GetSeatsAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new GetWorkspaceSeatsResponse(
            new WorkspaceMemberSeatsInfo(1, 1, 0),
            new WorkspaceGuestSeatsInfo(1, 1, 0)
        ); // Mock a response

        var mockWorkspacesService = new Mock<IWorkspacesService>();
        mockWorkspacesService.Setup(x => x.GetWorkspaceSeatsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Manually inject the mock service into the fluent API for testing
        var fluentWorkspacesApi = new WorkspacesFluentApi(mockWorkspacesService.Object);

        // Act
        var result = await fluentWorkspacesApi.GetSeatsAsync(workspaceId);

        // Assert
        Assert.Equal(expectedResponse, result);
        mockWorkspacesService.Verify(x => x.GetWorkspaceSeatsAsync(
            workspaceId,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
