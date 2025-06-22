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
using System.Collections.Generic; // For List<string>

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

            _mockApiConnection.Setup(x => x.PostMultipartAsync<CreateTaskAttachmentResponse>( // Changed to CreateTaskAttachmentResponse
                    It.IsAny<string>(),
                    It.IsAny<MultipartFormDataContent>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((CreateTaskAttachmentResponse?)null); // Changed to CreateTaskAttachmentResponse

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
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
        public async Task CreateTaskAttachmentAsync_ApiConnectionThrowsException_PropagatesException()
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
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _attachmentsService.CreateTaskAttachmentAsync(
                    taskId,
                    memoryStream,
                    fileName,
                    false,
                    null,
                    CancellationToken.None)
            );
        }
    }
}
