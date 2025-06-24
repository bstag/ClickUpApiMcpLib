using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Abstractions.Services.Folders;
using ClickUp.Api.Client.Abstractions.Services.Spaces;
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
        private string _testSpaceId; // Will be created for the test class
        private string _testFolderId; // Will be created for the test class

        private List<string> _createdListIds = new List<string>();

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
            _output.LogInformation("Starting ListServiceIntegrationTests class initialization: Creating shared test resources (Space, Folder).");
            try
            {
                // 1. Create a Test Space
                var spaceName = $"TestSpace_Lists_{Guid.NewGuid()}";
                _output.LogInformation($"Creating test space: {spaceName} in Workspace ID: {_testWorkspaceId}");
                // Corrected CreateSpaceRequest instantiation
                var createSpaceReq = new ClickUp.Api.Client.Models.RequestModels.Spaces.CreateSpaceRequest(spaceName, null, null);
                var space = await _spaceService.CreateSpaceAsync(_testWorkspaceId, createSpaceReq);
                _testSpaceId = space.Id;
                _output.LogInformation($"Test space created successfully. Space ID: {_testSpaceId}");

                // 2. Create a Test Folder in that Space
                var folderName = $"TestFolder_Lists_{Guid.NewGuid()}";
                _output.LogInformation($"Creating test folder: {folderName} in Space ID: {_testSpaceId}");
                var createFolderReq = new CreateFolderRequest(folderName);
                var folder = await _folderService.CreateFolderAsync(_testSpaceId, createFolderReq);
                _testFolderId = folder.Id;
                _output.LogInformation($"Test folder created successfully. Folder ID: {_testFolderId}");
            }
            catch (Exception ex)
            {
                _output.LogError($"Error during InitializeAsync: {ex.Message}", ex);
                await CleanupLingeringResourcesAsync(); // Attempt cleanup
                throw;
            }
        }

        public async Task DisposeAsync()
        {
            _output.LogInformation("Starting ListServiceIntegrationTests class disposal: Cleaning up created lists and shared resources.");
            foreach (var listId in _createdListIds)
            {
                try
                {
                    _output.LogInformation($"Deleting list: {listId}");
                    await _listService.DeleteListAsync(listId);
                }
                catch (Exception ex)
                {
                    _output.LogError($"Error deleting list {listId}: {ex.Message}", ex);
                }
            }
            _createdListIds.Clear();
            await CleanupLingeringResourcesAsync();
            _output.LogInformation("ListServiceIntegrationTests class disposal complete.");
        }

        private async Task CleanupLingeringResourcesAsync()
        {
            // Delete folder (which should delete lists within it if API behaves as expected, but we delete lists explicitly first)
            if (!string.IsNullOrWhiteSpace(_testFolderId))
            {
                try
                {
                    _output.LogInformation($"Deleting test folder: {_testFolderId}");
                    await _folderService.DeleteFolderAsync(_testFolderId);
                    _testFolderId = null; // Mark as deleted
                }
                catch (Exception ex)
                {
                    _output.LogError($"Error deleting folder {_testFolderId}: {ex.Message}", ex);
                }
            }

            // Delete space
            if (!string.IsNullOrWhiteSpace(_testSpaceId))
            {
                try
                {
                    _output.LogInformation($"Deleting test space: {_testSpaceId}");
                    await _spaceService.DeleteSpaceAsync(_testSpaceId);
                    _testSpaceId = null; // Mark as deleted
                }
                catch (Exception ex)
                {
                    _output.LogError($"Error deleting space {_testSpaceId}: {ex.Message}", ex);
                }
            }
        }


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
            var createListRequest = new CreateListRequest(listName);

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
            var createListRequest = new CreateListRequest(listName);

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
            var createdList = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest(listName));
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
            var createdList = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest(initialName));
            RegisterCreatedList(createdList.Id);
            _output.LogInformation($"List created for Update test. ID: {createdList.Id}, Name: {createdList.Name}");

            var updatedName = $"Updated List Name - {Guid.NewGuid()}";
            // Corrected UpdateListRequest instantiation
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
            var createdList = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest(listName));
            // Do NOT register for auto-cleanup, this test handles deletion.
            _output.LogInformation($"List created for Delete test. ID: {createdList.Id}");

            await _listService.DeleteListAsync(createdList.Id);
            _output.LogInformation($"DeleteListAsync called for list ID: {createdList.Id}.");

            await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _listService.GetListAsync(createdList.Id)
            );
            _output.LogInformation($"Verified list {createdList.Id} is deleted (GetListAsync threw NotFound).");
        }
    }
}
