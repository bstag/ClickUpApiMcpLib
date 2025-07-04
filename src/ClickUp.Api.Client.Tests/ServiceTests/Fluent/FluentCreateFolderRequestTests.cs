using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;

using Moq;

using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentCreateFolderRequestTests
{
    private readonly Mock<IFoldersService> _mockFoldersService;

    public FluentCreateFolderRequestTests()
    {
        _mockFoldersService = new Mock<IFoldersService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateFolderAsyncWithCorrectParameters()
    {
        // Arrange
        var spaceId = "testSpaceId";
        var name = "testFolderName";
        var expectedFolder = new Folder("folderId", "folderName", false, null); // Mock a Folder object

        _mockFoldersService.Setup(x => x.CreateFolderAsync(
            It.IsAny<string>(),
            It.IsAny<CreateFolderRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFolder);

        var fluentRequest = new FolderFluentCreateRequest(spaceId, _mockFoldersService.Object)
            .WithName(name);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedFolder, result);
        _mockFoldersService.Verify(x => x.CreateFolderAsync(
            spaceId,
            It.Is<CreateFolderRequest>(req => req.Name == name),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateFolderAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var spaceId = "testSpaceId";
        var expectedFolder = new Folder("folderId", "folderName", false, null); // Mock a Folder object

        _mockFoldersService.Setup(x => x.CreateFolderAsync(
            It.IsAny<string>(),
            It.IsAny<CreateFolderRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFolder);

        var fluentRequest = new FolderFluentCreateRequest(spaceId, _mockFoldersService.Object)
            .WithName("Default Folder Name"); // Required

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedFolder, result);
        _mockFoldersService.Verify(x => x.CreateFolderAsync(
            spaceId,
            It.Is<CreateFolderRequest>(req =>
                req.Name == "Default Folder Name"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
