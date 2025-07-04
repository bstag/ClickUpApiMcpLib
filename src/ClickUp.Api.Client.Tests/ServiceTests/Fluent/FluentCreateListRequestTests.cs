using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.RequestModels.Lists;

using Moq;

using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Lists;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentCreateListRequestTests
{
    private readonly Mock<IListsService> _mockListsService;

    public FluentCreateListRequestTests()
    {
        _mockListsService = new Mock<IListsService>();
    }

    [Fact]
    public async Task CreateAsync_FolderlessTrue_ShouldCallCreateFolderlessListAsync()
    {
        // Arrange
        var containerId = "testSpaceId";
        var name = "testListName";
        var content = "testContent";
        var markdownContent = "# testMarkdownContent";
        var assignee = 123;
        var dueDate = DateTimeOffset.UtcNow;
        var dueDateTime = true;
        var priority = 1;
        var status = "open";
        var expectedList = new ClickUpList(); // Mock a ClickUpList object

        _mockListsService.Setup(x => x.CreateFolderlessListAsync(
            It.IsAny<string>(),
            It.IsAny<CreateListRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedList);

        var fluentRequest = new ListFluentCreateRequest(containerId, _mockListsService.Object, true)
            .WithName(name)
            .WithContent(content)
            .WithMarkdownContent(markdownContent)
            .WithAssignee(assignee)
            .WithDueDate(dueDate)
            .WithDueDateTime(dueDateTime)
            .WithPriority(priority)
            .WithStatus(status);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedList, result);
        _mockListsService.Verify(x => x.CreateFolderlessListAsync(
            containerId,
            It.Is<CreateListRequest>(req =>
                req.Name == name &&
                req.Content == content &&
                req.MarkdownContent == markdownContent &&
                req.Assignee == assignee &&
                req.DueDate == dueDate &&
                req.DueDateTime == dueDateTime &&
                req.Priority == priority &&
                req.Status == status),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockListsService.Verify(x => x.CreateListInFolderAsync(
            It.IsAny<string>(),
            It.IsAny<CreateListRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_FolderlessFalse_ShouldCallCreateListInFolderAsync()
    {
        // Arrange
        var containerId = "testFolderId";
        var name = "testListName";
        var expectedList = new ClickUpList(); // Mock a ClickUpList object

        _mockListsService.Setup(x => x.CreateListInFolderAsync(
            It.IsAny<string>(),
            It.IsAny<CreateListRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedList);

        var fluentRequest = new ListFluentCreateRequest(containerId, _mockListsService.Object, false)
            .WithName(name);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedList, result);
        _mockListsService.Verify(x => x.CreateListInFolderAsync(
            containerId,
            It.Is<CreateListRequest>(req => req.Name == name),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockListsService.Verify(x => x.CreateFolderlessListAsync(
            It.IsAny<string>(),
            It.IsAny<CreateListRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_FolderlessTrue_ShouldCallCreateFolderlessListAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var containerId = "testSpaceId";
        var expectedList = new ClickUpList(); // Mock a ClickUpList object

        _mockListsService.Setup(x => x.CreateFolderlessListAsync(
            It.IsAny<string>(),
            It.IsAny<CreateListRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedList);

        var fluentRequest = new ListFluentCreateRequest(containerId, _mockListsService.Object, true) // Used containerId
            .WithName("Default Folderless List Name"); // Required

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedList, result);
        _mockListsService.Verify(x => x.CreateFolderlessListAsync(
            containerId, // This is spaceId in this test context
            It.Is<CreateListRequest>(req =>
                req.Name == "Default Folderless List Name" &&
                req.Content == null &&
                req.MarkdownContent == null &&
                req.Assignee == null &&
                req.DueDate == null &&
                req.DueDateTime == null &&
                req.Priority == null &&
                req.Status == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_FolderlessFalse_ShouldCallCreateListInFolderAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var containerId = "testFolderId";
        var expectedList = new ClickUpList(); // Mock a ClickUpList object

        _mockListsService.Setup(x => x.CreateListInFolderAsync(
            It.IsAny<string>(),
            It.IsAny<CreateListRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedList);

        var fluentRequest = new ListFluentCreateRequest(containerId, _mockListsService.Object, false) // Used containerId
            .WithName("Default List Name"); // Required

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedList, result);
        _mockListsService.Verify(x => x.CreateListInFolderAsync(
            containerId, // This is folderId in this test context
            It.Is<CreateListRequest>(req =>
                req.Name == "Default List Name" &&
                req.Content == null &&
                req.MarkdownContent == null &&
                req.Assignee == null &&
                req.DueDate == null &&
                req.DueDateTime == null &&
                req.Priority == null &&
                req.Status == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
