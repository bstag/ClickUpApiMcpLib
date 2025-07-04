using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tags;
using ClickUp.Api.Client.Models.RequestModels.Spaces; // For ModifyTagRequest
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class TagFluentValidationTests
{
    // --- TagFluentModifyRequest CreateAsync Tests ---

    [Fact]
    public async Task Create_Validate_MissingSpaceId_ThrowsException() // Changed to async Task
    {
        var tagsServiceMock = new Mock<ITagsService>();
        // Access private ValidateForCreate via public CreateAsync call for testing validation logic
        var request = new TagFluentModifyRequest(string.Empty, tagsServiceMock.Object).WithName("Test Tag"); // string.Empty for ID
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync()); // await
        Assert.Contains("SpaceId is required for creating a tag.", ex.ValidationErrors);
    }

    [Fact]
    public async Task Create_Validate_MissingName_ThrowsException() // Changed to async Task
    {
        var tagsServiceMock = new Mock<ITagsService>();
        var request = new TagFluentModifyRequest("space123", tagsServiceMock.Object);
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync()); // await
        Assert.Contains("Tag name is required for creating a tag.", ex.ValidationErrors);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_DoesNotThrowAndCallsService()
    {
        var tagsServiceMock = new Mock<ITagsService>();
        tagsServiceMock
            .Setup(s => s.CreateSpaceTagAsync(It.IsAny<string>(), It.IsAny<ModifyTagRequest>(), It.IsAny<System.Threading.CancellationToken>()))
            .Returns(Task.CompletedTask);

        var request = new TagFluentModifyRequest("space123", tagsServiceMock.Object).WithName("Test Tag");

        await request.CreateAsync(); // Should not throw due to validation

        tagsServiceMock.Verify(s => s.CreateSpaceTagAsync("space123", It.Is<ModifyTagRequest>(r => r.Name == "Test Tag"), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    // --- TagFluentModifyRequest EditAsync Tests ---

    [Fact]
    public async Task Edit_Validate_MissingSpaceId_ThrowsException() // Changed to async Task
    {
        var tagsServiceMock = new Mock<ITagsService>();
        var request = new TagFluentModifyRequest(string.Empty, tagsServiceMock.Object, "OriginalTag").WithName("New Tag"); // string.Empty for ID
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.EditAsync()); // await
        Assert.Contains("SpaceId is required for editing a tag.", ex.ValidationErrors);
    }

    [Fact]
    public async Task Edit_Validate_MissingOriginalTagName_ThrowsException() // Changed to async Task
    {
        var tagsServiceMock = new Mock<ITagsService>();
        var request = new TagFluentModifyRequest("space123", tagsServiceMock.Object, string.Empty).WithName("New Tag"); // string.Empty for ID
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.EditAsync()); // await
        Assert.Contains("Original tag name must be provided for editing.", ex.ValidationErrors);
    }

    [Fact]
    public async Task Edit_Validate_MissingNewName_ThrowsException() // Changed to async Task
    {
        var tagsServiceMock = new Mock<ITagsService>();
        var request = new TagFluentModifyRequest("space123", tagsServiceMock.Object, "OriginalTag");
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.EditAsync()); // await
        Assert.Contains("New tag name is required for editing.", ex.ValidationErrors);
    }

    [Fact]
    public async Task EditAsync_ValidRequest_DoesNotThrowAndCallsService()
    {
        var tagsServiceMock = new Mock<ITagsService>();
        tagsServiceMock
            .Setup(s => s.EditSpaceTagAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ModifyTagRequest>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new Tag("NewTag", "fg", "bg", new ClickUp.Api.Client.Models.Entities.Users.User(1, "test", "test@example.com", null,null,null))); // Corrected namespace and params

        var request = new TagFluentModifyRequest("space123", tagsServiceMock.Object, "OldTag").WithName("NewTag");

        await request.EditAsync(); // Should not throw due to validation

        tagsServiceMock.Verify(s => s.EditSpaceTagAsync("space123", "OldTag", It.Is<ModifyTagRequest>(r => r.Name == "NewTag"), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }
}
