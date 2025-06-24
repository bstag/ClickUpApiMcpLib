using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
// Removed incorrect using ClickUp.Api.Client.Abstractions.Services.Folders;
// Removed incorrect using ClickUp.Api.Client.Abstractions.Services.Spaces;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure;
using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using ClickUp.Api.Client.Models.RequestModels.Lists;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class ListServiceIntegrationTests : IntegrationTestBase, IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private readonly IListsService _listService;
        private readonly IFoldersService _folderService;
        private readonly ISpacesService _spaceService;

        private string _testWorkspaceId;
        private string _testSpaceId = null!;
        private string _testFolderId = null!;

        private List<string> _createdListIds = new List<string>();
        private TestHierarchyContext _hierarchyContext = null!;

        public ListServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _listService = ServiceProvider.GetRequiredService<IListsService>();
            _folderService = ServiceProvider.GetRequiredService<IFoldersService>();
            _spaceService = ServiceProvider.GetRequiredService<ISpacesService>();

            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"];

            if (string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured. Test setup for creating spaces will fail.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for ListServiceIntegrationTests.");
            }
        }

        public async Task InitializeAsync()
        {
            _output.LogInformation("Starting ListServiceIntegrationTests class initialization using TestHierarchyHelper.");
            try
            {
                // Create Space and Folder using the helper. Lists will be created by individual tests.
                // We use CreateSpaceFolderListHierarchyAsync and just use its SpaceId and FolderId.
                // Or we could make a CreateSpaceFolderHierarchyAsync if needed often.
                _hierarchyContext = await TestHierarchyHelper.CreateSpaceFolderListHierarchyAsync(
                    _spaceService, _folderService, _listService, // _listService is used by helper but we might not use its list
                    _testWorkspaceId, "ListsTest", _output);

                _testSpaceId = _hierarchyContext.SpaceId;
                _testFolderId = _hierarchyContext.FolderId;
                // _hierarchyContext.ListId is created but might not be the primary list for all tests here.
                // Tests that need a specific list will create it.
                _output.LogInformation($"Hierarchy created: SpaceId={_testSpaceId}, FolderId={_testFolderId}. A default list was also created: {_hierarchyContext.ListId}");
                 RegisterCreatedList(_hierarchyContext.ListId); // Register the default list for cleanup
            }
            catch (Exception ex)
            {
                _output.LogError($"Error during InitializeAsync: {ex.Message}", ex);
                if (_hierarchyContext != null)
                {
                    await TestHierarchyHelper.TeardownHierarchyAsync(_spaceService, _hierarchyContext, _output);
                }
                throw;
            }
        }

        public async Task DisposeAsync()
        {
            _output.LogInformation("Starting ListServiceIntegrationTests class disposal.");

            var listsToClean = new List<string>(_createdListIds);
            _createdListIds.Clear(); // Clear original list before async operations

            foreach (var listId in listsToClean)
            {
                // Don't delete the list that was part of the hierarchy context if it's still there,
                // as TeardownHierarchyAsync will handle it via space deletion.
                // However, lists created OUTSIDE this default list by tests should be cleaned.
                // For simplicity, we'll try to delete all registered lists. If it's already deleted (e.g. by folder/space delete), it should be fine.
                try
                {
                    _output.LogInformation($"Attempting to delete list: {listId}");
                    await _listService.DeleteListAsync(listId);
                    _output.LogInformation($"List {listId} deleted successfully or was already gone.");
                }
                catch (ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException)
                {
                    _output.LogInformation($"List {listId} was not found during cleanup (already deleted).");
                }
                catch (Exception ex)
                {
                    _output.LogError($"Error deleting list {listId}: {ex.Message}", ex);
                }
            }

            if (_hierarchyContext != null)
            {
                await TestHierarchyHelper.TeardownHierarchyAsync(_spaceService, _hierarchyContext, _output);
            }
            _output.LogInformation("ListServiceIntegrationTests class disposal complete.");
        }

        // Removed CleanupLingeringResourcesAsync

        private void RegisterCreatedList(string listId)
        {
            if (!string.IsNullOrWhiteSpace(listId))
            {
                _createdListIds.Add(listId);
            }
        }

        [Fact]
        public async Task CreateListInFolderAsync_WithValidData_ShouldCreateList()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testFolderId), "TestFolderId must be available. Check InitializeAsync.");
            var listName = $"My Test List in Folder - {Guid.NewGuid()}";
            var createListRequest = new CreateListRequest(
                Name: listName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null
            );

            _output.LogInformation($"Attempting to create list '{listName}' in folder '{_testFolderId}'.");
            ClickUpList createdList = null;
            try
            {
                createdList = await _listService.CreateListInFolderAsync(_testFolderId, createListRequest);
                if (createdList != null)
                {
                    RegisterCreatedList(createdList.Id);
                    _output.LogInformation($"List created in folder. ID: {createdList.Id}, Name: {createdList.Name}");
                }
            }
            catch (Exception ex)
            {
                _output.LogError($"Exception during CreateListInFolderAsync: {ex.Message}", ex);
                Assert.Fail($"CreateListInFolderAsync threw an exception: {ex.Message}");
            }

            Assert.NotNull(createdList);
            Assert.False(string.IsNullOrWhiteSpace(createdList.Id));
            Assert.Equal(listName, createdList.Name);
            Assert.Equal(_testFolderId, createdList.Folder?.Id);
        }

        [Fact]
        public async Task CreateFolderlessListAsync_WithValidData_ShouldCreateList()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testSpaceId), "TestSpaceId must be available. Check InitializeAsync.");
            var listName = $"My Folderless Test List - {Guid.NewGuid()}";
            var createListRequest = new CreateListRequest(
                Name: listName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null
            );

            _output.LogInformation($"Attempting to create folderless list '{listName}' in space '{_testSpaceId}'.");
            ClickUpList createdList = null;
            try
            {
                createdList = await _listService.CreateFolderlessListAsync(_testSpaceId, createListRequest);
                if (createdList != null)
                {
                    RegisterCreatedList(createdList.Id);
                    _output.LogInformation($"Folderless list created. ID: {createdList.Id}, Name: {createdList.Name}");
                }
            }
            catch (Exception ex)
            {
                _output.LogError($"Exception during CreateFolderlessListAsync: {ex.Message}", ex);
                Assert.Fail($"CreateFolderlessListAsync threw an exception: {ex.Message}");
            }

            Assert.NotNull(createdList);
            Assert.False(string.IsNullOrWhiteSpace(createdList.Id));
            Assert.Equal(listName, createdList.Name);
            Assert.Null(createdList.Folder?.Id); // Folderless lists shouldn't have a folder ID.
            Assert.Equal(_testSpaceId, createdList.Space?.Id);
        }

        [Fact]
        public async Task GetListAsync_WithExistingListId_ShouldReturnList()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testFolderId), "TestFolderId must be available.");
            var listName = $"My List To Get - {Guid.NewGuid()}";
            var createdList = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest(
                Name: listName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null
            ));
            RegisterCreatedList(createdList.Id);
            _output.LogInformation($"List created for Get test. ID: {createdList.Id}");

            var fetchedList = await _listService.GetListAsync(createdList.Id);
            _output.LogInformation($"Fetched list. ID: {fetchedList?.Id}");

            Assert.NotNull(fetchedList);
            Assert.Equal(createdList.Id, fetchedList.Id);
            Assert.Equal(listName, fetchedList.Name);
        }

        [Fact]
        public async Task UpdateListAsync_WithValidData_ShouldUpdateList()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testFolderId), "TestFolderId must be available.");
            var initialName = $"Initial List Name - {Guid.NewGuid()}";
            var createdList = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest(
                Name: initialName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null
            ));
            RegisterCreatedList(createdList.Id);
            _output.LogInformation($"List created for Update test. ID: {createdList.Id}, Name: {createdList.Name}");

            var updatedName = $"Updated List Name - {Guid.NewGuid()}";
            // UpdateListRequest constructor is: UpdateListRequest(string Name, string? Content, string? MarkdownContent, DateTimeOffset? DueDate, bool? DueDateTime, int? Priority, int? Assignee, string? Status, bool? UnsetStatus)
            // The existing call was correct based on the latest model definition (which includes UnsetStatus).
            // If UpdateListRequest model was different before, this would need change. Assuming it's:
            // Name, Content, MarkdownContent, DueDate, DueDateTime, Priority, Assignee, Status
            var updateListRequest = new UpdateListRequest(
                Name: updatedName,
                Content: null,
                MarkdownContent: null,
                DueDate: null,
                DueDateTime: null,
                Priority: null,
                Assignee: null,
                Status: null,
                UnsetStatus: null
            );

            _output.LogInformation($"Attempting to update list '{createdList.Id}' to name '{updatedName}'.");
            var updatedList = await _listService.UpdateListAsync(createdList.Id, updateListRequest);
            _output.LogInformation($"List updated. ID: {updatedList?.Id}, Name: {updatedList?.Name}");

            Assert.NotNull(updatedList);
            Assert.Equal(createdList.Id, updatedList.Id);
            Assert.Equal(updatedName, updatedList.Name);

            var refetchedList = await _listService.GetListAsync(createdList.Id);
            Assert.Equal(updatedName, refetchedList.Name);
        }

        [Fact]
        public async Task DeleteListAsync_WithExistingListId_ShouldDeleteList()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testFolderId), "TestFolderId must be available.");
            var listName = $"List To Delete - {Guid.NewGuid()}";
            var createdList = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest(
                Name: listName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null
            ));
            // Do NOT register for auto-cleanup, this test handles deletion.
            _output.LogInformation($"List created for Delete test. ID: {createdList.Id}");

            await _listService.DeleteListAsync(createdList.Id);
            _output.LogInformation($"DeleteListAsync called for list ID: {createdList.Id}.");

            await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _listService.GetListAsync(createdList.Id)
            );
            _output.LogInformation($"Verified list {createdList.Id} is deleted (GetListAsync threw NotFound).");
        }

        [Fact]
        public async Task GetListAsync_WithNonExistentListId_ShouldThrowNotFoundException()
        {
            var nonExistentListId = "0"; // Or any ID that's guaranteed not to exist
            _output.LogInformation($"Attempting to get non-existent list with ID: {nonExistentListId}");

            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _listService.GetListAsync(nonExistentListId)
            );

            _output.LogInformation($"Received expected ClickUpApiNotFoundException: {exception.Message}");
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task UpdateListAsync_WithNonExistentListId_ShouldThrowNotFoundException()
        {
            var nonExistentListId = "0";
            var updateRequest = new UpdateListRequest(
                Name: "Attempt to update non-existent list", Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null, UnsetStatus: null
            );
            _output.LogInformation($"Attempting to update non-existent list with ID: {nonExistentListId}");

            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _listService.UpdateListAsync(nonExistentListId, updateRequest)
            );

            _output.LogInformation($"Received expected ClickUpApiNotFoundException: {exception.Message}");
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task DeleteListAsync_WithNonExistentListId_ShouldThrowNotFoundException()
        {
            var nonExistentListId = "0";
            _output.LogInformation($"Attempting to delete non-existent list with ID: {nonExistentListId}");

            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _listService.DeleteListAsync(nonExistentListId)
            );

            _output.LogInformation($"Received expected ClickUpApiNotFoundException: {exception.Message}");
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetFolderlessListsAsyncEnumerableAsync_ShouldRetrieveAllFolderlessListsInSpace()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testSpaceId), "TestSpaceId must be available for this test.");

            int listsToCreate = 3; // Create a few folderless lists
            var createdFolderlessListIds = new List<string>();

            _output.LogInformation($"Creating {listsToCreate} folderless lists for stream test in space '{_testSpaceId}'.");
            for (int i = 0; i < listsToCreate; i++)
            {
                var listName = $"Folderless List {i + 1} - {Guid.NewGuid()}";
                var createReq = new CreateListRequest(Name: listName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null);
                var createdList = await _listService.CreateFolderlessListAsync(_testSpaceId, createReq);
                RegisterCreatedList(createdList.Id); // Ensure they are cleaned up by DisposeAsync
                createdFolderlessListIds.Add(createdList.Id);
                _output.LogInformation($"Created folderless list {i + 1}/{listsToCreate}, ID: {createdList.Id}");
                await Task.Delay(250); // API niceness
            }

            // Also create one list inside the folder to ensure it's NOT returned by GetFolderlessListsAsyncEnumerableAsync
            var listInFolderName = $"List_In_Folder_Not_Folderless_{Guid.NewGuid()}";
            var listInFolder = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest(Name: listInFolderName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null));
            RegisterCreatedList(listInFolder.Id);
            _output.LogInformation($"Created list '{listInFolderName}' (ID: {listInFolder.Id}) inside folder '{_testFolderId}' for negative test.");


            var retrievedLists = new List<ClickUpList>();
            int count = 0;
            _output.LogInformation($"Starting to stream folderless lists for space '{_testSpaceId}'.");

            await foreach (var list in _listService.GetFolderlessListsAsyncEnumerableAsync(_testSpaceId))
            {
                count++;
                retrievedLists.Add(list);
                _output.LogInformation($"Streamed folderless list {count}: ID {list.Id}, Name: '{list.Name}'...");
                Assert.Null(list.Folder?.Id); // Verify it is indeed folderless
                Assert.Equal(_testSpaceId, list.Space?.Id); // Verify it belongs to the correct space
            }

            _output.LogInformation($"Finished streaming folderless lists. Total lists received: {count}");

            // The GetFolderlessLists API endpoint itself is not paginated by `page` or `start_id`.
            // So, the IAsyncEnumerable wrapper will likely retrieve all in one go.
            // The test primarily verifies that it correctly calls the underlying GetFolderlessListsAsync and yields results.
            Assert.Equal(listsToCreate, count);
            Assert.Equal(listsToCreate, retrievedLists.Count);

            foreach (var createdId in createdFolderlessListIds)
            {
                Assert.Contains(retrievedLists, rl => rl.Id == createdId);
            }
            _output.LogInformation($"All {listsToCreate} created folderless lists were found in the streamed results from space '{_testSpaceId}'.");

            // Ensure the list created inside the folder was NOT returned
            Assert.DoesNotContain(retrievedLists, rl => rl.Id == listInFolder.Id);
            _output.LogInformation($"List '{listInFolderName}' (ID: {listInFolder.Id}) which is inside a folder was correctly NOT found in folderless stream.");
        }
    }
}
