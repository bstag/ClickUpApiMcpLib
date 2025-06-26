using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Members;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentMembersApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentMembersApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentMembersApi_GetTaskMembersAsync_ShouldCallService()
    {
        // Arrange
        var taskId = "testTaskId";
        var expectedMembers = new List<ClickUp.Api.Client.Models.ResponseModels.Members.Member>();

        var mockMembersService = new Mock<IMembersService>();
        mockMembersService.Setup(x => x.GetTaskMembersAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMembers);

        var fluentMembersApi = new FluentMembersApi(mockMembersService.Object);

        // Act
        var result = await fluentMembersApi.GetTaskMembersAsync(taskId);

        // Assert
        Assert.Equal(expectedMembers, result);
        mockMembersService.Verify(x => x.GetTaskMembersAsync(
            taskId,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
