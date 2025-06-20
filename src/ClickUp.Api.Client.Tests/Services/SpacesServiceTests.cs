using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using ClickUp.Api.Client.Models.ResponseModels.Spaces;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ClickUp.Api.Client.Tests.Services
{
    public class SpacesServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly SpacesService _spacesService;

        public SpacesServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _spacesService = new SpacesService(_mockApiConnection.Object);
        }

        // Placeholder for creating Space DTO instances
        private Space CreateSampleSpace(string id, string name)
        {
            var space = (Space)Activator.CreateInstance(typeof(Space), nonPublic: true)!;
            typeof(Space).GetProperty("Id")?.SetValue(space, id);
            typeof(Space).GetProperty("Name")?.SetValue(space, name);
            // Add other necessary properties if needed for specific tests
            return space;
        }

        [Fact]
        public async Task GetSpaceAsync_WhenSpaceExists_ReturnsSpace()
        {
            // Arrange
            var spaceId = "test-space-id";
            var expectedSpace = CreateSampleSpace(spaceId, "Test Space");

            _mockApiConnection.Setup(c => c.GetAsync<Space>(
                $"space/{spaceId}",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSpace);

            // Act
            var result = await _spacesService.GetSpaceAsync(spaceId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedSpace);
            _mockApiConnection.Verify(c => c.GetAsync<Space>(
                $"space/{spaceId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateSpaceAsync_ValidRequest_ReturnsCreatedSpace()
        {
            // Arrange
            var workspaceId = "test-workspace-id";
            var requestDto = new CreateSpaceRequest("New Test Space", false); // Assuming a constructor
            var expectedSpace = CreateSampleSpace("new-space-id", "New Test Space");

            _mockApiConnection.Setup(c => c.PostAsync<CreateSpaceRequest, Space>(
                $"team/{workspaceId}/space",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSpace);

            // Act
            var result = await _spacesService.CreateSpaceAsync(workspaceId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedSpace);
            _mockApiConnection.Verify(c => c.PostAsync<CreateSpaceRequest, Space>(
                $"team/{workspaceId}/space",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateSpaceAsync_ValidRequest_ReturnsUpdatedSpace()
        {
            // Arrange
            var spaceId = "space-to-update";
            var requestDto = new UpdateSpaceRequest { Name = "Updated Space Name" }; // Assuming settable Name
            var expectedSpace = CreateSampleSpace(spaceId, "Updated Space Name");

            _mockApiConnection.Setup(c => c.PutAsync<UpdateSpaceRequest, Space>(
                $"space/{spaceId}",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSpace);

            // Act
            var result = await _spacesService.UpdateSpaceAsync(spaceId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedSpace);
            _mockApiConnection.Verify(c => c.PutAsync<UpdateSpaceRequest, Space>(
                $"space/{spaceId}",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteSpaceAsync_ValidId_CallsApiConnectionDeleteAsync()
        {
            // Arrange
            var spaceId = "space-to-delete";
            var expectedEndpoint = $"space/{spaceId}";

            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _spacesService.DeleteSpaceAsync(spaceId, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // Note: If ISpacesService.GetSpacesAsync had an IAsyncEnumerable helper (e.g., GetSpacesAsyncEnumerableAsync),
        // tests would be similar to TaskServiceTests.GetTasksAsyncEnumerableAsync_WhenApiReturnsMultiplePages_YieldsAllTasks
        // or ListsServiceTests.GetFolderlessListsAsyncEnumerableAsync_WhenApiReturnsMultiplePages_YieldsAllLists,
        // mocking _apiConnection.GetAsync<GetSpacesResponse>(...) in sequence.
    }
}
