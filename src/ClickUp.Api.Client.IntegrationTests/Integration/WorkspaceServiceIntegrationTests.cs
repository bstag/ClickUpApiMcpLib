using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp; // Added for MockHttpHandler extension methods like .When()
using Xunit;
using Xunit.Abstractions; // For ITestOutputHelper if needed later

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class WorkspaceServiceIntegrationTests : IntegrationTestBase
    {
        private readonly IWorkspacesService _workspacesService;
        private readonly string _workspaceId;

        public WorkspaceServiceIntegrationTests() : base() // Add ITestOutputHelper if logging to test output
        {
            _workspacesService = ServiceProvider.GetRequiredService<IWorkspacesService>();

            // Ensure TestWorkspaceId is available, especially for Record/Passthrough modes
            if (string.IsNullOrWhiteSpace(ClientOptions.TestWorkspaceId) && CurrentTestMode != TestMode.Playback)
            {
                throw new InvalidOperationException(
                    "ClickUp TestWorkspaceId not configured for integration tests. " +
                    "Set via User Secrets (e.g., 'ClickUpApi:TestWorkspaceId') " +
                    "or Environment Variables (e.g., 'ClickUpApi__TestWorkspaceId'). " +
                    "Not strictly required for Playback if all paths are fully mocked, but good practice.");
            }
            _workspaceId = ClientOptions.TestWorkspaceId ?? "mock_workspace_id_for_playback"; // Fallback for playback if not set
        }

        [Fact]
        public async Task GetWorkspaceSeatsAsync_WhenApiCallIsSuccessful_ReturnsSeats()
        {
            // Arrange
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "WorkspacesService", "GetWorkspaceSeatsAsync", "Success.json");
                if (!File.Exists(responsePath))
                {
                    throw new FileNotFoundException($"Mock response file not found: {responsePath}. Ensure it's created for Playback mode.");
                }
                var responseContent = await File.ReadAllTextAsync(responsePath);
                MockHttpHandler.When($"https://api.clickup.com/api/v2/team/{_workspaceId}/seats")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }

            // Act
            var response = await _workspacesService.GetWorkspaceSeatsAsync(_workspaceId);

            // Assert
            response.Should().NotBeNull();
            response.Members.Should().NotBeNull(); // Changed from response.Seats
            response.Guests.Should().NotBeNull();  // Changed from response.Seats
            // Deeper assertions can be added based on WorkspaceMemberSeatsInfo and WorkspaceGuestSeatsInfo
            // For example:
            // response.Members.FilledSeats.Should().BeGreaterOrEqualTo(0);
            // response.Guests.FilledSeats.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task GetWorkspacePlanAsync_WhenApiCallIsSuccessful_ReturnsPlan()
        {
            // Arrange
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "WorkspacesService", "GetWorkspacePlanAsync", "Success.json");
                if (!File.Exists(responsePath))
                {
                    throw new FileNotFoundException($"Mock response file not found: {responsePath}. Ensure it's created for Playback mode.");
                }
                var responseContent = await File.ReadAllTextAsync(responsePath);
                MockHttpHandler.When($"https://api.clickup.com/api/v2/team/{_workspaceId}/plan")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }

            // Act
            var response = await _workspacesService.GetWorkspacePlanAsync(_workspaceId);

            // Assert
            response.Should().NotBeNull();
            response.PlanName.Should().NotBeNullOrEmpty(); // Changed from response.Plan
            response.PlanId.Should().BePositive();       // Changed from response.Plan and added a more specific check
            // Deeper assertions can be added based on actual plan details
            // For example:
            // response.PlanName.Should().Be("Free Forever"); // If that's what the mock returns
        }
    }
}
