using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using ClickUp.Api.Client.Models.ResponseModels.Comments;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using ClickUp.Api.Client.Models.Entities.Comments;

namespace ClickUp.Api.Client.Tests.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly CommentService _commentService;

        public CommentServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _commentService = new CommentService(_mockApiConnection.Object);
        }

        // Placeholder for creating Comment DTO instances. Actual DTO structure will determine how this is done.
        private Comment CreateSampleComment(string id, string text)
        {
            // This is a simplified way. If Comment is a record class, this is easy.
            // If it has a complex constructor or init-only properties, more setup is needed.
            // For now, assuming a way to construct/mock it for testing.
            var comment = (Comment)Activator.CreateInstance(typeof(Comment), nonPublic: true)!;
            typeof(Comment).GetProperty("Id")?.SetValue(comment, id);
            // typeof(Comment).GetProperty("CommentText")?.SetValue(comment, text); // Assuming a property for text
            // Add other necessary properties
            return comment;
        }

        private CreateCommentResponse CreateSampleCreateCommentResponse(string id, string historyId, DateTime dateCreated)
        {
             return new CreateCommentResponse(id, historyId, dateCreated);
        }


        [Fact]
        public async Task GetTaskCommentsAsync_WhenCommentsExist_ReturnsComments()
        {
            // Arrange
            var taskId = "test-task-id";
            var expectedComments = new List<Comment> { CreateSampleComment("comment1", "First comment") };
            var expectedResponse = new GetCommentsResponse(expectedComments); // Assuming GetCommentsResponse wraps List<Comment>

            _mockApiConnection.Setup(c => c.GetAsync<GetCommentsResponse>(
                It.Is<string>(s => s.StartsWith($"task/{taskId}/comment")),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _commentService.GetTaskCommentsAsync(taskId, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedComments);
            _mockApiConnection.Verify(c => c.GetAsync<GetCommentsResponse>(
                It.Is<string>(s => s.StartsWith($"task/{taskId}/comment")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateTaskCommentAsync_ValidRequest_ReturnsCreatedCommentResponse()
        {
            // Arrange
            var taskId = "test-task-id";
            var requestDto = new CreateCommentRequest("New comment", null, false);
            var expectedResponse = CreateSampleCreateCommentResponse("new-comment-id", "hist-id", DateTime.UtcNow);

            _mockApiConnection.Setup(c => c.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                It.Is<string>(s => s.StartsWith($"task/{taskId}/comment")),
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _commentService.CreateTaskCommentAsync(taskId, requestDto, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
            _mockApiConnection.Verify(c => c.PostAsync<CreateCommentRequest, CreateCommentResponse>(
                It.Is<string>(s => s.StartsWith($"task/{taskId}/comment")),
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCommentAsync_ValidRequest_CallsApiConnectionPutAsync()
        {
            // Arrange
            var commentId = "comment-to-update";
            var requestDto = new UpdateCommentRequest("Updated text", null, false, false);
            var expectedEndpoint = $"comment/{commentId}";

            _mockApiConnection.Setup(c => c.PutAsync<UpdateCommentRequest>(
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask); // UpdateCommentAsync in service is void

            // Act
            await _commentService.UpdateCommentAsync(commentId, requestDto, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.PutAsync<UpdateCommentRequest>(
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCommentAsync_ValidId_CallsApiConnectionDeleteAsync()
        {
            // Arrange
            var commentId = "comment-to-delete";
            var expectedEndpoint = $"comment/{commentId}";

            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _commentService.DeleteCommentAsync(commentId, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
