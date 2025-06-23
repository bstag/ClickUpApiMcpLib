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

        // --- GetDocAsync Tests ---
        [Fact]
        public async Task GetDocAsync_ValidRequest_ReturnsDoc()
        {
            // Arrange
            var workspaceId = "ws_get_doc";
            var docId = "doc_get_valid";
            var expectedDoc = CreateSampleDoc(docId, workspaceId: workspaceId);
            var apiResponse = new ClickUpV3DataResponse<Doc> { Data = expectedDoc };
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Doc>>(
                    $"v3/workspaces/{workspaceId}/docs/{docId}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _docsService.GetDocAsync(workspaceId, docId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDoc.Id, result.Id);
            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataResponse<Doc>>(
                $"v3/workspaces/{workspaceId}/docs/{docId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDocAsync_NullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_get_doc_null_resp";
            var docId = "doc_get_null_resp";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Doc>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataResponse<Doc>)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _docsService.GetDocAsync(workspaceId, docId));
        }

        [Fact]
        public async Task GetDocAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_get_doc_null_data";
            var docId = "doc_get_null_data";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Doc>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<Doc> { Data = null });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _docsService.GetDocAsync(workspaceId, docId));
        }

        [Fact]
        public async Task GetDocAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_get_doc_cancel";
            var docId = "doc_get_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Doc>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _docsService.GetDocAsync(workspaceId, docId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetDocAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_get_doc_ct";
            var docId = "doc_get_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedDoc = CreateSampleDoc(docId, workspaceId: workspaceId);
            var apiResponse = new ClickUpV3DataResponse<Doc> { Data = expectedDoc };
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Doc>>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(apiResponse);

            await _docsService.GetDocAsync(workspaceId, docId, expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataResponse<Doc>>(
                $"v3/workspaces/{workspaceId}/docs/{docId}",
                expectedToken), Times.Once);
        }

        // --- UpdateDocAsync Tests (Placeholder - Service method not fully implemented) ---
        // Add tests when UpdateDocAsync is implemented in DocsService.cs

        // --- DeleteDocAsync Tests (Placeholder - Service method not fully implemented) ---
        // Add tests when DeleteDocAsync is implemented in DocsService.cs

        // --- GetPagesAsync Tests (Placeholder - Service method not fully implemented) ---
        // Add tests when GetPagesAsync is implemented in DocsService.cs

        // --- CreatePageAsync (Add missing tests) ---
        [Fact]
        public async Task CreatePageAsync_ValidRequest_ReturnsPage()
        {
            // Arrange
            var workspaceId = "ws_create_page";
            var docId = "doc_for_page";
            var request = new CreatePageRequest(null, "New Page Title", null, "<p>Content</p>", "text/html", null, null, null);
            var expectedPage = CreateSamplePage("new_page_1", docId, "New Page Title");
            var apiResponse = new ClickUpV3DataResponse<Page> { Data = expectedPage };

            _mockApiConnection
                .Setup(c => c.PostAsync<CreatePageRequest, ClickUpV3DataResponse<Page>>(
                    $"v3/workspaces/{workspaceId}/docs/{docId}/pages", request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _docsService.CreatePageAsync(workspaceId, docId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPage.Id, result.Id);
            _mockApiConnection.Verify();
        }

        [Fact]
        public async Task CreatePageAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_cp_err";
            var docId = "doc_cp_err";
            var request = new CreatePageRequest(null, "Error Page", null, "Content", "text/plain", null, null, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreatePageRequest, ClickUpV3DataResponse<Page>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _docsService.CreatePageAsync(workspaceId, docId, request));
        }

        [Fact]
        public async Task CreatePageAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_cp_cancel";
            var docId = "doc_cp_cancel";
            var request = new CreatePageRequest(null, "Cancel Page", null, "Content", "text/plain", null, null, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreatePageRequest, ClickUpV3DataResponse<Page>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _docsService.CreatePageAsync(workspaceId, docId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreatePageAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_cp_ct";
            var docId = "doc_cp_ct";
            var request = new CreatePageRequest(null, "CT Page", null, "Content", "text/plain", null, null, null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedPage = CreateSamplePage("page_cp_ct", docId);
            var apiResponse = new ClickUpV3DataResponse<Page> { Data = expectedPage };

            _mockApiConnection
                .Setup(c => c.PostAsync<CreatePageRequest, ClickUpV3DataResponse<Page>>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(apiResponse);

            await _docsService.CreatePageAsync(workspaceId, docId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<CreatePageRequest, ClickUpV3DataResponse<Page>>(
                $"v3/workspaces/{workspaceId}/docs/{docId}/pages",
                request,
                expectedToken), Times.Once);
        }


        // --- SearchDocsAsync (Add missing tests) ---
        [Fact]
        public async Task SearchDocsAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_search_err";
            var request = new SearchDocsRequest { Query = "error" };
            _mockApiConnection
                .Setup(c => c.GetAsync<SearchDocsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _docsService.SearchDocsAsync(workspaceId, request));
        }

        [Fact]
        public async Task SearchDocsAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_search_null";
            var request = new SearchDocsRequest { Query = "null" };
            _mockApiConnection
                .Setup(c => c.GetAsync<SearchDocsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SearchDocsResponse)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _docsService.SearchDocsAsync(workspaceId, request));
        }

        [Fact]
        public async Task SearchDocsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_search_cancel";
            var request = new SearchDocsRequest { Query = "cancel" };
            _mockApiConnection
                .Setup(c => c.GetAsync<SearchDocsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _docsService.SearchDocsAsync(workspaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task SearchDocsAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_search_ct";
            var request = new SearchDocsRequest { Query = "ct_query" };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new SearchDocsResponse(new List<Doc>(), null, 0, true);
            _mockApiConnection
                .Setup(c => c.GetAsync<SearchDocsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _docsService.SearchDocsAsync(workspaceId, request, expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<SearchDocsResponse>(
                $"/v3/workspaces/{workspaceId}/docs?q={Uri.EscapeDataString(request.Query)}", // Basic URL with query
                expectedToken), Times.Once);
        }

        // --- CreateDocAsync (Add missing tests) ---
        [Fact]
        public async Task CreateDocAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_create_doc_err";
            var request = new CreateDocRequest("Error Doc", null, "private", false, null, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateDocRequest, ClickUpV3DataResponse<Doc>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _docsService.CreateDocAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateDocAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_create_doc_null_resp";
            var request = new CreateDocRequest("Null Resp Doc", null, "private", false, null, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateDocRequest, ClickUpV3DataResponse<Doc>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataResponse<Doc>)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _docsService.CreateDocAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateDocAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_create_doc_null_data";
            var request = new CreateDocRequest("Null Data Doc", null, "private", false, null, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateDocRequest, ClickUpV3DataResponse<Doc>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<Doc> { Data = null });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _docsService.CreateDocAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateDocAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_create_doc_cancel";
            var request = new CreateDocRequest("Cancel Doc", null, "private", false, null, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateDocRequest, ClickUpV3DataResponse<Doc>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _docsService.CreateDocAsync(workspaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreateDocAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_create_doc_ct";
            var request = new CreateDocRequest("CT Doc", null, "private", false, null, null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedDoc = CreateSampleDoc("doc_create_ct", workspaceId: workspaceId);
            var apiResponse = new ClickUpV3DataResponse<Doc> { Data = expectedDoc };
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateDocRequest, ClickUpV3DataResponse<Doc>>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(apiResponse);

            await _docsService.CreateDocAsync(workspaceId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<CreateDocRequest, ClickUpV3DataResponse<Doc>>(
                $"/v3/workspaces/{workspaceId}/docs",
                request,
                expectedToken), Times.Once);
        }
    }
}
