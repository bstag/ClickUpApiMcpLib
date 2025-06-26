using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentAddDependencyRequestTests
{
    private readonly Mock<ITaskRelationshipsService> _mockTaskRelationshipsService;

    public FluentAddDependencyRequestTests()
    {
        _mockTaskRelationshipsService = new Mock<ITaskRelationshipsService>();
    }

    [Fact]
    public async Task AddAsync_ShouldCallAddDependencyAsyncWithCorrectParameters()
    {
        // Arrange
        var taskId = "testTaskId";
        var dependsOnTaskId = "dependsOnTaskId";
        var dependencyOfTaskId = "dependencyOfTaskId";
        var customTaskIds = true;
        var teamId = "testTeamId";

        var fluentRequest = new FluentAddDependencyRequest(taskId, _mockTaskRelationshipsService.Object)
            .WithDependsOnTaskId(dependsOnTaskId)
            .WithDependencyOfTaskId(dependencyOfTaskId)
            .WithCustomTaskIds(customTaskIds)
            .WithTeamId(teamId);

        // Act
        await fluentRequest.AddAsync();

        // Assert
        _mockTaskRelationshipsService.Verify(x => x.AddDependencyAsync(
            taskId,
            dependsOnTaskId,
            dependencyOfTaskId,
            customTaskIds,
            teamId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldCallAddDependencyAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var taskId = "testTaskId";

        var fluentRequest = new FluentAddDependencyRequest(taskId, _mockTaskRelationshipsService.Object);

        // Act
        await fluentRequest.AddAsync();

        // Assert
        _mockTaskRelationshipsService.Verify(x => x.AddDependencyAsync(
            taskId,
            null,
            null,
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
