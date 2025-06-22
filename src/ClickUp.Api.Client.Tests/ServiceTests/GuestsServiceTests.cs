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

        // Removed duplicate constructor and extra brace

        private User CreateSampleUserEntity(int id = 1, string email = "user@example.com")
        {
            return new User(
                Id: id,
                Username: $"user{id}",
                Email: email,
                Color: "#FF0000",
                ProfilePicture: "url",
                Initials: $"U{id}"
                // Other optional params like Role, CustomRole, etc., are defaulted to null by User record constructor
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
                Initials = "IV", // Ensure non-null for DTO
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
            Shared = new ClickUp.Api.Client.Models.Entities.Users.GuestSharingDetails() // Fully qualified
        };

        [Fact]
        public async Task InviteGuestToWorkspaceAsync_ValidRequest_CallsPostAndReturnsResponse()
        {
            // Arrange
            var workspaceId = "ws_test";
            var request = new InviteGuestToWorkspaceRequest(
                Email: "guest@example.com",
                CanEditTags: false,
                CanSeeTimeSpent: true,
                CanSeeTimeEstimated: true,
                CanCreateViews: false,
                CanSeePointsEstimated: true,
                CustomRoleId: 123
            );

            var sampleInvitedUserDto = new ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseUser(
                Id: 1, Username: "guest1", Email: "guest@example.com", Color: "#00FF00", ProfilePicture: null, Initials: "G1", Role: 0, CustomRole: null, LastActive: null, DateJoined: null, DateInvited: DateTimeOffset.UtcNow
            );
            var sampleInviterDto = new ClickUp.Api.Client.Models.ResponseModels.Guests.InvitedByUserInfo(
                UserId: 99, Username: "AdminInviter", Email: "admin@example.com", Color: "#FFFF00", ProfilePicture: null, Initials: "AI"
            );
            var teamMember = new ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseTeamMember(
                User: sampleInvitedUserDto,
                InvitedBy: sampleInviterDto,
                CanSeeTimeSpent: true, CanSeeTimeEstimated: true, CanEditTags: false, CanCreateViews: false, CanSeePointsEstimated: true
            );
            var responseTeam = new ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseTeam(
                Id: workspaceId, Name: "Test Workspace", Color: "#123123", Avatar: null,
                Members: new List<ClickUp.Api.Client.Models.ResponseModels.Guests.InviteGuestToWorkspaceResponseTeamMember>{ teamMember },
                Roles: new List<Role>() // Role from Entities.Users
            );
            var expectedResponse = new InviteGuestToWorkspaceResponse(responseTeam);

            _mockApiConnection
                .Setup(c => c.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestToWorkspaceResponse>(
                    It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _guestsService.InviteGuestToWorkspaceAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Team.Id, result.Team.Id);
            Assert.Equal(expectedResponse.Team.Members.First().User.Email, result.Team.Members.First().User.Email);
            _mockApiConnection.Verify(c => c.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestToWorkspaceResponse>(
                $"team/{workspaceId}/guest",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetGuestAsync_ValidIds_BuildsCorrectUrlAndReturnsGuest()
        {
            // Arrange
            var workspaceId = "ws_test";
            var guestIdString = "123";
            int guestUserId = int.Parse(guestIdString);
            var expectedGuest = CreateSampleGuest(userId: guestUserId);
            var apiResponse = new GetGuestResponse { Guest = expectedGuest };
            _mockApiConnection
                .Setup(c => c.GetAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _guestsService.GetGuestAsync(workspaceId, guestIdString);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedGuest.User.Id, result.Guest.User.Id);
             _mockApiConnection.Verify(c => c.GetAsync<GetGuestResponse>(
                $"team/{workspaceId}/guest/{guestIdString}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetGuestAsync_ApiError_ThrowsHttpRequestException()
        {
            // Arrange
            var workspaceId = "ws_test";
            var guestId = "guest_error";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetGuestResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _guestsService.GetGuestAsync(workspaceId, guestId));
        }

        [Fact]
        public async Task AddGuestToTaskAsync_NullResponseData_ThrowsInvalidOperationException()
        {
            // Arrange
            var taskId = "task_1";
            var guestId = "guest_1";
            // AddGuestToItemRequest is a class with settable PermissionLevel, not a record with primary constructor.
            var request = new AddGuestToItemRequest { PermissionLevel = 4 }; // Assuming 4 is a valid int level, e.g. "Full"
            _mockApiConnection
                .Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetGuestResponse { Guest = null! }); // Simulate null Guest in response

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _guestsService.AddGuestToTaskAsync(taskId, guestId, request));
        }
    }
}
