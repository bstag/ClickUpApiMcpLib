using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Users; // For User
using ClickUp.Api.Client.Models.RequestModels.Users; // For EditUserOnWorkspaceRequest
using ClickUp.Api.Client.Models.ResponseModels.Users; // For GetUserResponse and its inner classes
using ClickUp.Api.Client.Models.ResponseModels.Guests; // Added for InviteGuestToWorkspaceResponseUser
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class UsersServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly UsersService _usersService;
        private readonly Mock<ILogger<UsersService>> _mockLogger;

        public UsersServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<UsersService>>();
            _usersService = new UsersService(_mockApiConnection.Object, _mockLogger.Object);
        }

        // Helper to create a User object similar to what GetUserResponse.Member.User might represent
        // Helper to create a sample InviteGuestToWorkspaceResponseUser which is nested in GetUserResponse
        private InviteGuestToWorkspaceResponseUser CreateSampleActualUserDto(int id = 1, string username = "Test User")
        {
            return new InviteGuestToWorkspaceResponseUser(
                Id: id,
                Username: username,
                Email: $"{username.Replace(" ", "")}@example.com",
                Color: "#AABBCC",
                Initials: username.Substring(0, Math.Min(username.Length, 2)).ToUpper(),
                ProfilePicture: "http://example.com/pic.jpg",
                Role: 2,
                CustomRole: null,
                LastActive: null, // These are often not present or not relevant for all user types
                DateJoined: null,
                DateInvited: DateTimeOffset.UtcNow
            );
        }

        private GetUserResponseMember CreateSampleGetUserResponseMember(int userId = 1, string username = "Test User")
        {
            // InvitedBy should be ClickUp.Api.Client.Models.ResponseModels.Guests.InvitedByUserInfo
            var invitedBy = new ClickUp.Api.Client.Models.ResponseModels.Guests.InvitedByUserInfo(
                Id: 99,
                Color: "#FFF",
                Username: "Inviter",
                Email: "inviter@example.com",
                Initials: "IV",
                ProfilePicture: null
            );
            // GuestSharingDetails requires Tasks, Lists, Folders in its constructor
            var sharedDetails = new ClickUp.Api.Client.Models.ResponseModels.Guests.GuestSharingDetails(
                Tasks: new List<string>(),
                Lists: new List<string>(),
                Folders: new List<string>()
            );
            return new GetUserResponseMember(User: CreateSampleActualUserDto(userId, username), InvitedBy: invitedBy, Shared: sharedDetails);
        }


        // --- Tests for GetUserFromWorkspaceAsync ---

        [Fact]
        public async Task GetUserFromWorkspaceAsync_ValidRequest_ReturnsUser()
        {
            // Arrange
            var workspaceId = "ws123";
            var userId = "user1"; // This string userId is used in the URL
            var actualUserDto = CreateSampleActualUserDto(1, "Workspace User"); // This creates the User DTO part
            var responseMember = CreateSampleGetUserResponseMember(1, "Workspace User"); // This creates the Member part containing the User DTO
            var apiResponse = new GetUserResponse(responseMember);


            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserResponse>(
                    $"team/{workspaceId}/user/{userId}", // Default URL
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _usersService.GetUserFromWorkspaceAsync(workspaceId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(actualUserDto.Id, result.Id); // Service maps from the DTO to the User entity
            Assert.Equal(actualUserDto.Username, result.Username);
            _mockApiConnection.Verify(x => x.GetAsync<GetUserResponse>(
                $"team/{workspaceId}/user/{userId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // The includeShared parameter is noted as not used in the service implementation,
        // so no specific URL change test for it is needed unless the service logic changes.

        [Fact]
        public async Task GetUserFromWorkspaceAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_get_user_null_api";
            var userId = "user_null_api";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserResponse)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _usersService.GetUserFromWorkspaceAsync(workspaceId, userId)
            );
        }

        [Fact]
        public async Task GetUserFromWorkspaceAsync_ApiReturnsResponseWithNullMember_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_get_user_null_member";
            var userId = "user_null_member";
            var apiResponse = new GetUserResponse(null!); // Member property is null
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _usersService.GetUserFromWorkspaceAsync(workspaceId, userId)
            );
        }

        [Fact]
        public async Task GetUserFromWorkspaceAsync_ApiReturnsResponseWithNullUserInMember_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_get_user_null_user_data";
            var userId = "user_null_user_data";
            // Create a GetUserResponseMember with a null User, but valid (though minimal) InvitedBy and Shared
            var invitedByMinimal = new ClickUp.Api.Client.Models.ResponseModels.Guests.InvitedByUserInfo(0, null, string.Empty, string.Empty, null, null);
            var sharedDetailsMinimal = new ClickUp.Api.Client.Models.ResponseModels.Guests.GuestSharingDetails(new List<string>(), new List<string>(), new List<string>());
            var memberWithNullUser = new GetUserResponseMember(User: null!, InvitedBy: invitedByMinimal, Shared: sharedDetailsMinimal);
            var apiResponse = new GetUserResponse(memberWithNullUser);
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _usersService.GetUserFromWorkspaceAsync(workspaceId, userId)
            );
        }

        [Fact]
        public async Task GetUserFromWorkspaceAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_get_user_http_ex";
            var userId = "user_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _usersService.GetUserFromWorkspaceAsync(workspaceId, userId)
            );
        }

        [Fact]
        public async Task GetUserFromWorkspaceAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_get_user_cancel_ex";
            var userId = "user_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _usersService.GetUserFromWorkspaceAsync(workspaceId, userId, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetUserFromWorkspaceAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_get_user_ct_pass";
            var userId = "user_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new GetUserResponse(CreateSampleGetUserResponseMember(1, "CT User"));


            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserResponse>(
                    It.IsAny<string>(),
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _usersService.GetUserFromWorkspaceAsync(workspaceId, userId, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetUserResponse>(
                $"team/{workspaceId}/user/{userId}",
                expectedToken), Times.Once);
        }

        // --- Tests for EditUserOnWorkspaceAsync ---

        [Fact]
        public async Task EditUserOnWorkspaceAsync_ValidRequest_ReturnsUpdatedUser()
        {
            // Arrange
            var workspaceId = "ws_edit_user";
            var userId = "user_to_edit";
            var request = new EditUserOnWorkspaceRequest(
                Username: "UpdatedUsername",
                Admin: true,
                CustomRoleId: 5 // Corrected from CustomRole to CustomRoleId
            );
            var updatedUserDto = CreateSampleActualUserDto(int.Parse(userId.Replace("user_","")), "UpdatedUsername"); // Assuming ID is part of userId string
            var responseMember = CreateSampleGetUserResponseMember(int.Parse(userId.Replace("user_","")), "UpdatedUsername");
            var apiResponse = new GetUserResponse(responseMember);


            _mockApiConnection
                .Setup(x => x.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(
                    $"team/{workspaceId}/user/{userId}",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _usersService.EditUserOnWorkspaceAsync(workspaceId, userId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedUserDto.Username, result.Username);
            // Further assertions could be made if the service mapped IsAdmin or CustomRole back to the User model,
            // but current User model doesn't have these. The test verifies the User object is returned.
            _mockApiConnection.Verify(x => x.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(
                $"team/{workspaceId}/user/{userId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditUserOnWorkspaceAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_edit_user_null_api";
            var userId = "user_edit_null_api";
            var request = new EditUserOnWorkspaceRequest("NewName", false, 0); // CustomRole changed from null to 0
            _mockApiConnection
                .Setup(x => x.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserResponse)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _usersService.EditUserOnWorkspaceAsync(workspaceId, userId, request)
            );
        }

        [Fact]
        public async Task EditUserOnWorkspaceAsync_ApiReturnsResponseWithNullMember_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_edit_user_null_member";
            var userId = "user_edit_null_member";
            var request = new EditUserOnWorkspaceRequest("NewName", false, 0); // CustomRole changed from null to 0
            var apiResponse = new GetUserResponse(null!);
            _mockApiConnection
                .Setup(x => x.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _usersService.EditUserOnWorkspaceAsync(workspaceId, userId, request)
            );
        }

        [Fact]
        public async Task EditUserOnWorkspaceAsync_ApiReturnsResponseWithNullUserInMember_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_edit_user_null_user_data";
            var userId = "user_edit_null_user_data";
            var request = new EditUserOnWorkspaceRequest("NewName", false, 0); // CustomRole changed from null to 0
            // Create a GetUserResponseMember with a null User, but valid (though minimal) InvitedBy and Shared
            var invitedByMinimal = new ClickUp.Api.Client.Models.ResponseModels.Guests.InvitedByUserInfo(0, null, string.Empty, string.Empty, null, null);
            var sharedDetailsMinimal = new ClickUp.Api.Client.Models.ResponseModels.Guests.GuestSharingDetails(new List<string>(), new List<string>(), new List<string>());
            var memberWithNullUser = new GetUserResponseMember(User: null!, InvitedBy: invitedByMinimal, Shared: sharedDetailsMinimal);
            var apiResponse = new GetUserResponse(memberWithNullUser);
            _mockApiConnection
                .Setup(x => x.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _usersService.EditUserOnWorkspaceAsync(workspaceId, userId, request)
            );
        }

        [Fact]
        public async Task EditUserOnWorkspaceAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_edit_user_http_ex";
            var userId = "user_edit_http_ex";
            var request = new EditUserOnWorkspaceRequest("NewName", false, 0); // CustomRole changed from null to 0
            _mockApiConnection
                .Setup(x => x.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _usersService.EditUserOnWorkspaceAsync(workspaceId, userId, request)
            );
        }

        [Fact]
        public async Task EditUserOnWorkspaceAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_edit_user_cancel_ex";
            var userId = "user_edit_cancel_ex";
            var request = new EditUserOnWorkspaceRequest("NewName", false, 0); // CustomRole changed from null to 0
            _mockApiConnection
                .Setup(x => x.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _usersService.EditUserOnWorkspaceAsync(workspaceId, userId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task EditUserOnWorkspaceAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_edit_user_ct_pass";
            var userId = "user_edit_ct_pass";
            var request = new EditUserOnWorkspaceRequest("NewName", false, 0); // CustomRole changed from null to 0
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new GetUserResponse(CreateSampleGetUserResponseMember(1, "CT Updated User"));

            _mockApiConnection
                .Setup(x => x.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _usersService.EditUserOnWorkspaceAsync(workspaceId, userId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(
                $"team/{workspaceId}/user/{userId}",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for RemoveUserFromWorkspaceAsync ---

        [Fact]
        public async Task RemoveUserFromWorkspaceAsync_ValidRequest_CallsDeleteAsync()
        {
            // Arrange
            var workspaceId = "ws_remove_user";
            var userId = "user_to_remove";

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"team/{workspaceId}/user/{userId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _usersService.RemoveUserFromWorkspaceAsync(workspaceId, userId);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"team/{workspaceId}/user/{userId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromWorkspaceAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_remove_user_http_ex";
            var userId = "user_remove_http_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _usersService.RemoveUserFromWorkspaceAsync(workspaceId, userId)
            );
        }

        [Fact]
        public async Task RemoveUserFromWorkspaceAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_remove_user_cancel_ex";
            var userId = "user_remove_cancel_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _usersService.RemoveUserFromWorkspaceAsync(workspaceId, userId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task RemoveUserFromWorkspaceAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_remove_user_ct_pass";
            var userId = "user_remove_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _usersService.RemoveUserFromWorkspaceAsync(workspaceId, userId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"team/{workspaceId}/user/{userId}",
                expectedToken), Times.Once);
        }
    }
}
