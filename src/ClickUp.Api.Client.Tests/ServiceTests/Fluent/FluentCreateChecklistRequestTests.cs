using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;

using Moq;

using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentCreateChecklistRequestTests
{
    private readonly Mock<ITaskChecklistsService> _mockTaskChecklistsService;

    public FluentCreateChecklistRequestTests()
    {
        _mockTaskChecklistsService = new Mock<ITaskChecklistsService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateChecklistAsyncWithCorrectParameters()
    {
        // Arrange
        var taskId = "testTaskId";
        var name = "testChecklistName";
        var customTaskIds = true;
        var teamId = "testTeamId";
        var expectedResponse = new CreateChecklistResponse(); // Mock a response

        _mockTaskChecklistsService.Setup(x => x.CreateChecklistAsync(
            It.IsAny<string>(),
            It.IsAny<CreateChecklistRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new ChecklistFluentCreateRequest(taskId, _mockTaskChecklistsService.Object)
            .WithName(name);

        // Act
        var result = await fluentRequest.CreateAsync(customTaskIds, teamId);

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockTaskChecklistsService.Verify(x => x.CreateChecklistAsync(
            taskId,
            It.Is<CreateChecklistRequest>(req => req.Name == name),
            customTaskIds,
            teamId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateChecklistAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var taskId = "testTaskId";
        var expectedResponse = new CreateChecklistResponse(); // Mock a response

        _mockTaskChecklistsService.Setup(x => x.CreateChecklistAsync(
            It.IsAny<string>(),
            It.IsAny<CreateChecklistRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new ChecklistFluentCreateRequest(taskId, _mockTaskChecklistsService.Object);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockTaskChecklistsService.Verify(x => x.CreateChecklistAsync(
            taskId,
            It.Is<CreateChecklistRequest>(req => req.Name == string.Empty),
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
