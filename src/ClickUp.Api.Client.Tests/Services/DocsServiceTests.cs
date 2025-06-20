using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using ClickUp.Api.Client.Models.ResponseModels.Docs;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ClickUp.Api.Client.Tests.Services
{
    public class DocsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly DocsService _docsService;
        private const string BaseV3WorkspaceEndpoint = "/v3/workspaces";


        public DocsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _docsService = new DocsService(_mockApiConnection.Object);
        }

        // Placeholder DTO creation helpers
        private Doc CreateSampleDoc(string id, string name)
        {
            // Simplified placeholder
            var doc = (Doc)Activator.CreateInstance(typeof(Doc), nonPublic: true)!;
            var props = typeof(Doc).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(doc, id);
            props.FirstOrDefault(p => p.Name == "Name")?.SetValue(doc, name);
            return doc;
        }

        // Conceptual V3 response wrapper for single entities
        private ClickUpV3DataResponse<T> CreateV3DataResponse<T>(T data)
        {
            var responseType = typeof(ClickUpV3DataResponse<>).MakeGenericType(typeof(T));
            var responseInstance = Activator.CreateInstance(responseType)!;
            responseType.GetProperty("Data")?.SetValue(responseInstance, data);
            return (ClickUpV3DataResponse<T>)responseInstance;
        }


        [Fact]
        public async Task GetDocAsync_WhenDocExists_ReturnsDoc()
        {
            // Arrange
            var workspaceId = "ws-id";
            var docId = "doc-id";
            var sampleDoc = CreateSampleDoc(docId, "Test Document");
            var expectedResponse = CreateV3DataResponse(sampleDoc);

            _mockApiConnection.Setup(c => c.GetAsync<ClickUpV3DataResponse<Doc>>(
                $"{BaseV3WorkspaceEndpoint}/{workspaceId}/docs/{docId}",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _docsService.GetDocAsync(workspaceId, docId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleDoc);
        }

        [Fact]
        public async Task CreateDocAsync_ValidRequest_ReturnsCreatedDoc()
        {
            // Arrange
            var workspaceId = "ws-id";
            var requestDto = new CreateDocRequest("New Doc", null, null, null, null);
            var sampleDoc = CreateSampleDoc("new-doc-id", "New Doc");
            var expectedResponse = CreateV3DataResponse(sampleDoc);

            _mockApiConnection.Setup(c => c.PostAsync<CreateDocRequest, ClickUpV3DataResponse<Doc>>(
                $"{BaseV3WorkspaceEndpoint}/{workspaceId}/docs",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _docsService.CreateDocAsync(workspaceId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleDoc);
        }

        [Fact]
        public async Task SearchDocsAsync_ValidQuery_ReturnsSearchDocsResponse()
        {
            // Arrange
            var workspaceId = "ws-id";
            var query = "important";
            var requestDto = new SearchDocsRequest(query, 10, null, null, null, null, null, null, null);
            var expectedResponse = new SearchDocsResponse(new List<Doc> { CreateSampleDoc("doc1", "Important Doc") }, "next-cursor");
            var expectedEndpoint = $"{BaseV3WorkspaceEndpoint}/{workspaceId}/docs?q={query}&limit=10";


            _mockApiConnection.Setup(c => c.GetAsync<SearchDocsResponse>(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _docsService.SearchDocsAsync(workspaceId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
            _mockApiConnection.Verify(c => c.GetAsync<SearchDocsResponse>(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditPageAsync_ValidRequest_CallsApiConnectionPutAsync() // Testing a void method
        {
            // Arrange
            var workspaceId = "ws-id";
            var docId = "doc-id";
            var pageId = "page-id";
            var requestDto = new UpdatePageRequest("New Page Name", "New Content");
            var expectedEndpoint = $"{BaseV3WorkspaceEndpoint}/{workspaceId}/docs/{docId}/pages/{pageId}";

            _mockApiConnection.Setup(c => c.PutAsync<UpdatePageRequest>(
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _docsService.EditPageAsync(workspaceId, docId, pageId, requestDto, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.PutAsync<UpdatePageRequest>(
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    // Minimal internal helper DTO for conceptual V3 data wrapping, if not defined in main models.
    // This should ideally be in the Models project if it's a confirmed pattern.
    internal class ClickUpV3DataResponse<T> { public T? Data { get; set; } }
}
