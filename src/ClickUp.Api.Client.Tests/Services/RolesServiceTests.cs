using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
// using ClickUp.Api.Client.Models.RequestModels.Roles; // No request DTOs for current methods
using ClickUp.Api.Client.Models.ResponseModels.Roles; // Assuming GetCustomRolesResponse
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ClickUp.Api.Client.Tests.Services
{
    public class RolesServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly RolesService _rolesService;

        public RolesServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _rolesService = new RolesService(_mockApiConnection.Object);
        }

        private CustomRole CreateSampleCustomRole(int id, string name) // Assuming CustomRole ID is int
        {
            var role = (CustomRole)Activator.CreateInstance(typeof(CustomRole), nonPublic: true)!;
            var props = typeof(CustomRole).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(role, id);
            props.FirstOrDefault(p => p.Name == "Name")?.SetValue(role, name);
            return role;
        }

        private GetCustomRolesResponse CreateSampleGetCustomRolesResponse(List<CustomRole> roles)
        {
            var responseType = typeof(GetCustomRolesResponse);
            var constructor = responseType.GetConstructors().FirstOrDefault(c =>
                c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(List<CustomRole>));
            if (constructor != null)
            {
                return (GetCustomRolesResponse)constructor.Invoke(new object[] { roles });
            }
            var instance = (GetCustomRolesResponse)Activator.CreateInstance(responseType, nonPublic: true)!;
            responseType.GetProperty("CustomRoles")?.SetValue(instance, roles); // Property from service impl
            return instance;
        }

        [Fact]
        public async Task GetCustomRolesAsync_WhenRolesExist_ReturnsRoles()
        {
            // Arrange
            var workspaceId = "ws-id";
            var expectedRoles = new List<CustomRole> { CreateSampleCustomRole(1, "Designer") };
            var expectedResponse = CreateSampleGetCustomRolesResponse(expectedRoles);

            _mockApiConnection.Setup(c => c.GetAsync<GetCustomRolesResponse>(
                $"team/{workspaceId}/customroles", // Basic endpoint, no query params in this simple test
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _rolesService.GetCustomRolesAsync(workspaceId, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedRoles);
            _mockApiConnection.Verify(c => c.GetAsync<GetCustomRolesResponse>(
                $"team/{workspaceId}/customroles",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCustomRolesAsync_WithIncludeMembers_ConstructsCorrectEndpoint()
        {
            // Arrange
            var workspaceId = "ws-id";
            var expectedRoles = new List<CustomRole> { CreateSampleCustomRole(1, "DesignerWithMembers") };
            var expectedResponse = CreateSampleGetCustomRolesResponse(expectedRoles);
            var expectedEndpoint = $"team/{workspaceId}/customroles?include_members=true";

            _mockApiConnection.Setup(c => c.GetAsync<GetCustomRolesResponse>(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _rolesService.GetCustomRolesAsync(workspaceId, includeMembers: true, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedRoles);
            _mockApiConnection.Verify(c => c.GetAsync<GetCustomRolesResponse>(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
