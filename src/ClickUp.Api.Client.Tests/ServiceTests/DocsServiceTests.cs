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
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ClickUp.Api.Client.Models.ResponseModels.Shared;

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
            var request = new SearchDocsRequest { Query = "test query", Limit = 10, Cursor = "cursor123" };
            var expectedDocs = new List<Doc> { CreateSampleDoc() };
            var expectedResponse = new SearchDocsResponse(expectedDocs, "next_cursor_id", expectedDocs.Count, true);
            _mockApiConnection
                .Setup(c => c.GetAsync<SearchDocsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _docsService.SearchDocsAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.NextPageId, result.NextPageId);
            _mockApiConnection.Verify(c => c.GetAsync<SearchDocsResponse>(
                $"/v3/workspaces/{workspaceId}/docs?q=test%20query&limit=10&cursor=cursor123",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateDocAsync_ValidRequest_CallsPostAndReturnsDoc()
        {
            // Arrange
            var workspaceId = "ws_test";
            long? parsedWorkspaceId = long.TryParse(workspaceId, out long id) ? id : (long?)null;
            var request = new CreateDocRequest(Name: "New Doc", Parent: null, Visibility: "private", CreatePage: true, TemplateId: null, WorkspaceId: parsedWorkspaceId);
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
            var request = new CreatePageRequest(ParentPageId: null, Name: "New Page", SubTitle: null, Content: "<p>Hello</p>", ContentFormat: "text/html", OrderIndex: null, Hidden: null, TemplateId: null);
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
                    $"/v3/workspaces/{workspaceId}/docs/{docId}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _docsService.GetDocAsync(workspaceId, docId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDoc.Id, result.Id);
            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataResponse<Doc>>( // Corrected Verify
                $"/v3/workspaces/{workspaceId}/docs/{docId}",
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
                $"/v3/workspaces/{workspaceId}/docs/{docId}",
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
                    $"/v3/workspaces/{workspaceId}/docs/{docId}/pages", request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _docsService.CreatePageAsync(workspaceId, docId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPage.Id, result.Id);
            _mockApiConnection.Verify(c => c.PostAsync<CreatePageRequest, ClickUpV3DataResponse<Page>>(
                 $"/v3/workspaces/{workspaceId}/docs/{docId}/pages", request, It.IsAny<CancellationToken>()), Times.Once);
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
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pages",
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
                $"/v3/workspaces/{workspaceId}/docs?q={Uri.EscapeDataString(request.Query)}",
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

        // --- GetDocPageListingAsync Tests ---
        [Fact]
        public async Task GetDocPageListingAsync_ValidRequest_BuildsUrlAndReturnsListing()
        {
            // Arrange
            var workspaceId = "ws_page_list";
            var docId = "doc_for_listing";
            var maxDepth = 2;
            var expectedListing = new List<DocPageListingItem> { new DocPageListingItem("page_item_1", "Page Item 1", "ws_page_list", 123, "doc_id", 0, false, new List<DocPageListingItem>()) };
            var apiResponse = new ClickUpV3DataListResponse<DocPageListingItem> { Data = expectedListing };
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<DocPageListingItem>>(
                    $"/v3/workspaces/{workspaceId}/docs/{docId}/pageListing?max_page_depth={maxDepth}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _docsService.GetDocPageListingAsync(workspaceId, docId, maxDepth);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("page_item_1", result.First().Id);
            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataListResponse<DocPageListingItem>>(
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pageListing?max_page_depth={maxDepth}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDocPageListingAsync_MinimalRequest_BuildsUrlAndReturnsListing()
        {
            // Arrange
            var workspaceId = "ws_page_list_min";
            var docId = "doc_for_listing_min";
            var expectedListing = new List<DocPageListingItem> { new DocPageListingItem("page_item_min", "Page Item Min", "ws_page_list_min", 456, "doc_id", 1, true, null) };
            var apiResponse = new ClickUpV3DataListResponse<DocPageListingItem> { Data = expectedListing };
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<DocPageListingItem>>(
                    $"/v3/workspaces/{workspaceId}/docs/{docId}/pageListing", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _docsService.GetDocPageListingAsync(workspaceId, docId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataListResponse<DocPageListingItem>>(
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pageListing",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDocPageListingAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_pl_err";
            var docId = "doc_pl_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<DocPageListingItem>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _docsService.GetDocPageListingAsync(workspaceId, docId));
        }

        [Fact]
        public async Task GetDocPageListingAsync_NullResponse_ReturnsEmptyEnumerable()
        {
            var workspaceId = "ws_pl_null";
            var docId = "doc_pl_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<DocPageListingItem>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataListResponse<DocPageListingItem>)null);

            var result = await _docsService.GetDocPageListingAsync(workspaceId, docId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDocPageListingAsync_NullDataInResponse_ReturnsEmptyEnumerable()
        {
            var workspaceId = "ws_pl_null_data";
            var docId = "doc_pl_null_data";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<DocPageListingItem>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataListResponse<DocPageListingItem>{ Data = null });

            var result = await _docsService.GetDocPageListingAsync(workspaceId, docId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDocPageListingAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_pl_cancel";
            var docId = "doc_pl_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<DocPageListingItem>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _docsService.GetDocPageListingAsync(workspaceId, docId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetDocPageListingAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_pl_ct";
            var docId = "doc_pl_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new ClickUpV3DataListResponse<DocPageListingItem> { Data = new List<DocPageListingItem>() };
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<DocPageListingItem>>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(apiResponse);

            await _docsService.GetDocPageListingAsync(workspaceId, docId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataListResponse<DocPageListingItem>>(
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pageListing",
                expectedToken), Times.Once);
        }

        // --- GetDocPagesAsync Tests ---
        [Fact]
        public async Task GetDocPagesAsync_ValidRequest_BuildsUrlAndReturnsPages()
        {
            // Arrange
            var workspaceId = "ws_get_pages";
            var docId = "doc_for_pages";
            var maxDepth = 1;
            var contentFormat = "html";
            var expectedPages = new List<Page> { CreateSamplePage("page_abc", docId) };
            var apiResponse = new ClickUpV3DataListResponse<Page> { Data = expectedPages };
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<Page>>(
                    $"/v3/workspaces/{workspaceId}/docs/{docId}/pages?max_page_depth={maxDepth}&content_format={contentFormat}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _docsService.GetDocPagesAsync(workspaceId, docId, maxDepth, contentFormat);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("page_abc", result.First().Id);
            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataListResponse<Page>>(
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pages?max_page_depth={maxDepth}&content_format={contentFormat}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDocPagesAsync_MinimalRequest_BuildsUrlAndReturnsPages()
        {
            var workspaceId = "ws_get_pages_min";
            var docId = "doc_for_pages_min";
            var expectedPages = new List<Page> { CreateSamplePage("page_min_1", docId) };
            var apiResponse = new ClickUpV3DataListResponse<Page> { Data = expectedPages };
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<Page>>(
                    $"/v3/workspaces/{workspaceId}/docs/{docId}/pages", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var result = await _docsService.GetDocPagesAsync(workspaceId, docId);

            Assert.NotNull(result);
            Assert.Single(result);
            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataListResponse<Page>>(
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pages",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDocPagesAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_gp_err";
            var docId = "doc_gp_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<Page>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _docsService.GetDocPagesAsync(workspaceId, docId));
        }

        [Fact]
        public async Task GetDocPagesAsync_NullResponse_ReturnsEmptyEnumerable()
        {
            var workspaceId = "ws_gp_null";
            var docId = "doc_gp_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<Page>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataListResponse<Page>)null);

            var result = await _docsService.GetDocPagesAsync(workspaceId, docId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDocPagesAsync_NullDataInResponse_ReturnsEmptyEnumerable()
        {
            var workspaceId = "ws_gp_null_data";
            var docId = "doc_gp_null_data";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<Page>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataListResponse<Page>{ Data = null });

            var result = await _docsService.GetDocPagesAsync(workspaceId, docId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDocPagesAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_gp_cancel";
            var docId = "doc_gp_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<Page>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _docsService.GetDocPagesAsync(workspaceId, docId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetDocPagesAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_gp_ct";
            var docId = "doc_gp_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new ClickUpV3DataListResponse<Page> { Data = new List<Page>() };
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataListResponse<Page>>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(apiResponse);

            await _docsService.GetDocPagesAsync(workspaceId, docId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataListResponse<Page>>(
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pages",
                expectedToken), Times.Once);
        }

        // --- GetPageAsync Tests ---
        [Fact]
        public async Task GetPageAsync_ValidRequest_BuildsUrlAndReturnsPage()
        {
            // Arrange
            var workspaceId = "ws_get_page";
            var docId = "doc_for_get_page";
            var pageId = "page_to_get";
            var contentFormat = "markdown";
            var expectedPage = CreateSamplePage(pageId, docId);
            var apiResponse = new ClickUpV3DataResponse<Page> { Data = expectedPage };
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Page>>(
                    $"/v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}?content_format={contentFormat}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _docsService.GetPageAsync(workspaceId, docId, pageId, contentFormat);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPage.Id, result.Id);
            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataResponse<Page>>(
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}?content_format={contentFormat}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPageAsync_MinimalRequest_BuildsUrlAndReturnsPage()
        {
            var workspaceId = "ws_get_page_min";
            var docId = "doc_for_get_page_min";
            var pageId = "page_to_get_min";
            var expectedPage = CreateSamplePage(pageId, docId);
            var apiResponse = new ClickUpV3DataResponse<Page> { Data = expectedPage };
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Page>>(
                    $"/v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var result = await _docsService.GetPageAsync(workspaceId, docId, pageId);

            Assert.NotNull(result);
            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataResponse<Page>>(
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPageAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_gpage_err";
            var docId = "doc_gpage_err";
            var pageId = "page_gpage_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Page>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _docsService.GetPageAsync(workspaceId, docId, pageId));
        }

        [Fact]
        public async Task GetPageAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_gpage_null";
            var docId = "doc_gpage_null";
            var pageId = "page_gpage_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Page>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataResponse<Page>)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _docsService.GetPageAsync(workspaceId, docId, pageId));
        }

        [Fact]
        public async Task GetPageAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_gpage_null_data";
            var docId = "doc_gpage_null_data";
            var pageId = "page_gpage_null_data";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Page>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<Page>{ Data = null });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _docsService.GetPageAsync(workspaceId, docId, pageId));
        }

        [Fact]
        public async Task GetPageAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_gpage_cancel";
            var docId = "doc_gpage_cancel";
            var pageId = "page_gpage_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Page>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _docsService.GetPageAsync(workspaceId, docId, pageId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetPageAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_gpage_ct";
            var docId = "doc_gpage_ct";
            var pageId = "page_gpage_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new ClickUpV3DataResponse<Page> { Data = CreateSamplePage(pageId, docId) };
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<Page>>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(apiResponse);

            await _docsService.GetPageAsync(workspaceId, docId, pageId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataResponse<Page>>(
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}",
                expectedToken), Times.Once);
        }

        // --- EditPageAsync Tests ---
        [Fact]
        public async Task EditPageAsync_ValidRequest_CallsPutAsync()
        {
            // Arrange
            var workspaceId = "ws_edit_page";
            var docId = "doc_for_edit";
            var pageId = "page_to_edit";
            var request = new EditPageRequest(
                Name: "Updated Page Name",
                SubTitle: "Updated Subtitle",
                Content: "<p>New Content</p>",
                ContentEditMode: null,
                ContentFormat: "html",
                OrderIndex: 1,
                Hidden: false,
                ParentPageId: "parent_123"
            );

            _mockApiConnection
                .Setup(c => c.PutAsync($"/v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}", request, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _docsService.EditPageAsync(workspaceId, docId, pageId, request);

            // Assert
            _mockApiConnection.Verify(c => c.PutAsync(
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditPageAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_epage_err";
            var docId = "doc_epage_err";
            var pageId = "page_epage_err";
            var request = new EditPageRequest("Error Page Edit", "ST", "Error content", "html", null, null, null, null);
            _mockApiConnection
                .Setup(c => c.PutAsync(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _docsService.EditPageAsync(workspaceId, docId, pageId, request));
        }

        [Fact]
        public async Task EditPageAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_epage_cancel";
            var docId = "doc_epage_cancel";
            var pageId = "page_epage_cancel";
            var request = new EditPageRequest("Cancel Page Edit", "ST", "Cancel content", "html", null, null, null, null);
            _mockApiConnection
                .Setup(c => c.PutAsync(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _docsService.EditPageAsync(workspaceId, docId, pageId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task EditPageAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_epage_ct";
            var docId = "doc_epage_ct";
            var pageId = "page_epage_ct";
            var request = new EditPageRequest("CT Page Edit", "ST", "CT content", "html", null, null, null, null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(c => c.PutAsync(It.IsAny<string>(), request, expectedToken))
                .Returns(Task.CompletedTask);

            await _docsService.EditPageAsync(workspaceId, docId, pageId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PutAsync(
                $"/v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}",
                request,
                expectedToken), Times.Once);
        }

        // --- SearchAllDocsAsync Tests ---
        [Fact]
        public async Task SearchAllDocsAsync_NoResults_ReturnsEmpty()
        {
            // Arrange
            var workspaceId = "ws_search_all_empty";
            var request = new SearchDocsRequest { Query = "empty_query" };
            var apiResponse = new SearchDocsResponse(new List<Doc>(), null, 0, false);

            _mockApiConnection
                .Setup(c => c.GetAsync<SearchDocsResponse>(
                    $"/v3/workspaces/{workspaceId}/docs?q={Uri.EscapeDataString(request.Query)}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var results = new List<Doc>();
            await foreach (var doc in _docsService.SearchAllDocsAsync(workspaceId, request, CancellationToken.None))
            {
                results.Add(doc);
            }

            // Assert
            Assert.Empty(results);
            _mockApiConnection.Verify(c => c.GetAsync<SearchDocsResponse>(
                It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SearchAllDocsAsync_SinglePageOfResults_ReturnsAllItems()
        {
            // Arrange
            var workspaceId = "ws_search_all_single";
            var request = new SearchDocsRequest { Query = "single_page_query" };
            var docs = new List<Doc> { CreateSampleDoc("doc1"), CreateSampleDoc("doc2") };
            var apiResponse = new SearchDocsResponse(docs, null, docs.Count, false);

            _mockApiConnection
                .Setup(c => c.GetAsync<SearchDocsResponse>(
                    $"/v3/workspaces/{workspaceId}/docs?q={Uri.EscapeDataString(request.Query)}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var results = new List<Doc>();
            await foreach (var doc in _docsService.SearchAllDocsAsync(workspaceId, request, CancellationToken.None))
            {
                results.Add(doc);
            }

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Contains(results, d => d.Id == "doc1");
            Assert.Contains(results, d => d.Id == "doc2");
            _mockApiConnection.Verify(c => c.GetAsync<SearchDocsResponse>(
                It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SearchAllDocsAsync_MultiplePagesOfResults_ReturnsAllItems()
        {
            // Arrange
            var workspaceId = "ws_search_all_multi";
            var request = new SearchDocsRequest { Query = "multi_page_query" };
            var docsPage1 = new List<Doc> { CreateSampleDoc("doc_page1_1"), CreateSampleDoc("doc_page1_2") };
            var docsPage2 = new List<Doc> { CreateSampleDoc("doc_page2_1") };

            var apiResponsePage1 = new SearchDocsResponse(docsPage1, "cursor_page2", docsPage1.Count, true);
            var apiResponsePage2 = new SearchDocsResponse(docsPage2, null, docsPage2.Count, false);

            _mockApiConnection
                .SetupSequence(c => c.GetAsync<SearchDocsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponsePage1)
                .ReturnsAsync(apiResponsePage2);

            // Act
            var results = new List<Doc>();
            await foreach (var doc in _docsService.SearchAllDocsAsync(workspaceId, request, CancellationToken.None))
            {
                results.Add(doc);
            }

            // Assert
            Assert.Equal(3, results.Count);
            Assert.Contains(results, d => d.Id == "doc_page1_1");
            Assert.Contains(results, d => d.Id == "doc_page1_2");
            Assert.Contains(results, d => d.Id == "doc_page2_1");
            _mockApiConnection.Verify(c => c.GetAsync<SearchDocsResponse>(
                 $"/v3/workspaces/{workspaceId}/docs?q={Uri.EscapeDataString(request.Query)}", It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(c => c.GetAsync<SearchDocsResponse>(
                $"/v3/workspaces/{workspaceId}/docs?q={Uri.EscapeDataString(request.Query)}&cursor=cursor_page2", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SearchAllDocsAsync_ApiConnectionThrowsException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_search_all_err";
            var request = new SearchDocsRequest { Query = "error_query" };
            var apiException = new HttpRequestException("Network error");

            _mockApiConnection
                .Setup(c => c.GetAsync<SearchDocsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(apiException);

            // Act & Assert
            var iterator = _docsService.SearchAllDocsAsync(workspaceId, request, CancellationToken.None).GetAsyncEnumerator();
            await Assert.ThrowsAsync<HttpRequestException>(async () => await iterator.MoveNextAsync());
        }

        [Fact]
        public async Task SearchAllDocsAsync_CancellationRequested_StopsIteration()
        {
            // Arrange
            var workspaceId = "ws_search_all_cancel";
            var request = new SearchDocsRequest { Query = "cancel_query" };
            var docsPage1 = new List<Doc> { CreateSampleDoc("doc_cancel1") };
            var apiResponsePage1 = new SearchDocsResponse(docsPage1, "cursor_cancel2", docsPage1.Count, true);

            using var cts = new CancellationTokenSource();

            _mockApiConnection
                .Setup(c => c.GetAsync<SearchDocsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponsePage1)
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (url.Contains("cursor_cancel2"))
                    {
                        cts.Cancel();
                        token.ThrowIfCancellationRequested();
                    }
                });

            // Act
            var results = new List<Doc>();
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var doc in _docsService.SearchAllDocsAsync(workspaceId, request, cts.Token))
                {
                    results.Add(doc);
                }
            });

            // Assert
            Assert.Single(results);
             _mockApiConnection.Verify(c => c.GetAsync<SearchDocsResponse>(
                 $"/v3/workspaces/{workspaceId}/docs?q={Uri.EscapeDataString(request.Query)}", It.IsAny<CancellationToken>()), Times.Once);

        }
    }
}
