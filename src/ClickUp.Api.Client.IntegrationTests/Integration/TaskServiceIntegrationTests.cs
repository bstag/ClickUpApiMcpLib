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
// Removed incorrect using ClickUp.Api.Client.Abstractions.Services.Folders; // For IFoldersService
// Removed incorrect using ClickUp.Api.Client.Abstractions.Services.Spaces; // For ISpacesService
using ClickUp.Api.Client.Models.RequestModels.Folders; // For CreateFolderRequest
using ClickUp.Api.Client.Models.RequestModels.Lists; // For CreateListRequest

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
        private string _testSpaceId; // Created in InitializeAsync of ListServiceTests, here we use a configured one or create one too.
        private string _testFolderId;
        private string _testListId; // This list will be created for each test run (or class run)

        private List<string> _createdTaskIds = new List<string>();

        public TaskServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _taskService = ServiceProvider.GetRequiredService<ITasksService>();
            _listService = ServiceProvider.GetRequiredService<IListsService>();
            _spaceService = ServiceProvider.GetRequiredService<ISpacesService>();
            _folderService = ServiceProvider.GetRequiredService<IFoldersService>();
            _authService = ServiceProvider.GetRequiredService<IAuthorizationService>();
            _tagsService = ServiceProvider.GetRequiredService<ITagsService>();


            // Load necessary IDs from configuration
            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"];
            // _testSpaceId will be created or fetched if a specific one is needed for setup.
            // For now, tests will create their own space, folder, list structure.

            if (string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured. Test setup will fail.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for TaskServiceIntegrationTests.");
            }
        }

        private string _currentUserId;
        private ClickUp.Api.Client.Models.Common.Status _defaultStatus;
        private ClickUp.Api.Client.Models.Common.Status _anotherStatus;
        private List<string> _createdTagNamesForCleanup = new List<string>();


        public async Task InitializeAsync()
        {
            _output.LogInformation("Starting test class initialization: Creating shared test resources (Space, Folder, List).");

            try
            {
                var user = await _authService.GetAuthorizedUserAsync();
                _currentUserId = user.Id.ToString();
                _output.LogInformation($"Current user ID: {_currentUserId}");

                // 1. Create a Test Space
                var spaceName = $"TestSpace_TasksQuery_{Guid.NewGuid()}";
                _output.LogInformation($"Creating test space: {spaceName} in Workspace ID: {_testWorkspaceId}");
                var createSpaceReq = new ClickUp.Api.Client.Models.RequestModels.Spaces.CreateSpaceRequest(spaceName, null, null);
                var space = await _spaceService.CreateSpaceAsync(_testWorkspaceId, createSpaceReq);
                _testSpaceId = space.Id;
                _output.LogInformation($"Test space created successfully. Space ID: {_testSpaceId}");

                // 2. Create a Test Folder
                var folderName = $"TestFolder_TasksQuery_{Guid.NewGuid()}";
                _output.LogInformation($"Creating test folder: {folderName} in Space ID: {_testSpaceId}");
                var createFolderReq = new CreateFolderRequest(folderName);
                var folder = await _folderService.CreateFolderAsync(_testSpaceId, createFolderReq);
                _testFolderId = folder.Id;
                _output.LogInformation($"Test folder created successfully. Folder ID: {_testFolderId}");

                // 3. Create a Test List in that Folder
                var listName = $"TestList_TasksQuery_{Guid.NewGuid()}";
                _output.LogInformation($"Creating test list: {listName} in Folder ID: {_testFolderId}");
                var createListReq = new CreateListRequest(
                    Name: listName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null
                );
                var list = await _listService.CreateListInFolderAsync(_testFolderId, createListReq);
                _testListId = list.Id;
                _output.LogInformation($"Test list created successfully. List ID: {_testListId}");

                // Get List details to find default statuses
                var listDetails = await _listService.GetListAsync(_testListId);
                if (listDetails.Statuses != null && listDetails.Statuses.Any())
                {
                    _defaultStatus = listDetails.Statuses.First(); // Often "Open" or "To Do"
                    _anotherStatus = listDetails.Statuses.Skip(1).FirstOrDefault(); // A second status if available
                    _output.LogInformation($"Default status: '{_defaultStatus.StatusValue}' (Order: {_defaultStatus.OrderIndex}). Another status: '{_anotherStatus?.StatusValue}'");

                    if (_anotherStatus == null && listDetails.Statuses.Count() > 0) {
                        // If only one status, try to create a new one or use a common one like "Closed" if that's an option
                        // For simplicity, we'll rely on the list having at least two for now, or tests for status filtering might be limited.
                        _output.LogWarning("Only one status found in the test list. Status filter tests might be limited.");
                    }
                }
                else
                {
                    _output.LogWarning("Could not retrieve statuses for the test list. Status filter tests will be impacted.");
                    // Potentially throw here if statuses are critical for all query tests
                }
            }
            catch (Exception ex)
            {
                _output.LogError($"Error during InitializeAsync: {ex.Message}", ex);
                await CleanupLingeringResourcesAsync(); // Attempt cleanup
                throw; // Re-throw to fail the test class setup
            }
        }
        private async Task CleanupLingeringResourcesAsync()
        {
            if (!string.IsNullOrWhiteSpace(_testFolderId))
            {
                try { await _folderService.DeleteFolderAsync(_testFolderId); _output.LogInformation($"Deleted folder {_testFolderId}"); _testFolderId = null; }
                catch (Exception ex) { _output.LogError($"Error deleting folder {_testFolderId}: {ex.Message}", ex); }
            }
            if (!string.IsNullOrWhiteSpace(_testSpaceId))
            {
                try { await _spaceService.DeleteSpaceAsync(_testSpaceId); _output.LogInformation($"Deleted space {_testSpaceId}"); _testSpaceId = null; }
                catch (Exception ex) { _output.LogError($"Error deleting space {_testSpaceId}: {ex.Message}", ex); }
            }
        }
        public async Task DisposeAsync()
        {
            _output.LogInformation("Starting test class disposal: Cleaning up shared test resources and created tasks.");
            // Delete tasks first
            foreach (var taskId in _createdTaskIds)
            {
                try
                {
                    _output.LogInformation($"Deleting task: {taskId}");
                    await _taskService.DeleteTaskAsync(taskId);
                }
                catch (Exception ex)
                {
                    _output.LogError($"Error deleting task {taskId}: {ex.Message}", ex);
                }
            }
            _createdTaskIds.Clear();

            // Cleanup tags
            foreach (var tagName in _createdTagNamesForCleanup)
            {
                if (!string.IsNullOrWhiteSpace(_testSpaceId) && !string.IsNullOrWhiteSpace(tagName))
                {
                    try
                    {
                        _output.LogInformation($"Deleting tag '{tagName}' from space '{_testSpaceId}'.");
                        await _tagsService.DeleteSpaceTagAsync(_testSpaceId, tagName);
                    }
                    catch (Exception ex)
                    {
                        _output.LogError($"Error deleting tag '{tagName}': {ex.Message}", ex);
                    }
                }
            }
            _createdTagNamesForCleanup.Clear();

            // Then delete the list (which should be deleted when folder is deleted)
            // Then delete the folder (which should be deleted when space is deleted by CleanupLingeringResourcesAsync)
            // Explicit list/folder deletion is handled by CleanupLingeringResourcesAsync to ensure hierarchy.
            await CleanupLingeringResourcesAsync(); // This will delete folder and then space

            _output.LogInformation("Test class disposal complete.");
        }

        private void RegisterCreatedTask(string taskId)
        {
            if (!string.IsNullOrWhiteSpace(taskId))
            {
                _createdTaskIds.Add(taskId);
            }
        }

        [Fact]
        public async Task CreateTaskAsync_WithValidData_ShouldCreateTask()
        {
            // Arrange
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available for this test. Check InitializeAsync and configuration.");
            var taskName = $"My Integration Test Task - {Guid.NewGuid()}";
            var createTaskRequest = new CreateTaskRequest(
                Name: taskName,
                Description: "This is a task created by an integration test.",
                Assignees: null,
                GroupAssignees: null,
                Tags: null,
                Status: null,
                Priority: null,
                DueDate: null,
                DueDateTime: null,
                TimeEstimate: null,
                StartDate: null,
                StartDateTime: null,
                NotifyAll: null,
                Parent: null,
                LinksTo: null,
                CheckRequiredCustomFields: null,
                CustomFields: null,
                CustomItemId: null,
                ListId: null // ListId is typically part of the URL, but the model supports it
            );

            _output.LogInformation($"Attempting to create task '{taskName}' in list '{_testListId}'.");

            // Act
            CuTask createdTask = null;
            try
            {
                createdTask = await _taskService.CreateTaskAsync(_testListId, createTaskRequest);
                if (createdTask != null)
                {
                    RegisterCreatedTask(createdTask.Id);
                    _output.LogInformation($"Task created successfully. ID: {createdTask.Id}, Name: {createdTask.Name}");
                }
            }
            catch (Exception ex)
            {
                _output.LogError($"Exception during CreateTaskAsync: {ex.Message}", ex);
                Assert.Fail($"CreateTaskAsync threw an exception: {ex.Message}");
            }


            // Assert
            Assert.NotNull(createdTask);
            Assert.False(string.IsNullOrWhiteSpace(createdTask.Id));
            Assert.Equal(taskName, createdTask.Name);
            Assert.Equal(createTaskRequest.Description, createdTask.Description);
            // Add more assertions for other properties if set
        }

        [Fact]
        public async Task GetTaskAsync_WithExistingTaskId_ShouldReturnTask()
        {
            // Arrange
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available for this test. Check InitializeAsync and configuration.");
            var taskName = $"My Task To Get - {Guid.NewGuid()}";
            var createTaskRequest = new CreateTaskRequest(
                Name: taskName,
                Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null,
                NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null,
                CustomItemId: null, ListId: null
            );
            var createdTask = await _taskService.CreateTaskAsync(_testListId, createTaskRequest);
            RegisterCreatedTask(createdTask.Id);
            _output.LogInformation($"Task created for Get test. ID: {createdTask.Id}");

            // Act
            var fetchedTask = await _taskService.GetTaskAsync(createdTask.Id);
            _output.LogInformation($"Fetched task. ID: {fetchedTask?.Id}");

            // Assert
            Assert.NotNull(fetchedTask);
            Assert.Equal(createdTask.Id, fetchedTask.Id);
            Assert.Equal(taskName, fetchedTask.Name);
        }

        [Fact]
        public async Task UpdateTaskAsync_WithValidData_ShouldUpdateTask()
        {
            // Arrange
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available for this test. Check InitializeAsync and configuration.");
            var initialName = $"Initial Task Name - {Guid.NewGuid()}";
            var createTaskRequest = new CreateTaskRequest(
                Name: initialName,
                Description: "Initial Description", // This was set
                Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null,
                NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null,
                CustomItemId: null, ListId: null
            );
            var createdTask = await _taskService.CreateTaskAsync(_testListId, createTaskRequest);
            RegisterCreatedTask(createdTask.Id);
            _output.LogInformation($"Task created for Update test. ID: {createdTask.Id}, Name: {createdTask.Name}");


            var updatedName = $"Updated Task Name - {Guid.NewGuid()}";
            var updatedDescription = "This task has been updated by an integration test.";
            var updateTaskRequest = new UpdateTaskRequest(
                Name: updatedName,
                Description: updatedDescription,
                Status: null, Priority: null, DueDate: null, DueDateTime: null, Parent: null, TimeEstimate: null,
                StartDate: null, StartDateTime: null, Assignees: null, GroupAssignees: null, Archived: null, CustomFields: null
            );
            _output.LogInformation($"Attempting to update task '{createdTask.Id}' to name '{updatedName}'.");

            // Act
            var updatedTask = await _taskService.UpdateTaskAsync(createdTask.Id, updateTaskRequest);
            _output.LogInformation($"Task updated. ID: {updatedTask?.Id}, Name: {updatedTask?.Name}");

            // Assert
            Assert.NotNull(updatedTask);
            Assert.Equal(createdTask.Id, updatedTask.Id);
            Assert.Equal(updatedName, updatedTask.Name);
            Assert.Equal(updatedDescription, updatedTask.Description);

            // Optionally, re-fetch to confirm persistence
            var refetchedTask = await _taskService.GetTaskAsync(createdTask.Id);
            Assert.Equal(updatedName, refetchedTask.Name);
            Assert.Equal(updatedDescription, refetchedTask.Description);
        }

        [Fact]
        public async Task DeleteTaskAsync_WithExistingTaskId_ShouldDeleteTask()
        {
            // Arrange
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available for this test. Check InitializeAsync and configuration.");
            var taskName = $"Task To Delete - {Guid.NewGuid()}";
            var createTaskRequest = new CreateTaskRequest(
                Name: taskName,
                Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null,
                NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null,
                CustomItemId: null, ListId: null
            );
            var createdTask = await _taskService.CreateTaskAsync(_testListId, createTaskRequest);
            // Do NOT register this task with _createdTaskIds for auto-cleanup, as this test is testing the deletion.
            _output.LogInformation($"Task created for Delete test. ID: {createdTask.Id}");

            // Act
            await _taskService.DeleteTaskAsync(createdTask.Id);
            _output.LogInformation($"DeleteTaskAsync called for task ID: {createdTask.Id}.");

            // Assert
            // Try to get the task and expect a ClickUpApiNotFoundException (or similar)
            // This requires knowing the specific exception type.
            // For now, we'll assume it throws something identifiable or GetTaskAsync returns null.
            // The exact exception type might need to be adjusted based on actual behavior.
            await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _taskService.GetTaskAsync(createdTask.Id)
            );
            _output.LogInformation($"Verified task {createdTask.Id} is deleted (GetTaskAsync threw NotFound).");
        }

        [Fact]
        public async Task GetTasksAsync_FilterByStatus_ShouldReturnFilteredTasks()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            Assert.NotNull(_defaultStatus); // Ensure we have at least one status to test with

            var taskName1 = $"Task_StatusFilter_1_{_defaultStatus.StatusValue}_{Guid.NewGuid()}";
            var taskName2 = $"Task_StatusFilter_2_Other_{Guid.NewGuid()}";

            // Task 1: Default Status
            var task1 = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(
                Name: taskName1, Status: _defaultStatus.StatusValue, Description: null, Assignees: null, GroupAssignees: null,
                Tags: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null,
                StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null,
                CustomFields: null, CustomItemId: null, ListId: null
            ));
            RegisterCreatedTask(task1.Id);
            _output.LogInformation($"Created task1 '{task1.Name}' with status '{_defaultStatus.StatusValue}'. ID: {task1.Id}");

            // Task 2: Another Status (if available and different), otherwise also default status (test will be less effective but won't fail setup)
            string statusForTask2 = _defaultStatus.StatusValue;
            if (_anotherStatus != null && _anotherStatus.StatusValue != _defaultStatus.StatusValue)
            {
                statusForTask2 = _anotherStatus.StatusValue;
            }
            else if (_anotherStatus != null) // Same as default, log it
            {
                 _output.LogWarning($"_anotherStatus ('{_anotherStatus.StatusValue}') is same as _defaultStatus ('{_defaultStatus.StatusValue}'). Filter test might be less specific.");
            }
            else // No other status
            {
                _output.LogWarning($"_anotherStatus is null. Both tasks will have status '{_defaultStatus.StatusValue}'. Filter test might be less specific.");
            }

            var task2 = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(
                Name: taskName2, Status: statusForTask2, Description: null, Assignees: null, GroupAssignees: null,
                Tags: null, Priority: null, DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null,
                StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null,
                CustomFields: null, CustomItemId: null, ListId: null
            ));
            RegisterCreatedTask(task2.Id);
            _output.LogInformation($"Created task2 '{task2.Name}' with status '{statusForTask2}'. ID: {task2.Id}");


            // Act: Filter for tasks with the _defaultStatus
            var getTasksRequest = new GetTasksRequest
            {
                Statuses = new List<string> { _defaultStatus.StatusValue }
            };
            _output.LogInformation($"Fetching tasks from list '{_testListId}' filtering by status: '{_defaultStatus.StatusValue}'.");
            var response = await _taskService.GetTasksAsync(_testListId, getTasksRequest);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Tasks);

            // Check if task1 is present
            Assert.Contains(response.Tasks, t => t.Id == task1.Id && t.Status.StatusValue == _defaultStatus.StatusValue);
            _output.LogInformation($"Found task1 with ID {task1.Id} in filtered results.");

            // Check if task2 is NOT present IF its status was different
            if (statusForTask2 != _defaultStatus.StatusValue)
            {
                Assert.DoesNotContain(response.Tasks, t => t.Id == task2.Id);
                _output.LogInformation($"Correctly did not find task2 with ID {task2.Id} (status: {statusForTask2}) in filtered results for status '{_defaultStatus.StatusValue}'.");
            }
            else // If task2 had the same status, it should be present
            {
                Assert.Contains(response.Tasks, t => t.Id == task2.Id && t.Status.StatusValue == _defaultStatus.StatusValue);
                 _output.LogInformation($"Task2 with ID {task2.Id} also has status '{_defaultStatus.StatusValue}' and was found (as expected in this case).");
            }

            // Verify total count if desired and predictable
            if (statusForTask2 != _defaultStatus.StatusValue)
            {
                Assert.Single(response.Tasks.Where(t => t.Status.StatusValue == _defaultStatus.StatusValue));
            }
            // If statuses were the same, there could be more than 1, or exactly 2 if no other tasks exist.
            // For more precise count assertions, ensure no other tasks are created in the list by other parallel tests or previous runs.
            // The current IAsyncLifetime setup creates a new list for each test class run, which helps.
        }

        [Fact]
        public async Task GetTasksAsync_FilterByAssignee_ShouldReturnFilteredTasks()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            Assert.False(string.IsNullOrWhiteSpace(_currentUserId), "_currentUserId must be available.");

            var taskNameAssigned = $"Task_AssignedToMe_{Guid.NewGuid()}";
            var taskNameUnassigned = $"Task_Unassigned_{Guid.NewGuid()}"; // Or assigned to someone else

            // Task 1: Assigned to current user
            var createTaskRequestAssigned = new CreateTaskRequest(
                Name: taskNameAssigned,
                Assignees: new List<int> { int.Parse(_currentUserId) }, // API expects int list for assignees
                Description: null, GroupAssignees: null, Tags: null, Status: null, Priority: null, DueDate: null,
                DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null,
                Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null
            );
            var taskAssigned = await _taskService.CreateTaskAsync(_testListId, createTaskRequestAssigned);
            RegisterCreatedTask(taskAssigned.Id);
            _output.LogInformation($"Created task '{taskAssigned.Name}' assigned to user '{_currentUserId}'. ID: {taskAssigned.Id}");

            // Task 2: No specific assignees in request (behavior depends on ClickUp defaults for the list/space)
            var createTaskRequestUnassigned = new CreateTaskRequest(
                Name: taskNameUnassigned,
                Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null,
                NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null,
                CustomItemId: null, ListId: null
            );
            var taskUnassigned = await _taskService.CreateTaskAsync(_testListId, createTaskRequestUnassigned);
            RegisterCreatedTask(taskUnassigned.Id);
            _output.LogInformation($"Created task '{taskUnassigned.Name}' with no explicit assignee. ID: {taskUnassigned.Id}");

            // Act: Filter for tasks assigned to the current user
            var getTasksRequest = new GetTasksRequest
            {
                Assignees = new List<string> { _currentUserId }
            };
            _output.LogInformation($"Fetching tasks from list '{_testListId}' filtering by assignee: '{_currentUserId}'.");
            var response = await _taskService.GetTasksAsync(_testListId, getTasksRequest);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Tasks);

            // Check if taskAssigned is present
            var foundTaskAssigned = response.Tasks.FirstOrDefault(t => t.Id == taskAssigned.Id);
            Assert.NotNull(foundTaskAssigned);
            Assert.Contains(foundTaskAssigned.Assignees, a => a.Id.ToString() == _currentUserId);
            _output.LogInformation($"Found task '{foundTaskAssigned.Name}' with ID {foundTaskAssigned.Id} (assigned to {_currentUserId}) in filtered results.");

            // Check if taskUnassigned is NOT present (assuming it wasn't auto-assigned to _currentUserId AND _currentUserId was the only filter)
            // This assertion depends on whether ClickUp auto-assigns unrequested tasks to the creator.
            // If it does, this assertion will fail. For now, we'll assume it might not be assigned or assigned to someone else.
            var foundTaskUnassigned = response.Tasks.FirstOrDefault(t => t.Id == taskUnassigned.Id);
            if (foundTaskUnassigned != null)
            {
                _output.LogWarning($"Task '{taskUnassigned.Name}' (ID: {taskUnassigned.Id}) was found. Its assignees: {string.Join(", ", foundTaskUnassigned.Assignees.Select(a => a.Id))}. This might be due to auto-assignment by ClickUp.");
                Assert.DoesNotContain(foundTaskUnassigned.Assignees, a => a.Id.ToString() == _currentUserId);
            }
            else
            {
                _output.LogInformation($"Task '{taskUnassigned.Name}' (ID: {taskUnassigned.Id}) was correctly not found or not assigned to {_currentUserId} in filtered results.");
            }

            // More specific assertion: only tasks assigned to _currentUserId should be in the list.
            Assert.All(response.Tasks, task => Assert.Contains(task.Assignees, a => a.Id.ToString() == _currentUserId));
            _output.LogInformation($"Verified all tasks in the response are assigned to user '{_currentUserId}'.");
        }

        [Fact]
        public async Task GetTasksAsync_FilterByDueDate_ShouldReturnFilteredTasks()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");

            var today = DateTimeOffset.UtcNow;
            var tomorrow = today.AddDays(1);
            var dayAfterTomorrow = today.AddDays(2);
            var weekLater = today.AddDays(7);

            long todayTimestampMs = today.ToUnixTimeMilliseconds();
            long tomorrowTimestampMs = tomorrow.ToUnixTimeMilliseconds();
            long dayAfterTomorrowTimestampMs = dayAfterTomorrow.ToUnixTimeMilliseconds();
            long weekLaterTimestampMs = weekLater.ToUnixTimeMilliseconds();

            // Task 1: Due Today
            var taskDueToday = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(
                Name: $"Task_DueToday_{Guid.NewGuid()}", DueDate: today, DueDateTime: true, // Use DateTimeOffset directly
                Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null,
                CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null
            ));
            RegisterCreatedTask(taskDueToday.Id);
            _output.LogInformation($"Created task '{taskDueToday.Name}' due today (timestamp: {todayTimestampMs}). ID: {taskDueToday.Id}");

            // Task 2: Due Next Week
            var taskDueNextWeek = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(
                Name: $"Task_DueNextWeek_{Guid.NewGuid()}", DueDate: weekLater, DueDateTime: true, // Use DateTimeOffset directly
                Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null, LinksTo: null,
                CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null
            ));
            RegisterCreatedTask(taskDueNextWeek.Id);
            _output.LogInformation($"Created task '{taskDueNextWeek.Name}' due next week (timestamp: {weekLaterTimestampMs}). ID: {taskDueNextWeek.Id}");

            // Act: Filter for tasks due on or before tomorrow
            var getTasksRequest = new GetTasksRequest
            {
                DueDateLessThan = dayAfterTomorrowTimestampMs // Due on or before dayAfterTomorrow (exclusive of its very start, effectively today or tomorrow)
            };
             _output.LogInformation($"Fetching tasks from list '{_testListId}' filtering by due date less than {dayAfterTomorrowTimestampMs} (Day After Tomorrow).");
            var response = await _taskService.GetTasksAsync(_testListId, getTasksRequest);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Tasks);

            Assert.Contains(response.Tasks, t => t.Id == taskDueToday.Id);
            _output.LogInformation($"Found task '{taskDueToday.Name}' in results.");
            Assert.DoesNotContain(response.Tasks, t => t.Id == taskDueNextWeek.Id);
            _output.LogInformation($"Correctly did not find task '{taskDueNextWeek.Name}' (due next week) in results for due date < DayAfterTomorrow.");

            // Act: Filter for tasks due after tomorrow
            getTasksRequest = new GetTasksRequest
            {
                DueDateGreaterThan = tomorrowTimestampMs // Due after tomorrow (exclusive of its very start, so effectively dayAfterTomorrow or later)
            };
            _output.LogInformation($"Fetching tasks from list '{_testListId}' filtering by due date greater than {tomorrowTimestampMs} (Tomorrow).");
            response = await _taskService.GetTasksAsync(_testListId, getTasksRequest);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Tasks);
            Assert.DoesNotContain(response.Tasks, t => t.Id == taskDueToday.Id);
            _output.LogInformation($"Correctly did not find task '{taskDueToday.Name}' (due today) in results for due date > Tomorrow.");
            Assert.Contains(response.Tasks, t => t.Id == taskDueNextWeek.Id);
            _output.LogInformation($"Found task '{taskDueNextWeek.Name}' in results for due date > Tomorrow.");
        }

        [Fact]
        public async Task GetTasksAsync_FilterByTags_ShouldReturnFilteredTasks()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            Assert.False(string.IsNullOrWhiteSpace(_testSpaceId), "_testSpaceId must be available to create tags.");

            var tagName = $"TestTag_{Guid.NewGuid()}";
            var tagColorBg = "#FF0000"; // Red
            var tagColorFg = "#FFFFFF"; // White

            // Create the tag in the space
            var createTagRequest = new ClickUp.Api.Client.Models.RequestModels.Spaces.ModifyTagRequest
            {
                Name = tagName,
                TagBackgroundColor = tagColorBg,
                TagForegroundColor = tagColorFg
            };
            await _tagsService.CreateSpaceTagAsync(_testSpaceId, createTagRequest);
            _createdTagNamesForCleanup.Add(tagName); // Ensure cleanup
            _output.LogInformation($"Created tag '{tagName}' in space '{_testSpaceId}'.");

            // Task 1: With the tag
            var taskWithTag = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(
                Name: $"Task_WithTag_{tagName}_{Guid.NewGuid()}",
                Tags: new List<string> { tagName },
                Description: null, Assignees: null, GroupAssignees: null, Status: null, Priority: null, DueDate: null,
                DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null,
                Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null
            ));
            RegisterCreatedTask(taskWithTag.Id);
            _output.LogInformation($"Created task '{taskWithTag.Name}' with tag '{tagName}'. ID: {taskWithTag.Id}");

            // Task 2: Without the tag
            var taskWithoutTag = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(
                Name: $"Task_WithoutTag_{Guid.NewGuid()}",
                Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null,
                NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null,
                CustomItemId: null, ListId: null
            ));
            RegisterCreatedTask(taskWithoutTag.Id);
            _output.LogInformation($"Created task '{taskWithoutTag.Name}' without the tag. ID: {taskWithoutTag.Id}");

            // Act: Filter for tasks with the tag
            var getTasksRequest = new GetTasksRequest
            {
                Tags = new List<string> { tagName }
            };
            _output.LogInformation($"Fetching tasks from list '{_testListId}' filtering by tag: '{tagName}'.");
            var response = await _taskService.GetTasksAsync(_testListId, getTasksRequest);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Tasks);

            Assert.Contains(response.Tasks, t => t.Id == taskWithTag.Id && t.Tags.Any(tag => tag.Name == tagName));
            _output.LogInformation($"Found task '{taskWithTag.Name}' with tag '{tagName}' in filtered results.");
            Assert.DoesNotContain(response.Tasks, t => t.Id == taskWithoutTag.Id);
            _output.LogInformation($"Correctly did not find task '{taskWithoutTag.Name}' (without tag) in filtered results for tag '{tagName}'.");

            Assert.Single(response.Tasks.Where(t => t.Tags.Any(tag => tag.Name == tagName)));
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_FilterByListId_ShouldReturnTasksFromSpecifiedList()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testWorkspaceId), "_testWorkspaceId must be available.");
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");

            // Create a couple of tasks in the primary test list
            var taskInList1 = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(
                Name: $"Task_In_TestList1_{Guid.NewGuid()}", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null,
                LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null));
            RegisterCreatedTask(taskInList1.Id);
            var taskInList2 = await _taskService.CreateTaskAsync(_testListId, new CreateTaskRequest(
                Name: $"Task_In_TestList2_{Guid.NewGuid()}", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null,
                LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null));
            RegisterCreatedTask(taskInList2.Id);
            _output.LogInformation($"Created tasks {taskInList1.Id} and {taskInList2.Id} in list {_testListId}.");

            // For a more robust test, create another list and a task in it,
            // then ensure this other task is NOT returned.
            // This requires creating another folder and list.
            var otherListName = $"OtherList_TeamTaskFilter_{Guid.NewGuid()}";
            var otherList = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest(
                Name: otherListName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null
            ));
             _output.LogInformation($"Created other list {otherList.Id} for negative test case.");
            // We need to ensure this list is cleaned up too, but since it's in _testFolderId, DisposeAsync should handle it.

            var taskInOtherList = await _taskService.CreateTaskAsync(otherList.Id, new CreateTaskRequest(
                Name: $"Task_In_OtherList_{Guid.NewGuid()}", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null,
                LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null));
            // Register this task too, so it's cleaned up by the main DisposeAsync if the list isn't deleted first.
            RegisterCreatedTask(taskInOtherList.Id);
            _output.LogInformation($"Created task {taskInOtherList.Id} in other list {otherList.Id}.");


            // Act: Filter team tasks by the primary _testListId
            _output.LogInformation($"Fetching team tasks from workspace '{_testWorkspaceId}' filtering by list ID: '{_testListId}'.");
            var response = await _taskService.GetFilteredTeamTasksAsync(
                workspaceId: _testWorkspaceId,
                listIds: new List<string> { _testListId }
            );

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Tasks);

            Assert.Contains(response.Tasks, t => t.Id == taskInList1.Id);
            _output.LogInformation($"Found task {taskInList1.Id} in filtered team tasks.");
            Assert.Contains(response.Tasks, t => t.Id == taskInList2.Id);
            _output.LogInformation($"Found task {taskInList2.Id} in filtered team tasks.");
            Assert.DoesNotContain(response.Tasks, t => t.Id == taskInOtherList.Id);
            _output.LogInformation($"Correctly did not find task {taskInOtherList.Id} (from other list) in filtered team tasks.");

            // Ensure all tasks returned actually belong to the queried list
            // This is a bit redundant if the previous assertions are correct but good for strictness.
            // The CuTask object from GetFilteredTeamTasks might not directly contain ListId,
            // but it should have a list property. Let's check CuTask structure.
            // For now, we rely on the API correctly filtering.
            // If CuTask has t.List.Id, we could do:
            // Assert.All(response.Tasks, t => Assert.Equal(_testListId, t.List.Id));

            // We also need to delete the 'otherList' if we created it and it's not auto-cleaned by folder deletion.
            // The current DisposeAsync deletes the folder, which should delete all lists in it.
            // If otherList was created directly under the space, it would need separate cleanup.
            // Since it's created in _testFolderId, it should be fine.
        }

        [Fact]
        public async Task GetTasksAsyncEnumerableAsync_ShouldRetrieveAllTasksInList()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");

            int tasksToCreate = 5; // A number likely to span pages if API default page size is small, or just to test streaming logic
            var createdTaskIds = new List<string>();

            _output.LogInformation($"Creating {tasksToCreate} tasks for pagination stream test in list '{_testListId}'.");
            for (int i = 0; i < tasksToCreate; i++)
            {
                var taskName = $"Paginated Task {i + 1} - {Guid.NewGuid()}";
                var createTaskReq = new CreateTaskRequest(
                    Name: taskName, Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                    DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null,
                    LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null);
                var createdTask = await _taskService.CreateTaskAsync(_testListId, createTaskReq);
                RegisterCreatedTask(createdTask.Id); // Ensure they are cleaned up by DisposeAsync
                createdTaskIds.Add(createdTask.Id);
                _output.LogInformation($"Created task {i + 1}/{tasksToCreate}, ID: {createdTask.Id}");
                await Task.Delay(200); // Be nice to the API
            }

            var retrievedTasks = new List<ClickUp.Api.Client.Models.Entities.Tasks.CuTask>();
            int count = 0;
            _output.LogInformation($"Starting to stream tasks for list '{_testListId}'.");

            // Using the GetTasksAsyncEnumerableAsync method
            await foreach (var task in _taskService.GetTasksAsyncEnumerableAsync(_testListId))
            {
                count++;
                retrievedTasks.Add(task);
                _output.LogInformation($"Streamed task {count}: ID {task.Id}, Name: '{task.Name}'...");
            }

            _output.LogInformation($"Finished streaming tasks. Total tasks received: {count}");

            Assert.Equal(tasksToCreate, count);
            Assert.Equal(tasksToCreate, retrievedTasks.Count);

            // Verify that all created tasks were retrieved
            foreach (var createdId in createdTaskIds)
            {
                Assert.Contains(retrievedTasks, rt => rt.Id == createdId);
            }
            _output.LogInformation($"All {tasksToCreate} created tasks were found in the streamed results from list '{_testListId}'.");
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsyncEnumerableAsync_ShouldRetrieveAllTasksInSpecifiedList()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testWorkspaceId), "_testWorkspaceId must be available.");
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");

            int tasksToCreateInTestList = 3; // Create a few tasks specifically in our main test list
            var createdTaskIdsInTestList = new List<string>();

            _output.LogInformation($"Creating {tasksToCreateInTestList} tasks for team task pagination stream test in list '{_testListId}'.");
            for (int i = 0; i < tasksToCreateInTestList; i++)
            {
                var taskName = $"TeamPaginated Task {i + 1} in List {_testListId} - {Guid.NewGuid()}";
                var createTaskReq = new CreateTaskRequest(
                    Name: taskName, Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                    DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null,
                    LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null);
                var createdTask = await _taskService.CreateTaskAsync(_testListId, createTaskReq);
                RegisterCreatedTask(createdTask.Id);
                createdTaskIdsInTestList.Add(createdTask.Id);
                _output.LogInformation($"Created task {i + 1}/{tasksToCreateInTestList}, ID: {createdTask.Id} in list {_testListId}.");
                await Task.Delay(200);
            }

            // Optionally, create another task in another list to ensure it's NOT streamed when filtering by _testListId
            var otherListName = $"OtherList_TeamPagStream_{Guid.NewGuid()}";
            var otherList = await _listService.CreateListInFolderAsync(_testFolderId, new CreateListRequest(
                Name: otherListName, Content: null, MarkdownContent: null, DueDate: null, DueDateTime: null, Priority: null, Assignee: null, Status: null
            ));
            var taskInOtherList = await _taskService.CreateTaskAsync(otherList.Id, new CreateTaskRequest(
                Name: $"Task_In_OtherList_TeamPagStream_{Guid.NewGuid()}", Description: null, Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null, NotifyAll: null, Parent: null,
                LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null, CustomItemId: null, ListId: null));
            RegisterCreatedTask(taskInOtherList.Id); // Ensure cleanup
            _output.LogInformation($"Created task {taskInOtherList.Id} in other list {otherList.Id} for negative check.");


            var retrievedTasks = new List<ClickUp.Api.Client.Models.Entities.Tasks.CuTask>();
            int count = 0;
            _output.LogInformation($"Starting to stream team tasks for workspace '{_testWorkspaceId}', filtered by list '{_testListId}'.");

            await foreach (var task in _taskService.GetFilteredTeamTasksAsyncEnumerableAsync(
                                            _testWorkspaceId,
                                            listIds: new List<string> { _testListId }))
            {
                count++;
                retrievedTasks.Add(task);
                _output.LogInformation($"Streamed team task {count}: ID {task.Id}, Name: '{task.Name}', List ID: {task.List?.Id}'...");
            }

            _output.LogInformation($"Finished streaming team tasks. Total tasks received: {count}");

            Assert.Equal(tasksToCreateInTestList, count);
            Assert.Equal(tasksToCreateInTestList, retrievedTasks.Count);

            foreach (var createdId in createdTaskIdsInTestList)
            {
                Assert.Contains(retrievedTasks, rt => rt.Id == createdId);
            }
            _output.LogInformation($"All {tasksToCreateInTestList} tasks created in list '{_testListId}' were found in the streamed team results.");

            // Ensure task from otherList was not streamed
            Assert.DoesNotContain(retrievedTasks, rt => rt.Id == taskInOtherList.Id);
            _output.LogInformation($"Task {taskInOtherList.Id} from other list was correctly not found in streamed results.");
        }

        [Fact]
        public async Task GetTaskAsync_WithNonExistentTaskId_ShouldThrowNotFoundException()
        {
            var nonExistentTaskId = "0"; // Or Guid.NewGuid().ToString(); ClickUp IDs are usually not "0"
            _output.LogInformation($"Attempting to get non-existent task with ID: {nonExistentTaskId}");

            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _taskService.GetTaskAsync(nonExistentTaskId)
            );

            _output.LogInformation($"Received expected ClickUpApiNotFoundException: {exception.Message}");
            Assert.NotNull(exception);
            // Optionally, check exception.ErrorCode or a pattern in exception.Message if consistent
        }

        [Fact]
        public async Task UpdateTaskAsync_WithNonExistentTaskId_ShouldThrowNotFoundException()
        {
            var nonExistentTaskId = "0";
            var updateRequest = new UpdateTaskRequest(
                Name: "Attempt to update non-existent task", Description: null, Status: null, Priority: null, DueDate: null,
                DueDateTime: null, Parent: null, TimeEstimate: null, StartDate: null, StartDateTime: null, Assignees: null,
                GroupAssignees: null, Archived: null, CustomFields: null
            );
            _output.LogInformation($"Attempting to update non-existent task with ID: {nonExistentTaskId}");

            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _taskService.UpdateTaskAsync(nonExistentTaskId, updateRequest)
            );

            _output.LogInformation($"Received expected ClickUpApiNotFoundException: {exception.Message}");
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task DeleteTaskAsync_WithNonExistentTaskId_ShouldThrowNotFoundException()
        {
            var nonExistentTaskId = "0";
            _output.LogInformation($"Attempting to delete non-existent task with ID: {nonExistentTaskId}");

            // Delete usually returns 204 No Content on success.
            // For a non-existent ID, ClickUp API returns 404, which our client translates to ClickUpApiNotFoundException.
            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _taskService.DeleteTaskAsync(nonExistentTaskId)
            );

            _output.LogInformation($"Received expected ClickUpApiNotFoundException: {exception.Message}");
            Assert.NotNull(exception);
        }
    }
}
