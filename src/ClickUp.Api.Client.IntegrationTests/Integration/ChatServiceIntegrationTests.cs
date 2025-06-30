using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Chat;
using ClickUp.Api.Client.Models.Entities.Chat;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure; // For TestMode & base class
using ClickUp.Api.Client.Models.Exceptions; // For ClickUpApiException
using RichardSzalay.MockHttp; // For MockHttpHandler extension methods like .When()

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class ChatServiceIntegrationTests : IntegrationTestBase
    {
        private readonly IChatService _chatService;
        private readonly ITestOutputHelper _output;
        private readonly string _workspaceId;

        // Hold onto created channel ID for cleanup or subsequent tests if needed
        private string? _createdChannelIdForCleanup;

        public ChatServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _chatService = ServiceProvider.GetRequiredService<IChatService>();
            _workspaceId = ClientOptions.TestWorkspaceId!;

            if (CurrentTestMode != TestMode.Playback && string.IsNullOrWhiteSpace(_workspaceId))
            {
                throw new InvalidOperationException("TestWorkspaceId is not configured. Required for ChatService tests in Record/Passthrough mode.");
            }
            _output.LogInformation($"Running ChatServiceTests in {CurrentTestMode} mode for workspace: {_workspaceId}");
        }

        [Fact]
        public async Task GetChatChannelsAsync_ShouldReturnChannels_WhenCalled()
        {
            // Arrange
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // Path for v3: /v3/workspaces/{workspaceId}/channels
                // RecordingDelegatingHandler needs to correctly parse this.
                // Expected: ChatService/GETChannels/Success.json (or similar)
                var responsePath = Path.Combine(RecordedResponsesBasePath, "ChatService", "GETChannels", "Success.json");
                _output.LogInformation($"[Playback] Attempting to use response file: {responsePath}");
                Assert.True(File.Exists(responsePath), $"Playback file not found: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                // The port might be an issue if BaseAddress in options includes it.
                // Standard ClickUp API is on 443 (HTTPS).
                MockHttpHandler.When(HttpMethod.Get, $"https{"://"}api.clickup.com/v3/workspaces/{_workspaceId}/channels")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }

            // Act
            var channelsResponse = await _chatService.GetChatChannelsAsync(_workspaceId, new GetChatChannelsRequest());

            // Assert
            Assert.NotNull(channelsResponse);
            Assert.NotNull(channelsResponse.Data); // Data is List<ChatChannel>
            Assert.NotEmpty(channelsResponse.Data); // Access Data directly as the list
            _output.LogInformation($"Successfully fetched {channelsResponse.Data.Count} chat channels.");
            foreach (var channel in channelsResponse.Data)
            {
                Assert.False(string.IsNullOrWhiteSpace(channel.Id));
                Assert.False(string.IsNullOrWhiteSpace(channel.Name));
                _output.LogInformation($"  Channel ID: {channel.Id}, Name: {channel.Name}");
            }

            if (CurrentTestMode == TestMode.Record)
            {
                _output.LogInformation($"[Record] Ensure response for GET /v3/workspaces/{_workspaceId}/channels is saved as 'ChatService/GETChannels/Success.json'.");
            }
        }

        [Fact]
        public async Task CreateChatChannelAsync_ShouldCreateChannel_WhenValidDataProvided()
        {
            // Arrange
            var newChannelName = $"Test Channel {Guid.NewGuid().ToString().Substring(0, 8)}";
            var createRequest = new ChatCreateChatChannelRequest(
                Description: "A test channel created by integration tests.",
                Name: newChannelName,
                Topic: null, // Optional, can be null
                UserIds: new() { /* Add user IDs if needed, or make them truly optional by passing null if your model supports it */ },
                Visibility: ClickUp.Api.Client.Models.Entities.Chat.Enums.ChatRoomVisibility.Public, // Use Visibility
                WorkspaceId: _workspaceId
            );

            string expectedCreatedChannelId = "playback_created_channel_id_123"; // Placeholder for playback

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // Path for v3: POST /v3/workspaces/{workspaceId}/channels
                var responsePath = Path.Combine(RecordedResponsesBasePath, "ChatService", "POSTChannels", "CreateChannel_Success.json");
                _output.LogInformation($"[Playback] Attempting to use response file: {responsePath}");
                Assert.True(File.Exists(responsePath), $"Playback file not found: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                // In playback, the response should contain the placeholder ID.
                // The request body matching can be tricky if dynamic data (like GUID in name) is used.
                // MockHttp allows matching any content: .WithContent("*") or specific JSON properties.
                // For simplicity, we'll match the URL and method.
                MockHttpHandler.When(HttpMethod.Post, $"https{"://"}api.clickup.com/v3/workspaces/{_workspaceId}/channels")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }

            // Act
            ChatChannel createdChannel = null;
            try
            {
                createdChannel = await _chatService.CreateChatChannelAsync(_workspaceId, createRequest);
                _createdChannelIdForCleanup = createdChannel?.Id; // Save for potential cleanup
            }
            catch (Exception ex)
            {
                _output.LogError($"Error during CreateChatChannelAsync: {ex}");
                throw;
            }


            // Assert
            Assert.NotNull(createdChannel);
            Assert.False(string.IsNullOrWhiteSpace(createdChannel.Id));
            Assert.Equal(newChannelName, createdChannel.Name); // Name should match
            _output.LogInformation($"Successfully created channel: {createdChannel.Name} (ID: {createdChannel.Id})");

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal(expectedCreatedChannelId, createdChannel.Id, ignoreCase: true); // Ensure playback ID matches
            }

            if (CurrentTestMode == TestMode.Record)
            {
                _output.LogInformation($"[Record] Ensure response for POST /v3/workspaces/{_workspaceId}/channels is saved as 'ChatService/POSTChannels/CreateChannel_Success.json'. " +
                                       $"Manually edit the 'id' in the saved JSON to '{expectedCreatedChannelId}' for consistent playback testing if needed.");
            }

            // Basic Teardown: Delete the created channel if in Record or Passthrough mode
            // In Playback mode, no actual deletion happens.
            if (CurrentTestMode != TestMode.Playback && !string.IsNullOrWhiteSpace(_createdChannelIdForCleanup))
            {
                _output.LogInformation($"Attempting to cleanup (delete) created channel: {_createdChannelIdForCleanup}");
                try
                {
                    await _chatService.DeleteChatChannelAsync(_workspaceId, _createdChannelIdForCleanup);
                    _output.LogInformation($"Successfully deleted channel: {_createdChannelIdForCleanup}");
                }
                catch (Exception ex)
                {
                    // Don't fail the test for cleanup failure, but log it.
                    _output.LogWarning($"Could not delete channel {_createdChannelIdForCleanup} during cleanup: {ex.Message}");
                }
            }
        }

        // TODO: Add tests for GetChatMessagesAsync, CreateChatMessageAsync, etc.
        // These would typically require a channel to be created first.
        // The CreateChatChannelAsync test already includes a basic cleanup.
        // For more complex scenarios, consider using IAsyncLifetime for setup/teardown of a test channel.

        // Example for GetChatMessagesAsync (conceptual)
        // [Fact]
        // public async Task GetChatMessagesAsync_ShouldReturnMessages_ForValidChannel()
        // {
        //     // Arrange: Ensure a channel exists (either pre-configured ID or created via IAsyncLifetime)
        //     string testChannelId = "some_known_channel_id_or_created_id";
        //     // ... setup playback mocks for GetChatMessages ...
        //
        //     // Act
        //     var messagesResponse = await _chatService.GetChatMessagesAsync(_workspaceId, testChannelId);
        //
        //     // Assert
        //     Assert.NotNull(messagesResponse);
        //     // ... further assertions ...
        // }

        // Teardown for any resources created if not handled per-test
        // This Dispose method might not be strictly necessary if cleanup is per-test
        // or if IAsyncLifetime is used for class-level setup/teardown.
        // public override void Dispose()
        // {
        //     if (CurrentTestMode != TestMode.Playback && !string.IsNullOrWhiteSpace(_createdChannelIdForCleanup))
        //     {
        //         _output.LogInformation($"Dispose: Attempting to cleanup created channel: {_createdChannelIdForCleanup}");
        //         _chatService.DeleteChatChannelAsync(_workspaceId, _createdChannelIdForCleanup)
        //             .GetAwaiter().GetResult(); // Synchronous for Dispose, or make Dispose async
        //     }
        //     base.Dispose();
        // }
    }
}
