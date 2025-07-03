using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.ResponseModels.Attachments; // Changed to use Response DTO
using ClickUp.Api.Client.Models.Entities.Users; // For User model within response
using ClickUp.Api.Client.Services;
using Moq;
using Xunit;
using System.Collections.Generic;
using ClickUp.Api.Client.Models.Exceptions; // For List<string>

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class AttachmentsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly AttachmentsService _attachmentsService;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<AttachmentsService>> _mockLogger;

        public AttachmentsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<AttachmentsService>>();
            _attachmentsService = new AttachmentsService(_mockApiConnection.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateTaskAttachmentAsync_ValidInput_CallsApiConnectionPostMultipartAsync()
        {
            // Arrange
            var taskId = "test-task-id";
            var customTaskIds = false;
            string? teamId = null;
            var fileName = "test-file.txt";
            var fileContent = "This is a test file.";
            using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

            var mockUser = new User(Id: 123, Username: "Test User", Email: "test@example.com", Color: "#ffffff", ProfilePicture: null, Initials: "TU", ProfileInfo: null);
            var expectedResponse = new CreateTaskAttachmentResponse(
                Id: "generated-id",
                Version: "1",
                Date: DateTimeOffset.UtcNow,
                Title: fileName,
                Extension: "txt",
                ThumbnailSmall: "http://example.com/thumb_small.txt",
                ThumbnailLarge: "http://example.com/thumb_large.txt",
                Url: "http://example.com/url.txt",
                UrlWQuery: "http://example.com/url.txt?query=1",
                UrlWHost: "https://host.com/url.txt",
                IsFolder: false,
                ParentId: "parent-id",
                Size: 1024,
                TotalComments: 0,
                ResolvedComments: 0,
                User: mockUser,
                Deleted: false,
                Orientation: null,
                Type: 1,
                Source: 1,
                EmailData: null,
                ResourceId: "resource-id"
            );

            _mockApiConnection.Setup(x => x.PostMultipartAsync<CreateTaskAttachmentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<MultipartFormDataContent>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _attachmentsService.CreateTaskAttachmentAsync(
                taskId,
                memoryStream,
                fileName,
                customTaskIds,
                teamId,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Id, result.Id);
            Assert.Equal(expectedResponse.Title, result.Title);

            _mockApiConnection.Verify(x => x.PostMultipartAsync<CreateTaskAttachmentResponse>(
                $"task/{taskId}/attachment?custom_task_ids=false",
                It.IsAny<MultipartFormDataContent>(), // Simplified check for unit test
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateTaskAttachmentAsync_WithCustomTaskIdsAndTeamId_BuildsCorrectEndpoint()
        {
            // Arrange
            var taskId = "custom-task-id";
            var customTaskIds = true;
            var teamId = "test-team-id";
            var fileName = "test-file.txt";
            var fileContent = "This is a test file.";
            using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

            var mockUser = new User(Id: 124, Username: "Test User 2", Email: "test2@example.com", Color: "#000000", ProfilePicture: null, Initials: "TU2", ProfileInfo: null);
            var expectedResponse = new CreateTaskAttachmentResponse(
                Id: "id", Version: "v", Date: DateTimeOffset.FromUnixTimeMilliseconds(123), Title: fileName, Extension: "ext", ThumbnailSmall: "ts",
                ThumbnailLarge: "tl", Url: "url", UrlWQuery: "url?q=2", UrlWHost: "host2/url",
                IsFolder: false, ParentId: "parent2", Size: 2048, TotalComments: 1, ResolvedComments: 0,
                User: mockUser, Deleted: false, Orientation: null, Type: 2, Source: 2,
                EmailData: null, ResourceId: "resource2"
            );


            _mockApiConnection.Setup(x => x.PostMultipartAsync<CreateTaskAttachmentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<MultipartFormDataContent>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            await _attachmentsService.CreateTaskAttachmentAsync(
                taskId,
                memoryStream,
                fileName,
                customTaskIds,
                teamId,
                CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(x => x.PostMultipartAsync<CreateTaskAttachmentResponse>(
                $"task/{taskId}/attachment?custom_task_ids=true&team_id={teamId}",
                It.IsAny<MultipartFormDataContent>(),
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateTaskAttachmentAsync_ApiConnectionReturnsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            var taskId = "test-task-id";
            var fileName = "test-file.txt";
            var fileContent = "This is a test file.";
            using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

            _mockApiConnection.Setup(x => x.PostMultipartAsync<CreateTaskAttachmentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<MultipartFormDataContent>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((CreateTaskAttachmentResponse?)null);

            // Act & Assert
            var expectedException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _attachmentsService.CreateTaskAttachmentAsync(
                    taskId,
                    memoryStream,
                    fileName,
                    false,
                    null,
                    CancellationToken.None)
            );

            Assert.NotNull(expectedException);
            Assert.Equal($"Failed to create attachment for task {taskId}, or the API returned an unexpected null or invalid response.", expectedException.Message);
        }

        [Fact]
        public async Task CreateTaskAttachmentAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var taskId = "test-task-id";
            var fileName = "test-file.txt";
            var fileContent = "This is a test file.";
            using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
            var cancellationTokenSource = new CancellationTokenSource();

            var mockUser = new User(Id: 1, Username: "dummy", Email: "dummy@example.com", Color: "", ProfilePicture: null, Initials: "DU", ProfileInfo: null);
            var dummyResponse = new CreateTaskAttachmentResponse("id", "v", DateTimeOffset.UtcNow, "title", "ext", null, null, "url", null, null, false, "pid", 0,0,0, mockUser, false, null, 0,0, null, null);


            _mockApiConnection.Setup(x => x.PostMultipartAsync<CreateTaskAttachmentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<MultipartFormDataContent>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, MultipartFormDataContent, CancellationToken>((url, content, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse); // Provide a dummy response for the non-cancelled path

            cancellationTokenSource.Cancel(); // Cancel the token

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _attachmentsService.CreateTaskAttachmentAsync(
                    taskId,
                    memoryStream,
                    fileName,
                    false,
                    null,
                    cancellationTokenSource.Token) // Pass the cancelled token
            );
        }

        [Fact]
        public async Task CreateTaskAttachmentAsync_ApiConnectionThrowsException_PropagatesException()
        {
            // Arrange
            var taskId = "test-task-id";
            var fileName = "test-file.txt";
            var fileContent = "This is a test file.";
            using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

            var expectedException = new ClickUpApiNotFoundException("Resource not found", System.Net.HttpStatusCode.NotFound, "NF_TEST_001", "{\"err\":\"Resource not found\",\"ECODE\":\"NF_TEST_001\"}");
            _mockApiConnection.Setup(x => x.PostMultipartAsync<CreateTaskAttachmentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<MultipartFormDataContent>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<ClickUpApiNotFoundException>(() =>
                _attachmentsService.CreateTaskAttachmentAsync(
                    taskId,
                    memoryStream,
                    fileName,
                    false,
                    null,
                    CancellationToken.None)
            );
        }

        [Fact]
        public async Task CreateTaskAttachmentAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var taskId = "test-task-id";
            var fileName = "test-file.txt";
            var fileContent = "This is a test file.";
            using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel(); // Pre-cancel to simulate immediate timeout or cancellation

            _mockApiConnection.Setup(x => x.PostMultipartAsync<CreateTaskAttachmentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<MultipartFormDataContent>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _attachmentsService.CreateTaskAttachmentAsync(
                    taskId,
                    memoryStream,
                    fileName,
                    false,
                    null,
                    cancellationTokenSource.Token) // Pass the cancelled token
            );
        }

        [Fact]
        public async Task CreateTaskAttachmentAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var taskId = "test-task-id";
            var fileName = "test-file.txt";
            var fileContent = "This is a test file.";
            using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            var mockUser = new User(Id: 123, Username: "Test User", Email: "test@example.com", Color: "#ffffff", ProfilePicture: null, Initials: "TU", ProfileInfo: null);
            var expectedResponse = new CreateTaskAttachmentResponse(
                Id: "generated-id", Version: "1", Date: DateTimeOffset.UtcNow, Title: fileName, Extension: "txt", ThumbnailSmall: "ts", ThumbnailLarge: "tl",
                Url: "url", UrlWQuery: "url?q", UrlWHost: "host/url", IsFolder: false, ParentId: "pid", Size: 100, TotalComments: 0, ResolvedComments: 0, User: mockUser,
                Deleted: false, Orientation: null, Type: 1, Source: 1, EmailData: null, ResourceId: "rid"
            );


            _mockApiConnection.Setup(x => x.PostMultipartAsync<CreateTaskAttachmentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<MultipartFormDataContent>(),
                    expectedToken)) // Expect the specific token
                .ReturnsAsync(expectedResponse);

            // Act
            await _attachmentsService.CreateTaskAttachmentAsync(
                taskId,
                memoryStream,
                fileName,
                false,
                null,
                expectedToken); // Pass the token

            // Assert
            _mockApiConnection.Verify(x => x.PostMultipartAsync<CreateTaskAttachmentResponse>(
                It.IsAny<string>(),
                It.IsAny<MultipartFormDataContent>(),
                expectedToken), // Verify the token was passed
                Times.Once);
        }
    }
}
