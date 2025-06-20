using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities; // For CuTask DTO (fully qualified if needed)
using ClickUp.Api.Client.Models.RequestModels.Tasks; // For CreateTaskRequest etc.
using ClickUp.Api.Client.Models.ResponseModels.Tasks; // For GetTasksResponse etc.
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic; // For IEnumerable etc.
using System; // For ArgumentNullException, Uri, etc.
using System.Linq; // For Any()

namespace ClickUp.Api.Client.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _taskService = new TaskService(_mockApiConnection.Object);
        }

        [Fact]
        public async Task GetTaskAsync_WhenTaskExists_ReturnsTask()
        {
            // Arrange
            var taskId = "test-task-id";
            var expectedTask = new Models.Entities.Task(taskId, null, "Test CuTask", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            _mockApiConnection.Setup(c => c.GetAsync<Models.Entities.Task>(
                $"task/{taskId}", // Basic endpoint, no query params in this simple test
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTask);

            // Act
            var result = await _taskService.GetTaskAsync(taskId, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTask);
            _mockApiConnection.Verify(c => c.GetAsync<Models.Entities.Task>(
                $"task/{taskId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTaskAsync_WithOptionalParams_ConstructsCorrectEndpointAndReturnsTask()
        {
            // Arrange
            var taskId = "test-task-id-opts";
            var teamIdForQuery = "test-team-id";
            var expectedTask = new Models.Entities.Task(taskId, null, "Test CuTask With Options", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            // Expected endpoint with all params true/set
            var expectedEndpoint = $"task/{taskId}?custom_task_ids=true&team_id={teamIdForQuery}&include_subtasks=true&include_markdown_description=true";

            _mockApiConnection.Setup(c => c.GetAsync<Models.Entities.Task>(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTask);

            // Act
            var result = await _taskService.GetTaskAsync(
                taskId,
                customTaskIds: true,
                teamId: teamIdForQuery,
                includeSubtasks: true,
                includeMarkdownDescription: true,
                cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTask);
            _mockApiConnection.Verify(c => c.GetAsync<Models.Entities.Task>(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task GetTaskAsync_WhenApiConnectionReturnsNull_ReturnsNull()
        {
            // Arrange
            var taskId = "another-task-id";
            _mockApiConnection.Setup(c => c.GetAsync<Models.Entities.Task>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Models.Entities.Task?)null);

            // Act
            var result = await _taskService.GetTaskAsync(taskId, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateTaskAsync_ValidRequest_ReturnsCreatedTask()
        {
            // Arrange
            var listId = "test-list-id";
            var requestDto = new CreateTaskRequest("New Test CuTask");
            var expectedCreatedTask = new Models.Entities.Task("new-task-id", null, "New Test CuTask", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            _mockApiConnection.Setup(c => c.PostAsync<CreateTaskRequest, Models.Entities.Task>(
                $"list/{listId}/task", // Basic endpoint, no query params in this simple test
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCreatedTask);

            // Act
            var result = await _taskService.CreateTaskAsync(listId, requestDto, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedCreatedTask);
            _mockApiConnection.Verify(c => c.PostAsync<CreateTaskRequest, Models.Entities.Task>(
                $"list/{listId}/task",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateTaskAsync_WithOptionalParams_ConstructsCorrectEndpointAndReturnsTask()
        {
            // Arrange
            var listId = "test-list-id-opts";
            var teamIdForQuery = "test-team-id";
            var requestDto = new CreateTaskRequest("New Test CuTask QParams");
            var expectedCreatedTask = new Models.Entities.Task("new-task-id-qparams", null, "New Test CuTask QParams", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            var expectedEndpoint = $"list/{listId}/task?custom_task_ids=true&team_id={teamIdForQuery}";

            _mockApiConnection.Setup(c => c.PostAsync<CreateTaskRequest, Models.Entities.Task>(
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCreatedTask);

            // Act
            var result = await _taskService.CreateTaskAsync(listId, requestDto, customTaskIds: true, teamId: teamIdForQuery, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedCreatedTask);
            _mockApiConnection.Verify(c => c.PostAsync<CreateTaskRequest, Models.Entities.Task>(
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- UpdateTaskAsync Tests ---
        [Fact]
        public async Task UpdateTaskAsync_ValidRequest_ReturnsUpdatedTask()
        {
            // Arrange
            var taskId = "task-to-update";
            var requestDto = new UpdateTaskRequest { Name = "Updated CuTask Name" }; // Assuming UpdateTaskRequest has a Name property
            var expectedUpdatedTask = new Models.Entities.Task(taskId, null, "Updated CuTask Name", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            _mockApiConnection.Setup(c => c.PutAsync<UpdateTaskRequest, Models.Entities.Task>(
                $"task/{taskId}", // Basic endpoint
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUpdatedTask);

            // Act
            var result = await _taskService.UpdateTaskAsync(taskId, requestDto, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUpdatedTask);
            _mockApiConnection.Verify(c => c.PutAsync<UpdateTaskRequest, Models.Entities.Task>(
                $"task/{taskId}",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAsync_WithOptionalParams_ConstructsCorrectEndpoint()
        {
            // Arrange
            var taskId = "task-to-update-opts";
            var teamIdForQuery = "test-team-id";
            var requestDto = new UpdateTaskRequest { Name = "Updated CuTask Name Opts" };
            var expectedUpdatedTask = new Models.Entities.Task(taskId, null, "Updated CuTask Name Opts", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            var expectedEndpoint = $"task/{taskId}?custom_task_ids=true&team_id={teamIdForQuery}";

            _mockApiConnection.Setup(c => c.PutAsync<UpdateTaskRequest, Models.Entities.Task>(
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUpdatedTask);

            // Act
            await _taskService.UpdateTaskAsync(taskId, requestDto, customTaskIds: true, teamId: teamIdForQuery, cancellationToken: CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.PutAsync<UpdateTaskRequest, Models.Entities.Task>(
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- DeleteTaskAsync Tests ---
        [Fact]
        public async Task DeleteTaskAsync_ValidTaskId_CallsApiConnectionDeleteAsync()
        {
            // Arrange
            var taskId = "task-to-delete";
            var expectedEndpoint = $"task/{taskId}";
            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskService.DeleteTaskAsync(taskId, cancellationToken: CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_WithOptionalParams_ConstructsCorrectEndpoint()
        {
            // Arrange
            var taskId = "task-to-delete-opts";
            var teamIdForQuery = "test-team-id";
            var expectedEndpoint = $"task/{taskId}?custom_task_ids=true&team_id={teamIdForQuery}";
            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskService.DeleteTaskAsync(taskId, customTaskIds: true, teamId: teamIdForQuery, cancellationToken: CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- GetTasksAsyncEnumerableAsync Tests ---
        [Fact]
        public async Task GetTasksAsyncEnumerableAsync_WhenApiReturnsMultiplePages_YieldsAllTasks()
        {
            // Arrange
            var listId = "list-id-for-enumerable";
            var task1 = new Models.Entities.Task("task1", null, "CuTask 1", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            var task2 = new Models.Entities.Task("task2", null, "CuTask 2", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            var task3 = new Models.Entities.Task("task3", null, "CuTask 3", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            var page0Response = new GetTasksResponse(new List<Models.Entities.Task> { task1, task2 }, LastPage: false);
            var page1Response = new GetTasksResponse(new List<Models.Entities.Task> { task3 }, LastPage: true);

            // Mocking the underlying _apiConnection.GetAsync calls that GetTasksAsync (paged version) makes.
            _mockApiConnection.SetupSequence(c => c.GetAsync<GetTasksResponse>(It.Is<string>(s => s.Contains($"list/{listId}/task") && s.Contains("page=0")), It.IsAny<CancellationToken>()))
                .ReturnsAsync(page0Response);
            _mockApiConnection.SetupSequence(c => c.GetAsync<GetTasksResponse>(It.Is<string>(s => s.Contains($"list/{listId}/task") && s.Contains("page=1")), It.IsAny<CancellationToken>()))
                .ReturnsAsync(page1Response);

            var collectedTasks = new List<Models.Entities.Task>();

            // Act
            await foreach (var task in _taskService.GetTasksAsyncEnumerableAsync(listId, cancellationToken: CancellationToken.None))
            {
                collectedTasks.Add(task);
            }

            // Assert
            collectedTasks.Should().HaveCount(3);
            collectedTasks.Should().ContainEquivalentOf(task1);
            collectedTasks.Should().ContainEquivalentOf(task2);
            collectedTasks.Should().ContainEquivalentOf(task3);
            _mockApiConnection.Verify(c => c.GetAsync<GetTasksResponse>(It.Is<string>(s => s.Contains($"list/{listId}/task") && s.Contains("page=0")), It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(c => c.GetAsync<GetTasksResponse>(It.Is<string>(s => s.Contains($"list/{listId}/task") && s.Contains("page=1")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTasksAsyncEnumerableAsync_WhenApiReturnsEmptyFirstPage_YieldsNoTasks()
        {
            // Arrange
            var listId = "empty-list-id";
            var emptyResponse = new GetTasksResponse(new List<Models.Entities.Task>(), LastPage: true);

            _mockApiConnection.Setup(c => c.GetAsync<GetTasksResponse>(It.Is<string>(s => s.Contains($"list/{listId}/task") && s.Contains("page=0")), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyResponse);

            var collectedTasks = new List<Models.Entities.Task>();

            // Act
            await foreach (var task in _taskService.GetTasksAsyncEnumerableAsync(listId, cancellationToken: CancellationToken.None))
            {
                collectedTasks.Add(task);
            }

            // Assert
            collectedTasks.Should().BeEmpty();
            _mockApiConnection.Verify(c => c.GetAsync<GetTasksResponse>(It.Is<string>(s => s.Contains($"list/{listId}/task") && s.Contains("page=0")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTasksAsyncEnumerableAsync_RespectsCancellationToken()
        {
            // Arrange
            var listId = "cancellable-list-id";
            var task1 = new Models.Entities.Task("task1_cancel", null, "CuTask 1 Cancel", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            var page0Response = new GetTasksResponse(new List<Models.Entities.Task> { task1 }, LastPage: false); // More pages available

            _mockApiConnection.Setup(c => c.GetAsync<GetTasksResponse>(It.Is<string>(s => s.Contains($"list/{listId}/task") && s.Contains("page=0")), It.IsAny<CancellationToken>()))
                .ReturnsAsync(page0Response);

            var cts = new CancellationTokenSource();
            var collectedTasks = new List<Models.Entities.Task>();

            // Act
            // We will cancel after processing the first item (or attempting to get the first page)
            // For simplicity, we pass an already cancelled token to the second effective call.
            // More robust: yield break inside the loop if token is cancelled.

            // This test will verify that the CancellationToken is passed to the underlying IApiConnection.
            // The actual cancellation behavior within the async iterator is complex to assert directly without more control.
            await foreach (var task in _taskService.GetTasksAsyncEnumerableAsync(listId, cancellationToken: cts.Token))
            {
                collectedTasks.Add(task);
                cts.Cancel(); // Cancel after the first item is yielded
            }

            // Assert
            collectedTasks.Should().HaveCount(1); // Only tasks from the first page before cancellation
            _mockApiConnection.Verify(c => c.GetAsync<GetTasksResponse>(
                It.IsAny<string>(),
                cts.Token // Verify that the specific token was passed
            ), Times.AtLeastOnce());
            // We expect it to be called for page 0. It might not be called for page 1 if cancellation is quick.
        }
    }
}
