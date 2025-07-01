using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent
{
    public class DocsFluentApiTests
    {
        private readonly Mock<IDocsService> _mockService = new();
        private readonly DocsFluentApi _api;

        public DocsFluentApiTests()
        {
            _api = new DocsFluentApi(_mockService.Object);
        }

        [Fact]
        public void SearchDocs_ReturnsRequest()
        {
            var req = _api.SearchDocs("ws1");
            Assert.NotNull(req);
        }

        [Fact]
        public void CreateDoc_ReturnsRequest()
        {
            var req = _api.CreateDoc("ws1", "docName");
            Assert.NotNull(req);
        }
    }
}
