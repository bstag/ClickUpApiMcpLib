using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class TaskFluentValidationTests
{
    // --- TaskFluentCreateRequest Tests ---

    [Fact]
    public void Create_Validate_MissingListId_ThrowsException()
    {
        var tasksServiceMock = new Mock<ITasksService>();
        var request = new TaskFluentCreateRequest(string.Empty, tasksServiceMock.Object).WithName("Test Task"); // Changed null to string.Empty
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("ListId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_MissingName_ThrowsException()
    {
        var tasksServiceMock = new Mock<ITasksService>();
        var request = new TaskFluentCreateRequest("list123", tasksServiceMock.Object);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Task name is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_ValidRequest_DoesNotThrow()
    {
        var tasksServiceMock = new Mock<ITasksService>();
        var request = new TaskFluentCreateRequest("list123", tasksServiceMock.Object).WithName("Test Task");
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsException()
    {
        var tasksServiceMock = new Mock<ITasksService>();
        var request = new TaskFluentCreateRequest(string.Empty, tasksServiceMock.Object); // Invalid, changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync());
        Assert.Contains("ListId is required.", ex.ValidationErrors);
        Assert.Contains("Task name is required.", ex.ValidationErrors);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CallsService()
    {
        var tasksServiceMock = new Mock<ITasksService>();
        tasksServiceMock
            .Setup(s => s.CreateTaskAsync(It.IsAny<string>(), It.IsAny<CreateTaskRequest>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new CuTask(
                Id: "task123",
                Name: "Test Task",
                Status: new ClickUp.Api.Client.Models.Common.Status("open", "blue", 0, "open"), // Corrected namespace and added params
                Creator: new ClickUp.Api.Client.Models.Entities.Users.User(1, "test", "test@example.com", null,null,null), // Corrected namespace and added params
                DateCreated: System.DateTimeOffset.UtcNow,
                // Providing null for other nullable properties to satisfy a minimal constructor if any
                CustomId: null,
                CustomItemId: null,
                TextContent: null,
                Description: null,
                MarkdownDescription: null,
                OrderIndex: null,
                DateUpdated: null,
                DateClosed: null,
                Archived: null,
                Assignees: null,
                GroupAssignees: null,
                Watchers: null,
                Checklists: null,
                Tags: null,
                Parent: null,
                Priority: null,
                DueDate: null,
                StartDate: null,
                Points: null,
                TimeEstimate: null,
                TimeSpent: null,
                CustomFields: null,
                Dependencies: null,
                LinkedTasks: null,
                TeamId: null,
                Url: null,
                Sharing: null,
                PermissionLevel: null,
                List: null,
                Folder: null,
                Space: null,
                Project: null
            ));

        var request = new TaskFluentCreateRequest("list123", tasksServiceMock.Object).WithName("Test Task");
        await request.CreateAsync();
        tasksServiceMock.Verify(s => s.CreateTaskAsync("list123", It.Is<CreateTaskRequest>(r => r.Name == "Test Task"), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    // --- TaskFluentUpdateRequest Tests ---

    [Fact]
    public void Update_Validate_MissingTaskId_ThrowsException()
    {
        var tasksServiceMock = new Mock<ITasksService>();
        var request = new TaskFluentUpdateRequest(string.Empty, tasksServiceMock.Object).WithName("New Name"); // Changed null to string.Empty
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("TaskId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_ValidRequest_DoesNotThrow()
    {
        // Note: Update validation for TaskFluentUpdateRequest is currently lenient.
        var tasksServiceMock = new Mock<ITasksService>();
        var request = new TaskFluentUpdateRequest("task123", tasksServiceMock.Object).WithName("New Name");
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task UpdateAsync_InvalidRequest_ThrowsException()
    {
        var tasksServiceMock = new Mock<ITasksService>();
        var request = new TaskFluentUpdateRequest(string.Empty, tasksServiceMock.Object); // Invalid, changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.UpdateAsync());
        Assert.Contains("TaskId is required.", ex.ValidationErrors);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_CallsService()
    {
        var tasksServiceMock = new Mock<ITasksService>();
        tasksServiceMock
            .Setup(s => s.UpdateTaskAsync(It.IsAny<string>(), It.IsAny<UpdateTaskRequest>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new CuTask(
                Id: "task123",
                Name: "Updated Task",
                Status: new ClickUp.Api.Client.Models.Common.Status("open", "blue", 0, "open"), // Corrected namespace and added params
                Creator: new ClickUp.Api.Client.Models.Entities.Users.User(1, "test", "test@example.com", null,null,null), // Corrected namespace and added params
                DateCreated: System.DateTimeOffset.UtcNow,
                // Providing null for other nullable properties
                CustomId: null,
                CustomItemId: null,
                TextContent: null,
                Description: null,
                MarkdownDescription: null,
                OrderIndex: null,
                DateUpdated: null,
                DateClosed: null,
                Archived: null,
                Assignees: null,
                GroupAssignees: null,
                Watchers: null,
                Checklists: null,
                Tags: null,
                Parent: null,
                Priority: null,
                DueDate: null,
                StartDate: null,
                Points: null,
                TimeEstimate: null,
                TimeSpent: null,
                CustomFields: null,
                Dependencies: null,
                LinkedTasks: null,
                TeamId: null,
                Url: null,
                Sharing: null,
                PermissionLevel: null,
                List: null,
                Folder: null,
                Space: null,
                Project: null
            ));

        var request = new TaskFluentUpdateRequest("task123", tasksServiceMock.Object).WithName("Updated Task");
        await request.UpdateAsync();
        tasksServiceMock.Verify(s => s.UpdateTaskAsync("task123", It.Is<UpdateTaskRequest>(r => r.Name == "Updated Task"), null, null, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }
}
