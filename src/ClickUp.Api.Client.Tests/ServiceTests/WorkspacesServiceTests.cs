using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces; // For response DTOs
using ClickUp.Api.Client.Models.Entities.Users; // For User if nested in response
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class WorkspacesServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly WorkspacesService _workspacesService;
        private readonly Mock<ILogger<WorkspacesService>> _mockLogger;

        public WorkspacesServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<WorkspacesService>>();
            _workspacesService = new WorkspacesService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private User CreateSampleUserForWorkspace(int id = 1, string username = "Workspace User")
        {
            return new User(id, username, $"{username.Replace(" ", "")}@example.com", "#ABC", null, "WU");
        }

        // Removed CreateSampleWorkspaceMember as it's not directly used in GetWorkspaceSeatsResponse like initially assumed.
        // The response contains counts, not lists of members.

        // --- Tests for GetWorkspaceSeatsAsync ---

        // --- Tests for GetAuthorizedWorkspacesAsync ---
        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_ValidRequest_ReturnsResponse()
        {
            // Arrange
            var expectedWorkspaces = new List<ClickUp.Api.Client.Models.Entities.WorkSpaces.ClickUpWorkspace>
            {
                new ClickUp.Api.Client.Models.Entities.WorkSpaces.ClickUpWorkspace { Id = "ws1", Name = "Workspace 1", Color = "#FF0000", Members = new List<ClickUp.Api.Client.Models.Common.Member>() }
            };
            var apiResponse = new ClickUp.Api.Client.Models.ResponseModels.Authorization.GetAuthorizedWorkspacesResponse { Workspaces = expectedWorkspaces };
            _mockApiConnection
                .Setup(x => x.GetAsync<ClickUp.Api.Client.Models.ResponseModels.Authorization.GetAuthorizedWorkspacesResponse>(
                    "team", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _workspacesService.GetAuthorizedWorkspacesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Workspaces);
            Assert.Single(result.Workspaces);
            Assert.Equal("ws1", result.Workspaces.First().Id);
            _mockApiConnection.Verify(x => x.GetAsync<ClickUp.Api.Client.Models.ResponseModels.Authorization.GetAuthorizedWorkspacesResponse>(
                "team", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new ClickUp.Api.Client.Models.ResponseModels.Authorization.GetAuthorizedWorkspacesResponse { Workspaces = new List<ClickUp.Api.Client.Models.Entities.WorkSpaces.ClickUpWorkspace>() };

            _mockApiConnection.Setup(c => c.GetAsync<ClickUp.Api.Client.Models.ResponseModels.Authorization.GetAuthorizedWorkspacesResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _workspacesService.GetAuthorizedWorkspacesAsync(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            _mockApiConnection
                .Setup(x => x.GetAsync<ClickUp.Api.Client.Models.ResponseModels.Authorization.GetAuthorizedWorkspacesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _workspacesService.GetAuthorizedWorkspacesAsync(new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_PassesCancellationTokenToApiConnection()
        {
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection.Setup(x => x.GetAsync<ClickUp.Api.Client.Models.ResponseModels.Authorization.GetAuthorizedWorkspacesResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new ClickUp.Api.Client.Models.ResponseModels.Authorization.GetAuthorizedWorkspacesResponse { Workspaces = new List<ClickUp.Api.Client.Models.Entities.WorkSpaces.ClickUpWorkspace>() });
            await _workspacesService.GetAuthorizedWorkspacesAsync(expectedToken);
            _mockApiConnection.Verify(x => x.GetAsync<ClickUp.Api.Client.Models.ResponseModels.Authorization.GetAuthorizedWorkspacesResponse>("team", expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetWorkspaceSeatsAsync_ValidWorkspaceId_ReturnsSeatsResponse()
        {
            // Arrange
            var workspaceId = "ws123_seats";
            var memberSeatsInfo = new WorkspaceMemberSeatsInfoResponse(
                FilledMembersSeats: 1,
                TotalMemberSeats: 5,
                EmptyMemberSeats: 4
            );
            var guestSeatsInfo = new WorkspaceGuestSeatsInfoResponse(
                FilledGuestSeats: 1,
                TotalGuestSeats: 10,
                EmptyGuestSeats: 9
            );
            var apiResponse = new GetWorkspaceSeatsResponse(memberSeatsInfo, guestSeatsInfo);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetWorkspaceSeatsResponse>(
                    $"team/{workspaceId}/seats",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _workspacesService.GetWorkspaceSeatsAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Members); // This is WorkspaceMemberSeatsInfo
            Assert.NotNull(result.Guests);  // This is WorkspaceGuestSeatsInfo
            Assert.Equal(1, result.Members.FilledMembersSeats);
            Assert.Equal(5, result.Members.TotalMemberSeats);
            Assert.Equal(1, result.Guests.FilledGuestSeats);
            Assert.Equal(10, result.Guests.TotalGuestSeats);
            _mockApiConnection.Verify(x => x.GetAsync<GetWorkspaceSeatsResponse>(
                $"team/{workspaceId}/seats",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetWorkspaceSeatsAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_seats_null_api";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetWorkspaceSeatsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetWorkspaceSeatsResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _workspacesService.GetWorkspaceSeatsAsync(workspaceId)
            );
        }

        // Test for null Seats property in GetWorkspaceSeatsResponse is not directly applicable
        // if the constructor of GetWorkspaceSeatsResponse requires a non-null Seats object.
        // The service's null check is on the entire response object.

        [Fact]
        public async Task GetWorkspaceSeatsAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_seats_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetWorkspaceSeatsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _workspacesService.GetWorkspaceSeatsAsync(workspaceId)
            );
        }

        [Fact]
        public async Task GetWorkspaceSeatsAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var workspaceId = "ws_seats_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyMemberSeats = new WorkspaceMemberSeatsInfoResponse(0,0,0);
            var dummyGuestSeats = new WorkspaceGuestSeatsInfoResponse(0,0,0);
            var dummyResponse = new GetWorkspaceSeatsResponse(dummyMemberSeats, dummyGuestSeats);

            _mockApiConnection.Setup(c => c.GetAsync<GetWorkspaceSeatsResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _workspacesService.GetWorkspaceSeatsAsync(workspaceId, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetWorkspaceSeatsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_seats_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetWorkspaceSeatsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _workspacesService.GetWorkspaceSeatsAsync(workspaceId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetWorkspaceSeatsAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_seats_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var memberSeatsInfo = new WorkspaceMemberSeatsInfoResponse(0,0,0);
            var guestSeatsInfo = new WorkspaceGuestSeatsInfoResponse(0,0,0);
            var apiResponse = new GetWorkspaceSeatsResponse(memberSeatsInfo, guestSeatsInfo);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetWorkspaceSeatsResponse>(
                    It.IsAny<string>(),
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _workspacesService.GetWorkspaceSeatsAsync(workspaceId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetWorkspaceSeatsResponse>(
                $"team/{workspaceId}/seats",
                expectedToken), Times.Once);
        }

        // --- Tests for GetWorkspacePlanAsync ---

        [Fact]
        public async Task GetWorkspacePlanAsync_ValidWorkspaceId_ReturnsPlanResponse()
        {
            // Arrange
            var workspaceId = "ws123_plan";
            // Corrected: GetWorkspacePlanResponse takes PlanName and PlanId directly.
            var apiResponse = new GetWorkspacePlanResponse(
                PlanName: "Enterprise",
                PlanId: 12345 // Example Plan ID
            );

            _mockApiConnection
                .Setup(x => x.GetAsync<GetWorkspacePlanResponse>(
                    $"team/{workspaceId}/plan",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _workspacesService.GetWorkspacePlanAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Enterprise", result.PlanName);
            Assert.Equal(12345, result.PlanId);
            _mockApiConnection.Verify(x => x.GetAsync<GetWorkspacePlanResponse>(
                $"team/{workspaceId}/plan",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetWorkspacePlanAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_plan_null_api";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetWorkspacePlanResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetWorkspacePlanResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _workspacesService.GetWorkspacePlanAsync(workspaceId)
            );
        }

        [Fact]
        public async Task GetWorkspacePlanAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_plan_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetWorkspacePlanResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _workspacesService.GetWorkspacePlanAsync(workspaceId)
            );
        }

        [Fact]
        public async Task GetWorkspacePlanAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var workspaceId = "ws_plan_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetWorkspacePlanResponse("Dummy Plan", 0);

            _mockApiConnection.Setup(c => c.GetAsync<GetWorkspacePlanResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _workspacesService.GetWorkspacePlanAsync(workspaceId, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetWorkspacePlanAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_plan_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetWorkspacePlanResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _workspacesService.GetWorkspacePlanAsync(workspaceId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetWorkspacePlanAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_plan_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new GetWorkspacePlanResponse("Test Plan CT", 67890);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetWorkspacePlanResponse>(
                    It.IsAny<string>(),
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _workspacesService.GetWorkspacePlanAsync(workspaceId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetWorkspacePlanResponse>(
                $"team/{workspaceId}/plan",
                expectedToken), Times.Once);
        }
    }
}
