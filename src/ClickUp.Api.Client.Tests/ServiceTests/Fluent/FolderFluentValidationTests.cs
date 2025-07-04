using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FolderFluentValidationTests
{
    // --- FolderFluentCreateRequest Tests ---

    [Fact]
    public void Create_Validate_MissingSpaceId_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var foldersServiceMock = new Mock<IFoldersService>();
        var request = new FolderFluentCreateRequest(string.Empty, foldersServiceMock.Object).WithName("Test Folder"); // Changed null to string.Empty

        // Act & Assert
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("SpaceId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_MissingName_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var foldersServiceMock = new Mock<IFoldersService>();
        var request = new FolderFluentCreateRequest("space123", foldersServiceMock.Object);

        // Act & Assert
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Folder name is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_ValidRequest_DoesNotThrow()
    {
        // Arrange
        var foldersServiceMock = new Mock<IFoldersService>();
        var request = new FolderFluentCreateRequest("space123", foldersServiceMock.Object).WithName("Test Folder");

        // Act
        request.Validate();

        // Assert (no exception thrown)
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var foldersServiceMock = new Mock<IFoldersService>();
        var request = new FolderFluentCreateRequest(string.Empty, foldersServiceMock.Object); // Changed null to string.Empty

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync());
        Assert.Contains("SpaceId is required.", ex.ValidationErrors);
        Assert.Contains("Folder name is required.", ex.ValidationErrors);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CallsService()
    {
        // Arrange
        var foldersServiceMock = new Mock<IFoldersService>();
        foldersServiceMock
            .Setup(s => s.CreateFolderAsync(It.IsAny<string>(), It.IsAny<CreateFolderRequest>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new Folder(Id: "folder123", Name: "Test Folder", Archived: false, Statuses: null)); // Added Statuses

        var request = new FolderFluentCreateRequest("space123", foldersServiceMock.Object).WithName("Test Folder");

        // Act
        await request.CreateAsync();

        // Assert
        foldersServiceMock.Verify(s => s.CreateFolderAsync(
            "space123",
            It.Is<CreateFolderRequest>(r => r.Name == "Test Folder"),
            It.IsAny<System.Threading.CancellationToken>()),
            Times.Once);
    }

    // --- FolderFluentUpdateRequest Tests ---

    [Fact]
    public void Update_Validate_MissingFolderId_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var foldersServiceMock = new Mock<IFoldersService>();
        var request = new FolderFluentUpdateRequest(string.Empty, foldersServiceMock.Object).WithName("Updated Folder"); // Changed null to string.Empty

        // Act & Assert
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("FolderId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_MissingNameAndArchived_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var foldersServiceMock = new Mock<IFoldersService>();
        var request = new FolderFluentUpdateRequest("folder123", foldersServiceMock.Object);

        // Act & Assert
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Either Name or Archived status must be provided for an update.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_ValidRequest_WithName_DoesNotThrow()
    {
        // Arrange
        var foldersServiceMock = new Mock<IFoldersService>();
        var request = new FolderFluentUpdateRequest("folder123", foldersServiceMock.Object).WithName("Updated Folder");

        // Act
        request.Validate();

        // Assert (no exception thrown)
    }

    [Fact]
    public void Update_Validate_ValidRequest_WithArchived_DoesNotThrow()
    {
        // Arrange
        var foldersServiceMock = new Mock<IFoldersService>();
        var request = new FolderFluentUpdateRequest("folder123", foldersServiceMock.Object).WithArchived(true);

        // Act
        request.Validate();

        // Assert (no exception thrown)
    }

    [Fact]
    public async Task UpdateAsync_InvalidRequest_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var foldersServiceMock = new Mock<IFoldersService>();
        var request = new FolderFluentUpdateRequest(string.Empty, foldersServiceMock.Object); // Changed null to string.Empty

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.UpdateAsync());
        Assert.Contains("FolderId is required.", ex.ValidationErrors);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_CallsService()
    {
        // Arrange
        var foldersServiceMock = new Mock<IFoldersService>();
        foldersServiceMock
            .Setup(s => s.UpdateFolderAsync(It.IsAny<string>(), It.IsAny<UpdateFolderRequest>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new Folder(Id: "folder123", Name: "Updated Folder", Archived: false, Statuses: null)); // Added Statuses

        var request = new FolderFluentUpdateRequest("folder123", foldersServiceMock.Object).WithName("Updated Folder");

        // Act
        await request.UpdateAsync();

        // Assert
        foldersServiceMock.Verify(s => s.UpdateFolderAsync(
            "folder123",
            It.Is<UpdateFolderRequest>(r => r.Name == "Updated Folder"),
            It.IsAny<System.Threading.CancellationToken>()),
            Times.Once);
    }
}
