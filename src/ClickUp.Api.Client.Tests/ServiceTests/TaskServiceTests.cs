using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Common; // For Status, Priority
using ClickUp.Api.Client.Models.Entities; // For CustomFieldValue
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.Entities.Checklists; // For Checklist
using ClickUp.Api.Client.Models.Entities.Tags;       // For Tag
using ClickUp.Api.Client.Services;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
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
                Status: new Models.Common.Status("open", "Open", 0, "open"), // Corrected namespace
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
                Priority: new Models.Common.Priority { Id = "1", PriorityValue = "High", Color = "#FF0000", OrderIndex = "1" },
                DueDate: null,
                StartDate: null,
                Points: null,
                TimeEstimate: null,
                TimeSpent: null,
                CustomFields: new List<CustomFieldValue>(), // Corrected namespace
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
            var result = await _taskService.GetTaskAsync(taskId, null, null, null, null, CancellationToken.None);

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

            // Act
            await _taskService.GetTaskAsync(taskId, customTaskIds, teamId, includeSubtasks, includeMarkdownDescription, CancellationToken.None);

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

            // Act
            await _taskService.GetTaskAsync(taskId, customTaskIds, teamId, includeSubtasks, includeMarkdownDescription, CancellationToken.None);

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
                _taskService.GetTaskAsync(taskId, null, null, null, null, CancellationToken.None)
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
                _taskService.GetTaskAsync(taskId, null, null, null, null, CancellationToken.None)
            );
            Assert.Equal(apiException.Message, actualException.Message);
        }
    }
}
