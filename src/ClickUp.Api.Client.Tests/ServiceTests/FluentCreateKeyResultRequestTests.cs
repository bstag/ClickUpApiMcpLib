using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentCreateKeyResultRequestTests
{
    private readonly Mock<IGoalsService> _mockGoalsService;

    public FluentCreateKeyResultRequestTests()
    {
        _mockGoalsService = new Mock<IGoalsService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateKeyResultAsyncWithCorrectParameters()
    {
        // Arrange
        var goalId = "testGoalId";
        var name = "testKeyResultName";
        var owners = new List<int> { 1, 2 };
        var type = "number";
        var stepsStart = 0;
        var stepsEnd = 100;
        var unit = "%";
        var taskIds = new List<string> { "taskId1", "taskId2" };
        var listIds = new List<string> { "listId1", "listId2" };
        var expectedKeyResult = new KeyResult(
            Id: "keyResultId",
            GoalId: goalId,
            Name: name,
            Type: type,
            Unit: unit,
            CreatorUser: null,
            DateCreated: DateTimeOffset.UtcNow,
            GoalPrettyId: null,
            PercentCompleted: null,
            Completed: false,
            TaskIds: null,
            ListIds: null,
            SubcategoryIds: null,
            Owners: null,
            LastAction: null,
            StepsCurrent: null,
            StepsStart: null,
            StepsEnd: null,
            StepsTaken: null,
            History: null,
            LastActionDate: null,
            Active: null
        ); // Mock a KeyResult object

        _mockGoalsService.Setup(x => x.CreateKeyResultAsync(
            It.IsAny<string>(),
            It.IsAny<CreateKeyResultRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedKeyResult);

        var fluentRequest = new FluentCreateKeyResultRequest(goalId, _mockGoalsService.Object)
            .WithName(name)
            .WithOwners(owners)
            .WithType(type)
            .WithStepsStart(stepsStart)
            .WithStepsEnd(stepsEnd)
            .WithUnit(unit)
            .WithTaskIds(taskIds)
            .WithListIds(listIds);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedKeyResult, result);
        _mockGoalsService.Verify(x => x.CreateKeyResultAsync(
            goalId,
            It.Is<CreateKeyResultRequest>(req =>
                req.Name == name &&
                req.Owners.SequenceEqual(owners) &&
                req.Type == type &&
                (int)req.StepsStart! == stepsStart &&
                (int)req.StepsEnd! == stepsEnd &&
                req.Unit == unit &&
                req.TaskIds!.SequenceEqual(taskIds) &&
                req.ListIds!.SequenceEqual(listIds)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateKeyResultAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var goalId = "testGoalId";
        var expectedKeyResult = new KeyResult(
            Id: "keyResultId",
            GoalId: goalId,
            Name: string.Empty,
            Type: string.Empty,
            Unit: null,
            CreatorUser: null,
            DateCreated: DateTimeOffset.UtcNow,
            GoalPrettyId: null,
            PercentCompleted: null,
            Completed: false,
            TaskIds: null,
            ListIds: null,
            SubcategoryIds: null,
            Owners: null,
            LastAction: null,
            StepsCurrent: null,
            StepsStart: null,
            StepsEnd: null,
            StepsTaken: null,
            History: null,
            LastActionDate: null,
            Active: null
        ); // Mock a KeyResult object

        _mockGoalsService.Setup(x => x.CreateKeyResultAsync(
            It.IsAny<string>(),
            It.IsAny<CreateKeyResultRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedKeyResult);

        var fluentRequest = new FluentCreateKeyResultRequest(goalId, _mockGoalsService.Object);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedKeyResult, result);
        _mockGoalsService.Verify(x => x.CreateKeyResultAsync(
            goalId,
            It.Is<CreateKeyResultRequest>(req =>
                req.Name == string.Empty &&
                !req.Owners.Any() &&
                req.Type == string.Empty &&
                req.StepsStart == null &&
                req.StepsEnd != null && // Should be a new object
                req.Unit == null &&
                req.TaskIds == null &&
                req.ListIds == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
