using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentAddTagsFromTimeEntriesRequestTests
{
    private readonly Mock<ITimeTrackingService> _mockTimeTrackingService;

    public FluentAddTagsFromTimeEntriesRequestTests()
    {
        _mockTimeTrackingService = new Mock<ITimeTrackingService>();
    }

    [Fact]
    public async Task AddAsync_ShouldCallAddTagsToTimeEntriesAsyncWithCorrectParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var timeEntryIds = new List<string> { "id1", "id2" };
        var tags = new List<string> { "tag1", "tag2" };

        var fluentRequest = new FluentAddTagsFromTimeEntriesRequest(workspaceId, _mockTimeTrackingService.Object)
            .WithTimeEntryIds(timeEntryIds)
            .WithTags(tags);

        // Act
        await fluentRequest.AddAsync();

        // Assert
        _mockTimeTrackingService.Verify(x => x.AddTagsToTimeEntriesAsync(
            workspaceId,
            It.Is<AddTagsFromTimeEntriesRequest>(req =>
                req.TimeEntryIds.SequenceEqual(timeEntryIds) &&
                req.Tags.Count == tags.Count &&
                req.Tags.All(t => tags.Contains(t.Name!))),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldCallAddTagsToTimeEntriesAsyncWithEmptyListsIfNoTagsOrTimeEntryIdsProvided()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";

        var fluentRequest = new FluentAddTagsFromTimeEntriesRequest(workspaceId, _mockTimeTrackingService.Object);

        // Act
        await fluentRequest.AddAsync();

        // Assert
        _mockTimeTrackingService.Verify(x => x.AddTagsToTimeEntriesAsync(
            workspaceId,
            It.Is<AddTagsFromTimeEntriesRequest>(req =>
                !req.TimeEntryIds.Any() &&
                !req.Tags.Any()),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
