using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels;
using ClickUp.Api.Client.Models.ResponseModels.Authorization;
using ClickUp.Api.Client.Models.ResponseModels; // For GetAuthorizedWorkspacesResponse if it's there
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Tests.Services
{
    public class AuthorizationServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly AuthorizationService _authorizationService;

        public AuthorizationServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _authorizationService = new AuthorizationService(_mockApiConnection.Object);
        }

        private User CreateSampleUser(int id, string username)
        {
            var user = (User)Activator.CreateInstance(typeof(User), nonPublic: true)!;
            var props = typeof(User).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(user, id);
            props.FirstOrDefault(p => p.Name == "Username")?.SetValue(user, username);
            return user;
        }

        private Workspace CreateSampleWorkspace(string id, string name)
        {
            var ws = (Workspace)Activator.CreateInstance(typeof(Workspace), nonPublic: true)!;
            var props = typeof(Workspace).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(ws, id);
            props.FirstOrDefault(p => p.Name == "Name")?.SetValue(ws, name);
            return ws;
        }

        // Assuming wrapper DTOs as implemented in AuthorizationService
        private GetAuthorizedUserResponse CreateSampleGetAuthorizedUserResponse(User user)
        {
            var responseType = typeof(GetAuthorizedUserResponse);
            var constructor = responseType.GetConstructors().FirstOrDefault(c =>
                c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(User));
            if (constructor != null)
            {
                return (GetAuthorizedUserResponse)constructor.Invoke(new object[] { user });
            }
            var instance = (GetAuthorizedUserResponse)Activator.CreateInstance(responseType, nonPublic: true)!;
            responseType.GetProperty("User")?.SetValue(instance, user);
            return instance;
        }

        private GetAuthorizedWorkspacesResponse CreateSampleGetAuthorizedWorkspacesResponse(List<Workspace> workspaces)
        {
             var responseType = typeof(GetAuthorizedWorkspacesResponse);
            var constructor = responseType.GetConstructors().FirstOrDefault(c =>
                c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(List<Workspace>));
            if (constructor != null)
            {
                return (GetAuthorizedWorkspacesResponse)constructor.Invoke(new object[] { workspaces });
            }
            var instance = (GetAuthorizedWorkspacesResponse)Activator.CreateInstance(responseType, nonPublic: true)!;
            responseType.GetProperty("Teams")?.SetValue(instance, workspaces); // Property is "Teams" in service impl
            return instance;
        }


        [Fact]
        public async Task GetAccessTokenAsync_ValidRequest_ReturnsTokenResponse()
        {
            // Arrange
            var clientId = "client-id";
            var clientSecret = "client-secret";
            var code = "auth-code";
            var expectedResponse = new AccessTokenResponse("fake-access-token", "bearer");
            var requestDto = new GetAccessTokenRequest(clientId, clientSecret, code);

            _mockApiConnection.Setup(c => c.PostAsync<GetAccessTokenRequest, AccessTokenResponse>(
                "oauth/token",
                It.Is<GetAccessTokenRequest>(r => r.ClientId == clientId && r.ClientSecret == clientSecret && r.Code == code),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _authorizationService.GetAccessTokenAsync(clientId, clientSecret, code, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetAuthorizedUserAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var sampleUser = CreateSampleUser(1, "auth_user");
            var expectedResponse = CreateSampleGetAuthorizedUserResponse(sampleUser);

            _mockApiConnection.Setup(c => c.GetAsync<GetAuthorizedUserResponse>(
                "user",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _authorizationService.GetAuthorizedUserAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleUser);
        }

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_WhenWorkspacesExist_ReturnsWorkspaces()
        {
            // Arrange
            var sampleWorkspaces = new List<Workspace> { CreateSampleWorkspace("ws1", "Workspace 1") };
            var expectedResponse = CreateSampleGetAuthorizedWorkspacesResponse(sampleWorkspaces);

            _mockApiConnection.Setup(c => c.GetAsync<GetAuthorizedWorkspacesResponse>(
                "team",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _authorizationService.GetAuthorizedWorkspacesAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleWorkspaces);
        }
    }
}
