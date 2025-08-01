using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent.Builders;
using ClickUp.Api.Client.Fluent.Configuration;
using ClickUp.Api.Client.Fluent.Validation;
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent.Enhanced;

/// <summary>
/// Enhanced fluent builder for creating spaces with improved validation and configuration patterns.
/// </summary>
public class SpaceFluentCreateRequestEnhanced : FluentBuilderBase<SpaceFluentCreateRequestEnhanced, Space>
{
    private readonly string _workspaceId;
    private readonly ISpacesService _spacesService;

    // State keys for type-safe state management
    private const string NameKey = "name";
    private const string FeaturesKey = "features";

    public SpaceFluentCreateRequestEnhanced(string workspaceId, ISpacesService spacesService)
    {
        _workspaceId = workspaceId ?? throw new ArgumentNullException(nameof(workspaceId));
        _spacesService = spacesService ?? throw new ArgumentNullException(nameof(spacesService));
    }

    /// <summary>
    /// Sets the space name.
    /// </summary>
    /// <param name="name">The space name (required)</param>
    /// <returns>The builder for chaining</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or empty</exception>
    public SpaceFluentCreateRequestEnhanced WithName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Space name cannot be null or empty.", nameof(name));
        
        GetOrSetState(NameKey, name);
        return this;
    }

    /// <summary>
    /// Configures space features using a fluent configuration builder.
    /// </summary>
    /// <param name="configure">The configuration action</param>
    /// <returns>The builder for chaining</returns>
    /// <example>
    /// <code>
    /// var space = await client.Spaces
    ///     .CreateSpace(workspaceId)
    ///     .WithName("Development Space")
    ///     .WithFeatures(features => features
    ///         .EnableTimeTracking()
    ///         .EnableCustomFields()
    ///         .WithIntegrations(integrations => integrations
    ///             .EnableGitHub()
    ///             .EnableSlack()))
    ///     .CreateAsync();
    /// </code>
    /// </example>
    public SpaceFluentCreateRequestEnhanced WithFeatures(Action<SpaceFeatureConfigurationBuilder> configure)
    {
        var builder = new SpaceFeatureConfigurationBuilder();
        configure(builder);
        GetOrSetState(FeaturesKey, builder.Build());
        return this;
    }

    /// <summary>
    /// Applies a development-friendly configuration preset.
    /// Enables time tracking, custom fields, tags, priorities, GitHub, and Slack integrations.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFluentCreateRequestEnhanced ForDevelopment()
    {
        return WithFeatures(features => features.ForDevelopment());
    }

    /// <summary>
    /// Applies a project management configuration preset.
    /// Enables multiple assignees, due dates, time tracking, priorities, portfolios, email, and Google Drive integrations.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFluentCreateRequestEnhanced ForProjectManagement()
    {
        return WithFeatures(features => features.ForProjectManagement());
    }

    /// <summary>
    /// Applies a minimal configuration preset.
    /// Disables advanced features and integrations for simple use cases.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFluentCreateRequestEnhanced WithMinimalFeatures()
    {
        return WithFeatures(features => features.Minimal());
    }

    /// <summary>
    /// Enables multiple assignees for tasks in this space.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFluentCreateRequestEnhanced EnableMultipleAssignees()
    {
        return WithFeatures(features => features.EnableMultipleAssignees());
    }

    /// <summary>
    /// Enables time tracking for tasks in this space.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFluentCreateRequestEnhanced EnableTimeTracking()
    {
        return WithFeatures(features => features.EnableTimeTracking());
    }

    /// <summary>
    /// Enables custom fields for tasks in this space.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFluentCreateRequestEnhanced EnableCustomFields()
    {
        return WithFeatures(features => features.EnableCustomFields());
    }

    /// <summary>
    /// Enables portfolios for this space.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFluentCreateRequestEnhanced EnablePortfolios()
    {
        return WithFeatures(features => features.EnablePortfolios());
    }

    /// <summary>
    /// Enables GitHub integration for this space.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFluentCreateRequestEnhanced EnableGitHubIntegration()
    {
        return WithFeatures(features => features.WithIntegrations(integrations => integrations.EnableGitHub()));
    }

    /// <summary>
    /// Enables Slack integration for this space.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFluentCreateRequestEnhanced EnableSlackIntegration()
    {
        return WithFeatures(features => features.WithIntegrations(integrations => integrations.EnableSlack()));
    }

    /// <summary>
    /// Creates a space optimized for software development teams.
    /// </summary>
    /// <param name="teamName">The development team name</param>
    /// <returns>The builder for chaining</returns>
    public SpaceFluentCreateRequestEnhanced ForDevelopmentTeam(string teamName)
    {
        return WithName($"{teamName} Development")
            .ForDevelopment();
    }

    /// <summary>
    /// Creates a space optimized for marketing teams.
    /// </summary>
    /// <param name="teamName">The marketing team name</param>
    /// <returns>The builder for chaining</returns>
    public SpaceFluentCreateRequestEnhanced ForMarketingTeam(string teamName)
    {
        return WithName($"{teamName} Marketing")
            .WithFeatures(features => features
                .EnableMultipleAssignees()
                .EnableDueDates()
                .EnableTags()
                .EnablePriorities()
                .WithIntegrations(integrations => integrations
                    .EnableEmail()
                    .EnableGoogleDrive()
                    .EnableSlack()));
    }

    /// <summary>
    /// Creates the space asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created space</returns>
    public async Task<Space> CreateAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(cancellationToken);
    }

    protected override void ValidateCore()
    {
        var pipeline = new FluentValidationPipeline()
            .RequiredField(_workspaceId, "WorkspaceId")
            .RequiredField(GetOrSetState<string>(NameKey), "Space Name")
            .MaxLengthField(GetOrSetState<string>(NameKey), 100, "Space Name")
            .MinLengthField(GetOrSetState<string>(NameKey), 1, "Space Name");

        pipeline.ValidateAndThrow();
    }

    protected override Space BuildCore()
    {
        // This method is not used for async operations
        throw new NotSupportedException("Use ExecuteAsync for space creation.");
    }

    public override async Task<Space> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Validate();

        var features = GetOrSetState<SpaceFeatureConfiguration>(FeaturesKey) ?? new SpaceFeatureConfiguration();
        
        var spaceFeatures = new Features(
            DueDates: new DueDatesFeature(Enabled: features.DueDatesEnabled, StartDateEnabled: null, RemapDueDatesEnabled: null, DueDatesForSubtasksRollUpEnabled: null),
            TimeTracking: new TimeTrackingFeature(Enabled: features.TimeTrackingEnabled, HarvestEnabled: null, RollUpEnabled: null),
            Tags: new TagsFeature(Enabled: features.TagsEnabled),
            TimeEstimates: new TimeEstimatesFeature(Enabled: features.TimeEstimatesEnabled, RollUpEnabled: null, PerAssigneeEnabled: null),
            Checklists: new ChecklistsFeature(Enabled: features.ChecklistsEnabled),
            CustomFields: new CustomFieldsFeature(Enabled: features.CustomFieldsEnabled),
            RemapDependencies: new RemapDependenciesFeature(Enabled: features.RemapDependenciesEnabled),
            DependencyWarning: new DependencyWarningFeature(Enabled: features.DependencyWarningEnabled),
            Portfolios: new PortfoliosFeature(Enabled: features.PortfoliosEnabled),
            Sprints: new SprintsFeature(Enabled: features.SprintsEnabled, LegacySprintsEnabled: null),
            Points: new PointsFeature(Enabled: features.PointsEnabled),
            CustomTaskIds: new CustomTaskIdsFeature(Enabled: features.CustomTaskIdsEnabled),
            MultipleAssignees: new MultipleAssigneesFeature(Enabled: features.MultipleAssignees),
            Emails: new EmailsFeature(Enabled: features.Integrations.EmailEnabled)
        );

        var createSpaceRequest = new CreateSpaceRequest(
            Name: GetOrSetState<string>(NameKey) ?? string.Empty,
            MultipleAssignees: features.MultipleAssignees,
            Features: spaceFeatures
        );

        return await _spacesService.CreateSpaceAsync(
            _workspaceId,
            createSpaceRequest,
            cancellationToken
        );
    }

    protected override SpaceFluentCreateRequestEnhanced CreateInstance()
    {
        return new SpaceFluentCreateRequestEnhanced(_workspaceId, _spacesService);
    }
}

/// <summary>
/// Extension methods for enhanced space fluent API.
/// </summary>
public static class SpaceFluentExtensions
{
    /// <summary>
    /// Creates an enhanced space fluent builder.
    /// </summary>
    /// <param name="spacesApi">The spaces fluent API</param>
    /// <param name="workspaceId">The workspace ID</param>
    /// <param name="spacesService">The spaces service</param>
    /// <returns>The enhanced space fluent builder</returns>
    public static SpaceFluentCreateRequestEnhanced CreateSpaceEnhanced(
        this SpacesFluentApi spacesApi, 
        string workspaceId, 
        ISpacesService spacesService)
    {
        return new SpaceFluentCreateRequestEnhanced(workspaceId, spacesService);
    }
}