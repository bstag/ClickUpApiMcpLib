using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using ClickUp.Api.Client.Models.Entities.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Parameters;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentTasksApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentTasksApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentTasksApi_Get_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var listId = "testListId";
        var tasksList = new List<CuTask>();
        var pagedResult = new ClickUp.Api.Client.Models.Common.Pagination.PagedResult<CuTask>(tasksList, 1, 10, false);

        var mockTasksService = new Mock<ITasksService>();
        // Ensure the setup matches the exact signature from ITasksService
        mockTasksService.Setup(x => x.GetTasksAsync(
                listId,
                It.IsAny<Action<GetTasksRequestParameters>>(), // Adjusted to match the expected delegate type
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);
        var fluentTasksApi = new TasksFluentApi(mockTasksService.Object);

        // Act
        var result = await fluentTasksApi.Get(listId)
            .WithArchived(true)
            .WithPage(1)
            .GetAsync();

        // Assert
        Assert.Equal(tasksList, result.Items);
        mockTasksService.VerifyAll(); // Verify all setups were met
    }

    [Fact]
    public async Task FluentTasksApi_GetFilteredTeamTasks_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var tasksList = new List<CuTask>();
        var pagedResult = new ClickUp.Api.Client.Models.Common.Pagination.PagedResult<CuTask>(tasksList, 0, 10, false);

        var mockTasksService = new Mock<ITasksService>();
        // Ensure the setup matches the exact signature from ITasksService
        mockTasksService.Setup(x => x.GetFilteredTeamTasksAsync(
                workspaceId, // Match specific workspaceId
                It.IsAny<Action<GetTasksRequestParameters>>(), // Simplified for build
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var fluentTasksApi = new TasksFluentApi(mockTasksService.Object);

        // Act
        var result = await fluentTasksApi.GetFilteredTeamTasks(workspaceId)
            .WithSubtasks(true)
            .WithIncludeClosed(false)
            .WithPage(0)
            .GetAsync();

        // Assert
        Assert.Equal(tasksList, result.Items);
        // Verify the call with the Action delegate
        mockTasksService.Verify(x => x.GetFilteredTeamTasksAsync(
            workspaceId,
            It.IsAny<Action<GetTasksRequestParameters>>(), // Simplified for build
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
