using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using ClickUp.Api.Client.Models.ResponseModels.Folders;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ClickUp.Api.Client.Tests.Services
{
    public class FoldersServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly FoldersService _foldersService;

        public FoldersServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _foldersService = new FoldersService(_mockApiConnection.Object);
        }

        // Placeholder for creating Folder DTO instances
        private Folder CreateSampleFolder(string id, string name)
        {
            var folder = (Folder)Activator.CreateInstance(typeof(Folder), nonPublic: true)!;
            typeof(Folder).GetProperty("Id")?.SetValue(folder, id);
            typeof(Folder).GetProperty("Name")?.SetValue(folder, name);
            return folder;
        }

        [Fact]
        public async Task GetFolderAsync_WhenFolderExists_ReturnsFolder()
        {
            // Arrange
            var folderId = "test-folder-id";
            var expectedFolder = CreateSampleFolder(folderId, "Test Folder");

            _mockApiConnection.Setup(c => c.GetAsync<Folder>(
                $"folder/{folderId}",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFolder);

            // Act
            var result = await _foldersService.GetFolderAsync(folderId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedFolder);
            _mockApiConnection.Verify(c => c.GetAsync<Folder>(
                $"folder/{folderId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateFolderAsync_ValidRequest_ReturnsCreatedFolder()
        {
            // Arrange
            var spaceId = "test-space-id";
            var requestDto = new CreateFolderRequest("New Test Folder");
            var expectedFolder = CreateSampleFolder("new-folder-id", "New Test Folder");

            _mockApiConnection.Setup(c => c.PostAsync<CreateFolderRequest, Folder>(
                $"space/{spaceId}/folder",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFolder);

            // Act
            var result = await _foldersService.CreateFolderAsync(spaceId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedFolder);
            _mockApiConnection.Verify(c => c.PostAsync<CreateFolderRequest, Folder>(
                $"space/{spaceId}/folder",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateFolderAsync_ValidRequest_ReturnsUpdatedFolder()
        {
            // Arrange
            var folderId = "folder-to-update";
            var requestDto = new UpdateFolderRequest { Name = "Updated Folder Name" };
            var expectedFolder = CreateSampleFolder(folderId, "Updated Folder Name");

            _mockApiConnection.Setup(c => c.PutAsync<UpdateFolderRequest, Folder>(
                $"folder/{folderId}",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFolder);

            // Act
            var result = await _foldersService.UpdateFolderAsync(folderId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedFolder);
            _mockApiConnection.Verify(c => c.PutAsync<UpdateFolderRequest, Folder>(
                $"folder/{folderId}",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteFolderAsync_ValidId_CallsApiConnectionDeleteAsync()
        {
            // Arrange
            var folderId = "folder-to-delete";
            var expectedEndpoint = $"folder/{folderId}";

            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _foldersService.DeleteFolderAsync(folderId, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // Note: GetFoldersAsyncEnumerableAsync is not on IFoldersService, but IListsService has GetFolderlessListsAsyncEnumerableAsync.
        // If IFoldersService had a GetFoldersAsync that returned IEnumerable and an equivalent IAsyncEnumerable version,
        // tests would be similar to TaskServiceTests.GetTasksAsyncEnumerableAsync_WhenApiReturnsMultiplePages_YieldsAllTasks
        // mocking _apiConnection.GetAsync<GetFoldersResponse>(...) in sequence.
    }
}
