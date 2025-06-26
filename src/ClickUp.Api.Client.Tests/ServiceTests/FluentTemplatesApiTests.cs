using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Templates;
using ClickUp.Api.Client.Models.Entities.Templates;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentTemplatesApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentTemplatesApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentTemplatesApi_GetTaskTemplates_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new GetTaskTemplatesResponse(new List<TaskTemplate>());

        var mockTemplatesService = new Mock<ITemplatesService>();
        mockTemplatesService.Setup(x => x.GetTaskTemplatesAsync(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentTemplatesApi = new FluentTemplatesApi(mockTemplatesService.Object);

        // Act
        var result = await fluentTemplatesApi.GetTaskTemplates(workspaceId)
            .WithPage(0)
            .GetAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockTemplatesService.Verify(x => x.GetTaskTemplatesAsync(
            workspaceId,
            0,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
