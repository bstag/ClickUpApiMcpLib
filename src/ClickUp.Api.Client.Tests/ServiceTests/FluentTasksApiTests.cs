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
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

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
        var expectedResponse = new GetTasksResponse(new List<ClickUp.Api.Client.Models.Entities.Tasks.CuTask>(), true); // Mock a response

        var mockTasksService = new Mock<ITasksService>();
        mockTasksService.Setup(x => x.GetTasksAsync(
            It.IsAny<string>(),
            It.IsAny<ClickUp.Api.Client.Models.RequestModels.Tasks.GetTasksRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Manually inject the mock service into the fluent API for testing
        var fluentTasksApi = new FluentTasksApi(mockTasksService.Object);

        // Act
        var result = await fluentTasksApi.Get(listId)
            .WithArchived(true)
            .WithPage(1)
            .GetAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockTasksService.Verify(x => x.GetTasksAsync(
            listId,
            It.Is<ClickUp.Api.Client.Models.RequestModels.Tasks.GetTasksRequest>(req =>
                req.Archived == true &&
                req.Page == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentTasksApi_GetFilteredTeamTasks_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new GetTasksResponse(new List<CuTask>(), true);

        var mockTasksService = new Mock<ITasksService>();
        mockTasksService.Setup(x => x.GetFilteredTeamTasksAsync(
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<bool?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<bool?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<IEnumerable<long>?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentTasksApi = new FluentTasksApi(mockTasksService.Object);

        // Act
        var result = await fluentTasksApi.GetFilteredTeamTasks(workspaceId)
            .WithSubtasks(true)
            .WithIncludeClosed(false)
            .GetAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockTasksService.Verify(x => x.GetFilteredTeamTasksAsync(
            workspaceId,
            null, // page
            null, // orderBy
            null, // reverse
            true, // subtasks
            null, // spaceIds
            null, // projectIds
            null, // listIds
            null, // statuses
            false, // includeClosed
            null, // assignees
            null, // tags
            null, // dueDateGreaterThan
            null, // dueDateLessThan
            null, // dateCreatedGreaterThan
            null, // dateCreatedLessThan
            null, // dateUpdatedGreaterThan
            null, // dateUpdatedLessThan
            null, // customFields
            null, // customTaskIds
            null, // teamIdForCustomTaskIds
            null, // customItems
            null, // dateDoneGreaterThan
            null, // dateDoneLessThan
            null, // parentTaskId
            null, // includeMarkdownDescription
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
