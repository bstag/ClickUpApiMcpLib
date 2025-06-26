using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models; // For ClickUpWorkspace
using ClickUp.Api.Client.Models.Common; // For Member
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Authorization;
using ClickUp.Api.Client.Models.ResponseModels.Authorization;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces; // For GetAuthorizedWorkspacesResponse
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class AuthorizationServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly AuthorizationService _authorizationService;
        private readonly Mock<ILogger<AuthorizationService>> _mockLogger;

        public AuthorizationServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<AuthorizationService>>();
            _authorizationService = new AuthorizationService(_mockApiConnection.Object, _mockLogger.Object);
        }

        // --- Test for GetAccessTokenAsync ---

        [Fact]
        public async Task GetAccessTokenAsync_ValidRequest_ReturnsTokenResponse()
        {
            // Arrange
            var clientId = "test_client_id";
            var clientSecret = "test_client_secret";
            var code = "test_code";
            var expectedResponse = new GetAccessTokenResponse("test_access_token");
            var requestPayload = new GetAccessTokenRequest { ClientId = clientId, ClientSecret = clientSecret, Code = code };

            _mockApiConnection
                .Setup(x => x.PostAsync<GetAccessTokenRequest, GetAccessTokenResponse>(
                    "oauth/token",
                    It.Is<GetAccessTokenRequest>(r => r.ClientId == clientId && r.ClientSecret == clientSecret && r.Code == code),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _authorizationService.GetAccessTokenAsync(clientId, clientSecret, code, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.AccessToken, result.AccessToken);
            _mockApiConnection.Verify(x => x.PostAsync<GetAccessTokenRequest, GetAccessTokenResponse>(
                "oauth/token",
                It.Is<GetAccessTokenRequest>(r => r.ClientId == clientId && r.ClientSecret == clientSecret && r.Code == code),
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetAccessTokenAsync_ApiConnectionReturnsNull_ReturnsNull()
        {
            // Arrange
            _mockApiConnection
                .Setup(x => x.PostAsync<GetAccessTokenRequest, GetAccessTokenResponse>(
                    It.IsAny<string>(),
                    It.IsAny<GetAccessTokenRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetAccessTokenResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _authorizationService.GetAccessTokenAsync("id", "secret", "code", CancellationToken.None));
        }

        [Fact]
        public async Task GetAccessTokenAsync_ApiConnectionThrowsException_PropagatesException()
        {
            // Arrange
            var apiException = new HttpRequestException("API error");
            _mockApiConnection
                .Setup(x => x.PostAsync<GetAccessTokenRequest, GetAccessTokenResponse>(
                    It.IsAny<string>(),
                    It.IsAny<GetAccessTokenRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(apiException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _authorizationService.GetAccessTokenAsync("id", "secret", "code", CancellationToken.None)
            );
            Assert.Equal(apiException.Message, actualException.Message);
        }

        // --- Test for GetAuthorizedUserAsync ---

        [Fact]
        public async Task GetAuthorizedUserAsync_ValidRequest_ReturnsUser()
        {
            // Arrange
            var expectedUser = new User(1, "Test User", "test@example.com", "#FFFFFF", "http://example.com/pic.jpg", "TU", null);
            var apiResponse = new GetAuthorizedUserResponse { User = expectedUser };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedUserResponse>("user", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _authorizationService.GetAuthorizedUserAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(expectedUser.Username, result.Username);
            _mockApiConnection.Verify(x => x.GetAsync<GetAuthorizedUserResponse>("user", CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetAuthorizedUserAsync_ApiConnectionReturnsNull_ReturnsNull()
        {
            // Arrange
            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedUserResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetAuthorizedUserResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _authorizationService.GetAuthorizedUserAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAuthorizedUserAsync_ApiConnectionResponseHasNullUser_ReturnsNull()
        {
            // Arrange
            var apiResponse = new GetAuthorizedUserResponse { User = null! };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedUserResponse>("user", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _authorizationService.GetAuthorizedUserAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAuthorizedUserAsync_ApiConnectionThrowsException_PropagatesException()
        {
            // Arrange
            var apiException = new HttpRequestException("API error");
            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedUserResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(apiException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _authorizationService.GetAuthorizedUserAsync(CancellationToken.None)
            );
            Assert.Equal(apiException.Message, actualException.Message);
        }

        // --- Test for GetAuthorizedWorkspacesAsync ---

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_ValidRequest_ReturnsWorkspaces()
        {
            // Arrange
            var expectedWorkspaces = new List<ClickUpWorkspace>
            {
                new ClickUpWorkspace { Id = "ws_1", Name = "Workspace 1", Color = "#FF0000", Members = new List<Member>() }
            };
            var apiResponse = new GetAuthorizedWorkspacesResponse { Workspaces = expectedWorkspaces };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedWorkspacesResponse>("team", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _authorizationService.GetAuthorizedWorkspacesAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedWorkspaces.First().Id, result.First().Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetAuthorizedWorkspacesResponse>("team", CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_ApiConnectionReturnsNull_ReturnsNull()
        {
            // Arrange
            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedWorkspacesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetAuthorizedWorkspacesResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _authorizationService.GetAuthorizedWorkspacesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_ApiConnectionResponseHasNullWorkspaces_ReturnsNull()
        {
            // Arrange
            var apiResponse = new GetAuthorizedWorkspacesResponse { Workspaces = null! };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedWorkspacesResponse>("team", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _authorizationService.GetAuthorizedWorkspacesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_ApiConnectionThrowsException_PropagatesException()
        {
            // Arrange
            var apiException = new HttpRequestException("API error");
            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedWorkspacesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(apiException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _authorizationService.GetAuthorizedWorkspacesAsync(CancellationToken.None)
            );
            Assert.Equal(apiException.Message, actualException.Message);
        }

        // --- Tests for TaskCanceledException and CancellationToken pass-through ---

        // GetAccessTokenAsync
        [Fact]
        public async Task GetAccessTokenAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            _mockApiConnection
                .Setup(x => x.PostAsync<GetAccessTokenRequest, GetAccessTokenResponse>(It.IsAny<string>(), It.IsAny<GetAccessTokenRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _authorizationService.GetAccessTokenAsync("id", "secret", "code", new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetAccessTokenAsync_PassesCancellationTokenToApiConnection()
        {
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetAccessTokenResponse("test_token");

            _mockApiConnection
                .Setup(x => x.PostAsync<GetAccessTokenRequest, GetAccessTokenResponse>(It.IsAny<string>(), It.IsAny<GetAccessTokenRequest>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _authorizationService.GetAccessTokenAsync("id", "secret", "code", expectedToken);

            _mockApiConnection.Verify(x => x.PostAsync<GetAccessTokenRequest, GetAccessTokenResponse>(
                "oauth/token",
                It.IsAny<GetAccessTokenRequest>(),
                expectedToken), Times.Once);
        }

        // GetAuthorizedUserAsync
        [Fact]
        public async Task GetAuthorizedUserAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedUserResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _authorizationService.GetAuthorizedUserAsync(new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetAuthorizedUserAsync_PassesCancellationTokenToApiConnection()
        {
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var mockUser = new User(1, "Test", "t@e.com", "#fff", null, "T");
            var expectedResponse = new GetAuthorizedUserResponse { User = mockUser };


            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedUserResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _authorizationService.GetAuthorizedUserAsync(expectedToken);

            _mockApiConnection.Verify(x => x.GetAsync<GetAuthorizedUserResponse>(
                "user",
                expectedToken), Times.Once);
        }

        // GetAuthorizedWorkspacesAsync
        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedWorkspacesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _authorizationService.GetAuthorizedWorkspacesAsync(new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_PassesCancellationTokenToApiConnection()
        {
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var mockWorkspace = new ClickUpWorkspace { Id = "ws1", Name = "WS1", Color = "#000", Members = new List<Member>() };
            var expectedResponse = new GetAuthorizedWorkspacesResponse { Workspaces = new List<ClickUpWorkspace> { mockWorkspace } };


            _mockApiConnection
                .Setup(x => x.GetAsync<GetAuthorizedWorkspacesResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _authorizationService.GetAuthorizedWorkspacesAsync(expectedToken);

            _mockApiConnection.Verify(x => x.GetAsync<GetAuthorizedWorkspacesResponse>(
                "team",
                expectedToken), Times.Once);
        }
    }
}
