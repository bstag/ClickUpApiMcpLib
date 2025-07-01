using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent
{
    public class TaskChecklistsFluentApiTests
    {
        private readonly Mock<ITaskChecklistsService> _mockService = new();
        private readonly TaskChecklistsFluentApi _api;

        public TaskChecklistsFluentApiTests()
        {
            _api = new TaskChecklistsFluentApi(_mockService.Object);
        }

        [Fact]
        public void CreateChecklist_ReturnsRequest()
        {
            var req = _api.CreateChecklist("task1");
            Assert.NotNull(req);
        }

        [Fact]
        public void EditChecklist_ReturnsRequest()
        {
            var req = _api.EditChecklist("chk1");
            Assert.NotNull(req);
        }

        [Fact]
        public void CreateChecklistItem_ReturnsRequest()
        {
            var req = _api.CreateChecklistItem("chk1");
            Assert.NotNull(req);
        }

        [Fact]
        public void EditChecklistItem_ReturnsRequest()
        {
            var req = _api.EditChecklistItem("chk1", "item1");
            Assert.NotNull(req);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteChecklistAsync_CallsService()
        {
            _mockService.Setup(x => x.DeleteChecklistAsync("chk1", default)).Returns(System.Threading.Tasks.Task.CompletedTask).Verifiable();
            await _api.DeleteChecklistAsync("chk1");
            _mockService.Verify(x => x.DeleteChecklistAsync("chk1", default), Times.Once);
        }
    }
}
