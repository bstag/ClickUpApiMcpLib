using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using System.Threading.Tasks;
using System.Threading;
using System.IO; // For MemoryStream
using System.Net.Http; // For MultipartFormDataContent
using System;
using System.Linq;

namespace ClickUp.Api.Client.Tests.Services
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

        private Attachment CreateSampleAttachment(string id, string title)
        {
            var attachment = (Attachment)Activator.CreateInstance(typeof(Attachment), nonPublic: true)!;
            var props = typeof(Attachment).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(attachment, id);
            props.FirstOrDefault(p => p.Name == "Title")?.SetValue(attachment, title);
            // Set other properties if needed for assertions
            return attachment;
        }

        [Fact]
        public async Task CreateTaskAttachmentAsync_ValidFile_ReturnsAttachment()
        {
            // Arrange
            var taskId = "task-id-for-attachment";
            var fileName = "test.txt";
            var fileContent = "Hello World";
            await using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

            var expectedAttachment = CreateSampleAttachment("attachment-id-1", fileName);

            _mockApiConnection.Setup(c => c.PostMultipartAsync<Attachment>(
                It.Is<string>(s => s.StartsWith($"task/{taskId}/attachment")),
                It.IsAny<MultipartFormDataContent>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAttachment)
                .Callback<string, MultipartFormDataContent, CancellationToken>(async (endpoint, content, token) =>
                {
                    // Optionally, inspect the MultipartFormDataContent here
                    // For example, verify it contains a part with name "attachment" and correct filename
                    var filePart = content.FirstOrDefault(p => p.Headers.ContentDisposition?.Name?.Contains("attachment") == true) as StreamContent;
                    filePart.Should().NotBeNull();
                    filePart?.Headers.ContentDisposition?.FileName.Should().Be($"\"{fileName}\""); // Or just fileName if not quoted by default
                    var readContent = await filePart!.ReadAsStringAsync(token);
                    readContent.Should().Be(fileContent);
                });

            // Act
            var result = await _attachmentsService.CreateTaskAttachmentAsync(taskId, memoryStream, fileName, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedAttachment);
            _mockApiConnection.Verify(c => c.PostMultipartAsync<Attachment>(
                It.Is<string>(s => s.StartsWith($"task/{taskId}/attachment")),
                It.IsAny<MultipartFormDataContent>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
