using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs; // Required for CreateDocRequest

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class DocFluentValidationTests
{
    [Fact]
    public void Validate_MissingWorkspaceId_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var docsServiceMock = new Mock<IDocsService>();
        var request = new DocFluentCreateRequest(string.Empty, docsServiceMock.Object, "Test Doc"); // Changed null to string.Empty

        // Act & Assert
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("WorkspaceId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Validate_MissingName_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var docsServiceMock = new Mock<IDocsService>();
        var request = new DocFluentCreateRequest("ws123", docsServiceMock.Object, string.Empty); // Changed null to string.Empty

        // Act & Assert
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Doc name is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Validate_ValidRequest_DoesNotThrow()
    {
        // Arrange
        var docsServiceMock = new Mock<IDocsService>();
        var request = new DocFluentCreateRequest("ws123", docsServiceMock.Object, "Test Doc");

        // Act
        request.Validate();

        // Assert (no exception thrown)
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsClickUpRequestValidationException()
    {
        // Arrange
        var docsServiceMock = new Mock<IDocsService>();
        var request = new DocFluentCreateRequest(string.Empty, docsServiceMock.Object, string.Empty); // Changed nulls to string.Empty

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync());
        Assert.Contains("WorkspaceId is required.", ex.ValidationErrors);
        Assert.Contains("Doc name is required.", ex.ValidationErrors);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CallsService()
    {
        // Arrange
        var docsServiceMock = new Mock<IDocsService>();
        docsServiceMock
            .Setup(s => s.CreateDocAsync(It.IsAny<string>(), It.IsAny<CreateDocRequest>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new Doc(Id: "doc123", Name: "Test Doc", WorkspaceId: "ws123", Creator: null)); // Corrected param names and minimal set

        var request = new DocFluentCreateRequest("ws123", docsServiceMock.Object, "Test Doc");

        // Act
        await request.CreateAsync();

        // Assert
        docsServiceMock.Verify(s => s.CreateDocAsync(
            "ws123",
            It.Is<CreateDocRequest>(r => r.Name == "Test Doc"),
            It.IsAny<System.Threading.CancellationToken>()),
            Times.Once);
    }
}
