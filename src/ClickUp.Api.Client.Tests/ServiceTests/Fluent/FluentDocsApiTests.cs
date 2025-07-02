using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Docs;
using ClickUp.Api.Client.Models.Entities.Docs;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentDocsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentDocsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentDocsApi_SearchDocs_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedDocsList = new List<Doc>();
        var pagedResult = new ClickUp.Api.Client.Models.Common.Pagination.PagedResult<Doc>(
            expectedDocsList, 0, 10, false // items, page, pageSize, hasNextPage
        );

        var mockDocsService = new Mock<IDocsService>();
        mockDocsService.Setup(x => x.SearchDocsAsync(
            It.IsAny<string>(),
            It.IsAny<Client.Models.RequestModels.Docs.SearchDocsRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var fluentDocsApi = new DocsFluentApi(mockDocsService.Object);

        // Act
        var result = await fluentDocsApi.SearchDocs(workspaceId)
            .WithQuery("testQuery")
            .WithLimit(5)
            .SearchAsync();

        // Assert
        Assert.Equal(expectedDocsList, result.Items);
        Assert.Equal(pagedResult.HasNextPage, result.HasNextPage); // Example of asserting IPagedResult property
        mockDocsService.Verify(x => x.SearchDocsAsync(
            workspaceId,
            It.Is<Client.Models.RequestModels.Docs.SearchDocsRequest>(req =>
                req.Query == "testQuery" &&
                req.Limit == 5),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
