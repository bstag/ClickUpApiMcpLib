using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Users;
using ClickUp.Api.Client.Models.ResponseModels.Users; // Assuming response wrappers
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Tests.Services
{
    public class UsersServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly UsersService _usersService;
        private const string BaseWorkspaceEndpoint = "team";


        public UsersServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _usersService = new UsersService(_mockApiConnection.Object);
        }

        private User CreateSampleUser(int id, string username)
        {
            var user = (User)Activator.CreateInstance(typeof(User), nonPublic: true)!;
            var props = typeof(User).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(user, id);
            props.FirstOrDefault(p => p.Name == "Username")?.SetValue(user, username);
            return user;
        }

        private GetUserResponse CreateSampleGetUserResponse(User user)
        {
            var responseType = typeof(GetUserResponse);
            var constructor = responseType.GetConstructors().FirstOrDefault(c =>
                c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(User));
            if (constructor != null)
            {
                return (GetUserResponse)constructor.Invoke(new object[] { user });
            }
            var instance = (GetUserResponse)Activator.CreateInstance(responseType, nonPublic: true)!;
            responseType.GetProperty("User")?.SetValue(instance, user);
            return instance;
        }


        [Fact]
        public async Task GetUserFromWorkspaceAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var workspaceId = "ws-id";
            var userId = "user-123";
            var expectedUser = CreateSampleUser(123, "Test User");
            var expectedResponse = CreateSampleGetUserResponse(expectedUser);

            _mockApiConnection.Setup(c => c.GetAsync<GetUserResponse>(
                $"{BaseWorkspaceEndpoint}/{workspaceId}/user/{userId}",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _usersService.GetUserFromWorkspaceAsync(workspaceId, userId, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task EditUserOnWorkspaceAsync_ValidRequest_ReturnsUpdatedUser()
        {
            // Arrange
            var workspaceId = "ws-id";
            var userId = "user-456";
            var requestDto = new EditUserOnWorkspaceRequest("Updated Name", "admin");
            var expectedUser = CreateSampleUser(456, "Updated Name");
            var expectedResponse = CreateSampleGetUserResponse(expectedUser);


            _mockApiConnection.Setup(c => c.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(
                $"{BaseWorkspaceEndpoint}/{workspaceId}/user/{userId}",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _usersService.EditUserOnWorkspaceAsync(workspaceId, userId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUser);
        }
    }
}
