using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Guests;

using Moq;

using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentAddGuestToItemRequestTests
{
    private readonly Mock<IGuestsService> _mockGuestsService;

    public FluentAddGuestToItemRequestTests()
    {
        _mockGuestsService = new Mock<IGuestsService>();
    }

    [Fact]
    public async Task AddAsync_TaskItemType_ShouldCallAddGuestToTaskAsync()
    {
        // Arrange
        var itemId = "testTaskId";
        var guestId = "testGuestId";
        var permissionLevel = 1;
        var includeShared = true;
        var customTaskIds = true;
        var teamId = "testTeamId";
        var expectedGuest = new Guest(); // Mock a guest object

        _mockGuestsService.Setup(x => x.AddGuestToTaskAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<AddGuestToItemRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGuest);

        var fluentRequest = new ItemFluentAddGuestRequest(itemId, guestId, _mockGuestsService.Object, ItemFluentAddGuestRequest.ItemType.Task)
            .WithPermissionLevel(permissionLevel)
            .WithIncludeShared(includeShared)
            .WithCustomTaskIds(customTaskIds)
            .WithTeamId(teamId);

        // Act
        var result = await fluentRequest.AddAsync();

        // Assert
        Assert.Equal(expectedGuest, result);
        _mockGuestsService.Verify(x => x.AddGuestToTaskAsync(
            itemId,
            guestId,
            It.Is<AddGuestToItemRequest>(req => req.PermissionLevel == permissionLevel),
            includeShared,
            customTaskIds,
            teamId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ListItemType_ShouldCallAddGuestToListAsync()
    {
        // Arrange
        var itemId = "testListId";
        var guestId = "testGuestId";
        var permissionLevel = 1;
        var includeShared = true;
        var expectedGuest = new Guest(); // Mock a guest object

        _mockGuestsService.Setup(x => x.AddGuestToListAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<AddGuestToItemRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGuest);

        var fluentRequest = new ItemFluentAddGuestRequest(itemId, guestId, _mockGuestsService.Object, ItemFluentAddGuestRequest.ItemType.List)
            .WithPermissionLevel(permissionLevel)
            .WithIncludeShared(includeShared);

        // Act
        var result = await fluentRequest.AddAsync();

        // Assert
        Assert.Equal(expectedGuest, result);
        _mockGuestsService.Verify(x => x.AddGuestToListAsync(
            itemId,
            guestId,
            It.Is<AddGuestToItemRequest>(req => req.PermissionLevel == permissionLevel),
            includeShared,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_FolderItemType_ShouldCallAddGuestToFolderAsync()
    {
        // Arrange
        var itemId = "testFolderId";
        var guestId = "testGuestId";
        var permissionLevel = 1;
        var includeShared = true;
        var expectedGuest = new Guest(); // Mock a guest object

        _mockGuestsService.Setup(x => x.AddGuestToFolderAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<AddGuestToItemRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGuest);

        var fluentRequest = new ItemFluentAddGuestRequest(itemId, guestId, _mockGuestsService.Object, ItemFluentAddGuestRequest.ItemType.Folder)
            .WithPermissionLevel(permissionLevel)
            .WithIncludeShared(includeShared);

        // Act
        var result = await fluentRequest.AddAsync();

        // Assert
        Assert.Equal(expectedGuest, result);
        _mockGuestsService.Verify(x => x.AddGuestToFolderAsync(
            itemId,
            guestId,
            It.Is<AddGuestToItemRequest>(req => req.PermissionLevel == permissionLevel),
            includeShared,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_TaskItemType_ShouldCallAddGuestToTaskAsync_WithOnlyRequiredParameters()
    {
        // Arrange
        var itemId = "testTaskId";
        var guestId = "testGuestId";
        var expectedGuest = new Guest(); // Mock a guest object

        _mockGuestsService.Setup(x => x.AddGuestToTaskAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<AddGuestToItemRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGuest);

        var fluentRequest = new ItemFluentAddGuestRequest(itemId, guestId, _mockGuestsService.Object, ItemFluentAddGuestRequest.ItemType.Task)
            .WithPermissionLevel(1); // Required

        // Act
        var result = await fluentRequest.AddAsync();

        // Assert
        Assert.Equal(expectedGuest, result);
        _mockGuestsService.Verify(x => x.AddGuestToTaskAsync(
            itemId,
            guestId,
            It.Is<AddGuestToItemRequest>(req => req.PermissionLevel == 1),
            null, // includeShared
            null, // customTaskIds
            null, // teamId
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
