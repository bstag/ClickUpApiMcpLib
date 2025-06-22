using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using ClickUp.Api.Client.Models.ResponseModels.Docs;
using ClickUp.Api.Client.Services; // Required for ClickUpV3DataResponse
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class DocsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly DocsService _docsService;
        private readonly Mock<ILogger<DocsService>> _mockLogger;

        public DocsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<DocsService>>();
            _docsService = new DocsService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private ClickUp.Api.Client.Models.Entities.Users.User CreateSampleUser(int id = 1) =>
            new ClickUp.Api.Client.Models.Entities.Users.User(id, $"user{id}", $"user{id}@example.com", "#FFFFFF", "http://example.com/pic.jpg", $"U{id}");

        private Doc CreateSampleDoc(string id = "doc_1", string name = "Sample Doc", string workspaceId = "ws_1") => new Doc(
            Id: id,
            Name: name,
            WorkspaceId: workspaceId,
            Creator: CreateSampleUser()
        );

        private Page CreateSamplePage(string id = "page_1", string docId = "doc_1", string name = "Sample Page") => new Page(
            Id: id,
            DocId: docId,
            Name: name,
            Content: "Sample page content"
        );


        [Fact]
        public async Task SearchDocsAsync_ValidRequest_BuildsCorrectUrlAndReturnsResponse()
        {
            // Arrange
            var workspaceId = "ws_test";
            var request = new SearchDocsRequest { Query = "test query", Limit = 10, Cursor = "cursor123" }; // Use object initializer
            var expectedDocs = new List<Doc> { CreateSampleDoc() };
            var expectedResponse = new SearchDocsResponse(expectedDocs, "next_cursor_id", expectedDocs.Count, true); // Use constructor
            _mockApiConnection
                .Setup(c => c.GetAsync<SearchDocsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _docsService.SearchDocsAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.NextPageId, result.NextPageId); // Property is NextPageId
            _mockApiConnection.Verify(c => c.GetAsync<SearchDocsResponse>(
                $"/v3/workspaces/{workspaceId}/docs?q=test%20query&limit=10&cursor=cursor123",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateDocAsync_ValidRequest_CallsPostAndReturnsDoc()
        {
            // Arrange
            var workspaceId = "ws_test";
            var request = new CreateDocRequest(Name: "New Doc", Parent: null, Visibility: "private", CreatePage: true, TemplateId: null, WorkspaceId: null); // Use constructor
            var expectedDoc = CreateSampleDoc("new_doc_1", workspaceId: workspaceId);
            var apiResponse = new ClickUpV3DataResponse<Doc> { Data = expectedDoc };
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateDocRequest, ClickUpV3DataResponse<Doc>>(
                    It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _docsService.CreateDocAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDoc.Id, result.Id);
            _mockApiConnection.Verify(c => c.PostAsync<CreateDocRequest, ClickUpV3DataResponse<Doc>>(
                $"/v3/workspaces/{workspaceId}/docs",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDocAsync_ApiError_ThrowsHttpRequestException()
        {
            // Arrange
            var workspaceId = "ws_test";
            var docId = "doc_error";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Doc>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _docsService.GetDocAsync(workspaceId, docId));
        }

        [Fact]
        public async Task CreatePageAsync_NullResponseData_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_test";
            var docId = "doc_1";
            var request = new CreatePageRequest(ParentPageId: null, Name: "New Page", SubTitle: null, Content: "<p>Hello</p>", ContentFormat: "text/html", OrderIndex: null, Hidden: null, TemplateId: null); // Use constructor
             _mockApiConnection
                .Setup(c => c.PostAsync<CreatePageRequest, ClickUpV3DataResponse<Page>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<Page> { Data = null });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _docsService.CreatePageAsync(workspaceId, docId, request));
        }
    }
}
