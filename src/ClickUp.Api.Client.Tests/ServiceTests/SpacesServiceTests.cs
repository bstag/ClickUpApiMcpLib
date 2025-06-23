using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Space
using ClickUp.Api.Client.Models.RequestModels.Spaces; // For CreateSpaceRequest, UpdateSpaceRequest
using ClickUp.Api.Client.Models.ResponseModels.Spaces; // For GetSpacesResponse
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class SpacesServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly SpacesService _spacesService;
        private readonly Mock<ILogger<SpacesService>> _mockLogger;

        public SpacesServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<SpacesService>>();
            _spacesService = new SpacesService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private Space CreateSampleSpace(string id = "space_1", string name = "Sample Space")
        {
            // This is a simplified example. Adjust based on the actual structure of Space.
            return new Space(
                Id: id,
                Name: name,
        Private: false,
        Color: null,
        Avatar: null,
        AdminCanManage: null,
        Archived: false,
        Members: null,
        Statuses: new List<ClickUp.Api.Client.Models.Common.Status>(),
        MultipleAssignees: false,
        // Correctly instantiate Features and its components
        Features: new Features(
            DueDates: new DueDatesFeature(Enabled: false, StartDateEnabled: null, RemapDueDatesEnabled: null, DueDatesForSubtasksRollUpEnabled: null),
            Sprints: new SprintsFeature(Enabled: false, LegacySprintsEnabled: null),
            Points: new PointsFeature(Enabled: false),
            CustomTaskIds: new CustomTaskIdsFeature(Enabled: false),
            TimeTracking: new TimeTrackingFeature(Enabled: false, HarvestEnabled: null, RollUpEnabled: null),
            Tags: new TagsFeature(Enabled: false),
            TimeEstimates: new TimeEstimatesFeature(Enabled: false, RollUpEnabled: null, PerAssigneeEnabled: null),
            Checklists: new ChecklistsFeature(Enabled: false),
            CustomFields: new CustomFieldsFeature(Enabled: false), // Correct type name
            RemapDependencies: new RemapDependenciesFeature(Enabled: false),
            DependencyWarning: new DependencyWarningFeature(Enabled: false),
            MultipleAssignees: new MultipleAssigneesFeature(Enabled: false), // Correct type name
            Portfolios: new PortfoliosFeature(Enabled: false),
            Emails: new EmailsFeature(Enabled: false)
            // Removed incorrect CustomItemsFeature, PrioritiesFeature, ZoomFeature, MilestonesFeature from here as they are not direct params of Features constructor
                ),
        TeamId: null,
        DefaultListSettings: null
            );
        }

        // --- Tests for GetSpacesAsync ---

        [Fact]
        public async Task GetSpacesAsync_ValidWorkspaceId_ReturnsSpaces()
        {
            // Arrange
            var workspaceId = "ws123";
            var expectedSpaces = new List<Space> { CreateSampleSpace("space1", "Space One") };
            var apiResponse = new GetSpacesResponse(expectedSpaces);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpacesResponse>(
                    It.Is<string>(s => s.StartsWith($"team/{workspaceId}/space")),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _spacesService.GetSpacesAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("space1", result.First().Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetSpacesResponse>(
                $"team/{workspaceId}/space", // No query params by default
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetSpacesAsync_WithArchivedTrue_BuildsCorrectUrl()
        {
            // Arrange
            var workspaceId = "ws456";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpacesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetSpacesResponse(new List<Space>()));

            // Act
            await _spacesService.GetSpacesAsync(workspaceId, archived: true);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetSpacesResponse>(
                $"team/{workspaceId}/space?archived=true",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetSpacesAsync_WithArchivedFalse_BuildsCorrectUrl()
        {
            // Arrange
            var workspaceId = "ws789";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpacesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetSpacesResponse(new List<Space>()));

            // Act
            await _spacesService.GetSpacesAsync(workspaceId, archived: false);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetSpacesResponse>(
                $"team/{workspaceId}/space?archived=false",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetSpacesAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpacesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetSpacesResponse)null);

            // Act
            var result = await _spacesService.GetSpacesAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSpacesAsync_ApiReturnsResponseWithNullSpaces_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_null_spaces_in_resp";
            var apiResponse = new GetSpacesResponse(null!); // Spaces property is null
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpacesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _spacesService.GetSpacesAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSpacesAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpacesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _spacesService.GetSpacesAsync(workspaceId)
            );
        }

        [Fact]
        public async Task GetSpacesAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpacesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _spacesService.GetSpacesAsync(workspaceId, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetSpacesAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpacesResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetSpacesResponse(new List<Space>()));

            // Act
            await _spacesService.GetSpacesAsync(workspaceId, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetSpacesResponse>(
                $"team/{workspaceId}/space",
                expectedToken), Times.Once);
        }

        // --- Tests for CreateSpaceAsync ---

        [Fact]
        public async Task CreateSpaceAsync_ValidRequest_ReturnsSpace()
        {
            // Arrange
            var workspaceId = "ws123_create";
            var request = new CreateSpaceRequest(
                Name: "New Awesome Space",
                MultipleAssignees: true,
                // Correctly instantiate Features and its components for the request
                Features: new Features(
                    DueDates: new DueDatesFeature(Enabled: true, StartDateEnabled: true, RemapDueDatesEnabled: true, DueDatesForSubtasksRollUpEnabled: true),
                    Sprints: new SprintsFeature(Enabled: true, LegacySprintsEnabled: null), // Assuming null for optional if not specified
                    Points: new PointsFeature(Enabled: true),
                    CustomTaskIds: new CustomTaskIdsFeature(Enabled: false), // Default to false or based on test needs
                    TimeTracking: new TimeTrackingFeature(Enabled: true, HarvestEnabled: null, RollUpEnabled: null),
                    Tags: new TagsFeature(Enabled: true),
                    TimeEstimates: new TimeEstimatesFeature(Enabled: true, RollUpEnabled: null, PerAssigneeEnabled: null),
                    Checklists: new ChecklistsFeature(Enabled: true),
                    CustomFields: new CustomFieldsFeature(Enabled: true),
                    RemapDependencies: new RemapDependenciesFeature(Enabled: true),
                    DependencyWarning: new DependencyWarningFeature(Enabled: false), // Default to false or based on test needs
                    MultipleAssignees: new MultipleAssigneesFeature(Enabled: true), // Matches request.MultipleAssignees
                    Portfolios: new PortfoliosFeature(Enabled: false), // Default to false or based on test needs
                    Emails: new EmailsFeature(Enabled: true)
                )
            );
            var expectedSpace = CreateSampleSpace("space_new", "New Awesome Space");

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateSpaceRequest, Space>(
                    $"team/{workspaceId}/space",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSpace);

            // Act
            var result = await _spacesService.CreateSpaceAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedSpace.Id, result.Id);
            Assert.Equal(expectedSpace.Name, result.Name);
            _mockApiConnection.Verify(x => x.PostAsync<CreateSpaceRequest, Space>(
                $"team/{workspaceId}/space",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateSpaceAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_create_null_api_resp";
            var request = new CreateSpaceRequest("Null Resp Space", false, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateSpaceRequest, Space>(It.IsAny<string>(), It.IsAny<CreateSpaceRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Space)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _spacesService.CreateSpaceAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task CreateSpaceAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_create_http_ex";
            var request = new CreateSpaceRequest("Http Ex Space", false, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateSpaceRequest, Space>(It.IsAny<string>(), It.IsAny<CreateSpaceRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _spacesService.CreateSpaceAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task CreateSpaceAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_create_cancel_ex";
            var request = new CreateSpaceRequest("Cancel Ex Space", false, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateSpaceRequest, Space>(It.IsAny<string>(), It.IsAny<CreateSpaceRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _spacesService.CreateSpaceAsync(workspaceId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task CreateSpaceAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_create_ct_pass";
            var request = new CreateSpaceRequest("CT Pass Space", false, null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedSpace = CreateSampleSpace("space_ct_new", "CT Pass Space");

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateSpaceRequest, Space>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(expectedSpace);

            // Act
            await _spacesService.CreateSpaceAsync(workspaceId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateSpaceRequest, Space>(
                $"team/{workspaceId}/space",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for GetSpaceAsync ---

        [Fact]
        public async Task GetSpaceAsync_ValidSpaceId_ReturnsSpace()
        {
            // Arrange
            var spaceId = "space123_get";
            var expectedSpace = CreateSampleSpace(spaceId, "Specific Space");

            _mockApiConnection
                .Setup(x => x.GetAsync<Space>(
                    $"space/{spaceId}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSpace);

            // Act
            var result = await _spacesService.GetSpaceAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedSpace.Id, result.Id);
            _mockApiConnection.Verify(x => x.GetAsync<Space>(
                $"space/{spaceId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetSpaceAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var spaceId = "space_get_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<Space>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Space)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _spacesService.GetSpaceAsync(spaceId)
            );
        }

        [Fact]
        public async Task GetSpaceAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_get_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<Space>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _spacesService.GetSpaceAsync(spaceId)
            );
        }

        [Fact]
        public async Task GetSpaceAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_get_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<Space>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _spacesService.GetSpaceAsync(spaceId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetSpaceAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var spaceId = "space_get_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedSpace = CreateSampleSpace(spaceId, "CT Pass Get Space");

            _mockApiConnection
                .Setup(x => x.GetAsync<Space>(
                    It.IsAny<string>(),
                    expectedToken))
                .ReturnsAsync(expectedSpace);

            // Act
            await _spacesService.GetSpaceAsync(spaceId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<Space>(
                $"space/{spaceId}",
                expectedToken), Times.Once);
        }

        // --- Tests for UpdateSpaceAsync ---

        [Fact]
        public async Task UpdateSpaceAsync_ValidRequest_ReturnsUpdatedSpace()
        {
            // Arrange
            var spaceId = "space123_update";
            var request = new UpdateSpaceRequest(
                Name: "Updated Space Name",
                Color: "#00FF00",
                Private: true, // Corrected from IsPrivate
                AdminCanManage: null,
                MultipleAssignees: null, // Added to match constructor, assuming null for this test
                Features: null,
                Archived: null
            );
            var expectedSpace = CreateSampleSpace(spaceId, "Updated Space Name");
            // Use 'Private' to match the DTO property for the 'with' expression
            expectedSpace = expectedSpace with { Color = "#00FF00", Private = true }; // This line was already correct in my previous application.

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateSpaceRequest, Space>(
                    $"space/{spaceId}",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSpace);

            // Act
            var result = await _spacesService.UpdateSpaceAsync(spaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedSpace.Name, result.Name);
            Assert.Equal(expectedSpace.Color, result.Color);
            Assert.Equal(expectedSpace.Private, result.Private); // Corrected IsPrivate to Private in the Assert
            _mockApiConnection.Verify(x => x.PutAsync<UpdateSpaceRequest, Space>(
                $"space/{spaceId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateSpaceAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var spaceId = "space_update_null_api_resp";
            var request = new UpdateSpaceRequest(Name: "Update Null Resp", Color: null, Private: null, AdminCanManage: null, MultipleAssignees: null, Features: null, Archived: null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateSpaceRequest, Space>(It.IsAny<string>(), It.IsAny<UpdateSpaceRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Space)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _spacesService.UpdateSpaceAsync(spaceId, request)
            );
        }

        [Fact]
        public async Task UpdateSpaceAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_update_http_ex";
            var request = new UpdateSpaceRequest(Name: "Update Http Ex", Color: null, Private: null, AdminCanManage: null, MultipleAssignees: null, Features: null, Archived: null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateSpaceRequest, Space>(It.IsAny<string>(), It.IsAny<UpdateSpaceRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _spacesService.UpdateSpaceAsync(spaceId, request)
            );
        }

        [Fact]
        public async Task UpdateSpaceAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_update_cancel_ex";
            var request = new UpdateSpaceRequest(Name: "Update Cancel Ex", Color: null, Private: null, AdminCanManage: null, MultipleAssignees: null, Features: null, Archived: null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateSpaceRequest, Space>(It.IsAny<string>(), It.IsAny<UpdateSpaceRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _spacesService.UpdateSpaceAsync(spaceId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task UpdateSpaceAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var spaceId = "space_update_ct_pass";
            var request = new UpdateSpaceRequest(Name: "Update CT Pass", Color: null, Private: null, AdminCanManage: null, MultipleAssignees: null, Features: null, Archived: null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedSpace = CreateSampleSpace(spaceId, "Update CT Pass");

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateSpaceRequest, Space>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(expectedSpace);

            // Act
            await _spacesService.UpdateSpaceAsync(spaceId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync<UpdateSpaceRequest, Space>(
                $"space/{spaceId}",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for DeleteSpaceAsync ---

        [Fact]
        public async Task DeleteSpaceAsync_ValidSpaceId_CallsDeleteAsync()
        {
            // Arrange
            var spaceId = "space123_delete";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"space/{spaceId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _spacesService.DeleteSpaceAsync(spaceId);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"space/{spaceId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteSpaceAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_delete_http_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _spacesService.DeleteSpaceAsync(spaceId)
            );
        }

        [Fact]
        public async Task DeleteSpaceAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_delete_cancel_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _spacesService.DeleteSpaceAsync(spaceId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task DeleteSpaceAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var spaceId = "space_delete_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _spacesService.DeleteSpaceAsync(spaceId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"space/{spaceId}",
                expectedToken), Times.Once);
        }
    }
}
