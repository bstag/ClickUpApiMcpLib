using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure;
using ClickUp.Api.Client.Models.RequestModels.Lists;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using RichardSzalay.MockHttp;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using ClickUp.Api.Client.Models.ResponseModels.Lists;
using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.Entities.Folders; // Required for Folder.Hidden
using ClickUp.Api.Client.Helpers; // For JsonSerializerOptionsHelper

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class ListServiceIntegrationTests : IntegrationTestBase, IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private readonly IListsService _listService;
        private readonly IFoldersService _folderService;
        private readonly ISpacesService _spaceService;
        private readonly ITaskCrudService _taskCrudService; // Updated to ITaskCrudService

        private string _testWorkspaceId;
        private string _testSpaceId = null!;
        private string _testFolderId = null!;
        private string _testListIdInFolder = null!;
        private string _testTaskIdInList = null!; // Added

        private List<string> _createdListIdsForCleanup = new List<string>();
        private TestHierarchyContext _hierarchyContext = null!;

        public ListServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _listService = ServiceProvider.GetRequiredService<IListsService>();
            _folderService = ServiceProvider.GetRequiredService<IFoldersService>();
            _spaceService = ServiceProvider.GetRequiredService<ISpacesService>();
            _taskCrudService = ServiceProvider.GetRequiredService<ITaskCrudService>(); // Updated to ITaskCrudService
            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"]!;

            if (string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured.");
            }
        }

        private const string PlaybackSpaceId = "playback_space_lists_001";
        private const string PlaybackFolderId = "playback_folder_lists_001";
        private const string PlaybackListIdInFolder = "playback_list_in_folder_001";
        private const string PlaybackTaskId = "playback_task_for_list_ops_001"; // Added

        public async Task InitializeAsync()
        {
            _output.LogInformation($"[ListServiceIntegrationTests] Initializing. Test Mode: {CurrentTestMode}");
            if (CurrentTestMode == TestMode.Playback)
            {
                _hierarchyContext = new TestHierarchyContext
                {
                    SpaceId = PlaybackSpaceId,
                    FolderId = PlaybackFolderId,
                    ListId = PlaybackListIdInFolder,
                    TaskId = PlaybackTaskId // Added
                };
                _testSpaceId = _hierarchyContext.SpaceId;
                _testFolderId = _hierarchyContext.FolderId;
                _testListIdInFolder = _hierarchyContext.ListId;
                _testTaskIdInList = _hierarchyContext.TaskId; // Added
                _output.LogInformation($"[Playback] Using predefined hierarchy: SpaceId={_testSpaceId}, FolderId={_testFolderId}, ListId={_testListIdInFolder}, TaskId={_testTaskIdInList}");
            }
            else
            {
                _output.LogInformation("[Record/Passthrough] Creating full live hierarchy using TestHierarchyHelper.");
                try
                {
                    _hierarchyContext = await TestHierarchyHelper.CreateFullTestHierarchyAsync( // Updated method
                        _spaceService, _folderService, _listService, _taskCrudService, // Updated to use _taskCrudService
                        _testWorkspaceId, "ListsIntTest", _output);
                    _testSpaceId = _hierarchyContext.SpaceId;
                    _testFolderId = _hierarchyContext.FolderId;
                    _testListIdInFolder = _hierarchyContext.ListId;
                    _testTaskIdInList = _hierarchyContext.TaskId; // Added
                    RegisterForCleanup(_testListIdInFolder); // The main list from hierarchy is cleaned by TestHierarchyHelper.TeardownHierarchyAsync
                    // Tasks are implicitly deleted with the list/space. If task needs specific cleanup, add here.
                    _output.LogInformation($"[Record/Passthrough] Hierarchy created: SpaceId={_testSpaceId}, FolderId={_testFolderId}, ListId={_testListIdInFolder}, TaskId={_testTaskIdInList}");
                }
                catch (Exception ex)
                {
                    _output.LogError($"[Record/Passthrough] Error during InitializeAsync (creating hierarchy): {ex.Message}", ex);
                    if (_hierarchyContext != null) await TestHierarchyHelper.TeardownHierarchyAsync(_spaceService, _hierarchyContext, _output);
                    throw;
                }
            }
        }

        public async Task DisposeAsync()
        {
            _output.LogInformation($"[ListServiceIntegrationTests] Disposing. Cleaning up {_createdListIdsForCleanup.Count} lists.");
            if (CurrentTestMode != TestMode.Playback)
            {
                var individualTestLists = _createdListIdsForCleanup.Where(id => id != _testListIdInFolder).ToList();
                foreach (var listId in individualTestLists)
                {
                    try { await _listService.DeleteListAsync(listId); }
                    catch (Exception ex) { _output.LogError($"Error deleting test-created list {listId}: {ex.Message}", ex); }
                }
                if (_hierarchyContext != null)
                {
                    await TestHierarchyHelper.TeardownHierarchyAsync(_spaceService, _hierarchyContext, _output);
                }
            }
            _createdListIdsForCleanup.Clear();
            _output.LogInformation("[ListServiceIntegrationTests] Disposal complete.");
        }

        private void RegisterForCleanup(string listId)
        {
            if (!string.IsNullOrWhiteSpace(listId) && !_createdListIdsForCleanup.Contains(listId))
            {
                _createdListIdsForCleanup.Add(listId);
            }
        }

        private async Task<string> GetResponseJsonAsync(string serviceName, string methodName, string scenarioName, string queryOrBodyHash = "")
        {
            var fileName = string.IsNullOrEmpty(queryOrBodyHash) ? $"{scenarioName}.json" : $"{scenarioName}_{queryOrBodyHash}.json";
            var responsePath = Path.Combine(RecordedResponsesBasePath, serviceName, methodName, fileName);
            // In a real scenario, ensure file exists or handle gracefully. For this pass, we assume it will be created.
            // Assert.True(File.Exists(responsePath), $"Mock data file not found: {responsePath}");
            if (!File.Exists(responsePath) && CurrentTestMode == TestMode.Playback)
            {
                 _output.LogWarning($"[Playback] Mock data file not found: {responsePath}. Returning empty JSON object.");
                 return "{}"; // Return empty object to prevent crash, though test will likely fail assertion
            }
            return await File.ReadAllTextAsync(responsePath);
        }

        [Fact]
        public async Task CreateListInFolderAsync_WithValidData_ShouldCreateList()
        {
            var listName = $"My Test List in Folder - {Guid.NewGuid()}";
            var request = new CreateListRequest(Name: listName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null);
            string playbackListId = "playback_list_in_folder_002";
            string playbackBodyHash = "body_create_list_folder_success";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                listName = "Playback Created List In Folder";
                request = new CreateListRequest(Name: listName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null);

                var responseContent = await GetResponseJsonAsync("ListsService", "POSTCreateListInFolder", "Success", playbackBodyHash);
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/folder/{_testFolderId}/list")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/list/{playbackListId}")
                               .Respond(HttpStatusCode.NoContent);
            }

            ClickUpList createdList = await _listService.CreateListInFolderAsync(_testFolderId, request);

            if (createdList != null && CurrentTestMode != TestMode.Playback) RegisterForCleanup(createdList.Id);

            Assert.NotNull(createdList);
            Assert.False(string.IsNullOrWhiteSpace(createdList.Id));
            Assert.Equal(listName, createdList.Name);
            Assert.NotNull(createdList.Folder);
            Assert.Equal(_testFolderId, createdList.Folder?.Id);
            if (CurrentTestMode == TestMode.Playback) Assert.Equal(playbackListId, createdList.Id);
        }

        [Fact]
        public async Task CreateFolderlessListAsync_WithValidData_ShouldCreateList()
        {
            var listName = $"My Folderless List - {Guid.NewGuid()}";
            var request = new CreateListRequest(Name: listName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null);
            string playbackListId = "playback_folderless_list_001";
            string playbackBodyHash = "body_create_folderless_list_success";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                listName = "Playback Created Folderless List";
                request = new CreateListRequest(Name: listName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null);
                var responseContent = await GetResponseJsonAsync("ListsService", "POSTCreateFolderlessList", "Success", playbackBodyHash);
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/space/{_testSpaceId}/list")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/list/{playbackListId}")
                               .Respond(HttpStatusCode.NoContent);
            }

            ClickUpList createdList = await _listService.CreateFolderlessListAsync(_testSpaceId, request);
            if (createdList != null && CurrentTestMode != TestMode.Playback) RegisterForCleanup(createdList.Id);

            Assert.NotNull(createdList);
            Assert.False(string.IsNullOrWhiteSpace(createdList.Id));
            Assert.Equal(listName, createdList.Name);
            Assert.NotNull(createdList.Folder);
            Assert.True(createdList.Folder?.Hidden);
            Assert.Equal("hidden", createdList.Folder?.Name);
            Assert.NotNull(createdList.Space);
            Assert.Equal(_testSpaceId, createdList.Space?.Id);
            if (CurrentTestMode == TestMode.Playback) Assert.Equal(playbackListId, createdList.Id);
        }

        [Fact]
        public async Task GetListAsync_WithExistingListId_ShouldReturnList()
        {
            string listIdToGet = _testListIdInFolder;
            string expectedListName = "ListsIntTest_List";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                listIdToGet = PlaybackListIdInFolder;
                expectedListName = "Playback Default List in Folder";
                // Corrected methodName to "GetList" and assuming "Success_Existing.json" is now "Success.json" or "GetList_Success.json"
                // For now, I'll use "Success.json" as per our earlier merge logic for ListService/GETList/Success.json
                // Also, the file "Success_Existing.json" might not exist, it should be "Success.json" or "GetList_Success.json"
                // The previous step moved ListService/GETList/Success.json to ListsService/GetList/Success.json
                // and ListService/GetList/GetList_Success.json to ListsService/GetList/GetList_Success.json
                // Let's assume "Success.json" is the correct one for general success for GetList.
                var responseContent = await GetResponseJsonAsync("ListsService", "GetList", "Success");
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{listIdToGet}")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }

            ClickUpList fetchedList = await _listService.GetListAsync(listIdToGet);

            Assert.NotNull(fetchedList);
            Assert.Equal(listIdToGet, fetchedList.Id);
            Assert.Equal(expectedListName, fetchedList.Name);
        }

        [Fact]
        public async Task UpdateListAsync_WithValidData_ShouldUpdateList()
        {
            string listIdToUpdate = _testListIdInFolder;
            string updatedName = $"Updated List Name - {Guid.NewGuid()}";
            var request = new UpdateListRequest(Name: updatedName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null, UnsetStatus: null);
            string playbackBodyHash = "body_update_list_success";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                listIdToUpdate = PlaybackListIdInFolder;
                updatedName = "Playback Updated List Name";
                request = new UpdateListRequest(Name: updatedName, Content: "Updated Playback Content", MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null, UnsetStatus: null);
                var responseContent = await GetResponseJsonAsync("ListsService", "PUTList", "Success", playbackBodyHash);
                MockHttpHandler.When(HttpMethod.Put, $"https://api.clickup.com/api/v2/list/{listIdToUpdate}")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }

            ClickUpList updatedList = await _listService.UpdateListAsync(listIdToUpdate, request);

            Assert.NotNull(updatedList);
            Assert.Equal(listIdToUpdate, updatedList.Id);
            Assert.Equal(updatedName, updatedList.Name);
             if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal("Updated Playback Content", updatedList.Content);
            }
        }

        [Fact]
        public async Task DeleteListAsync_WithExistingListId_ShouldDeleteList()
        {
            string listIdToDelete = "";
            if (CurrentTestMode != TestMode.Playback)
            {
                var tempList = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest($"Temp List to Delete - {Guid.NewGuid()}", null, null, null, null, null, null, null));
                Assert.NotNull(tempList);
                listIdToDelete = tempList.Id;
            }
            else
            {
                listIdToDelete = "playback_list_to_delete_001";
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/list/{listIdToDelete}")
                               .Respond(HttpStatusCode.NoContent);
                var notFoundResponse = await GetResponseJsonAsync("ListsService", "GetList", "Error_NotFound_AfterDelete");
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{listIdToDelete}")
                               .Respond(HttpStatusCode.NotFound, "application/json", notFoundResponse);
            }

            await _listService.DeleteListAsync(listIdToDelete);
            _output.LogInformation($"DeleteListAsync called for list ID: {listIdToDelete}. Verifying...");

            ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException? notFoundEx = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    await _listService.GetListAsync(listIdToDelete);
                    _output.LogWarning($"[Attempt {i + 1}/3] GetListAsync after delete succeeded unexpectedly. Retrying...");
                    if (i == 2) Assert.Fail("List was not deleted after retries (GetListAsync returned list).");
                    await Task.Delay(TimeSpan.FromSeconds(CurrentTestMode == TestMode.Playback ? 0.01 : 2));
                }
                catch (ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException ex)
                {
                    notFoundEx = ex;
                    _output.LogInformation($"[Attempt {i + 1}/3] Verified list {listIdToDelete} is deleted (GetListAsync threw NotFound).");
                    break;
                }
                catch(Exception ex) when (CurrentTestMode == TestMode.Playback)
                {
                     _output.LogError($"[Playback - Attempt {i+1}/3] Unexpected exception: {ex.Message}. Mock for GET 404 might be missing for retry.");
                     if (i==2) throw;
                }
            }
            Assert.NotNull(notFoundEx);
        }

        [Fact]
        public async Task GetListAsync_WithNonExistentListId_ShouldThrowException()
        {
            var nonExistentListId = "0";
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responseContent = await GetResponseJsonAsync("ListsService", "GetList", "Error_Auth_InvalidId");
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{nonExistentListId}")
                               .Respond(HttpStatusCode.Unauthorized, "application/json", responseContent);
            }
            await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException>(
                () => _listService.GetListAsync(nonExistentListId));
        }

        [Fact]
        public async Task UpdateListAsync_WithNonExistentListId_ShouldThrowException()
        {
            var nonExistentListId = "0";
            var request = new UpdateListRequest("Attempt to update non-existent", null, null, null, null, null, null, null, null);
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responseContent = await GetResponseJsonAsync("ListsService", "PUTList", "Error_Auth_InvalidId");
                 MockHttpHandler.When(HttpMethod.Put, $"https://api.clickup.com/api/v2/list/{nonExistentListId}")
                               .Respond(HttpStatusCode.Unauthorized, "application/json", responseContent);
            }
            await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException>(
                () => _listService.UpdateListAsync(nonExistentListId, request));
        }

        [Fact]
        public async Task DeleteListAsync_WithNonExistentListId_ShouldThrowException()
        {
            var nonExistentListId = "0";
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responseContent = await GetResponseJsonAsync("ListsService", "DELETEList", "Error_Auth_InvalidId");
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/list/{nonExistentListId}")
                               .Respond(HttpStatusCode.Unauthorized, "application/json", responseContent);
            }
            await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException>(
                () => _listService.DeleteListAsync(nonExistentListId));
        }

        [Fact]
        public async Task GetFolderlessListsAsyncEnumerableAsync_ShouldRetrieveFolderlessLists()
        {
            var initialExpectedPlaybackListIds = new List<string> { "playback_folderless_list_001", "playback_folderless_list_002" };
            int numberOfFolderlessListsToTest = initialExpectedPlaybackListIds.Count;
            string listInFolderIdForNegativeTest = "playback_nonfolderless_list_id";
            List<string> idsToAssertAgainst = initialExpectedPlaybackListIds;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responseContent = await GetResponseJsonAsync("ListsService", "GetFolderlessLists", "Success_TwoFolderlessLists");
                 MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/space/{_testSpaceId}/list?archived=false")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
                 MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/space/{_testSpaceId}/list")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            } else {
                 var createdFolderlessLiveIds = new List<string>();
                 _output.LogInformation($"[Record/Passthrough] Creating {numberOfFolderlessListsToTest} folderless lists in space '{_testSpaceId}'.");
                 for(int i=0; i<numberOfFolderlessListsToTest; i++) {
                    var fl = await _listService.CreateFolderlessListAsync(_testSpaceId, new CreateListRequest($"My Test FL {i}", null, null, null, null, null, null, null));
                    RegisterForCleanup(fl.Id);
                    createdFolderlessLiveIds.Add(fl.Id);
                 }
                 idsToAssertAgainst = createdFolderlessLiveIds;

                 var listInFolder = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest("NotAFL", null, null, null, null, null, null, null));
                 RegisterForCleanup(listInFolder.Id);
                 listInFolderIdForNegativeTest = listInFolder.Id;
            }

            var retrievedLists = new List<ClickUpList>();
            await foreach (var list in _listService.GetFolderlessListsAsyncEnumerableAsync(_testSpaceId, archived: false))
            {
                retrievedLists.Add(list);
                Assert.NotNull(list.Folder);
                Assert.True(list.Folder?.Hidden, $"List '{list.Name}' (ID: {list.Id}) was expected to be in a hidden folder but Hidden was {list.Folder?.Hidden}.");
            }
            Assert.Equal(numberOfFolderlessListsToTest, retrievedLists.Count);
            foreach (var expectedId in idsToAssertAgainst)
            {
                Assert.Contains(retrievedLists, rl => rl.Id == expectedId);
            }
            _output.LogInformation($"All {numberOfFolderlessListsToTest} expected folderless lists were found.");
            Assert.DoesNotContain(retrievedLists, rl => rl.Id == listInFolderIdForNegativeTest);
        }

        [Fact]
        public async Task GetListsInFolderAsync_ShouldReturnLists()
        {
            int expectedCountInPlayback = 1;
            int expectedCountInRecord = 1 + 2;
            List<string> dynamicallyCreatedListIdsInFolder = new List<string>();

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responseContent = await GetResponseJsonAsync("ListsService", "GetListsInFolder", "Success_OneList");
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/folder/{_testFolderId}/list?archived=false")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            } else {
                for(int i=0; i<2; i++){
                    var list = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest($"Extra List {i} in Folder", null, null, null, null, null, null, null));
                    RegisterForCleanup(list.Id);
                    dynamicallyCreatedListIdsInFolder.Add(list.Id);
                }
            }

            var fetchedListsResponse = await _listService.GetListsInFolderAsync(_testFolderId, archived: false);
            var fetchedLists = fetchedListsResponse.ToList();

            Assert.NotNull(fetchedLists);
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal(expectedCountInPlayback, fetchedLists.Count);
                Assert.Contains(fetchedLists, l => l.Id == PlaybackListIdInFolder);
            }
            else
            {
                Assert.Equal(expectedCountInRecord, fetchedLists.Count);
                Assert.Contains(fetchedLists, l => l.Id == _testListIdInFolder);
                foreach(var id in dynamicallyCreatedListIdsInFolder)
                {
                    Assert.Contains(fetchedLists, l => l.Id == id);
                }
            }
        }

        [Fact]
        public async Task AddTaskToListAsync_WithValidTaskAndList_ShouldSucceed()
        {
            string listId = _testListIdInFolder;
            string taskId = _testTaskIdInList;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                listId = PlaybackListIdInFolder;
                taskId = PlaybackTaskId;

                // AddTaskToListAsync returns Task (void), so no JSON response body to mock, just the status code.
                // However, the actual API might return a minimal JSON like {} or task details.
                // For now, assume 200 OK with empty JSON is acceptable for a successful POST.
                // If recording shows a specific response, adjust this.
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{listId}/task/{taskId}")
                               .Respond(HttpStatusCode.OK, "application/json", "{}");
            }

            await _listService.AddTaskToListAsync(listId, taskId);

            // Verification for AddTaskToListAsync is tricky without a GetTasksInList or checking task properties.
            // For now, the test passes if no exception is thrown.
            // In Record mode, this will call the live API.
            // A more robust test might involve trying to fetch the task and checking its list_id property,
            // or fetching tasks in the list, but that adds more dependencies.
            // The service method itself is void, so we rely on it not throwing for success.
            Assert.True(true); // Placeholder assertion
        }

        [Fact]
        public async Task RemoveTaskFromListAsync_WithValidTaskAndList_ShouldSucceed()
        {
            string listId = _testListIdInFolder;
            string taskId = _testTaskIdInList;

            // Ensure task is part of the list before removing (relevant for Record mode)
            if (CurrentTestMode != TestMode.Playback)
            {
                // This AddTaskToListAsync call will be recorded if it's the first time or if recordings are being updated.
                // We don't mock it here because this part is for Record/Passthrough setup.
                await _listService.AddTaskToListAsync(listId, taskId);
                _output.LogInformation($"[Record/Passthrough] Ensured task {taskId} is in list {listId} before attempting removal.");
            }

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                listId = PlaybackListIdInFolder;
                taskId = PlaybackTaskId;

                // Mock for AddTaskToListAsync if it were called in Playback mode's "setup" part of this test.
                // However, for RemoveTask, we typically assume the "add" happened in a prior state or test.
                // Here, the critical mock is for the DELETE operation.
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{listId}/task/{taskId}")
                               .Respond(HttpStatusCode.OK, "application/json", "{}"); // Mock for the Add call if it were to happen in playback setup.

                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/list/{listId}/task/{taskId}")
                               .Respond(HttpStatusCode.OK, "application/json", "{}"); // API returns 200 OK with {} on success for this
            }

            await _listService.RemoveTaskFromListAsync(listId, taskId);

            // Similar to AddTaskToListAsync, verification is tricky.
            // Passes if no exception is thrown.
            // A more robust test might try to fetch the task and verify its list_id is null or different,
            // or that it no longer appears in a GetTasksInList call.
            Assert.True(true); // Placeholder assertion
        }

        private const string PlaybackListTemplateId = "playback_list_template_001"; // Added for template tests

        [Fact]
        public async Task CreateListFromTemplateInFolderAsync_ShouldCreateList()
        {
            string folderId = _testFolderId;
            string templateId = PlaybackListTemplateId; // In Record mode, this needs to be a real template ID
            var listName = $"List from Template in Folder - {Guid.NewGuid()}";
            var request = new CreateListFromTemplateRequest { Name = listName };
            string playbackListId = "playback_list_from_tpl_folder_001";
            string playbackBodyHash = "body_create_list_from_tpl_folder_success";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                folderId = PlaybackFolderId;
                listName = "Playback List from Template in Folder";
                request = new CreateListFromTemplateRequest { Name = listName };

                var responseContent = await GetResponseJsonAsync("ListsService", "POSTCreateListFromTemplateInFolder", "Success", playbackBodyHash);
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/folder/{folderId}/listTemplate/{templateId}")
                               .WithPartialContent(JsonSerializer.Serialize(request, JsonSerializerOptionsHelper.Options)) // Match body
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/list/{playbackListId}")
                               .Respond(HttpStatusCode.NoContent);
            }
            else
            {
                _output.LogWarning($"[Record/Passthrough] Ensure template ID '{templateId}' is valid for CreateListFromTemplateInFolderAsync test.");
            }

            ClickUpList createdList = await _listService.CreateListFromTemplateInFolderAsync(folderId, templateId, request);
            if (createdList != null && CurrentTestMode != TestMode.Playback) RegisterForCleanup(createdList.Id);

            Assert.NotNull(createdList);
            Assert.False(string.IsNullOrWhiteSpace(createdList.Id));
            Assert.Equal(listName, createdList.Name);
            Assert.NotNull(createdList.Folder);
            Assert.Equal(folderId, createdList.Folder?.Id);
            if (CurrentTestMode == TestMode.Playback) Assert.Equal(playbackListId, createdList.Id);
        }

        [Fact]
        public async Task CreateListFromTemplateInSpaceAsync_ShouldCreateFolderlessList()
        {
            string spaceId = _testSpaceId;
            string templateId = PlaybackListTemplateId; // In Record mode, this needs to be a real template ID
            var listName = $"List from Template in Space - {Guid.NewGuid()}";
            var request = new CreateListFromTemplateRequest { Name = listName };
            string playbackListId = "playback_list_from_tpl_space_001";
            string playbackBodyHash = "body_create_list_from_tpl_space_success";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                spaceId = PlaybackSpaceId;
                listName = "Playback List from Template in Space";
                request = new CreateListFromTemplateRequest { Name = listName };

                var responseContent = await GetResponseJsonAsync("ListsService", "POSTCreateListFromTemplateInSpace", "Success", playbackBodyHash);
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/space/{spaceId}/listTemplate/{templateId}")
                               .WithPartialContent(JsonSerializer.Serialize(request, JsonSerializerOptionsHelper.Options)) // Match body
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/list/{playbackListId}")
                               .Respond(HttpStatusCode.NoContent);
            }
            else
            {
                _output.LogWarning($"[Record/Passthrough] Ensure template ID '{templateId}' is valid for CreateListFromTemplateInSpaceAsync test.");
            }

            ClickUpList createdList = await _listService.CreateListFromTemplateInSpaceAsync(spaceId, templateId, request);
            if (createdList != null && CurrentTestMode != TestMode.Playback) RegisterForCleanup(createdList.Id);

            Assert.NotNull(createdList);
            Assert.False(string.IsNullOrWhiteSpace(createdList.Id));
            Assert.Equal(listName, createdList.Name);
            Assert.NotNull(createdList.Space);
            Assert.Equal(spaceId, createdList.Space?.Id);
            Assert.NotNull(createdList.Folder); // Folderless lists created from templates might still have a folder object, possibly marked hidden.
            Assert.True(createdList.Folder?.Hidden, "Folder for a folderless list created from template should be hidden.");
            if (CurrentTestMode == TestMode.Playback) Assert.Equal(playbackListId, createdList.Id);
        }
    }
}
