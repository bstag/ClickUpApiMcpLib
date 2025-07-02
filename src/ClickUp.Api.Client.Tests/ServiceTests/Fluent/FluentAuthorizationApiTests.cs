using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Authorization;
using ClickUp.Api.Client.Models.Entities.Users;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ClickUp.Api.Client.Models.Entities.WorkSpaces;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentAuthorizationApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentAuthorizationApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentAuthorizationApi_GetAccessToken_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var clientId = "testClientId";
        var clientSecret = "testClientSecret";
        var code = "testCode";
        var expectedResponse = new GetAccessTokenResponse("testAccessToken");

        var mockAuthorizationService = new Mock<IAuthorizationService>();
        mockAuthorizationService.Setup(x => x.GetAccessTokenAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentAuthorizationApi = new AuthorizationFluentApi(mockAuthorizationService.Object);

        // Act
        var result = await fluentAuthorizationApi.GetAccessToken()
            .WithClientId(clientId)
            .WithClientSecret(clientSecret)
            .WithCode(code)
            .GetAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockAuthorizationService.Verify(x => x.GetAccessTokenAsync(
            clientId,
            clientSecret,
            code,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentAuthorizationApi_GetAuthorizedUserAsync_ShouldCallService()
    {
        // Arrange
        var expectedUser = new User(1, "testuser", "test@example.com", "#000000", null, "TU", null);
        var mockAuthorizationService = new Mock<IAuthorizationService>();
        mockAuthorizationService.Setup(x => x.GetAuthorizedUserAsync(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        var fluentAuthorizationApi = new AuthorizationFluentApi(mockAuthorizationService.Object);

        // Act
        var result = await fluentAuthorizationApi.GetAuthorizedUserAsync();

        // Assert
        Assert.Equal(expectedUser, result);
        mockAuthorizationService.Verify(x => x.GetAuthorizedUserAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentAuthorizationApi_GetAuthorizedWorkspacesAsync_ShouldCallService()
    {
        // Arrange
        var expectedWorkspaces = new List<ClickUpWorkspace>();
        var mockAuthorizationService = new Mock<IAuthorizationService>();
        mockAuthorizationService.Setup(x => x.GetAuthorizedWorkspacesAsync(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedWorkspaces);

        var fluentAuthorizationApi = new AuthorizationFluentApi(mockAuthorizationService.Object);

        // Act
        var result = await fluentAuthorizationApi.GetAuthorizedWorkspacesAsync();

        // Assert
        Assert.Equal(expectedWorkspaces, result);
        mockAuthorizationService.Verify(x => x.GetAuthorizedWorkspacesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
