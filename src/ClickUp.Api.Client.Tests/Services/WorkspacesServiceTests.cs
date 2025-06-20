using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
// Assuming WorkspaceSeats and WorkspacePlan are directly in ResponseModels.Workspaces or Entities
using ClickUp.Api.Client.Models.ResponseModels.Workspaces;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic; // Required for List if used in DTOs

namespace ClickUp.Api.Client.Tests.Services
{
    public class WorkspacesServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly WorkspacesService _workspacesService;
        private const string BaseWorkspaceEndpoint = "team";


        public WorkspacesServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _workspacesService = new WorkspacesService(_mockApiConnection.Object);
        }

        // Placeholder DTO creation helpers
        private WorkspaceSeats CreateSampleWorkspaceSeats()
        {
            // Assuming WorkspaceSeats is a record or class with a parameterless constructor
            // and settable properties, or a constructor that can be used.
            // For this example, let's assume it has a Members property with a Count.
            var seatsInstance = (WorkspaceSeats)Activator.CreateInstance(typeof(WorkspaceSeats), nonPublic: true)!;

            // If WorkspaceSeats has a structure like: public class Members { public int Count { get; set; } }
            // And WorkspaceSeats has: public Members Members { get; set; }
            // This setup would be more complex. For now, assume simple properties if any are asserted.
            // If it's just a wrapper around a list or count, that's simpler.
            // For now, just creating an instance is enough as BeEquivalentTo will check structure.
            // If WorkspaceSeats is: public record WorkspaceSeats(MembersSeats Members);
            // and MembersSeats is: public record MembersSeats(int FilledMembersSeats, int TotalMembersSeats, int PendingSeats);
            // then:
            // var membersSeats = (MembersSeats)Activator.CreateInstance(typeof(MembersSeats), new object[] { 5, 10, 1 });
            // return (WorkspaceSeats)Activator.CreateInstance(typeof(WorkspaceSeats), new object[] { membersSeats });
            return seatsInstance;
        }

        private WorkspacePlan CreateSampleWorkspacePlan()
        {
            var planInstance = (WorkspacePlan)Activator.CreateInstance(typeof(WorkspacePlan), nonPublic: true)!;
            // Example if it has a Name property:
            // typeof(WorkspacePlan).GetProperty("Name")?.SetValue(planInstance, "Enterprise");
            return planInstance;
        }


        [Fact]
        public async Task GetWorkspaceSeatsAsync_WhenDataExists_ReturnsWorkspaceSeats()
        {
            // Arrange
            var workspaceId = "ws-id-seats";
            var expectedSeats = CreateSampleWorkspaceSeats();

            _mockApiConnection.Setup(c => c.GetAsync<WorkspaceSeats>(
                $"{BaseWorkspaceEndpoint}/{workspaceId}/seats",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSeats);

            // Act
            var result = await _workspacesService.GetWorkspaceSeatsAsync(workspaceId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedSeats);
        }

        [Fact]
        public async Task GetWorkspacePlanAsync_WhenDataExists_ReturnsWorkspacePlan()
        {
            // Arrange
            var workspaceId = "ws-id-plan";
            var expectedPlan = CreateSampleWorkspacePlan();

            _mockApiConnection.Setup(c => c.GetAsync<WorkspacePlan>(
                $"{BaseWorkspaceEndpoint}/{workspaceId}/plan",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPlan);

            // Act
            var result = await _workspacesService.GetWorkspacePlanAsync(workspaceId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedPlan);
        }
    }
}
