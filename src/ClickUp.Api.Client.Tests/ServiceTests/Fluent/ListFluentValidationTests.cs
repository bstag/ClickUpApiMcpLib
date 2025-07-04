using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.RequestModels.Lists;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class ListFluentValidationTests
{
    // --- ListFluentCreateRequest Tests ---

    [Fact]
    public void Create_Validate_MissingContainerId_ThrowsException()
    {
        var listsServiceMock = new Mock<IListsService>();
        var request = new ListFluentCreateRequest(string.Empty, listsServiceMock.Object, false).WithName("Test List"); // Changed null to string.Empty
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("ContainerId (FolderId or SpaceId) is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_MissingName_ThrowsException()
    {
        var listsServiceMock = new Mock<IListsService>();
        var request = new ListFluentCreateRequest("container123", listsServiceMock.Object, false);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("List name is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_ValidRequest_DoesNotThrow()
    {
        var listsServiceMock = new Mock<IListsService>();
        var request = new ListFluentCreateRequest("container123", listsServiceMock.Object, false).WithName("Test List");
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsException()
    {
        var listsServiceMock = new Mock<IListsService>();
        var request = new ListFluentCreateRequest(string.Empty, listsServiceMock.Object, false); // Invalid, Changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync());
        Assert.Contains("ContainerId (FolderId or SpaceId) is required.", ex.ValidationErrors);
        Assert.Contains("List name is required.", ex.ValidationErrors);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_FolderList_CallsCorrectServiceMethod()
    {
        var listsServiceMock = new Mock<IListsService>();
        listsServiceMock
            .Setup(s => s.CreateListInFolderAsync(It.IsAny<string>(), It.IsAny<CreateListRequest>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new ClickUpList());

        var request = new ListFluentCreateRequest("folder123", listsServiceMock.Object, false).WithName("Test List");
        await request.CreateAsync();
        listsServiceMock.Verify(s => s.CreateListInFolderAsync("folder123", It.Is<CreateListRequest>(r => r.Name == "Test List"), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_FolderlessList_CallsCorrectServiceMethod()
    {
        var listsServiceMock = new Mock<IListsService>();
        listsServiceMock
            .Setup(s => s.CreateFolderlessListAsync(It.IsAny<string>(), It.IsAny<CreateListRequest>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new ClickUpList());

        var request = new ListFluentCreateRequest("space123", listsServiceMock.Object, true).WithName("Test Folderless List");
        await request.CreateAsync();
        listsServiceMock.Verify(s => s.CreateFolderlessListAsync("space123", It.Is<CreateListRequest>(r => r.Name == "Test Folderless List"), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    // --- ListFluentUpdateRequest Tests ---

    [Fact]
    public void Update_Validate_MissingListId_ThrowsException()
    {
        var listsServiceMock = new Mock<IListsService>();
        var request = new ListFluentUpdateRequest(string.Empty, listsServiceMock.Object).WithName("New Name"); // Changed null to string.Empty
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("ListId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_NoFieldsSet_ThrowsException()
    {
        var listsServiceMock = new Mock<IListsService>();
        var request = new ListFluentUpdateRequest("list123", listsServiceMock.Object); // No fields set
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("At least one property must be set for updating a List.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_ValidRequest_DoesNotThrow()
    {
        var listsServiceMock = new Mock<IListsService>();
        var request = new ListFluentUpdateRequest("list123", listsServiceMock.Object).WithName("New Name");
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task UpdateAsync_InvalidRequest_ThrowsException()
    {
        var listsServiceMock = new Mock<IListsService>();
        var request = new ListFluentUpdateRequest(string.Empty, listsServiceMock.Object); // Invalid, Changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.UpdateAsync());
        Assert.Contains("ListId is required.", ex.ValidationErrors);
    }
}
