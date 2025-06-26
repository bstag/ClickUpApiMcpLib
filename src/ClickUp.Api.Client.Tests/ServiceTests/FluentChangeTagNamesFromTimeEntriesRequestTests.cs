using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentChangeTagNamesFromTimeEntriesRequestTests
{
    private readonly Mock<ITimeTrackingService> _mockTimeTrackingService;

    public FluentChangeTagNamesFromTimeEntriesRequestTests()
    {
        _mockTimeTrackingService = new Mock<ITimeTrackingService>();
    }

    [Fact]
    public async Task ChangeAsync_ShouldCallChangeTimeEntryTagNameAsyncWithCorrectParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var oldName = "oldTag";
        var newName = "newTag";
        var newColor = "#FFFFFF";

        var fluentRequest = new FluentChangeTagNamesFromTimeEntriesRequest(workspaceId, _mockTimeTrackingService.Object)
            .WithOldName(oldName)
            .WithNewName(newName)
            .WithNewColor(newColor);

        // Act
        await fluentRequest.ChangeAsync();

        // Assert
        _mockTimeTrackingService.Verify(x => x.ChangeTimeEntryTagNameAsync(
            workspaceId,
            It.Is<ChangeTagNamesFromTimeEntriesRequest>(req =>
                req.Name == oldName &&
                req.NewName == newName &&
                req.TagBg == newColor &&
                req.TagFg == newColor),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangeAsync_ShouldCallChangeTimeEntryTagNameAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";

        var fluentRequest = new FluentChangeTagNamesFromTimeEntriesRequest(workspaceId, _mockTimeTrackingService.Object);

        // Act
        await fluentRequest.ChangeAsync();

        // Assert
        _mockTimeTrackingService.Verify(x => x.ChangeTimeEntryTagNameAsync(
            workspaceId,
            It.Is<ChangeTagNamesFromTimeEntriesRequest>(req =>
                req.Name == string.Empty &&
                req.NewName == string.Empty &&
                req.TagBg == string.Empty &&
                req.TagFg == string.Empty),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
