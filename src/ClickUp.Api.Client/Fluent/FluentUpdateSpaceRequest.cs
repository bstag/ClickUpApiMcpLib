using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentUpdateSpaceRequest
{
    private string? _name;
    private string? _color;
    private bool? _private;
    private bool? _adminCanManage;
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
    private bool? _archived;

    private readonly string _spaceId;
    private readonly ISpacesService _spacesService;

    public FluentUpdateSpaceRequest(string spaceId, ISpacesService spacesService)
    {
        _spaceId = spaceId;
        _spacesService = spacesService;
    }

    public FluentUpdateSpaceRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public FluentUpdateSpaceRequest WithColor(string color)
    {
        _color = color;
        return this;
    }

    public FluentUpdateSpaceRequest WithPrivate(bool @private)
    {
        _private = @private;
        return this;
    }

    public FluentUpdateSpaceRequest WithAdminCanManage(bool adminCanManage)
    {
        _adminCanManage = adminCanManage;
        return this;
    }

    public FluentUpdateSpaceRequest WithMultipleAssignees(bool multipleAssignees)
    {
        _multipleAssignees = multipleAssignees;
        return this;
    }

    public FluentUpdateSpaceRequest WithDueDatesEnabled(bool enabled)
    {
        _dueDatesEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithTimeTrackingEnabled(bool enabled)
    {
        _timeTrackingEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithTagsEnabled(bool enabled)
    {
        _tagsEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithPrioritiesEnabled(bool enabled)
    {
        _prioritiesEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithCustomFieldsEnabled(bool enabled)
    {
        _customFieldsEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithRemapDependenciesEnabled(bool enabled)
    {
        _remapDependenciesEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithDependencyWarningEnabled(bool enabled)
    {
        _dependencyWarningEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithPortfoliosEnabled(bool enabled)
    {
        _portfoliosEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithSprintsEnabled(bool enabled)
    {
        _sprintsEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithPointsEnabled(bool enabled)
    {
        _pointsEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithCustomTaskIdsEnabled(bool enabled)
    {
        _customTaskIdsEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithRescheduleDependenciesEnabled(bool enabled)
    {
        _rescheduleDependenciesEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithWeeklyProgressEnabled(bool enabled)
    {
        _weeklyProgressEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithGanttEnabled(bool enabled)
    {
        _ganttEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithTimeEstimatesEnabled(bool enabled)
    {
        _timeEstimatesEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithMilestonesEnabled(bool enabled)
    {
        _milestonesEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithChecklistsEnabled(bool enabled)
    {
        _checklistsEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithZoomEnabled(bool enabled)
    {
        _zoomEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithGoogleDriveEnabled(bool enabled)
    {
        _googleDriveEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithSlackEnabled(bool enabled)
    {
        _slackEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithGithubEnabled(bool enabled)
    {
        _githubEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithGitlabEnabled(bool enabled)
    {
        _gitlabEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithBitbucketEnabled(bool enabled)
    {
        _bitbucketEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithEmailEnabled(bool enabled)
    {
        _emailEnabled = enabled;
        return this;
    }

    public FluentUpdateSpaceRequest WithArchived(bool archived)
    {
        _archived = archived;
        return this;
    }

    public async Task<Space> UpdateAsync(CancellationToken cancellationToken = default)
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

        var updateSpaceRequest = new UpdateSpaceRequest(
            Name: _name,
            Color: _color,
            Private: _private,
            AdminCanManage: _adminCanManage,
            MultipleAssignees: _multipleAssignees,
            Features: features,
            Archived: _archived
        );

        return await _spacesService.UpdateSpaceAsync(
            _spaceId,
            updateSpaceRequest,
            cancellationToken
        );
    }
}