using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.RequestModels.Chat;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent
{
    public class ChatChannelFluentQueryRequestTests
    {
        private readonly Mock<IChatService> _mockChatService = new();
        private const string WorkspaceId = "test_workspace";

        [Fact]
        public void WithDescriptionFormat_SetsProperty()
        {
            var request = new ChatChannelFluentQueryRequest(WorkspaceId, _mockChatService.Object)
                .WithDescriptionFormat("html");
            // Use reflection to check _request.DescriptionFormat
            var field = typeof(ChatChannelFluentQueryRequest).GetField("_request", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(field);
            var req = (GetChatChannelsRequest?)field.GetValue(request);
            Assert.NotNull(req);
            Assert.Equal("html", req.DescriptionFormat);
        }

        [Fact]
        public void WithCursor_SetsProperty()
        {
            var request = new ChatChannelFluentQueryRequest(WorkspaceId, _mockChatService.Object)
                .WithCursor("cursor123");
            var field = typeof(ChatChannelFluentQueryRequest).GetField("_request", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(field);
            var req = (GetChatChannelsRequest?)field.GetValue(request);
            Assert.NotNull(req);
            Assert.Equal("cursor123", req.Cursor);
        }

        [Fact]
        public void WithLimit_SetsProperty()
        {
            var request = new ChatChannelFluentQueryRequest(WorkspaceId, _mockChatService.Object)
                .WithLimit(42);
            var field = typeof(ChatChannelFluentQueryRequest).GetField("_request", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(field);
            var req = (GetChatChannelsRequest?)field.GetValue(request);
            Assert.NotNull(req);
            Assert.Equal(42, req.Limit);
        }

        [Fact]
        public void WithIsFollower_SetsProperty()
        {
            var request = new ChatChannelFluentQueryRequest(WorkspaceId, _mockChatService.Object)
                .WithIsFollower(true);
            var field = typeof(ChatChannelFluentQueryRequest).GetField("_request", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(field);
            var req = (GetChatChannelsRequest?)field.GetValue(request);
            Assert.NotNull(req);
            Assert.True(req.IsFollower);
        }

        [Fact]
        public void WithIncludeHidden_SetsProperty()
        {
            var request = new ChatChannelFluentQueryRequest(WorkspaceId, _mockChatService.Object)
                .WithIncludeHidden(true);
            var field = typeof(ChatChannelFluentQueryRequest).GetField("_request", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(field);
            var req = (GetChatChannelsRequest?)field.GetValue(request);
            Assert.NotNull(req);
            Assert.True(req.IncludeHidden);
        }

        [Fact]
        public void WithWithCommentSince_SetsProperty()
        {
            var request = new ChatChannelFluentQueryRequest(WorkspaceId, _mockChatService.Object)
                .WithWithCommentSince(1234567890);
            var field = typeof(ChatChannelFluentQueryRequest).GetField("_request", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(field);
            var req = (GetChatChannelsRequest?)field.GetValue(request);
            Assert.NotNull(req);
            Assert.Equal(1234567890, req.WithCommentSince);
        }

        [Fact]
        public void WithRoomTypes_SetsProperty()
        {
            var types = new[] { "public_channel", "private_channel" };
            var request = new ChatChannelFluentQueryRequest(WorkspaceId, _mockChatService.Object)
                .WithRoomTypes(types);
            var field = typeof(ChatChannelFluentQueryRequest).GetField("_request", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var req = (GetChatChannelsRequest)field.GetValue(request);
            Assert.Equal(types, req.RoomTypes);
        }

        [Fact]
        public async Task GetAsync_CallsServiceWithCorrectParameters()
        {
            var expectedResponse = new ClickUp.Api.Client.Models.ResponseModels.Chat.ChatChannelPaginatedResponse(null, new System.Collections.Generic.List<ClickUp.Api.Client.Models.Entities.Chat.ChatChannel>());
            _mockChatService.Setup(x => x.GetChatChannelsAsync(
                WorkspaceId,
                It.IsAny<GetChatChannelsRequest>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            var request = new ChatChannelFluentQueryRequest(WorkspaceId, _mockChatService.Object)
                .WithLimit(5);
            var result = await request.GetAsync();

            Assert.Equal(expectedResponse, result);
            _mockChatService.Verify(x => x.GetChatChannelsAsync(
                WorkspaceId,
                It.Is<GetChatChannelsRequest>(r => r.Limit == 5),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
