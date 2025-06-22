using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.Entities.Users; // Added for User model
using ClickUp.Api.Client.Models.ResponseModels.Comments; // Corrected namespace
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging; // Added for ILogger
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class CommentServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly Mock<ILogger<CommentService>> _mockLogger;
        private readonly CommentService _commentService;

        public CommentServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<CommentService>>();
            _commentService = new CommentService(_mockApiConnection.Object, _mockLogger.Object);
        }

        // Helper method to create a list of comments for testing
        private static List<Comment> CreateSampleComments(int count, int startId = 0)
        {
            var comments = new List<Comment>();
            for (int i = 0; i < count; i++)
            {
                var commentText = $"Comment {startId + i}";
                comments.Add(new Comment
                {
                    Id = (startId + i).ToString(),
                    CommentTextEntries = new List<CommentTextEntry> { new CommentTextEntry { Text = commentText } },
                    CommentText = commentText,
                    User = new User(Id: i, Username: $"User {i}", Email: $"user{i}@example.com", Color: "#000000", ProfilePicture: "url", Initials: "U" + i),
                    Resolved = false,
                    Reactions = new List<object>(),
                    Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                    ReplyCount = "0",
                    Assignee = null, // Optional
                    AssignedBy = null // Optional
                });
            }
            return comments;
        }

        // Tests for GetTaskCommentsAsync IAsyncEnumerable
        [Fact]
        public async Task GetTaskCommentsAsync_IAsyncEnumerable_ReturnsAllComments_WhenMultiplePages()
        {
            // Arrange
            var taskId = "task123";
            var firstPageComments = CreateSampleComments(2, 0);
            var secondPageComments = CreateSampleComments(1, 2);

            _mockApiConnection.SetupSequence(api => api.GetAsync<GetTaskCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTaskCommentsResponse(firstPageComments)) // Page 1 (startId=0)
                .ReturnsAsync(new GetTaskCommentsResponse(secondPageComments)) // Page 2 (startId=2)
                .ReturnsAsync(new GetTaskCommentsResponse(new List<Comment>())); // Empty page to terminate

            var allComments = new List<Comment>();

            // Act
            await foreach (var comment in _commentService.GetTaskCommentsStreamAsync(taskId, start: 0, cancellationToken: CancellationToken.None))
            {
                allComments.Add(comment);
            }

            // Assert
            Assert.Equal(3, allComments.Count);
            Assert.Contains(allComments, c => c.Id == "0");
            Assert.Contains(allComments, c => c.Id == "1");
            Assert.Contains(allComments, c => c.Id == "2");

            // Verify the sequence of calls with correct parameters
            // Initial call uses the 'start' timestamp and no 'start_id'
            _mockApiConnection.Verify(api => api.GetAsync<GetTaskCommentsResponse>(
                $"task/{taskId}/comment?start=0", // Initial 'start' is 0, start_id is null
                It.IsAny<CancellationToken>()), Times.Once);

            // Second call uses 'start_id' from the last comment of the first page, and 'start' is null
            _mockApiConnection.Verify(api => api.GetAsync<GetTaskCommentsResponse>(
                $"task/{taskId}/comment?start_id=1", // start_id is "1" (last of firstPageComments)
                It.IsAny<CancellationToken>()), Times.Once);

            // Third call uses 'start_id' from the last comment of the second page, and 'start' is null
            _mockApiConnection.Verify(api => api.GetAsync<GetTaskCommentsResponse>(
                $"task/{taskId}/comment?start_id=2", // start_id is "2" (last of secondPageComments)
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTaskCommentsAsync_IAsyncEnumerable_ReturnsEmpty_WhenNoComments()
        {
            // Arrange
            var taskId = "task_empty";
            _mockApiConnection.Setup(api => api.GetAsync<GetTaskCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTaskCommentsResponse(new List<Comment>()));

            var count = 0;

            // Act
            await foreach (var _ in _commentService.GetTaskCommentsStreamAsync(taskId, start: 0, cancellationToken: CancellationToken.None))
            {
                count++;
            }

            // Assert
            Assert.Equal(0, count);
            _mockApiConnection.Verify(api => api.GetAsync<GetTaskCommentsResponse>(
                $"task/{taskId}/comment?start=0", // Initial call with start=0
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTaskCommentsAsync_IAsyncEnumerable_ReturnsAllComments_WhenSinglePage()
        {
            // Arrange
            var taskId = "task_single_page";
            var comments = CreateSampleComments(2, 0); // e.g., IDs "0", "1"

            _mockApiConnection.SetupSequence(api => api.GetAsync<GetTaskCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTaskCommentsResponse(comments))
                .ReturnsAsync(new GetTaskCommentsResponse(new List<Comment>())); // Empty page to terminate

            var allComments = new List<Comment>();

            // Act
            await foreach (var comment in _commentService.GetTaskCommentsStreamAsync(taskId, start: 0, cancellationToken: CancellationToken.None))
            {
                allComments.Add(comment);
            }

            // Assert
            Assert.Equal(2, allComments.Count);
            Assert.Contains(allComments, c => c.Id == "0");
            Assert.Contains(allComments, c => c.Id == "1");
            _mockApiConnection.Verify(api => api.GetAsync<GetTaskCommentsResponse>(
                $"task/{taskId}/comment?start=0", // Initial call
                It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(api => api.GetAsync<GetTaskCommentsResponse>(
                $"task/{taskId}/comment?start_id=1", // Second call with start_id from last comment
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTaskCommentsAsync_IAsyncEnumerable_HandlesCancellation()
        {
            // Arrange
            var taskId = "task_cancel";
            var firstPageComments = CreateSampleComments(2, 0);
            var cts = new CancellationTokenSource();

            _mockApiConnection.Setup(api => api.GetAsync<GetTaskCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTaskCommentsResponse(firstPageComments));

            var commentsProcessed = 0;

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var comment in _commentService.GetTaskCommentsStreamAsync(taskId, start: 0, cancellationToken: cts.Token))
                {
                    commentsProcessed++;
                    if (commentsProcessed == 1)
                    {
                        cts.Cancel();
                    }
                }
            });

            Assert.Equal(1, commentsProcessed); // Only one comment should be processed before cancellation
            _mockApiConnection.Verify(api => api.GetAsync<GetTaskCommentsResponse>(
                $"task/{taskId}/comment?start=0",
                It.IsAny<CancellationToken>()), Times.Once); // Underlying method called once
        }

        [Fact]
        public async Task GetTaskCommentsAsync_IAsyncEnumerable_HandlesApiError()
        {
            // Arrange
            var taskId = "task_api_error";
            _mockApiConnection.Setup(api => api.GetAsync<GetTaskCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await foreach (var _ in _commentService.GetTaskCommentsStreamAsync(taskId, start: 0, cancellationToken: CancellationToken.None))
                {
                    // Should not reach here
                }
            });
            _mockApiConnection.Verify(api => api.GetAsync<GetTaskCommentsResponse>(
                $"task/{taskId}/comment?start=0",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // Tests for GetListCommentsStreamAsync IAsyncEnumerable
        [Fact]
        public async Task GetListCommentsStreamAsync_IAsyncEnumerable_ReturnsAllComments_WhenMultiplePages()
        {
            // Arrange
            var listId = "list123";
            var firstPageComments = CreateSampleComments(2, 0); // IDs "0", "1"
            var secondPageComments = CreateSampleComments(1, 2); // ID "2"

            _mockApiConnection.SetupSequence(api => api.GetAsync<GetListCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListCommentsResponse(firstPageComments))
                .ReturnsAsync(new GetListCommentsResponse(secondPageComments))
                .ReturnsAsync(new GetListCommentsResponse(new List<Comment>()));

            var allComments = new List<Comment>();

            // Act
            await foreach (var comment in _commentService.GetListCommentsStreamAsync(listId, start: 0, cancellationToken: CancellationToken.None))
            {
                allComments.Add(comment);
            }

            // Assert
            Assert.Equal(3, allComments.Count);
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>( $"list/{listId}/comment?start=0", It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>( $"list/{listId}/comment?start_id=1", It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>( $"list/{listId}/comment?start_id=2", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListCommentsStreamAsync_IAsyncEnumerable_ReturnsEmpty_WhenNoComments()
        {
            // Arrange
            var listId = "list_empty";
            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListCommentsResponse(new List<Comment>()));
            var count = 0;

            // Act
            await foreach (var _ in _commentService.GetListCommentsStreamAsync(listId, start: 0, cancellationToken: CancellationToken.None))
            {
                count++;
            }

            // Assert
            Assert.Equal(0, count);
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>( $"list/{listId}/comment?start=0", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListCommentsStreamAsync_IAsyncEnumerable_HandlesCancellation()
        {
            // Arrange
            var listId = "list_cancel";
            var firstPageComments = CreateSampleComments(2, 0);
            var cts = new CancellationTokenSource();

            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListCommentsResponse(firstPageComments));
            var commentsProcessed = 0;

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var comment in _commentService.GetListCommentsStreamAsync(listId, start: 0, cancellationToken: cts.Token))
                {
                    commentsProcessed++;
                    if (commentsProcessed == 1) cts.Cancel();
                }
            });
            Assert.Equal(1, commentsProcessed);
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>( $"list/{listId}/comment?start=0", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListCommentsStreamAsync_IAsyncEnumerable_HandlesApiError()
        {
            // Arrange
            var listId = "list_api_error";
            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await foreach (var _ in _commentService.GetListCommentsStreamAsync(listId, start: 0, cancellationToken: CancellationToken.None)) { }
            });
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>( $"list/{listId}/comment?start=0", It.IsAny<CancellationToken>()), Times.Once);
        }

        // Tests for GetChatViewCommentsStreamAsync IAsyncEnumerable
        [Fact]
        public async Task GetChatViewCommentsStreamAsync_IAsyncEnumerable_ReturnsAllComments_WhenMultiplePages()
        {
            // Arrange
            var viewId = "view123";
            var firstPageComments = CreateSampleComments(2, 0); // IDs "0", "1"
            var secondPageComments = CreateSampleComments(1, 2); // ID "2"

            _mockApiConnection.SetupSequence(api => api.GetAsync<GetListCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListCommentsResponse(firstPageComments))
                .ReturnsAsync(new GetListCommentsResponse(secondPageComments))
                .ReturnsAsync(new GetListCommentsResponse(new List<Comment>()));

            var allComments = new List<Comment>();

            // Act
            await foreach (var comment in _commentService.GetChatViewCommentsStreamAsync(viewId, start: 0, cancellationToken: CancellationToken.None))
            {
                allComments.Add(comment);
            }

            // Assert
            Assert.Equal(3, allComments.Count);
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>( $"view/{viewId}/comment?start=0", It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>( $"view/{viewId}/comment?start_id=1", It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>( $"view/{viewId}/comment?start_id=2", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatViewCommentsStreamAsync_IAsyncEnumerable_ReturnsEmpty_WhenNoComments()
        {
            // Arrange
            var viewId = "view_empty";
            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListCommentsResponse(new List<Comment>()));
            var count = 0;

            // Act
            await foreach (var _ in _commentService.GetChatViewCommentsStreamAsync(viewId, start: 0, cancellationToken: CancellationToken.None))
            {
                count++;
            }

            // Assert
            Assert.Equal(0, count);
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>(
                $"view/{viewId}/comment?start=0", // Corrected to use viewId and view endpoint
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatViewCommentsStreamAsync_IAsyncEnumerable_HandlesCancellation()
        {
            // Arrange
            var viewId = "view_cancel";
            var firstPageComments = CreateSampleComments(2, 0);
            var cts = new CancellationTokenSource();

            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListCommentsResponse(firstPageComments));
            var commentsProcessed = 0;

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var comment in _commentService.GetChatViewCommentsStreamAsync(viewId, start: 0, cancellationToken: cts.Token))
                {
                    commentsProcessed++;
                    if (commentsProcessed == 1) cts.Cancel();
                }
            });
            Assert.Equal(1, commentsProcessed);
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>( $"view/{viewId}/comment?start=0", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatViewCommentsStreamAsync_IAsyncEnumerable_HandlesApiError()
        {
            // Arrange
            var viewId = "view_api_error";
            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await foreach (var _ in _commentService.GetChatViewCommentsStreamAsync(viewId, start: 0, cancellationToken: CancellationToken.None)) { }
            });
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>( $"view/{viewId}/comment?start=0", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
