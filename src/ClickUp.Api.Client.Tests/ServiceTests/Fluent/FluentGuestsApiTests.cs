using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Guests;
using ClickUp.Api.Client.Models.Entities.Users;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentGuestsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentGuestsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentGuestsApi_GetGuestAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var guestId = "testGuestId";
        var mockGuestUserInfo = new GuestUserInfo
        {
            Id = 1,
            Username = "testguest",
            Email = "test@example.com",
            Color = "#000000",
            ProfilePicture = null,
            Initials = "TG",
            Role = 0, // Assuming a default role
            CustomRole = null,
            LastActive = null,
            DateJoined = null,
            DateInvited = DateTimeOffset.UtcNow // Assuming a default date
        };
        var mockGuestSharingDetails = new Client.Models.Entities.Users.GuestSharingDetails
        {
            Tasks = new List<string>(),
            Lists = new List<string>(),
            Folders = new List<string>()
        };
        var expectedGuest = new Guest
        {
            User = mockGuestUserInfo,
            Shared = mockGuestSharingDetails
        };
        var expectedResponse = new GetGuestResponse { Guest = expectedGuest };

        var mockGuestsService = new Mock<IGuestsService>();
        mockGuestsService.Setup(x => x.GetGuestAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentGuestsApi = new GuestsFluentApi(mockGuestsService.Object);

        // Act
        var result = await fluentGuestsApi.GetGuestAsync(workspaceId, guestId);

        // Assert
        Assert.Equal(expectedResponse, result);
        mockGuestsService.Verify(x => x.GetGuestAsync(
            workspaceId,
            guestId,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
