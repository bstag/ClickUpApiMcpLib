using System;
using System.Collections.Generic;
using System.Linq;

namespace ClickUp.Api.Client.Fluent.Configuration;

/// <summary>
/// A fluent builder for creating complex configuration objects.
/// </summary>
/// <typeparam name="TConfiguration">The type of configuration being built</typeparam>
public abstract class FluentConfigurationBuilder<TConfiguration>
    where TConfiguration : class
{
    private readonly Dictionary<string, object?> _properties = new();
    private readonly List<Action<TConfiguration>> _postBuildActions = new();

    /// <summary>
    /// Sets a property value in the configuration.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value</typeparam>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="value">The value to set</param>
    /// <returns>The builder for chaining</returns>
    protected FluentConfigurationBuilder<TConfiguration> SetProperty<TValue>(string propertyName, TValue value)
    {
        _properties[propertyName] = value;
        return this;
    }

    /// <summary>
    /// Gets a property value from the configuration.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value</typeparam>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="defaultValue">The default value if property is not set</param>
    /// <returns>The property value or default</returns>
    protected TValue? GetProperty<TValue>(string propertyName, TValue? defaultValue = default)
    {
        return _properties.TryGetValue(propertyName, out var value) && value is TValue typedValue
            ? typedValue
            : defaultValue;
    }

    /// <summary>
    /// Adds a post-build action to be executed after the configuration is built.
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <returns>The builder for chaining</returns>
    protected FluentConfigurationBuilder<TConfiguration> AddPostBuildAction(Action<TConfiguration> action)
    {
        _postBuildActions.Add(action);
        return this;
    }

    /// <summary>
    /// Builds the configuration object.
    /// </summary>
    /// <returns>The built configuration</returns>
    public TConfiguration Build()
    {
        var configuration = CreateConfiguration();
        
        // Execute post-build actions
        foreach (var action in _postBuildActions)
        {
            action(configuration);
        }
        
        return configuration;
    }

    /// <summary>
    /// Creates the configuration object. Must be implemented by derived classes.
    /// </summary>
    /// <returns>The configuration object</returns>
    protected abstract TConfiguration CreateConfiguration();
}

/// <summary>
/// Configuration for space features.
/// </summary>
public class SpaceFeatureConfiguration
{
    public bool MultipleAssignees { get; set; }
    public bool DueDatesEnabled { get; set; }
    public bool TimeTrackingEnabled { get; set; }
    public bool TagsEnabled { get; set; }
    public bool PrioritiesEnabled { get; set; }
    public bool CustomFieldsEnabled { get; set; }
    public bool RemapDependenciesEnabled { get; set; }
    public bool DependencyWarningEnabled { get; set; }
    public bool PortfoliosEnabled { get; set; }
    public bool SprintsEnabled { get; set; }
    public bool PointsEnabled { get; set; }
    public bool CustomTaskIdsEnabled { get; set; }
    public bool RescheduleDependenciesEnabled { get; set; }
    public bool WeeklyProgressEnabled { get; set; }
    public bool GanttEnabled { get; set; }
    public bool TimeEstimatesEnabled { get; set; }
    public bool MilestonesEnabled { get; set; }
    public bool ChecklistsEnabled { get; set; }
    
    // Integration settings
    public IntegrationConfiguration Integrations { get; set; } = new();
}

/// <summary>
/// Configuration for integrations.
/// </summary>
public class IntegrationConfiguration
{
    public bool ZoomEnabled { get; set; }
    public bool GoogleDriveEnabled { get; set; }
    public bool SlackEnabled { get; set; }
    public bool GitHubEnabled { get; set; }
    public bool GitLabEnabled { get; set; }
    public bool BitbucketEnabled { get; set; }
    public bool EmailEnabled { get; set; }
}

/// <summary>
/// Fluent builder for space feature configuration.
/// </summary>
public class SpaceFeatureConfigurationBuilder : FluentConfigurationBuilder<SpaceFeatureConfiguration>
{
    private const string MultipleAssigneesKey = "MultipleAssignees";
    private const string DueDatesEnabledKey = "DueDatesEnabled";
    private const string TimeTrackingEnabledKey = "TimeTrackingEnabled";
    private const string TagsEnabledKey = "TagsEnabled";
    private const string PrioritiesEnabledKey = "PrioritiesEnabled";
    private const string CustomFieldsEnabledKey = "CustomFieldsEnabled";
    private const string RemapDependenciesEnabledKey = "RemapDependenciesEnabled";
    private const string DependencyWarningEnabledKey = "DependencyWarningEnabled";
    private const string PortfoliosEnabledKey = "PortfoliosEnabled";
    private const string SprintsEnabledKey = "SprintsEnabled";
    private const string PointsEnabledKey = "PointsEnabled";
    private const string CustomTaskIdsEnabledKey = "CustomTaskIdsEnabled";
    private const string RescheduleDependenciesEnabledKey = "RescheduleDependenciesEnabled";
    private const string WeeklyProgressEnabledKey = "WeeklyProgressEnabled";
    private const string GanttEnabledKey = "GanttEnabled";
    private const string TimeEstimatesEnabledKey = "TimeEstimatesEnabled";
    private const string MilestonesEnabledKey = "MilestonesEnabled";
    private const string ChecklistsEnabledKey = "ChecklistsEnabled";
    private const string IntegrationsKey = "Integrations";

    /// <summary>
    /// Enables or disables multiple assignees.
    /// </summary>
    /// <param name="enabled">True to enable multiple assignees</param>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder WithMultipleAssignees(bool enabled = true)
    {
        return (SpaceFeatureConfigurationBuilder)SetProperty(MultipleAssigneesKey, enabled);
    }

    /// <summary>
    /// Enables multiple assignees.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder EnableMultipleAssignees() => WithMultipleAssignees(true);

    /// <summary>
    /// Disables multiple assignees.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder DisableMultipleAssignees() => WithMultipleAssignees(false);

    /// <summary>
    /// Enables or disables due dates.
    /// </summary>
    /// <param name="enabled">True to enable due dates</param>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder WithDueDates(bool enabled = true)
    {
        return (SpaceFeatureConfigurationBuilder)SetProperty(DueDatesEnabledKey, enabled);
    }

    /// <summary>
    /// Enables due dates.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder EnableDueDates() => WithDueDates(true);

    /// <summary>
    /// Disables due dates.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder DisableDueDates() => WithDueDates(false);

    /// <summary>
    /// Enables or disables time tracking.
    /// </summary>
    /// <param name="enabled">True to enable time tracking</param>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder WithTimeTracking(bool enabled = true)
    {
        return (SpaceFeatureConfigurationBuilder)SetProperty(TimeTrackingEnabledKey, enabled);
    }

    /// <summary>
    /// Enables time tracking.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder EnableTimeTracking() => WithTimeTracking(true);

    /// <summary>
    /// Disables time tracking.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder DisableTimeTracking() => WithTimeTracking(false);

    /// <summary>
    /// Enables or disables tags.
    /// </summary>
    /// <param name="enabled">True to enable tags</param>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder WithTags(bool enabled = true)
    {
        return (SpaceFeatureConfigurationBuilder)SetProperty(TagsEnabledKey, enabled);
    }

    /// <summary>
    /// Enables tags.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder EnableTags() => WithTags(true);

    /// <summary>
    /// Disables tags.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder DisableTags() => WithTags(false);

    /// <summary>
    /// Enables or disables priorities.
    /// </summary>
    /// <param name="enabled">True to enable priorities</param>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder WithPriorities(bool enabled = true)
    {
        return (SpaceFeatureConfigurationBuilder)SetProperty(PrioritiesEnabledKey, enabled);
    }

    /// <summary>
    /// Enables priorities.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder EnablePriorities() => WithPriorities(true);

    /// <summary>
    /// Disables priorities.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder DisablePriorities() => WithPriorities(false);

    /// <summary>
    /// Enables or disables custom fields.
    /// </summary>
    /// <param name="enabled">True to enable custom fields</param>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder WithCustomFields(bool enabled = true)
    {
        return (SpaceFeatureConfigurationBuilder)SetProperty(CustomFieldsEnabledKey, enabled);
    }

    /// <summary>
    /// Enables custom fields.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder EnableCustomFields() => WithCustomFields(true);

    /// <summary>
    /// Disables custom fields.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder DisableCustomFields() => WithCustomFields(false);

    /// <summary>
    /// Enables or disables portfolios.
    /// </summary>
    /// <param name="enabled">True to enable portfolios</param>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder WithPortfolios(bool enabled = true)
    {
        return (SpaceFeatureConfigurationBuilder)SetProperty(PortfoliosEnabledKey, enabled);
    }

    /// <summary>
    /// Enables portfolios.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder EnablePortfolios() => WithPortfolios(true);

    /// <summary>
    /// Disables portfolios.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder DisablePortfolios() => WithPortfolios(false);

    /// <summary>
    /// Configures integrations using a nested builder.
    /// </summary>
    /// <param name="configure">The configuration action</param>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder WithIntegrations(Action<IntegrationConfigurationBuilder> configure)
    {
        var integrationBuilder = new IntegrationConfigurationBuilder();
        configure(integrationBuilder);
        return (SpaceFeatureConfigurationBuilder)SetProperty(IntegrationsKey, integrationBuilder.Build());
    }

    /// <summary>
    /// Applies a development-friendly configuration preset.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder ForDevelopment()
    {
        return EnableTimeTracking()
            .EnableCustomFields()
            .EnableTags()
            .EnablePriorities()
            .WithIntegrations(integrations => integrations
                .EnableGitHub()
                .EnableSlack());
    }

    /// <summary>
    /// Applies a project management configuration preset.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder ForProjectManagement()
    {
        return EnableMultipleAssignees()
            .EnableDueDates()
            .EnableTimeTracking()
            .EnablePriorities()
            .EnablePortfolios()
            .WithIntegrations(integrations => integrations
                .EnableEmail()
                .EnableGoogleDrive());
    }

    /// <summary>
    /// Applies a minimal configuration preset.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public SpaceFeatureConfigurationBuilder Minimal()
    {
        return DisablePortfolios()
            .DisableCustomFields()
            .WithIntegrations(integrations => integrations.DisableAll());
    }

    protected override SpaceFeatureConfiguration CreateConfiguration()
    {
        return new SpaceFeatureConfiguration
        {
            MultipleAssignees = GetProperty<bool>(MultipleAssigneesKey),
            DueDatesEnabled = GetProperty<bool>(DueDatesEnabledKey),
            TimeTrackingEnabled = GetProperty<bool>(TimeTrackingEnabledKey),
            TagsEnabled = GetProperty<bool>(TagsEnabledKey),
            PrioritiesEnabled = GetProperty<bool>(PrioritiesEnabledKey),
            CustomFieldsEnabled = GetProperty<bool>(CustomFieldsEnabledKey),
            RemapDependenciesEnabled = GetProperty<bool>(RemapDependenciesEnabledKey),
            DependencyWarningEnabled = GetProperty<bool>(DependencyWarningEnabledKey),
            PortfoliosEnabled = GetProperty<bool>(PortfoliosEnabledKey),
            SprintsEnabled = GetProperty<bool>(SprintsEnabledKey),
            PointsEnabled = GetProperty<bool>(PointsEnabledKey),
            CustomTaskIdsEnabled = GetProperty<bool>(CustomTaskIdsEnabledKey),
            RescheduleDependenciesEnabled = GetProperty<bool>(RescheduleDependenciesEnabledKey),
            WeeklyProgressEnabled = GetProperty<bool>(WeeklyProgressEnabledKey),
            GanttEnabled = GetProperty<bool>(GanttEnabledKey),
            TimeEstimatesEnabled = GetProperty<bool>(TimeEstimatesEnabledKey),
            MilestonesEnabled = GetProperty<bool>(MilestonesEnabledKey),
            ChecklistsEnabled = GetProperty<bool>(ChecklistsEnabledKey),
            Integrations = GetProperty<IntegrationConfiguration>(IntegrationsKey) ?? new IntegrationConfiguration()
        };
    }
}

/// <summary>
/// Fluent builder for integration configuration.
/// </summary>
public class IntegrationConfigurationBuilder : FluentConfigurationBuilder<IntegrationConfiguration>
{
    /// <summary>
    /// Enables or disables Zoom integration.
    /// </summary>
    /// <param name="enabled">True to enable Zoom</param>
    /// <returns>The builder for chaining</returns>
    public IntegrationConfigurationBuilder WithZoom(bool enabled = true)
    {
        return (IntegrationConfigurationBuilder)SetProperty("ZoomEnabled", enabled);
    }

    /// <summary>
    /// Enables Zoom integration.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public IntegrationConfigurationBuilder EnableZoom() => WithZoom(true);

    /// <summary>
    /// Enables or disables GitHub integration.
    /// </summary>
    /// <param name="enabled">True to enable GitHub</param>
    /// <returns>The builder for chaining</returns>
    public IntegrationConfigurationBuilder WithGitHub(bool enabled = true)
    {
        return (IntegrationConfigurationBuilder)SetProperty("GitHubEnabled", enabled);
    }

    /// <summary>
    /// Enables GitHub integration.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public IntegrationConfigurationBuilder EnableGitHub() => WithGitHub(true);

    /// <summary>
    /// Enables or disables Slack integration.
    /// </summary>
    /// <param name="enabled">True to enable Slack</param>
    /// <returns>The builder for chaining</returns>
    public IntegrationConfigurationBuilder WithSlack(bool enabled = true)
    {
        return (IntegrationConfigurationBuilder)SetProperty("SlackEnabled", enabled);
    }

    /// <summary>
    /// Enables Slack integration.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public IntegrationConfigurationBuilder EnableSlack() => WithSlack(true);

    /// <summary>
    /// Enables or disables Google Drive integration.
    /// </summary>
    /// <param name="enabled">True to enable Google Drive</param>
    /// <returns>The builder for chaining</returns>
    public IntegrationConfigurationBuilder WithGoogleDrive(bool enabled = true)
    {
        return (IntegrationConfigurationBuilder)SetProperty("GoogleDriveEnabled", enabled);
    }

    /// <summary>
    /// Enables Google Drive integration.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public IntegrationConfigurationBuilder EnableGoogleDrive() => WithGoogleDrive(true);

    /// <summary>
    /// Enables or disables email integration.
    /// </summary>
    /// <param name="enabled">True to enable email</param>
    /// <returns>The builder for chaining</returns>
    public IntegrationConfigurationBuilder WithEmail(bool enabled = true)
    {
        return (IntegrationConfigurationBuilder)SetProperty("EmailEnabled", enabled);
    }

    /// <summary>
    /// Enables email integration.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public IntegrationConfigurationBuilder EnableEmail() => WithEmail(true);

    /// <summary>
    /// Disables all integrations.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public IntegrationConfigurationBuilder DisableAll()
    {
        SetProperty("ZoomEnabled", false);
        SetProperty("GitHubEnabled", false);
        SetProperty("SlackEnabled", false);
        SetProperty("GoogleDriveEnabled", false);
        SetProperty("EmailEnabled", false);
        return this;
    }

    protected override IntegrationConfiguration CreateConfiguration()
    {
        return new IntegrationConfiguration
        {
            ZoomEnabled = GetProperty<bool>("ZoomEnabled"),
            GitHubEnabled = GetProperty<bool>("GitHubEnabled"),
            SlackEnabled = GetProperty<bool>("SlackEnabled"),
            GoogleDriveEnabled = GetProperty<bool>("GoogleDriveEnabled"),
            EmailEnabled = GetProperty<bool>("EmailEnabled")
        };
    }
}