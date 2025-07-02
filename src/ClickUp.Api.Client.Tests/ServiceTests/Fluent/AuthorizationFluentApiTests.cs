using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Users;
using Moq;
using Xunit;
using ClickUp.Api.Client.Models.Entities.WorkSpaces;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent
{
    public class AuthorizationFluentApiTests : IDisposable
    {
        private readonly Mock<IAuthorizationService> _mockAuthService;
        private readonly AuthorizationFluentApi _authApi;

        public AuthorizationFluentApiTests()
        {
            _mockAuthService = new Mock<IAuthorizationService>();
            _authApi = new AuthorizationFluentApi(_mockAuthService.Object);
        }

        [Fact]
        public void Constructor_NullAuthService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AuthorizationFluentApi(null!));
        }

        [Fact]
        public void GetAccessToken_ReturnsNewAccessTokenRequest()
        {
            var request = _authApi.GetAccessToken();
            Assert.NotNull(request);
        }

        [Fact]
        public async Task GetAuthorizedUserAsync_CallsServiceWithCorrectParameters()
        {
            // Arrange
            var expectedUser = new User(
                Id: 123,
                Username: "testuser", 
                Email: "test@example.com",
                Color: "#000000",
                ProfilePicture: null,
                Initials: "TU",
                Role: 1,
                CustomRole: null,
                LastActive: null,
                DateJoined: DateTime.UtcNow,
                DateInvited: DateTime.UtcNow
            );

            _mockAuthService
                .Setup(x => x.GetAuthorizedUserAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _authApi.GetAuthorizedUserAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
            _mockAuthService.Verify(x => x.GetAuthorizedUserAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAuthorizedUserAsync_ServiceThrows_PropagatesException()
        {
            // Arrange
            _mockAuthService
                .Setup(x => x.GetAuthorizedUserAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authApi.GetAuthorizedUserAsync());
        }

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_CallsServiceWithCorrectParameters()
        {
            // Arrange
            var expectedWorkspace = new ClickUpWorkspace
            {
                Id = "workspace_123",
                Name = "Test Workspace",
                Color = "#3498db"
            };

            var expectedWorkspaces = new List<ClickUpWorkspace> { expectedWorkspace };

            _mockAuthService
                .Setup(x => x.GetAuthorizedWorkspacesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedWorkspaces);

            // Act
            var result = await _authApi.GetAuthorizedWorkspacesAsync();

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(expectedWorkspace.Id, resultList[0].Id);
            _mockAuthService.Verify(x => x.GetAuthorizedWorkspacesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_ServiceThrows_PropagatesException()
        {
            // Arrange
            _mockAuthService
                .Setup(x => x.GetAuthorizedWorkspacesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authApi.GetAuthorizedWorkspacesAsync());
        }

        public void Dispose()
        {
            _mockAuthService.VerifyAll();
        }
    }
}