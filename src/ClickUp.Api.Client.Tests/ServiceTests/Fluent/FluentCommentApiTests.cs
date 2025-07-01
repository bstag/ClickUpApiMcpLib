using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq; // Required for ToAsyncEnumerable
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent
{
    public class FluentCommentApiTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        // private readonly ClickUpClient _client;

        public FluentCommentApiTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();
            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(Mock.Of<ILogger>());
            // _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
        }

        

        [Fact]
        public async Task GetTaskCommentsAsyncEnumerableAsync_DirectCall_WithAllParams_CallsServiceCorrectly()
        {
            // Arrange
            var taskId = "test-task-direct";
            var mockCommentService = new Mock<ICommentsService>();
            var expectedComments = ToAsyncEnumerable(new List<Comment>());
            bool customTaskIds = true;
            string teamId = "test-team";
            long start = 12345L;
            string startId = "commentStartId";

            mockCommentService.Setup(x => x.GetTaskCommentsStreamAsync(
                taskId,
                It.Is<GetTaskCommentsRequest>(r =>
                    r.CustomTaskIds == customTaskIds &&
                    r.TeamId == teamId &&
                    r.Start == start &&
                    r.StartId == startId),
                It.IsAny<CancellationToken>()))
                .Returns(expectedComments);

            var fluentCommentApi = new CommentFluentApi(mockCommentService.Object);

            // Act
            var resultStream = fluentCommentApi.GetTaskCommentsAsyncEnumerableAsync(taskId, customTaskIds, teamId, start, startId, CancellationToken.None);
            await foreach (var item in resultStream) { /* Consume */ }

            // Assert
            mockCommentService.Verify(x => x.GetTaskCommentsStreamAsync(
                taskId,
                It.Is<GetTaskCommentsRequest>(r =>
                    r.CustomTaskIds == customTaskIds &&
                    r.TeamId == teamId &&
                    r.Start == start &&
                    r.StartId == startId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTaskCommentsAsyncEnumerableAsync_DirectCall_WithOnlyTaskId_CallsServiceCorrectly()
        {
            // Arrange
            var taskId = "test-task-direct-simple";
            var mockCommentService = new Mock<ICommentsService>();
            var expectedComments = ToAsyncEnumerable(new List<Comment>());

            mockCommentService.Setup(x => x.GetTaskCommentsStreamAsync(
                taskId,
                It.IsAny<GetTaskCommentsRequest>(),
                It.IsAny<CancellationToken>()))
                .Returns(expectedComments);

            var fluentCommentApi = new CommentFluentApi(mockCommentService.Object);

            // Act
            var resultStream = fluentCommentApi.GetTaskCommentsAsyncEnumerableAsync(taskId, cancellationToken: CancellationToken.None);
            await foreach (var item in resultStream) { /* Consume */ }

            // Assert
            mockCommentService.Verify(x => x.GetTaskCommentsStreamAsync(
                taskId,
                It.Is<GetTaskCommentsRequest>(r =>
                    r.CustomTaskIds == null &&
                    r.TeamId == null &&
                    r.Start == null &&
                    r.StartId == null),
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
}
