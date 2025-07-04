using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using System.Threading.Tasks;
using System.Collections.Generic; // Required for List

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class TimeEntryFluentValidationTests
{
    // --- TimeEntryFluentCreateRequest Tests ---

    [Fact]
    public void Create_Validate_MissingWorkspaceId_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentCreateRequest(string.Empty, timeTrackingServiceMock.Object) // Changed null to string.Empty
            .WithStart(System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            .WithDuration(1000);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("WorkspaceId (TeamId) is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_MissingStart_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentCreateRequest("ws123", timeTrackingServiceMock.Object)
            .WithDuration(1000);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Start time is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_MissingDuration_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentCreateRequest("ws123", timeTrackingServiceMock.Object)
            .WithStart(System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("A positive Duration is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_InvalidDuration_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentCreateRequest("ws123", timeTrackingServiceMock.Object)
            .WithStart(System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            .WithDuration(0);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("A positive Duration is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_ValidRequest_DoesNotThrow()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentCreateRequest("ws123", timeTrackingServiceMock.Object)
            .WithStart(System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            .WithDuration(1000);
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentCreateRequest(string.Empty, timeTrackingServiceMock.Object); // Invalid, Changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync());
        Assert.Contains("WorkspaceId (TeamId) is required.", ex.ValidationErrors);
        Assert.Contains("Start time is required.", ex.ValidationErrors);
        Assert.Contains("A positive Duration is required.", ex.ValidationErrors);
    }

    // --- TimeEntryFluentUpdateRequest Tests ---

    [Fact]
    public void Update_Validate_MissingWorkspaceId_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentUpdateRequest(string.Empty, "timer123", timeTrackingServiceMock.Object) // Changed null to string.Empty
            .WithDescription("Updated desc");
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("WorkspaceId (TeamId) is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_MissingTimerId_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentUpdateRequest("ws123", string.Empty, timeTrackingServiceMock.Object) // Changed null to string.Empty
            .WithDescription("Updated desc");
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("TimerId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_InvalidDuration_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentUpdateRequest("ws123", "timer123", timeTrackingServiceMock.Object)
            .WithDuration(0);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("If Duration is provided, it must be positive.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_InvalidTagAction_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentUpdateRequest("ws123", "timer123", timeTrackingServiceMock.Object)
            .WithTagAction("modify") // Invalid action
            .WithTags(new List<string> { "tag1" });
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("TagAction must be 'add' or 'remove' if provided.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_TagActionWithoutTags_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentUpdateRequest("ws123", "timer123", timeTrackingServiceMock.Object)
            .WithTagAction("add"); // No tags provided
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Tags must be provided when TagAction is specified.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_NoFieldsSet_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentUpdateRequest("ws123", "timer123", timeTrackingServiceMock.Object);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("At least one property must be set for updating a Time Entry.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_ValidRequest_DoesNotThrow()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentUpdateRequest("ws123", "timer123", timeTrackingServiceMock.Object)
            .WithDescription("Updated description");
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task UpdateAsync_InvalidRequest_ThrowsException()
    {
        var timeTrackingServiceMock = new Mock<ITimeTrackingService>();
        var request = new TimeEntryFluentUpdateRequest(string.Empty, string.Empty, timeTrackingServiceMock.Object); // Invalid, Changed nulls to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.UpdateAsync());
        Assert.Contains("WorkspaceId (TeamId) is required.", ex.ValidationErrors);
        Assert.Contains("TimerId is required.", ex.ValidationErrors);
        Assert.Contains("At least one property must be set for updating a Time Entry.", ex.ValidationErrors);
    }
}
