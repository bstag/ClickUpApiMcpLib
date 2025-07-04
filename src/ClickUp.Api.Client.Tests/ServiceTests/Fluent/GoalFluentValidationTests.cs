using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using System.Collections.Generic; // Required for List<int>
using System.Threading.Tasks; // Required for Task

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class GoalFluentValidationTests
{
    // --- GoalFluentCreateRequest Tests ---

    [Fact]
    public void GoalCreate_Validate_MissingWorkspaceId_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new GoalFluentCreateRequest(string.Empty, goalsServiceMock.Object) // Changed null to string.Empty
            .WithName("Test Goal")
            .WithDueDate(System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            .WithOwners(new List<int> { 1 });
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("WorkspaceId (TeamId) is required.", ex.ValidationErrors);
    }

    [Fact]
    public void GoalCreate_Validate_MissingName_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new GoalFluentCreateRequest("ws123", goalsServiceMock.Object)
            .WithDueDate(System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            .WithOwners(new List<int> { 1 });
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Goal name is required.", ex.ValidationErrors);
    }

    [Fact]
    public void GoalCreate_Validate_MissingDueDate_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new GoalFluentCreateRequest("ws123", goalsServiceMock.Object)
            .WithName("Test Goal")
            .WithOwners(new List<int> { 1 });
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("DueDate is required.", ex.ValidationErrors);
    }

    [Fact]
    public void GoalCreate_Validate_MissingOwners_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new GoalFluentCreateRequest("ws123", goalsServiceMock.Object)
            .WithName("Test Goal")
            .WithDueDate(System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("At least one owner is required for the Goal.", ex.ValidationErrors);
    }

    [Fact]
    public void GoalCreate_Validate_ValidRequest_DoesNotThrow()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new GoalFluentCreateRequest("ws123", goalsServiceMock.Object)
            .WithName("Test Goal")
            .WithDueDate(System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            .WithOwners(new List<int> { 1 });
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task GoalCreateAsync_InvalidRequest_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new GoalFluentCreateRequest(string.Empty, goalsServiceMock.Object); // Invalid state, Changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync());
        Assert.Contains("WorkspaceId (TeamId) is required.", ex.ValidationErrors);
    }

    // --- GoalFluentUpdateRequest Tests ---

    [Fact]
    public void GoalUpdate_Validate_MissingGoalId_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new GoalFluentUpdateRequest(string.Empty, goalsServiceMock.Object).WithName("New Name"); // Changed null to string.Empty
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("GoalId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void GoalUpdate_Validate_ValidRequest_DoesNotThrow()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new GoalFluentUpdateRequest("goal123", goalsServiceMock.Object).WithName("New Name");
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task GoalUpdateAsync_InvalidRequest_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new GoalFluentUpdateRequest(string.Empty, goalsServiceMock.Object); // Invalid state, Changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.UpdateAsync());
        Assert.Contains("GoalId is required.", ex.ValidationErrors);
    }


    // --- KeyResultFluentCreateRequest Tests ---

    [Fact]
    public void KeyResultCreate_Validate_MissingGoalId_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new KeyResultFluentCreateRequest(string.Empty, goalsServiceMock.Object) // Changed null to string.Empty
            .WithName("KR1").WithType("number").WithOwners(new List<int>{1}).WithStepsEnd(100);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("GoalId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void KeyResultCreate_Validate_MissingName_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new KeyResultFluentCreateRequest("goal123", goalsServiceMock.Object)
            .WithType("number").WithOwners(new List<int>{1}).WithStepsEnd(100);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Key Result name is required.", ex.ValidationErrors);
    }

    [Fact]
    public void KeyResultCreate_Validate_MissingType_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new KeyResultFluentCreateRequest("goal123", goalsServiceMock.Object)
            .WithName("KR1").WithOwners(new List<int>{1}).WithStepsEnd(100);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Key Result type is required.", ex.ValidationErrors);
    }

    [Fact]
    public void KeyResultCreate_Validate_MissingOwners_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new KeyResultFluentCreateRequest("goal123", goalsServiceMock.Object)
            .WithName("KR1").WithType("number").WithStepsEnd(100);
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("At least one owner is required for the Key Result.", ex.ValidationErrors);
    }

    [Fact]
    public void KeyResultCreate_Validate_MissingStepsEnd_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new KeyResultFluentCreateRequest("goal123", goalsServiceMock.Object)
            .WithName("KR1").WithType("number").WithOwners(new List<int>{1});
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("StepsEnd is required for the Key Result.", ex.ValidationErrors);
    }

    [Fact]
    public void KeyResultCreate_Validate_ValidRequest_DoesNotThrow()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new KeyResultFluentCreateRequest("goal123", goalsServiceMock.Object)
            .WithName("KR1").WithType("number").WithOwners(new List<int>{1}).WithStepsEnd(100);
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task KeyResultCreateAsync_InvalidRequest_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new KeyResultFluentCreateRequest(string.Empty, goalsServiceMock.Object); // Invalid state, Changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync());
        Assert.Contains("GoalId is required.", ex.ValidationErrors);
    }

    // --- KeyResultFluentEditRequest Tests ---

    [Fact]
    public void KeyResultEdit_Validate_MissingKeyResultId_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new KeyResultFluentEditRequest(null, goalsServiceMock.Object).WithName("New KR Name");
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("KeyResultId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void KeyResultEdit_Validate_ValidRequest_DoesNotThrow()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new KeyResultFluentEditRequest("kr123", goalsServiceMock.Object).WithName("New KR Name");
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task KeyResultEditAsync_InvalidRequest_ThrowsException()
    {
        var goalsServiceMock = new Mock<IGoalsService>();
        var request = new KeyResultFluentEditRequest(null, goalsServiceMock.Object); // Invalid state
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.EditAsync());
        Assert.Contains("KeyResultId is required.", ex.ValidationErrors);
    }
}
