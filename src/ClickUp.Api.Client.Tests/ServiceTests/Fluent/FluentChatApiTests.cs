using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Chat;
using ClickUp.Api.Client.Models.Entities.Chat;
using ClickUp.Api.Client.Models.RequestModels.Chat; // Added for GetChatChannelsRequest
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent
{
    public class FluentChatApiTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly ClickUpClient _client;

        public FluentChatApiTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();
            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(Mock.Of<ILogger>());
            _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
        }

        [Fact]
        public async Task FluentChatApi_GetChatChannels_ShouldBuildCorrectRequestAndCallService()
        {
            // Arrange
            var workspaceId = "testWorkspaceId";
            var expectedResponse = new ChatChannelPaginatedResponse(null, new List<ChatChannel>());

            var mockChatService = new Mock<IChatService>();
            mockChatService.Setup(x => x.GetChatChannelsAsync(
                It.IsAny<string>(),
                It.IsAny<GetChatChannelsRequest>(), // Expect DTO
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var fluentChatApi = new ChatFluentApi(mockChatService.Object);

            // Act
            var result = await fluentChatApi.GetChatChannels(workspaceId)
                .WithLimit(10)
                .GetAsync();

            // Assert
            Assert.Equal(expectedResponse, result);
            mockChatService.Verify(x => x.GetChatChannelsAsync(
                workspaceId,
                It.Is<GetChatChannelsRequest>(r => r.Limit == 10 && r.Cursor == null), // Verify DTO content
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
