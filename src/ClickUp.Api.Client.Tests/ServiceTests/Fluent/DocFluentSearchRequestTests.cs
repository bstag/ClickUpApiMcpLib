using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent
{
    public class DocFluentSearchRequestTests
    {
        private readonly Mock<IDocsService> _mockService = new();
        private const string WorkspaceId = "ws1";

        [Fact]
        public void WithQuery_SetsQuery()
        {
            var req = new DocFluentSearchRequest(WorkspaceId, _mockService.Object).WithQuery("search");
            var field = typeof(DocFluentSearchRequest).GetField("_query", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Equal("search", field.GetValue(req));
        }

        [Fact]
        public void WithParentId_SetsParentId()
        {
            var req = new DocFluentSearchRequest(WorkspaceId, _mockService.Object).WithParentId("pid");
            var field = typeof(DocFluentSearchRequest).GetField("_parentId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Equal("pid", field.GetValue(req));
        }

        [Fact]
        public void WithParentType_SetsParentType()
        {
            var req = new DocFluentSearchRequest(WorkspaceId, _mockService.Object).WithParentType(2);
            var field = typeof(DocFluentSearchRequest).GetField("_parentType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Equal(2, field.GetValue(req));
        }

        [Fact]
        public void WithCursor_SetsCursor()
        {
            var req = new DocFluentSearchRequest(WorkspaceId, _mockService.Object).WithCursor("cur");
            var field = typeof(DocFluentSearchRequest).GetField("_cursor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Equal("cur", field.GetValue(req));
        }

        [Fact]
        public void WithLimit_SetsLimit()
        {
            var req = new DocFluentSearchRequest(WorkspaceId, _mockService.Object).WithLimit(33);
            var field = typeof(DocFluentSearchRequest).GetField("_limit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Equal(33, field.GetValue(req));
        }

        [Fact]
        public async Task SearchAsync_CallsServiceWithCorrectParameters()
        {
            var docsList = new List<ClickUp.Api.Client.Models.Entities.Docs.Doc>();
            var pagedResult = new ClickUp.Api.Client.Models.Common.Pagination.PagedResult<ClickUp.Api.Client.Models.Entities.Docs.Doc>(
                docsList, 0, 10, false // items, page, pageSize, hasNextPage
            );
            _mockService.Setup(x => x.SearchDocsAsync(WorkspaceId, It.IsAny<ClickUp.Api.Client.Models.RequestModels.Docs.SearchDocsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult).Verifiable();
            var req = new DocFluentSearchRequest(WorkspaceId, _mockService.Object).WithQuery("abc").WithLimit(2);
            await req.SearchAsync();
            _mockService.Verify(x => x.SearchDocsAsync(WorkspaceId, It.Is<ClickUp.Api.Client.Models.RequestModels.Docs.SearchDocsRequest>(r => r.Query == "abc" && r.Limit == 2), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
