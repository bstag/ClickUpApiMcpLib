using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;

using Moq;

using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentCreateChecklistItemRequestTests
{
    private readonly Mock<ITaskChecklistsService> _mockTaskChecklistsService;

    public FluentCreateChecklistItemRequestTests()
    {
        _mockTaskChecklistsService = new Mock<ITaskChecklistsService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateChecklistItemAsyncWithCorrectParameters()
    {
        // Arrange
        var checklistId = "testChecklistId";
        var name = "testItemName";
        var assignee = "123";
        var expectedResponse = new CreateChecklistItemResponse(); // Mock a response

        _mockTaskChecklistsService.Setup(x => x.CreateChecklistItemAsync(
            It.IsAny<string>(),
            It.IsAny<CreateChecklistItemRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new ChecklistItemFluentCreateRequest(checklistId, _mockTaskChecklistsService.Object)
            .WithName(name)
            .WithAssignee(assignee);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockTaskChecklistsService.Verify(x => x.CreateChecklistItemAsync(
            checklistId,
            It.Is<CreateChecklistItemRequest>(req =>
                req.Name == name &&
                req.Assignee == int.Parse(assignee)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateChecklistItemAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var checklistId = "testChecklistId";
        var expectedResponse = new CreateChecklistItemResponse(); // Mock a response

        _mockTaskChecklistsService.Setup(x => x.CreateChecklistItemAsync(
            It.IsAny<string>(),
            It.IsAny<CreateChecklistItemRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new ChecklistItemFluentCreateRequest(checklistId, _mockTaskChecklistsService.Object);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockTaskChecklistsService.Verify(x => x.CreateChecklistItemAsync(
            checklistId,
            It.Is<CreateChecklistItemRequest>(req =>
                req.Name == string.Empty &&
                req.Assignee == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
