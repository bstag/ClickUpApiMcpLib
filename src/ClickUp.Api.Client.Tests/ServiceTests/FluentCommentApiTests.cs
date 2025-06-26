using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Comments;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentCommentApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentCommentApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentCommentApi_GetTaskComments_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var taskId = "testTaskId";
        var expectedComments = new List<Comment>();

        var mockCommentsService = new Mock<ICommentsService>();
        mockCommentsService.Setup(x => x.GetTaskCommentsStreamAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<long?>(),
            It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(expectedComments));

        var fluentCommentApi = new FluentCommentApi(mockCommentsService.Object);

        // Act
        var result = new List<Comment>();
        await foreach (var comment in fluentCommentApi.GetTaskComments(taskId).GetStreamAsync())
        {
            result.Add(comment);
        }

        // Assert
        Assert.Equal(expectedComments, result);
        mockCommentsService.Verify(x => x.GetTaskCommentsStreamAsync(
            taskId,
            null,
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> enumerable)
    {
        foreach (var item in enumerable)
        {
            await Task.Yield();
            yield return item;
        }
    }
}
