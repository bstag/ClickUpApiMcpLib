using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models; // For ClickUpList and related models if not directly in .Entities
using ClickUp.Api.Client.Models.Entities; // For concrete entities used in Lists if any
using ClickUp.Api.Client.Models.ResponseModels.Lists; // For GetListsResponse
using ClickUp.Api.Client.Models.RequestModels.Lists; // If any request models are used by ListsService methods
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class ListsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly ListsService _listsService;
        private readonly Mock<ILogger<ListsService>> _mockLogger;

        public ListsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<ListsService>>();
            _listsService = new ListsService(_mockApiConnection.Object, _mockLogger.Object);
        }

        // Helper to create a sample Folder DTO for use in List DTO
        private ClickUp.Api.Client.Models.Entities.Folders.Folder CreateSampleFolderDto(string id = "folder_1", string name = "Sample Folder")
        {
            return new ClickUp.Api.Client.Models.Entities.Folders.Folder(
                Id: id,
                Name: name,
                Archived: false,
                Statuses: new List<ClickUp.Api.Client.Models.Common.Status>() // Assuming empty or default statuses
            );
        }

        // Helper to create a sample ListPriorityInfo DTO (if needed and defined)
        // Assuming ListPriorityInfo is a simple record/class if it exists
        // For now, let's assume it can be null if not critical for these tests or not fully defined yet.
        // private ClickUp.Api.Client.Models.Entities.Lists.ListPriorityInfo CreateSampleListPriorityInfo()
        // {
        //     return new ClickUp.Api.Client.Models.Entities.Lists.ListPriorityInfo(...);
        // }

        // Corrected CreateSampleClickUpListRaw based on actual Entities.Lists.List DTO
        private ClickUp.Api.Client.Models.Entities.Lists.List CreateSampleClickUpListRaw(
            string id = "list_1",
            string name = "Sample List",
            ClickUp.Api.Client.Models.Entities.Folders.Folder? folder = null, // Optional folder
            ClickUp.Api.Client.Models.Entities.Lists.ListPriorityInfo? priority = null) // Optional priority
        {
            return new ClickUp.Api.Client.Models.Entities.Lists.List(
                Id: id,
                Name: name,
                Folder: folder ?? CreateSampleFolderDto(id + "_folder", name + " Folder"), // Provide a default folder if null
                Priority: priority // Can be null
            );
        }

        // --- Tests for GetListsInFolderAsync ---

        [Fact]
        public async Task GetListsInFolderAsync_ValidFolderId_ReturnsLists()
        {
            // Arrange
            var folderId = "folder123";
            var rawListsFromApi = new List<ClickUp.Api.Client.Models.Entities.Lists.List> { CreateSampleClickUpListRaw("list1", "List One") };
            var apiResponse = new GetListsResponse(rawListsFromApi);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.Is<string>(s => s.StartsWith($"folder/{folderId}/list")), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _listsService.GetListsInFolderAsync(folderId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("list1", result.First().Id);
            Assert.Equal("List One", result.First().Name);
            _mockApiConnection.Verify(x => x.GetAsync<GetListsResponse>(
                $"folder/{folderId}/list", // No query params by default
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListsInFolderAsync_WithArchivedTrue_BuildsCorrectUrl()
        {
            // Arrange
            var folderId = "folder456";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListsResponse(new List<ClickUp.Api.Client.Models.Entities.Lists.List>()));

            // Act
            await _listsService.GetListsInFolderAsync(folderId, archived: true);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetListsResponse>(
                $"folder/{folderId}/list?archived=true",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListsInFolderAsync_WithArchivedFalse_BuildsCorrectUrl()
        {
            // Arrange
            var folderId = "folder789";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListsResponse(new List<ClickUp.Api.Client.Models.Entities.Lists.List>()));

            // Act
            await _listsService.GetListsInFolderAsync(folderId, archived: false);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetListsResponse>(
                $"folder/{folderId}/list?archived=false",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListsInFolderAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            // Arrange
            var folderId = "folder_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetListsResponse)null);

            // Act
            var result = await _listsService.GetListsInFolderAsync(folderId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetListsInFolderAsync_ApiReturnsResponseWithNullLists_ReturnsEmptyEnumerable()
        {
            // Arrange
            var folderId = "folder_null_lists_in_resp";
            var apiResponse = new GetListsResponse(null!); // Lists property is null
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _listsService.GetListsInFolderAsync(folderId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetListsInFolderAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var folderId = "folder_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _listsService.GetListsInFolderAsync(folderId)
            );
        }

        [Fact]
        public async Task GetListsInFolderAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var folderId = "folder_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _listsService.GetListsInFolderAsync(folderId, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetListsInFolderAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var folderId = "folder_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetListsResponse(new List<ClickUp.Api.Client.Models.Entities.Lists.List>()));

            // Act
            await _listsService.GetListsInFolderAsync(folderId, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetListsResponse>(
                $"folder/{folderId}/list",
                expectedToken), Times.Once);
        }

        // --- Tests for CreateListInFolderAsync ---

        [Fact]
        public async Task CreateListInFolderAsync_ValidRequest_ReturnsList()
        {
            // Arrange
            var folderId = "folder123";
            var request = new CreateListRequest("New List", null, null, null, null, null, null, null);
            var expectedList = new ClickUpList { Id = "list_new", Name = "New List" }; // Simplified for example

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListRequest, ClickUpList>(
                    $"folder/{folderId}/list",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _listsService.CreateListInFolderAsync(folderId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedList.Id, result.Id);
            Assert.Equal(expectedList.Name, result.Name);
            _mockApiConnection.Verify(x => x.PostAsync<CreateListRequest, ClickUpList>(
                $"folder/{folderId}/list",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateListInFolderAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var folderId = "folder_create_null_api_resp";
            var request = new CreateListRequest("Null Resp List", null, null, null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpList)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _listsService.CreateListInFolderAsync(folderId, request)
            );
        }

        [Fact]
        public async Task CreateListInFolderAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var folderId = "folder_create_http_ex";
            var request = new CreateListRequest("Http Ex List", null, null, null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _listsService.CreateListInFolderAsync(folderId, request)
            );
        }

        [Fact]
        public async Task CreateListInFolderAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var folderId = "folder_create_cancel_ex";
            var request = new CreateListRequest("Cancel Ex List", null, null, null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _listsService.CreateListInFolderAsync(folderId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task CreateListInFolderAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var folderId = "folder_create_ct_pass";
            var request = new CreateListRequest("CT Pass List", null, null, null, null, null, null, null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedList = new ClickUpList { Id = "list_ct_new", Name = "CT Pass List" };

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListRequest, ClickUpList>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(expectedList);

            // Act
            await _listsService.CreateListInFolderAsync(folderId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateListRequest, ClickUpList>(
                $"folder/{folderId}/list",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for GetFolderlessListsAsync ---

        [Fact]
        public async Task GetFolderlessListsAsync_ValidSpaceId_ReturnsLists()
        {
            // Arrange
            var spaceId = "space123";
            var expectedRawLists = new List<ClickUp.Api.Client.Models.Entities.Lists.List> { CreateSampleClickUpListRaw("list_fl1", "Folderless List One") };
            var apiResponse = new GetListsResponse(expectedRawLists);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.Is<string>(s => s.StartsWith($"space/{spaceId}/list")), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _listsService.GetFolderlessListsAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("list_fl1", result.First().Id);
            Assert.Equal("Folderless List One", result.First().Name);
            _mockApiConnection.Verify(x => x.GetAsync<GetListsResponse>(
                $"space/{spaceId}/list", // No query params by default
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderlessListsAsync_WithArchivedTrue_BuildsCorrectUrl()
        {
            // Arrange
            var spaceId = "space456";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListsResponse(new List<ClickUp.Api.Client.Models.Entities.Lists.List>()));

            // Act
            await _listsService.GetFolderlessListsAsync(spaceId, archived: true);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetListsResponse>(
                $"space/{spaceId}/list?archived=true",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderlessListsAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            // Arrange
            var spaceId = "space_fl_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetListsResponse)null);

            // Act
            var result = await _listsService.GetFolderlessListsAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFolderlessListsAsync_ApiReturnsResponseWithNullLists_ReturnsEmptyEnumerable()
        {
            // Arrange
            var spaceId = "space_fl_null_lists_in_resp";
            var apiResponse = new GetListsResponse(null!); // Lists property is null
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _listsService.GetFolderlessListsAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFolderlessListsAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_fl_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _listsService.GetFolderlessListsAsync(spaceId)
            );
        }

        [Fact]
        public async Task GetFolderlessListsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_fl_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _listsService.GetFolderlessListsAsync(spaceId, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetFolderlessListsAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var spaceId = "space_fl_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetListsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetListsResponse(new List<ClickUp.Api.Client.Models.Entities.Lists.List>()));

            // Act
            await _listsService.GetFolderlessListsAsync(spaceId, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetListsResponse>(
                $"space/{spaceId}/list",
                expectedToken), Times.Once);
        }

        // --- Tests for CreateFolderlessListAsync ---

        [Fact]
        public async Task CreateFolderlessListAsync_ValidRequest_ReturnsList()
        {
            // Arrange
            var spaceId = "space123";
            var request = new CreateListRequest("New Folderless List", null, null, null, null, null, null, null);
            var expectedList = new ClickUpList { Id = "list_fl_new", Name = "New Folderless List" };

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListRequest, ClickUpList>(
                    $"space/{spaceId}/list",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _listsService.CreateFolderlessListAsync(spaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedList.Id, result.Id);
            Assert.Equal(expectedList.Name, result.Name);
            _mockApiConnection.Verify(x => x.PostAsync<CreateListRequest, ClickUpList>(
                $"space/{spaceId}/list",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateFolderlessListAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var spaceId = "space_fl_create_null_api_resp";
            var request = new CreateListRequest("Null Resp FL List", null, null, null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpList)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _listsService.CreateFolderlessListAsync(spaceId, request)
            );
        }

        [Fact]
        public async Task CreateFolderlessListAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_fl_create_http_ex";
            var request = new CreateListRequest("Http Ex FL List", null, null, null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _listsService.CreateFolderlessListAsync(spaceId, request)
            );
        }

        [Fact]
        public async Task CreateFolderlessListAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_fl_create_cancel_ex";
            var request = new CreateListRequest("Cancel Ex FL List", null, null, null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _listsService.CreateFolderlessListAsync(spaceId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task CreateFolderlessListAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var spaceId = "space_fl_create_ct_pass";
            var request = new CreateListRequest("CT Pass FL List", null, null, null, null, null, null, null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedList = new ClickUpList { Id = "list_fl_ct_new", Name = "CT Pass FL List" };

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListRequest, ClickUpList>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(expectedList);

            // Act
            await _listsService.CreateFolderlessListAsync(spaceId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateListRequest, ClickUpList>(
                $"space/{spaceId}/list",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for GetListAsync ---

        [Fact]
        public async Task GetListAsync_ValidListId_ReturnsList()
        {
            // Arrange
            var listId = "list123";
            var expectedList = new ClickUpList { Id = listId, Name = "Specific List" };

            _mockApiConnection
                .Setup(x => x.GetAsync<ClickUpList>(
                    $"list/{listId}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _listsService.GetListAsync(listId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedList.Id, result.Id);
            _mockApiConnection.Verify(x => x.GetAsync<ClickUpList>(
                $"list/{listId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var listId = "list_get_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<ClickUpList>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpList)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _listsService.GetListAsync(listId)
            );
        }

        [Fact]
        public async Task GetListAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var listId = "list_get_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<ClickUpList>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _listsService.GetListAsync(listId)
            );
        }

        [Fact]
        public async Task GetListAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var listId = "list_get_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<ClickUpList>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _listsService.GetListAsync(listId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetListAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var listId = "list_get_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedList = new ClickUpList { Id = listId, Name = "CT Pass Get List" };

            _mockApiConnection
                .Setup(x => x.GetAsync<ClickUpList>(
                    It.IsAny<string>(),
                    expectedToken))
                .ReturnsAsync(expectedList);

            // Act
            await _listsService.GetListAsync(listId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<ClickUpList>(
                $"list/{listId}",
                expectedToken), Times.Once);
        }

        // --- Tests for UpdateListAsync ---

        [Fact]
        public async Task UpdateListAsync_ValidRequest_ReturnsUpdatedList()
        {
            // Arrange
            var listId = "list123_update";
            // Use constructor: UpdateListRequest(string Name, string? Content, ..., bool? UnsetStatus)
            var request = new UpdateListRequest("Updated List Name", null, null, null, null, null, null, null, null);
            var expectedList = new ClickUpList { Id = listId, Name = "Updated List Name" };

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateListRequest, ClickUpList>(
                    $"list/{listId}",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _listsService.UpdateListAsync(listId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedList.Name, result.Name);
            _mockApiConnection.Verify(x => x.PutAsync<UpdateListRequest, ClickUpList>(
                $"list/{listId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateListAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var listId = "list_update_null_api_resp";
            var request = new UpdateListRequest("Update Null Resp", null, null, null, null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateListRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<UpdateListRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpList)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _listsService.UpdateListAsync(listId, request)
            );
        }

        [Fact]
        public async Task UpdateListAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var listId = "list_update_http_ex";
            var request = new UpdateListRequest("Update Http Ex", null, null, null, null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateListRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<UpdateListRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _listsService.UpdateListAsync(listId, request)
            );
        }

        [Fact]
        public async Task UpdateListAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var listId = "list_update_cancel_ex";
            var request = new UpdateListRequest("Update Cancel Ex", null, null, null, null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateListRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<UpdateListRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _listsService.UpdateListAsync(listId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task UpdateListAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var listId = "list_update_ct_pass";
            var request = new UpdateListRequest("Update CT Pass", null, null, null, null, null, null, null, null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedList = new ClickUpList { Id = listId, Name = "Update CT Pass" };

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateListRequest, ClickUpList>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(expectedList);

            // Act
            await _listsService.UpdateListAsync(listId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync<UpdateListRequest, ClickUpList>(
                $"list/{listId}",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for DeleteListAsync ---

        [Fact]
        public async Task DeleteListAsync_ValidListId_CallsDeleteAsync()
        {
            // Arrange
            var listId = "list123_delete";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"list/{listId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _listsService.DeleteListAsync(listId);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"list/{listId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteListAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var listId = "list_delete_http_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _listsService.DeleteListAsync(listId)
            );
        }

        [Fact]
        public async Task DeleteListAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var listId = "list_delete_cancel_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _listsService.DeleteListAsync(listId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task DeleteListAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var listId = "list_delete_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _listsService.DeleteListAsync(listId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"list/{listId}",
                expectedToken), Times.Once);
        }

        // --- Tests for AddTaskToListAsync ---

        [Fact]
        public async Task AddTaskToListAsync_ValidIds_CallsPostAsync()
        {
            // Arrange
            var listId = "list_add_task";
            var taskId = "task_to_add";
            _mockApiConnection
                .Setup(x => x.PostAsync<object>( // Expecting object as TBody since no actual body is sent
                    $"list/{listId}/task/{taskId}",
                    It.IsAny<object>(), // Match any object for the body
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask); // PostAsync<TBody> returns Task, not Task<TResponse>

            // Act
            await _listsService.AddTaskToListAsync(listId, taskId);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<object>(
                $"list/{listId}/task/{taskId}",
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddTaskToListAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var listId = "list_add_task_http_ex";
            var taskId = "task_http_ex";
            _mockApiConnection
                .Setup(x => x.PostAsync<object>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _listsService.AddTaskToListAsync(listId, taskId)
            );
        }

        [Fact]
        public async Task AddTaskToListAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var listId = "list_add_task_cancel_ex";
            var taskId = "task_cancel_ex";
            _mockApiConnection
                .Setup(x => x.PostAsync<object>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _listsService.AddTaskToListAsync(listId, taskId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task AddTaskToListAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var listId = "list_add_task_ct_pass";
            var taskId = "task_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.PostAsync<object>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _listsService.AddTaskToListAsync(listId, taskId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<object>(
                $"list/{listId}/task/{taskId}",
                It.IsAny<object>(),
                expectedToken), Times.Once);
        }

        // --- Tests for RemoveTaskFromListAsync ---

        [Fact]
        public async Task RemoveTaskFromListAsync_ValidIds_CallsDeleteAsync()
        {
            // Arrange
            var listId = "list_remove_task";
            var taskId = "task_to_remove";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"list/{listId}/task/{taskId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _listsService.RemoveTaskFromListAsync(listId, taskId);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"list/{listId}/task/{taskId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveTaskFromListAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var listId = "list_remove_task_http_ex";
            var taskId = "task_rem_http_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _listsService.RemoveTaskFromListAsync(listId, taskId)
            );
        }

        [Fact]
        public async Task RemoveTaskFromListAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var listId = "list_remove_task_cancel_ex";
            var taskId = "task_rem_cancel_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _listsService.RemoveTaskFromListAsync(listId, taskId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task RemoveTaskFromListAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var listId = "list_remove_task_ct_pass";
            var taskId = "task_rem_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _listsService.RemoveTaskFromListAsync(listId, taskId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"list/{listId}/task/{taskId}",
                expectedToken), Times.Once);
        }

        // --- Tests for CreateListFromTemplateInFolderAsync ---

        [Fact]
        public async Task CreateListFromTemplateInFolderAsync_ValidRequest_ReturnsList()
        {
            // Arrange
            var folderId = "folder_tpl_folder";
            var templateId = "template_abc";
            var request = new CreateListFromTemplateRequest { Name = "New List From Template In Folder" };
            var expectedList = new ClickUpList { Id = "list_tpl_folder_new", Name = "New List From Template In Folder" };

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(
                    $"folder/{folderId}/listTemplate/{templateId}", // Corrected endpoint
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _listsService.CreateListFromTemplateInFolderAsync(folderId, templateId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedList.Id, result.Id);
            _mockApiConnection.Verify(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(
                $"folder/{folderId}/listTemplate/{templateId}", // Corrected endpoint
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateListFromTemplateInFolderAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var folderId = "folder_tpl_f_null_api";
            var templateId = "template_f_null";
            var request = new CreateListFromTemplateRequest { Name = "Null Resp Template List Folder" };
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListFromTemplateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpList)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _listsService.CreateListFromTemplateInFolderAsync(folderId, templateId, request)
            );
        }

        [Fact]
        public async Task CreateListFromTemplateInFolderAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var folderId = "folder_tpl_f_http_ex";
            var templateId = "template_f_http";
            var request = new CreateListFromTemplateRequest { Name = "Http Ex Template List Folder" };
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListFromTemplateRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _listsService.CreateListFromTemplateInFolderAsync(folderId, templateId, request)
            );
        }

        [Fact]
        public async Task CreateListFromTemplateInFolderAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var folderId = "folder_tpl_f_cancel_ex";
            var templateId = "template_f_cancel";
            var request = new CreateListFromTemplateRequest { Name = "Cancel Ex Template List Folder" };
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListFromTemplateRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _listsService.CreateListFromTemplateInFolderAsync(folderId, templateId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task CreateListFromTemplateInFolderAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var folderId = "folder_tpl_f_ct_pass";
            var templateId = "template_f_ct";
            var request = new CreateListFromTemplateRequest { Name = "CT Pass Template List Folder" };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedList = new ClickUpList { Id = "list_tpl_f_ct_new", Name = "CT Pass Template List Folder" };

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(expectedList);

            // Act
            await _listsService.CreateListFromTemplateInFolderAsync(folderId, templateId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(
                $"folder/{folderId}/listTemplate/{templateId}", // Corrected endpoint
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for CreateListFromTemplateInSpaceAsync ---

        [Fact]
        public async Task CreateListFromTemplateInSpaceAsync_ValidRequest_ReturnsList()
        {
            // Arrange
            var spaceId = "space_tpl_space";
            var templateId = "template_xyz";
            var request = new CreateListFromTemplateRequest { Name = "New List From Template In Space" };
            var expectedList = new ClickUpList { Id = "list_tpl_space_new", Name = "New List From Template In Space" };

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(
                    $"space/{spaceId}/listTemplate/{templateId}", // Corrected endpoint
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _listsService.CreateListFromTemplateInSpaceAsync(spaceId, templateId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedList.Id, result.Id);
            _mockApiConnection.Verify(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(
                $"space/{spaceId}/listTemplate/{templateId}", // Corrected endpoint
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateListFromTemplateInSpaceAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var spaceId = "space_tpl_s_null_api";
            var templateId = "template_s_null";
            var request = new CreateListFromTemplateRequest { Name = "Null Resp Template List Space" };
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListFromTemplateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpList)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _listsService.CreateListFromTemplateInSpaceAsync(spaceId, templateId, request)
            );
        }

        [Fact]
        public async Task CreateListFromTemplateInSpaceAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_tpl_s_http_ex";
            var templateId = "template_s_http";
            var request = new CreateListFromTemplateRequest { Name = "Http Ex Template List Space" };
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListFromTemplateRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _listsService.CreateListFromTemplateInSpaceAsync(spaceId, templateId, request)
            );
        }

        [Fact]
        public async Task CreateListFromTemplateInSpaceAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var spaceId = "space_tpl_s_cancel_ex";
            var templateId = "template_s_cancel";
            var request = new CreateListFromTemplateRequest { Name = "Cancel Ex Template List Space" };
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(It.IsAny<string>(), It.IsAny<CreateListFromTemplateRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _listsService.CreateListFromTemplateInSpaceAsync(spaceId, templateId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task CreateListFromTemplateInSpaceAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var spaceId = "space_tpl_s_ct_pass";
            var templateId = "template_s_ct";
            var request = new CreateListFromTemplateRequest { Name = "CT Pass Template List Space" };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedList = new ClickUpList { Id = "list_tpl_s_ct_new", Name = "CT Pass Template List Space" };

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(expectedList);

            // Act
            await _listsService.CreateListFromTemplateInSpaceAsync(spaceId, templateId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateListFromTemplateRequest, ClickUpList>(
                $"space/{spaceId}/listTemplate/{templateId}", // Corrected endpoint
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for GetFolderlessListsAsyncEnumerableAsync ---
        private List<ClickUp.Api.Client.Models.Entities.Lists.List> CreateSampleRawDtoListsForPaging(int count, int pageNum) // Renamed for clarity
        {
            var lists = new List<ClickUp.Api.Client.Models.Entities.Lists.List>();
            for (int i = 0; i < count; i++)
            {
                // Use the corrected CreateSampleClickUpListRaw helper
                lists.Add(CreateSampleClickUpListRaw($"list_p{pageNum}_{i}", $"List Page {pageNum} Item {i}"));
            }
            return lists;
        }

        [Fact]
        public async Task GetFolderlessListsAsyncEnumerableAsync_ReturnsAllLists_WhenMultiplePages()
        {
            // Arrange
            var spaceId = "space_page_multi";
            var firstPageLists = CreateSampleRawDtoListsForPaging(2, 0); // Page 0
            var secondPageLists = CreateSampleRawDtoListsForPaging(1, 1); // Page 1

            _mockApiConnection.SetupSequence(api => api.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListsResponse(firstPageLists)) // Page 0 returns 2 items
                .ReturnsAsync(new GetListsResponse(secondPageLists)) // Page 1 returns 1 item
                .ReturnsAsync(new GetListsResponse(new List<ClickUp.Api.Client.Models.Entities.Lists.List>()));  // Page 2 returns 0 items (empty list signifies end)

            var allLists = new List<ClickUpList>();

            // Act
            await foreach (var list in _listsService.GetFolderlessListsAsyncEnumerableAsync(spaceId, cancellationToken: CancellationToken.None))
            {
                allLists.Add(list);
            }

            // Assert
            Assert.Equal(3, allLists.Count);
            Assert.Contains(allLists, l => l.Id == "list_p0_0");
            Assert.Contains(allLists, l => l.Id == "list_p0_1");
            Assert.Contains(allLists, l => l.Id == "list_p1_0");

            _mockApiConnection.Verify(api => api.GetAsync<GetListsResponse>(
                It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(api => api.GetAsync<GetListsResponse>(
                It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=1")),
                It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(api => api.GetAsync<GetListsResponse>(
                It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=2")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderlessListsAsyncEnumerableAsync_WithArchivedTrue_BuildsCorrectUrl()
        {
            var spaceId = "space_page_archived";
             _mockApiConnection.Setup(api => api.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListsResponse(new List<ClickUp.Api.Client.Models.Entities.Lists.List>())); // Empty list to terminate quickly

            await foreach (var _ in _listsService.GetFolderlessListsAsyncEnumerableAsync(spaceId, archived: true, cancellationToken: CancellationToken.None)) { }

            _mockApiConnection.Verify(api => api.GetAsync<GetListsResponse>(
                It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("archived=true") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task GetFolderlessListsAsyncEnumerableAsync_ReturnsEmpty_WhenNoLists()
        {
            // Arrange
            var spaceId = "space_page_empty";
            _mockApiConnection.Setup(api => api.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListsResponse(new List<ClickUp.Api.Client.Models.Entities.Lists.List>())); // Empty list signifies end

            var count = 0;

            // Act
            await foreach (var _ in _listsService.GetFolderlessListsAsyncEnumerableAsync(spaceId, cancellationToken: CancellationToken.None))
            {
                count++;
            }

            // Assert
            Assert.Equal(0, count);
            _mockApiConnection.Verify(api => api.GetAsync<GetListsResponse>(
                It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderlessListsAsyncEnumerableAsync_HandlesCancellation()
        {
            // Arrange
            var spaceId = "space_page_cancel";
            var firstPageLists = CreateSampleRawDtoListsForPaging(2, 0); // Corrected method name
            var cts = new CancellationTokenSource();

            _mockApiConnection.Setup(api => api.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListsResponse(firstPageLists));

            var listsProcessed = 0;

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var list in _listsService.GetFolderlessListsAsyncEnumerableAsync(spaceId, cancellationToken: cts.Token))
                {
                    listsProcessed++;
                    if (listsProcessed == 1)
                    {
                        cts.Cancel();
                    }
                }
            });

            Assert.Equal(1, listsProcessed);
            _mockApiConnection.Verify(api => api.GetAsync<GetListsResponse>(
                It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderlessListsAsyncEnumerableAsync_HandlesApiError()
        {
            // Arrange
            var spaceId = "space_page_api_error";
            _mockApiConnection.Setup(api => api.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await foreach (var _ in _listsService.GetFolderlessListsAsyncEnumerableAsync(spaceId, cancellationToken: CancellationToken.None))
                {
                    // Should not reach here
                }
            });
            _mockApiConnection.Verify(api => api.GetAsync<GetListsResponse>(
                It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderlessListsAsyncEnumerableAsync_ApiReturnsNullResponse_StopsIteration()
        {
            // Arrange
            var spaceId = "space_page_null_resp";
            _mockApiConnection.Setup(api => api.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetListsResponse)null); // Null response

            var count = 0;
            // Act
            await foreach (var _ in _listsService.GetFolderlessListsAsyncEnumerableAsync(spaceId, cancellationToken: CancellationToken.None))
            {
                count++;
            }
            // Assert
            Assert.Equal(0, count); // Should not yield any items
            _mockApiConnection.Verify(api => api.GetAsync<GetListsResponse>(
                It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderlessListsAsyncEnumerableAsync_ApiReturnsResponseWithNullLists_StopsIteration()
        {
            // Arrange
            var spaceId = "space_page_null_lists_data";
             _mockApiConnection.Setup(api => api.GetAsync<GetListsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetListsResponse(null!)); // Response with null Lists

            var count = 0;
            // Act
            await foreach (var _ in _listsService.GetFolderlessListsAsyncEnumerableAsync(spaceId, cancellationToken: CancellationToken.None))
            {
                count++;
            }
            // Assert
            Assert.Equal(0, count); // Should not yield any items
            _mockApiConnection.Verify(api => api.GetAsync<GetListsResponse>(
                It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
