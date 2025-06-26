using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using ClickUp.Api.Client.Models.Entities.Users; // For User
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentCreateTimeEntryRequestTests
{
    private readonly Mock<ITimeTrackingService> _mockTimeTrackingService;

    public FluentCreateTimeEntryRequestTests()
    {
        _mockTimeTrackingService = new Mock<ITimeTrackingService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateTimeEntryAsyncWithCorrectParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var taskId = "testTaskId";
        var description = "testDescription";
        var tags = new List<string> { "tag1", "tag2" };
        var start = 1234567890L;
        var duration = 3600000; // 1 hour
        var billable = true;
        var assignee = 123;
        var customTaskIds = true;
        var teamIdForCustomTaskIds = "testTeamId";
        var expectedTimeEntry = new TimeEntry(
            Id: "timeEntryId",
            Task: null,
            Wid: workspaceId,
            User: new User(Id: 1, Username: "testuser", Email: "test@example.com", Color: "#000000", ProfilePicture: null, Initials: "TU"),
            Billable: false,
            Start: "1234567890",
            End: null,
            Duration: 0,
            Description: null,
            Tags: null,
            Source: null,
            At: "1234567890",
            TaskLocationInfo: null,
            TaskTags: null,
            TaskUrl: null,
            IsLocked: null,
            LockedDetails: null
        ); // Mock a TimeEntry object

        _mockTimeTrackingService.Setup(x => x.CreateTimeEntryAsync(
            It.IsAny<string>(),
            It.IsAny<CreateTimeEntryRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTimeEntry);

        var fluentRequest = new FluentCreateTimeEntryRequest(workspaceId, _mockTimeTrackingService.Object)
            .WithTaskId(taskId)
            .WithDescription(description)
            .WithTags(tags)
            .WithStart(start)
            .WithDuration(duration)
            .WithBillable(billable)
            .WithAssignee(assignee);

        // Act
        var result = await fluentRequest.CreateAsync(customTaskIds, teamIdForCustomTaskIds);

        // Assert
        Assert.Equal(expectedTimeEntry, result);
        _mockTimeTrackingService.Verify(x => x.CreateTimeEntryAsync(
            workspaceId,
            It.Is<CreateTimeEntryRequest>(req =>
                req.TaskId == taskId &&
                req.Description == description &&
                req.Tags!.All(t => tags.Contains(t.Name!)) &&
                req.Start == start &&
                req.Duration == duration &&
                req.Billable == billable &&
                req.Assignee == assignee),
            customTaskIds,
            teamIdForCustomTaskIds,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateTimeEntryAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedTimeEntry = new TimeEntry(
            Id: "timeEntryId",
            Task: null,
            Wid: workspaceId,
            User: new User(Id: 1, Username: "testuser", Email: "test@example.com", Color: "#000000", ProfilePicture: null, Initials: "TU"),
            Billable: false,
            Start: "1234567890",
            End: null,
            Duration: 0,
            Description: null,
            Tags: null,
            Source: null,
            At: "1234567890",
            TaskLocationInfo: null,
            TaskTags: null,
            TaskUrl: null,
            IsLocked: null,
            LockedDetails: null
        ); // Mock a TimeEntry object

        _mockTimeTrackingService.Setup(x => x.CreateTimeEntryAsync(
            It.IsAny<string>(),
            It.IsAny<CreateTimeEntryRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTimeEntry);

        var fluentRequest = new FluentCreateTimeEntryRequest(workspaceId, _mockTimeTrackingService.Object);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedTimeEntry, result);
        _mockTimeTrackingService.Verify(x => x.CreateTimeEntryAsync(
            workspaceId,
            It.Is<CreateTimeEntryRequest>(req =>
                req.TaskId == null &&
                req.Description == null &&
                (req.Tags == null || !req.Tags.Any()) &&
                req.Start == 0 &&
                req.Duration == 0 &&
                req.Billable == null &&
                req.Assignee == null),
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
