using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

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
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        var fluentRequest = new FluentCreateTaskFromTemplateRequest(listId, templateId, _mockTasksService.Object)
            .WithName(name)
            .WithCustomTaskIds(customTaskIds)
            .WithTeamId(teamId);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedTask, result);
        _mockTasksService.Verify(x => x.CreateTaskFromTemplateAsync(
            listId,
            templateId,
            It.Is<CreateTaskFromTemplateRequest>(req => req.Name == name),
            customTaskIds,
            teamId,
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
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        var fluentRequest = new FluentCreateTaskFromTemplateRequest(listId, templateId, _mockTasksService.Object);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedTask, result);
        _mockTasksService.Verify(x => x.CreateTaskFromTemplateAsync(
            listId,
            templateId,
            It.Is<CreateTaskFromTemplateRequest>(req => req.Name == string.Empty),
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
