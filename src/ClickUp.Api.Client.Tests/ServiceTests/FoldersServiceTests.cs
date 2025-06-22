using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using ClickUp.Api.Client.Models.ResponseModels.Folders;
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class FoldersServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly FoldersService _foldersService;
        private readonly Mock<ILogger<FoldersService>> _mockLogger;

        public FoldersServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<FoldersService>>();
            _foldersService = new FoldersService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private Folder CreateSampleFolder(string id = "folder_1", string name = "Sample Folder") => new Folder(
            Id: id,
            Name: name,
            Archived: false,
            Statuses: new List<ClickUp.Api.Client.Models.Common.Status>()
        );

        [Fact]
        public async Task GetFoldersAsync_ValidSpaceId_BuildsCorrectUrlAndReturnsFolders()
        {
            // Arrange
            var spaceId = "space_123";
            var expectedFolders = new List<Folder> { CreateSampleFolder("folder_abc") };
            var apiResponse = new GetFoldersResponse(expectedFolders);
            _mockApiConnection
                .Setup(c => c.GetAsync<GetFoldersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _foldersService.GetFoldersAsync(spaceId, false);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("folder_abc", result.First().Id);
            _mockApiConnection.Verify(c => c.GetAsync<GetFoldersResponse>(
                $"space/{spaceId}/folder?archived=false",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateFolderAsync_ValidRequest_CallsPostAndReturnsFolder()
        {
            // Arrange
            var spaceId = "space_test";
            var request = new CreateFolderRequest("New Folder");
            var expectedFolder = CreateSampleFolder("new_folder_1");
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateFolderRequest, Folder>(
                    It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFolder);

            // Act
            var result = await _foldersService.CreateFolderAsync(spaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedFolder.Id, result.Id);
            _mockApiConnection.Verify(c => c.PostAsync<CreateFolderRequest, Folder>(
                $"space/{spaceId}/folder",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderAsync_ApiError_ThrowsHttpRequestException()
        {
            // Arrange
            var folderId = "folder_error";
            _mockApiConnection
                .Setup(c => c.GetAsync<Folder>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _foldersService.GetFolderAsync(folderId));
        }

        [Fact]
        public async Task UpdateFolderAsync_NullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var folderId = "folder_null_response";
            var request = new UpdateFolderRequest("Updated Name");
            _mockApiConnection
                .Setup(c => c.PutAsync<UpdateFolderRequest, Folder>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Folder)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _foldersService.UpdateFolderAsync(folderId, request));
        }
    }
}
