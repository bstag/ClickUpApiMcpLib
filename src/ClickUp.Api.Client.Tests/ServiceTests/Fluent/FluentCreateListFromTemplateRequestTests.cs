using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.RequestModels.Lists;

using Moq;

using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Lists;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentCreateListFromTemplateRequestTests
{
    private readonly Mock<IListsService> _mockListsService;

    public FluentCreateListFromTemplateRequestTests()
    {
        _mockListsService = new Mock<IListsService>();
    }

    [Fact]
    public async Task CreateAsync_InFolderTrue_ShouldCallCreateListFromTemplateInFolderAsync()
    {
        // Arrange
        var containerId = "testFolderId";
        var templateId = "testTemplateId";
        var name = "testListName";
        var expectedList = new ClickUpList(); // Mock a ClickUpList object

        _mockListsService.Setup(x => x.CreateListFromTemplateInFolderAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreateListFromTemplateRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedList);

        var fluentRequest = new TemplateFluentCreateListRequest(containerId, templateId, _mockListsService.Object, true)
            .WithName(name);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedList, result);
        _mockListsService.Verify(x => x.CreateListFromTemplateInFolderAsync(
            containerId,
            templateId,
            It.Is<CreateListFromTemplateRequest>(req => req.Name == name),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockListsService.Verify(x => x.CreateListFromTemplateInSpaceAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreateListFromTemplateRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_InFolderFalse_ShouldCallCreateListFromTemplateInSpaceAsync()
    {
        // Arrange
        var containerId = "testSpaceId";
        var templateId = "testTemplateId";
        var name = "testListName";
        var expectedList = new ClickUpList(); // Mock a ClickUpList object

        _mockListsService.Setup(x => x.CreateListFromTemplateInSpaceAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreateListFromTemplateRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedList);

        var fluentRequest = new TemplateFluentCreateListRequest(containerId, templateId, _mockListsService.Object, false)
            .WithName(name);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedList, result);
        _mockListsService.Verify(x => x.CreateListFromTemplateInSpaceAsync(
            containerId,
            templateId,
            It.Is<CreateListFromTemplateRequest>(req => req.Name == name),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockListsService.Verify(x => x.CreateListFromTemplateInFolderAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreateListFromTemplateRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_InFolderTrue_ShouldCallCreateListFromTemplateInFolderAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var containerId = "testFolderId";
        var templateId = "testTemplateId";
        var expectedList = new ClickUpList(); // Mock a ClickUpList object

        _mockListsService.Setup(x => x.CreateListFromTemplateInFolderAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreateListFromTemplateRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedList);

        var fluentRequest = new TemplateFluentCreateListRequest(containerId, templateId, _mockListsService.Object, true);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedList, result);
        _mockListsService.Verify(x => x.CreateListFromTemplateInFolderAsync(
            containerId,
            templateId,
            It.Is<CreateListFromTemplateRequest>(req => req.Name == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_InFolderFalse_ShouldCallCreateListFromTemplateInSpaceAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var containerId = "testSpaceId";
        var templateId = "testTemplateId";
        var expectedList = new ClickUpList(); // Mock a ClickUpList object

        _mockListsService.Setup(x => x.CreateListFromTemplateInSpaceAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreateListFromTemplateRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedList);

        var fluentRequest = new TemplateFluentCreateListRequest(containerId, templateId, _mockListsService.Object, false);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedList, result);
        _mockListsService.Verify(x => x.CreateListFromTemplateInSpaceAsync(
            containerId,
            templateId,
            It.Is<CreateListFromTemplateRequest>(req => req.Name == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
