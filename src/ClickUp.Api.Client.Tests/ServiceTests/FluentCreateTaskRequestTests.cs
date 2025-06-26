using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentCreateTaskRequestTests
{
    private readonly Mock<ITasksService> _mockTasksService;

    public FluentCreateTaskRequestTests()
    {
        _mockTasksService = new Mock<ITasksService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateTaskAsyncWithCorrectParameters()
    {
        // Arrange
        var listId = "testListId";
        var name = "testTaskName";
        var description = "testDescription";
        var assignees = new List<int> { 1, 2 };
        var groupAssignees = new List<string> { "groupId1", "groupId2" };
        var tags = new List<string> { "tag1", "tag2" };
        var status = 1L;
        var priority = 2;
        var dueDate = 1234567890L;
        var dueDateTime = true;
        var timeEstimate = 3600000L; // 1 hour in milliseconds
        var startDate = 9876543210L;
        var startDateTime = true;
        var parent = "testParentId";
        var notifyAll = true;
        var customFields = "{}"; // Mock custom fields JSON
        var linksTo = "testLinksToId";
        var checkRequiredCustomFields = true;
        var customItemId = 12345L;
        var customTaskIds = true;
        var teamId = "testTeamId";

        var expectedTask = new CuTask(
            Id: "taskId",
            CustomId: null,
            CustomItemId: null,
            Name: name,
            TextContent: null,
            Description: description,
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

        _mockTasksService.Setup(x => x.CreateTaskAsync(
            It.IsAny<string>(),
            It.IsAny<CreateTaskRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        var fluentRequest = new FluentCreateTaskRequest(listId, _mockTasksService.Object)
            .WithName(name)
            .WithDescription(description)
            .WithAssignees(assignees)
            .WithGroupAssignees(groupAssignees)
            .WithTags(tags)
            .WithStatus(status)
            .WithPriority(priority)
            .WithDueDate(dueDate)
            .WithDueDateTime(dueDateTime)
            .WithTimeEstimate(timeEstimate)
            .WithStartDate(startDate)
            .WithStartDateTime(startDateTime)
            .WithParent(parent)
            .WithNotifyAll(notifyAll)
            .WithCustomFields(customFields)
            .WithLinksTo(linksTo)
            .WithCheckRequiredCustomFields(checkRequiredCustomFields)
            .WithCustomItemId(customItemId);

        // Act
        var result = await fluentRequest.CreateAsync(customTaskIds, teamId);

        // Assert
        Assert.Equal(expectedTask, result);
        _mockTasksService.Verify(x => x.CreateTaskAsync(
            listId,
            It.Is<CreateTaskRequest>(req =>
                req.Name == name &&
                req.Description == description &&
                req.Assignees!.SequenceEqual(assignees) &&
                req.GroupAssignees!.SequenceEqual(groupAssignees) &&
                req.Tags!.SequenceEqual(tags) &&
                req.Status == status.ToString() &&
                req.Priority == priority &&
                req.DueDate == DateTimeOffset.FromUnixTimeMilliseconds(dueDate) &&
                req.DueDateTime == dueDateTime &&
                req.TimeEstimate == (int)timeEstimate &&
                req.StartDate == DateTimeOffset.FromUnixTimeMilliseconds(startDate) &&
                req.StartDateTime == startDateTime &&
                req.Parent == parent &&
                req.NotifyAll == notifyAll &&
                req.LinksTo == linksTo &&
                req.CheckRequiredCustomFields == checkRequiredCustomFields &&
                req.CustomItemId == customItemId
                // CustomFields are not directly set in the request object from the fluent builder
            ),
            customTaskIds,
            teamId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateTaskAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var listId = "testListId";
        var expectedTask = new CuTask(
            Id: "taskId",
            CustomId: null,
            CustomItemId: null,
            Name: string.Empty,
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

        _mockTasksService.Setup(x => x.CreateTaskAsync(
            It.IsAny<string>(),
            It.IsAny<CreateTaskRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        var fluentRequest = new FluentCreateTaskRequest(listId, _mockTasksService.Object);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedTask, result);
        _mockTasksService.Verify(x => x.CreateTaskAsync(
            listId,
            It.Is<CreateTaskRequest>(req =>
                req.Name == string.Empty &&
                req.Description == null &&
                req.Assignees == null &&
                req.GroupAssignees == null &&
                req.Tags == null &&
                req.Status == null &&
                req.Priority == null &&
                req.DueDate == null &&
                req.DueDateTime == null &&
                req.TimeEstimate == null &&
                req.StartDate == null &&
                req.StartDateTime == null &&
                req.Parent == null &&
                req.NotifyAll == null &&
                req.LinksTo == null &&
                req.CheckRequiredCustomFields == null &&
                req.CustomItemId == null
            ),
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
