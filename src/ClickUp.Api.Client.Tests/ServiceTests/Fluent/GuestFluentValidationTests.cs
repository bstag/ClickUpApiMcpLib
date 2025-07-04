using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Users; // Required for Guest
using ClickUp.Api.Client.Models.RequestModels.Guests; // Required for AddGuestToItemRequest

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class GuestFluentValidationTests
{
    [Fact]
    public void AddGuest_Validate_MissingItemId_ThrowsException()
    {
        var guestsServiceMock = new Mock<IGuestsService>();
        var request = new ItemFluentAddGuestRequest(string.Empty, "guest123", guestsServiceMock.Object, ItemFluentAddGuestRequest.ItemType.Task) // Changed null to string.Empty
            .WithPermissionLevel(1);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("ItemId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void AddGuest_Validate_MissingGuestId_ThrowsException()
    {
        var guestsServiceMock = new Mock<IGuestsService>();
        var request = new ItemFluentAddGuestRequest("item123", string.Empty, guestsServiceMock.Object, ItemFluentAddGuestRequest.ItemType.Task) // Changed null to string.Empty
            .WithPermissionLevel(1);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("GuestId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void AddGuest_Validate_InvalidPermissionLevel_ThrowsException()
    {
        var guestsServiceMock = new Mock<IGuestsService>();
        var request = new ItemFluentAddGuestRequest("item123", "guest123", guestsServiceMock.Object, ItemFluentAddGuestRequest.ItemType.Task)
            .WithPermissionLevel(0); // Assuming 0 or less is invalid
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("A valid PermissionLevel is required.", ex.ValidationErrors);
    }

    [Fact]
    public void AddGuest_Validate_ValidRequest_DoesNotThrow()
    {
        var guestsServiceMock = new Mock<IGuestsService>();
        var request = new ItemFluentAddGuestRequest("item123", "guest123", guestsServiceMock.Object, ItemFluentAddGuestRequest.ItemType.Task)
            .WithPermissionLevel(1);
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task AddGuestAsync_InvalidRequest_ThrowsException()
    {
        var guestsServiceMock = new Mock<IGuestsService>();
        var request = new ItemFluentAddGuestRequest(string.Empty, string.Empty, guestsServiceMock.Object, ItemFluentAddGuestRequest.ItemType.Task); // Invalid, Changed nulls to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.AddAsync());
        Assert.Contains("ItemId is required.", ex.ValidationErrors);
        Assert.Contains("GuestId is required.", ex.ValidationErrors);
        // PermissionLevel will also fail as it's not set and defaults to 0
        Assert.Contains("A valid PermissionLevel is required.", ex.ValidationErrors);
    }

    [Fact]
    public async Task AddGuestAsync_ValidRequest_Task_CallsCorrectServiceMethod()
    {
        // Arrange
        var guestsServiceMock = new Mock<IGuestsService>();
        guestsServiceMock
            .Setup(s => s.AddGuestToTaskAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AddGuestToItemRequest>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new Guest());

        var request = new ItemFluentAddGuestRequest("task123", "guest123", guestsServiceMock.Object, ItemFluentAddGuestRequest.ItemType.Task)
            .WithPermissionLevel(1);

        // Act
        await request.AddAsync();

        // Assert
        guestsServiceMock.Verify(s => s.AddGuestToTaskAsync(
            "task123",
            "guest123",
            It.Is<AddGuestToItemRequest>(r => r.PermissionLevel == 1),
            null, null, null,
            It.IsAny<System.Threading.CancellationToken>()),
            Times.Once);
    }
}
