using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentCreateSpaceRequestTests
{
    private readonly Mock<ISpacesService> _mockSpacesService;

    public FluentCreateSpaceRequestTests()
    {
        _mockSpacesService = new Mock<ISpacesService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateSpaceAsyncWithCorrectParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var name = "testSpaceName";
        var multipleAssignees = true;
        var dueDatesEnabled = true;
        var timeTrackingEnabled = true;
        var tagsEnabled = true;
        var prioritiesEnabled = true;
        var customFieldsEnabled = true;
        var remapDependenciesEnabled = true;
        var dependencyWarningEnabled = true;
        var portfoliosEnabled = true;
        var sprintsEnabled = true;
        var pointsEnabled = true;
        var customTaskIdsEnabled = true;
        var rescheduleDependenciesEnabled = true;
        var weeklyProgressEnabled = true;
        var ganttEnabled = true;
        var timeEstimatesEnabled = true;
        var milestonesEnabled = true;
        var checklistsEnabled = true;
        var zoomEnabled = true;
        var googleDriveEnabled = true;
        var slackEnabled = true;
        var githubEnabled = true;
        var gitlabEnabled = true;
        var bitbucketEnabled = true;
        var emailEnabled = true;

        var expectedSpace = new Space(
            Id: "spaceId",
            Name: name,
            Private: false,
            Color: null,
            Avatar: null,
            AdminCanManage: null,
            Archived: null,
            Members: null,
            Statuses: null,
            MultipleAssignees: multipleAssignees,
            Features: new Features(
                DueDates: new DueDatesFeature(dueDatesEnabled, null, null, null),
                Sprints: new SprintsFeature(sprintsEnabled, null),
                Points: new PointsFeature(pointsEnabled),
                CustomTaskIds: new CustomTaskIdsFeature(customTaskIdsEnabled),
                TimeTracking: new TimeTrackingFeature(timeTrackingEnabled, null, null),
                Tags: new TagsFeature(tagsEnabled),
                TimeEstimates: new TimeEstimatesFeature(timeEstimatesEnabled, null, null),
                Checklists: new ChecklistsFeature(checklistsEnabled),
                CustomFields: new CustomFieldsFeature(customFieldsEnabled),
                RemapDependencies: new RemapDependenciesFeature(remapDependenciesEnabled),
                DependencyWarning: new DependencyWarningFeature(dependencyWarningEnabled),
                MultipleAssignees: new MultipleAssigneesFeature(multipleAssignees),
                Portfolios: new PortfoliosFeature(portfoliosEnabled),
                Emails: new EmailsFeature(emailEnabled)
            ),
            TeamId: workspaceId,
            DefaultListSettings: null
        );

        _mockSpacesService.Setup(x => x.CreateSpaceAsync(
            It.IsAny<string>(),
            It.IsAny<CreateSpaceRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpace);

        var fluentRequest = new FluentCreateSpaceRequest(workspaceId, _mockSpacesService.Object)
            .WithName(name)
            .WithMultipleAssignees(multipleAssignees)
            .WithDueDatesEnabled(dueDatesEnabled)
            .WithTimeTrackingEnabled(timeTrackingEnabled)
            .WithTagsEnabled(tagsEnabled)
            .WithPrioritiesEnabled(prioritiesEnabled)
            .WithCustomFieldsEnabled(customFieldsEnabled)
            .WithRemapDependenciesEnabled(remapDependenciesEnabled)
            .WithDependencyWarningEnabled(dependencyWarningEnabled)
            .WithPortfoliosEnabled(portfoliosEnabled)
            .WithSprintsEnabled(sprintsEnabled)
            .WithPointsEnabled(pointsEnabled)
            .WithCustomTaskIdsEnabled(customTaskIdsEnabled)
            .WithRescheduleDependenciesEnabled(rescheduleDependenciesEnabled)
            .WithWeeklyProgressEnabled(weeklyProgressEnabled)
            .WithGanttEnabled(ganttEnabled)
            .WithTimeEstimatesEnabled(timeEstimatesEnabled)
            .WithMilestonesEnabled(milestonesEnabled)
            .WithChecklistsEnabled(checklistsEnabled)
            .WithZoomEnabled(zoomEnabled)
            .WithGoogleDriveEnabled(googleDriveEnabled)
            .WithSlackEnabled(slackEnabled)
            .WithGithubEnabled(githubEnabled)
            .WithGitlabEnabled(gitlabEnabled)
            .WithBitbucketEnabled(bitbucketEnabled)
            .WithEmailEnabled(emailEnabled);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedSpace, result);
        _mockSpacesService.Verify(x => x.CreateSpaceAsync(
            workspaceId,
            It.Is<CreateSpaceRequest>(req =>
                req.Name == name &&
                req.MultipleAssignees == multipleAssignees &&
                req.Features.DueDates!.Enabled == dueDatesEnabled &&
                req.Features.TimeTracking!.Enabled == timeTrackingEnabled &&
                req.Features.Tags!.Enabled == tagsEnabled &&
                req.Features.CustomFields!.Enabled == customFieldsEnabled &&
                req.Features.RemapDependencies!.Enabled == remapDependenciesEnabled &&
                req.Features.DependencyWarning!.Enabled == dependencyWarningEnabled &&
                req.Features.Portfolios!.Enabled == portfoliosEnabled &&
                req.Features.Sprints!.Enabled == sprintsEnabled &&
                req.Features.Points!.Enabled == pointsEnabled &&
                req.Features.CustomTaskIds!.Enabled == customTaskIdsEnabled &&
                req.Features.Checklists!.Enabled == checklistsEnabled &&
                req.Features.TimeEstimates!.Enabled == timeEstimatesEnabled &&
                req.Features.Emails!.Enabled == emailEnabled
            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateSpaceAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var name = "testSpaceName";

        var expectedSpace = new Space(
            Id: "spaceId",
            Name: name,
            Private: false,
            Color: null,
            Avatar: null,
            AdminCanManage: null,
            Archived: null,
            Members: null,
            Statuses: null,
            MultipleAssignees: false,
            Features: new Features(
                DueDates: null,
                Sprints: null,
                Points: null,
                CustomTaskIds: null,
                TimeTracking: null,
                Tags: null,
                TimeEstimates: null,
                Checklists: null,
                CustomFields: null,
                RemapDependencies: null,
                DependencyWarning: null,
                MultipleAssignees: null,
                Portfolios: null,
                Emails: null
            ),
            TeamId: workspaceId,
            DefaultListSettings: null
        );

        _mockSpacesService.Setup(x => x.CreateSpaceAsync(
            It.IsAny<string>(),
            It.IsAny<CreateSpaceRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpace);

        var fluentRequest = new FluentCreateSpaceRequest(workspaceId, _mockSpacesService.Object)
            .WithName(name);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedSpace, result);
        _mockSpacesService.Verify(x => x.CreateSpaceAsync(
            workspaceId,
            It.Is<CreateSpaceRequest>(req =>
                req.Name == name &&
                req.MultipleAssignees == null &&
                req.Features.DueDates == null &&
                req.Features.TimeTracking == null &&
                req.Features.Tags == null &&
                req.Features.CustomFields == null &&
                req.Features.RemapDependencies == null &&
                req.Features.DependencyWarning == null &&
                req.Features.Portfolios == null &&
                req.Features.Sprints == null &&
                req.Features.Points == null &&
                req.Features.CustomTaskIds == null &&
                req.Features.Checklists == null &&
                req.Features.TimeEstimates == null &&
                req.Features.Emails == null
            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
