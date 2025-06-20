using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities.Chat;
using ClickUp.Api.Client.Models.RequestModels.Chat;
using ClickUp.Api.Client.Models.ResponseModels.Chat;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ClickUp.Api.Client.Tests.Services
{
    public class ChatServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly ChatService _chatService;
        private const string BaseV3WorkspaceEndpoint = "/v3/workspaces";

        public ChatServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _chatService = new ChatService(_mockApiConnection.Object);
        }

        // Placeholder DTO creation helpers
        private ChatChannel CreateSampleChatChannel(string id, string name)
        {
            var channel = (ChatChannel)Activator.CreateInstance(typeof(ChatChannel), nonPublic: true)!;
            var props = typeof(ChatChannel).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(channel, id);
            props.FirstOrDefault(p => p.Name == "Name")?.SetValue(channel, name);
            // Set other required properties for ChatChannel if any
            return channel;
        }

        private ChatMessage CreateSampleChatMessage(string id, string messageText)
        {
            var message = (ChatMessage)Activator.CreateInstance(typeof(ChatMessage), nonPublic: true)!;
             var props = typeof(ChatMessage).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(message, id);
            props.FirstOrDefault(p => p.Name == "Message")?.SetValue(message, messageText); // Assuming "Message" property for text
            return message;
        }

        // Conceptual V3 response wrapper
        private ClickUpV3ChatDataResponse<T> CreateV3ChatDataResponse<T>(T data)
        {
            var responseType = typeof(ClickUpV3ChatDataResponse<>).MakeGenericType(typeof(T));
            var responseInstance = Activator.CreateInstance(responseType)!;
            responseType.GetProperty("Data")?.SetValue(responseInstance, data);
            return (ClickUpV3ChatDataResponse<T>)responseInstance;
        }


        [Fact]
        public async Task GetChatChannelsAsync_WhenChannelsExist_ReturnsChannelsResponse()
        {
            // Arrange
            var workspaceId = "ws-id";
            var expectedResponse = new GetChatChannelsResponse(
                new List<ChatChannel> { CreateSampleChatChannel("chan1", "Channel 1") },
                "next-cursor"
            );
            var expectedEndpoint = $"{BaseV3WorkspaceEndpoint}/{workspaceId}/channels"; // Basic, no optional params

            _mockApiConnection.Setup(c => c.GetAsync<GetChatChannelsResponse>(
                It.Is<string>(s => s.StartsWith(expectedEndpoint)),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatChannelsAsync(workspaceId, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task CreateChatChannelAsync_ValidRequest_ReturnsCreatedChannel()
        {
            // Arrange
            var workspaceId = "ws-id";
            var requestDto = new CreateChatChannelRequest("New Channel", false, null);
            var sampleChannel = CreateSampleChatChannel("new-chan-id", "New Channel");
            var expectedResponse = CreateV3ChatDataResponse(sampleChannel);


            _mockApiConnection.Setup(c => c.PostAsync<CreateChatChannelRequest, ClickUpV3ChatDataResponse<ChatChannel>>(
                $"{BaseV3WorkspaceEndpoint}/{workspaceId}/channels",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.CreateChatChannelAsync(workspaceId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleChannel);
        }

        [Fact]
        public async Task GetChatMessagesAsync_ValidChannel_ReturnsMessagesResponse()
        {
            // Arrange
            var workspaceId = "ws-id";
            var channelId = "chan-id";
            var expectedResponse = new GetChatMessagesResponse(
                new List<ChatMessage> { CreateSampleChatMessage("msg1", "Hello") },
                "next-msg-cursor"
            );
            var expectedEndpoint = $"{BaseV3WorkspaceEndpoint}/{workspaceId}/channels/{channelId}/messages";

            _mockApiConnection.Setup(c => c.GetAsync<GetChatMessagesResponse>(
                It.Is<string>(s => s.StartsWith(expectedEndpoint)),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.GetChatMessagesAsync(workspaceId, channelId, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task CreateChatMessageAsync_ValidRequest_ReturnsPostedMessage() // Renamed from PostChatMessageAsync
        {
            // Arrange
            var workspaceId = "ws-id";
            var channelId = "chan-id";
            var requestDto = new CreateChatMessageRequest("Test message");
            var sampleMessage = CreateSampleChatMessage("new-msg-id", "Test message");
            var expectedResponse = CreateV3ChatDataResponse(sampleMessage);

            _mockApiConnection.Setup(c => c.PostAsync<CreateChatMessageRequest, ClickUpV3ChatDataResponse<ChatMessage>>(
                $"{BaseV3WorkspaceEndpoint}/{workspaceId}/channels/{channelId}/messages",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatService.CreateChatMessageAsync(workspaceId, channelId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleMessage);
        }

        [Fact]
        public async Task DeleteChatMessageAsync_ValidIds_CallsApiConnectionDeleteAsync()
        {
            // Arrange
            var workspaceId = "ws-id"; // Not used in this specific endpoint path
            var messageId = "msg-to-delete";
            // Corrected endpoint based on ChatService implementation for DeleteChatMessageAsync
            var expectedEndpoint = $"/v3/chat/messages/{messageId}";

            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _chatService.DeleteChatMessageAsync(workspaceId, messageId, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    // Minimal internal helper DTO for conceptual V3 data wrapping, if not defined in main models.
    internal class ClickUpV3ChatDataResponse<T> { public T? Data { get; set; } }
}
