using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class SpaceFluentCreateRequest
{
    private string? _name;
    private bool? _multipleAssignees;
    private bool? _dueDatesEnabled;
    private bool? _timeTrackingEnabled;
    private bool? _tagsEnabled;
    private bool? _prioritiesEnabled;
    private bool? _customFieldsEnabled;
    private bool? _remapDependenciesEnabled;
    private bool? _dependencyWarningEnabled;
    private bool? _portfoliosEnabled;
    private bool? _sprintsEnabled;
    private bool? _pointsEnabled;
    private bool? _customTaskIdsEnabled;
    private bool? _rescheduleDependenciesEnabled;
    private bool? _weeklyProgressEnabled;
    private bool? _ganttEnabled;
    private bool? _timeEstimatesEnabled;
    private bool? _milestonesEnabled;
    private bool? _checklistsEnabled;
    private bool? _zoomEnabled;
    private bool? _googleDriveEnabled;
    private bool? _slackEnabled;
    private bool? _githubEnabled;
    private bool? _gitlabEnabled;
    private bool? _bitbucketEnabled;
    private bool? _emailEnabled;

    private readonly string _workspaceId;
    private readonly ISpacesService _spacesService;

    public SpaceFluentCreateRequest(string workspaceId, ISpacesService spacesService)
    {
        _workspaceId = workspaceId;
        _spacesService = spacesService;
    }

    public SpaceFluentCreateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public SpaceFluentCreateRequest WithMultipleAssignees(bool multipleAssignees)
    {
        _multipleAssignees = multipleAssignees;
        return this;
    }

    public SpaceFluentCreateRequest WithDueDatesEnabled(bool enabled)
    {
        _dueDatesEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithTimeTrackingEnabled(bool enabled)
    {
        _timeTrackingEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithTagsEnabled(bool enabled)
    {
        _tagsEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithPrioritiesEnabled(bool enabled)
    {
        _prioritiesEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithCustomFieldsEnabled(bool enabled)
    {
        _customFieldsEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithRemapDependenciesEnabled(bool enabled)
    {
        _remapDependenciesEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithDependencyWarningEnabled(bool enabled)
    {
        _dependencyWarningEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithPortfoliosEnabled(bool enabled)
    {
        _portfoliosEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithSprintsEnabled(bool enabled)
    {
        _sprintsEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithPointsEnabled(bool enabled)
    {
        _pointsEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithCustomTaskIdsEnabled(bool enabled)
    {
        _customTaskIdsEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithRescheduleDependenciesEnabled(bool enabled)
    {
        _rescheduleDependenciesEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithWeeklyProgressEnabled(bool enabled)
    {
        _weeklyProgressEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithGanttEnabled(bool enabled)
    {
        _ganttEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithTimeEstimatesEnabled(bool enabled)
    {
        _timeEstimatesEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithMilestonesEnabled(bool enabled)
    {
        _milestonesEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithChecklistsEnabled(bool enabled)
    {
        _checklistsEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithZoomEnabled(bool enabled)
    {
        _zoomEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithGoogleDriveEnabled(bool enabled)
    {
        _googleDriveEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithSlackEnabled(bool enabled)
    {
        _slackEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithGithubEnabled(bool enabled)
    {
        _githubEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithGitlabEnabled(bool enabled)
    {
        _gitlabEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithBitbucketEnabled(bool enabled)
    {
        _bitbucketEnabled = enabled;
        return this;
    }

    public SpaceFluentCreateRequest WithEmailEnabled(bool enabled)
    {
        _emailEnabled = enabled;
        return this;
    }

    public async Task<Space> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var features = new Features(
            DueDates: _dueDatesEnabled.HasValue ? new DueDatesFeature(Enabled: _dueDatesEnabled.Value, StartDateEnabled: null, RemapDueDatesEnabled: null, DueDatesForSubtasksRollUpEnabled: null) : null,
            Sprints: _sprintsEnabled.HasValue ? new SprintsFeature(Enabled: _sprintsEnabled.Value, LegacySprintsEnabled: null) : null,
            Points: _pointsEnabled.HasValue ? new PointsFeature(Enabled: _pointsEnabled.Value) : null,
            CustomTaskIds: _customTaskIdsEnabled.HasValue ? new CustomTaskIdsFeature(Enabled: _customTaskIdsEnabled.Value) : null,
            TimeTracking: _timeTrackingEnabled.HasValue ? new TimeTrackingFeature(Enabled: _timeTrackingEnabled.Value, HarvestEnabled: null, RollUpEnabled: null) : null,
            Tags: _tagsEnabled.HasValue ? new TagsFeature(Enabled: _tagsEnabled.Value) : null,
            TimeEstimates: _timeEstimatesEnabled.HasValue ? new TimeEstimatesFeature(Enabled: _timeEstimatesEnabled.Value, RollUpEnabled: null, PerAssigneeEnabled: null) : null,
            Checklists: _checklistsEnabled.HasValue ? new ChecklistsFeature(Enabled: _checklistsEnabled.Value) : null,
            CustomFields: _customFieldsEnabled.HasValue ? new CustomFieldsFeature(Enabled: _customFieldsEnabled.Value) : null,
            RemapDependencies: _remapDependenciesEnabled.HasValue ? new RemapDependenciesFeature(Enabled: _remapDependenciesEnabled.Value) : null,
            DependencyWarning: _dependencyWarningEnabled.HasValue ? new DependencyWarningFeature(Enabled: _dependencyWarningEnabled.Value) : null,
            MultipleAssignees: _multipleAssignees.HasValue ? new MultipleAssigneesFeature(Enabled: _multipleAssignees.Value) : null,
            Portfolios: _portfoliosEnabled.HasValue ? new PortfoliosFeature(Enabled: _portfoliosEnabled.Value) : null,
            Emails: _emailEnabled.HasValue ? new EmailsFeature(Enabled: _emailEnabled.Value) : null
        );

        var createSpaceRequest = new CreateSpaceRequest(
            Name: _name ?? string.Empty,
            MultipleAssignees: _multipleAssignees,
            Features: features
        );

        return await _spacesService.CreateSpaceAsync(
            _workspaceId,
            createSpaceRequest,
            cancellationToken
        );
    }
}