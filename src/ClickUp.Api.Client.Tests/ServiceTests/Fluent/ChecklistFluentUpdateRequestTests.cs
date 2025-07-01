using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent
{
    public class ChecklistFluentUpdateRequestTests
    {
        private readonly Mock<ITaskChecklistsService> _mockService = new();
        private const string ChecklistId = "chk_1";

        [Fact]
        public void WithName_SetsName()
        {
            var request = new ChecklistFluentUpdateRequest(ChecklistId, _mockService.Object)
                .WithName("Test List");
            var nameField = typeof(ChecklistFluentUpdateRequest).GetField("_name", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Equal("Test List", nameField.GetValue(request));
        }

        [Fact]
        public void WithPosition_SetsPosition()
        {
            var request = new ChecklistFluentUpdateRequest(ChecklistId, _mockService.Object)
                .WithPosition(3);
            var posField = typeof(ChecklistFluentUpdateRequest).GetField("_position", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Equal(3, posField.GetValue(request));
        }

        [Fact]
        public async Task EditAsync_CallsServiceWithCorrectParameters()
        {
            _mockService.Setup(x => x.EditChecklistAsync(ChecklistId, It.IsAny<ClickUp.Api.Client.Models.RequestModels.Checklists.EditChecklistRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask).Verifiable();
            var request = new ChecklistFluentUpdateRequest(ChecklistId, _mockService.Object)
                .WithName("ListName").WithPosition(2);
            await request.EditAsync();
            _mockService.Verify(x => x.EditChecklistAsync(ChecklistId, It.Is<ClickUp.Api.Client.Models.RequestModels.Checklists.EditChecklistRequest>(r => r.Name == "ListName" && r.Position == 2), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
