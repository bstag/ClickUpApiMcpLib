using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.ResponseModels.Sharing; // For SharedHierarchyResponse
using ClickUp.Api.Client.Models.Entities; // For Space, Folder, List if they are part of SharedHierarchyResponse
using ClickUp.Api.Client.Models.Entities.Spaces; // For Space class
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Collections.Generic; // Required for List<T>

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class SharedHierarchyServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly SharedHierarchyService _sharedHierarchyService;
        private readonly Mock<ILogger<SharedHierarchyService>> _mockLogger;

        public SharedHierarchyServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<SharedHierarchyService>>();
            _sharedHierarchyService = new SharedHierarchyService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private SharedHierarchyResponse CreateSampleSharedHierarchyResponse()
        {
            // SharedHierarchyDetails does not contain Spaces.
            // Lists and Folders should be of type SharedHierarchyListItem and SharedHierarchyFolderItem respectively.
            // For now, providing empty lists for these as their specific DTOs are not yet read.
            var sharedDetails = new SharedHierarchyDetailsResponse(
                Tasks: new List<string>(),
                Lists: new List<SharedHierarchyListItem>(),
                Folders: new List<SharedHierarchyFolderItem>()
            );
            return new SharedHierarchyResponse(sharedDetails);
        }

        // Helper for Space, correcting 'IsPrivate' to 'Private' and ensuring all required params are met.
        // This helper might not be directly used in THIS test file anymore if SharedHierarchyResponse doesn't contain full Space objects.
        private Space CreateSampleSpaceForTest(string id = "space_1", string name = "Sample Space")
        {
            return new Space(
                Id: id, Name: name, Private: false, Color: null, Avatar: null, AdminCanManage: null, Archived: null,
                Members: null, Statuses: new List<ClickUp.Api.Client.Models.Common.Status>(),
                MultipleAssignees: false,
                Features: new ClickUp.Api.Client.Models.Entities.Spaces.Features(
                    DueDates: new ClickUp.Api.Client.Models.Entities.Spaces.DueDatesFeature(Enabled: false, StartDateEnabled: null, RemapDueDatesEnabled: null, DueDatesForSubtasksRollUpEnabled: null),
                    Sprints: new ClickUp.Api.Client.Models.Entities.Spaces.SprintsFeature(Enabled: false, LegacySprintsEnabled: null),
                    Points: new ClickUp.Api.Client.Models.Entities.Spaces.PointsFeature(Enabled: false),
                    CustomTaskIds: new ClickUp.Api.Client.Models.Entities.Spaces.CustomTaskIdsFeature(Enabled: false),
                    TimeTracking: new ClickUp.Api.Client.Models.Entities.Spaces.TimeTrackingFeature(Enabled: false, HarvestEnabled: null, RollUpEnabled: null),
                    Tags: new ClickUp.Api.Client.Models.Entities.Spaces.TagsFeature(Enabled: false),
                    TimeEstimates: new ClickUp.Api.Client.Models.Entities.Spaces.TimeEstimatesFeature(Enabled: false, RollUpEnabled: null, PerAssigneeEnabled: null),
                    Checklists: new ClickUp.Api.Client.Models.Entities.Spaces.ChecklistsFeature(Enabled: false),
                    CustomFields: new ClickUp.Api.Client.Models.Entities.Spaces.CustomFieldsFeature(Enabled: false),
                    RemapDependencies: new ClickUp.Api.Client.Models.Entities.Spaces.RemapDependenciesFeature(Enabled: false),
                    DependencyWarning: new ClickUp.Api.Client.Models.Entities.Spaces.DependencyWarningFeature(Enabled: false),
                    MultipleAssignees: new ClickUp.Api.Client.Models.Entities.Spaces.MultipleAssigneesFeature(Enabled: false),
                    Portfolios: new ClickUp.Api.Client.Models.Entities.Spaces.PortfoliosFeature(Enabled: false),
                    Emails: new ClickUp.Api.Client.Models.Entities.Spaces.EmailsFeature(Enabled: false)
                ),
                TeamId: null, DefaultListSettings: null
            );
        }


        // --- Tests for GetSharedHierarchyAsync ---

        [Fact]
        public async Task GetSharedHierarchyAsync_ValidWorkspaceId_ReturnsSharedHierarchy()
        {
            // Arrange
            var workspaceId = "ws123";
            var expectedResponse = CreateSampleSharedHierarchyResponse(); // This now creates an empty shared hierarchy

            _mockApiConnection
                .Setup(x => x.GetAsync<SharedHierarchyResponse>(
                    $"team/{workspaceId}/shared",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _sharedHierarchyService.GetSharedHierarchyAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Shared);
            // Cannot assert on result.Shared.Spaces as it doesn't exist on SharedHierarchyDetails
            // Assert.Single(result.Shared.Spaces);
            // Assert.Equal("space_1", result.Shared.Spaces[0].Id);
            Assert.Empty(result.Shared.Lists); // Based on corrected sample data
            Assert.Empty(result.Shared.Folders); // Based on corrected sample data
            _mockApiConnection.Verify(x => x.GetAsync<SharedHierarchyResponse>(
                $"team/{workspaceId}/shared",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetSharedHierarchyAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<SharedHierarchyResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SharedHierarchyResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sharedHierarchyService.GetSharedHierarchyAsync(workspaceId)
            );
        }

        [Fact]
        public async Task GetSharedHierarchyAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<SharedHierarchyResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _sharedHierarchyService.GetSharedHierarchyAsync(workspaceId)
            );
        }

        [Fact]
        public async Task GetSharedHierarchyAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<SharedHierarchyResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _sharedHierarchyService.GetSharedHierarchyAsync(workspaceId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetSharedHierarchyAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.GetAsync<SharedHierarchyResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(CreateSampleSharedHierarchyResponse()); // Return a valid response

            // Act
            await _sharedHierarchyService.GetSharedHierarchyAsync(workspaceId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<SharedHierarchyResponse>(
                $"team/{workspaceId}/shared",
                expectedToken), Times.Once);
        }
    }
}
