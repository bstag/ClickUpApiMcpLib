using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Spaces; // Ensures Space and Features are available

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

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
        var expectedSpaces = new List<Space>(); // Remains empty as we are not testing content here, just the call

        var mockSpacesService = new Mock<ISpacesService>();
        mockSpacesService.Setup(x => x.GetSpacesAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpaces);

        var fluentSpacesApi = new SpacesFluentApi(mockSpacesService.Object);

        // Act
        var result = await fluentSpacesApi.GetSpacesAsync(workspaceId);

        // Assert
        Assert.Equal(expectedSpaces, result);
        mockSpacesService.Verify(x => x.GetSpacesAsync(
            workspaceId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentSpacesApi_GetSpacesAsyncEnumerableAsync_ShouldCallServiceAndReturnItems()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var defaultFeatures = new Features(null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        var expectedSpaces = new List<Space> {
            new Space("space1", "Space 1", false, null, null, null, null, null, null, false, defaultFeatures, null, null),
            new Space("space2", "Space 2", false, null, null, null, null, null, null, false, defaultFeatures, null, null)
        };
        bool? archived = false;
        var cancellationToken = new CancellationTokenSource().Token;

        var mockSpacesService = new Mock<ISpacesService>();
        mockSpacesService.Setup(x => x.GetSpacesAsync(workspaceId, archived, cancellationToken))
            .ReturnsAsync(expectedSpaces);

        var fluentSpacesApi = new SpacesFluentApi(mockSpacesService.Object);

        // Act
        var result = new List<Space>();
        await foreach (var item in fluentSpacesApi.GetSpacesAsyncEnumerableAsync(workspaceId, archived, cancellationToken))
        {
            result.Add(item);
        }

        // Assert
        Assert.Equal(expectedSpaces.Count, result.Count);
        for (int i = 0; i < expectedSpaces.Count; i++)
        {
            Assert.Equal(expectedSpaces[i].Id, result[i].Id);
        }
        mockSpacesService.Verify(x => x.GetSpacesAsync(workspaceId, archived, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task FluentSpacesApi_GetSpacesAsyncEnumerableAsync_WithNullArchived_ShouldCallServiceAndReturnItems()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var defaultFeatures = new Features(null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        var expectedSpaces = new List<Space> {
            new Space("space1", "Space 1", false, null, null, null, null, null, null, false, defaultFeatures, null, null)
        };
        var cancellationToken = new CancellationTokenSource().Token;

        var mockSpacesService = new Mock<ISpacesService>();
        mockSpacesService.Setup(x => x.GetSpacesAsync(workspaceId, null, cancellationToken))
            .ReturnsAsync(expectedSpaces);

        var fluentSpacesApi = new SpacesFluentApi(mockSpacesService.Object);

        // Act
        var result = new List<Space>();
        await foreach (var item in fluentSpacesApi.GetSpacesAsyncEnumerableAsync(workspaceId, null, cancellationToken))
        {
            result.Add(item);
        }

        // Assert
        Assert.Single(result);
        Assert.Equal(expectedSpaces[0].Id, result[0].Id);
        mockSpacesService.Verify(x => x.GetSpacesAsync(workspaceId, null, cancellationToken), Times.Once);
    }
}
