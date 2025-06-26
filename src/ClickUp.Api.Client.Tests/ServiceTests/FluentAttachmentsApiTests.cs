using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Attachments;
using ClickUp.Api.Client.Models.Entities.Users;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentAttachmentsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentAttachmentsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentAttachmentsApi_Create_ShouldBuildCorrectRequestAndCallService()
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
            User: new User(1, "testuser", "test@example.com", "#000000", null, "TU", null),
            Deleted: false,
            Orientation: null,
            Type: 0,
            Source: 0,
            EmailData: null,
            ResourceId: "resourceId"
        );

        var mockAttachmentsService = new Mock<IAttachmentsService>();
        mockAttachmentsService.Setup(x => x.CreateTaskAttachmentAsync(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentAttachmentsApi = new FluentAttachmentsApi(mockAttachmentsService.Object);

        // Act
        var result = await fluentAttachmentsApi.Create(taskId, fileStream, fileName)
            .WithCustomTaskIds(true)
            .WithTeamId("testTeamId")
            .CreateAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockAttachmentsService.Verify(x => x.CreateTaskAttachmentAsync(
            taskId,
            fileStream,
            fileName,
            true,
            "testTeamId",
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
