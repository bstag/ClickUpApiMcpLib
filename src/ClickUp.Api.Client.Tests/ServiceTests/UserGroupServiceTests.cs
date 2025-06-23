using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Common; // Added for Member
using ClickUp.Api.Client.Models.Entities.UserGroups; // For UserGroup
using ClickUp.Api.Client.Models.Entities.Users; // For User (if part of UserGroup)
using ClickUp.Api.Client.Models.RequestModels.UserGroups; // For request DTOs
using ClickUp.Api.Client.Models.ResponseModels.UserGroups; // For GetUserGroupsResponse
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class UserGroupServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly UserGroupsService _userGroupsService;
        private readonly Mock<ILogger<UserGroupsService>> _mockLogger;

        public UserGroupServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<UserGroupsService>>();
            _userGroupsService = new UserGroupsService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private User CreateSampleUser(long id = 1, string username = "Group Member", string emailSuffix = "@example.com", string color = "#456", string profilePicture = null, string initials = "GM")
        {
            return new User((int)id, username, $"{username.Replace(" ", "")}{emailSuffix}", color, profilePicture, initials);
        }

        private Member CreateSampleMember(long userId = 1, string username = "Group Member", string role = null, string permissionLevel = null)
        {
            return new Member(User: CreateSampleUser(userId, username), Role: role, PermissionLevel: permissionLevel);
        }

        private UserGroup CreateSampleUserGroup(string id = "ug_1", string name = "Developers", int creatorUserId = 99)
        {
            return new UserGroup(
                Id: id,
                TeamId: "ws_abc", // Example workspace ID
                UserId: creatorUserId, // Added UserId
                Name: name,
                Handle: name.ToLower().Replace(" ", "_"),
                DateCreated: DateTimeOffset.UtcNow.AddDays(-10).ToUnixTimeMilliseconds().ToString(),
                Initials: name.Substring(0, Math.Min(name.Length, 3)).ToUpper(),
                Members: new List<Member> { CreateSampleMember(1, "Member One"), CreateSampleMember(2, "Member Two") }, // Changed to List<Member>
                Avatar: null
            );
        }

        // --- Tests for GetUserGroupsAsync ---

        [Fact]
        public async Task GetUserGroupsAsync_ValidWorkspaceId_ReturnsUserGroups()
        {
            // Arrange
            var workspaceId = "ws123";
            var expectedGroups = new List<UserGroup> { CreateSampleUserGroup("ug1", "Group Alpha") };
            var apiResponse = new GetUserGroupsResponse(expectedGroups);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserGroupsResponse>(
                    $"team/{workspaceId}/group", // Default URL, no group_ids
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _userGroupsService.GetUserGroupsAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("ug1", result.First().Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetUserGroupsResponse>(
                $"team/{workspaceId}/group",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUserGroupsAsync_WithGroupIds_BuildsCorrectUrl()
        {
            // Arrange
            var workspaceId = "ws_with_ids";
            var groupIds = new List<string> { "ug_abc", "ug_xyz" };
            var apiResponse = new GetUserGroupsResponse(new List<UserGroup>());

            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserGroupsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _userGroupsService.GetUserGroupsAsync(workspaceId, groupIds);

            // Assert
            var expectedUrl = $"team/{workspaceId}/group?group_ids={Uri.EscapeDataString(string.Join(",", groupIds))}";
            _mockApiConnection.Verify(x => x.GetAsync<GetUserGroupsResponse>(
                expectedUrl,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUserGroupsAsync_WithNullGroupIds_BuildsCorrectUrlWithoutGroupIdsParam()
        {
            // Arrange
            var workspaceId = "ws_null_ids";
            var apiResponse = new GetUserGroupsResponse(new List<UserGroup>());
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserGroupsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _userGroupsService.GetUserGroupsAsync(workspaceId, groupIds: null);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetUserGroupsResponse>(
                $"team/{workspaceId}/group", // Should not contain group_ids query parameter
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUserGroupsAsync_WithEmptyGroupIds_BuildsCorrectUrlWithoutGroupIdsParam()
        {
            // Arrange
            var workspaceId = "ws_empty_ids";
            var groupIds = new List<string>();
             var apiResponse = new GetUserGroupsResponse(new List<UserGroup>());
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserGroupsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _userGroupsService.GetUserGroupsAsync(workspaceId, groupIds);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetUserGroupsResponse>(
                $"team/{workspaceId}/group", // Should not contain group_ids query parameter
                It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task GetUserGroupsAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_ug_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserGroupsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserGroupsResponse)null);

            // Act
            var result = await _userGroupsService.GetUserGroupsAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserGroupsAsync_ApiReturnsResponseWithNullGroups_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_ug_null_groups_in_resp";
            var apiResponse = new GetUserGroupsResponse(null!); // Groups property is null
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserGroupsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _userGroupsService.GetUserGroupsAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserGroupsAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_ug_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserGroupsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _userGroupsService.GetUserGroupsAsync(workspaceId)
            );
        }

        [Fact]
        public async Task GetUserGroupsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_ug_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserGroupsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _userGroupsService.GetUserGroupsAsync(workspaceId, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetUserGroupsAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_ug_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetUserGroupsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetUserGroupsResponse(new List<UserGroup>()));

            // Act
            await _userGroupsService.GetUserGroupsAsync(workspaceId, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetUserGroupsResponse>(
                $"team/{workspaceId}/group",
                expectedToken), Times.Once);
        }

        // --- Tests for CreateUserGroupAsync ---

        [Fact]
        public async Task CreateUserGroupAsync_ValidRequest_ReturnsUserGroup()
        {
            // Arrange
            var workspaceId = "ws_create_ug";
            var request = new CreateUserGroupRequest(
                Name: "New User Group",
                Handle: "new_user_group_handle", // Added optional handle
                Members: new List<int> { 1, 2 } // Changed MemberIds to Members, type to List<int>
            );
            var expectedGroup = CreateSampleUserGroup("ug_new", "New User Group");

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateUserGroupRequest, UserGroup>(
                    $"team/{workspaceId}/group",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedGroup);

            // Act
            var result = await _userGroupsService.CreateUserGroupAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedGroup.Id, result.Id);
            Assert.Equal(expectedGroup.Name, result.Name);
            _mockApiConnection.Verify(x => x.PostAsync<CreateUserGroupRequest, UserGroup>(
                $"team/{workspaceId}/group",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserGroupAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_create_ug_null_api";
            // Removed workspaceId from DTO, ensuring Members is List<int>
            var request = new CreateUserGroupRequest("Null UG", "null_ug_handle", new List<int>());
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateUserGroupRequest, UserGroup>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserGroup)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userGroupsService.CreateUserGroupAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task CreateUserGroupAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_create_ug_http_ex";
            var request = new CreateUserGroupRequest("HTTP UG", "http_ug_handle", new List<int>());
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateUserGroupRequest, UserGroup>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _userGroupsService.CreateUserGroupAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task CreateUserGroupAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_create_ug_cancel_ex";
            var request = new CreateUserGroupRequest("Cancel UG", "cancel_ug_handle", new List<int>());
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateUserGroupRequest, UserGroup>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _userGroupsService.CreateUserGroupAsync(workspaceId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task CreateUserGroupAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_create_ug_ct_pass";
            var request = new CreateUserGroupRequest("CT UG", "ct_ug_handle", new List<int>());
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedGroup = CreateSampleUserGroup("ug_ct_new", "CT UG");

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateUserGroupRequest, UserGroup>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(expectedGroup);

            // Act
            await _userGroupsService.CreateUserGroupAsync(workspaceId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateUserGroupRequest, UserGroup>(
                $"team/{workspaceId}/group",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for UpdateUserGroupAsync ---

        [Fact]
        public async Task UpdateUserGroupAsync_ValidRequest_ReturnsUpdatedUserGroup()
        {
            // Arrange
            var groupId = "ug_update_1";
            var membersUpdate = new UserGroupMembersUpdate(Add: new List<int> { 3, 4 }, Rem: null);
            var request = new UpdateUserGroupRequest(
                Name: "Updated Group Name",
                Handle: "updated_handle",
                Members: membersUpdate
            );
            var expectedGroup = CreateSampleUserGroup(groupId, "Updated Group Name", creatorUserId: 123); // Assuming a creator ID
            // Simulate the update in the expected object for assertion
            expectedGroup = expectedGroup with
            {
                Handle = "updated_handle",
                // Assuming the API returns the full member list after update,
                // and that members with ID 3 and 4 are now part of the group.
                // For simplicity, we're creating new Member objects.
                // In a real scenario, you might need more sophisticated logic if the API only returns changed fields.
                Members = new List<Member> { CreateSampleMember(3, "User Three"), CreateSampleMember(4, "User Four") }
            };

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateUserGroupRequest, UserGroup>(
                    $"group/{groupId}",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedGroup);

            // Act
            var result = await _userGroupsService.UpdateUserGroupAsync(groupId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedGroup.Name, result.Name);
            Assert.Equal("updated_handle", result.Handle);
            _mockApiConnection.Verify(x => x.PutAsync<UpdateUserGroupRequest, UserGroup>(
                $"group/{groupId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserGroupAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var groupId = "ug_update_null_api";
            var membersUpdate = new UserGroupMembersUpdate(Add: new List<int> { 5 }, Rem: null);
            var request = new UpdateUserGroupRequest("Update Null", "upd_null", membersUpdate);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateUserGroupRequest, UserGroup>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserGroup)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userGroupsService.UpdateUserGroupAsync(groupId, request)
            );
        }

        [Fact]
        public async Task UpdateUserGroupAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var groupId = "ug_update_http_ex";
            var membersUpdate = new UserGroupMembersUpdate(Add: new List<int> { 6 }, Rem: null);
            var request = new UpdateUserGroupRequest("Update HTTP", "upd_http", membersUpdate);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateUserGroupRequest, UserGroup>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _userGroupsService.UpdateUserGroupAsync(groupId, request)
            );
        }

        [Fact]
        public async Task UpdateUserGroupAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var groupId = "ug_update_cancel_ex";
            var membersUpdate = new UserGroupMembersUpdate(Add: new List<int> { 7 }, Rem: null);
            var request = new UpdateUserGroupRequest("Update Cancel", "upd_cancel", membersUpdate);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateUserGroupRequest, UserGroup>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _userGroupsService.UpdateUserGroupAsync(groupId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task UpdateUserGroupAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var groupId = "ug_update_ct_pass";
            var membersUpdate = new UserGroupMembersUpdate(Add: new List<int> { 8 }, Rem: null);
            var request = new UpdateUserGroupRequest("Update CT", "upd_ct", membersUpdate);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedGroup = CreateSampleUserGroup(groupId, "Update CT");


            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateUserGroupRequest, UserGroup>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(expectedGroup);

            // Act
            await _userGroupsService.UpdateUserGroupAsync(groupId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync<UpdateUserGroupRequest, UserGroup>(
                $"group/{groupId}",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for DeleteUserGroupAsync ---

        [Fact]
        public async Task DeleteUserGroupAsync_ValidGroupId_CallsDeleteAsync()
        {
            // Arrange
            var groupId = "ug_delete_1";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"group/{groupId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userGroupsService.DeleteUserGroupAsync(groupId);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"group/{groupId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUserGroupAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var groupId = "ug_delete_http_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _userGroupsService.DeleteUserGroupAsync(groupId)
            );
        }

        [Fact]
        public async Task DeleteUserGroupAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var groupId = "ug_delete_cancel_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _userGroupsService.DeleteUserGroupAsync(groupId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task DeleteUserGroupAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var groupId = "ug_delete_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _userGroupsService.DeleteUserGroupAsync(groupId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"group/{groupId}",
                expectedToken), Times.Once);
        }
    }
}
