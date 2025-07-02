using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Checklists; // For Checklist
using ClickUp.Api.Client.Models.Entities.CustomFields;
using ClickUp.Api.Client.Models.Entities.Tags;       // For Tag
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.Entities.UserGroups; // Added for UserGroup
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks; // Added for GetTasksResponse
using ClickUp.Api.Client.Services;

using Moq;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class TaskServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TaskService _taskService;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<TaskService>> _mockLogger;

        public TaskServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<TaskService>>();
            _taskService = new TaskService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private CuTask CreateSampleTask(string taskId = "sample-task-id")
        {
            return new CuTask(
                Id: taskId,
                CustomId: "custom-id",
                CustomItemId: 1,
                Name: "Sample Task Name",
                TextContent: "Sample text content",
                Description: "Sample markdown description",
                MarkdownDescription: "Sample markdown description",
                Status: new Status("open", "Open", 0, "open"), // Removed Models.Common prefix
                OrderIndex: "1.0",
                DateCreated: DateTimeOffset.FromUnixTimeMilliseconds(1678886400000L), // Example timestamp
                DateUpdated: DateTimeOffset.FromUnixTimeMilliseconds(1678886400000L),
                DateClosed: null,
                Archived: false,
                Creator: new User(1, "Test User", "test@example.com", "#FFFFFF", "http://example.com/pic.jpg", "TU", null), // Corrected constructor
                Assignees: new List<User>(),
                GroupAssignees: new List<UserGroup>(),
                Watchers: new List<User>(),
                Checklists: new List<Checklist>(),
                Tags: new List<Tag>(),
                Parent: null,
                Priority: new Priority { Id = "1", PriorityValue = "High", Color = "#FF0000", OrderIndex = "1" }, // Removed Models.Common prefix
                DueDate: null,
                StartDate: null,
                Points: null,
                TimeEstimate: null,
                TimeSpent: null,
                CustomFields: new List<CustomFieldValue>(),
                Dependencies: new List<Dependency>(),
                LinkedTasks: new List<TaskLink>(),
                TeamId: "workspace-id",
                Url: $"http://example.com/task/{taskId}",
                Sharing: null,
                PermissionLevel: "read",
                List: new TaskListReference("list-id", "Sample List", true),
                Folder: new TaskFolderReference("folder-id", "Sample Folder", true),
                Space: new TaskSpaceReference("space-id"),
                Project: new TaskFolderReference("project-id", "Sample Project", true) // Assuming project is like folder
            );
        }

        [Fact]
        public async Task GetTaskAsync_ValidTaskId_ReturnsTask()
        {
            // Arrange
            var taskId = "test-task-123";
            var expectedTask = CreateSampleTask(taskId);

            _mockApiConnection
                .Setup(x => x.GetAsync<CuTask>(It.Is<string>(s => s.StartsWith($"task/{taskId}")), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTask);

            // Act
            var result = await _taskService.GetTaskAsync(taskId, new GetTaskRequest(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
            Assert.Equal(expectedTask.Name, result.Name);
            _mockApiConnection.Verify(x => x.GetAsync<CuTask>($"task/{taskId}", CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetTaskAsync_WithAllQueryParameters_BuildsCorrectEndpoint()
        {
            // Arrange
            var taskId = "test-task-456";
            var customTaskIds = true;
            var teamId = "team-abc";
            var includeSubtasks = true;
            var includeMarkdownDescription = true;
            var expectedTask = CreateSampleTask(taskId);

            _mockApiConnection
                .Setup(x => x.GetAsync<CuTask>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTask);

            var expectedEndpoint = $"task/{taskId}?custom_task_ids=true&team_id={teamId}&include_subtasks=true&include_markdown_description=true";
            var requestModel = new GetTaskRequest
            {
                CustomTaskIds = customTaskIds,
                TeamId = teamId,
                IncludeSubtasks = includeSubtasks,
                IncludeMarkdownDescription = includeMarkdownDescription
            };

            // Act
            await _taskService.GetTaskAsync(taskId, requestModel, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<CuTask>(expectedEndpoint, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetTaskAsync_WithSomeQueryParameters_BuildsCorrectEndpoint()
        {
            // Arrange
            var taskId = "test-task-789";
            bool? customTaskIds = null; // Not provided
            var teamId = "team-xyz";
            var includeSubtasks = true;
            bool? includeMarkdownDescription = null; // Not provided
            var expectedTask = CreateSampleTask(taskId);

            _mockApiConnection
                .Setup(x => x.GetAsync<CuTask>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTask);

            var expectedEndpoint = $"task/{taskId}?team_id={teamId}&include_subtasks=true";
            var requestModel = new GetTaskRequest
            {
                CustomTaskIds = customTaskIds, // Will be null, so not added to query
                TeamId = teamId,
                IncludeSubtasks = includeSubtasks,
                IncludeMarkdownDescription = includeMarkdownDescription // Will be null
            };

            // Act
            await _taskService.GetTaskAsync(taskId, requestModel, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<CuTask>(expectedEndpoint, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetTaskAsync_ApiConnectionReturnsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            var taskId = "non-existent-task";
            _mockApiConnection
                .Setup(x => x.GetAsync<CuTask>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CuTask?)null); // Added ? for nullable return

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _taskService.GetTaskAsync(taskId, new GetTaskRequest(), CancellationToken.None)
            );
        }

        [Fact]
        public async Task GetTaskAsync_ApiConnectionThrowsException_PropagatesException()
        {
            // Arrange
            var taskId = "error-task";
            var apiException = new HttpRequestException("API error");
            _mockApiConnection
                .Setup(x => x.GetAsync<CuTask>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(apiException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskService.GetTaskAsync(taskId, new GetTaskRequest(), CancellationToken.None)
            );
            Assert.Equal(apiException.Message, actualException.Message);
        }
        [Fact]
        public async Task GetFilteredTeamTasksAsync_WithNewParameters_BuildsCorrectEndpoint()
        {
            // Arrange
            var workspaceId = "ws-123";
            var dateDoneGreaterThan = 1678886400000;
            var dateDoneLessThan = 1678887400000;
            var parentTaskId = "parent-task-abc";
            var includeMarkdownDescription = true;

            var expectedResponse = new ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTasksResponse(
                new List<CuTask> { CreateSampleTask() },
                false
            );

            _mockApiConnection
                .Setup(x => x.GetAsync<ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTasksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var expectedEndpoint = $"team/{workspaceId}/task?page=0&date_done_gt={dateDoneGreaterThan}&date_done_lt={dateDoneLessThan}&parent={parentTaskId}&include_markdown_description=true";

            // Act
            await _taskService.GetFilteredTeamTasksAsync(
                workspaceId,
                requestModel: new GetFilteredTeamTasksRequest
                {
                    DateDoneGreaterThan = dateDoneGreaterThan,
                    DateDoneLessThan = dateDoneLessThan,
                    ParentTaskId = parentTaskId,
                    IncludeMarkdownDescription = includeMarkdownDescription
                },
                cancellationToken: CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTasksResponse>(expectedEndpoint, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_WithSomeNewAndSomeOldParameters_BuildsCorrectEndpoint()
        {
            // Arrange
            var workspaceId = "ws-456";
            var orderBy = "due_date";
            long? dateDoneGreaterThan = null; // New param not used
            long dateDoneLessThan = 1678889900000; // New param used
            string? parentTaskId = null; // New param not used
            var includeMarkdownDescription = false; // New param used
            var statuses = new List<string> { "open", "in progress" };
            var assignees = new List<string> { "user1", "user2" };

            var expectedResponse = new ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTasksResponse(
                new List<CuTask> { CreateSampleTask() },
                false
            );

            _mockApiConnection
                .Setup(x => x.GetAsync<ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTasksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var expectedEndpoint = $"team/{workspaceId}/task?page=0&order_by={orderBy}&date_done_lt={dateDoneLessThan}&include_markdown_description=false&statuses[]={Uri.EscapeDataString(statuses[0])}&statuses[]={Uri.EscapeDataString(statuses[1])}&assignees[]={assignees[0]}&assignees[]={assignees[1]}";

            // Act
            await _taskService.GetFilteredTeamTasksAsync(
                workspaceId,
                requestModel: new GetFilteredTeamTasksRequest
                {
                    OrderBy = orderBy,
                    DateDoneLessThan = dateDoneLessThan,
                    IncludeMarkdownDescription = includeMarkdownDescription,
                    Statuses = statuses,
                    Assignees = assignees
                },
                cancellationToken: CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTasksResponse>(expectedEndpoint, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_NullWorkspaceId_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>("workspaceId", () =>
                _taskService.GetFilteredTeamTasksAsync(
                    workspaceId: null!,
                    requestModel: new GetFilteredTeamTasksRequest(), // Added requestModel parameter
                    cancellationToken: CancellationToken.None)
            );
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_EmptyWorkspaceId_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>("workspaceId", () =>
                _taskService.GetFilteredTeamTasksAsync(
                    workspaceId: "",
                    requestModel: new GetFilteredTeamTasksRequest(), // Added requestModel parameter
                    cancellationToken: CancellationToken.None)
            );
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_WhitespaceWorkspaceId_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>("workspaceId", () =>
                _taskService.GetFilteredTeamTasksAsync(
                    workspaceId: "   ",
                    requestModel: new GetFilteredTeamTasksRequest(), // Added requestModel parameter
                    cancellationToken: CancellationToken.None)
            );
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws-789";
            _mockApiConnection
                .Setup(x => x.GetAsync<ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTasksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTasksResponse?)null);

            // Act
            var result = await _taskService.GetFilteredTeamTasksAsync(
                workspaceId: workspaceId,
                requestModel: new GetFilteredTeamTasksRequest(),
                cancellationToken: CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.False(result.HasNextPage);
            // Verify the mock was called with page=0
            _mockApiConnection.Verify(x => x.GetAsync<GetTasksResponse>(
                $"team/{workspaceId}/task?page=0", // Ensure this matches the URL built by the service
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- Start of tests for GetFilteredTeamTasksAsyncEnumerableAsync ---

        private static List<CuTask> CreateSimpleTasks(int count, int pageNumForId)
        {
            var tasks = new List<CuTask>();
            for (int i = 0; i < count; i++)
            {
                var taskId = $"task_p{pageNumForId}_{i}";
                tasks.Add(new CuTask(
                    Id: taskId,
                    Name: $"Task {taskId}",
                    CustomId: null, CustomItemId: null, TextContent: null, Description: null, MarkdownDescription: null,
                    Status: null, OrderIndex: null, DateCreated: null, DateUpdated: null, DateClosed: null,
                    Archived: null, Creator: null, Assignees: null, GroupAssignees: null, Watchers: null,
                    Checklists: null, Tags: null, Parent: null, Priority: null, DueDate: null, StartDate: null,
                    Points: null, TimeEstimate: null, TimeSpent: null, CustomFields: null, Dependencies: null,
                    LinkedTasks: null, TeamId: null, Url: null, Sharing: null, PermissionLevel: null, List: null,
                    Folder: null, Space: null, Project: null
                ));
            }
            return tasks;
        }

       [Fact]
        public async Task GetFilteredTeamTasksAsyncEnumerable_ReturnsEmpty_WhenNoTasks()
        {
            // Arrange
            var workspaceId = "ws_empty";
            var requestModel = new GetFilteredTeamTasksRequest(); // Added requestModel initialization
            _mockApiConnection.Setup(api => api.GetAsync<GetTasksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTasksResponse(new List<CuTask>(), true)); // Empty list, is last page

            var count = 0;

            // Act
            await foreach (var _ in _taskService.GetFilteredTeamTasksAsyncEnumerableAsync(workspaceId, requestModel, cancellationToken: CancellationToken.None)) // Added requestModel parameter
            {
                count++;
            }

            // Assert
            Assert.Equal(0, count);
            _mockApiConnection.Verify(api => api.GetAsync<GetTasksResponse>(
                It.Is<string>(s => s.Contains($"team/{workspaceId}/task") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsyncEnumerable_ReturnsAllTasks_WhenSinglePage()
        {
            // Arrange
            var workspaceId = "ws_single_page";
            var requestModel = new GetFilteredTeamTasksRequest(); // Added requestModel initialization
            var tasks = CreateSimpleTasks(2, 0);

            _mockApiConnection.Setup(api => api.GetAsync<GetTasksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTasksResponse(tasks, true)); // Tasks, is last page

            var allTasks = new List<CuTask>();

            // Act
            await foreach (var task in _taskService.GetFilteredTeamTasksAsyncEnumerableAsync(workspaceId, requestModel, cancellationToken: CancellationToken.None)) // Added requestModel parameter
            {
                allTasks.Add(task);
            }

            // Assert
            Assert.Equal(2, allTasks.Count);
            _mockApiConnection.Verify(api => api.GetAsync<GetTasksResponse>(
                It.Is<string>(s => s.Contains($"team/{workspaceId}/task") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- End of tests for GetFilteredTeamTasksAsyncEnumerableAsync ---

        // --- Tests for TaskCanceledException and CancellationToken pass-through for GetTasksAsync ---
        [Fact]
        public async Task GetTasksAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var listId = "list_op_cancel";
            var requestModel = new GetTasksRequest();
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetTasksResponse(new List<CuTask>(), true);

            _mockApiConnection.Setup(c => c.GetAsync<GetTasksResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskService.GetTasksAsync(listId, requestModel, cancellationToken: cancellationTokenSource.Token));
        }

        // --- Tests for TaskCanceledException and CancellationToken pass-through for GetTaskAsync ---
        [Fact]
        public async Task GetTaskAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var taskId = "task_get_op_cancel";
            var requestModel = new GetTaskRequest();
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = CreateSampleTask(taskId);

            _mockApiConnection.Setup(x => x.GetAsync<CuTask>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskService.GetTaskAsync(taskId, requestModel, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetTaskAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var taskId = "error-task-cancel";
            var cts = new CancellationTokenSource();
            cts.Cancel(); // Simulate timeout

            _mockApiConnection
                .Setup(x => x.GetAsync<CuTask>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated API timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskService.GetTaskAsync(taskId, new GetTaskRequest(), cts.Token)
            );
        }

        [Fact]
        public async Task GetTaskAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var taskId = "test-task-ct";
            var expectedTask = CreateSampleTask(taskId);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.GetAsync<CuTask>(It.Is<string>(s => s.StartsWith($"task/{taskId}")), expectedToken))
                .ReturnsAsync(expectedTask);

            // Act
            await _taskService.GetTaskAsync(taskId, new GetTaskRequest(), expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<CuTask>(
                $"task/{taskId}",
                expectedToken), Times.Once);
        }

        // --- Tests for TaskCanceledException and CancellationToken pass-through for CreateTaskAsync ---
        [Fact]
        public async Task CreateTaskAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var listId = "list_create_op_cancel";
            var request = new CreateTaskRequest(
                Name: "Cancel Task",
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
                ListId: null);
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = CreateSampleTask();

            _mockApiConnection.Setup(x => x.PostAsync<CreateTaskRequest, CuTask>(
                    It.IsAny<string>(), It.IsAny<CreateTaskRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, CreateTaskRequest, CancellationToken>((url, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskService.CreateTaskAsync(listId, request, cancellationToken: cancellationTokenSource.Token));
        }

        // --- Tests for TaskCanceledException and CancellationToken pass-through for GetFilteredTeamTasksAsync ---
        [Fact]
        public async Task GetFilteredTeamTasksAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws-cancel-ex";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            var requestModel = new GetFilteredTeamTasksRequest(); // Added requestModel initialization

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTasksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated API timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                    _taskService.GetFilteredTeamTasksAsync(workspaceId, requestModel, cancellationToken: cts.Token) // Added requestModel parameter
            );
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws-ct-pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetTasksResponse(new List<CuTask> { CreateSampleTask() }, false);

            var requestModel = new GetFilteredTeamTasksRequest(); // Added requestModel initialization

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTasksResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            // Act
            await _taskService.GetFilteredTeamTasksAsync(workspaceId, requestModel, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetTasksResponse>(
                $"team/{workspaceId}/task?page=0", // Base endpoint now includes page=0
                expectedToken), Times.Once);
        }

        // --- Tests for TaskCanceledException and CancellationToken pass-through for UpdateTaskAsync ---
        [Fact]
        public async Task UpdateTaskAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var taskId = "task_update_op_cancel";
            var request = new UpdateTaskRequest(
                Name: "Cancel Update",
                Description: null,
                Status: null,
                Priority: null,
                DueDate: null,
                DueDateTime: null,
                Parent: null,
                TimeEstimate: null,
                StartDate: null,
                StartDateTime: null,
                Assignees: null,
                GroupAssignees: null,
                Archived: null,
                CustomFields: null);
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = CreateSampleTask(taskId);

            _mockApiConnection.Setup(x => x.PutAsync<UpdateTaskRequest, CuTask>(
                    It.IsAny<string>(), It.IsAny<UpdateTaskRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, UpdateTaskRequest, CancellationToken>((url, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskService.UpdateTaskAsync(taskId, request, cancellationToken: cancellationTokenSource.Token));
        }
        // NOTE: Existing TaskCanceledException and PassesCancellationTokenToApiConnection tests for UpdateTaskAsync are missing, should be added if not present elsewhere.

        // --- Tests for TaskCanceledException and CancellationToken pass-through for DeleteTaskAsync ---
        [Fact]
        public async Task DeleteTaskAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var taskId = "task_delete_op_cancel";
            var requestModel = new DeleteTaskRequest();
            var cancellationTokenSource = new CancellationTokenSource();

            _mockApiConnection.Setup(x => x.DeleteAsync(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .Returns(Task.CompletedTask);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskService.DeleteTaskAsync(taskId, requestModel, cancellationTokenSource.Token));
        }
        // NOTE: Existing TaskCanceledException and PassesCancellationTokenToApiConnection tests for DeleteTaskAsync are missing.

        // --- Tests for TaskCanceledException and CancellationToken pass-through for MergeTasksAsync ---
        [Fact]
        public async Task MergeTasksAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var targetTaskId = "target_merge_op_cancel";
            var sourceTaskId = "source_merge_op_cancel";
            var request = new MergeTasksRequest { SourceTaskIds = new List<string> { sourceTaskId } };
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = CreateSampleTask(targetTaskId);

            _mockApiConnection.Setup(x => x.PostAsync<object, CuTask>( // Assuming payload is object for merge
                    It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Callback<string, object, CancellationToken>((url, body, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskService.MergeTasksAsync(targetTaskId, request, cancellationTokenSource.Token));
        }
        // NOTE: Existing TaskCanceledException and PassesCancellationTokenToApiConnection tests for MergeTasksAsync are missing.

        // --- Tests for TaskCanceledException and CancellationToken pass-through for GetTaskTimeInStatusAsync ---
        [Fact]
        public async Task GetTaskTimeInStatusAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var taskId = "task_time_status_op_cancel";
            var requestModel = new GetTaskTimeInStatusRequest();
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyTimeData = new TaskTimeInStatusData { ByMinute = null, Since = null };
            var dummyStatusHistoryItem = new StatusHistoryItem { Status = "open", TotalTime = dummyTimeData };
            var dummyResponse = new TaskTimeInStatusResponse
            {
                TotalTime = dummyTimeData,
                StatusHistory = new List<StatusHistoryItem> { dummyStatusHistoryItem },
                CurrentStatus = dummyStatusHistoryItem
            };

            _mockApiConnection.Setup(x => x.GetAsync<TaskTimeInStatusResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskService.GetTaskTimeInStatusAsync(taskId, requestModel, cancellationTokenSource.Token));
        }
        // NOTE: Existing TaskCanceledException and PassesCancellationTokenToApiConnection tests for GetTaskTimeInStatusAsync are missing.

        // --- Tests for TaskCanceledException and CancellationToken pass-through for GetBulkTasksTimeInStatusAsync ---
        [Fact]
        public async Task GetBulkTasksTimeInStatusAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var requestModel = new GetBulkTasksTimeInStatusRequest(new List<string> { "task1_bulk_op_cancel" });
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetBulkTasksTimeInStatusResponse(); // Initialize empty

            _mockApiConnection.Setup(x => x.GetAsync<GetBulkTasksTimeInStatusResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskService.GetBulkTasksTimeInStatusAsync(requestModel, cancellationTokenSource.Token));
        }
        // NOTE: Existing TaskCanceledException and PassesCancellationTokenToApiConnection tests for GetBulkTasksTimeInStatusAsync are missing.

        // --- Tests for TaskCanceledException and CancellationToken pass-through for CreateTaskFromTemplateAsync ---
        [Fact]
        public async Task CreateTaskFromTemplateAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var listId = "list_tpl_op_cancel";
            var templateId = "tpl_op_cancel";
            var request = new CreateTaskFromTemplateRequest(Name: "Cancel Template Task");
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = CreateSampleTask();

            _mockApiConnection.Setup(x => x.PostAsync<CreateTaskFromTemplateRequest, CuTask>(
                    It.IsAny<string>(), It.IsAny<CreateTaskFromTemplateRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, CreateTaskFromTemplateRequest, CancellationToken>((url, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskService.CreateTaskFromTemplateAsync(listId, templateId, request, cancellationTokenSource.Token));
        }
        // NOTE: Existing TaskCanceledException and PassesCancellationTokenToApiConnection tests for CreateTaskFromTemplateAsync are missing.
    }
}
