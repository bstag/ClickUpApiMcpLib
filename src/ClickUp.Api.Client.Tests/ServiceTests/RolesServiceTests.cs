using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
// Remove ambiguous using: using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.ResponseModels.Roles; // This namespace contains the CustomRole class used by the service
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class RolesServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly RolesService _rolesService;
        private readonly Mock<ILogger<RolesService>> _mockLogger;

        public RolesServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<RolesService>>();
            _rolesService = new RolesService(_mockApiConnection.Object, _mockLogger.Object);
        }

        // This helper now creates ClickUp.Api.Client.Models.ResponseModels.Roles.CustomRole
        private CustomRole CreateSampleCustomRole(int id = 1, string name = "Admin", int? inheritedRole = 0)
        {
            // Using the constructor: CustomRole(int id, string name, DateTimeOffset dateCreated)
            // Or public setters if more convenient for optional properties.
            return new CustomRole(id, name, DateTimeOffset.UtcNow)
            {
                InheritedRole = inheritedRole, // Nullable int
                Members = new List<long> { 100L, 200L } // Nullable list of longs
            };
            // Note: The ResponseModels.Roles.CustomRole does not have TeamId.
            // If TeamId were conceptually part of it (e.g. from URL context), it's not in this DTO.
        }

        // --- Tests for GetCustomRolesAsync ---

        [Fact]
        public async Task GetCustomRolesAsync_ValidRequest_ReturnsRoles()
        {
            // Arrange
            var workspaceId = "ws_test";
            // expectedRoles should be a list of ResponseModels.Roles.CustomRole
            var expectedRoles = new List<CustomRole>
            {
                CreateSampleCustomRole(1, "Role One"),
                CreateSampleCustomRole(2, "Role Two")
            };
            var apiResponse = new GetCustomRolesResponse(expectedRoles);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetCustomRolesResponse>(
                    $"team/{workspaceId}/customroles",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            // The result will be IEnumerable<ResponseModels.Roles.CustomRole>
            var result = await _rolesService.GetCustomRolesAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Role One", result.First().Name);
            _mockApiConnection.Verify(x => x.GetAsync<GetCustomRolesResponse>(
                $"team/{workspaceId}/customroles",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(true, "team/ws_params/customroles?include_members=true")]
        [InlineData(false, "team/ws_params/customroles?include_members=false")]
        public async Task GetCustomRolesAsync_WithIncludeMembers_ConstructsCorrectUrl(bool includeMembers, string expectedUrl)
        {
            // Arrange
            var workspaceId = "ws_params";
            var apiResponse = new GetCustomRolesResponse(new List<CustomRole> { CreateSampleCustomRole() });

            _mockApiConnection
                .Setup(x => x.GetAsync<GetCustomRolesResponse>(
                    expectedUrl,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _rolesService.GetCustomRolesAsync(workspaceId, includeMembers);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetCustomRolesResponse>(
                expectedUrl,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCustomRolesAsync_IncludeMembersNull_ConstructsCorrectUrl()
        {
            // Arrange
            var workspaceId = "ws_no_params";
            var expectedUrl = $"team/{workspaceId}/customroles";
            var apiResponse = new GetCustomRolesResponse(new List<CustomRole> { CreateSampleCustomRole() });

            _mockApiConnection
                .Setup(x => x.GetAsync<GetCustomRolesResponse>(
                    expectedUrl,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _rolesService.GetCustomRolesAsync(workspaceId, null);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetCustomRolesResponse>(
                expectedUrl,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCustomRolesAsync_DefaultIncludeMembers_ConstructsCorrectUrl()
        {
            // Arrange
            var workspaceId = "ws_default_params";
            var expectedUrl = $"team/{workspaceId}/customroles";
            var apiResponse = new GetCustomRolesResponse(new List<CustomRole> { CreateSampleCustomRole() });

            _mockApiConnection
                .Setup(x => x.GetAsync<GetCustomRolesResponse>(
                    expectedUrl,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _rolesService.GetCustomRolesAsync(workspaceId);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetCustomRolesResponse>(
                expectedUrl,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCustomRolesAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetCustomRolesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetCustomRolesResponse)null);

            // Act
            var result = await _rolesService.GetCustomRolesAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCustomRolesAsync_ApiReturnsResponseWithNullRolesList_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_null_roles_list";
            // GetCustomRolesResponse constructor initializes Roles to an empty list if null is passed.
            // If Roles property is set to null directly after construction, it will be null.
            // The service handles this by returning Enumerable.Empty<CustomRole>() if response.Roles is null.
            var apiResponse = new GetCustomRolesResponse { Roles = null! };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetCustomRolesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _rolesService.GetCustomRolesAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Service returns empty if response.Roles is null
        }

        [Fact]
        public async Task GetCustomRolesAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetCustomRolesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _rolesService.GetCustomRolesAsync(workspaceId)
            );
        }

        [Fact]
        public async Task GetCustomRolesAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_cancel_ex";
            var cts = new CancellationTokenSource();
            _mockApiConnection
                .Setup(x => x.GetAsync<GetCustomRolesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _rolesService.GetCustomRolesAsync(workspaceId, cancellationToken: cts.Token)
            );
        }

        [Fact]
        public async Task GetCustomRolesAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new GetCustomRolesResponse(new List<CustomRole>());


            _mockApiConnection
                .Setup(x => x.GetAsync<GetCustomRolesResponse>(
                    It.IsAny<string>(),
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _rolesService.GetCustomRolesAsync(workspaceId, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetCustomRolesResponse>(
                $"team/{workspaceId}/customroles",
                expectedToken), Times.Once);
        }
    }
}
