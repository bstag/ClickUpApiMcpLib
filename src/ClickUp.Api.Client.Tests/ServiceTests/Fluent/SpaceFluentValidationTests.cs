using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class SpaceFluentValidationTests
{
    // --- SpaceFluentCreateRequest Tests ---

    [Fact]
    public void Create_Validate_MissingWorkspaceId_ThrowsException()
    {
        var spacesServiceMock = new Mock<ISpacesService>();
        var request = new SpaceFluentCreateRequest(string.Empty, spacesServiceMock.Object).WithName("Test Space"); // Changed null to string.Empty
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("WorkspaceId (TeamId) is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_MissingName_ThrowsException()
    {
        var spacesServiceMock = new Mock<ISpacesService>();
        var request = new SpaceFluentCreateRequest("ws123", spacesServiceMock.Object);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Space name is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_ValidRequest_DoesNotThrow()
    {
        var spacesServiceMock = new Mock<ISpacesService>();
        var request = new SpaceFluentCreateRequest("ws123", spacesServiceMock.Object).WithName("Test Space");
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsException()
    {
        var spacesServiceMock = new Mock<ISpacesService>();
        var request = new SpaceFluentCreateRequest(string.Empty, spacesServiceMock.Object); // Invalid, Changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync());
        Assert.Contains("WorkspaceId (TeamId) is required.", ex.ValidationErrors);
        Assert.Contains("Space name is required.", ex.ValidationErrors);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CallsService()
    {
        var spacesServiceMock = new Mock<ISpacesService>();
        spacesServiceMock
            .Setup(s => s.CreateSpaceAsync(It.IsAny<string>(), It.IsAny<CreateSpaceRequest>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new Space(
                Id: "space123",
                Name: "Test Space",
                Private: false,
                Features: new ClickUp.Api.Client.Models.Entities.Spaces.Features(null,null,null,null,null,null,null,null,null,null,null,null,null,null),
                Color: null,
                Avatar: null,
                AdminCanManage: null,
                Archived: false,
                Members: null,
                MultipleAssignees: false,
                Statuses: null,
                TeamId: "team123", // Added missing TeamId
                DefaultListSettings: null
            ));

        var request = new SpaceFluentCreateRequest("ws123", spacesServiceMock.Object).WithName("Test Space");
        await request.CreateAsync();
        spacesServiceMock.Verify(s => s.CreateSpaceAsync("ws123", It.Is<CreateSpaceRequest>(r => r.Name == "Test Space"), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    // --- SpaceFluentUpdateRequest Tests ---

    [Fact]
    public void Update_Validate_MissingSpaceId_ThrowsException()
    {
        var spacesServiceMock = new Mock<ISpacesService>();
        var request = new SpaceFluentUpdateRequest(string.Empty, spacesServiceMock.Object).WithName("New Name"); // Changed null to string.Empty
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("SpaceId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_ValidRequest_DoesNotThrow()
    {
        // Note: Update validation for SpaceFluentUpdateRequest is currently lenient (allows no fields set).
        // This test confirms it doesn't throw if SpaceId is present.
        var spacesServiceMock = new Mock<ISpacesService>();
        var request = new SpaceFluentUpdateRequest("space123", spacesServiceMock.Object).WithName("New Name"); // Setting a field
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task UpdateAsync_InvalidRequest_ThrowsException()
    {
        var spacesServiceMock = new Mock<ISpacesService>();
        var request = new SpaceFluentUpdateRequest(string.Empty, spacesServiceMock.Object); // Invalid, Changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.UpdateAsync());
        Assert.Contains("SpaceId is required.", ex.ValidationErrors);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_CallsService()
    {
        var spacesServiceMock = new Mock<ISpacesService>();
        spacesServiceMock
            .Setup(s => s.UpdateSpaceAsync(It.IsAny<string>(), It.IsAny<UpdateSpaceRequest>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new Space(
                Id: "space123",
                Name: "Updated Space",
                Private: false,
                Features: new ClickUp.Api.Client.Models.Entities.Spaces.Features(null,null,null,null,null,null,null,null,null,null,null,null,null,null),
                Color: null,
                Avatar: null,
                AdminCanManage: null,
                Archived: false,
                Members: null,
                MultipleAssignees: false,
                Statuses: null,
                TeamId: "team123", // Added missing TeamId
                DefaultListSettings: null
            ));

        var request = new SpaceFluentUpdateRequest("space123", spacesServiceMock.Object).WithName("Updated Space");
        await request.UpdateAsync();
        spacesServiceMock.Verify(s => s.UpdateSpaceAsync("space123", It.Is<UpdateSpaceRequest>(r => r.Name == "Updated Space"), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }
}
