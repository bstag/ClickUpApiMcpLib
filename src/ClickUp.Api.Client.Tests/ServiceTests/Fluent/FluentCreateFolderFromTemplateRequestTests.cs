using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;

using Moq;

using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentCreateFolderFromTemplateRequestTests
{
    private readonly Mock<IFoldersService> _mockFoldersService;

    public FluentCreateFolderFromTemplateRequestTests()
    {
        _mockFoldersService = new Mock<IFoldersService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateFolderFromTemplateAsyncWithCorrectParameters()
    {
        // Arrange
        var spaceId = "testSpaceId";
        var templateId = "testTemplateId";
        var name = "testFolderName";
        var expectedFolder = new Folder("folderId", "folderName", false, null); // Mock a Folder object

        _mockFoldersService.Setup(x => x.CreateFolderFromTemplateAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreateFolderFromTemplateRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFolder);

        var fluentRequest = new TemplateFluentCreateFolderRequest(spaceId, templateId, _mockFoldersService.Object)
            .WithName(name);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedFolder, result);
        _mockFoldersService.Verify(x => x.CreateFolderFromTemplateAsync(
            spaceId,
            templateId,
            It.Is<CreateFolderFromTemplateRequest>(req => req.Name == name),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateFolderFromTemplateAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var spaceId = "testSpaceId";
        var templateId = "testTemplateId";
        var expectedFolder = new Folder("folderId", "folderName", false, null); // Mock a Folder object

        _mockFoldersService.Setup(x => x.CreateFolderFromTemplateAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreateFolderFromTemplateRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFolder);

        var fluentRequest = new TemplateFluentCreateFolderRequest(spaceId, templateId, _mockFoldersService.Object);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedFolder, result);
        _mockFoldersService.Verify(x => x.CreateFolderFromTemplateAsync(
            spaceId,
            templateId,
            It.Is<CreateFolderFromTemplateRequest>(req => req.Name == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
