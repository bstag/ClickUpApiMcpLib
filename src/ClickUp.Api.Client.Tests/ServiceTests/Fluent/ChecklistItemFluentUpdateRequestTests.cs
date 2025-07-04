using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;
using ClickUp.Api.Client.Models.Entities.Checklists;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent
{
    public class ChecklistItemFluentUpdateRequestTests
    {
        private readonly Mock<ITaskChecklistsService> _mockService = new();
        private const string ChecklistId = "chk_1";
        private const string ChecklistItemId = "item_1";

        [Fact]
        public void WithName_SetsName()
        {
            var req = new ChecklistItemFluentUpdateRequest(ChecklistId, ChecklistItemId, _mockService.Object).WithName("ItemName");
            var field = typeof(ChecklistItemFluentUpdateRequest).GetField("_name", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Equal("ItemName", field!.GetValue(req));
        }

        [Fact]
        public void WithResolved_SetsResolved()
        {
            var req = new ChecklistItemFluentUpdateRequest(ChecklistId, ChecklistItemId, _mockService.Object).WithResolved(true);
            var field = typeof(ChecklistItemFluentUpdateRequest).GetField("_resolved", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var value = field!.GetValue(req);
            Assert.NotNull(value);
            Assert.True((bool)value!);
        }

        [Fact]
        public void WithAssignee_SetsAssignee()
        {
            var req = new ChecklistItemFluentUpdateRequest(ChecklistId, ChecklistItemId, _mockService.Object).WithAssignee("user1");
            var field = typeof(ChecklistItemFluentUpdateRequest).GetField("_assignee", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Equal("user1", field!.GetValue(req));
        }

        [Fact]
        public void WithParent_SetsParent()
        {
            var req = new ChecklistItemFluentUpdateRequest(ChecklistId, ChecklistItemId, _mockService.Object).WithParent("parent1");
            var field = typeof(ChecklistItemFluentUpdateRequest).GetField("_parent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Equal("parent1", field!.GetValue(req));
        }

        [Fact]
        public async Task EditAsync_CallsServiceWithCorrectParameters()
        {
            // Setup: Assignee is int? (parsed from string), returns EditChecklistItemResponse
            _mockService.Setup(x => x.EditChecklistItemAsync(ChecklistId, ChecklistItemId, It.IsAny<EditChecklistItemRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EditChecklistItemResponse { Checklist = new("dummy_checklist_id", "dummy_task_id", "dummy_name", 0, 0, 0, null) });
            var req = new ChecklistItemFluentUpdateRequest(ChecklistId, ChecklistItemId, _mockService.Object)
                .WithName("ItemName").WithResolved(true).WithAssignee("123").WithParent("parent1");
            await req.EditAsync();
            _mockService.Verify(x => x.EditChecklistItemAsync(
                ChecklistId,
                ChecklistItemId,
                It.Is<EditChecklistItemRequest>(r => r.Name == "ItemName" && r.Resolved == true && r.Assignee == 123 && r.Parent == "parent1"),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }
    }
}
