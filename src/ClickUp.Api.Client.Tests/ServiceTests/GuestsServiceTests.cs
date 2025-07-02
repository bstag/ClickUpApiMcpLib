using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Guests;
using ClickUp.Api.Client.Models.ResponseModels.Guests;
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class GuestsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly GuestsService _guestsService;
        private readonly Mock<ILogger<GuestsService>> _mockLogger;

        public GuestsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<GuestsService>>();
            _guestsService = new GuestsService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private User CreateSampleUserEntity(int id = 1, string email = "user@example.com")
        {
            return new User(
                Id: id,
                Username: $"user{id}",
                Email: email,
                Color: "#FF0000",
                ProfilePicture: "url",
                Initials: $"U{id}"
            );
        }

        private GuestUserInfo CreateSampleGuestUserInfo(int id = 1, string email = "guest@example.com")
        {
            return new GuestUserInfo {
                Id = id,
                Username = $"guest{id}",
                Email = email,
                Color = "#00FF00",
                Initials = $"G{id}",
                ProfilePicture = null,
                Role = 0,
                DateInvited = DateTimeOffset.UtcNow
            };
        }

        private ClickUp.Api.Client.Models.Entities.Users.InvitedByUserInfo CreateSampleInvitedByUserInfoEntity(int id = 99) =>
            new ClickUp.Api.Client.Models.Entities.Users.InvitedByUserInfo {
                Id = id,
                Username = "Inviter",
                Email = "inviter@example.com",
                Color = "#00FFFF",
                Initials = "IV",
                ProfilePicture = null
            };

        private Guest CreateSampleGuest(int userId = 1, string email = "guest@example.com") => new Guest
        {
            User = CreateSampleGuestUserInfo(userId, email),
            InvitedBy = CreateSampleInvitedByUserInfoEntity(),
            CanSeeTimeSpent = true,
            CanSeeTimeEstimated = true,
            CanEditTags = false,
            CanCreateViews = false,
            Shared = new ClickUp.Api.Client.Models.Entities.Users.GuestSharingDetails()
        };

        [Fact]
        public async Task InviteGuestToWorkspaceAsync_ValidRequest_CallsPostAndReturnsResponse()
        {
            var workspaceId = "ws_test";
            var request = new InviteGuestToWorkspaceRequest("guest@example.com", false, true, true, false, true, 123);
            var sampleInvitedUserDto = new ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseUser(1, "guest1", "guest@example.com", "#00FF00", null, "G1", 0, null, null, null, DateTimeOffset.UtcNow);
            var sampleInviterDto = new ClickUp.Api.Client.Models.ResponseModels.Guests.InvitedByUserInfoResponse(99, "#FFFF00", "AdminInviter", "admin@example.com", "AI", null);
            var teamMember = new ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseTeamMember(sampleInvitedUserDto, sampleInviterDto, true, true, false, false, true);
            var responseTeam = new ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseTeam("id", "Test Workspace", "#123123", null, new List<ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseTeamMember>{ teamMember }, new List<Role>());
            var expectedResponse = new InviteGuestToWorkspaceResponse(responseTeam);
            _mockApiConnection.Setup(c => c.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestToWorkspaceResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
            var result = await _guestsService.InviteGuestToWorkspaceAsync(workspaceId, request);
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Team.Id, result.Team.Id);
            _mockApiConnection.Verify(c => c.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestToWorkspaceResponse>($"team/{workspaceId}/guest", request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetGuestAsync_ValidIds_BuildsCorrectUrlAndReturnsGuest()
        {
            var workspaceId = "ws_test";
            var guestIdString = "123";
            var expectedGuest = CreateSampleGuest(userId: 123);
            var apiResponse = new GetGuestResponse { Guest = expectedGuest };
            _mockApiConnection.Setup(c => c.GetAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);
            var result = await _guestsService.GetGuestAsync(workspaceId, guestIdString);
            Assert.NotNull(result);
            Assert.Equal(expectedGuest.User.Id, result.Guest.User.Id);
            _mockApiConnection.Verify(c => c.GetAsync<GetGuestResponse>($"team/{workspaceId}/guest/{guestIdString}", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetGuestAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_test";
            var guestId = "guest_error";
            _mockApiConnection.Setup(c => c.GetAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API Down"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _guestsService.GetGuestAsync(workspaceId, guestId));
        }

        [Fact]
        public async Task AddGuestToTaskAsync_NullResponseData_ThrowsInvalidOperationException()
        {
            var taskId = "task_1";
            var guestId = "guest_1";
            var request = new AddGuestToItemRequest { PermissionLevel = 4 };
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = null! });
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.AddGuestToTaskAsync(taskId, guestId, request));
        }

        [Fact]
        public async Task InviteGuestToWorkspaceAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_invite_err";
            var request = new InviteGuestToWorkspaceRequest("err@example.com", false, false, false, false, false, 0);
            _mockApiConnection.Setup(c => c.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestToWorkspaceResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API error"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _guestsService.InviteGuestToWorkspaceAsync(workspaceId, request));
        }

        [Fact]
        public async Task InviteGuestToWorkspaceAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_invite_null";
            var request = new InviteGuestToWorkspaceRequest("null@example.com", false, false, false, false, false, 0);
            _mockApiConnection.Setup(c => c.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestToWorkspaceResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync((InviteGuestToWorkspaceResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.InviteGuestToWorkspaceAsync(workspaceId, request));
        }

        [Fact]
        public async Task GetGuestAsync_NullApiResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_get_null_api";
            var guestId = "guest_null_api";
            _mockApiConnection.Setup(c => c.GetAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetGuestResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.GetGuestAsync(workspaceId, guestId));
        }

        [Fact]
        public async Task GetGuestAsync_NullGuestInResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_get_null_guest";
            var guestId = "guest_null_guest";
            _mockApiConnection.Setup(c => c.GetAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = null! });
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.GetGuestAsync(workspaceId, guestId));
        }

        [Fact]
        public async Task EditGuestOnWorkspaceAsync_ValidRequest_CallsPutAndReturnsGuest()
        {
            var workspaceId = "ws_edit";
            var guestId = "guest_edit_1";
            var request = new EditGuestOnWorkspaceRequest(CanEditTags: true, CanSeeTimeEstimated: true, CanSeeTimeSpent: true, CanCreateViews: true, CustomRoleId: 1, CanSeePointsEstimated: true);
            var originalGuest = CreateSampleGuest(userId: 246);
            var expectedGuestResponse = originalGuest with { CanEditTags = true, CanCreateViews = true };
            _mockApiConnection.Setup(c => c.PutAsync<EditGuestOnWorkspaceRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = expectedGuestResponse });
            var result = await _guestsService.EditGuestOnWorkspaceAsync(workspaceId, guestId, request);
            Assert.NotNull(result);
            Assert.Equal(originalGuest.User.Username, result.User.Username);
            Assert.True(result.CanEditTags);
            _mockApiConnection.Verify(c => c.PutAsync<EditGuestOnWorkspaceRequest, GetGuestResponse>($"team/{workspaceId}/guest/{guestId}", request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditGuestOnWorkspaceAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_edit_err";
            var guestId = "guest_edit_err";
            var request = new EditGuestOnWorkspaceRequest(CanEditTags: false, CanSeeTimeEstimated: false, CanSeeTimeSpent: false, CanCreateViews: false, CustomRoleId: 0, CanSeePointsEstimated: false);
            _mockApiConnection.Setup(c => c.PutAsync<EditGuestOnWorkspaceRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API error"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _guestsService.EditGuestOnWorkspaceAsync(workspaceId, guestId, request));
        }

        [Fact]
        public async Task EditGuestOnWorkspaceAsync_NullApiResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_edit_null_api";
            var guestId = "guest_edit_null_api";
            var request = new EditGuestOnWorkspaceRequest(CanEditTags: false, CanSeeTimeEstimated: false, CanSeeTimeSpent: false, CanCreateViews: false, CustomRoleId: 0, CanSeePointsEstimated: false);
            _mockApiConnection.Setup(c => c.PutAsync<EditGuestOnWorkspaceRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync((GetGuestResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.EditGuestOnWorkspaceAsync(workspaceId, guestId, request));
        }

        [Fact]
        public async Task EditGuestOnWorkspaceAsync_NullGuestInResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_edit_null_guest";
            var guestId = "guest_edit_null_guest";
            var request = new EditGuestOnWorkspaceRequest(CanEditTags: false, CanSeeTimeEstimated: false, CanSeeTimeSpent: false, CanCreateViews: false, CustomRoleId: 0, CanSeePointsEstimated: false);
            _mockApiConnection.Setup(c => c.PutAsync<EditGuestOnWorkspaceRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = null! });
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.EditGuestOnWorkspaceAsync(workspaceId, guestId, request));
        }

        [Fact]
        public async Task RemoveGuestFromWorkspaceAsync_ValidIds_CallsDelete()
        {
            var workspaceId = "ws_remove";
            var guestId = "guest_remove_1";
            _mockApiConnection.Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            await _guestsService.RemoveGuestFromWorkspaceAsync(workspaceId, guestId);
            _mockApiConnection.Verify(c => c.DeleteAsync($"team/{workspaceId}/guest/{guestId}", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveGuestFromWorkspaceAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_remove_err";
            var guestId = "guest_remove_err";
            _mockApiConnection.Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API error"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _guestsService.RemoveGuestFromWorkspaceAsync(workspaceId, guestId));
        }

        [Fact]
        public async Task AddGuestToTaskAsync_ValidRequest_CallsPostAndReturnsGuest()
        {
            var taskId = "task_add_guest";
            var guestId = "guest_for_task";
            var request = new AddGuestToItemRequest { PermissionLevel = 1 };
            var expectedGuest = CreateSampleGuest(100);
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });
            var result = await _guestsService.AddGuestToTaskAsync(taskId, guestId, request, true, true, "team_abc");
            Assert.NotNull(result);
            Assert.Equal(expectedGuest.User.Id, result.User.Id);
            _mockApiConnection.Verify(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>($"task/{taskId}/guest/{guestId}?include_shared=true&custom_task_ids=true&team_id=team_abc", request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddGuestToTaskAsync_ApiError_ThrowsHttpRequestException()
        {
            var taskId = "task_add_err";
            var guestId = "guest_add_err";
            var request = new AddGuestToItemRequest { PermissionLevel = 1 };
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API error"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _guestsService.AddGuestToTaskAsync(taskId, guestId, request));
        }

        [Fact]
        public async Task AddGuestToTaskAsync_NullApiResponse_ThrowsInvalidOperationException()
        {
            var taskId = "task_add_null_api";
            var guestId = "guest_add_null_api";
            var request = new AddGuestToItemRequest { PermissionLevel = 1 };
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync((GetGuestResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.AddGuestToTaskAsync(taskId, guestId, request));
        }

        [Fact]
        public async Task RemoveGuestFromTaskAsync_ValidRequest_CallsDeleteAndReturnsGuest()
        {
            var taskId = "task_remove_guest";
            var guestId = "guest_from_task";
            var expectedGuest = CreateSampleGuest(200);
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });
            var result = await _guestsService.RemoveGuestFromTaskAsync(taskId, guestId, false, false, "team_xyz");
            Assert.NotNull(result);
            Assert.Equal(expectedGuest.User.Id, result.User.Id);
            _mockApiConnection.Verify(c => c.DeleteAsync<GetGuestResponse>($"task/{taskId}/guest/{guestId}?include_shared=false&custom_task_ids=false&team_id=team_xyz", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveGuestFromTaskAsync_ApiError_ThrowsHttpRequestException()
        {
            var taskId = "task_remove_err";
            var guestId = "guest_remove_err";
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API error"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _guestsService.RemoveGuestFromTaskAsync(taskId, guestId));
        }

        [Fact]
        public async Task RemoveGuestFromTaskAsync_NullApiResponse_ThrowsInvalidOperationException()
        {
            var taskId = "task_remove_null_api";
            var guestId = "guest_remove_null_api";
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetGuestResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.RemoveGuestFromTaskAsync(taskId, guestId));
        }

        [Fact]
        public async Task RemoveGuestFromTaskAsync_NullGuestInResponse_ThrowsInvalidOperationException()
        {
            var taskId = "task_remove_null_guest";
            var guestId = "guest_remove_null_guest";
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = null! });
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.RemoveGuestFromTaskAsync(taskId, guestId));
        }

        [Fact]
        public async Task AddGuestToListAsync_ValidRequest_CallsPostAndReturnsGuest()
        {
            var listId = "list_add_guest";
            var guestId = "guest_for_list";
            var request = new AddGuestToItemRequest { PermissionLevel = 2 };
            var expectedGuest = CreateSampleGuest(300);
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });
            var result = await _guestsService.AddGuestToListAsync(listId, guestId, request, true);
            Assert.NotNull(result);
            Assert.Equal(expectedGuest.User.Id, result.User.Id);
            _mockApiConnection.Verify(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>($"list/{listId}/guest/{guestId}?include_shared=true", request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddGuestToListAsync_ApiError_ThrowsHttpRequestException()
        {
            var listId = "list_add_err";
            var guestId = "guest_add_err_list";
            var request = new AddGuestToItemRequest { PermissionLevel = 2 };
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API error"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _guestsService.AddGuestToListAsync(listId, guestId, request));
        }

        [Fact]
        public async Task AddGuestToListAsync_NullApiResponse_ThrowsInvalidOperationException()
        {
            var listId = "list_add_null_api";
            var guestId = "guest_add_null_api_list";
            var request = new AddGuestToItemRequest { PermissionLevel = 2 };
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync((GetGuestResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.AddGuestToListAsync(listId, guestId, request));
        }

        [Fact]
        public async Task AddGuestToListAsync_NullGuestInResponse_ThrowsInvalidOperationException()
        {
            var listId = "list_add_null_guest";
            var guestId = "guest_add_null_guest_list";
            var request = new AddGuestToItemRequest { PermissionLevel = 2 };
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse{ Guest = null! });
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.AddGuestToListAsync(listId, guestId, request));
        }

        [Fact]
        public async Task RemoveGuestFromListAsync_ValidRequest_CallsDeleteAndReturnsGuest()
        {
            var listId = "list_remove_guest";
            var guestId = "guest_from_list";
            var expectedGuest = CreateSampleGuest(400);
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });
            var result = await _guestsService.RemoveGuestFromListAsync(listId, guestId, false);
            Assert.NotNull(result);
            Assert.Equal(expectedGuest.User.Id, result.User.Id);
            _mockApiConnection.Verify(c => c.DeleteAsync<GetGuestResponse>($"list/{listId}/guest/{guestId}?include_shared=false", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveGuestFromListAsync_ApiError_ThrowsHttpRequestException()
        {
            var listId = "list_remove_err";
            var guestId = "guest_remove_err_list";
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API error"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _guestsService.RemoveGuestFromListAsync(listId, guestId));
        }

        [Fact]
        public async Task RemoveGuestFromListAsync_NullApiResponse_ThrowsInvalidOperationException()
        {
            var listId = "list_remove_null_api";
            var guestId = "guest_remove_null_api_list";
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetGuestResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.RemoveGuestFromListAsync(listId, guestId));
        }

        [Fact]
        public async Task RemoveGuestFromListAsync_NullGuestInResponse_ThrowsInvalidOperationException()
        {
            var listId = "list_remove_null_guest";
            var guestId = "guest_remove_null_guest_list";
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = null! });
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.RemoveGuestFromListAsync(listId, guestId));
        }

        [Fact]
        public async Task AddGuestToFolderAsync_ValidRequest_CallsPostAndReturnsGuest()
        {
            var folderId = "folder_add_guest";
            var guestId = "guest_for_folder";
            var request = new AddGuestToItemRequest { PermissionLevel = 3 };
            var expectedGuest = CreateSampleGuest(500);
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });
            var result = await _guestsService.AddGuestToFolderAsync(folderId, guestId, request, true);
            Assert.NotNull(result);
            Assert.Equal(expectedGuest.User.Id, result.User.Id);
            _mockApiConnection.Verify(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>($"folder/{folderId}/guest/{guestId}?include_shared=true", request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddGuestToFolderAsync_ApiError_ThrowsHttpRequestException()
        {
            var folderId = "folder_add_err";
            var guestId = "guest_add_err_folder";
            var request = new AddGuestToItemRequest { PermissionLevel = 3 };
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API error"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _guestsService.AddGuestToFolderAsync(folderId, guestId, request));
        }

        [Fact]
        public async Task AddGuestToFolderAsync_NullApiResponse_ThrowsInvalidOperationException()
        {
            var folderId = "folder_add_null_api";
            var guestId = "guest_add_null_api_folder";
            var request = new AddGuestToItemRequest { PermissionLevel = 3 };
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync((GetGuestResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.AddGuestToFolderAsync(folderId, guestId, request));
        }

        [Fact]
        public async Task AddGuestToFolderAsync_NullGuestInResponse_ThrowsInvalidOperationException()
        {
            var folderId = "folder_add_null_guest";
            var guestId = "guest_add_null_guest_folder";
            var request = new AddGuestToItemRequest { PermissionLevel = 3 };
            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse{ Guest = null! });
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.AddGuestToFolderAsync(folderId, guestId, request));
        }

        [Fact]
        public async Task RemoveGuestFromFolderAsync_ValidRequest_CallsDeleteAndReturnsGuest()
        {
            var folderId = "folder_remove_guest";
            var guestId = "guest_from_folder";
            var expectedGuest = CreateSampleGuest(600);
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });
            var result = await _guestsService.RemoveGuestFromFolderAsync(folderId, guestId, false);
            Assert.NotNull(result);
            Assert.Equal(expectedGuest.User.Id, result.User.Id);
            _mockApiConnection.Verify(c => c.DeleteAsync<GetGuestResponse>($"folder/{folderId}/guest/{guestId}?include_shared=false", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveGuestFromFolderAsync_ApiError_ThrowsHttpRequestException()
        {
            var folderId = "folder_remove_err";
            var guestId = "guest_remove_err_folder";
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API error"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _guestsService.RemoveGuestFromFolderAsync(folderId, guestId));
        }

        [Fact]
        public async Task RemoveGuestFromFolderAsync_NullApiResponse_ThrowsInvalidOperationException()
        {
            var folderId = "folder_remove_null_api";
            var guestId = "guest_remove_null_api_folder";
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetGuestResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.RemoveGuestFromFolderAsync(folderId, guestId));
        }

        [Fact]
        public async Task RemoveGuestFromFolderAsync_NullGuestInResponse_ThrowsInvalidOperationException()
        {
            var folderId = "folder_remove_null_guest";
            var guestId = "guest_remove_null_guest_folder";
            _mockApiConnection.Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetGuestResponse { Guest = null! });
            await Assert.ThrowsAsync<InvalidOperationException>(() => _guestsService.RemoveGuestFromFolderAsync(folderId, guestId));
        }

        // --- Tests for TaskCanceledException and CancellationToken pass-through ---

        // InviteGuestToWorkspaceAsync
        [Fact]
        public async Task InviteGuestToWorkspaceAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_invite_cancel";
            var request = new InviteGuestToWorkspaceRequest("cancel@example.com", false, false, false, false, false, 0);
            _mockApiConnection.Setup(c => c.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestToWorkspaceResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ThrowsAsync(new TaskCanceledException("API timeout"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _guestsService.InviteGuestToWorkspaceAsync(workspaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task InviteGuestToWorkspaceAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_invite_ct";
            var request = new InviteGuestToWorkspaceRequest("ct@example.com", false, false, false, false, false, 0);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var dummyUser = new ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseUser(1, "u", "e", "c", null, "i", 0, null, null, null, DateTimeOffset.UtcNow);
            var dummyInviter = new ClickUp.Api.Client.Models.ResponseModels.Guests.InvitedByUserInfoResponse(2, "iu", "ie", "ic", null, "ii");
            var dummyMember = new ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseTeamMember(dummyUser, dummyInviter, false, false, false, false, false);
            var dummyTeam = new ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseTeam("id", "name", "c", null, new List<ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseTeamMember>{dummyMember}, new List<Role>());
            var expectedResponse = new InviteGuestToWorkspaceResponse(dummyTeam);
            _mockApiConnection.Setup(c => c.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestToWorkspaceResponse>(It.IsAny<string>(), request, expectedToken)).ReturnsAsync(expectedResponse);
            await _guestsService.InviteGuestToWorkspaceAsync(workspaceId, request, expectedToken);
            _mockApiConnection.Verify(c => c.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestToWorkspaceResponse>($"team/{workspaceId}/guest", request, expectedToken), Times.Once);
        }

        // GetGuestAsync
        [Fact]
        public async Task GetGuestAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_get_cancel";
            var guestId = "guest_get_cancel";
            _mockApiConnection.Setup(c => c.GetAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new TaskCanceledException("API timeout"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _guestsService.GetGuestAsync(workspaceId, guestId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetGuestAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_get_ct";
            var guestId = "guest_get_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedGuest = CreateSampleGuest(int.Parse(guestId.Replace("guest_get_ct", "300")));
            _mockApiConnection.Setup(c => c.GetAsync<GetGuestResponse>(It.IsAny<string>(), expectedToken)).ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });
            await _guestsService.GetGuestAsync(workspaceId, guestId, expectedToken);
            _mockApiConnection.Verify(c => c.GetAsync<GetGuestResponse>($"team/{workspaceId}/guest/{guestId}", expectedToken), Times.Once);
        }

        // EditGuestOnWorkspaceAsync
        [Fact]
        public async Task EditGuestOnWorkspaceAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_edit_cancel";
            var guestId = "guest_edit_cancel";
            var request = new EditGuestOnWorkspaceRequest(CanEditTags: false, CanSeeTimeEstimated: false, CanSeeTimeSpent: false, CanCreateViews: false, CustomRoleId: 0, CanSeePointsEstimated: false);
            _mockApiConnection.Setup(c => c.PutAsync<EditGuestOnWorkspaceRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>())).ThrowsAsync(new TaskCanceledException("API timeout"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _guestsService.EditGuestOnWorkspaceAsync(workspaceId, guestId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task EditGuestOnWorkspaceAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_edit_ct";
            var guestId = "guest_edit_ct";
            var request = new EditGuestOnWorkspaceRequest(CanEditTags: false, CanSeeTimeEstimated: false, CanSeeTimeSpent: false, CanCreateViews: false, CustomRoleId: 0, CanSeePointsEstimated: false);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedGuest = CreateSampleGuest(int.Parse(guestId.Replace("guest_edit_ct", "400")));
            _mockApiConnection.Setup(c => c.PutAsync<EditGuestOnWorkspaceRequest, GetGuestResponse>(It.IsAny<string>(), request, expectedToken)).ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });
            await _guestsService.EditGuestOnWorkspaceAsync(workspaceId, guestId, request, expectedToken);
            _mockApiConnection.Verify(c => c.PutAsync<EditGuestOnWorkspaceRequest, GetGuestResponse>($"team/{workspaceId}/guest/{guestId}", request, expectedToken), Times.Once);
        }

        // RemoveGuestFromWorkspaceAsync
        [Fact]
        public async Task RemoveGuestFromWorkspaceAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_remove_cancel";
            var guestId = "guest_remove_cancel";
            _mockApiConnection.Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new TaskCanceledException("API timeout"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _guestsService.RemoveGuestFromWorkspaceAsync(workspaceId, guestId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task RemoveGuestFromWorkspaceAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_remove_ct";
            var guestId = "guest_remove_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection.Setup(c => c.DeleteAsync(It.IsAny<string>(), expectedToken)).Returns(Task.CompletedTask);
            await _guestsService.RemoveGuestFromWorkspaceAsync(workspaceId, guestId, expectedToken);
            _mockApiConnection.Verify(c => c.DeleteAsync($"team/{workspaceId}/guest/{guestId}", expectedToken), Times.Once);
        }

        // AddGuestToTaskAsync
        /*
        [Fact]
        public async Task AddGuestToTaskAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var taskId = "task_add_cancel";
            var guestId = "guest_add_cancel";
            var request = new AddGuestToItemRequest { PermissionLevel = 1 };
            _mockApiConnection
                .Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _guestsService.AddGuestToTaskAsync(taskId, guestId, request, false, false, string.Empty, new CancellationTokenSource().Token));
        }
        */


        [Fact]
        public async Task AddGuestToTaskAsync_PassesCancellationTokenToApiConnection()
        {
            var taskId = "task_add_ct";
            var guestId = "guest_add_ct";
            var request = new AddGuestToItemRequest { PermissionLevel = 1 };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedGuest = CreateSampleGuest(500); // Simplified guest creation
            _mockApiConnection
                .Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(
                    $"task/{taskId}/guest/{guestId}", // URL should be exact if no optional params are used
                    request,
                    expectedToken))
                .ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });

            // Calling with null for all optional bool? and string? params
            await _guestsService.AddGuestToTaskAsync(taskId, guestId, request, null, null, null, expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(
                $"task/{taskId}/guest/{guestId}",
                request,
                expectedToken), Times.Once);
        }


        // RemoveGuestFromTaskAsync
        /*
        [Fact]
        public async Task RemoveGuestFromTaskAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var taskId = "task_rem_cancel";
            var guestId = "guest_rem_cancel";
            _mockApiConnection
                .Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _guestsService.RemoveGuestFromTaskAsync(taskId, guestId, false, false, string.Empty, new CancellationTokenSource().Token));
        }
        */

        [Fact]
        public async Task RemoveGuestFromTaskAsync_PassesCancellationTokenToApiConnection()
        {
            var taskId = "task_rem_ct";
            var guestId = "guest_rem_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedGuest = CreateSampleGuest(600); // Simplified
            _mockApiConnection
                .Setup(c => c.DeleteAsync<GetGuestResponse>(
                    $"task/{taskId}/guest/{guestId}", // URL should be exact
                    expectedToken))
                .ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });

            // Calling with null for all optional bool? and string? params
            await _guestsService.RemoveGuestFromTaskAsync(taskId, guestId, null, null, null, expectedToken);

            _mockApiConnection.Verify(c => c.DeleteAsync<GetGuestResponse>(
                $"task/{taskId}/guest/{guestId}",
                expectedToken), Times.Once);
        }


        // AddGuestToListAsync
        [Fact]
        public async Task AddGuestToListAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var listId = "list_add_cancel";
            var guestId = "guest_list_add_cancel";
            var request = new AddGuestToItemRequest { PermissionLevel = 2 };
            _mockApiConnection
                .Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _guestsService.AddGuestToListAsync(listId, guestId, request, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task AddGuestToListAsync_PassesCancellationTokenToApiConnection()
        {
            var listId = "list_add_ct";
            var guestId = "guest_list_add_ct";
            var request = new AddGuestToItemRequest { PermissionLevel = 2 };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedGuest = CreateSampleGuest(int.Parse(guestId.Replace("guest_list_add_ct", "700")));
            _mockApiConnection
                .Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });

            await _guestsService.AddGuestToListAsync(listId, guestId, request, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(
                $"list/{listId}/guest/{guestId}", // Basic URL
                request,
                expectedToken), Times.Once);
        }

        // RemoveGuestFromListAsync
        [Fact]
        public async Task RemoveGuestFromListAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var listId = "list_rem_cancel";
            var guestId = "guest_list_rem_cancel";
            _mockApiConnection
                .Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _guestsService.RemoveGuestFromListAsync(listId, guestId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task RemoveGuestFromListAsync_PassesCancellationTokenToApiConnection()
        {
            var listId = "list_rem_ct";
            var guestId = "guest_list_rem_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedGuest = CreateSampleGuest(int.Parse(guestId.Replace("guest_list_rem_ct", "800")));
            _mockApiConnection
                .Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });

            await _guestsService.RemoveGuestFromListAsync(listId, guestId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.DeleteAsync<GetGuestResponse>(
                $"list/{listId}/guest/{guestId}", // Basic URL
                expectedToken), Times.Once);
        }

        // AddGuestToFolderAsync
        [Fact]
        public async Task AddGuestToFolderAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var folderId = "folder_add_cancel";
            var guestId = "guest_folder_add_cancel";
            var request = new AddGuestToItemRequest { PermissionLevel = 3 };
            _mockApiConnection
                .Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _guestsService.AddGuestToFolderAsync(folderId, guestId, request, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task AddGuestToFolderAsync_PassesCancellationTokenToApiConnection()
        {
            var folderId = "folder_add_ct";
            var guestId = "guest_folder_add_ct";
            var request = new AddGuestToItemRequest { PermissionLevel = 3 };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedGuest = CreateSampleGuest(int.Parse(guestId.Replace("guest_folder_add_ct", "900")));
            _mockApiConnection
                .Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });

            await _guestsService.AddGuestToFolderAsync(folderId, guestId, request, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(
                $"folder/{folderId}/guest/{guestId}", // Basic URL
                request,
                expectedToken), Times.Once);
        }

        // RemoveGuestFromFolderAsync
        [Fact]
        public async Task RemoveGuestFromFolderAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var folderId = "folder_rem_cancel";
            var guestId = "guest_folder_rem_cancel";
            _mockApiConnection
                .Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _guestsService.RemoveGuestFromFolderAsync(folderId, guestId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task RemoveGuestFromFolderAsync_PassesCancellationTokenToApiConnection()
        {
            var folderId = "folder_rem_ct";
            var guestId = "guest_folder_rem_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedGuest = CreateSampleGuest(int.Parse(guestId.Replace("guest_folder_rem_ct", "1000")));
            _mockApiConnection
                .Setup(c => c.DeleteAsync<GetGuestResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetGuestResponse { Guest = expectedGuest });

            await _guestsService.RemoveGuestFromFolderAsync(folderId, guestId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.DeleteAsync<GetGuestResponse>(
                $"folder/{folderId}/guest/{guestId}", // Basic URL
                expectedToken), Times.Once);
        }
    }
}
