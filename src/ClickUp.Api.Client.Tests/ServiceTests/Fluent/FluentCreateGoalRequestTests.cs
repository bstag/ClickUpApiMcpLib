using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;

using Moq;

using System; // Required for DateTimeOffset
using System.Collections.Generic;
using System.Linq; // Required for SequenceEqual
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentCreateGoalRequestTests
{
    private readonly Mock<IGoalsService> _mockGoalsService;

    public FluentCreateGoalRequestTests()
    {
        _mockGoalsService = new Mock<IGoalsService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateGoalAsyncWithCorrectParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var name = "testGoalName";
        var dueDate = 1234567890L;
        var description = "testDescription";
        var multipleOwners = true;
        var owners = new List<int> { 1, 2 };
        var color = "#FF0000";
        var folderId = "testFolderId";
        var expectedGoal = new Goal(
            Id: "goalId",
            PrettyId: null,
            Name: "testGoalName",
            TeamId: "testTeamId",
            CreatorUser: null,
            OwnerUser: null,
            Color: null,
            DateCreated: null,
            StartDate: null,
            DueDate: null,
            Description: null,
            Private: false,
            Archived: false,
            MultipleOwners: false,
            EditorToken: null,
            DateUpdated: null,
            LastUpdate: null,
            FolderId: null,
            Pinned: false,
            Owners: null,
            KeyResultCount: 0,
            Members: null,
            GroupMembers: null,
            PercentCompleted: 0,
            History: null,
            PrettyUrl: null
        );

        _mockGoalsService.Setup(x => x.CreateGoalAsync(
            It.IsAny<string>(),
            It.IsAny<CreateGoalRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGoal);

        var fluentRequest = new GoalFluentCreateRequest(workspaceId, _mockGoalsService.Object)
            .WithName(name)
            .WithDueDate(dueDate)
            .WithDescription(description)
            .WithMultipleOwners(multipleOwners)
            .WithOwners(owners)
            .WithColor(color)
            .WithFolderId(folderId);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedGoal, result);
        _mockGoalsService.Verify(x => x.CreateGoalAsync(
            workspaceId,
            It.Is<CreateGoalRequest>(req =>
                req.Name == name &&
                req.DueDate == dueDate &&
                req.Description == description &&
                req.MultipleOwners == multipleOwners &&
                req.Owners.SequenceEqual(owners) &&
                req.Color == color &&
                req.FolderId == folderId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateGoalAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var name = "testGoalName";
        var expectedGoal = new Goal(
            Id: "goalId",
            PrettyId: null,
            Name: "testGoalName",
            TeamId: "testTeamId",
            CreatorUser: null,
            OwnerUser: null,
            Color: null,
            DateCreated: null,
            StartDate: null,
            DueDate: null,
            Description: null,
            Private: false,
            Archived: false,
            MultipleOwners: false,
            EditorToken: null,
            DateUpdated: null,
            LastUpdate: null,
            FolderId: null,
            Pinned: false,
            Owners: null,
            KeyResultCount: 0,
            Members: null,
            GroupMembers: null,
            PercentCompleted: 0,
            History: null,
            PrettyUrl: null
        );

        var dueDateMillis = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var ownersList = new List<int> { 1 };

        _mockGoalsService.Setup(x => x.CreateGoalAsync(
            It.IsAny<string>(),
            It.IsAny<CreateGoalRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGoal);

        var fluentRequest = new GoalFluentCreateRequest(workspaceId, _mockGoalsService.Object)
            .WithName(name)
            .WithDueDate(dueDateMillis)
            .WithOwners(ownersList);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedGoal, result);
        _mockGoalsService.Verify(x => x.CreateGoalAsync(
            workspaceId,
            It.Is<CreateGoalRequest>(req =>
                req.Name == name &&
                req.DueDate == dueDateMillis &&
                req.Description == string.Empty &&
                req.MultipleOwners == false &&
                req.Owners.SequenceEqual(ownersList) &&
                req.Color == null &&
                req.FolderId == null &&
                req.TeamId == workspaceId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
