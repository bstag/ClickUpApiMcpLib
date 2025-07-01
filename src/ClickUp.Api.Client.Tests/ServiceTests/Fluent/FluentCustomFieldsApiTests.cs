using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.CustomFields;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentCustomFieldsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentCustomFieldsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentCustomFieldsApi_GetAccessibleCustomFieldsAsync_ShouldCallService()
    {
        // Arrange
        var listId = "testListId";
        var expectedFields = new List<CustomFieldDefinition>();

        var mockCustomFieldsService = new Mock<ICustomFieldsService>();
        mockCustomFieldsService.Setup(x => x.GetAccessibleCustomFieldsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFields);

        var fluentCustomFieldsApi = new CustomFieldsFluentApi(mockCustomFieldsService.Object);

        // Act
        var result = await fluentCustomFieldsApi.GetAccessibleCustomFieldsAsync(listId);

        // Assert
        Assert.Equal(expectedFields, result);
        mockCustomFieldsService.Verify(x => x.GetAccessibleCustomFieldsAsync(
            listId,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
