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
                .ReturnsAsync((Folder?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _foldersService.UpdateFolderAsync(folderId, request));
        }

        // --- GetFoldersAsync Tests ---

        [Fact]
        public async Task GetFoldersAsync_ArchivedTrue_BuildsCorrectUrl()
        {
            var spaceId = "space_456";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetFoldersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetFoldersResponse(new List<Folder>()));

            await _foldersService.GetFoldersAsync(spaceId, true);

            _mockApiConnection.Verify(c => c.GetAsync<GetFoldersResponse>(
                $"space/{spaceId}/folder?archived=true",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFoldersAsync_ArchivedNull_BuildsCorrectUrl()
        {
            var spaceId = "space_789";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetFoldersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetFoldersResponse(new List<Folder>()));

            await _foldersService.GetFoldersAsync(spaceId, null);

            _mockApiConnection.Verify(c => c.GetAsync<GetFoldersResponse>(
                $"space/{spaceId}/folder", // No archived query param
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFoldersAsync_ApiReturnsEmptyList_ReturnsEmptyEnumerable()
        {
            var spaceId = "space_empty";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetFoldersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetFoldersResponse(new List<Folder>()));

            var result = await _foldersService.GetFoldersAsync(spaceId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFoldersAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            var spaceId = "space_null_resp";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetFoldersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetFoldersResponse?)null);

            var result = await _foldersService.GetFoldersAsync(spaceId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFoldersAsync_ApiError_ThrowsHttpRequestException()
        {
            var spaceId = "space_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetFoldersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _foldersService.GetFoldersAsync(spaceId));
        }

        // --- CreateFolderAsync Tests ---

        [Fact]
        public async Task CreateFolderAsync_ApiError_ThrowsHttpRequestException()
        {
            var spaceId = "space_create_err";
            var request = new CreateFolderRequest("Error Folder");
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateFolderRequest, Folder>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _foldersService.CreateFolderAsync(spaceId, request));
        }

        [Fact]
        public async Task CreateFolderAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var spaceId = "space_create_null";
            var request = new CreateFolderRequest("Null Folder");
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateFolderRequest, Folder>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Folder?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _foldersService.CreateFolderAsync(spaceId, request));
        }

        // --- GetFolderAsync Tests ---
        [Fact]
        public async Task GetFolderAsync_ValidId_ReturnsFolder()
        {
            var folderId = "folder_xyz";
            var expectedFolder = CreateSampleFolder(folderId);
            _mockApiConnection
                .Setup(c => c.GetAsync<Folder>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFolder);

            var result = await _foldersService.GetFolderAsync(folderId);

            Assert.NotNull(result);
            Assert.Equal(expectedFolder.Id, result.Id);
            _mockApiConnection.Verify(c => c.GetAsync<Folder>(
                $"folder/{folderId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var folderId = "folder_get_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<Folder>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Folder?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _foldersService.GetFolderAsync(folderId));
        }


        // --- UpdateFolderAsync Tests ---
        [Fact]
        public async Task UpdateFolderAsync_ValidRequest_CallsPutAndReturnsFolder()
        {
            var folderId = "folder_update_1";
            var request = new UpdateFolderRequest("Updated Name");
            var expectedFolder = CreateSampleFolder(folderId, "Updated Name");
            _mockApiConnection
                .Setup(c => c.PutAsync<UpdateFolderRequest, Folder>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFolder);

            var result = await _foldersService.UpdateFolderAsync(folderId, request);

            Assert.NotNull(result);
            Assert.Equal(expectedFolder.Name, result.Name);
            _mockApiConnection.Verify(c => c.PutAsync<UpdateFolderRequest, Folder>(
                $"folder/{folderId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateFolderAsync_ApiError_ThrowsHttpRequestException()
        {
            var folderId = "folder_update_err";
            var request = new UpdateFolderRequest("Error Update");
            _mockApiConnection
                .Setup(c => c.PutAsync<UpdateFolderRequest, Folder>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _foldersService.UpdateFolderAsync(folderId, request));
        }

        // --- DeleteFolderAsync Tests ---
        [Fact]
        public async Task DeleteFolderAsync_ValidId_CallsDelete()
        {
            var folderId = "folder_delete_1";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask); // Corrected to Task.CompletedTask for non-generic Task

            await _foldersService.DeleteFolderAsync(folderId);

            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"folder/{folderId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteFolderAsync_ApiError_ThrowsHttpRequestException()
        {
            var folderId = "folder_delete_err";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _foldersService.DeleteFolderAsync(folderId));
        }

        // --- CreateFolderFromTemplateAsync Tests ---
        [Fact]
        public async Task CreateFolderFromTemplateAsync_ValidRequest_CallsPostAndReturnsFolder()
        {
            var spaceId = "space_template";
            var templateId = "template_abc";
            var request = new CreateFolderFromTemplateRequest { Name = "Templated Folder" };
            var expectedFolder = CreateSampleFolder("templated_folder_1", "Templated Folder");
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateFolderFromTemplateRequest, Folder>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFolder);

            var result = await _foldersService.CreateFolderFromTemplateAsync(spaceId, templateId, request);

            Assert.NotNull(result);
            Assert.Equal(expectedFolder.Name, result.Name);
            _mockApiConnection.Verify(c => c.PostAsync<CreateFolderFromTemplateRequest, Folder>(
                $"space/{spaceId}/folderTemplate/{templateId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateFolderFromTemplateAsync_ApiError_ThrowsHttpRequestException()
        {
            var spaceId = "space_template_err";
            var templateId = "template_err";
            var request = new CreateFolderFromTemplateRequest { Name = "Error Templated Folder" };
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateFolderFromTemplateRequest, Folder>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _foldersService.CreateFolderFromTemplateAsync(spaceId, templateId, request));
        }

        [Fact]
        public async Task CreateFolderFromTemplateAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var spaceId = "space_template_null";
            var templateId = "template_null";
            var request = new CreateFolderFromTemplateRequest { Name = "Null Templated Folder" };
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateFolderFromTemplateRequest, Folder>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Folder?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _foldersService.CreateFolderFromTemplateAsync(spaceId, templateId, request));
        }

        // --- Tests for TaskCanceledException and CancellationToken pass-through ---

        // GetFoldersAsync
        [Fact]
        public async Task GetFoldersAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var spaceId = "space_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetFoldersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _foldersService.GetFoldersAsync(spaceId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetFoldersAsync_PassesCancellationTokenToApiConnection()
        {
            var spaceId = "space_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetFoldersResponse(new List<Folder>());
            _mockApiConnection
                .Setup(c => c.GetAsync<GetFoldersResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _foldersService.GetFoldersAsync(spaceId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<GetFoldersResponse>(
                $"space/{spaceId}/folder", // Assuming archived is null by default
                expectedToken), Times.Once);
        }

        // CreateFolderAsync
        [Fact]
        public async Task CreateFolderAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var spaceId = "space_create_cancel";
            var request = new CreateFolderRequest("Cancel Folder");
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateFolderRequest, Folder>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _foldersService.CreateFolderAsync(spaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreateFolderAsync_PassesCancellationTokenToApiConnection()
        {
            var spaceId = "space_create_ct_pass";
            var request = new CreateFolderRequest("CT Pass Folder");
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedFolder = CreateSampleFolder("new_folder_ct");

            _mockApiConnection
                .Setup(c => c.PostAsync<CreateFolderRequest, Folder>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(expectedFolder);

            await _foldersService.CreateFolderAsync(spaceId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<CreateFolderRequest, Folder>(
                $"space/{spaceId}/folder",
                request,
                expectedToken), Times.Once);
        }

        // GetFolderAsync
        [Fact]
        public async Task GetFolderAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var folderId = "folder_get_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<Folder>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _foldersService.GetFolderAsync(folderId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetFolderAsync_PassesCancellationTokenToApiConnection()
        {
            var folderId = "folder_get_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedFolder = CreateSampleFolder(folderId);

            _mockApiConnection
                .Setup(c => c.GetAsync<Folder>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedFolder);

            await _foldersService.GetFolderAsync(folderId, expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<Folder>(
                $"folder/{folderId}",
                expectedToken), Times.Once);
        }

        // UpdateFolderAsync
        [Fact]
        public async Task UpdateFolderAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var folderId = "folder_update_cancel";
            var request = new UpdateFolderRequest("Cancel Update");
            _mockApiConnection
                .Setup(c => c.PutAsync<UpdateFolderRequest, Folder>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _foldersService.UpdateFolderAsync(folderId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task UpdateFolderAsync_PassesCancellationTokenToApiConnection()
        {
            var folderId = "folder_update_ct_pass";
            var request = new UpdateFolderRequest("CT Pass Update");
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedFolder = CreateSampleFolder(folderId, "CT Pass Update");

            _mockApiConnection
                .Setup(c => c.PutAsync<UpdateFolderRequest, Folder>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(expectedFolder);

            await _foldersService.UpdateFolderAsync(folderId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PutAsync<UpdateFolderRequest, Folder>(
                $"folder/{folderId}",
                request,
                expectedToken), Times.Once);
        }

        // DeleteFolderAsync
        [Fact]
        public async Task DeleteFolderAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var folderId = "folder_delete_cancel";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _foldersService.DeleteFolderAsync(folderId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task DeleteFolderAsync_PassesCancellationTokenToApiConnection()
        {
            var folderId = "folder_delete_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), expectedToken))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            await _foldersService.DeleteFolderAsync(folderId, expectedToken);

            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"folder/{folderId}",
                expectedToken), Times.Once);
        }

        // CreateFolderFromTemplateAsync
        [Fact]
        public async Task CreateFolderFromTemplateAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var spaceId = "space_tpl_cancel";
            var templateId = "tpl_cancel";
            var request = new CreateFolderFromTemplateRequest { Name = "Cancel Template Folder" };
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateFolderFromTemplateRequest, Folder>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _foldersService.CreateFolderFromTemplateAsync(spaceId, templateId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreateFolderFromTemplateAsync_PassesCancellationTokenToApiConnection()
        {
            var spaceId = "space_tpl_ct_pass";
            var templateId = "tpl_ct_pass";
            var request = new CreateFolderFromTemplateRequest { Name = "CT Pass Template Folder" };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedFolder = CreateSampleFolder("tpl_folder_ct");

            _mockApiConnection
                .Setup(c => c.PostAsync<CreateFolderFromTemplateRequest, Folder>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(expectedFolder);

            await _foldersService.CreateFolderFromTemplateAsync(spaceId, templateId, request, expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<CreateFolderFromTemplateRequest, Folder>(
                $"space/{spaceId}/folderTemplate/{templateId}",
                request,
                expectedToken), Times.Once);
        }
    }
}
