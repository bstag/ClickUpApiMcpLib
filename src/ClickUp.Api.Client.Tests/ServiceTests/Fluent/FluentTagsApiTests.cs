using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Tags;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentTagsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentTagsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentTagsApi_GetSpaceTagsAsync_ShouldCallService()
    {
        // Arrange
        var spaceId = "testSpaceId";
        var expectedTags = new List<Tag>();

        var mockTagsService = new Mock<ITagsService>();
        mockTagsService.Setup(x => x.GetSpaceTagsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTags);

        var fluentTagsApi = new TagsFluentApi(mockTagsService.Object);

        // Act
        var result = await fluentTagsApi.GetSpaceTagsAsync(spaceId);

        // Assert
        Assert.Equal(expectedTags, result);
        mockTagsService.Verify(x => x.GetSpaceTagsAsync(
            spaceId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentTagsApi_GetSpaceTagsAsyncEnumerableAsync_ShouldCallServiceAndReturnItems()
    {
        // Arrange
        var spaceId = "testSpaceId";
        var expectedTags = new List<Tag> { new Tag("tag1", null, null, null), new Tag("tag2", null, null, null) };
        var cancellationToken = new CancellationTokenSource().Token;

        var mockTagsService = new Mock<ITagsService>();
        mockTagsService.Setup(x => x.GetSpaceTagsAsync(spaceId, cancellationToken))
            .ReturnsAsync(expectedTags);

        var fluentTagsApi = new TagsFluentApi(mockTagsService.Object);

        // Act
        var result = new List<Tag>();
        await foreach (var item in fluentTagsApi.GetSpaceTagsAsyncEnumerableAsync(spaceId, cancellationToken))
        {
            result.Add(item);
        }

        // Assert
        Assert.Equal(expectedTags.Count, result.Count);
        for (int i = 0; i < expectedTags.Count; i++)
        {
            Assert.Equal(expectedTags[i].Name, result[i].Name);
        }
        mockTagsService.Verify(x => x.GetSpaceTagsAsync(spaceId, cancellationToken), Times.Once);
    }
}
