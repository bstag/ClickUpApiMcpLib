using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Tasks;

using Moq;

using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentAddTaskLinkRequestTests
{
    private readonly Mock<ITaskRelationshipsService> _mockTaskRelationshipsService;

    public FluentAddTaskLinkRequestTests()
    {
        _mockTaskRelationshipsService = new Mock<ITaskRelationshipsService>();
    }

    [Fact]
    public async Task AddAsync_ShouldCallAddTaskLinkAsyncWithCorrectParameters()
    {
        // Arrange
        var taskId = "testTaskId";
        var linksToTaskId = "testLinksToTaskId";
        var customTaskIds = true;
        var teamId = "testTeamId";
        var expectedTask = new CuTask(
            Id: "testId",
            CustomId: null,
            CustomItemId: null,
            Name: "testName",
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

        _mockTaskRelationshipsService.Setup(x => x.AddTaskLinkAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        var fluentRequest = new TaskLinkFluentAddRequest(taskId, linksToTaskId, _mockTaskRelationshipsService.Object)
            .WithCustomTaskIds(customTaskIds)
            .WithTeamId(teamId);

        // Act
        var result = await fluentRequest.AddAsync();

        // Assert
        Assert.Equal(expectedTask, result);
        _mockTaskRelationshipsService.Verify(x => x.AddTaskLinkAsync(
            taskId,
            linksToTaskId,
            customTaskIds,
            teamId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldCallAddTaskLinkAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var taskId = "testTaskId";
        var linksToTaskId = "testLinksToTaskId";
        var expectedTask = new CuTask(
            Id: "testId",
            CustomId: null,
            CustomItemId: null,
            Name: "testName",
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

        _mockTaskRelationshipsService.Setup(x => x.AddTaskLinkAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        var fluentRequest = new TaskLinkFluentAddRequest(taskId, linksToTaskId, _mockTaskRelationshipsService.Object);

        // Act
        var result = await fluentRequest.AddAsync();

        // Assert
        Assert.Equal(expectedTask, result);
        _mockTaskRelationshipsService.Verify(x => x.AddTaskLinkAsync(
            taskId,
            linksToTaskId,
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
