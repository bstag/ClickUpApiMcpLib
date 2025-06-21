using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Guests;
using ClickUp.Api.Client.Models.ResponseModels.Guests; // Assuming response wrappers
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Tests.Services
{
    public class GuestsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly GuestsService _guestsService;

        public GuestsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _guestsService = new GuestsService(_mockApiConnection.Object);
        }

        // Placeholder DTO creation helpers
        private Guest CreateSampleGuest(string id, string username)
        {
            var guest = (Guest)Activator.CreateInstance(typeof(Guest), nonPublic: true)!;
            var userProp = typeof(Guest).GetProperty("User");
            if (userProp != null)
            {
                var user = (User)Activator.CreateInstance(typeof(User), nonPublic: true)!;
                typeof(User).GetProperty("Id")?.SetValue(user, int.Parse(id)); // Assuming Guest User Id is int
                typeof(User).GetProperty("Username")?.SetValue(user, username);
                userProp.SetValue(guest, user);
            }
            // Set other Guest specific properties
            return guest;
        }

        private TeamMember CreateSampleTeamMember(int userId, string username)
        {
            var user = (User)Activator.CreateInstance(typeof(User), nonPublic: true)!;
            typeof(User).GetProperty("Id")?.SetValue(user, userId);
            typeof(User).GetProperty("Username")?.SetValue(user, username);

            var teamMember = (TeamMember)Activator.CreateInstance(typeof(TeamMember), nonPublic: true)!;
            typeof(TeamMember).GetProperty("User")?.SetValue(teamMember, user);
            return teamMember;
        }

        private GetGuestResponse CreateSampleGetGuestResponse(Guest guest) => new GetGuestResponse(guest); // Assuming record constructor
        private InviteGuestResponse CreateSampleInviteGuestResponse(TeamMember member) => new InviteGuestResponse(member);


        [Fact]
        public async Task GetGuestAsync_WhenGuestExists_ReturnsGuest()
        {
            // Arrange
            var workspaceId = "test-workspace-id";
            var guestId = "123"; // Guest IDs are often numeric
            var sampleGuest = CreateSampleGuest(guestId, "Test Guest");
            var expectedResponse = CreateSampleGetGuestResponse(sampleGuest);

            _mockApiConnection.Setup(c => c.GetAsync<GetGuestResponse>(
                $"team/{workspaceId}/guest/{guestId}",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _guestsService.GetGuestAsync(workspaceId, guestId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleGuest);
        }

        [Fact]
        public async Task InviteGuestToWorkspaceAsync_ValidRequest_ReturnsInvitedGuest()
        {
            // Arrange
            var workspaceId = "test-workspace-id";
            var requestDto = new InviteGuestToWorkspaceRequest("guest@example.com", 123, 456); // Example values
            var expectedTeamMember = CreateSampleTeamMember(789, "guest@example.com");
            var expectedResponse = CreateSampleInviteGuestResponse(expectedTeamMember);

            _mockApiConnection.Setup(c => c.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestResponse>(
                $"team/{workspaceId}/guest",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _guestsService.InviteGuestToWorkspaceAsync(workspaceId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTeamMember);
        }

        [Fact]
        public async Task RemoveGuestFromWorkspaceAsync_ValidId_CallsApiConnectionDeleteAsync()
        {
            // Arrange
            var workspaceId = "test-workspace-id";
            var guestId = "guest-to-remove";
            var expectedEndpoint = $"team/{workspaceId}/guest/{guestId}";

            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _guestsService.RemoveGuestFromWorkspaceAsync(workspaceId, guestId, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddGuestToTaskAsync_ValidRequest_ReturnsGuest()
        {
            // Arrange
            var taskId = "test-task-id";
            var guestId = "guest-to-add-to-task";
            var requestDto = new AddGuestToItemRequest("read"); // Example permission
            var sampleGuest = CreateSampleGuest(guestId, "Guest Added To CuTask");
            var expectedResponse = CreateSampleGetGuestResponse(sampleGuest);

            var expectedEndpointPrefix = $"task/{taskId}/guest/{guestId}";

            _mockApiConnection.Setup(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(
                It.Is<string>(s => s.StartsWith(expectedEndpointPrefix)),
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _guestsService.AddGuestToTaskAsync(taskId, guestId, requestDto, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleGuest);
            _mockApiConnection.Verify(c => c.PostAsync<AddGuestToItemRequest, GetGuestResponse>(
                It.Is<string>(s => s.StartsWith(expectedEndpointPrefix)),
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
