using System;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Configuration; // Required for IConfiguration
using ClickUp.Api.Client.Models.Entities.Tasks;
using System.Collections.Generic;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure; // Added for TestOutputHelperExtensions
using ClickUp.Api.Client.Models.RequestModels.Folders; // For CreateFolderRequest
using ClickUp.Api.Client.Models.RequestModels.Lists; // For CreateListRequest
using System.IO; // For Path
using System.Net; // For HttpStatusCode
using System.Net.Http; // For HttpMethod
using System.Web; // For HttpUtility
using RichardSzalay.MockHttp; // For MockHttpMessageHandler specific methods like When
using ClickUp.Api.Client.Models.Common; // For Status
using ClickUp.Api.Client.Models.ResponseModels.Tasks; // For TaskTimeInStatusResponse and GetBulkTasksTimeInStatusResponse
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.Entities.Lists; // For User

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class TaskServiceIntegrationTests : IntegrationTestBase, IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private readonly ITasksService _taskService;
        private readonly IListsService _listService;
        private readonly ISpacesService _spaceService;
        private readonly IFoldersService _folderService;
        private readonly IAuthorizationService _authService; // To get current user ID
        private readonly ITagsService _tagsService; // To create and assign tags

        private string _testWorkspaceId;
        private string _testSpaceId = null!;
        private string _testFolderId = null!;
        private string _testListId = null!;

        private List<string> _createdTaskIds = new List<string>();
        private TestHierarchyContext _hierarchyContext = null!; // For Space, Folder, List

        // Playback Mode Constants
        private const string PlaybackUserId = "playback_user_id_123";
        private const int PlaybackUserIdInt = 123;
        private const string PlaybackSpaceId = "playback_space_id_abc";
        private const string PlaybackFolderId = "playback_folder_id_def";
        private const string PlaybackListId = "playback_list_id_ghi";
        private const string PlaybackDefaultStatusValue = "playback_status_open";
        private const string PlaybackAnotherStatusValue = "playback_status_closed";
        // private const string PlaybackDefaultStatusId = "status_id_open"; // Not directly used for Status record construction
        // private const string PlaybackAnotherStatusId = "status_id_closed"; // Not directly used for Status record construction
        private const string PlaybackDefaultTagName = "playback_tag_1";


        public TaskServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _taskService = ServiceProvider.GetRequiredService<ITasksService>();
            _listService = ServiceProvider.GetRequiredService<IListsService>();
            _spaceService = ServiceProvider.GetRequiredService<ISpacesService>();
            _folderService = ServiceProvider.GetRequiredService<IFoldersService>();
            _authService = ServiceProvider.GetRequiredService<IAuthorizationService>();
            _tagsService = ServiceProvider.GetRequiredService<ITagsService>();

            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"];
            if (CurrentTestMode != TestMode.Playback && string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured. Test setup will fail for non-Playback modes.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for TaskServiceIntegrationTests unless in Playback mode.");
            }
            if (CurrentTestMode == TestMode.Playback && string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _testWorkspaceId = "playback_workspace_id"; // Default for playback if not set
                 _output.LogInformation($"[PLAYBACK] Using default PlaybackWorkspaceId: {_testWorkspaceId}");
            }
        }

        private string _currentUserId = null!;
        private ClickUp.Api.Client.Models.Common.Status _defaultStatus = null!;
        private ClickUp.Api.Client.Models.Common.Status _anotherStatus = null!; // Can be null if only one status exists
        private List<string> _createdTagNamesForCleanup = new List<string>();

        public async Task InitializeAsync()
        {
            _output.LogInformation($"Starting TaskServiceIntegrationTests class initialization (Mode: {CurrentTestMode}).");

            if (CurrentTestMode == TestMode.Playback)
            {
                _output.LogInformation("[PLAYBACK] Setting up mocks for InitializeAsync.");
                Assert.NotNull(MockHttpHandler); // Ensure it's available

                // 1. Mock _authService.GetAuthorizedUserAsync()
                MockHttpHandler.When(HttpMethod.Get, "https://api.clickup.com/api/v2/user")
                               .Respond("application/json", await MockedFileContentAsync("AuthorizationService/GetAuthorizedUser/AuthUser_Playback.json"));
                _currentUserId = PlaybackUserId; // Set directly for playback

                // 2. Mock TestHierarchyHelper.CreateSpaceFolderListHierarchyAsync calls
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/team/{_testWorkspaceId}/space")
                               .Respond("application/json", await MockedFileContentAsync("SpacesService/CreateSpace/Space_Playback.json"));
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/space/{PlaybackSpaceId}/folder")
                               .Respond("application/json", await MockedFileContentAsync("FoldersService/CreateFolder/Folder_Playback.json"));
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/folder/{PlaybackFolderId}/list")
                               .Respond("application/json", await MockedFileContentAsync("ListsService/CreateListInFolder/List_Playback.json"));

                _testSpaceId = PlaybackSpaceId;
                _testFolderId = PlaybackFolderId;
                _testListId = PlaybackListId;
                _hierarchyContext = new TestHierarchyContext // Correctly initialize the class
                {
                    SpaceId = PlaybackSpaceId,
                    FolderId = PlaybackFolderId,
                    ListId = PlaybackListId
                };


                // 3. Mock _listService.GetListAsync(_testListId) for statuses
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{PlaybackListId}")
                               .Respond("application/json", await MockedFileContentAsync("ListsService/GetList/ListDetails_Playback.json"));

                // Manually set statuses for Playback based on mocked JSON content
                _defaultStatus = new ClickUp.Api.Client.Models.Common.Status(PlaybackDefaultStatusValue, "#000000", 0, "open");
                _anotherStatus = new ClickUp.Api.Client.Models.Common.Status(PlaybackAnotherStatusValue, "#111111", 1, "closed");

                _output.LogInformation($"[PLAYBACK] Initialized with Playback IDs: UserId={_currentUserId}, SpaceId={_testSpaceId}, FolderId={_testFolderId}, ListId={_testListId}");
                _output.LogInformation($"[PLAYBACK] Default status: '{_defaultStatus.StatusValue}'. Another status: '{_anotherStatus?.StatusValue}'");
            }
            else // Record or Passthrough
            {
                var user = await _authService.GetAuthorizedUserAsync();
                Assert.NotNull(user); // Ensure user is not null before accessing Id
                _currentUserId = user.Id.ToString();
                _output.LogInformation($"Current user ID: {_currentUserId}");

                _hierarchyContext = await TestHierarchyHelper.CreateSpaceFolderListHierarchyAsync(
                    _spaceService, _folderService, _listService,
                    _testWorkspaceId, "TasksQueryTest", _output);

                _testSpaceId = _hierarchyContext.SpaceId;
                _testFolderId = _hierarchyContext.FolderId;
                _testListId = _hierarchyContext.ListId;
                _output.LogInformation($"Hierarchy created: SpaceId={_testSpaceId}, FolderId={_testFolderId}, ListId={_testListId}");

                var listDetails = await _listService.GetListAsync(_testListId);
                if (listDetails.Statuses != null && listDetails.Statuses.Any())
                {
                    _defaultStatus = listDetails.Statuses.First();
                    _anotherStatus = listDetails.Statuses.Skip(1).FirstOrDefault();
                    _output.LogInformation($"Default status: '{_defaultStatus.StatusValue}'. Another status: '{_anotherStatus?.StatusValue}'");
                }
                else
                {
                    _output.LogWarning("Could not retrieve statuses for the test list. Status filter tests will be impacted.");
                }
            }
        }

        public async Task DisposeAsync()
        {
            _output.LogInformation($"Starting TaskServiceIntegrationTests class disposal (Mode: {CurrentTestMode}).");

            if (CurrentTestMode == TestMode.Playback)
            {
                _output.LogInformation("[PLAYBACK] Setting up mocks for DisposeAsync.");
                Assert.NotNull(MockHttpHandler);

                // Mock for TestHierarchyHelper.TeardownHierarchyAsync -> _spaceService.DeleteSpaceAsync
                if (_hierarchyContext != null && !string.IsNullOrEmpty(_hierarchyContext.SpaceId)) // SpaceId would be PlaybackSpaceId
                {
                    MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/space/{_hierarchyContext.SpaceId}")
                                   .Respond(HttpStatusCode.NoContent);
                    _output.LogInformation($"[PLAYBACK] Mocked DELETE space/{_hierarchyContext.SpaceId}");
                }

                // Mock for any tags that might have been "created" (e.g., PlaybackDefaultTagName)
                if (_createdTagNamesForCleanup.Contains(PlaybackDefaultTagName) && !string.IsNullOrWhiteSpace(_testSpaceId))
                {
                     var encodedTagName = HttpUtility.UrlEncode(PlaybackDefaultTagName);
                     MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/space/{_testSpaceId}/tag/{encodedTagName}")
                                    .Respond(HttpStatusCode.NoContent);
                     _output.LogInformation($"[PLAYBACK] Mocked DELETE tag/{encodedTagName} from space/{_testSpaceId}");
                }
            }

            // Actual cleanup logic (runs in all modes, but API calls are only live in Record/Passthrough)
            var tasksToDelete = new List<string>(_createdTaskIds);
            _createdTaskIds.Clear();
            if (CurrentTestMode != TestMode.Playback) // Only delete if not in playback (mocks handle "deletions")
            {
                foreach (var taskId in tasksToDelete)
                {
                    try { _output.LogInformation($"Deleting task: {taskId}"); await _taskService.DeleteTaskAsync(taskId, new DeleteTaskRequest()); }
                    catch (Exception ex) { _output.LogError($"Error deleting task {taskId}: {ex.Message}", ex); }
                }
            }

            var tagsToCleanup = new List<string>(_createdTagNamesForCleanup);
            _createdTagNamesForCleanup.Clear();
            if (CurrentTestMode != TestMode.Playback) // Only delete if not in playback
            {
                foreach (var tagName in tagsToCleanup)
                {
                    if (!string.IsNullOrWhiteSpace(_testSpaceId) && !string.IsNullOrWhiteSpace(tagName))
                    {
                        try { _output.LogInformation($"Deleting tag '{tagName}' from space '{_testSpaceId}'."); await _tagsService.DeleteSpaceTagAsync(_testSpaceId, tagName); }
                        catch (Exception ex) { _output.LogError($"Error deleting tag '{tagName}': {ex.Message}", ex); }
                    }
                }
            }

            if (_hierarchyContext != null)
            {
                await TestHierarchyHelper.TeardownHierarchyAsync(_spaceService, _hierarchyContext, _output);
            }
            _output.LogInformation("TaskServiceIntegrationTests class disposal complete.");
        }

        private void RegisterCreatedTask(string taskId)
        {
            if (CurrentTestMode != TestMode.Playback && !string.IsNullOrWhiteSpace(taskId))
            {
                _createdTaskIds.Add(taskId);
            }
        }

        private async Task<string> MockedFileContentAsync(string relativePath)
        {
            // In a real scenario, this would read from RecordedResponsesBasePath.
            var fullPath = Path.Combine(RecordedResponsesBasePath, relativePath);
            return await System.IO.File.ReadAllTextAsync(fullPath);
        }


        [Fact]
        public async Task CreateTaskAsync_WithValidData_ShouldCreateTask()
        {
            // Arrange
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            var taskName = $"My Integration Test Task - {(CurrentTestMode == TestMode.Playback ? "Playback" : Guid.NewGuid().ToString())}";
            var taskDescription = "This is a task created by an integration test.";
            var createTaskRequest = new CreateTaskRequest(Name: taskName, Description: taskDescription, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null);

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var mockResponsePath = "TaskService/CreateTaskAsync_WithValidData_ShouldCreateTask/CreateTask_Success.json";
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync(mockResponsePath));
            }
            _output.LogInformation($"Attempting to create task '{taskName}' in list '{_testListId}'.");

            // Act
            CuTask createdTask = null;
            try { createdTask = await _taskService.CreateTaskAsync(_testListId, createTaskRequest); }
            catch (Exception ex) { _output.LogError($"Exception during CreateTaskAsync: {ex.Message}", ex); Assert.Fail($"CreateTaskAsync threw an exception: {ex.Message}"); }

            if (createdTask != null) { RegisterCreatedTask(createdTask.Id); _output.LogInformation($"Task created successfully. ID: {createdTask.Id}, Name: {createdTask.Name}"); }

            // Assert
            Assert.NotNull(createdTask);
            Assert.False(string.IsNullOrWhiteSpace(createdTask.Id));
            Assert.Equal(taskName, createdTask.Name); // Will use "Playback" name if in Playback
            Assert.Equal(taskDescription, createdTask.Description);
            if (CurrentTestMode == TestMode.Playback) Assert.Equal("playback_task_id_createTest_1", createdTask.Id);
        }

        [Fact]
        public async Task GetTaskAsync_WithExistingTaskId_ShouldReturnTask()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            var taskName = $"My Task To Get - {(CurrentTestMode == TestMode.Playback ? "Playback" : Guid.NewGuid().ToString())}";
            var createdTaskId = (CurrentTestMode == TestMode.Playback) ? "playback_task_id_getTest_1" : null;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task") // Mock creation
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTaskAsync_WithExistingTaskId_ShouldReturnTask/CreateTaskForGet_Success.json"));
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/task/{createdTaskId}")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTaskAsync_WithExistingTaskId_ShouldReturnTask/GetTask_Success.json"));
            }

            var createTaskRequest = new CreateTaskRequest(Name: taskName, Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null);
            var taskToGet = await _taskService.CreateTaskAsync(_testListId, createTaskRequest);
            RegisterCreatedTask(taskToGet.Id);
            _output.LogInformation($"Task created for Get test. ID: {taskToGet.Id}");
            if (CurrentTestMode == TestMode.Playback) Assert.Equal(createdTaskId, taskToGet.Id);


            var fetchedTask = await _taskService.GetTaskAsync(taskToGet.Id, new GetTaskRequest());
            _output.LogInformation($"Fetched task. ID: {fetchedTask?.Id}");

            Assert.NotNull(fetchedTask);
            Assert.Equal(taskToGet.Id, fetchedTask.Id);
            Assert.Equal(taskName, fetchedTask.Name);
        }

        [Fact]
        public async Task UpdateTaskAsync_WithValidData_ShouldUpdateTask()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            var initialName = $"Initial Task Name - {(CurrentTestMode == TestMode.Playback ? "Playback" : Guid.NewGuid().ToString())}";
            var updatedName = $"Updated Task Name - {(CurrentTestMode == TestMode.Playback ? "PlaybackUpdate" : Guid.NewGuid().ToString())}";
            var updatedDescription = "This task has been updated by an integration test.";
            var createdTaskId = (CurrentTestMode == TestMode.Playback) ? "playback_task_id_updateTest_1" : null;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/UpdateTaskAsync_WithValidData_ShouldUpdateTask/CreateTaskForUpdate_Success.json"));
                MockHttpHandler.When(HttpMethod.Put, $"https://api.clickup.com/api/v2/task/{createdTaskId}")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/UpdateTaskAsync_WithValidData_ShouldUpdateTask/UpdateTask_Success.json"));
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/task/{createdTaskId}") // For re-fetch
                               .Respond("application/json", await MockedFileContentAsync("TaskService/UpdateTaskAsync_WithValidData_ShouldUpdateTask/GetUpdatedTask_Success.json"));
            }

            var createTaskRequest = new CreateTaskRequest(Name: initialName, Description: "Initial Description", Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null);
            var createdTask = await _taskService.CreateTaskAsync(_testListId, createTaskRequest);
            RegisterCreatedTask(createdTask.Id);
            _output.LogInformation($"Task created for Update test. ID: {createdTask.Id}, Name: {createdTask.Name}");
            if (CurrentTestMode == TestMode.Playback) Assert.Equal(createdTaskId, createdTask.Id);

            var updateTaskRequest = new UpdateTaskRequest(Name: updatedName, Description: updatedDescription, Status: null, Priority: null, DueDate: null, DueDateTime: null, Parent: null, TimeEstimate: null, StartDate: null, StartDateTime: null, Assignees: null, GroupAssignees: null, Archived: null, CustomFields: null);
            var updatedTask = await _taskService.UpdateTaskAsync(createdTask.Id, updateTaskRequest);
            _output.LogInformation($"Task updated. ID: {updatedTask?.Id}, Name: {updatedTask?.Name}");

            Assert.NotNull(updatedTask);
            Assert.Equal(createdTask.Id, updatedTask.Id);
            Assert.Equal(updatedName, updatedTask.Name);
            Assert.Equal(updatedDescription, updatedTask.Description);

            var refetchedTask = await _taskService.GetTaskAsync(createdTask.Id, new GetTaskRequest());
            Assert.Equal(updatedName, refetchedTask.Name);
            Assert.Equal(updatedDescription, refetchedTask.Description);
        }

        [Fact]
        public async Task DeleteTaskAsync_WithExistingTaskId_ShouldDeleteTask()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            var taskName = $"Task To Delete - {(CurrentTestMode == TestMode.Playback ? "Playback" : Guid.NewGuid().ToString())}";
            var createdTaskId = (CurrentTestMode == TestMode.Playback) ? "playback_task_id_deleteTest_1" : null;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/DeleteTaskAsync_WithExistingTaskId_ShouldDeleteTask/CreateTaskForDelete_Success.json"));
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{createdTaskId}")
                               .Respond(HttpStatusCode.NoContent);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/task/{createdTaskId}") // For verification
                               .Respond(HttpStatusCode.NotFound, "application/json", await MockedFileContentAsync("TaskService/DeleteTaskAsync_WithExistingTaskId_ShouldDeleteTask/GetTask_NotFound_AfterDelete.json"));
            }

            var createTaskRequest = new CreateTaskRequest(Name: taskName, Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null);
            var createdTask = await _taskService.CreateTaskAsync(_testListId, createTaskRequest);
            // Do NOT register this task for auto-cleanup if not in playback, as this test is testing the deletion.
            if (CurrentTestMode == TestMode.Playback) Assert.Equal(createdTaskId, createdTask.Id); else _createdTaskIds.Remove(createdTask.Id); // Ensure it's not cleaned up by Dispose if test fails before explicit delete
            _output.LogInformation($"Task created for Delete test. ID: {createdTask.Id}");

            await _taskService.DeleteTaskAsync(createdTask.Id, new DeleteTaskRequest());
            _output.LogInformation($"DeleteTaskAsync called for task ID: {createdTask.Id}.");

            await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(() => _taskService.GetTaskAsync(createdTask.Id, new GetTaskRequest()));
            _output.LogInformation($"Verified task {createdTask.Id} is deleted (GetTaskAsync threw NotFound).");
        }

        [Fact]
        public async Task GetTasksAsync_FilterByStatus_ShouldReturnFilteredTasks()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            Assert.NotNull(_defaultStatus);
            var defaultStatusValueToTest = CurrentTestMode == TestMode.Playback ? PlaybackDefaultStatusValue : _defaultStatus.StatusValue;
            var anotherStatusValueToTest = CurrentTestMode == TestMode.Playback ? PlaybackAnotherStatusValue : _anotherStatus?.StatusValue;

            var task1Id = "playback_task_filterStatus_1";
            var task2Id = "playback_task_filterStatus_2";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // Mock creation of two tasks. Order of mocks matters if requests are identical.
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByStatus_ShouldReturnFilteredTasks/CreateTask1_StatusDefault.json"));
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByStatus_ShouldReturnFilteredTasks/CreateTask2_StatusAnother.json"));

                var encodedStatus = HttpUtility.UrlEncode(defaultStatusValueToTest);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{_testListId}/task?statuses[]={encodedStatus}")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByStatus_ShouldReturnFilteredTasks/GetTasks_FilteredByStatusDefault.json"));
            }

            var taskName1 = $"Task_StatusFilter_1_{(CurrentTestMode == TestMode.Playback ? "PlaybackDef" : Guid.NewGuid().ToString())}";
            var task1 = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: taskName1, Status: defaultStatusValueToTest, Description: null, Assignees: null, GroupAssignees: null, Tags: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null));
            RegisterCreatedTask(task1.Id);
            if (CurrentTestMode == TestMode.Playback) Assert.Equal(task1Id, task1.Id);

            string statusForTask2 = defaultStatusValueToTest;
            if (anotherStatusValueToTest != null && anotherStatusValueToTest != defaultStatusValueToTest) statusForTask2 = anotherStatusValueToTest;

            var taskName2 = $"Task_StatusFilter_2_{(CurrentTestMode == TestMode.Playback ? "PlaybackAn" : Guid.NewGuid().ToString())}";
            var task2 = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: taskName2, Status: statusForTask2, Description: null, Assignees: null, GroupAssignees: null, Tags: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null));
            RegisterCreatedTask(task2.Id);
            if (CurrentTestMode == TestMode.Playback) Assert.Equal(task2Id, task2.Id);

            // var getTasksRequest = new GetTasksRequest { Statuses = new List<string> { defaultStatusValueToTest } };
            // var response = await _taskService.GetTasksAsync(_testListId, getTasksRequest);
            var response = await _taskService.GetTasksAsync(_testListId, parameters =>
            {
                parameters.Statuses = new List<string> { defaultStatusValueToTest };
            });

            Assert.NotNull(response); Assert.NotNull(response.Items);
            Assert.Contains(response.Items, t => t.Id == task1.Id && t.Status.StatusValue == defaultStatusValueToTest);
            if (statusForTask2 != defaultStatusValueToTest) Assert.DoesNotContain(response.Items, t => t.Id == task2.Id);
            else Assert.Contains(response.Items, t => t.Id == task2.Id && t.Status.StatusValue == defaultStatusValueToTest);
        }

        [Fact]
        public async Task GetTasksAsync_FilterByAssignee_ShouldReturnFilteredTasks()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            Assert.False(string.IsNullOrWhiteSpace(_currentUserId), "_currentUserId must be available.");
            var userIdToTest = CurrentTestMode == TestMode.Playback ? PlaybackUserId : _currentUserId;
            var userIdIntToTest = CurrentTestMode == TestMode.Playback ? PlaybackUserIdInt : int.Parse(_currentUserId);

            var taskAssignedId = "playback_task_filterAssignee_1";
            var taskUnassignedId = "playback_task_filterAssignee_2";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByAssignee_ShouldReturnFilteredTasks/CreateTask_Assigned.json"));
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByAssignee_ShouldReturnFilteredTasks/CreateTask_Unassigned.json"));

                var encodedAssignee = HttpUtility.UrlEncode(userIdToTest);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{_testListId}/task?assignees[]={encodedAssignee}")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByAssignee_ShouldReturnFilteredTasks/GetTasks_FilteredByAssignee.json"));
            }

            var taskAssigned = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "AssignedTask", Assignees: new List<int> { userIdIntToTest }, Description: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null));
            RegisterCreatedTask(taskAssigned.Id);
            if (CurrentTestMode == TestMode.Playback) Assert.Equal(taskAssignedId, taskAssigned.Id);

            var taskUnassigned = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "UnassignedTask", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null));
            RegisterCreatedTask(taskUnassigned.Id);
            if (CurrentTestMode == TestMode.Playback) Assert.Equal(taskUnassignedId, taskUnassigned.Id);

            // var getTasksRequest = new GetTasksRequest { Assignees = new List<string> { userIdToTest } };
            // var response = await _taskService.GetTasksAsync(_testListId, getTasksRequest);
            var response = await _taskService.GetTasksAsync(_testListId, parameters =>
            {
                parameters.AssigneeIds = new List<int> { userIdIntToTest }; // Changed to AssigneeIds (long)
            });

            Assert.NotNull(response); Assert.NotNull(response.Items);
            Assert.Contains(response.Items, t => t.Id == taskAssigned.Id);
            Assert.All(response.Items, task => Assert.Contains(task.Assignees ?? new List<User>(), a => a.Id == userIdIntToTest)); // Compare int ID with int ID
             // Verifying taskUnassigned is NOT present is tricky due to auto-assignment behavior. The mock JSON should only return assigned tasks.
            if (CurrentTestMode == TestMode.Playback) Assert.DoesNotContain(response.Items, t => t.Id == taskUnassigned.Id);
        }

        [Fact]
        public async Task GetTasksAsync_FilterByDueDate_ShouldReturnFilteredTasks()
        {
            // This test uses dynamic dates. For playback, we'd need to fix these dates in the mock files
            // and ensure the mock URLs match exactly or use flexible matchers.
            // For simplicity in this conceptual pass, I'll focus on the structure.
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            var taskDueTodayId = "playback_task_dueDate_1";
            var taskDueNextWeekId = "playback_task_dueDate_2";
            long fixedDueDateLessThanTs = 1678900000000; // Example: A fixed timestamp for "dayAfterTomorrow" in playback
            long fixedDueDateGreaterThanTs = 1678800000000; // Example: A fixed timestamp for "tomorrow" in playback

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByDueDate_ShouldReturnFilteredTasks/CreateTask_DueToday.json")); // Contains taskDueTodayId
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByDueDate_ShouldReturnFilteredTasks/CreateTask_DueNextWeek.json")); // Contains taskDueNextWeekId

                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{_testListId}/task?due_date_lt={fixedDueDateLessThanTs}")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByDueDate_ShouldReturnFilteredTasks/GetTasks_DueDateLessThan.json"));
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{_testListId}/task?due_date_gt={fixedDueDateGreaterThanTs}")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByDueDate_ShouldReturnFilteredTasks/GetTasks_DueDateGreaterThan.json"));
            }

            var today = DateTimeOffset.UtcNow; long dayAfterTomorrowTimestampMs = today.AddDays(2).ToUnixTimeMilliseconds(); long tomorrowTimestampMs = today.AddDays(1).ToUnixTimeMilliseconds();
            if (CurrentTestMode == TestMode.Playback) { dayAfterTomorrowTimestampMs = fixedDueDateLessThanTs; tomorrowTimestampMs = fixedDueDateGreaterThanTs; }


            var taskDueToday = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "TaskDueToday", DueDate: today, DueDateTime: true, Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskDueToday.Id);
            var taskDueNextWeek = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "TaskDueNextWeek", DueDate: today.AddDays(7), DueDateTime: true, Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskDueNextWeek.Id);
            if(CurrentTestMode == TestMode.Playback) { Assert.Equal(taskDueTodayId, taskDueToday.Id); Assert.Equal(taskDueNextWeekId, taskDueNextWeek.Id); }

            // var getTasksRequestLT = new GetTasksRequest { DueDateLessThan = dayAfterTomorrowTimestampMs };
            // var responseLT = await _taskService.GetTasksAsync(_testListId, getTasksRequestLT);
            var responseLT = await _taskService.GetTasksAsync(_testListId, parameters =>
            {
                parameters.DueDateRange = new Models.Common.ValueObjects.TimeRange(DateTimeOffset.MinValue, new DateTimeOffset(dayAfterTomorrowTimestampMs * 10000L + DateTimeOffset.UnixEpoch.Ticks, TimeSpan.Zero)); // Assuming DueDateLessThan means before this date
            });
            Assert.NotNull(responseLT?.Items); Assert.Contains(responseLT.Items, t => t.Id == taskDueToday.Id); Assert.DoesNotContain(responseLT.Items, t => t.Id == taskDueNextWeek.Id);

            // var getTasksRequestGT = new GetTasksRequest { DueDateGreaterThan = tomorrowTimestampMs };
            // var responseGT = await _taskService.GetTasksAsync(_testListId, getTasksRequestGT);
            var responseGT = await _taskService.GetTasksAsync(_testListId, parameters =>
            {
                parameters.DueDateRange = new Models.Common.ValueObjects.TimeRange(new DateTimeOffset(tomorrowTimestampMs * 10000L + DateTimeOffset.UnixEpoch.Ticks, TimeSpan.Zero), DateTimeOffset.MaxValue); // Assuming DueDateGreaterThan means after this date
            });
            Assert.NotNull(responseGT?.Items); Assert.DoesNotContain(responseGT.Items, t => t.Id == taskDueToday.Id); Assert.Contains(responseGT.Items, t => t.Id == taskDueNextWeek.Id);
        }

        [Fact]
        public async Task GetTasksAsync_FilterByTags_ShouldReturnFilteredTasks()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            Assert.False(string.IsNullOrWhiteSpace(_testSpaceId), "_testSpaceId must be available to create tags.");
            var tagName = CurrentTestMode == TestMode.Playback ? PlaybackDefaultTagName : $"TestTag_{Guid.NewGuid()}";
            var taskWithTagId = "playback_task_filterTag_1";
            var taskWithoutTagId = "playback_task_filterTag_2";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/space/{_testSpaceId}/tag")
                               .Respond("application/json", await MockedFileContentAsync("TagsService/CreateSpaceTag/Tag_Playback.json"));
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task") // Task with tag
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByTags_ShouldReturnFilteredTasks/CreateTask_WithTag.json"));
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task") // Task without tag
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByTags_ShouldReturnFilteredTasks/CreateTask_WithoutTag.json"));

                var encodedTag = HttpUtility.UrlEncode(tagName);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{_testListId}/task?tags[]={encodedTag}")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByTags_ShouldReturnFilteredTasks/GetTasks_FilteredByTag.json"));
            }

            if (CurrentTestMode != TestMode.Playback) await _tagsService.CreateSpaceTagAsync(_testSpaceId, new ClickUp.Api.Client.Models.RequestModels.Spaces.ModifyTagRequest { Name = tagName, TagBackgroundColor = "#FF0000", TagForegroundColor = "#FFFFFF" });
            _createdTagNamesForCleanup.Add(tagName);

            var taskWithTag = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "TaskWithTag", Tags: new List<string> { tagName }, Description: null, Assignees: null, GroupAssignees: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskWithTag.Id);
            var taskWithoutTag = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "TaskWithoutTag", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskWithoutTag.Id);
            if(CurrentTestMode == TestMode.Playback) { Assert.Equal(taskWithTagId, taskWithTag.Id); Assert.Equal(taskWithoutTagId, taskWithoutTag.Id); }

            // var getTasksRequest = new GetTasksRequest { Tags = new List<string> { tagName } };
            // var response = await _taskService.GetTasksAsync(_testListId, getTasksRequest);
            var response = await _taskService.GetTasksAsync(_testListId, parameters =>
            {
                parameters.Tags = new List<string> { tagName };
            });
            Assert.NotNull(response?.Items);
            Assert.Contains(response.Items, t => t.Id == taskWithTag.Id && t.Tags?.Any(tag => tag.Name == tagName) == true);
            Assert.DoesNotContain(response.Items, t => t.Id == taskWithoutTag.Id);
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_FilterByListId_ShouldReturnTasksFromSpecifiedList()
        {
            // Simplified for brevity, focusing on mock setup
            var taskInList1Id = "playback_teamTask_inList1";
            var taskInOtherListId = "playback_teamTask_inOtherList";
            var otherListId = "playback_other_list_id";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // Mock creation of tasks in _testListId
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetFilteredTeamTasksAsync_FilterByListId_ShouldReturnTasksFromSpecifiedList/CreateTaskInTestList1.json")); // Contains taskInList1Id
                // Mock creation of otherList
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/folder/{_testFolderId}/list")
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetFilteredTeamTasksAsync_FilterByListId_ShouldReturnTasksFromSpecifiedList/CreateOtherList.json")); // Contains otherListId
                // Mock creation of task in otherList
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{otherListId}/task")
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetFilteredTeamTasksAsync_FilterByListId_ShouldReturnTasksFromSpecifiedList/CreateTaskInOtherList.json")); // Contains taskInOtherListId

                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{_testWorkspaceId}/task?list_ids[]={_testListId}")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetFilteredTeamTasksAsync_FilterByListId_ShouldReturnTasksFromSpecifiedList/GetFilteredTeamTasks.json"));
            }

            var taskInList1 = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "TaskInList1", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskInList1.Id);

            ClickUpList otherList = null;
            if (CurrentTestMode != TestMode.Playback) otherList = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest(Name: "OtherList", Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null));
            else otherList = new ClickUpList { Id = otherListId, Name = "OtherListPlayback" }; // Simulated for playback

            var taskInOtherList = await _taskService.CreateTaskAsync(otherList.Id, new CreateTaskRequest(Name: "TaskInOtherList", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskInOtherList.Id);
            if(CurrentTestMode == TestMode.Playback) { Assert.Equal(taskInList1Id, taskInList1.Id); Assert.Equal(taskInOtherListId, taskInOtherList.Id); }


            // var requestModel = new GetFilteredTeamTasksRequest { ListIds = new List<string> { _testListId } };
            // var response = await _taskService.GetFilteredTeamTasksAsync(workspaceId: _testWorkspaceId, requestModel: requestModel); Assert.NotNull(response?.Items);
            var response = await _taskService.GetFilteredTeamTasksAsync(_testWorkspaceId, parameters =>
            {
                parameters.ListIds = new List<string> { _testListId };
            });
            Assert.NotNull(response?.Items);
            Assert.Contains(response.Items, t => t.Id == taskInList1.Id);
            Assert.DoesNotContain(response.Items, t => t.Id == taskInOtherList.Id);
        }

        [Fact]
        public async Task GetTasksAsyncEnumerableAsync_ShouldRetrieveAllTasksInList()
        {
            // For Enumerable, mock the first page GET. The mock JSON should contain all tasks.
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                 // Mock task creations - simplified to one generic mock if names are not critical for this test's playback
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", "{ \"id\": \"playback_paginated_task_generic\" }"); // Generic response for creations

                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{_testListId}/task?page=0")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsyncEnumerableAsync_ShouldRetrieveAllTasksInList/GetTasks_Page0_AllItems.json"));
            }

            int tasksToCreate = CurrentTestMode == TestMode.Playback ? 2 : 3; // Playback JSON has 2 items
            var createdTaskIds = new List<string>();
            for (int i = 0; i < tasksToCreate; i++) { var task = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: $"PagTask{i}", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(task.Id); createdTaskIds.Add(task.Id); if(CurrentTestMode != TestMode.Playback) await Task.Delay(100); }

            var retrievedTasks = new List<CuTask>();
            // await foreach (var task in _taskService.GetTasksAsyncEnumerableAsync(_testListId, new GetTasksRequest())) { retrievedTasks.Add(task); } // Pass empty GetTasksRequest
            await foreach (var task in _taskService.GetTasksAsyncEnumerableAsync(_testListId, new Models.Parameters.GetTasksRequestParameters())) { retrievedTasks.Add(task); }
            Assert.Equal(tasksToCreate, retrievedTasks.Count);
            foreach (var id in createdTaskIds) if(CurrentTestMode != TestMode.Playback) Assert.Contains(retrievedTasks, rt => rt.Id == id); // In playback, IDs from JSON are asserted
             if(CurrentTestMode == TestMode.Playback) { Assert.Contains(retrievedTasks, rt => rt.Id == "playback_pag_task_1"); Assert.Contains(retrievedTasks, rt => rt.Id == "playback_pag_task_2"); }

        }

        [Fact]
        public async Task GetFilteredTeamTasksAsyncEnumerableAsync_ShouldRetrieveAllTasksInSpecifiedList()
        {
             if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task") // Generic for creations
                               .Respond("application/json", "{ \"id\": \"playback_team_paginated_task_generic\" }");
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/folder/{_testFolderId}/list") // otherList creation
                               .Respond("application/json", "{ \"id\": \"playback_other_list_for_team_pag\" }");
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/playback_other_list_for_team_pag/task") // Task in otherList
                               .Respond("application/json", "{ \"id\": \"playback_task_in_other_list_team_pag\" }");

                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{_testWorkspaceId}/task?list_ids[]={_testListId}&page=0")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetFilteredTeamTasksAsyncEnumerableAsync_ShouldRetrieveAllTasksInSpecifiedList/GetTeamTasks_Page0_AllItems.json"));
            }

            int tasksToCreate = CurrentTestMode == TestMode.Playback ? 2 : 3;
            var createdTaskIdsInTestList = new List<string>();
            for (int i = 0; i < tasksToCreate; i++) { var task = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: $"TeamPagTask{i}", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(task.Id); createdTaskIdsInTestList.Add(task.Id); if(CurrentTestMode != TestMode.Playback) await Task.Delay(100); }

            var otherListId = CurrentTestMode == TestMode.Playback ? "playback_other_list_for_team_pag" : (await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest(Name: "OtherListTeamPag", Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null))).Id;
            var taskInOtherList = await _taskService.CreateTaskAsync(otherListId, new CreateTaskRequest(Name: "TaskInOtherListTeamPag", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskInOtherList.Id);


            var retrievedTasks = new List<CuTask>();
            // var requestModel = new GetFilteredTeamTasksRequest { ListIds = new List<string> { _testListId } };
            // await foreach (var task in _taskService.GetFilteredTeamTasksAsyncEnumerableAsync(_testWorkspaceId, requestModel))
            var parameters = new Models.Parameters.GetTasksRequestParameters { ListIds = new List<string> { _testListId } };
            await foreach (var task in _taskService.GetFilteredTeamTasksAsyncEnumerableAsync(_testWorkspaceId, parameters))
            {
                retrievedTasks.Add(task);
            }
            Assert.Equal(tasksToCreate, retrievedTasks.Count);
            if(CurrentTestMode == TestMode.Playback) { Assert.Contains(retrievedTasks, rt => rt.Id == "playback_team_pag_task_1"); Assert.Contains(retrievedTasks, rt => rt.Id == "playback_team_pag_task_2"); Assert.DoesNotContain(retrievedTasks, rt => rt.Id == "playback_task_in_other_list_team_pag");}
        }

        [Fact]
        public async Task GetTaskAsync_WithNonExistentTaskId_ShouldThrowNotFoundException()
        {
            var nonExistentTaskId = "0_playback_nonexistent";
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/task/{nonExistentTaskId}")
                               .Respond(HttpStatusCode.NotFound, "application/json", await MockedFileContentAsync("TaskService/GetTaskAsync_WithNonExistentTaskId_ShouldThrowNotFoundException/GetNonExistentTask_NotFound.json"));
            }
            await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(() => _taskService.GetTaskAsync(nonExistentTaskId, new GetTaskRequest()));
        }

        [Fact]
        public async Task UpdateTaskAsync_WithNonExistentTaskId_ShouldThrowNotFoundException()
        {
            var nonExistentTaskId = "0_playback_nonexistent_update";
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Put, $"https://api.clickup.com/api/v2/task/{nonExistentTaskId}")
                               .Respond(HttpStatusCode.NotFound, "application/json", "{ \"err\": \"Task not found\", \"ECODE\": \"TASK_001\" }"); // Simplified JSON
            }
            var updateRequest = new UpdateTaskRequest(Name: "Attempt update", Description: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, Parent: null, TimeEstimate: null, StartDate: null, StartDateTime: null, Assignees: null, GroupAssignees: null, Archived: null, CustomFields: null);
            await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(() => _taskService.UpdateTaskAsync(nonExistentTaskId, updateRequest));
        }

        [Fact]
        public async Task DeleteTaskAsync_WithNonExistentTaskId_ShouldThrowNotFoundException()
        {
            var nonExistentTaskId = "0_playback_nonexistent_delete";
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{nonExistentTaskId}")
                               .Respond(HttpStatusCode.NotFound, "application/json", "{ \"err\": \"Task not found\", \"ECODE\": \"TASK_001\" }");
            }
            await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(() => _taskService.DeleteTaskAsync(nonExistentTaskId, new DeleteTaskRequest()));
        }

        // GetTasksAsync_FilterByDateCreated_ShouldReturnFilteredTasks and GetTasksAsync_OrderByDueDateAndReversed_ShouldReturnOrderedTasks
        // are more complex due to dynamic date/time and ordering.
        // For Playback, these would require fixed date strings in mock JSONs and careful URL matching.
        // I'll skip detailed playback mocking for these two for brevity, but the principle is the same:
        // 1. Mock task creations with fixed dates/properties.
        // 2. Mock the GetTasksAsync call with the exact URL parameters used in the test (with fixed dates for playback).
        // 3. Ensure the mock JSON response contains tasks that satisfy the conditions and ordering.
        [Fact]
        public async Task GetTasksAsync_FilterByDateCreated_ShouldReturnFilteredTasks()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            long fixedDateCreatedGreaterThan = 1678880000000; // Example fixed timestamp
            long fixedDateCreatedLessThan = 1678890000000; // Example fixed timestamp

            var taskNowId = "playback_dateCreated_now";
            var taskLaterId = "playback_dateCreated_later";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByDateCreated_ShouldReturnFilteredTasks/CreateTask_Now.json")); // taskNowId
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByDateCreated_ShouldReturnFilteredTasks/CreateTask_Later.json")); // taskLaterId

                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{_testListId}/task?date_created_gt={fixedDateCreatedGreaterThan}")
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByDateCreated_ShouldReturnFilteredTasks/GetTasks_DateCreatedGreaterThan.json"));
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{_testListId}/task?date_created_lt={fixedDateCreatedLessThan}")
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_FilterByDateCreated_ShouldReturnFilteredTasks/GetTasks_DateCreatedLessThan.json"));
            }

            var taskNow = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "TaskNow", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskNow.Id);
            if(CurrentTestMode != TestMode.Playback) await Task.Delay(2000); // Ensure time difference
            var pointInTimeAfterFirstTask = CurrentTestMode == TestMode.Playback ? fixedDateCreatedGreaterThan : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var taskLater = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "TaskLater", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskLater.Id);
            if(CurrentTestMode == TestMode.Playback) { Assert.Equal(taskNowId, taskNow.Id); Assert.Equal(taskLaterId, taskLater.Id); }


            // var getTasksRequestGT = new GetTasksRequest { DateCreatedGreaterThan = pointInTimeAfterFirstTask };
            // var responseGT = await _taskService.GetTasksAsync(_testListId, getTasksRequestGT);
            var responseGT = await _taskService.GetTasksAsync(_testListId, parameters =>
            {
                parameters.DateCreatedRange = new Models.Common.ValueObjects.TimeRange(
                    new DateTimeOffset(pointInTimeAfterFirstTask * 10000L + DateTimeOffset.UnixEpoch.Ticks, TimeSpan.Zero),
                    DateTimeOffset.MaxValue);
            });
            Assert.NotNull(responseGT?.Items); Assert.DoesNotContain(responseGT.Items, t => t.Id == taskNow.Id); Assert.Contains(responseGT.Items, t => t.Id == taskLater.Id);

            var pointInTimeBeforeSecondTask = CurrentTestMode == TestMode.Playback ? fixedDateCreatedLessThan : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // Point before taskLater effectively
            if (CurrentTestMode != TestMode.Playback)
            {
                // Assuming CuTask.DateCreated is actually DateTimeOffset? based on compiler errors
                DateTimeOffset? dateCreatedActual = taskLater.DateCreated as DateTimeOffset?;
                if (dateCreatedActual.HasValue)
                {
                    pointInTimeBeforeSecondTask = dateCreatedActual.Value.ToUnixTimeMilliseconds() - 1000;
                }
                // If it were a string, the original long.TryParse would be:
                // else if (taskLater.DateCreated != null && long.TryParse(taskLater.DateCreated, out long dateCreatedMs))
                // {
                //    pointInTimeBeforeSecondTask = dateCreatedMs - 1000;
                // }
                else // If dateCreatedActual is null and not in playback, use a fallback or log
                {
                    _output.LogWarning($"[Record/Passthrough] taskLater.DateCreated was not a DateTimeOffset or was null. Using current time for pointInTimeBeforeSecondTask calculation basis.");
                    // Fallback, though this might make the assertion less meaningful if taskLater was just created
                    pointInTimeBeforeSecondTask = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
            }

            // var getTasksRequestLT = new GetTasksRequest { DateCreatedLessThan = pointInTimeBeforeSecondTask };
            // var responseLT = await _taskService.GetTasksAsync(_testListId, getTasksRequestLT);
            var responseLT = await _taskService.GetTasksAsync(_testListId, parameters =>
            {
                parameters.DateCreatedRange = new Models.Common.ValueObjects.TimeRange(
                    DateTimeOffset.MinValue,
                    new DateTimeOffset(pointInTimeBeforeSecondTask * 10000L + DateTimeOffset.UnixEpoch.Ticks, TimeSpan.Zero));
            });
            Assert.NotNull(responseLT?.Items); Assert.Contains(responseLT.Items, t => t.Id == taskNow.Id); Assert.DoesNotContain(responseLT.Items, t => t.Id == taskLater.Id);
        }

        [Fact]
        public async Task GetTasksAsync_OrderByDueDateAndReversed_ShouldReturnOrderedTasks()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            var taskDueTodayId = "playback_orderBy_dueToday";
            var taskDueTomorrowId = "playback_orderBy_dueTomorrow";
            var taskDueDayAfterId = "playback_orderBy_dueDayAfter";

            if (CurrentTestMode == TestMode.Playback)
            {
                 Assert.NotNull(MockHttpHandler);
                // Mock task creations. Order of .Respond matters if POST requests are identical.
                // Using Expect for ordered responses to the same POST URL.
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_OrderByDueDateAndReversed_ShouldReturnOrderedTasks/CreateTask_DueTomorrow.json")); // taskDueTomorrowId
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_OrderByDueDateAndReversed_ShouldReturnOrderedTasks/CreateTask_DueDayAfter.json")); // taskDueDayAfterId
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_OrderByDueDateAndReversed_ShouldReturnOrderedTasks/CreateTask_DueToday.json")); // taskDueTodayId

                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{_testListId}/task?order_by=due_date&reverse=true")
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_OrderByDueDateAndReversed_ShouldReturnOrderedTasks/GetTasks_OrderByDueDate_Reversed.json"));
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/list/{_testListId}/task?order_by=due_date&reverse=false") // Assuming reverse=false is the query param for natural
                    .Respond("application/json", await MockedFileContentAsync("TaskService/GetTasksAsync_OrderByDueDateAndReversed_ShouldReturnOrderedTasks/GetTasks_OrderByDueDate_Natural.json"));
            }

            var today = DateTimeOffset.UtcNow; var tomorrow = today.AddDays(1); var dayAfterTomorrow = today.AddDays(2);
            var taskDueTomorrow = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "OrderDueTomorrow", DueDate: tomorrow, DueDateTime: true, Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskDueTomorrow.Id);
            var taskDueDayAfter = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "OrderDueDayAfter", DueDate: dayAfterTomorrow, DueDateTime: true, Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskDueDayAfter.Id);
            var taskDueToday = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(Name: "OrderDueToday", DueDate: today, DueDateTime: true, Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null)); RegisterCreatedTask(taskDueToday.Id);
            if(CurrentTestMode == TestMode.Playback) { Assert.Equal(taskDueTomorrowId, taskDueTomorrow.Id); Assert.Equal(taskDueDayAfterId, taskDueDayAfter.Id); Assert.Equal(taskDueTodayId, taskDueToday.Id); }


            // var getTasksRequestReversed = new GetTasksRequest { OrderBy = "due_date", Reverse = true };
            // var responseReversed = await _taskService.GetTasksAsync(_testListId, getTasksRequestReversed);
            var responseReversed = await _taskService.GetTasksAsync(_testListId, parameters =>
            {
                parameters.SortBy = new Models.Common.ValueObjects.SortOption("due_date", Models.Common.ValueObjects.SortDirection.Descending);
            });
            Assert.NotNull(responseReversed?.Items); Assert.True(responseReversed.Items.Count >= 3);
            var relevantTasksReversed = responseReversed.Items.Where(t => t.Id == taskDueToday.Id || t.Id == taskDueTomorrow.Id || t.Id == taskDueDayAfter.Id).ToList();
            Assert.Equal(3, relevantTasksReversed.Count);
            Assert.Equal(taskDueDayAfter.Id, relevantTasksReversed[0].Id); Assert.Equal(taskDueTomorrow.Id, relevantTasksReversed[1].Id); Assert.Equal(taskDueToday.Id, relevantTasksReversed[2].Id);

            // var getTasksRequestNatural = new GetTasksRequest { OrderBy = "due_date", Reverse = false };
            // var responseNatural = await _taskService.GetTasksAsync(_testListId, getTasksRequestNatural);
            var responseNatural = await _taskService.GetTasksAsync(_testListId, parameters =>
            {
                parameters.SortBy = new Models.Common.ValueObjects.SortOption("due_date", Models.Common.ValueObjects.SortDirection.Ascending);
            });
            Assert.NotNull(responseNatural?.Items); Assert.True(responseNatural.Items.Count >= 3);
            var relevantTasksNatural = responseNatural.Items.Where(t => t.Id == taskDueToday.Id || t.Id == taskDueTomorrow.Id || t.Id == taskDueDayAfter.Id).ToList();
            Assert.Equal(3, relevantTasksNatural.Count);
            Assert.Equal(taskDueToday.Id, relevantTasksNatural[0].Id); Assert.Equal(taskDueTomorrow.Id, relevantTasksNatural[1].Id); Assert.Equal(taskDueDayAfter.Id, relevantTasksNatural[2].Id);
        }

        private const string PlaybackTaskTemplateId = "playback_task_template_001"; // Added for template tests

        [Fact]
        public async Task CreateTaskFromTemplateAsync_WithValidTemplate_ShouldCreateTask()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            string templateId = PlaybackTaskTemplateId; // In Record mode, this needs to be a real template ID
            var taskName = $"Task from Template - {(CurrentTestMode == TestMode.Playback ? "Playback" : Guid.NewGuid().ToString())}";
            var request = new CreateTaskFromTemplateRequest(taskName);
            string playbackTaskId = "playback_task_from_tpl_001";
            // string playbackBodyHash = "body_create_task_from_tpl_success"; // If matching specific body content

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // taskName = "Playback Task from Template"; // Name is in request, already set
                // request = new CreateTaskFromTemplateRequest(taskName); // Already set

                var mockResponsePath = "TaskService/CreateTaskFromTemplateAsync_WithValidTemplate_ShouldCreateTask/CreateTaskFromTemplate_Success.json";
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/taskTemplate/{templateId}")
                               // .WithPartialContent(JsonSerializer.Serialize(request, _jsonSerializerOptions)) // Match body if needed
                               .Respond("application/json", await MockedFileContentAsync(mockResponsePath));

                // If this task needs to be cleaned up in Playback by DeleteTaskAsync mock in DisposeAsync, register its playback ID
                // However, tasks are usually cleaned up by deleting the list/space in hierarchy teardown.
            }
            else
            {
                _output.LogWarning($"[Record/Passthrough] Ensure task template ID '{templateId}' is valid for CreateTaskFromTemplateAsync test in list '{_testListId}'.");
            }

            CuTask createdTask = await _taskService.CreateTaskFromTemplateAsync(_testListId, templateId, request);
            if (createdTask != null && CurrentTestMode != TestMode.Playback) RegisterCreatedTask(createdTask.Id);

            Assert.NotNull(createdTask);
            Assert.False(string.IsNullOrWhiteSpace(createdTask.Id));
            Assert.Equal(taskName, createdTask.Name); // The API should use the name from the request
            Assert.Equal(_testListId, createdTask.List?.Id);

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal(playbackTaskId, createdTask.Id);
                // Potentially assert other properties if the mocked JSON has them, e.g., if template populates description.
            }
        }

        [Fact]
        public async Task GetTaskTimeInStatusAsync_WithExistingTask_ShouldReturnTimeInStatus()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available for GetTaskTimeInStatusAsync.");
            var taskName = $"TaskForTimeInStatus - {(CurrentTestMode == TestMode.Playback ? "Playback" : Guid.NewGuid().ToString())}";
            var playbackCreatedTaskId = "playback_task_timeInStatus_001";
            CuTask testTask;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // Mock the task creation
                var createTaskMockPath = "TaskService/GetTaskTimeInStatusAsync_WithExistingTask_ShouldReturnTimeInStatus/CreateTask_Success.json";
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync(createTaskMockPath));

                testTask = new CuTask(Id: playbackCreatedTaskId, CustomId: null, CustomItemId: null, Name: taskName, TextContent: null, Description: null, MarkdownDescription: null, Status: null, OrderIndex: null, DateCreated: null, DateUpdated: null, DateClosed: null, Archived: null, Creator: null, Assignees: null, GroupAssignees: null, Watchers: null, Checklists: null, Tags: null, Parent: null, Priority: null, DueDate: null, StartDate: null, Points: null, TimeEstimate: null, TimeSpent: null, CustomFields: null, Dependencies: null, LinkedTasks: null, TeamId: null, Url: null, Sharing: null, PermissionLevel: null, List: new TaskListReference(Id: _testListId, Name: null, Access: null), Folder: null, Space: null, Project: null); // Simulate created task for Playback

                // Mock the GetTaskTimeInStatusAsync call
                var timeInStatusMockPath = "TaskService/GetTaskTimeInStatusAsync_WithExistingTask_ShouldReturnTimeInStatus/TimeInStatus_Success.json";
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/task/{playbackCreatedTaskId}/time_in_status")
                               .Respond("application/json", await MockedFileContentAsync(timeInStatusMockPath));
            }
            else
            {
                testTask = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(taskName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null));
                RegisterCreatedTask(testTask.Id);
                _output.LogInformation($"[Record/Passthrough] Created task '{testTask.Name}' (ID: {testTask.Id}) for GetTaskTimeInStatusAsync test.");
                // Allow some time or perform status changes for more meaningful data in Record mode.
                await Task.Delay(1000);
            }

            Assert.NotNull(testTask);
            TaskTimeInStatusResponse timeInStatusResponse = await _taskService.GetTaskTimeInStatusAsync(testTask.Id, new GetTaskTimeInStatusRequest());

            Assert.NotNull(timeInStatusResponse);
            Assert.NotNull(timeInStatusResponse.CurrentStatus);
            Assert.True(timeInStatusResponse.StatusHistory.Any());
            // Further assertions can be made based on the expected content of the mock JSON or actual API response
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal("playback_status_open", timeInStatusResponse.CurrentStatus.Status); // Example, adjust to your mock data
                Assert.Contains(timeInStatusResponse.StatusHistory, h => h.Status == "playback_status_open");
            }
            else
            {
                // In record mode, the status would likely be the default "open" status or whatever it was set to.
                Assert.NotNull(timeInStatusResponse.CurrentStatus.Status);
            }
        }

        [Fact]
        public async Task GetBulkTasksTimeInStatusAsync_WithExistingTasks_ShouldReturnTimeInStatus()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available for GetBulkTasksTimeInStatusAsync.");
            var taskName1 = $"BulkTaskTime_1 - {(CurrentTestMode == TestMode.Playback ? "Playback" : Guid.NewGuid().ToString())}";
            var taskName2 = $"BulkTaskTime_2 - {(CurrentTestMode == TestMode.Playback ? "Playback" : Guid.NewGuid().ToString())}";
            var playbackTask1Id = "playback_bulkTask1_timeInStatus_001";
            var playbackTask2Id = "playback_bulkTask2_timeInStatus_002";
            List<string> taskIdsForTest = new List<string>();
            CuTask testTask1, testTask2;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // Mock task creations
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetBulkTasksTimeInStatusAsync_WithExistingTasks_ShouldReturnTimeInStatus/CreateTask1_Success.json"));
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .Respond("application/json", await MockedFileContentAsync("TaskService/GetBulkTasksTimeInStatusAsync_WithExistingTasks_ShouldReturnTimeInStatus/CreateTask2_Success.json"));

                testTask1 = new CuTask(Id: playbackTask1Id, CustomId: null, CustomItemId: null, Name: taskName1, TextContent: null, Description: null, MarkdownDescription: null, Status: null, OrderIndex: null, DateCreated: null, DateUpdated: null, DateClosed: null, Archived: null, Creator: null, Assignees: null, GroupAssignees: null, Watchers: null, Checklists: null, Tags: null, Parent: null, Priority: null, DueDate: null, StartDate: null, Points: null, TimeEstimate: null, TimeSpent: null, CustomFields: null, Dependencies: null, LinkedTasks: null, TeamId: null, Url: null, Sharing: null, PermissionLevel: null, List: new TaskListReference(Id: _testListId, Name: null, Access: null), Folder: null, Space: null, Project: null); // Simplified for playback setup
                testTask2 = new CuTask(Id: playbackTask2Id, CustomId: null, CustomItemId: null, Name: taskName2, TextContent: null, Description: null, MarkdownDescription: null, Status: null, OrderIndex: null, DateCreated: null, DateUpdated: null, DateClosed: null, Archived: null, Creator: null, Assignees: null, GroupAssignees: null, Watchers: null, Checklists: null, Tags: null, Parent: null, Priority: null, DueDate: null, StartDate: null, Points: null, TimeEstimate: null, TimeSpent: null, CustomFields: null, Dependencies: null, LinkedTasks: null, TeamId: null, Url: null, Sharing: null, PermissionLevel: null, List: new TaskListReference(Id: _testListId, Name: null, Access: null), Folder: null, Space: null, Project: null);
                taskIdsForTest.Add(playbackTask1Id);
                taskIdsForTest.Add(playbackTask2Id);

                // Mock the GetBulkTasksTimeInStatusAsync call
                var bulkTimeInStatusMockPath = "TaskService/GetBulkTasksTimeInStatusAsync_WithExistingTasks_ShouldReturnTimeInStatus/BulkTimeInStatus_Success.json";
                // The API takes task_ids as a comma-separated query parameter
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/task/bulk_time_in_status/task_ids?task_ids={playbackTask1Id}%2C{playbackTask2Id}") // Ensure URL encoding for comma
                               .Respond("application/json", await MockedFileContentAsync(bulkTimeInStatusMockPath));
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/task/bulk_time_in_status/task_ids?task_ids={playbackTask2Id}%2C{playbackTask1Id}") // Order might vary
                               .Respond("application/json", await MockedFileContentAsync(bulkTimeInStatusMockPath));
            }
            else
            {
                testTask1 = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(taskName1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null));
                RegisterCreatedTask(testTask1.Id);
                taskIdsForTest.Add(testTask1.Id);
                testTask2 = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(taskName2, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null));
                RegisterCreatedTask(testTask2.Id);
                taskIdsForTest.Add(testTask2.Id);
                _output.LogInformation($"[Record/Passthrough] Created tasks '{testTask1.Id}' and '{testTask2.Id}' for GetBulkTasksTimeInStatusAsync test.");
                await Task.Delay(1000); // Allow time for tasks to settle
            }

            GetBulkTasksTimeInStatusResponse bulkTimeResponse = await _taskService.GetBulkTasksTimeInStatusAsync(new GetBulkTasksTimeInStatusRequest(taskIdsForTest));

            Assert.NotNull(bulkTimeResponse);
            // Assert.NotNull(bulkTimeResponse.TasksTimeInStatus); // bulkTimeResponse *is* the dictionary
            Assert.Equal(taskIdsForTest.Count, bulkTimeResponse.Count);

            foreach (var taskId in taskIdsForTest)
            {
                Assert.True(bulkTimeResponse.ContainsKey(taskId));
                var timeInStatusData = bulkTimeResponse[taskId];
                Assert.NotNull(timeInStatusData.CurrentStatus);
                Assert.True(timeInStatusData.StatusHistory.Any());
                if (CurrentTestMode == TestMode.Playback)
                {
                     Assert.Equal("playback_status_open", timeInStatusData.CurrentStatus.Status); // Example
                }
            }
        }

        [Fact]
        public async Task MergeTasksAsync_WithValidTasks_ShouldMergeAndReturnTargetTask()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available for MergeTasksAsync.");
            var targetTaskName = $"MergeTargetTask - {(CurrentTestMode == TestMode.Playback ? "Playback" : Guid.NewGuid().ToString())}";
            var sourceTaskName = $"MergeSourceTask - {(CurrentTestMode == TestMode.Playback ? "Playback" : Guid.NewGuid().ToString())}";

            string playbackTargetTaskId = "playback_merge_target_task_001";
            string playbackSourceTaskId = "playback_merge_source_task_001";
            CuTask targetTask, sourceTask;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // Mock creations
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task") // Target Task
                               .Respond("application/json", await MockedFileContentAsync("TaskService/MergeTasksAsync_WithValidTasks_ShouldMergeAndReturnTargetTask/CreateTargetTask_Success.json"));
                MockHttpHandler.Expect(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task") // Source Task
                               .Respond("application/json", await MockedFileContentAsync("TaskService/MergeTasksAsync_WithValidTasks_ShouldMergeAndReturnTargetTask/CreateSourceTask_Success.json"));

                targetTask = new CuTask(Id: playbackTargetTaskId, CustomId: null, CustomItemId: null, Name: targetTaskName, TextContent: null, Description: null, MarkdownDescription: null, Status: null, OrderIndex: null, DateCreated: null, DateUpdated: null, DateClosed: null, Archived: null, Creator: null, Assignees: null, GroupAssignees: null, Watchers: null, Checklists: null, Tags: null, Parent: null, Priority: null, DueDate: null, StartDate: null, Points: null, TimeEstimate: null, TimeSpent: null, CustomFields: null, Dependencies: null, LinkedTasks: null, TeamId: null, Url: null, Sharing: null, PermissionLevel: null, List: new TaskListReference(Id: _testListId, Name: null, Access: null), Folder: null, Space: null, Project: null);
                sourceTask = new CuTask(Id: playbackSourceTaskId, CustomId: null, CustomItemId: null, Name: sourceTaskName, TextContent: null, Description: null, MarkdownDescription: null, Status: null, OrderIndex: null, DateCreated: null, DateUpdated: null, DateClosed: null, Archived: null, Creator: null, Assignees: null, GroupAssignees: null, Watchers: null, Checklists: null, Tags: null, Parent: null, Priority: null, DueDate: null, StartDate: null, Points: null, TimeEstimate: null, TimeSpent: null, CustomFields: null, Dependencies: null, LinkedTasks: null, TeamId: null, Url: null, Sharing: null, PermissionLevel: null, List: new TaskListReference(Id: _testListId, Name: null, Access: null), Folder: null, Space: null, Project: null);

                // Mock the MergeTasksAsync call (POST to /task/{source_task_id}/merge)
                var mergeResponseMockPath = "TaskService/MergeTasksAsync_WithValidTasks_ShouldMergeAndReturnTargetTask/Merge_Success.json";
                // This endpoint is called for each source task. If multiple sources, mock each.
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/task/{playbackSourceTaskId}/merge")
                               .WithJsonContentMatcher(new { target_task_id = playbackTargetTaskId })
                               .Respond("application/json", await MockedFileContentAsync(mergeResponseMockPath));

                // Mock for GetTaskAsync on sourceTask (to verify it's deleted/merged)
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/task/{playbackSourceTaskId}")
                               .Respond(HttpStatusCode.NotFound, "application/json", "{ \"err\": \"Task not found\" }"); // Source task should be gone
            }
            else
            {
                targetTask = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(targetTaskName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null));
                RegisterCreatedTask(targetTask.Id); // Target task remains, so register for cleanup
                sourceTask = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(sourceTaskName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null));
                // Source task will be deleted by merge, so typically not registered for separate cleanup.
                // However, if Merge fails, it might linger. For safety in tests, one might register then unregister if merge is successful.
                _output.LogInformation($"[Record/Passthrough] Created target task '{targetTask.Id}' and source task '{sourceTask.Id}' for MergeTasksAsync test.");
            }

            var mergeRequest = new MergeTasksRequest { SourceTaskIds = new List<string> { sourceTask.Id } };
            CuTask updatedTargetTask = await _taskService.MergeTasksAsync(targetTask.Id, mergeRequest);

            Assert.NotNull(updatedTargetTask);
            Assert.Equal(targetTask.Id, updatedTargetTask.Id); // ID of target task should remain the same
            // Name of target task might also remain the same, or API might append info. Check API behavior.
            // For now, assume name remains or is updated as per mock.
            if(CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal("Merged Playback Task Name", updatedTargetTask.Name); // Example from a potential mock JSON
            }

            // Verify source task is gone (in live mode)
            if (CurrentTestMode != TestMode.Playback)
            {
                await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(() => _taskService.GetTaskAsync(sourceTask.Id, new GetTaskRequest()));
                _output.LogInformation($"[Record/Passthrough] Verified source task '{sourceTask.Id}' is deleted after merge.");
            }
            else
            {
                // Playback verification relies on the mock for GetTaskAsync(sourceTask.Id) returning NotFound.
                 await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(() => _taskService.GetTaskAsync(playbackSourceTaskId, new GetTaskRequest()));
            }
        }
    }
}
