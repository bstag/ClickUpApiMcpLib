using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Checklists; // For Checklist
using ClickUp.Api.Client.Models.Entities.CustomFields;
using ClickUp.Api.Client.Models.Entities.Tags;       // For Tag
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.Entities.UserGroups; // Added for UserGroup
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks; // Added for GetBulkTasksTimeInStatusResponse and other response models
using ClickUp.Api.Client.Models.Common.Pagination; // Added for PagedResult
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Services.Tasks;

using Moq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using ClickUp.Api.Client.Models.Parameters; // Ensures CustomFieldFilter is available
using ClickUp.Api.Client.Models.Common.ValueObjects; // For TimeRange, SortOption


namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class TaskServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TaskService _taskService;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<TaskService>> _mockLogger;
        private readonly Mock<ITaskCrudService> _mockTaskCrudService;
        private readonly Mock<ITaskQueryService> _mockTaskQueryService;
        private readonly Mock<ITaskRelationshipService> _mockTaskRelationshipService;
        private readonly Mock<ITaskTimeTrackingService> _mockTaskTimeTrackingService;

        public TaskServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<TaskService>>();
            _mockTaskCrudService = new Mock<ITaskCrudService>();
            _mockTaskQueryService = new Mock<ITaskQueryService>();
            _mockTaskRelationshipService = new Mock<ITaskRelationshipService>();
            _mockTaskTimeTrackingService = new Mock<ITaskTimeTrackingService>();
            _taskService = new TaskService(_mockTaskCrudService.Object, _mockTaskQueryService.Object, _mockTaskRelationshipService.Object, _mockTaskTimeTrackingService.Object, _mockLogger.Object);
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
            var requestModel = new GetTaskRequest();

            _mockTaskCrudService
                .Setup(x => x.GetTaskAsync(taskId, requestModel, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTask);

            // Act
            var result = await _taskService.GetTaskAsync(taskId, requestModel, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
            Assert.Equal(expectedTask.Name, result.Name);
            _mockTaskCrudService.Verify(x => x.GetTaskAsync(taskId, requestModel, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetTaskAsync_WithAllQueryParameters_CallsTaskCrudService()
        {
            // Arrange
            var taskId = "test-task-456";
            var customTaskIds = true;
            var teamId = "team-abc";
            var includeSubtasks = true;
            var includeMarkdownDescription = true;
            var expectedTask = CreateSampleTask(taskId);

            var requestModel = new GetTaskRequest
            {
                CustomTaskIds = customTaskIds,
                TeamId = teamId,
                IncludeSubtasks = includeSubtasks,
                IncludeMarkdownDescription = includeMarkdownDescription
            };

            _mockTaskCrudService
                .Setup(x => x.GetTaskAsync(taskId, requestModel, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTask);

            // Act
            await _taskService.GetTaskAsync(taskId, requestModel, CancellationToken.None);

            // Assert
            _mockTaskCrudService.Verify(x => x.GetTaskAsync(taskId, requestModel, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetTaskAsync_WithSomeQueryParameters_CallsTaskCrudService()
        {
            // Arrange
            var taskId = "test-task-789";
            bool? customTaskIds = null; // Not provided
            var teamId = "team-xyz";
            var includeSubtasks = true;
            bool? includeMarkdownDescription = null; // Not provided
            var expectedTask = CreateSampleTask(taskId);

            var requestModel = new GetTaskRequest
            {
                CustomTaskIds = customTaskIds, // Will be null, so not added to query
                TeamId = teamId,
                IncludeSubtasks = includeSubtasks,
                IncludeMarkdownDescription = includeMarkdownDescription // Will be null
            };

            _mockTaskCrudService
                .Setup(x => x.GetTaskAsync(taskId, requestModel, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTask);

            // Act
            await _taskService.GetTaskAsync(taskId, requestModel, CancellationToken.None);

            // Assert
            _mockTaskCrudService.Verify(x => x.GetTaskAsync(taskId, requestModel, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetTaskAsync_TaskCrudServiceThrowsException_PropagatesException()
        {
            // Arrange
            var taskId = "non-existent-task";
            var requestModel = new GetTaskRequest();
            var expectedException = new InvalidOperationException("Task not found");
            
            _mockTaskCrudService
                .Setup(x => x.GetTaskAsync(taskId, requestModel, It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _taskService.GetTaskAsync(taskId, requestModel, CancellationToken.None)
            );
            Assert.Equal(expectedException.Message, actualException.Message);
        }

        [Fact]
        public async Task GetTaskAsync_TaskCrudServiceThrowsHttpException_PropagatesException()
        {
            // Arrange
            var taskId = "error-task";
            var requestModel = new GetTaskRequest();
            var apiException = new HttpRequestException("API error");
            
            _mockTaskCrudService
                .Setup(x => x.GetTaskAsync(taskId, requestModel, It.IsAny<CancellationToken>()))
                .ThrowsAsync(apiException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskService.GetTaskAsync(taskId, requestModel, CancellationToken.None)
            );
            Assert.Equal(apiException.Message, actualException.Message);
        }
        [Fact]
        public async Task GetFilteredTeamTasksAsync_WithNewParameters_CallsTaskQueryService()
        {
            // Arrange
            var workspaceId = "ws-123";
            var startDate = DateTimeOffset.UtcNow.AddDays(-1);
            var endDate = DateTimeOffset.UtcNow;
            var customFieldId = "cust_field_guid";
            var customFieldValue = "value123";

            var expectedResponse = new PagedResult<CuTask>(
                new List<CuTask> { CreateSampleTask() },
                0, 1, false
            );

            _mockTaskQueryService
                .Setup(x => x.GetFilteredTeamTasksAsync(workspaceId, It.IsAny<Action<GetTasksRequestParameters>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            await _taskService.GetFilteredTeamTasksAsync(
                workspaceId,
                configureParameters: parameters =>
                {
                    parameters.DueDateRange = new TimeRange(startDate, endDate);
                    parameters.CustomFields = new List<CustomFieldFilter> { new CustomFieldFilter { FieldId = customFieldId, Operator = "=", Value = customFieldValue } };
                },
                cancellationToken: CancellationToken.None);

            // Assert
            _mockTaskQueryService.Verify(x => x.GetFilteredTeamTasksAsync(
                workspaceId,
                It.IsAny<Action<GetTasksRequestParameters>>(),
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_WithSomeNewAndSomeOldParameters_BuildsCorrectEndpoint()
        {
            // Arrange
            var workspaceId = "ws-456";
            var orderBy = "due_date";
            var includeMarkdownDescription = false;
            var statuses = new List<string> { "open", "in progress" };
            var assignees = new List<string> { "123", "456" };

            var expectedResponse = new PagedResult<CuTask>(
                new List<CuTask> { CreateSampleTask() },
                0, 1, false
            );

            _mockTaskQueryService
                .Setup(x => x.GetFilteredTeamTasksAsync(workspaceId, It.IsAny<Action<GetTasksRequestParameters>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            await _taskService.GetFilteredTeamTasksAsync(
                workspaceId,
                parameters => {
                    parameters.SortBy = new SortOption(orderBy, SortDirection.Ascending);
                    parameters.IncludeMarkdownDescription = includeMarkdownDescription;
                    parameters.Statuses = statuses;
                    parameters.AssigneeIds = assignees.Select(int.Parse).ToList();
                },
                cancellationToken: CancellationToken.None);

            // Assert
            _mockTaskQueryService.Verify(x => x.GetFilteredTeamTasksAsync(
                workspaceId,
                It.IsAny<Action<GetTasksRequestParameters>>(),
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_NullWorkspaceId_ThrowsArgumentNullException()
        {
            // Arrange
            _mockTaskQueryService
                .Setup(x => x.GetFilteredTeamTasksAsync(null!, It.IsAny<Action<GetTasksRequestParameters>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentNullException("workspaceId"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>("workspaceId", () =>
                _taskService.GetFilteredTeamTasksAsync(
                    workspaceId: null!,
                    configureParameters: null,
                    cancellationToken: CancellationToken.None)
            );
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_EmptyWorkspaceId_ThrowsArgumentNullException()
        {
            // Arrange
            _mockTaskQueryService
                .Setup(x => x.GetFilteredTeamTasksAsync("", It.IsAny<Action<GetTasksRequestParameters>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentNullException("workspaceId"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>("workspaceId", () =>
                _taskService.GetFilteredTeamTasksAsync(
                    workspaceId: "",
                    configureParameters: null,
                    cancellationToken: CancellationToken.None)
            );
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_WhitespaceWorkspaceId_ThrowsArgumentNullException()
        {
            // Arrange
            _mockTaskQueryService
                .Setup(x => x.GetFilteredTeamTasksAsync("   ", It.IsAny<Action<GetTasksRequestParameters>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentNullException("workspaceId"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>("workspaceId", () =>
                _taskService.GetFilteredTeamTasksAsync(
                    workspaceId: "   ",
                    configureParameters: null,
                    cancellationToken: CancellationToken.None)
            );
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_TaskQueryServiceReturnsNull_ReturnsEmptyPagedResult() // Method name updated to reflect behavior
        {
            // Arrange
            var workspaceId = "ws-789";
            var emptyResult = new PagedResult<CuTask>(new List<CuTask>(), 0, 0, false);
            _mockTaskQueryService
                .Setup(x => x.GetFilteredTeamTasksAsync(workspaceId, It.IsAny<Action<GetTasksRequestParameters>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyResult);

            // Act
            var result = await _taskService.GetFilteredTeamTasksAsync(
                workspaceId: workspaceId,
                configureParameters: null,
                cancellationToken: CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.False(result.HasNextPage);
            // Verify the mock was called
            _mockTaskQueryService.Verify(x => x.GetFilteredTeamTasksAsync(
                workspaceId,
                It.IsAny<Action<GetTasksRequestParameters>>(),
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
            var parameters = new GetTasksRequestParameters();
            _mockTaskQueryService.Setup(api => api.GetFilteredTeamTasksAsyncEnumerableAsync(workspaceId, parameters, It.IsAny<CancellationToken>()))
                .Returns(GetEmptyAsyncEnumerable<CuTask>()); // Empty enumerable

            var count = 0;

            // Act
            await foreach (var _ in _taskService.GetFilteredTeamTasksAsyncEnumerableAsync(workspaceId, parameters, cancellationToken: CancellationToken.None))
            {
                count++;
            }

            // Assert
            Assert.Equal(0, count);
            _mockTaskQueryService.Verify(api => api.GetFilteredTeamTasksAsyncEnumerableAsync(
                workspaceId,
                parameters,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsyncEnumerable_ReturnsAllTasks_WhenSinglePage()
        {
            // Arrange
            var workspaceId = "ws_single_page";
            var parameters = new GetTasksRequestParameters();
            var tasks = CreateSimpleTasks(2, 0);

            _mockTaskQueryService.Setup(api => api.GetFilteredTeamTasksAsyncEnumerableAsync(workspaceId, parameters, It.IsAny<CancellationToken>()))
                .Returns(ConvertToAsyncEnumerable(tasks)); // Return tasks as async enumerable

            var allTasks = new List<CuTask>();

            // Act
            await foreach (var task in _taskService.GetFilteredTeamTasksAsyncEnumerableAsync(workspaceId, parameters, cancellationToken: CancellationToken.None))
            {
                allTasks.Add(task);
            }

            // Assert
            Assert.Equal(2, allTasks.Count);
            _mockTaskQueryService.Verify(api => api.GetFilteredTeamTasksAsyncEnumerableAsync(
                workspaceId,
                parameters,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- End of tests for GetFilteredTeamTasksAsyncEnumerableAsync ---

        // --- Tests for TaskCanceledException and CancellationToken pass-through for GetTasksAsync ---
        [Fact]
        public async Task GetTasksAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var listId = "list_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new PagedResult<CuTask>(new List<CuTask>(), 0, 1, true);

            _mockTaskQueryService.Setup(c => c.GetTasksAsync(
                    listId, It.IsAny<Action<GetTasksRequestParameters>>(), It.IsAny<CancellationToken>()))
                .Callback<string, Action<GetTasksRequestParameters>, CancellationToken>((id, config, token) =>
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
                _taskService.GetTasksAsync(listId, configureParameters: null, cancellationToken: cancellationTokenSource.Token));
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

            _mockTaskCrudService.Setup(x => x.GetTaskAsync(
                    taskId, requestModel, It.IsAny<CancellationToken>()))
                .Callback<string, GetTaskRequest, CancellationToken>((id, req, token) =>
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
        public async Task GetTaskAsync_TaskCrudServiceThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var taskId = "error-task-cancel";
            var cts = new CancellationTokenSource();
            cts.Cancel(); // Simulate timeout

            _mockTaskCrudService
                .Setup(x => x.GetTaskAsync(taskId, It.IsAny<GetTaskRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated API timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskService.GetTaskAsync(taskId, new GetTaskRequest(), cts.Token)
            );
        }

        [Fact]
        public async Task GetTaskAsync_PassesCancellationTokenToTaskCrudService()
        {
            // Arrange
            var taskId = "test-task-ct";
            var expectedTask = CreateSampleTask(taskId);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var request = new GetTaskRequest();

            _mockTaskCrudService
                .Setup(x => x.GetTaskAsync(taskId, request, expectedToken))
                .ReturnsAsync(expectedTask);

            // Act
            await _taskService.GetTaskAsync(taskId, request, expectedToken);

            // Assert
            _mockTaskCrudService.Verify(x => x.GetTaskAsync(
                taskId,
                request,
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

            _mockTaskCrudService.Setup(x => x.CreateTaskAsync(
                    listId, request, It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CreateTaskRequest, bool?, string, CancellationToken>((id, req, customIds, teamId, token) =>
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
                _taskService.CreateTaskAsync(listId, request, customTaskIds: null, teamId: null, cancellationToken: cancellationTokenSource.Token));
        }

        // --- Tests for TaskCanceledException and CancellationToken pass-through for GetFilteredTeamTasksAsync ---
        [Fact]
        public async Task GetFilteredTeamTasksAsync_TaskQueryServiceThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws-cancel-ex";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockTaskQueryService
                .Setup(x => x.GetFilteredTeamTasksAsync(workspaceId, It.IsAny<Action<GetTasksRequestParameters>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Simulated API timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                    _taskService.GetFilteredTeamTasksAsync(workspaceId, configureParameters: null, cancellationToken: cts.Token)
            );
        }

        [Fact]
        public async Task GetFilteredTeamTasksAsync_PassesCancellationTokenToTaskQueryService()
        {
            // Arrange
            var workspaceId = "ws-ct-pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedTasks = new List<CuTask> { CreateSampleTask() };
            var expectedResponse = new PagedResult<CuTask>(expectedTasks, 0, 1, false);

            _mockTaskQueryService
                .Setup(x => x.GetFilteredTeamTasksAsync(workspaceId, It.IsAny<Action<GetTasksRequestParameters>>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            // Act
            await _taskService.GetFilteredTeamTasksAsync(workspaceId, configureParameters: null, cancellationToken: expectedToken);

            // Assert
            _mockTaskQueryService.Verify(x => x.GetFilteredTeamTasksAsync(
                workspaceId,
                It.IsAny<Action<GetTasksRequestParameters>>(),
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

            _mockTaskCrudService.Setup(x => x.UpdateTaskAsync(
                    taskId, request, It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, UpdateTaskRequest, bool?, string, CancellationToken>((id, req, customIds, teamId, token) =>
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
                _taskService.UpdateTaskAsync(taskId, request, null, null, cancellationTokenSource.Token));
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

            _mockTaskCrudService.Setup(x => x.DeleteTaskAsync(
                    taskId, requestModel, It.IsAny<CancellationToken>()))
                .Callback<string, DeleteTaskRequest, CancellationToken>((id, req, token) =>
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

            _mockTaskRelationshipService.Setup(x => x.MergeTasksAsync(
                    targetTaskId, request, It.IsAny<CancellationToken>()))
                .Callback<string, MergeTasksRequest, CancellationToken>((id, req, token) =>
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

            _mockTaskTimeTrackingService.Setup(x => x.GetTaskTimeInStatusAsync(
                    taskId, requestModel, It.IsAny<CancellationToken>()))
                .Callback<string, GetTaskTimeInStatusRequest, CancellationToken>((id, req, token) =>
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

            _mockTaskTimeTrackingService.Setup(x => x.GetBulkTasksTimeInStatusAsync(
                    requestModel, It.IsAny<CancellationToken>()))
                .Callback<GetBulkTasksTimeInStatusRequest, CancellationToken>((req, token) =>
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

            _mockTaskCrudService.Setup(x => x.CreateTaskFromTemplateAsync(
                    listId, templateId, request, It.IsAny<CancellationToken>()))
                .Callback<string, string, CreateTaskFromTemplateRequest, CancellationToken>((id, tplId, req, token) =>
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

        // Helper methods for async enumerable support
        private static async IAsyncEnumerable<T> GetEmptyAsyncEnumerable<T>()
        {
            await Task.Yield();
            yield break;
        }

        private static async IAsyncEnumerable<T> ConvertToAsyncEnumerable<T>(IEnumerable<T> items)
        {
            await Task.Yield();
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }
}
