using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentCreatePageRequestTests
{
    private readonly Mock<IDocsService> _mockDocsService;

    public FluentCreatePageRequestTests()
    {
        _mockDocsService = new Mock<IDocsService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreatePageAsyncWithCorrectParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var docId = "testDocId";
        var name = "testPageName";
        var content = "testContent";
        var contentFormat = "markdown";
        var parentPageId = "testParentPageId";
        var subTitle = "testSubTitle";
        var orderIndex = 1;
        var hidden = true;
        var templateId = "testTemplateId";
        var expectedPage = new Page("pageId", "docId", "pageName", "pageContent"); // Mock a Page object

        _mockDocsService.Setup(x => x.CreatePageAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreatePageRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPage);

        var fluentRequest = new FluentCreatePageRequest(workspaceId, docId, _mockDocsService.Object, name, content, contentFormat)
            .WithParentPageId(parentPageId)
            .WithSubTitle(subTitle)
            .WithOrderIndex(orderIndex)
            .WithHidden(hidden)
            .WithTemplateId(templateId);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedPage, result);
        _mockDocsService.Verify(x => x.CreatePageAsync(
            workspaceId,
            docId,
            It.Is<CreatePageRequest>(req =>
                req.Name == name &&
                req.Content == content &&
                req.ContentFormat == contentFormat &&
                req.ParentPageId == parentPageId &&
                req.SubTitle == subTitle &&
                req.OrderIndex == orderIndex &&
                req.Hidden == hidden &&
                req.TemplateId == templateId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreatePageAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var docId = "testDocId";
        var name = "testPageName";
        var content = "testContent";
        var contentFormat = "markdown";
        var expectedPage = new Page("pageId", "docId", "pageName", "pageContent"); // Mock a Page object

        _mockDocsService.Setup(x => x.CreatePageAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreatePageRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPage);

        var fluentRequest = new FluentCreatePageRequest(workspaceId, docId, _mockDocsService.Object, name, content, contentFormat);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedPage, result);
        _mockDocsService.Verify(x => x.CreatePageAsync(
            workspaceId,
            docId,
            It.Is<CreatePageRequest>(req =>
                req.Name == name &&
                req.Content == content &&
                req.ContentFormat == contentFormat &&
                req.ParentPageId == null &&
                req.SubTitle == null &&
                req.OrderIndex == null &&
                req.Hidden == null &&
                req.TemplateId == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
