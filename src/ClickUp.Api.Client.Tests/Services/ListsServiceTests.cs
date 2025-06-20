using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities; // For ClickUpList DTO
using ClickUp.Api.Client.Models.RequestModels.Lists;
using ClickUp.Api.Client.Models.ResponseModels.Lists;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ClickUp.Api.Client.Tests.Services
{
    public class ListsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly ListsService _listsService;

        public ListsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _listsService = new ListsService(_mockApiConnection.Object);
        }

        // Helper to create a sample ClickUpList for tests
        private ClickUpList CreateSampleList(string id, string name)
        {
            // This is a simplified constructor. Actual ClickUpList DTO will have many more properties.
            // For testing equivalence, only relevant properties need to be set if using BeEquivalentTo.
            // Using reflection to set properties if no suitable constructor is available for tests.
            // For now, let's assume a simple record structure or that we can construct it.
            // If ClickUpList is complex, more elaborate test data setup might be needed.
            // This is a placeholder. Actual construction depends on the DTO definition.
            var list = (ClickUpList)Activator.CreateInstance(typeof(ClickUpList), nonPublic: true)!;
            // Use reflection or a test helper to set properties if they are init-only or private set
            typeof(ClickUpList).GetProperty("Id")?.SetValue(list, id);
            typeof(ClickUpList).GetProperty("Name")?.SetValue(list, name);
            // Set other properties as needed for your tests, e.g., Orderindex, Status, etc.
            return list;
        }

        // --- GetListAsync Tests ---
        [Fact]
        public async Task GetListAsync_WhenListExists_ReturnsList()
        {
            // Arrange
            var listId = "test-list-id";
            var expectedList = CreateSampleList(listId, "Test List");

            _mockApiConnection.Setup(c => c.GetAsync<ClickUpList>(
                $"list/{listId}",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _listsService.GetListAsync(listId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedList);
            _mockApiConnection.Verify(c => c.GetAsync<ClickUpList>(
                $"list/{listId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListAsync_WhenApiReturnsNull_ReturnsNull()
        {
            // Arrange
            var listId = "another-list-id";
            _mockApiConnection.Setup(c => c.GetAsync<ClickUpList>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUpList?)null);

            // Act
            var result = await _listsService.GetListAsync(listId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        // --- CreateListInFolderAsync Tests --- (Example, similar for CreateFolderlessListAsync)
        [Fact]
        public async Task CreateListInFolderAsync_ValidRequest_ReturnsCreatedList()
        {
            // Arrange
            var folderId = "test-folder-id";
            var requestDto = new CreateListRequest("New Test List");
            var expectedList = CreateSampleList("new-list-id", "New Test List");

            _mockApiConnection.Setup(c => c.PostAsync<CreateListRequest, ClickUpList>(
                $"folder/{folderId}/list",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _listsService.CreateListInFolderAsync(folderId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedList);
            _mockApiConnection.Verify(c => c.PostAsync<CreateListRequest, ClickUpList>(
                $"folder/{folderId}/list",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- UpdateListAsync Tests ---
        [Fact]
        public async Task UpdateListAsync_ValidRequest_ReturnsUpdatedList()
        {
            // Arrange
            var listId = "list-to-update";
            var requestDto = new UpdateListRequest { Name = "Updated List Name" }; // Assuming settable Name
            var expectedList = CreateSampleList(listId, "Updated List Name");

            _mockApiConnection.Setup(c => c.PutAsync<UpdateListRequest, ClickUpList>(
                $"list/{listId}",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _listsService.UpdateListAsync(listId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedList);
            _mockApiConnection.Verify(c => c.PutAsync<UpdateListRequest, ClickUpList>(
                $"list/{listId}",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- DeleteListAsync Tests ---
        [Fact]
        public async Task DeleteListAsync_ValidId_CallsApiConnectionDeleteAsync()
        {
            // Arrange
            var listId = "list-to-delete";
            var expectedEndpoint = $"list/{listId}";
            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _listsService.DeleteListAsync(listId, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- GetFolderlessListsAsyncEnumerableAsync Tests ---
        [Fact]
        public async Task GetFolderlessListsAsyncEnumerableAsync_WhenApiReturnsMultiplePages_YieldsAllLists()
        {
            // Arrange
            var spaceId = "space-for-folderless-lists";
            var list1 = CreateSampleList("flist1", "Folderless List 1");
            var list2 = CreateSampleList("flist2", "Folderless List 2");
            var list3 = CreateSampleList("flist3", "Folderless List 3");

            var page0Response = new GetListsResponse(new List<ClickUpList> { list1, list2 }); // Assume LastPage is implicitly handled or not part of this DTO
            var page1Response = new GetListsResponse(new List<ClickUpList> { list3 });
            var emptyPageResponse = new GetListsResponse(new List<ClickUpList>());


            _mockApiConnection.SetupSequence(c => c.GetAsync<GetListsResponse>(It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=0")), It.IsAny<CancellationToken>()))
                .ReturnsAsync(page0Response);
            _mockApiConnection.SetupSequence(c => c.GetAsync<GetListsResponse>(It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=1")), It.IsAny<CancellationToken>()))
                .ReturnsAsync(page1Response);
            _mockApiConnection.SetupSequence(c => c.GetAsync<GetListsResponse>(It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=2")), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyPageResponse); // To terminate the loop

            var collectedLists = new List<ClickUpList>();

            // Act
            await foreach (var list in _listsService.GetFolderlessListsAsyncEnumerableAsync(spaceId, cancellationToken: CancellationToken.None))
            {
                collectedLists.Add(list);
            }

            // Assert
            collectedLists.Should().HaveCount(3);
            collectedLists.Should().ContainEquivalentOf(list1);
            collectedLists.Should().ContainEquivalentOf(list2);
            collectedLists.Should().ContainEquivalentOf(list3);
            _mockApiConnection.Verify(c => c.GetAsync<GetListsResponse>(It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=0")), It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(c => c.GetAsync<GetListsResponse>(It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=1")), It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(c => c.GetAsync<GetListsResponse>(It.Is<string>(s => s.Contains($"space/{spaceId}/list") && s.Contains("page=2")), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
