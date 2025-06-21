using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Attachments; // Corrected Namespace
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

        public AttachmentsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _attachmentsService = new AttachmentsService(_mockApiConnection.Object);
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

            var expectedAttachment = new Attachment(
                Id: "generated-id",
                Version: "1",
                Date: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Title: fileName,
                Extension: "txt",
                ThumbnailSmall: "http://example.com/thumb_small.txt",
                ThumbnailLarge: "http://example.com/thumb_large.txt",
                Url: "http://example.com/url.txt",
                Orientation: null,
                Type: null,
                Source: null,
                EmailFrom: null,
                EmailTo: null,
                EmailCc: null,
                EmailReplyTo: null,
                EmailDate: null,
                EmailSubject: null,
                EmailPreview: null,
                EmailTextContent: null,
                EmailHtmlContentId: null,
                EmailAttachmentsCount: null,
                User: null
            );

            _mockApiConnection.Setup(x => x.PostMultipartAsync<Attachment>(
                    It.IsAny<string>(),
                    It.IsAny<MultipartFormDataContent>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAttachment);

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
            Assert.Equal(expectedAttachment.Id, result.Id);
            Assert.Equal(expectedAttachment.Title, result.Title);

            _mockApiConnection.Verify(x => x.PostMultipartAsync<Attachment>(
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

            var expectedAttachment = new Attachment(
                Id: "id", Version: "v", Date: 123, Title: "title", Extension: "ext", ThumbnailSmall: "ts",
                ThumbnailLarge: "tl", Url: "url", Orientation: null, Type: null, Source: null,
                EmailFrom: null, EmailTo: null, EmailCc: null, EmailReplyTo: null, EmailDate: null,
                EmailSubject: null, EmailPreview: null, EmailTextContent: null, EmailHtmlContentId: null,
                EmailAttachmentsCount: null, User: null
            );


            _mockApiConnection.Setup(x => x.PostMultipartAsync<Attachment>(
                    It.IsAny<string>(),
                    It.IsAny<MultipartFormDataContent>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAttachment);

            // Act
            await _attachmentsService.CreateTaskAttachmentAsync(
                taskId,
                memoryStream,
                fileName,
                customTaskIds,
                teamId,
                CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(x => x.PostMultipartAsync<Attachment>(
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

            _mockApiConnection.Setup(x => x.PostMultipartAsync<Attachment>(
                    It.IsAny<string>(),
                    It.IsAny<MultipartFormDataContent>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Attachment?)null); // Added ? for nullable return

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
    }
}
