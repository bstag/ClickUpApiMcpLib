using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;

using Microsoft.Extensions.Logging;

using Moq;

using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentTaskRelationshipsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentTaskRelationshipsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentTaskRelationshipsApi_AddDependency_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var taskId = "testTaskId";
        var dependsOnTaskId = "dependsOnTaskId";

        var mockTaskRelationshipsService = new Mock<ITaskRelationshipsService>();
        mockTaskRelationshipsService.Setup(x => x.AddDependencyAsync(
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var fluentTaskRelationshipsApi = new TaskRelationshipsFluentApi(mockTaskRelationshipsService.Object);

        // Act
        await fluentTaskRelationshipsApi.AddDependency(taskId)
            .WithDependsOnTaskId(dependsOnTaskId)
            .AddAsync();

        // Assert
        mockTaskRelationshipsService.Verify(x => x.AddDependencyAsync(
            taskId,
            dependsOnTaskId,
            null,
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
