using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Attachments;
using ClickUp.Api.Client.Models.Entities.Users;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentCreateTaskAttachmentRequestTests
{
    private readonly Mock<IAttachmentsService> _mockAttachmentsService;

    public FluentCreateTaskAttachmentRequestTests()
    {
        _mockAttachmentsService = new Mock<IAttachmentsService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateTaskAttachmentAsyncWithCorrectParameters()
    {
        // Arrange
        var taskId = "testTaskId";
        var fileName = "testFile.txt";
        var fileStream = new MemoryStream();
        var customTaskIds = true;
        var teamId = "testTeamId";
        var expectedResponse = new CreateTaskAttachmentResponse(
            Id: "attachmentId",
            Version: "v1",
            Date: DateTimeOffset.UtcNow,
            Title: "test.txt",
            Extension: "txt",
            ThumbnailSmall: null,
            ThumbnailLarge: null,
            Url: "http://example.com/test.txt",
            UrlWQuery: "http://example.com/test.txt?query",
            UrlWHost: "http://example.com/test.txt",
            IsFolder: false,
            ParentId: "parentId",
            Size: 12345,
            TotalComments: 0,
            ResolvedComments: 0,
            User: new User(Id: 1, Username: "testuser", Email: "test@example.com", Color: "#000000", ProfilePicture: null, Initials: "TU"),
            Deleted: false,
            Orientation: null,
            Type: 0,
            Source: 0,
            EmailData: null,
            ResourceId: "resourceId"
        );

        _mockAttachmentsService.Setup(x => x.CreateTaskAttachmentAsync(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new FluentCreateTaskAttachmentRequest(taskId, fileStream, fileName, _mockAttachmentsService.Object)
            .WithCustomTaskIds(customTaskIds)
            .WithTeamId(teamId);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockAttachmentsService.Verify(x => x.CreateTaskAttachmentAsync(
            taskId,
            fileStream,
            fileName,
            customTaskIds,
            teamId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateTaskAttachmentAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var taskId = "testTaskId";
        var fileName = "testFile.txt";
        var fileStream = new MemoryStream();
        var expectedResponse = new CreateTaskAttachmentResponse(
            Id: "attachmentId",
            Version: "v1",
            Date: DateTimeOffset.UtcNow,
            Title: "test.txt",
            Extension: "txt",
            ThumbnailSmall: null,
            ThumbnailLarge: null,
            Url: "http://example.com/test.txt",
            UrlWQuery: "http://example.com/test.txt?query",
            UrlWHost: "http://example.com/test.txt",
            IsFolder: false,
            ParentId: "parentId",
            Size: 12345,
            TotalComments: 0,
            ResolvedComments: 0,
            User: new User(Id: 1, Username: "testuser", Email: "test@example.com", Color: "#000000", ProfilePicture: null, Initials: "TU"),
            Deleted: false,
            Orientation: null,
            Type: 0,
            Source: 0,
            EmailData: null,
            ResourceId: "resourceId"
        );

        _mockAttachmentsService.Setup(x => x.CreateTaskAttachmentAsync(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new FluentCreateTaskAttachmentRequest(taskId, fileStream, fileName, _mockAttachmentsService.Object);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockAttachmentsService.Verify(x => x.CreateTaskAttachmentAsync(
            taskId,
            fileStream,
            fileName,
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
