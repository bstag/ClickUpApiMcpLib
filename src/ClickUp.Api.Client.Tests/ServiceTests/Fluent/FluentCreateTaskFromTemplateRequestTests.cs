using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;

using Moq;

using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentCreateTaskFromTemplateRequestTests
{
    private readonly Mock<ITasksService> _mockTasksService;

    public FluentCreateTaskFromTemplateRequestTests()
    {
        _mockTasksService = new Mock<ITasksService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateTaskFromTemplateAsyncWithCorrectParameters()
    {
        // Arrange
        var listId = "testListId";
        var templateId = "testTemplateId";
        var name = "testTaskName";
        var customTaskIds = true;
        var teamId = "testTeamId";
        var expectedTask = new CuTask(
            Id: "taskId",
            CustomId: null,
            CustomItemId: null,
            Name: "testTaskName",
            TextContent: null,
            Description: null,
            MarkdownDescription: null,
            Status: null,
            OrderIndex: null,
            DateCreated: null,
            DateUpdated: null,
            DateClosed: null,
            Archived: null,
            Creator: null,
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
        ); // Mock a CuTask object

        _mockTasksService.Setup(x => x.CreateTaskFromTemplateAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreateTaskFromTemplateRequest>(),
            // It.IsAny<bool?>(), // Removed customTaskIds
            // It.IsAny<string?>(), // Removed teamId
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        var fluentRequest = new TemplateFluentCreateTaskRequest(listId, templateId, _mockTasksService.Object)
            .WithName(name);
            // .WithCustomTaskIds(customTaskIds) // Removed
            // .WithTeamId(teamId); // Removed

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedTask, result);
        _mockTasksService.Verify(x => x.CreateTaskFromTemplateAsync(
            listId,
            templateId,
            It.Is<CreateTaskFromTemplateRequest>(req => req.Name == name),
            // customTaskIds, // Removed
            // teamId, // Removed
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateTaskFromTemplateAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var listId = "testListId";
        var templateId = "testTemplateId";
        var expectedTask = new CuTask(
            Id: "taskId",
            CustomId: null,
            CustomItemId: null,
            Name: "testTaskName",
            TextContent: null,
            Description: null,
            MarkdownDescription: null,
            Status: null,
            OrderIndex: null,
            DateCreated: null,
            DateUpdated: null,
            DateClosed: null,
            Archived: null,
            Creator: null,
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
        ); // Mock a CuTask object

        _mockTasksService.Setup(x => x.CreateTaskFromTemplateAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreateTaskFromTemplateRequest>(),
            // It.IsAny<bool?>(), // Removed customTaskIds
            // It.IsAny<string?>(), // Removed teamId
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        var fluentRequest = new TemplateFluentCreateTaskRequest(listId, templateId, _mockTasksService.Object);
        // This test will now implicitly test with Name being null in the DTO,
        // which then gets ?? string.Empty in the fluent builder.
        // If CreateTaskFromTemplateRequest requires a Name, this might need adjustment
        // or the DTO's Name property should be nullable if the API allows it.
        // For now, assuming string.Empty is the intended default if WithName is not called.

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedTask, result);
        _mockTasksService.Verify(x => x.CreateTaskFromTemplateAsync(
            listId,
            templateId,
            It.Is<CreateTaskFromTemplateRequest>(req => req.Name == string.Empty), // Name is string.Empty due to ?? in fluent builder
            // null, // Removed customTaskIds
            // null, // Removed teamId
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
