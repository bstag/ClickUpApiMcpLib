using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Chat;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Chat;
using ClickUp.Api.Client.Models.ResponseModels.Chat;
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ClickUp.Api.Client.Models.Entities.Chat.Enums;
using ClickUp.Api.Client.Models.ResponseModels.Shared;

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

        private User CreateSampleUser(int id = 1) => new User(id, $"user{id}", $"user{id}@example.com", "#FFFFFF", $"http://example.com/pic{id}.jpg", $"U{id}");
        private ChatSimpleUser CreateSampleChatSimpleUser(string id = "user_1_id", string email = "user1@example.com", string initials = "U1", string name = "User One", string username = "userone", string? profilePicture = null)
            => new ChatSimpleUser(Id: id, Email: email, Initials: initials, Name: name, Username: username, ProfilePicture: profilePicture);


        private ChatMessage CreateSampleChatMessage(string id = "msg_1", string channelId = "ch_1", string teamId = "team_1", string content = "Sample Message") => new ChatMessage(
            Id: id,
            Type: "comment",
            User: CreateSampleUser(),
            Date: DateTimeOffset.UtcNow,
            GroupId: "group_1",
            TeamId: teamId,
            ChannelId: channelId,
            Deleted: false,
            Edited: false
        ) { TextContent = content };

        private ChatChannel CreateSampleChannel(string id = "channel_1", string teamId = "ws_1") => new ChatChannel(
            Id: id,
            Name: $"Sample Channel {id}",
            UnreadCount: 0,
            LastMessageAt: DateTimeOffset.UtcNow.AddMinutes(-5),
            TeamId: teamId,
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
            Type: ChatRoomType.Channel,
            Visibility: ChatRoomVisibility.Public,
            IsAiEnabled: false
        )
        {
            LastMessageId = "last_comment_1",
            Members = new List<User> { CreateSampleUser(2) }
        };

        private ClickUpV3DataResponse<T> CreateV3DataResponse<T>(T data) where T : class
        {
            return new ClickUpV3DataResponse<T> { Data = data };
        }


        [Fact]
        public async Task GetChatChannelsAsync_ValidRequest_BuildsCorrectUrlAndReturnsResponse()
        {
            // Arrange
            var workspaceId = "ws_test";
            var sampleChannel = CreateSampleChannel(teamId: workspaceId);
            var expectedResponse = new ChatChannelPaginatedResponse("next_cursor", new List<ChatChannel> { sampleChannel });
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
            Assert.Equal(expectedResponse.Data.First().Id, result.Data.First().Id);
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
                Visibility: ChatRoomVisibility.Public,
                WorkspaceId: workspaceId
            );
            var expectedChannel = CreateSampleChannel("new_channel_1", workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
                    It.IsAny<string>(),
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateV3DataResponse(expectedChannel));

            // Act
            var result = await _chatService.CreateChatChannelAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedChannel.Id, result.Id);
            _mockApiConnection.Verify(c => c.PostAsync<ChatCreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
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
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<ChatChannel> { Data = null });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.GetChatChannelAsync(workspaceId, channelId));
        }

        // --- Tests for API Error Cases (HttpRequestException) ---
        [Fact]
        public async Task GetChatChannelsAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_channels_error";
            _mockApiConnection
                .Setup(c => c.GetAsync<ChatChannelPaginatedResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.GetChatChannelsAsync(workspaceId));
        }

        [Fact]
        public async Task CreateChatChannelAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_create_channel_error";
            var request = new ChatCreateChatChannelRequest("Desc", "Name", "Topic", new List<string>(), ChatRoomVisibility.Public, workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.CreateChatChannelAsync(workspaceId, request));
        }

        // --- Tests for Null/Unexpected API Responses ---
        [Fact]
        public async Task GetChatChannelsAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_channels_null_resp";
            _mockApiConnection
                .Setup(c => c.GetAsync<ChatChannelPaginatedResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ChatChannelPaginatedResponse?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.GetChatChannelsAsync(workspaceId));
        }

        [Fact]
        public async Task GetChatChannelsAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_channels_null_data";
            _mockApiConnection
                .Setup(c => c.GetAsync<ChatChannelPaginatedResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ChatChannelPaginatedResponse("next", null!));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.GetChatChannelsAsync(workspaceId));
        }

        [Fact]
        public async Task CreateChatChannelAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_create_null_resp";
            var request = new ChatCreateChatChannelRequest("Desc", "Name", "Topic", new List<string>(), ChatRoomVisibility.Public, workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataResponse<ChatChannel>?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateChatChannelAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateChatChannelAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_create_null_data";
            var request = new ChatCreateChatChannelRequest("Desc", "Name", "Topic", new List<string>(), ChatRoomVisibility.Public, workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<ChatChannel> { Data = null });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateChatChannelAsync(workspaceId, request));
        }

        // --- Tests for TaskCanceledException and CancellationToken pass-through ---

        // GetChatChannelsAsync
        [Fact]
        public async Task GetChatChannelsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<ChatChannelPaginatedResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.GetChatChannelsAsync(workspaceId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetChatChannelsAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new ChatChannelPaginatedResponse("next", new List<ChatChannel>());
            _mockApiConnection
                .Setup(c => c.GetAsync<ChatChannelPaginatedResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _chatService.GetChatChannelsAsync(workspaceId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<ChatChannelPaginatedResponse>(
                $"/v3/workspaces/{workspaceId}/channels",
                expectedToken), Times.Once);
        }

        // CreateChatChannelAsync
        [Fact]
        public async Task CreateChatChannelAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_create_cancel";
            var request = new ChatCreateChatChannelRequest("D", "N", "T", new List<string>(), ChatRoomVisibility.Public, workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.CreateChatChannelAsync(workspaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreateChatChannelAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_create_ct_pass";
            var request = new ChatCreateChatChannelRequest("D", "N", "T", new List<string>(), ChatRoomVisibility.Public, workspaceId);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedChannel = CreateSampleChannel("new_ch", workspaceId);
            var expectedResponse = CreateV3DataResponse(expectedChannel);

            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(expectedResponse);

            await _chatService.CreateChatChannelAsync(workspaceId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<ChatCreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
                $"/v3/workspaces/{workspaceId}/channels",
                request,
                expectedToken), Times.Once);
        }

        // GetChatChannelAsync
        [Fact]
        public async Task GetChatChannelAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_get_ch_cancel";
            var channelId = "ch_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.GetChatChannelAsync(workspaceId, channelId, descriptionFormat: null, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetChatChannelAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_get_ch_ct_pass";
            var channelId = "ch_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedChannel = CreateSampleChannel(channelId, workspaceId);
            var expectedResponse = CreateV3DataResponse(expectedChannel);


            _mockApiConnection
                .Setup(c => c.GetAsync<ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _chatService.GetChatChannelAsync(workspaceId, channelId, descriptionFormat: null, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<ClickUpV3DataResponse<ChatChannel>>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}",
                expectedToken), Times.Once);
        }

        // --- CreateLocationChatChannelAsync Tests ---
        [Fact]
        public async Task CreateLocationChatChannelAsync_ValidRequest_CallsPostAndReturnsChannel()
        {
            // Arrange
            var workspaceId = "ws_loc_create";
            var request = new ChatCreateLocationChatChannelRequest(
                Name: "Location Channel",
                Description: "Channel for location",
                UserIds: new List<string> { "user_1" },
                Visibility: ChatRoomVisibility.Public,
                Location: new ChatChannelLocation("space_123", "space"),
                WorkspaceId: workspaceId,
                Topic: "Some Topic" // Added Topic as it's a positional param
            );
            var expectedChannel = CreateSampleChannel("loc_ch_1", workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateLocationChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
                    $"/v3/workspaces/{workspaceId}/channels/location",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateV3DataResponse(expectedChannel));

            // Act
            var result = await _chatService.CreateLocationChatChannelAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedChannel.Id, result.Id);
            _mockApiConnection.Verify(c => c.PostAsync<ChatCreateLocationChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
                $"/v3/workspaces/{workspaceId}/channels/location",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateLocationChatChannelAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_loc_create_err";
            var request = new ChatCreateLocationChatChannelRequest("D", "T", new List<string>(), ChatRoomVisibility.Private, new ChatChannelLocation("s", "s"), "N", workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateLocationChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.CreateLocationChatChannelAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateLocationChatChannelAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_loc_create_null";
            var request = new ChatCreateLocationChatChannelRequest("D", "T", new List<string>(), ChatRoomVisibility.Private, new ChatChannelLocation("s", "s"), "N", workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateLocationChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataResponse<ChatChannel>?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateLocationChatChannelAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateLocationChatChannelAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_loc_create_null_data";
            var request = new ChatCreateLocationChatChannelRequest("D", "T", new List<string>(), ChatRoomVisibility.Private, new ChatChannelLocation("s", "s"), "N", workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateLocationChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<ChatChannel> { Data = null });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateLocationChatChannelAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateLocationChatChannelAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_loc_create_cancel";
            var request = new ChatCreateLocationChatChannelRequest("D", "T", new List<string>(), ChatRoomVisibility.Private, new ChatChannelLocation("s", "s"), "N", workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateLocationChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.CreateLocationChatChannelAsync(workspaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreateLocationChatChannelAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_loc_create_ct";
            var request = new ChatCreateLocationChatChannelRequest("D", "T", new List<string>(), ChatRoomVisibility.Private, new ChatChannelLocation("s", "s"), "N", workspaceId);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedChannel = CreateSampleChannel("loc_ch_ct", workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateLocationChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(CreateV3DataResponse(expectedChannel));

            await _chatService.CreateLocationChatChannelAsync(workspaceId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<ChatCreateLocationChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
                $"/v3/workspaces/{workspaceId}/channels/location",
                request,
                expectedToken), Times.Once);
        }

        // --- CreateDirectMessageChatChannelAsync Tests ---
        [Fact]
        public async Task CreateDirectMessageChatChannelAsync_ValidRequest_CallsPostAndReturnsChannel()
        {
            // Arrange
            var workspaceId = "ws_dm_create";
            var request = new ChatCreateDirectMessageChatChannelRequest(
                UserIds: new List<string> { "user_1", "user_2" },
                WorkspaceId: workspaceId
            );
            var expectedChannel = CreateSampleChannel("dm_ch_1", workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateDirectMessageChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
                    $"/v3/workspaces/{workspaceId}/channels/dm",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateV3DataResponse(expectedChannel));

            // Act
            var result = await _chatService.CreateDirectMessageChatChannelAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedChannel.Id, result.Id);
            _mockApiConnection.Verify(c => c.PostAsync<ChatCreateDirectMessageChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
                $"/v3/workspaces/{workspaceId}/channels/dm",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateDirectMessageChatChannelAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_dm_create_err";
            var request = new ChatCreateDirectMessageChatChannelRequest(new List<string> { "u1" }, workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateDirectMessageChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.CreateDirectMessageChatChannelAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateDirectMessageChatChannelAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_dm_create_null";
            var request = new ChatCreateDirectMessageChatChannelRequest(new List<string> { "u1" }, workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateDirectMessageChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataResponse<ChatChannel>?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateDirectMessageChatChannelAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateDirectMessageChatChannelAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_dm_create_null_data";
            var request = new ChatCreateDirectMessageChatChannelRequest(new List<string> { "u1" }, workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateDirectMessageChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<ChatChannel> { Data = null });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateDirectMessageChatChannelAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateDirectMessageChatChannelAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_dm_create_cancel";
            var request = new ChatCreateDirectMessageChatChannelRequest(new List<string> { "u1" }, workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateDirectMessageChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.CreateDirectMessageChatChannelAsync(workspaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreateDirectMessageChatChannelAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_dm_create_ct";
            var request = new ChatCreateDirectMessageChatChannelRequest(new List<string> { "u1" }, workspaceId);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedChannel = CreateSampleChannel("dm_ch_ct", workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<ChatCreateDirectMessageChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(CreateV3DataResponse(expectedChannel));

            await _chatService.CreateDirectMessageChatChannelAsync(workspaceId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<ChatCreateDirectMessageChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
                $"/v3/workspaces/{workspaceId}/channels/dm",
                request,
                expectedToken), Times.Once);
        }

        // --- UpdateChatChannelAsync Tests ---
        [Fact]
        public async Task UpdateChatChannelAsync_ValidRequest_CallsPutAndReturnsChannel()
        {
            // Arrange
            var workspaceId = "ws_update_ch";
            var channelId = "ch_to_update";
            var request = new ChatUpdateChatChannelRequest(
                Name: "Updated Channel Name",
                Topic: "Updated Topic",
                Description: "Updated Description",
                Location: null,
                ContentFormat: "html",
                Visibility: ChatRoomVisibility.Private,
                Archived: false
            );
            var updatedChannel = CreateSampleChannel(channelId, workspaceId);
            updatedChannel = updatedChannel with { Name = request.Name! };

            _mockApiConnection
                .Setup(c => c.PutAsync<ChatUpdateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
                    $"/v3/workspaces/{workspaceId}/channels/{channelId}",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateV3DataResponse(updatedChannel));

            // Act
            var result = await _chatService.UpdateChatChannelAsync(workspaceId, channelId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedChannel.Name, result.Name);
            _mockApiConnection.Verify(c => c.PutAsync<ChatUpdateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateChatChannelAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_update_ch_err";
            var channelId = "ch_update_err";
            var request = new ChatUpdateChatChannelRequest(ContentFormat: "text", Description: "Err desc", Location: null, Name: "N", Topic: null, Visibility: null, Archived: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<ChatUpdateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.UpdateChatChannelAsync(workspaceId, channelId, request));
        }

        [Fact]
        public async Task UpdateChatChannelAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_update_ch_null";
            var channelId = "ch_update_null";
            var request = new ChatUpdateChatChannelRequest(ContentFormat: "text", Description: "Null desc", Location: null, Name: "N", Topic: null, Visibility: null, Archived: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<ChatUpdateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataResponse<ChatChannel>?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.UpdateChatChannelAsync(workspaceId, channelId, request));
        }

        [Fact]
        public async Task UpdateChatChannelAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_update_ch_null_data";
            var channelId = "ch_update_null_data";
            var request = new ChatUpdateChatChannelRequest(ContentFormat: "text", Description: "Null data desc", Location: null, Name: "N", Topic: null, Visibility: null, Archived: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<ChatUpdateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<ChatChannel> { Data = null });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.UpdateChatChannelAsync(workspaceId, channelId, request));
        }

        [Fact]
        public async Task UpdateChatChannelAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_update_ch_cancel";
            var channelId = "ch_update_cancel";
            var request = new ChatUpdateChatChannelRequest(ContentFormat: "text", Description: "Cancel desc", Location: null, Name: "N", Topic: null, Visibility: null, Archived: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<ChatUpdateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.UpdateChatChannelAsync(workspaceId, channelId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task UpdateChatChannelAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_update_ch_ct";
            var channelId = "ch_update_ct";
            var request = new ChatUpdateChatChannelRequest(ContentFormat: "text", Description: "CT desc", Location: null, Name: "N", Topic: null, Visibility: null, Archived: null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var updatedChannel = CreateSampleChannel(channelId, workspaceId);
            _mockApiConnection
                .Setup(c => c.PutAsync<ChatUpdateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(CreateV3DataResponse(updatedChannel));

            await _chatService.UpdateChatChannelAsync(workspaceId, channelId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PutAsync<ChatUpdateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}",
                request,
                expectedToken), Times.Once);
        }

        // --- DeleteChatChannelAsync Tests ---
        [Fact]
        public async Task DeleteChatChannelAsync_ValidRequest_CallsDelete()
        {
            // Arrange
            var workspaceId = "ws_delete_ch";
            var channelId = "ch_to_delete";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(
                    $"/v3/workspaces/{workspaceId}/channels/{channelId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _chatService.DeleteChatChannelAsync(workspaceId, channelId);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteChatChannelAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_delete_ch_err";
            var channelId = "ch_delete_err";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.DeleteChatChannelAsync(workspaceId, channelId));
        }

        [Fact]
        public async Task DeleteChatChannelAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_delete_ch_cancel";
            var channelId = "ch_delete_cancel";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.DeleteChatChannelAsync(workspaceId, channelId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task DeleteChatChannelAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_delete_ch_ct";
            var channelId = "ch_delete_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), expectedToken))
                .Returns(Task.CompletedTask);

            await _chatService.DeleteChatChannelAsync(workspaceId, channelId, expectedToken);

            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}",
                expectedToken), Times.Once);
        }

        // --- GetChatChannelFollowersAsync Tests ---
        [Fact]
        public async Task GetChatChannelFollowersAsync_ValidRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var workspaceId = "ws_followers";
            var channelId = "ch_followers";
            var cursor = "next_cursor_val";
            var limit = 20;
            var expectedResponse = new GetChatUsersResponse(
                Data: new List<ChatSimpleUser> { CreateSampleChatSimpleUser("user_abc") },
                Meta: new GetChatUsersResponse.ResponseMeta(NextCursor: "next_page_cursor", PreviousCursor: null)
            );
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(
                    $"/v3/workspaces/{workspaceId}/channels/{channelId}/followers?cursor={cursor}&limit={limit}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatChannelFollowersAsync(workspaceId, channelId, cursor, limit);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Meta.NextCursor, result.Meta.NextCursor);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatUsersResponse>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}/followers?cursor={cursor}&limit={limit}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatChannelFollowersAsync_MinimalRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var workspaceId = "ws_followers_min";
            var channelId = "ch_followers_min";
            var expectedResponse = new GetChatUsersResponse(
                Data: new List<ChatSimpleUser>(),
                Meta: new GetChatUsersResponse.ResponseMeta(NextCursor: null, PreviousCursor: null)
            );
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(
                    $"/v3/workspaces/{workspaceId}/channels/{channelId}/followers",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatChannelFollowersAsync(workspaceId, channelId);

            // Assert
            Assert.NotNull(result);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatUsersResponse>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}/followers",
                It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task GetChatChannelFollowersAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_followers_err";
            var channelId = "ch_followers_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.GetChatChannelFollowersAsync(workspaceId, channelId));
        }

        [Fact]
        public async Task GetChatChannelFollowersAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_followers_null";
            var channelId = "ch_followers_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetChatUsersResponse?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.GetChatChannelFollowersAsync(workspaceId, channelId));
        }

        [Fact]
        public async Task GetChatChannelFollowersAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_followers_cancel";
            var channelId = "ch_followers_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.GetChatChannelFollowersAsync(workspaceId, channelId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetChatChannelFollowersAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_followers_ct";
            var channelId = "ch_followers_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetChatUsersResponse(new List<ChatSimpleUser>(), new GetChatUsersResponse.ResponseMeta(null, null));
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _chatService.GetChatChannelFollowersAsync(workspaceId, channelId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<GetChatUsersResponse>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}/followers",
                expectedToken), Times.Once);
        }

        // --- GetChatChannelMembersAsync Tests ---
        [Fact]
        public async Task GetChatChannelMembersAsync_ValidRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var workspaceId = "ws_members";
            var channelId = "ch_members";
            var cursor = "cursor_val_members";
            var limit = 15;
            var expectedResponse = new GetChatUsersResponse(
                Data: new List<ChatSimpleUser> { CreateSampleChatSimpleUser("user_xyz") },
                Meta: new GetChatUsersResponse.ResponseMeta(NextCursor: "next_members_cursor", PreviousCursor: null)
            );
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(
                    $"/v3/workspaces/{workspaceId}/channels/{channelId}/members?cursor={cursor}&limit={limit}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatChannelMembersAsync(workspaceId, channelId, cursor, limit);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Meta.NextCursor, result.Meta.NextCursor);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatUsersResponse>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}/members?cursor={cursor}&limit={limit}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatChannelMembersAsync_MinimalRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var workspaceId = "ws_members_min";
            var channelId = "ch_members_min";
            var expectedResponse = new GetChatUsersResponse(new List<ChatSimpleUser>(), new GetChatUsersResponse.ResponseMeta(null, null));
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(
                    $"/v3/workspaces/{workspaceId}/channels/{channelId}/members",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatChannelMembersAsync(workspaceId, channelId);

            // Assert
            Assert.NotNull(result);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatUsersResponse>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}/members",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatChannelMembersAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_members_err";
            var channelId = "ch_members_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.GetChatChannelMembersAsync(workspaceId, channelId));
        }

        [Fact]
        public async Task GetChatChannelMembersAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_members_null";
            var channelId = "ch_members_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetChatUsersResponse?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.GetChatChannelMembersAsync(workspaceId, channelId));
        }

        [Fact]
        public async Task GetChatChannelMembersAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_members_cancel";
            var channelId = "ch_members_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.GetChatChannelMembersAsync(workspaceId, channelId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetChatChannelMembersAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_members_ct";
            var channelId = "ch_members_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetChatUsersResponse(new List<ChatSimpleUser>(), new GetChatUsersResponse.ResponseMeta(null, null));
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _chatService.GetChatChannelMembersAsync(workspaceId, channelId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<GetChatUsersResponse>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}/members",
                expectedToken), Times.Once);
        }

        // --- GetChatMessagesAsync Tests ---
        [Fact]
        public async Task GetChatMessagesAsync_ValidRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var workspaceId = "ws_msgs";
            var channelId = "ch_msgs";
            var cursor = "msg_cursor";
            var limit = 25;
            var contentFormat = "html";
            var sampleMessage = CreateSampleChatMessage("msg_123", channelId, workspaceId);
            var expectedResponse = new GetChatMessagesResponse(
                Data: new List<ChatMessage> { sampleMessage },
                Meta: new GetChatMessagesResponse.ResponseMeta(NextCursor: "next_msg_cursor", PreviousCursor: null)
            );

            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(
                    $"/v3/workspaces/{workspaceId}/channels/{channelId}/messages?cursor={cursor}&limit={limit}&content_format={contentFormat}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatMessagesAsync(workspaceId, channelId, cursor, limit, contentFormat);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.Equal(expectedResponse.Meta.NextCursor, result.Meta.NextCursor);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatMessagesResponse>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}/messages?cursor={cursor}&limit={limit}&content_format={contentFormat}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatMessagesAsync_MinimalRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var workspaceId = "ws_msgs_min";
            var channelId = "ch_msgs_min";
            var expectedResponse = new GetChatMessagesResponse(new List<ChatMessage>(), new GetChatMessagesResponse.ResponseMeta(null, null));
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(
                    $"/v3/workspaces/{workspaceId}/channels/{channelId}/messages",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatMessagesAsync(workspaceId, channelId);

            // Assert
            Assert.NotNull(result);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatMessagesResponse>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}/messages",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatMessagesAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_msgs_err";
            var channelId = "ch_msgs_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.GetChatMessagesAsync(workspaceId, channelId));
        }

        [Fact]
        public async Task GetChatMessagesAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_msgs_null";
            var channelId = "ch_msgs_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetChatMessagesResponse?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.GetChatMessagesAsync(workspaceId, channelId));
        }

        [Fact]
        public async Task GetChatMessagesAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_msgs_cancel";
            var channelId = "ch_msgs_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.GetChatMessagesAsync(workspaceId, channelId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetChatMessagesAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_msgs_ct";
            var channelId = "ch_msgs_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetChatMessagesResponse(new List<ChatMessage>(), new GetChatMessagesResponse.ResponseMeta(null, null));
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _chatService.GetChatMessagesAsync(workspaceId, channelId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<GetChatMessagesResponse>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}/messages",
                expectedToken), Times.Once);
        }

        // --- CreateChatMessageAsync Tests ---
        [Fact]
        public async Task CreateChatMessageAsync_ValidRequest_CallsPostAndReturnsMessage()
        {
            // Arrange
            var workspaceId = "ws_create_msg"; // This is for URL, not DTO
            var channelId = "ch_create_msg";   // This is for URL, not DTO
            var request = new CommentCreateChatMessageRequest(
                Assignee: null, GroupAssignee: null, TriagedAction: null, TriagedObjectId: null, TriagedObjectType: null,
                Type: "message", Content: "Hello world!", Reactions: null, Followers: null, ContentFormat: null,
                PostData: null, ParentMessageId: null
            );
            var expectedMessage = CreateSampleChatMessage("new_msg_1", channelId, workspaceId, request.Content);

            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(
                    $"/v3/workspaces/{workspaceId}/channels/{channelId}/messages",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateV3DataResponse(expectedMessage));

            // Act
            var result = await _chatService.CreateChatMessageAsync(workspaceId, channelId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMessage.Id, result.Id);
            _mockApiConnection.Verify(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}/messages",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateChatMessageAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_create_msg_err";
            var channelId = "ch_create_msg_err";
            var request = new CommentCreateChatMessageRequest(null, null, null, null, null, "message", "Error message", null, null, null, null, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.CreateChatMessageAsync(workspaceId, channelId, request));
        }

        [Fact]
        public async Task CreateChatMessageAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_create_msg_null";
            var channelId = "ch_create_msg_null";
            var request = new CommentCreateChatMessageRequest(null, null, null, null, null, "message", "Null response message", null, null, null, null, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataResponse<ChatMessage>?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateChatMessageAsync(workspaceId, channelId, request));
        }

        [Fact]
        public async Task CreateChatMessageAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_create_msg_null_data";
            var channelId = "ch_create_msg_null_data";
            var request = new CommentCreateChatMessageRequest(null, null, null, null, null, "message", "Null data message", null, null, null, null, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<ChatMessage> { Data = null });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateChatMessageAsync(workspaceId, channelId, request));
        }

        [Fact]
        public async Task CreateChatMessageAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_create_msg_cancel";
            var channelId = "ch_create_msg_cancel";
            var request = new CommentCreateChatMessageRequest(null, null, null, null, null, "message", "Cancel message", null, null, null, null, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.CreateChatMessageAsync(workspaceId, channelId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreateChatMessageAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_create_msg_ct";
            var channelId = "ch_create_msg_ct";
            var request = new CommentCreateChatMessageRequest(null, null, null, null, null, "message", "CT message", null, null, null, null, null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedMessage = CreateSampleChatMessage("msg_ct_1", channelId, workspaceId);
            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(CreateV3DataResponse(expectedMessage));

            await _chatService.CreateChatMessageAsync(workspaceId, channelId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(
                $"/v3/workspaces/{workspaceId}/channels/{channelId}/messages",
                request,
                expectedToken), Times.Once);
        }

        // --- UpdateChatMessageAsync Tests ---
        [Fact]
        public async Task UpdateChatMessageAsync_ValidRequest_CallsPutAndReturnsMessage()
        {
            // Arrange
            var messageId = "msg_to_update"; // For URL
            var request = new CommentPatchChatMessageRequest(
                Content: "Updated text",
                Assignee: null,
                GroupAssignee: null,
                ContentFormat: null,
                PostData: null,
                Resolved: null,
                ParentMessageId: null
            );
            var updatedMessage = CreateSampleChatMessage(messageId, "ch_irrelevant", "ws_irrelevant") with { TextContent = request.Content }; // Use TextContent for assertion

            _mockApiConnection
                .Setup(c => c.PutAsync<CommentPatchChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(
                    $"/v3/chat/messages/{messageId}",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateV3DataResponse(updatedMessage));

            // Act
            var result = await _chatService.UpdateChatMessageAsync("ws_ignored", messageId, request); // workspaceId here is ignored by service method

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedMessage.TextContent, result.TextContent); // Use TextContent for assertion
            _mockApiConnection.Verify(c => c.PutAsync<CommentPatchChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(
                $"/v3/chat/messages/{messageId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateChatMessageAsync_ApiError_ThrowsHttpRequestException()
        {
            var messageId = "msg_update_err";
            var request = new CommentPatchChatMessageRequest(Content: "Error update", Assignee: null, GroupAssignee: null, ContentFormat: null, PostData: null, Resolved: null, ParentMessageId: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<CommentPatchChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.UpdateChatMessageAsync("ws_ignored", messageId, request));
        }

        [Fact]
        public async Task UpdateChatMessageAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var messageId = "msg_update_null";
            var request = new CommentPatchChatMessageRequest(Content: "Null update", Assignee: null, GroupAssignee: null, ContentFormat: null, PostData: null, Resolved: null, ParentMessageId: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<CommentPatchChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataResponse<ChatMessage>?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.UpdateChatMessageAsync("ws_ignored", messageId, request));
        }

        [Fact]
        public async Task UpdateChatMessageAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            var messageId = "msg_update_null_data";
            var request = new CommentPatchChatMessageRequest(Content: "Null data update", Assignee: null, GroupAssignee: null, ContentFormat: null, PostData: null, Resolved: null, ParentMessageId: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<CommentPatchChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<ChatMessage> { Data = null });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.UpdateChatMessageAsync("ws_ignored", messageId, request));
        }

        [Fact]
        public async Task UpdateChatMessageAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var messageId = "msg_update_cancel";
            var request = new CommentPatchChatMessageRequest(Content: "Cancel update", Assignee: null, GroupAssignee: null, ContentFormat: null, PostData: null, Resolved: null, ParentMessageId: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<CommentPatchChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.UpdateChatMessageAsync("ws_ignored", messageId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task UpdateChatMessageAsync_PassesCancellationTokenToApiConnection()
        {
            var messageId = "msg_update_ct";
            var request = new CommentPatchChatMessageRequest(Content: "CT update", Assignee: null, GroupAssignee: null, ContentFormat: null, PostData: null, Resolved: null, ParentMessageId: null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var updatedMessage = CreateSampleChatMessage(messageId, "ch_irrelevant", "ws_irrelevant");
            _mockApiConnection
                .Setup(c => c.PutAsync<CommentPatchChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(CreateV3DataResponse(updatedMessage));

            await _chatService.UpdateChatMessageAsync("ws_ignored", messageId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PutAsync<CommentPatchChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(
                $"/v3/chat/messages/{messageId}",
                request,
                expectedToken), Times.Once);
        }

        // --- DeleteChatMessageAsync Tests ---
        [Fact]
        public async Task DeleteChatMessageAsync_ValidRequest_CallsDelete()
        {
            // Arrange
            var messageId = "msg_to_delete";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(
                    $"/v3/chat/messages/{messageId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _chatService.DeleteChatMessageAsync("ws_ignored", messageId);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"/v3/chat/messages/{messageId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteChatMessageAsync_ApiError_ThrowsHttpRequestException()
        {
            var messageId = "msg_delete_err";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.DeleteChatMessageAsync("ws_ignored", messageId));
        }

        [Fact]
        public async Task DeleteChatMessageAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var messageId = "msg_delete_cancel";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.DeleteChatMessageAsync("ws_ignored", messageId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task DeleteChatMessageAsync_PassesCancellationTokenToApiConnection()
        {
            var messageId = "msg_delete_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), expectedToken))
                .Returns(Task.CompletedTask);

            await _chatService.DeleteChatMessageAsync("ws_ignored", messageId, expectedToken);

            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"/v3/chat/messages/{messageId}",
                expectedToken), Times.Once);
        }

        // --- CreateReplyMessageAsync Tests ---
        [Fact]
        public async Task CreateReplyMessageAsync_ValidRequest_CallsPostAndReturnsMessage()
        {
            // Arrange
            var parentMessageId = "parent_msg_1";
            var request = new CommentCreateChatMessageRequest(
                Content: "This is a reply", ParentMessageId: parentMessageId, Type: "message",
                Assignee: null, GroupAssignee: null, TriagedAction: null, TriagedObjectId: null, TriagedObjectType: null, Reactions: null, Followers: null, ContentFormat: null, PostData: null
            );
            var expectedReplyMessage = CreateSampleChatMessage("reply_msg_1", "ch_irrelevant", "ws_irrelevant");

            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(
                    $"/v3/chat/messages/{parentMessageId}/messages",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateV3DataResponse(expectedReplyMessage));

            // Act
            var result = await _chatService.CreateReplyMessageAsync("ws_ignored", parentMessageId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedReplyMessage.Id, result.Id);
            _mockApiConnection.Verify(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(
                $"/v3/chat/messages/{parentMessageId}/messages",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateReplyMessageAsync_ApiError_ThrowsHttpRequestException()
        {
            var parentMessageId = "parent_reply_err";
            var request = new CommentCreateChatMessageRequest(null, null, null, null, null, "message", "Error reply", null, null, null, null, parentMessageId);
            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.CreateReplyMessageAsync("ws_ignored", parentMessageId, request));
        }

        [Fact]
        public async Task CreateReplyMessageAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var parentMessageId = "parent_reply_null";
            var request = new CommentCreateChatMessageRequest(null, null, null, null, null, "message", "Null reply", null, null, null, null, parentMessageId);
            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataResponse<ChatMessage>?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateReplyMessageAsync("ws_ignored", parentMessageId, request));
        }

        [Fact]
        public async Task CreateReplyMessageAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            var parentMessageId = "parent_reply_null_data";
            var request = new CommentCreateChatMessageRequest(null, null, null, null, null, "message", "Null data reply", null, null, null, null, parentMessageId);
            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<ChatMessage> { Data = null });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateReplyMessageAsync("ws_ignored", parentMessageId, request));
        }

        [Fact]
        public async Task CreateReplyMessageAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var parentMessageId = "parent_reply_cancel";
            var request = new CommentCreateChatMessageRequest(null, null, null, null, null, "message", "Cancel reply", null, null, null, null, parentMessageId);
            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.CreateReplyMessageAsync("ws_ignored", parentMessageId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreateReplyMessageAsync_PassesCancellationTokenToApiConnection()
        {
            var parentMessageId = "parent_reply_ct";
            var request = new CommentCreateChatMessageRequest(null, null, null, null, null, "message", "CT reply", null, null, null, null, parentMessageId);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedReplyMessage = CreateSampleChatMessage("reply_ct_1", "ch_irrelevant", "ws_irrelevant");
            _mockApiConnection
                .Setup(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(CreateV3DataResponse(expectedReplyMessage));

            await _chatService.CreateReplyMessageAsync("ws_ignored", parentMessageId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(
                $"/v3/chat/messages/{parentMessageId}/messages",
                request,
                expectedToken), Times.Once);
        }

        // --- GetChatMessageRepliesAsync Tests ---
        [Fact]
        public async Task GetChatMessageRepliesAsync_ValidRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var parentMessageId = "parent_msg_replies";
            var cursor = "reply_cursor";
            var limit = 10;
            var contentFormat = "text";
            var sampleReply = CreateSampleChatMessage("reply_1", "ch_irrelevant", "ws_irrelevant");
            var expectedResponse = new GetChatMessagesResponse(
                Data: new List<ChatMessage> { sampleReply },
                Meta: new GetChatMessagesResponse.ResponseMeta(NextCursor: "next_reply_cursor", PreviousCursor: null)
            );

            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(
                    $"/v3/chat/messages/{parentMessageId}/messages?cursor={cursor}&limit={limit}&content_format={contentFormat}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatMessageRepliesAsync("ws_ignored", parentMessageId, cursor, limit, contentFormat);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.Equal(expectedResponse.Meta.NextCursor, result.Meta.NextCursor);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatMessagesResponse>(
                $"/v3/chat/messages/{parentMessageId}/messages?cursor={cursor}&limit={limit}&content_format={contentFormat}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatMessageRepliesAsync_MinimalRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var parentMessageId = "parent_msg_replies_min";
            var expectedResponse = new GetChatMessagesResponse(new List<ChatMessage>(), new GetChatMessagesResponse.ResponseMeta(null, null));
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(
                    $"/v3/chat/messages/{parentMessageId}/messages",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatMessageRepliesAsync("ws_ignored", parentMessageId);

            // Assert
            Assert.NotNull(result);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatMessagesResponse>(
                $"/v3/chat/messages/{parentMessageId}/messages",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatMessageRepliesAsync_ApiError_ThrowsHttpRequestException()
        {
            var parentMessageId = "parent_replies_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.GetChatMessageRepliesAsync("ws_ignored", parentMessageId));
        }

        [Fact]
        public async Task GetChatMessageRepliesAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var parentMessageId = "parent_replies_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetChatMessagesResponse?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.GetChatMessageRepliesAsync("ws_ignored", parentMessageId));
        }

        [Fact]
        public async Task GetChatMessageRepliesAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var parentMessageId = "parent_replies_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.GetChatMessageRepliesAsync("ws_ignored", parentMessageId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetChatMessageRepliesAsync_PassesCancellationTokenToApiConnection()
        {
            var parentMessageId = "parent_replies_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetChatMessagesResponse(new List<ChatMessage>(), new GetChatMessagesResponse.ResponseMeta(null, null));
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatMessagesResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _chatService.GetChatMessageRepliesAsync("ws_ignored", parentMessageId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<GetChatMessagesResponse>(
                $"/v3/chat/messages/{parentMessageId}/messages",
                expectedToken), Times.Once);
        }

        // --- GetChatMessageReactionsAsync Tests ---
        [Fact]
        public async Task GetChatMessageReactionsAsync_ValidRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var messageId = "msg_reactions";
            var cursor = "reaction_cursor";
            var limit = 5;
            var sampleReaction = new ChatReaction(DateTimeOffset.UtcNow, "👍", "user_id_string");
            var expectedResponse = new GetChatReactionsResponse(
                Data: new List<ChatReaction> { sampleReaction },
                Meta: new GetChatReactionsResponse.ResponseMeta(NextCursor: "next_reaction_cursor", PreviousCursor: null)
            );

            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatReactionsResponse>(
                    $"/v3/chat/messages/{messageId}/reactions?cursor={cursor}&limit={limit}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatMessageReactionsAsync("ws_ignored", messageId, cursor, limit);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.Equal(expectedResponse.Meta.NextCursor, result.Meta.NextCursor);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatReactionsResponse>(
                $"/v3/chat/messages/{messageId}/reactions?cursor={cursor}&limit={limit}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatMessageReactionsAsync_MinimalRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var messageId = "msg_reactions_min";
            var expectedResponse = new GetChatReactionsResponse(new List<ChatReaction>(), new GetChatReactionsResponse.ResponseMeta(null, null));
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatReactionsResponse>(
                    $"/v3/chat/messages/{messageId}/reactions",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatMessageReactionsAsync("ws_ignored", messageId);

            // Assert
            Assert.NotNull(result);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatReactionsResponse>(
                $"/v3/chat/messages/{messageId}/reactions",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatMessageReactionsAsync_ApiError_ThrowsHttpRequestException()
        {
            var messageId = "msg_reactions_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatReactionsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.GetChatMessageReactionsAsync("ws_ignored", messageId));
        }

        [Fact]
        public async Task GetChatMessageReactionsAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var messageId = "msg_reactions_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatReactionsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetChatReactionsResponse?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.GetChatMessageReactionsAsync("ws_ignored", messageId));
        }

        [Fact]
        public async Task GetChatMessageReactionsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var messageId = "msg_reactions_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatReactionsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.GetChatMessageReactionsAsync("ws_ignored", messageId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetChatMessageReactionsAsync_PassesCancellationTokenToApiConnection()
        {
            var messageId = "msg_reactions_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetChatReactionsResponse(new List<ChatReaction>(), new GetChatReactionsResponse.ResponseMeta(null, null));
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatReactionsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _chatService.GetChatMessageReactionsAsync("ws_ignored", messageId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<GetChatReactionsResponse>(
                $"/v3/chat/messages/{messageId}/reactions",
                expectedToken), Times.Once);
        }

        // --- CreateChatReactionAsync Tests ---
        [Fact]
        public async Task CreateChatReactionAsync_ValidRequest_CallsPostAndReturnsReaction()
        {
            // Arrange
            var messageId = "msg_create_reaction";
            var request = new CreateChatReactionRequest { Reaction = "🚀", UserId = 1 }; // Assuming UserId is int
            var expectedReaction = new ChatReaction(DateTimeOffset.UtcNow, request.Reaction, request.UserId.ToString());

            _mockApiConnection
                .Setup(c => c.PostAsync<CreateChatReactionRequest, ClickUpV3DataResponse<ChatReaction>>(
                    $"/v3/chat/messages/{messageId}/reactions",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateV3DataResponse(expectedReaction));

            // Act
            var result = await _chatService.CreateChatReactionAsync("ws_ignored", messageId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedReaction.Reaction, result.Reaction); // DTO uses Reaction, not Emoji
            _mockApiConnection.Verify(c => c.PostAsync<CreateChatReactionRequest, ClickUpV3DataResponse<ChatReaction>>(
                $"/v3/chat/messages/{messageId}/reactions",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateChatReactionAsync_ApiError_ThrowsHttpRequestException()
        {
            var messageId = "msg_react_err";
            var request = new CreateChatReactionRequest { Reaction = "👎", UserId = 2 };
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateChatReactionRequest, ClickUpV3DataResponse<ChatReaction>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.CreateChatReactionAsync("ws_ignored", messageId, request));
        }

        [Fact]
        public async Task CreateChatReactionAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var messageId = "msg_react_null";
            var request = new CreateChatReactionRequest { Reaction = "😀", UserId = 3 };
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateChatReactionRequest, ClickUpV3DataResponse<ChatReaction>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpV3DataResponse<ChatReaction>?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateChatReactionAsync("ws_ignored", messageId, request));
        }

        [Fact]
        public async Task CreateChatReactionAsync_NullDataInResponse_ThrowsInvalidOperationException()
        {
            var messageId = "msg_react_null_data";
            var request = new CreateChatReactionRequest { Reaction = "😂", UserId = 4 };
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateChatReactionRequest, ClickUpV3DataResponse<ChatReaction>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickUpV3DataResponse<ChatReaction> { Data = null });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.CreateChatReactionAsync("ws_ignored", messageId, request));
        }

        [Fact]
        public async Task CreateChatReactionAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var messageId = "msg_react_cancel";
            var request = new CreateChatReactionRequest { Reaction = "🤔", UserId = 5 };
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateChatReactionRequest, ClickUpV3DataResponse<ChatReaction>>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.CreateChatReactionAsync("ws_ignored", messageId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreateChatReactionAsync_PassesCancellationTokenToApiConnection()
        {
            var messageId = "msg_react_ct";
            var request = new CreateChatReactionRequest { Reaction = "💡", UserId = 6 };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedReaction = new ChatReaction(DateTimeOffset.UtcNow, request.Reaction, request.UserId.ToString());
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateChatReactionRequest, ClickUpV3DataResponse<ChatReaction>>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(CreateV3DataResponse(expectedReaction));

            await _chatService.CreateChatReactionAsync("ws_ignored", messageId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<CreateChatReactionRequest, ClickUpV3DataResponse<ChatReaction>>(
                $"/v3/chat/messages/{messageId}/reactions",
                request,
                expectedToken), Times.Once);
        }

        // --- DeleteChatReactionAsync Tests ---
        [Fact]
        public async Task DeleteChatReactionAsync_ValidRequest_CallsDelete()
        {
            // Arrange
            var messageId = "msg_delete_reaction";
            var reactionEmoji = "👍";
            var encodedReaction = Uri.EscapeDataString(reactionEmoji);

            _mockApiConnection
                .Setup(c => c.DeleteAsync(
                    $"/v3/chat/messages/{messageId}/reactions/{encodedReaction}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _chatService.DeleteChatReactionAsync("ws_ignored", messageId, reactionEmoji);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"/v3/chat/messages/{messageId}/reactions/{encodedReaction}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteChatReactionAsync_ApiError_ThrowsHttpRequestException()
        {
            var messageId = "msg_del_react_err";
            var reactionEmoji = "🎉";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.DeleteChatReactionAsync("ws_ignored", messageId, reactionEmoji));
        }

        [Fact]
        public async Task DeleteChatReactionAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var messageId = "msg_del_react_cancel";
            var reactionEmoji = "❤️";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.DeleteChatReactionAsync("ws_ignored", messageId, reactionEmoji, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task DeleteChatReactionAsync_PassesCancellationTokenToApiConnection()
        {
            var messageId = "msg_del_react_ct";
            var reactionEmoji = "⭐";
            var encodedReaction = Uri.EscapeDataString(reactionEmoji);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), expectedToken))
                .Returns(Task.CompletedTask);

            await _chatService.DeleteChatReactionAsync("ws_ignored", messageId, reactionEmoji, expectedToken);

            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"/v3/chat/messages/{messageId}/reactions/{encodedReaction}",
                expectedToken), Times.Once);
        }

        // --- GetChatMessageTaggedUsersAsync Tests ---
        [Fact]
        public async Task GetChatMessageTaggedUsersAsync_ValidRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var messageId = "msg_tagged_users";
            var cursor = "tagged_cursor";
            var limit = 7;
            var sampleUser = CreateSampleChatSimpleUser("tagged_user_1");
            var expectedResponse = new GetChatUsersResponse(
                Data: new List<ChatSimpleUser> { sampleUser },
                Meta: new GetChatUsersResponse.ResponseMeta(NextCursor: "next_tagged_cursor", PreviousCursor: null)
            );

            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(
                    $"/v3/chat/messages/{messageId}/tagged_users?cursor={cursor}&limit={limit}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatMessageTaggedUsersAsync("ws_ignored", messageId, cursor, limit);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.Equal(expectedResponse.Meta.NextCursor, result.Meta.NextCursor);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatUsersResponse>(
                $"/v3/chat/messages/{messageId}/tagged_users?cursor={cursor}&limit={limit}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatMessageTaggedUsersAsync_MinimalRequest_BuildsUrlAndReturnsResponse()
        {
            // Arrange
            var messageId = "msg_tagged_users_min";
            var expectedResponse = new GetChatUsersResponse(new List<ChatSimpleUser>(), new GetChatUsersResponse.ResponseMeta(null, null));
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(
                    $"/v3/chat/messages/{messageId}/tagged_users",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatMessageTaggedUsersAsync("ws_ignored", messageId);

            // Assert
            Assert.NotNull(result);
            _mockApiConnection.Verify(c => c.GetAsync<GetChatUsersResponse>(
                $"/v3/chat/messages/{messageId}/tagged_users",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetChatMessageTaggedUsersAsync_ApiError_ThrowsHttpRequestException()
        {
            var messageId = "msg_tagged_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.GetChatMessageTaggedUsersAsync("ws_ignored", messageId));
        }

        [Fact]
        public async Task GetChatMessageTaggedUsersAsync_NullResponse_ThrowsInvalidOperationException()
        {
            var messageId = "msg_tagged_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetChatUsersResponse?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _chatService.GetChatMessageTaggedUsersAsync("ws_ignored", messageId));
        }

        [Fact]
        public async Task GetChatMessageTaggedUsersAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var messageId = "msg_tagged_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _chatService.GetChatMessageTaggedUsersAsync("ws_ignored", messageId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetChatMessageTaggedUsersAsync_PassesCancellationTokenToApiConnection()
        {
            var messageId = "msg_tagged_ct";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetChatUsersResponse(new List<ChatSimpleUser>(), new GetChatUsersResponse.ResponseMeta(null, null));
            _mockApiConnection
                .Setup(c => c.GetAsync<GetChatUsersResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _chatService.GetChatMessageTaggedUsersAsync("ws_ignored", messageId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<GetChatUsersResponse>(
                $"/v3/chat/messages/{messageId}/tagged_users",
                expectedToken), Times.Once);
        }
    }
}
