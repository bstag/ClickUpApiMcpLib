using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Chat;
using ClickUp.Api.Client.Models.Entities.Users; // Added for User model
using ClickUp.Api.Client.Models.RequestModels.Chat;
using ClickUp.Api.Client.Models.ResponseModels.Chat;
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class ChatServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly ChatService _chatService;
        private readonly Mock<ILogger<ChatService>> _mockLogger;

        public ChatServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<ChatService>>();
            _chatService = new ChatService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private User CreateSampleUser(int id = 1) => new User(id, $"user{id}", $"user{id}@example.com", "#FFFFFF", "http://example.com/pic.jpg", $"U{id}"); // Removed role, will use positional or default
        private ChatSimpleUser CreateSampleChatSimpleUser(string id = "user_1") => new ChatSimpleUser(Email: $"user{id}@example.com", Id: id, Initials: "SU", Name: $"Simple User {id}", Username: $"simpleuser{id}", ProfilePicture: null);

        private ChatMessage CreateSampleChatMessage(string id = "msg_1", string channelId = "ch_1", string teamId = "team_1") => new ChatMessage(
            Id: id,
            Type: "comment",
            User: CreateSampleUser(),
            Date: DateTimeOffset.UtcNow,
            GroupId: "group_1",
            TeamId: teamId, // Use parameter
            ChannelId: channelId,
            Deleted: false,
            Edited: false
        ) { TextContent = "Sample Message" };

        private ChatChannel CreateSampleChannel(string id = "channel_1", string teamId = "ws_1") => new ChatChannel(
            Id: id,
            Name: $"Sample Channel {id}",
            UnreadCount: 0,
            LastMessageAt: DateTimeOffset.UtcNow.AddMinutes(-5),
            TeamId: teamId, // This is correct as per ChatChannel constructor
            CreatedAt: DateTimeOffset.UtcNow.AddHours(-1),
            UpdatedAt: DateTimeOffset.UtcNow,
            IsPrivate: false,
            IsFavorite: false,
            IsMuted: false,
            IsHidden: false,
            IsDirect: false,
            IsReadOnly: false,
            IsSystem: false,
            IsGroupChannel: false,
            Type: ClickUp.Api.Client.Models.Entities.Chat.Enums.ChatRoomType.Channel,
            Visibility: ClickUp.Api.Client.Models.Entities.Chat.Enums.ChatRoomVisibility.Public,
            IsAiEnabled: false
        )
        {
            // These are init-only properties, not part of the primary constructor
            LastMessageId = "last_comment_1",
            Members = new List<User> { CreateSampleUser(2) }
            // Location and Followers are not direct properties of ChatChannel model
        };

        private ClickUpV3DataResponse<ChatChannel> CreateV3DataResponse(ChatChannel channel)
        {
            return new ClickUpV3DataResponse<ChatChannel> { Data = channel };
        }


        [Fact]
        public async Task GetChatChannelsAsync_ValidRequest_BuildsCorrectUrlAndReturnsResponse()
        {
            // Arrange
            var workspaceId = "ws_test";
            var sampleChannel = CreateSampleChannel(teamId: workspaceId);
            var expectedResponse = new ChatChannelPaginatedResponse("next_cursor", new List<ChatChannel> { sampleChannel } );
            _mockApiConnection
                .Setup(c => c.GetAsync<ChatChannelPaginatedResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatChannelsAsync(workspaceId,
                descriptionFormat: "html",
                cursor: "prev_cursor",
                limit: 10,
                isFollower: true,
                includeHidden: false,
                withCommentSince: 1234567890,
                roomTypes: new List<string> { "public_channel", "private_channel" });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Data.First().Id, result.Data.First().Id); // Corrected to access Data property
            _mockApiConnection.Verify(c => c.GetAsync<ChatChannelPaginatedResponse>(
                $"/v3/workspaces/{workspaceId}/channels?description_format=html&cursor=prev_cursor&limit=10&is_follower=true&include_hidden=false&with_comment_since=1234567890&room_types[]={Uri.EscapeDataString("public_channel")}&room_types[]={Uri.EscapeDataString("private_channel")}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateChatChannelAsync_ValidRequest_CallsPostAndReturnsChannel()
        {
            // Arrange
            var workspaceId = "ws_test";
            var request = new ChatCreateChatChannelRequest(
                Description: "A new channel",
                Name: "New Channel",
                Topic: "General",
                UserIds: new List<string> { "1", "2" },
                Visibility: ClickUp.Api.Client.Models.Entities.Chat.Enums.ChatRoomVisibility.Public,
                WorkspaceId: workspaceId // This param seems redundant if workspaceId is already in path
            );
            var expectedChannel = CreateSampleChannel("new_channel_1", workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>( // Changed type reference
                    It.IsAny<string>(),
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateV3DataResponse(expectedChannel));

            // Act
            var result = await _chatService.CreateChatChannelAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedChannel.Id, result.Id);
            _mockApiConnection.Verify(c => c.PostAsync<ChatCreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>( // Changed type reference
                $"/v3/workspaces/{workspaceId}/channels",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatChannelAsync_ApiError_ThrowsHttpRequestException()
        {
            // Arrange
            var workspaceId = "ws_test";
            var channelId = "ch_error";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), It.IsAny<CancellationToken>())) // Changed type reference
                .ThrowsAsync(new HttpRequestException("API Down"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.GetChatChannelAsync(workspaceId, channelId));
        }

        [Fact]
        public async Task GetChatChannelAsync_NullResponseData_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_test";
            var channelId = "ch_null_data";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), It.IsAny<CancellationToken>())) // Changed type reference
                .ReturnsAsync(new ClickUpV3DataResponse<ChatChannel> { Data = null }); // Null Data property

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.GetChatChannelAsync(workspaceId, channelId));
        }
    }
}
