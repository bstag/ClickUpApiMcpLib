using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent.Builders;
using ClickUp.Api.Client.Fluent.Configuration;
using ClickUp.Api.Client.Fluent.Enhanced;
using ClickUp.Api.Client.Fluent.Validation;
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.Fluent.Enhanced;

public class FluentApiEnhancedTests
{
    private readonly Mock<ISpacesService> _mockSpacesService;
    private const string WorkspaceId = "workspace123";
    private const string SpaceName = "Test Space";

    public FluentApiEnhancedTests()
    {
        _mockSpacesService = new Mock<ISpacesService>();
    }

    [Fact]
    public void FluentBuilderBase_Should_ManageState_Correctly()
    {
        // Arrange
        var builder = new TestFluentBuilder();

        // Act
        var result1 = builder.WithValue("test");
        var result2 = builder.WithNumber(42);

        // Assert
        result1.Should().BeSameAs(builder);
        result2.Should().BeSameAs(builder);
        builder.GetTestValue().Should().Be("test");
        builder.GetTestNumber().Should().Be(42);
    }

    [Fact]
    public void FluentBuilderBase_Should_SupportConditionalConfiguration()
    {
        // Arrange
        var builder = new TestFluentBuilder();
        string? nullValue = null;
        string validValue = "valid";
        int? nullNumber = null;
        int validNumber = 100;

        // Act
        builder
            .WhenNotNull(nullValue, (b, v) => b.WithValue(v))
            .WhenNotNull(validValue, (b, v) => b.WithValue(v))
            .WhenHasValue(nullNumber, (b, v) => b.WithNumber(v))
            .WhenHasValue(validNumber, (b, v) => b.WithNumber(v))
            .When(true, b => b.WithValue("conditional"))
            .When(false, b => b.WithValue("should not set"));

        // Assert
        builder.GetTestValue().Should().Be("conditional"); // Last valid assignment
        builder.GetTestNumber().Should().Be(100);
    }

    [Fact]
    public void FluentValidationPipeline_Should_ValidateRequiredFields()
    {
        // Arrange
        var pipeline = new FluentValidationPipeline();

        // Act & Assert
        var exception = Assert.Throws<FluentValidationException>(() =>
            pipeline
                .RequiredField("", "TestField")
                .ValidateAndThrow());

        exception.Errors.Should().ContainSingle()
            .Which.Should().Be("TestField is required.");
    }

    [Fact]
    public void FluentValidationPipeline_Should_ValidateMultipleRules()
    {
        // Arrange
        var pipeline = new FluentValidationPipeline();

        // Act & Assert
        var exception = Assert.Throws<FluentValidationException>(() =>
            pipeline
                .RequiredField("", "Field1")
                .RequiredField(null, "Field2")
                .MaxLengthField("toolongvalue", 5, "Field3")
                .ValidateAndThrow());

        exception.Errors.Should().HaveCount(3);
        exception.Errors.Should().Contain("Field1 is required.");
        exception.Errors.Should().Contain("Field2 is required.");
        exception.Errors.Should().Contain("Field3 cannot exceed 5 characters.");
    }

    [Fact]
    public async Task FluentValidationPipeline_Should_SupportAsyncValidation()
    {
        // Arrange
        var pipeline = new FluentValidationPipeline();

        // Act
        var result = await pipeline
            .RequiredField("valid", "TestField")
            .ValidateAsync();

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void SpaceFeatureConfigurationBuilder_Should_BuildCorrectConfiguration()
    {
        // Arrange & Act
        var config = new SpaceFeatureConfigurationBuilder()
            .EnableTimeTracking()
            .EnableCustomFields()
            .EnableMultipleAssignees()
            .WithIntegrations(integrations => integrations
                .EnableGitHub()
                .EnableSlack())
            .Build();

        // Assert
        config.TimeTrackingEnabled.Should().BeTrue();
        config.CustomFieldsEnabled.Should().BeTrue();
        config.MultipleAssignees.Should().BeTrue();
        config.Integrations.GitHubEnabled.Should().BeTrue();
        config.Integrations.SlackEnabled.Should().BeTrue();
        config.Integrations.EmailEnabled.Should().BeFalse(); // Not enabled
    }

    [Fact]
    public void SpaceFeatureConfigurationBuilder_Should_ApplyDevelopmentPreset()
    {
        // Arrange & Act
        var config = new SpaceFeatureConfigurationBuilder()
            .ForDevelopment()
            .Build();

        // Assert
        config.TimeTrackingEnabled.Should().BeTrue();
        config.CustomFieldsEnabled.Should().BeTrue();
        config.TagsEnabled.Should().BeTrue();
        config.PrioritiesEnabled.Should().BeTrue();
        config.Integrations.GitHubEnabled.Should().BeTrue();
        config.Integrations.SlackEnabled.Should().BeTrue();
    }

    [Fact]
    public void SpaceFeatureConfigurationBuilder_Should_ApplyProjectManagementPreset()
    {
        // Arrange & Act
        var config = new SpaceFeatureConfigurationBuilder()
            .ForProjectManagement()
            .Build();

        // Assert
        config.MultipleAssignees.Should().BeTrue();
        config.DueDatesEnabled.Should().BeTrue();
        config.TimeTrackingEnabled.Should().BeTrue();
        config.PrioritiesEnabled.Should().BeTrue();
        config.PortfoliosEnabled.Should().BeTrue();
        config.Integrations.EmailEnabled.Should().BeTrue();
        config.Integrations.GoogleDriveEnabled.Should().BeTrue();
    }

    [Fact]
    public void SpaceFluentCreateRequestEnhanced_Should_ThrowException_WhenNameIsEmpty()
    {
        // Arrange
        var builder = new SpaceFluentCreateRequestEnhanced(WorkspaceId, _mockSpacesService.Object);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.WithName(""));
        Assert.Throws<ArgumentException>(() => builder.WithName("   "));
    }

    [Fact]
    public void SpaceFluentCreateRequestEnhanced_Should_ChainMethods_Correctly()
    {
        // Arrange
        var builder = new SpaceFluentCreateRequestEnhanced(WorkspaceId, _mockSpacesService.Object);

        // Act
        var result = builder
            .WithName(SpaceName)
            .EnableTimeTracking()
            .EnableCustomFields()
            .EnableGitHubIntegration();

        // Assert
        result.Should().BeSameAs(builder);
    }

    [Fact]
    public void SpaceFluentCreateRequestEnhanced_Should_ApplyPresets_Correctly()
    {
        // Arrange
        var builder = new SpaceFluentCreateRequestEnhanced(WorkspaceId, _mockSpacesService.Object);

        // Act
        var devBuilder = builder.ForDevelopmentTeam("Backend");
        var marketingBuilder = new SpaceFluentCreateRequestEnhanced(WorkspaceId, _mockSpacesService.Object)
            .ForMarketingTeam("Growth");

        // Assert
        devBuilder.Should().BeSameAs(builder);
        marketingBuilder.Should().NotBeSameAs(builder);
    }

    [Fact]
    public async Task SpaceFluentCreateRequestEnhanced_Should_ValidateBeforeExecution()
    {
        // Arrange
        var builder = new SpaceFluentCreateRequestEnhanced(WorkspaceId, _mockSpacesService.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FluentValidationException>(
            () => builder.CreateAsync());

        exception.Errors.Should().Contain("Space Name is required.");
    }

    [Fact]
    public async Task SpaceFluentCreateRequestEnhanced_Should_CallService_WithCorrectParameters()
    {
        // Arrange
        var expectedSpace = new Space { Id = "space123", Name = SpaceName };
        _mockSpacesService
            .Setup(s => s.CreateSpaceAsync(It.IsAny<string>(), It.IsAny<CreateSpaceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpace);

        var builder = new SpaceFluentCreateRequestEnhanced(WorkspaceId, _mockSpacesService.Object)
            .WithName(SpaceName)
            .EnableTimeTracking()
            .EnableCustomFields();

        // Act
        var result = await builder.CreateAsync();

        // Assert
        result.Should().BeSameAs(expectedSpace);
        _mockSpacesService.Verify(
            s => s.CreateSpaceAsync(
                WorkspaceId,
                It.Is<CreateSpaceRequest>(r => 
                    r.Name == SpaceName &&
                    r.TimeTrackingEnabled == true &&
                    r.CustomFieldsEnabled == true),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SpaceFluentCreateRequestEnhanced_Should_HandleComplexConfiguration()
    {
        // Arrange
        var expectedSpace = new Space { Id = "space123", Name = SpaceName };
        _mockSpacesService
            .Setup(s => s.CreateSpaceAsync(It.IsAny<string>(), It.IsAny<CreateSpaceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpace);

        var builder = new SpaceFluentCreateRequestEnhanced(WorkspaceId, _mockSpacesService.Object)
            .WithName(SpaceName)
            .WithFeatures(features => features
                .EnableTimeTracking()
                .EnableCustomFields()
                .EnableMultipleAssignees()
                .WithIntegrations(integrations => integrations
                    .EnableGitHub()
                    .EnableSlack()
                    .EnableEmail()));

        // Act
        var result = await builder.CreateAsync();

        // Assert
        result.Should().BeSameAs(expectedSpace);
        _mockSpacesService.Verify(
            s => s.CreateSpaceAsync(
                WorkspaceId,
                It.Is<CreateSpaceRequest>(r => 
                    r.Name == SpaceName &&
                    r.TimeTrackingEnabled == true &&
                    r.CustomFieldsEnabled == true &&
                    r.MultipleAssignees == true &&
                    r.GithubEnabled == true &&
                    r.SlackEnabled == true &&
                    r.EmailEnabled == true),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

// Test helper class
public class TestFluentBuilder : FluentBuilderBase<TestFluentBuilder, string>
{
    private const string ValueKey = "value";
    private const string NumberKey = "number";

    public TestFluentBuilder WithValue(string value)
    {
        GetOrSetState(ValueKey, value);
        return this;
    }

    public TestFluentBuilder WithNumber(int number)
    {
        GetOrSetState(NumberKey, number);
        return this;
    }

    public string? GetTestValue() => GetOrSetState<string>(ValueKey);
    public int GetTestNumber() => GetOrSetState<int>(NumberKey);

    protected override void ValidateCore()
    {
        // No validation for test
    }

    protected override string BuildCore()
    {
        return GetOrSetState<string>(ValueKey) ?? "default";
    }

    protected override TestFluentBuilder CreateInstance()
    {
        return new TestFluentBuilder();
    }
}