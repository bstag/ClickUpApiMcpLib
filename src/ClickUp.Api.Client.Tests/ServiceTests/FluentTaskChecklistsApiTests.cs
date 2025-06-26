using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentTaskChecklistsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentTaskChecklistsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentTaskChecklistsApi_CreateChecklist_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var taskId = "testTaskId";
        var checklistName = "Test Checklist";
        var expectedResponse = new CreateChecklistResponse { Checklist = new ClickUp.Api.Client.Models.Entities.Checklists.Checklist("checklistId", "testTaskId", checklistName, 0, 0, 0, null) };

        var mockTaskChecklistsService = new Mock<ITaskChecklistsService>();
        mockTaskChecklistsService.Setup(x => x.CreateChecklistAsync(
            It.IsAny<string>(),
            It.IsAny<ClickUp.Api.Client.Models.RequestModels.Checklists.CreateChecklistRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentTaskChecklistsApi = new FluentTaskChecklistsApi(mockTaskChecklistsService.Object);

        // Act
        var result = await fluentTaskChecklistsApi.CreateChecklist(taskId)
            .WithName(checklistName)
            .CreateAsync(true, "testTeamId");

        // Assert
        Assert.Equal(expectedResponse, result);
        mockTaskChecklistsService.Verify(x => x.CreateChecklistAsync(
            taskId,
            It.Is<ClickUp.Api.Client.Models.RequestModels.Checklists.CreateChecklistRequest>(req =>
                req.Name == checklistName),
            true,
            "testTeamId",
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
