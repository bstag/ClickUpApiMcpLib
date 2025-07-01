using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure;
using ClickUp.Api.Client.Models.Entities.Tasks; // For CuTask
using ClickUp.Api.Client.Models.RequestModels.Tasks; // For CreateTaskRequest
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp; // For MockHttpMessageHandler and HttpMethod
using Xunit;
using Xunit.Abstractions;

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class TaskRelationshipsServiceIntegrationTests : IntegrationTestBase, IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private ITaskRelationshipsService _taskRelationshipsService = null!;
        private ITasksService _tasksService = null!; // For creating prerequisite tasks
        private string _testWorkspaceId = null!;
        private string _testListId = null!; // A list to create tasks in

        // Store IDs of created resources for cleanup
        private readonly List<string> _createdTaskIdsForCleanup = new List<string>();

        public TaskRelationshipsServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            // Services will be resolved in InitializeAsync
        }

        public async Task InitializeAsync()
        {
            _output.LogInformation($"[TaskRelationshipsServiceIntegrationTests] Initializing. Test Mode: {CurrentTestMode}");

            _taskRelationshipsService = ServiceProvider.GetRequiredService<ITaskRelationshipsService>();
            _tasksService = ServiceProvider.GetRequiredService<ITasksService>();

            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"];
            if (string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured. Tests may fail.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for these tests.");
            }

            // For task creation, we need a List.
            // Option 1: Use a pre-existing List ID from config.
            _testListId = Configuration["ClickUpApi:TestListIdForRelationships"]!;
            if (string.IsNullOrWhiteSpace(_testListId))
            {
                _output.LogWarning("ClickUpApi:TestListIdForRelationships is not configured. Tests requiring task creation will fail in Record/Passthrough mode.");
                // In Playback mode, we can mock list creation or use mocked task IDs directly without a real list.
                // For now, we'll assume Playback mocks will handle task creation without needing a real list ID.
                if (CurrentTestMode != TestMode.Playback)
                {
                    throw new InvalidOperationException("ClickUpApi:TestListIdForRelationships must be configured for Record/Passthrough modes.");
                }
                else
                {
                     _testListId = "mocked_list_id_for_relationships"; // Placeholder for playback if needed
                }
            }
            _output.LogInformation($"Using Workspace ID: {_testWorkspaceId}, List ID: {_testListId}");

            // Mock common setup calls for Playback mode if any
            if (CurrentTestMode == TestMode.Playback)
            {
                // Example: If we were programmatically fetching the list ID:
                // MockHttpHandler.When($"https://api.clickup.com/api/v2/space/{_someSpaceId}/list")
                //                .Respond("application/json", "{\"lists\":[{\"id\":\"mocked_list_id_for_relationships\",\"name\":\"Mocked Test List\"}]}");
            }
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _output.LogInformation($"[TaskRelationshipsServiceIntegrationTests] Disposing. Cleaning up {_createdTaskIdsForCleanup.Count} tasks.");
            var cleanupTasks = new List<Task>();
            foreach (var taskId in _createdTaskIdsForCleanup)
            {
                var currentTaskId = taskId; // Capture variable for lambda
                cleanupTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        if (CurrentTestMode == TestMode.Playback)
                        {
                            _output.LogInformation($"[Playback] Expecting DELETE mock for task {currentTaskId} during cleanup.");
                            // The actual mock for DELETE task should be set up in the test method
                            // that created this task and registered it for cleanup.
                            // If the test itself calls DeleteTestTaskAsync, that method would handle the mock.
                            // If cleanup is deferred to here, the test must have set up the mock.
                        }
                        await DeleteTestTaskAsync(currentTaskId); // This helper will handle mocking if in Playback
                    }
                    catch (Exception ex)
                    {
                        _output.LogError($"Error deleting task {currentTaskId}: {ex.Message}", ex);
                    }
                }));
            }

            if (cleanupTasks.Any())
            {
                await Task.WhenAll(cleanupTasks);
            }
            _createdTaskIdsForCleanup.Clear();
            _output.LogInformation("[TaskRelationshipsServiceIntegrationTests] Disposal complete.");
        }

        private async Task<CuTask> CreateTestTaskAsync(string name, string? listId = null)
        {
            listId = listId ?? _testListId;
            if (string.IsNullOrWhiteSpace(listId) && CurrentTestMode != TestMode.Playback) {
                 throw new InvalidOperationException("List ID for task creation is not set and not in Playback mode.");
            }

            var request = new CreateTaskRequest(
                Name: name,
                Description: null,
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
                ListId: null // listId is passed in URL for _tasksService.CreateTaskAsync
            );
            CuTask? createdTask = null;

            _output.LogInformation($"Creating task '{name}' in list '{listId}'. Mode: {CurrentTestMode}");

            if (CurrentTestMode == TestMode.Playback)
            {
                // Playback logic for creating a task is usually specific to the test
                // (e.g., expecting a specific mocked ID).
                // This helper might need to be more flexible or tests set up mocks directly.
                // For now, assume the calling test sets up the specific mock for task creation.
                _output.LogInformation($"[Playback] CreateTestTaskAsync called for '{name}'. Expecting specific mock in test method.");
                // Fall through to service call, which will use MockHttpHandler
            }

            try
            {
                 createdTask = await _tasksService.CreateTaskAsync(listId!, request); // listId is asserted not null for non-Playback
            }
            catch(Exception ex)
            {
                _output.LogError($"Failed to create task '{name}' in list '{listId}': {ex.Message}", ex);
                if (CurrentTestMode == TestMode.Record) _output.LogWarning("Ensure recording is active for CreateTask if this is unexpected.");
                throw;
            }


            Assert.NotNull(createdTask);
            Assert.False(string.IsNullOrWhiteSpace(createdTask.Id));
            _createdTaskIdsForCleanup.Add(createdTask.Id);
            _output.LogInformation($"Task '{createdTask.Name}' created with ID '{createdTask.Id}'. Registered for cleanup.");
            return createdTask;
        }

        private async Task DeleteTestTaskAsync(string taskId)
        {
            _output.LogInformation($"Deleting task '{taskId}'. Mode: {CurrentTestMode}");
            if (CurrentTestMode == TestMode.Playback)
            {
                // Similar to CreateTestTaskAsync, the specific mock for deletion
                // should ideally be set up by the test method that created the task.
                _output.LogInformation($"[Playback] DeleteTestTaskAsync called for '{taskId}'. Expecting specific mock in test method or DisposeAsync setup.");
                // Fall through to service call
            }

            try
            {
                await _tasksService.DeleteTaskAsync(taskId, new DeleteTaskRequest()); // Pass empty request DTO
                _output.LogInformation($"Task '{taskId}' deleted successfully or mock processed.");
            }
            catch (Exception ex)
            {
                _output.LogError($"Failed to delete task '{taskId}': {ex.Message}", ex);
                if (CurrentTestMode == TestMode.Record) _output.LogWarning("Ensure recording is active for DeleteTask if this is unexpected.");
                // Do not rethrow from delete helper to allow other cleanups
            }
        }

        // Test methods will be added in subsequent steps

        [Fact]
        public async Task AddDependencyAsync_DependsOn_ShouldSucceed()
        {
            // Arrange
            var taskAName = $"TaskA_DependsOn_{Guid.NewGuid()}";
            var taskBName = $"TaskB_Target_{Guid.NewGuid()}";

            string taskAId = "mocked-taskA-dependsOn-id";
            string taskBId = "mocked-taskB-dependsOn-target-id";
            string postDependencyBodyHash = "bodyHashDependsOnSuccess"; // Placeholder - will be determined by recording

            CuTask taskA = null!;
            CuTask taskB = null!;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // 1. Mock creation of Task A
                var createTaskAResponsePath = Path.Combine(RecordedResponsesBasePath, "TasksService", "CreateTask", $"Success_TaskA_DependsOn_{taskAId}.json");
                File.WriteAllText(createTaskAResponsePath, $"{{\"id\":\"{taskAId}\", \"name\":\"{taskAName}\"}}"); // Simplified JSON
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .WithPartialContent(taskAName) // Try to match based on name
                               .Respond("application/json", await File.ReadAllTextAsync(createTaskAResponsePath));

                // 2. Mock creation of Task B
                var createTaskBResponsePath = Path.Combine(RecordedResponsesBasePath, "TasksService", "CreateTask", $"Success_TaskB_DependsOn_Target_{taskBId}.json");
                File.WriteAllText(createTaskBResponsePath, $"{{\"id\":\"{taskBId}\", \"name\":\"{taskBName}\"}}"); // Simplified JSON
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .WithPartialContent(taskBName)
                               .Respond("application/json", await File.ReadAllTextAsync(createTaskBResponsePath));

                // 3. Mock AddDependency call
                // POST /api/v2/task/{task_id}/dependency
                // Body: { "depends_on": "task_id_2" }
                // Response: 200 OK with the task object (or empty if service is void)
                // The service method is void, so ClickUp API might return 200 OK with task or 204. Assuming 200 OK for recording.
                var addDependencyResponsePath = Path.Combine(RecordedResponsesBasePath, "TaskRelationshipsService", "POSTAddDependency", $"DependsOn_Success_{postDependencyBodyHash}.json");
                // The actual API returns the task. Our service method is void. So empty JSON response is fine for mock.
                File.WriteAllText(addDependencyResponsePath, "{}");
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/task/{taskAId}/dependency")
                               // .WithBody(...) // TODO: Figure out how to match body for {"depends_on": taskBId }
                               .Respond(HttpStatusCode.OK, "application/json", await File.ReadAllTextAsync(addDependencyResponsePath));

                // 4. Mock deletions for cleanup
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskAId}")
                               .Respond(HttpStatusCode.NoContent);
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskBId}")
                               .Respond(HttpStatusCode.NoContent);
            }

            taskA = await CreateTestTaskAsync(taskAName);
            taskB = await CreateTestTaskAsync(taskBName);

            if (CurrentTestMode == TestMode.Playback) // Override with mocked IDs for playback consistency
            {
                 taskA = new CuTask(taskAId, null, null, taskAName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                 taskB = new CuTask(taskBId, null, null, taskBName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }


            // Act
            Exception? thrownException = null;
            try
            {
                await _taskRelationshipsService.AddDependencyAsync(taskA.Id, dependsOnTaskId: taskB.Id);
                _output.LogInformation($"Successfully added 'depends_on' dependency from Task {taskA.Id} to Task {taskB.Id}.");
            }
            catch (Exception ex)
            {
                thrownException = ex;
                _output.LogError($"Error adding 'depends_on' dependency: {ex.Message}", ex);
            }

            // Assert
            Assert.Null(thrownException);

            // Optional: Verify dependency (would require GET task and checking its dependencies array)
            // This would need additional mocking for GET /task/{taskA.Id} in Playback mode.
            // For now, a successful call without exception is the primary assertion.
        }

        [Fact]
        public async Task AddDependencyAsync_DependencyOf_ShouldSucceed()
        {
            // Arrange
            var taskAName = $"TaskA_DepOf_{Guid.NewGuid()}"; // Task that will have a task depending on it
            var taskBName = $"TaskB_Source_{Guid.NewGuid()}"; // Task that will depend on Task A

            string taskAId = "mocked-taskA-depOf-id";
            string taskBId = "mocked-taskB-depOf-source-id";
            string postDependencyBodyHash = "bodyHashDependencyOfSuccess"; // Placeholder

            CuTask taskA = null!;
            CuTask taskB = null!;


            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // 1. Mock creation of Task A
                var createTaskAResponsePath = Path.Combine(RecordedResponsesBasePath, "TasksService", "CreateTask", $"Success_TaskA_DepOf_{taskAId}.json");
                File.WriteAllText(createTaskAResponsePath, $"{{\"id\":\"{taskAId}\", \"name\":\"{taskAName}\"}}");
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .WithPartialContent(taskAName)
                               .Respond("application/json", await File.ReadAllTextAsync(createTaskAResponsePath));

                // 2. Mock creation of Task B
                var createTaskBResponsePath = Path.Combine(RecordedResponsesBasePath, "TasksService", "CreateTask", $"Success_TaskB_DepOf_Source_{taskBId}.json");
                File.WriteAllText(createTaskBResponsePath, $"{{\"id\":\"{taskBId}\", \"name\":\"{taskBName}\"}}");
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .WithPartialContent(taskBName)
                               .Respond("application/json", await File.ReadAllTextAsync(createTaskBResponsePath));

                // 3. Mock AddDependency call for "dependency_of"
                // POST /api/v2/task/{task_id}/dependency
                // Body: { "dependency_of": "task_id_0" } -> taskB is dependency_of taskA
                var addDependencyResponsePath = Path.Combine(RecordedResponsesBasePath, "TaskRelationshipsService", "POSTAddDependency", $"DependencyOf_Success_{postDependencyBodyHash}.json");
                File.WriteAllText(addDependencyResponsePath, "{}"); // Empty JSON as service method is void
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/task/{taskAId}/dependency")
                               // .WithBody(...) // TODO: Match body for {"dependency_of": taskBId }
                               .Respond(HttpStatusCode.OK, "application/json", await File.ReadAllTextAsync(addDependencyResponsePath));

                // 4. Mock deletions for cleanup
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskAId}")
                               .Respond(HttpStatusCode.NoContent);
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskBId}")
                               .Respond(HttpStatusCode.NoContent);
            }

            taskA = await CreateTestTaskAsync(taskAName); // Task that taskB will depend on
            taskB = await CreateTestTaskAsync(taskBName); // Task that depends on taskA

            if (CurrentTestMode == TestMode.Playback)
            {
                 taskA = new CuTask(taskAId, null, null, taskAName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                 taskB = new CuTask(taskBId, null, null, taskBName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }

            // Act: Add taskB as a task that is a "dependency of" taskA
            // This means taskB depends on taskA.
            // The call is made on taskA's ID, specifying taskB is its 'dependency_of'.
            Exception? thrownException = null;
            try
            {
                await _taskRelationshipsService.AddDependencyAsync(taskA.Id, dependencyOfTaskId: taskB.Id);
                _output.LogInformation($"Successfully added 'dependency_of' relationship: Task {taskB.Id} now depends on Task {taskA.Id}.");
            }
            catch (Exception ex)
            {
                thrownException = ex;
                _output.LogError($"Error adding 'dependency_of' relationship: {ex.Message}", ex);
            }

            // Assert
            Assert.Null(thrownException);
        }

        [Fact]
        public async Task DeleteDependencyAsync_ShouldSucceed()
        {
            // Arrange
            var taskAName = $"TaskA_DelDep_{Guid.NewGuid()}";
            var taskBName = $"TaskB_DelDepTarget_{Guid.NewGuid()}";

            string taskAId = "mocked-taskA-delDep-id";
            string taskBId = "mocked-taskB-delDep-target-id";
            // Hashes for recorded responses - will be determined by actual recording
            string createAMockFile = $"Success_TaskA_DelDep_{taskAId}.json";
            string createBMockFile = $"Success_TaskB_DelDepTarget_{taskBId}.json";
            string addDepBodyHash = "bodyHashAddDepForDelete";
            string addDepMockFile = $"DependsOn_Success_{addDepBodyHash}.json";
            string deleteDepQueryHash = "queryHashDeleteDepSuccess"; // For DELETE /task/{id}/dependency?depends_on={id2}

            CuTask taskA = null!;
            CuTask taskB = null!;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);

                // 1. Mock Task A creation
                var createTaskAResponsePath = Path.Combine(RecordedResponsesBasePath, "TasksService", "CreateTask", createAMockFile);
                File.WriteAllText(createTaskAResponsePath, $"{{\"id\":\"{taskAId}\", \"name\":\"{taskAName}\"}}");
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .WithPartialContent(taskAName)
                               .Respond("application/json", await File.ReadAllTextAsync(createTaskAResponsePath));

                // 2. Mock Task B creation
                var createTaskBResponsePath = Path.Combine(RecordedResponsesBasePath, "TasksService", "CreateTask", createBMockFile);
                File.WriteAllText(createTaskBResponsePath, $"{{\"id\":\"{taskBId}\", \"name\":\"{taskBName}\"}}");
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .WithPartialContent(taskBName)
                               .Respond("application/json", await File.ReadAllTextAsync(createTaskBResponsePath));

                // 3. Mock AddDependency call (prerequisite)
                var addDependencyResponsePath = Path.Combine(RecordedResponsesBasePath, "TaskRelationshipsService", "POSTAddDependency", addDepMockFile);
                File.WriteAllText(addDependencyResponsePath, "{}"); // Service method is void
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/task/{taskAId}/dependency")
                               // .WithBody(...) // Match body for {"depends_on": taskBId }
                               .Respond(HttpStatusCode.OK, "application/json", await File.ReadAllTextAsync(addDependencyResponsePath));

                // 4. Mock DeleteDependency call (the actual method under test)
                // DELETE /api/v2/task/{task_id}/dependency?depends_on={depends_on_task_id}
                // Response: 200 OK (empty or task) or 204 No Content. Service method is void.
                var deleteDependencyResponsePath = Path.Combine(RecordedResponsesBasePath, "TaskRelationshipsService", "DELETEDeleteDependency", $"Success_{deleteDepQueryHash}.json");
                File.WriteAllText(deleteDependencyResponsePath, "{}"); // Service method is void
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskAId}/dependency?depends_on={taskBId}")
                               .Respond(HttpStatusCode.OK, "application/json", await File.ReadAllTextAsync(deleteDependencyResponsePath));

                // 5. Mock deletions for cleanup
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskAId}")
                               .Respond(HttpStatusCode.NoContent);
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskBId}")
                               .Respond(HttpStatusCode.NoContent);
            }

            taskA = await CreateTestTaskAsync(taskAName);
            taskB = await CreateTestTaskAsync(taskBName);

            if (CurrentTestMode == TestMode.Playback) // Ensure IDs are consistent for playback
            {
                taskA = new CuTask(taskAId, null, null, taskAName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                taskB = new CuTask(taskBId, null, null, taskBName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }

            // Add dependency first
            await _taskRelationshipsService.AddDependencyAsync(taskA.Id, dependsOnTaskId: taskB.Id);
            _output.LogInformation($"Setup: Added 'depends_on' dependency from Task {taskA.Id} to Task {taskB.Id}.");

            // Act
            Exception? thrownException = null;
            try
            {
                await _taskRelationshipsService.DeleteDependencyAsync(taskA.Id, new Models.RequestModels.TaskRelationships.DeleteDependencyRequest(dependsOnTaskId: taskB.Id));
                _output.LogInformation($"Successfully deleted 'depends_on' dependency from Task {taskA.Id} to Task {taskB.Id}.");
            }
            catch (Exception ex)
            {
                thrownException = ex;
                _output.LogError($"Error deleting 'depends_on' dependency: {ex.Message}", ex);
            }

            // Assert
            Assert.Null(thrownException);
            // Optional: Verify by trying to get task A and checking its dependencies (would need more mocking).
        }

        [Fact]
        public async Task AddTaskLinkAsync_ShouldReturnLinkedTask()
        {
            // Arrange
            var taskFromName = $"TaskFrom_Link_{Guid.NewGuid()}";
            var taskToName = $"TaskTo_Link_{Guid.NewGuid()}";

            string taskFromId = "mocked-taskFrom-link-id";
            string taskToId = "mocked-taskTo-link-id";
            // Hashes/filenames for recorded responses
            string createTaskFromMockFile = $"Success_TaskFrom_Link_{taskFromId}.json";
            string createTaskToMockFile = $"Success_TaskTo_Link_{taskToId}.json";
            // For POST /task/{task_id}/link/{links_to_task_id}, the API returns the source task.
            string addTaskLinkResponseMockFile = $"Success_Link_{taskFromId}_to_{taskToId}.json";

            CuTask taskFrom = null!;
            CuTask taskTo = null!;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);

                // 1. Mock TaskFrom creation
                var createTaskFromResponsePath = Path.Combine(RecordedResponsesBasePath, "TasksService", "CreateTask", createTaskFromMockFile);
                // This JSON should represent the task object that AddTaskLinkAsync is expected to return
                File.WriteAllText(createTaskFromResponsePath, $"{{\"id\":\"{taskFromId}\", \"name\":\"{taskFromName}\", \"description\":\"Initial From Task\"}}");
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .WithPartialContent(taskFromName)
                               .Respond("application/json", await File.ReadAllTextAsync(createTaskFromResponsePath));

                // 2. Mock TaskTo creation
                var createTaskToResponsePath = Path.Combine(RecordedResponsesBasePath, "TasksService", "CreateTask", createTaskToMockFile);
                File.WriteAllText(createTaskToResponsePath, $"{{\"id\":\"{taskToId}\", \"name\":\"{taskToName}\"}}");
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .WithPartialContent(taskToName)
                               .Respond("application/json", await File.ReadAllTextAsync(createTaskToResponsePath));

                // 3. Mock AddTaskLink call
                // POST /api/v2/task/{task_id}/link/{links_to_task_id} -> returns the source task_id object
                var addTaskLinkResponsePath = Path.Combine(RecordedResponsesBasePath, "TaskRelationshipsService", "POSTAddTaskLink", addTaskLinkResponseMockFile);
                // The response for AddTaskLink should be the 'taskFrom' object, potentially updated with link info.
                // For simplicity, we use the same content as taskFrom's creation for now.
                // A real recording would capture the actual response from ClickUp.
                File.WriteAllText(addTaskLinkResponsePath, $"{{\"id\":\"{taskFromId}\", \"name\":\"{taskFromName}\", \"description\":\"Linked From Task\"}}"); // Potentially updated
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/task/{taskFromId}/link/{taskToId}")
                               .Respond(HttpStatusCode.OK, "application/json", await File.ReadAllTextAsync(addTaskLinkResponsePath));

                // 4. Mock deletions for cleanup
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskFromId}")
                               .Respond(HttpStatusCode.NoContent);
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskToId}")
                               .Respond(HttpStatusCode.NoContent);
            }

            taskFrom = await CreateTestTaskAsync(taskFromName);
            taskTo = await CreateTestTaskAsync(taskToName);

            if (CurrentTestMode == TestMode.Playback) // Ensure IDs are consistent for playback
            {
                taskFrom = new CuTask(taskFromId, null, null, taskFromName, null, "Initial From Task", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                taskTo = new CuTask(taskToId, null, null, taskToName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }


            // Act
            CuTask? resultTask = null;
            Exception? thrownException = null;
            try
            {
                resultTask = await _taskRelationshipsService.AddTaskLinkAsync(taskFrom.Id, taskTo.Id, new Models.RequestModels.TaskRelationships.AddTaskLinkRequest());
                _output.LogInformation($"Successfully linked Task {taskFrom.Id} to Task {taskTo.Id}. Result task ID: {resultTask?.Id}");
            }
            catch (Exception ex)
            {
                thrownException = ex;
                _output.LogError($"Error linking tasks: {ex.Message}", ex);
            }

            // Assert
            Assert.Null(thrownException);
            Assert.NotNull(resultTask);
            Assert.Equal(taskFrom.Id, resultTask.Id); // API returns the source task

            if (CurrentTestMode == TestMode.Playback)
            {
                // Assert against the content of the mocked JSON response
                Assert.Equal("Linked From Task", resultTask.Description);
            }
            else
            {
                // In Record/Passthrough, the description might not change or might be the original one.
                // This depends on the actual API behavior for linking.
                // For now, we only assert ID consistency for non-Playback.
            }
            // Optional: Further assertions on resultTask.linked_tasks if the model supports it and API populates it.
        }

        [Fact]
        public async Task DeleteTaskLinkAsync_ShouldReturnNullAndSucceed()
        {
            // Arrange
            var taskFromName = $"TaskFrom_Unlink_{Guid.NewGuid()}";
            var taskToName = $"TaskTo_Unlink_{Guid.NewGuid()}";

            string taskFromId = "mocked-taskFrom-unlink-id";
            string taskToId = "mocked-taskTo-unlink-id";

            // Mock file names
            string createTaskFromMockFile = $"Success_TaskFrom_Unlink_{taskFromId}.json";
            string createTaskToMockFile = $"Success_TaskTo_Unlink_{taskToId}.json";
            string addTaskLinkResponseMockFile = $"Success_Link_{taskFromId}_to_{taskToId}_ForUnlink.json";
            // For DELETE /task/{task_id}/link/{links_to_task_id}
            // API returns the task, but our service returns null. Mock should reflect API (task obj), service behavior tested by assertion.
            string deleteTaskLinkResponseMockFile = $"Success_Unlink_{taskFromId}_from_{taskToId}.json";

            CuTask taskFrom = null!;
            CuTask taskTo = null!;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);

                // 1. Mock TaskFrom creation
                var createTaskFromResponsePath = Path.Combine(RecordedResponsesBasePath, "TasksService", "CreateTask", createTaskFromMockFile);
                File.WriteAllText(createTaskFromResponsePath, $"{{\"id\":\"{taskFromId}\", \"name\":\"{taskFromName}\"}}");
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .WithPartialContent(taskFromName)
                               .Respond("application/json", await File.ReadAllTextAsync(createTaskFromResponsePath));

                // 2. Mock TaskTo creation
                var createTaskToResponsePath = Path.Combine(RecordedResponsesBasePath, "TasksService", "CreateTask", createTaskToMockFile);
                File.WriteAllText(createTaskToResponsePath, $"{{\"id\":\"{taskToId}\", \"name\":\"{taskToName}\"}}");
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                               .WithPartialContent(taskToName)
                               .Respond("application/json", await File.ReadAllTextAsync(createTaskToResponsePath));

                // 3. Mock AddTaskLink call (prerequisite)
                var addLinkResponsePath = Path.Combine(RecordedResponsesBasePath, "TaskRelationshipsService", "POSTAddTaskLink", addTaskLinkResponseMockFile);
                File.WriteAllText(addLinkResponsePath, $"{{\"id\":\"{taskFromId}\", \"name\":\"{taskFromName}\"}}"); // Returns source task
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/task/{taskFromId}/link/{taskToId}")
                               .Respond(HttpStatusCode.OK, "application/json", await File.ReadAllTextAsync(addLinkResponsePath));

                // 4. Mock DeleteTaskLink call (method under test)
                // DELETE /api/v2/task/{task_id}/link/{links_to_task_id}
                // ClickUp API documentation says this returns the updated task.
                // However, our IApiConnection.DeleteAsync is void, and TaskRelationshipsService.DeleteTaskLinkAsync returns null.
                // The MOCK should represent what the API *actually* returns for the RecordingDelegatingHandler to save it.
                // The ASSERTION will check that our service method returns null.
                var deleteLinkResponsePath = Path.Combine(RecordedResponsesBasePath, "TaskRelationshipsService", "DELETEDeleteTaskLink", deleteTaskLinkResponseMockFile);
                // This JSON should be a valid task object, representing taskFrom after unlinking.
                File.WriteAllText(deleteLinkResponsePath, $"{{\"id\":\"{taskFromId}\", \"name\":\"{taskFromName}\", \"description\":\"Task From After Unlink\"}}");
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskFromId}/link/{taskToId}")
                               .Respond(HttpStatusCode.OK, "application/json", await File.ReadAllTextAsync(deleteLinkResponsePath));

                // 5. Mock deletions for cleanup
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskFromId}")
                               .Respond(HttpStatusCode.NoContent);
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/task/{taskToId}")
                               .Respond(HttpStatusCode.NoContent);
            }

            taskFrom = await CreateTestTaskAsync(taskFromName);
            taskTo = await CreateTestTaskAsync(taskToName);

            if (CurrentTestMode == TestMode.Playback) // Ensure IDs are consistent
            {
                taskFrom = new CuTask(taskFromId, null, null, taskFromName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                taskTo = new CuTask(taskToId, null, null, taskToName, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }

            // Link them first
            await _taskRelationshipsService.AddTaskLinkAsync(taskFrom.Id, taskTo.Id, new Models.RequestModels.TaskRelationships.AddTaskLinkRequest());
            _output.LogInformation($"Setup: Linked Task {taskFrom.Id} to Task {taskTo.Id}.");

            // Act
            CuTask? resultTask = null; // Should remain null
            Exception? thrownException = null;
            try
            {
                // This method in the service currently returns Task.FromResult<CuTask?>(null)
                resultTask = await _taskRelationshipsService.DeleteTaskLinkAsync(taskFrom.Id, taskTo.Id, new Models.RequestModels.TaskRelationships.DeleteTaskLinkRequest());
                _output.LogInformation($"Successfully called DeleteTaskLinkAsync for Task {taskFrom.Id} and Task {taskTo.Id}.");
            }
            catch (Exception ex)
            {
                thrownException = ex;
                _output.LogError($"Error unlinking tasks: {ex.Message}", ex);
            }

            // Assert
            Assert.Null(thrownException);
            // CRITICAL ASSERTION: Verify that the service method returns null, as per its current implementation.
            Assert.Null(resultTask);

            _output.LogInformation("DeleteTaskLinkAsync returned null as expected by current service implementation.");
            // Optional: To truly verify unlinking, one might try to GET taskFrom and check its linked_tasks.
            // This would require additional mocking for GET /task/{taskFromId} in Playback mode,
            // and the response JSON for that GET should show no link to taskToId.
        }
    }
}
