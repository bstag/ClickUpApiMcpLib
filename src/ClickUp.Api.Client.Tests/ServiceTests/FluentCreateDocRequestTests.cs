using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentCreateDocRequestTests
{
    private readonly Mock<IDocsService> _mockDocsService;

    public FluentCreateDocRequestTests()
    {
        _mockDocsService = new Mock<IDocsService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateDocAsyncWithCorrectParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var name = "testDocName";
        var parent = new ParentDocIdentifier("parentId", 0);
        var visibility = "private";
        var createPage = true;
        var templateId = "templateId";
        var workspaceIdForDoc = 123L;
        var expectedDoc = new Doc("docId", "docName", "workspaceId", null); // Mock a Doc object

        _mockDocsService.Setup(x => x.CreateDocAsync(
            It.IsAny<string>(),
            It.IsAny<CreateDocRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDoc);

        var fluentRequest = new FluentCreateDocRequest(workspaceId, _mockDocsService.Object, name)
            .WithParent(parent)
            .WithVisibility(visibility)
            .WithCreatePage(createPage)
            .WithTemplateId(templateId)
            .WithWorkspaceIdForDoc(workspaceIdForDoc);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedDoc, result);
        _mockDocsService.Verify(x => x.CreateDocAsync(
            workspaceId,
            It.Is<CreateDocRequest>(req =>
                req.Name == name &&
                req.Parent == parent &&
                req.Visibility == visibility &&
                req.CreatePage == createPage &&
                req.TemplateId == templateId &&
                req.WorkspaceId == workspaceIdForDoc),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateDocAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var name = "testDocName";
        var expectedDoc = new Doc("docId", "docName", "workspaceId", null); // Mock a Doc object

        _mockDocsService.Setup(x => x.CreateDocAsync(
            It.IsAny<string>(),
            It.IsAny<CreateDocRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDoc);

        var fluentRequest = new FluentCreateDocRequest(workspaceId, _mockDocsService.Object, name);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedDoc, result);
        _mockDocsService.Verify(x => x.CreateDocAsync(
            workspaceId,
            It.Is<CreateDocRequest>(req =>
                req.Name == name &&
                req.Parent == null &&
                req.Visibility == null &&
                req.CreatePage == null &&
                req.TemplateId == null &&
                req.WorkspaceId == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
