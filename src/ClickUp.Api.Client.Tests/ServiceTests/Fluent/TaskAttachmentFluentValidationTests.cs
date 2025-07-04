using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using System.IO;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Attachments;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class TaskAttachmentFluentValidationTests
{
    [Fact]
    public void Validate_MissingTaskId_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var attachmentsServiceMock = new Mock<IAttachmentsService>();
        var request = new TaskAttachmentFluentCreateRequest(string.Empty, new MemoryStream(), "test.txt", attachmentsServiceMock.Object); // Changed null to string.Empty

        // Act & Assert
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("TaskId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Validate_MissingFileStream_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var attachmentsServiceMock = new Mock<IAttachmentsService>();
        var request = new TaskAttachmentFluentCreateRequest("task123", Stream.Null, "test.txt", attachmentsServiceMock.Object); // Changed null to Stream.Null

        // Act & Assert
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("FileStream is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Validate_MissingFileName_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var attachmentsServiceMock = new Mock<IAttachmentsService>();
        var request = new TaskAttachmentFluentCreateRequest("task123", new MemoryStream(), string.Empty, attachmentsServiceMock.Object); // Changed null to string.Empty

        // Act & Assert
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("FileName is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Validate_ValidRequest_DoesNotThrow()
    {
        // Arrange
        var attachmentsServiceMock = new Mock<IAttachmentsService>();
        var request = new TaskAttachmentFluentCreateRequest("task123", new MemoryStream(), "test.txt", attachmentsServiceMock.Object);

        // Act
        request.Validate();

        // Assert (no exception thrown)
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var attachmentsServiceMock = new Mock<IAttachmentsService>();
        var request = new TaskAttachmentFluentCreateRequest(string.Empty, Stream.Null, string.Empty, attachmentsServiceMock.Object); // Changed nulls

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync());
        Assert.Contains("TaskId is required.", ex.ValidationErrors);
        Assert.Contains("FileStream is required.", ex.ValidationErrors);
        Assert.Contains("FileName is required.", ex.ValidationErrors);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CallsService()
    {
        // Arrange
        var attachmentsServiceMock = new Mock<IAttachmentsService>();
        attachmentsServiceMock
            .Setup(s => s.CreateTaskAttachmentAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new CreateTaskAttachmentResponse( // Added required parameters
                Id: "att123",
                Version: "1",
                Date: System.DateTimeOffset.UtcNow,
                Title: "test.txt",
                Extension: "txt",
                ThumbnailSmall: null,
                // ThumbnailMedium is not a parameter in the CreateTaskAttachmentResponse constructor
                ThumbnailLarge: null,
                Url: "http://example.com/test.txt",
                UrlWQuery: "http://example.com/test.txt?query", // Added missing required param
                UrlWHost: "http://example.com/test.txt", // Added missing required param
                ParentId: "parent123",
                Orientation: null,
                Source: 0,
                IsFolder: false,
                // Mimetype is not a constructor parameter
                Size: 0,
                TotalComments: 0,
                ResolvedComments: 0,
                User: new ClickUp.Api.Client.Models.Entities.Users.User(1, "test", "test@example.com", null,null,null),
                Deleted: false,
                Type: 0,
                ResourceId: "resource123",
                EmailData: null
                // OcrStatus, OcrErrorcode, OcrData, OcrFailedAttempts are not constructor parameters
            ));

        var request = new TaskAttachmentFluentCreateRequest("task123", new MemoryStream(), "test.txt", attachmentsServiceMock.Object);

        // Act
        await request.CreateAsync();

        // Assert
        attachmentsServiceMock.Verify(s => s.CreateTaskAttachmentAsync("task123", It.IsAny<Stream>(), "test.txt", null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }
}
