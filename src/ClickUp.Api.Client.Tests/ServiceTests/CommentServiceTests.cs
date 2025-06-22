using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.ResponseModels.Comments;
using ClickUp.Api.Client.Models.RequestModels.Comments; // Added for request models
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
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

        [Fact]
        public async Task CreateTaskCommentAsync_ValidInput_CallsApiConnectionPostAsyncWithCorrectParameters()
        {
            // Arrange
            var taskId = "task123";
            var request = new CreateCommentRequest { CommentText = "Test comment", Assignee = 123, NotifyAll = null };
            var mockUser = new User(1, "User", "user@example.com", "#fff", null, "U");
            var expectedComment = new Comment
            {
                Id = "comment_1",
                CommentTextEntries = new List<CommentTextEntry> { new CommentTextEntry { Text = "Test comment" } },
                CommentText = "Test comment",
                User = mockUser,
                Resolved = false,
                Reactions = new List<object>(),
                Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                ReplyCount = "0"
            };
            var expectedResponse = new CreateCommentResponse { Id = "comment_1", Comment = expectedComment };

            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<CreateCommentRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _commentService.CreateTaskCommentAsync(taskId, request, null, null, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Id, result.Id);
            _mockApiConnection.Verify(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                $"task/{taskId}/comment",
                request,
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateTaskCommentAsync_WithCustomTaskIdsAndTeamId_BuildsCorrectEndpoint()
        {
            // Arrange
            var taskId = "custom-task-id";
            var request = new CreateCommentRequest { CommentText = "Test comment", Assignee = 123, NotifyAll = null };
            var customTaskIds = true;
            var teamId = "team-123";
            var mockUser = new User(2, "User2", "user2@example.com", "#000", null, "U2");
            var expectedComment = new Comment
            {
                Id = "comment_2",
                CommentTextEntries = new List<CommentTextEntry> { new CommentTextEntry { Text = "Test comment" } },
                CommentText = "Test comment",
                User = mockUser,
                Resolved = false,
                Reactions = new List<object>(),
                Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                ReplyCount = "0"
            };
            var expectedResponse = new CreateCommentResponse { Id = "comment_2", Comment = expectedComment };

            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<CreateCommentRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            await _commentService.CreateTaskCommentAsync(taskId, request, customTaskIds, teamId, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                $"task/{taskId}/comment?custom_task_ids=true&team_id={teamId}",
                request,
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateChatViewCommentAsync_ValidInput_CallsApiConnectionPostAsyncWithCorrectParameters()
        {
            // Arrange
            var viewId = "view123";
            var request = new CreateCommentRequest { CommentText = "Test comment", Assignee = 123, NotifyAll = null };
            var mockUser = new User(3, "User3", "user3@example.com", "#111", null, "U3");
            var expectedComment = new Comment
            {
                Id = "comment_3",
                CommentTextEntries = new List<CommentTextEntry> { new CommentTextEntry { Text = "Test comment" } },
                CommentText = "Test comment",
                User = mockUser,
                Resolved = false,
                Reactions = new List<object>(),
                Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                ReplyCount = "0"
            };
            var expectedResponse = new CreateCommentResponse { Id = "comment_3", Comment = expectedComment };

            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<CreateCommentRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _commentService.CreateChatViewCommentAsync(viewId, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Id, result.Id);
            _mockApiConnection.Verify(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                $"view/{viewId}/comment",
                request,
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateListCommentAsync_ValidInput_CallsApiConnectionPostAsyncWithCorrectParameters()
        {
            // Arrange
            var listId = "list123";
            var request = new CreateCommentRequest { CommentText = "Test comment", Assignee = 123, NotifyAll = null };
            var mockUser = new User(4, "User4", "user4@example.com", "#222", null, "U4");
            var expectedComment = new Comment
            {
                Id = "comment_4",
                CommentTextEntries = new List<CommentTextEntry> { new CommentTextEntry { Text = "Test comment" } },
                CommentText = "Test comment",
                User = mockUser,
                Resolved = false,
                Reactions = new List<object>(),
                Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                ReplyCount = "0"
            };
            var expectedResponse = new CreateCommentResponse { Id = "comment_4", Comment = expectedComment };

            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<CreateCommentRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _commentService.CreateListCommentAsync(listId, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Id, result.Id);
            _mockApiConnection.Verify(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                $"list/{listId}/comment",
                request,
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task UpdateCommentAsync_ValidInput_CallsApiConnectionPutAsyncWithCorrectParameters()
        {
            // Arrange
            var commentId = "comment123";
            var request = new UpdateCommentRequest("Updated comment", null, false, null);
            var mockUser = new User(0, "", "", "", null, ""); // Add a default user or use a specific one
            var expectedResponse = new Comment
            {
                Id = commentId,
                CommentText = "Updated comment",
                CommentTextEntries = new List<CommentTextEntry> { new CommentTextEntry { Text = "Updated comment" } },
                User = mockUser,
                Resolved = false,
                Reactions = new List<object>(),
                Date = "",
                ReplyCount = "0"
            };

            _mockApiConnection.Setup(api => api.PutAsync<UpdateCommentRequest, Comment>(
                    It.IsAny<string>(),
                    It.IsAny<UpdateCommentRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _commentService.UpdateCommentAsync(commentId, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Id, result.Id);
            _mockApiConnection.Verify(api => api.PutAsync<UpdateCommentRequest, Comment>(
                $"comment/{commentId}",
                request,
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task DeleteCommentAsync_ValidInput_CallsApiConnectionDeleteAsyncWithCorrectParameters()
        {
            // Arrange
            var commentId = "comment123";
            _mockApiConnection.Setup(api => api.DeleteAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            await _commentService.DeleteCommentAsync(commentId, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(api => api.DeleteAsync(
                $"comment/{commentId}",
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetThreadedCommentsAsync_ValidInput_CallsApiConnectionGetAsyncWithCorrectParameters()
        {
            // Arrange
            var commentId = "comment123";
            var expectedResponse = new GetListCommentsResponse(CreateSampleComments(2)); // Assuming GetListCommentsResponse is used for replies

            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _commentService.GetThreadedCommentsAsync(commentId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>(
                $"comment/{commentId}/reply",
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateThreadedCommentAsync_ValidInput_CallsApiConnectionPostAsyncWithCorrectParameters()
        {
            // Arrange
            var commentId = "comment123";
            var request = new CreateCommentRequest { CommentText = "Threaded comment", Assignee = 456, NotifyAll = null };
            var mockUser = new User(5, "User5", "user5@example.com", "#333", null, "U5");
            var expectedComment = new Comment
            {
                Id = "comment_5",
                CommentTextEntries = new List<CommentTextEntry> { new CommentTextEntry { Text = "Threaded comment" } },
                CommentText = "Threaded comment",
                User = mockUser,
                Resolved = false,
                Reactions = new List<object>(),
                Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                ReplyCount = "0"
            };
            var expectedResponse = new CreateCommentResponse { Id = "comment_5", Comment = expectedComment };

            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<CreateCommentRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _commentService.CreateThreadedCommentAsync(commentId, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Id, result.Id);
            _mockApiConnection.Verify(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                $"comment/{commentId}/reply",
                request,
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateTaskCommentAsync_ApiConnectionThrowsException_PropagatesException()
        {
            // Arrange
            var taskId = "task123";
            var request = new CreateCommentRequest { CommentText = "Test comment", Assignee = 123, NotifyAll = null };

            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<CreateCommentRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _commentService.CreateTaskCommentAsync(taskId, request, null, null, CancellationToken.None));
        }

        [Fact]
        public async Task GetTaskCommentsAsync_ApiConnectionThrowsException_PropagatesException()
        {
            // Arrange
            var taskId = "task123";

            _mockApiConnection.Setup(api => api.GetAsync<GetTaskCommentsResponse>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _commentService.GetTaskCommentsAsync(taskId, null, null, null, null, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateCommentAsync_ApiConnectionThrowsException_PropagatesException()
        {
            // Arrange
            var commentId = "comment123";
            var request = new UpdateCommentRequest("Updated comment", null, false, null);

            _mockApiConnection.Setup(api => api.PutAsync<UpdateCommentRequest, Comment>(
                    It.IsAny<string>(),
                    It.IsAny<UpdateCommentRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _commentService.UpdateCommentAsync(commentId, request, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteCommentAsync_ApiConnectionThrowsException_PropagatesException()
        {
            // Arrange
            var commentId = "comment123";

            _mockApiConnection.Setup(api => api.DeleteAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _commentService.DeleteCommentAsync(commentId, CancellationToken.None));
        }

        // --- Tests for TaskCanceledException and CancellationToken pass-through ---

        [Fact]
        public async Task CreateTaskCommentAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var taskId = "task_cancel_ex";
            var request = new CreateCommentRequest { CommentText = "Test" };
            var cts = new CancellationTokenSource();
            cts.Cancel(); // Simulate timeout/cancellation

            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(), It.IsAny<CreateCommentRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _commentService.CreateTaskCommentAsync(taskId, request, null, null, cts.Token));
        }

        [Fact]
        public async Task CreateTaskCommentAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var taskId = "task_ct_pass";
            var request = new CreateCommentRequest { CommentText = "Test" };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var mockUser = new User(0, "", "", "", null, "");
            var expectedResponse = new CreateCommentResponse { Id = "1", Comment = new Comment { Id = "1", User = mockUser } };


            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(), It.IsAny<CreateCommentRequest>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            // Act
            await _commentService.CreateTaskCommentAsync(taskId, request, null, null, expectedToken);

            // Assert
            _mockApiConnection.Verify(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                $"task/{taskId}/comment", request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetTaskCommentsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var taskId = "task_cancel_ex_get";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockApiConnection.Setup(api => api.GetAsync<GetTaskCommentsResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _commentService.GetTaskCommentsAsync(taskId, null, null, null, null, cts.Token));
        }

        [Fact]
        public async Task GetTaskCommentsAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var taskId = "task_ct_pass_get";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetTaskCommentsResponse(new List<Comment>());

            _mockApiConnection.Setup(api => api.GetAsync<GetTaskCommentsResponse>(
                    It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            // Act
            await _commentService.GetTaskCommentsAsync(taskId, null, null, null, null, expectedToken);

            // Assert
            _mockApiConnection.Verify(api => api.GetAsync<GetTaskCommentsResponse>(
                $"task/{taskId}/comment", expectedToken), Times.Once);
        }


        [Fact]
        public async Task CreateChatViewCommentAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var viewId = "view_cancel_ex";
            var request = new CreateCommentRequest { CommentText = "Test" };
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(), It.IsAny<CreateCommentRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _commentService.CreateChatViewCommentAsync(viewId, request, cts.Token));
        }

        [Fact]
        public async Task CreateChatViewCommentAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var viewId = "view_ct_pass";
            var request = new CreateCommentRequest { CommentText = "Test" };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var mockUser = new User(0, "", "", "", null, "");
            var expectedResponse = new CreateCommentResponse { Id = "1", Comment = new Comment { Id = "1", User = mockUser } };


            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(), It.IsAny<CreateCommentRequest>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            // Act
            await _commentService.CreateChatViewCommentAsync(viewId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                $"view/{viewId}/comment", request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetChatViewCommentsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var viewId = "view_cancel_ex_get";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>( // Uses GetListCommentsResponse
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _commentService.GetChatViewCommentsAsync(viewId, null, null, cts.Token));
        }

        [Fact]
        public async Task GetChatViewCommentsAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var viewId = "view_ct_pass_get";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetListCommentsResponse(new List<Comment>()); // Uses GetListCommentsResponse

            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>(
                    It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            // Act
            await _commentService.GetChatViewCommentsAsync(viewId, null, null, expectedToken);

            // Assert
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>(
                $"view/{viewId}/comment", expectedToken), Times.Once);
        }

        [Fact]
        public async Task CreateListCommentAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var listId = "list_cancel_ex";
            var request = new CreateCommentRequest { CommentText = "Test" };
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(), It.IsAny<CreateCommentRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _commentService.CreateListCommentAsync(listId, request, cts.Token));
        }

        [Fact]
        public async Task CreateListCommentAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var listId = "list_ct_pass";
            var request = new CreateCommentRequest { CommentText = "Test" };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var mockUser = new User(0, "", "", "", null, "");
            var expectedResponse = new CreateCommentResponse { Id = "1", Comment = new Comment { Id = "1", User = mockUser } };


            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(), It.IsAny<CreateCommentRequest>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            // Act
            await _commentService.CreateListCommentAsync(listId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                $"list/{listId}/comment", request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetListCommentsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var listId = "list_cancel_ex_get";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _commentService.GetListCommentsAsync(listId, null, null, cts.Token));
        }

        [Fact]
        public async Task GetListCommentsAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var listId = "list_ct_pass_get";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetListCommentsResponse(new List<Comment>());

            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>(
                    It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            // Act
            await _commentService.GetListCommentsAsync(listId, null, null, expectedToken);

            // Assert
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>(
                $"list/{listId}/comment", expectedToken), Times.Once);
        }

        [Fact]
        public async Task UpdateCommentAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var commentId = "comment_cancel_ex";
            var request = new UpdateCommentRequest("Updated", null, false, null);
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockApiConnection.Setup(api => api.PutAsync<UpdateCommentRequest, Comment>(
                    It.IsAny<string>(), It.IsAny<UpdateCommentRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _commentService.UpdateCommentAsync(commentId, request, cts.Token));
        }

        [Fact]
        public async Task UpdateCommentAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var commentId = "comment_ct_pass";
            var request = new UpdateCommentRequest("Updated", null, false, null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var mockUser = new User(0, "", "", "", null, "");
            var expectedResponse = new Comment { Id = commentId, User = mockUser };


            _mockApiConnection.Setup(api => api.PutAsync<UpdateCommentRequest, Comment>(
                    It.IsAny<string>(), It.IsAny<UpdateCommentRequest>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            // Act
            await _commentService.UpdateCommentAsync(commentId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(api => api.PutAsync<UpdateCommentRequest, Comment>(
                $"comment/{commentId}", request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task DeleteCommentAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var commentId = "comment_cancel_ex_delete";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockApiConnection.Setup(api => api.DeleteAsync(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _commentService.DeleteCommentAsync(commentId, cts.Token));
        }

        [Fact]
        public async Task DeleteCommentAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var commentId = "comment_ct_pass_delete";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection.Setup(api => api.DeleteAsync(
                    It.IsAny<string>(), expectedToken))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            await _commentService.DeleteCommentAsync(commentId, expectedToken);

            // Assert
            _mockApiConnection.Verify(api => api.DeleteAsync(
                $"comment/{commentId}", expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetThreadedCommentsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var commentId = "comment_thread_cancel_ex_get";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>( // Uses GetListCommentsResponse
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _commentService.GetThreadedCommentsAsync(commentId, cts.Token));
        }

        [Fact]
        public async Task GetThreadedCommentsAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var commentId = "comment_thread_ct_pass_get";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetListCommentsResponse(new List<Comment>());

            _mockApiConnection.Setup(api => api.GetAsync<GetListCommentsResponse>(
                    It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            // Act
            await _commentService.GetThreadedCommentsAsync(commentId, expectedToken);

            // Assert
            _mockApiConnection.Verify(api => api.GetAsync<GetListCommentsResponse>(
                $"comment/{commentId}/reply", expectedToken), Times.Once);
        }

        [Fact]
        public async Task CreateThreadedCommentAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var commentId = "comment_thread_cancel_ex_create";
            var request = new CreateCommentRequest { CommentText = "Test" };
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(), It.IsAny<CreateCommentRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _commentService.CreateThreadedCommentAsync(commentId, request, cts.Token));
        }

        [Fact]
        public async Task CreateThreadedCommentAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var commentId = "comment_thread_ct_pass_create";
            var request = new CreateCommentRequest { CommentText = "Test" };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var mockUser = new User(0, "", "", "", null, "");
            var expectedResponse = new CreateCommentResponse { Id = "1", Comment = new Comment { Id = "1", User = mockUser } };

            _mockApiConnection.Setup(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                    It.IsAny<string>(), It.IsAny<CreateCommentRequest>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            // Act
            await _commentService.CreateThreadedCommentAsync(commentId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(api => api.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                $"comment/{commentId}/reply", request, expectedToken), Times.Once);
        }
    }
}
