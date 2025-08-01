using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent.Enhanced;
using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.Entities.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent.Composition;

/// <summary>
/// Demonstrates fluent API composition patterns for complex multi-step operations.
/// </summary>
public class FluentApiComposition
{
    private readonly ISpacesService _spacesService;
    private readonly IListsService _listsService;
    private readonly ITasksService _tasksService;

    public FluentApiComposition(
        ISpacesService spacesService,
        IListsService listsService,
        ITasksService tasksService)
    {
        _spacesService = spacesService ?? throw new ArgumentNullException(nameof(spacesService));
        _listsService = listsService ?? throw new ArgumentNullException(nameof(listsService));
        _tasksService = tasksService ?? throw new ArgumentNullException(nameof(tasksService));
    }

    /// <summary>
    /// Creates a complete project setup with space, lists, and initial tasks.
    /// </summary>
    /// <param name="workspaceId">The workspace ID</param>
    /// <returns>A fluent project setup builder</returns>
    public ProjectSetupBuilder CreateProject(string workspaceId)
    {
        return new ProjectSetupBuilder(workspaceId, _spacesService, _listsService, _tasksService);
    }

    /// <summary>
    /// Creates a development team setup with predefined structure.
    /// </summary>
    /// <param name="workspaceId">The workspace ID</param>
    /// <returns>A fluent development team setup builder</returns>
    public DevelopmentTeamSetupBuilder CreateDevelopmentTeam(string workspaceId)
    {
        return new DevelopmentTeamSetupBuilder(workspaceId, _spacesService, _listsService, _tasksService);
    }

    /// <summary>
    /// Creates a marketing campaign setup with predefined structure.
    /// </summary>
    /// <param name="workspaceId">The workspace ID</param>
    /// <returns>A fluent marketing campaign setup builder</returns>
    public MarketingCampaignSetupBuilder CreateMarketingCampaign(string workspaceId)
    {
        return new MarketingCampaignSetupBuilder(workspaceId, _spacesService, _listsService, _tasksService);
    }
}

/// <summary>
/// Fluent builder for setting up a complete project with space, lists, and tasks.
/// </summary>
public class ProjectSetupBuilder
{
    private readonly string _workspaceId;
    private readonly ISpacesService _spacesService;
    private readonly IListsService _listsService;
    private readonly ITasksService _tasksService;

    private string? _projectName;
    private readonly List<string> _listNames = new();
    private readonly List<TaskTemplate> _taskTemplates = new();
    private bool _enableTimeTracking = false;
    private bool _enableCustomFields = false;
    private bool _enableGitHubIntegration = false;

    internal ProjectSetupBuilder(
        string workspaceId,
        ISpacesService spacesService,
        IListsService listsService,
        ITasksService tasksService)
    {
        _workspaceId = workspaceId;
        _spacesService = spacesService;
        _listsService = listsService;
        _tasksService = tasksService;
    }

    /// <summary>
    /// Sets the project name.
    /// </summary>
    /// <param name="name">The project name</param>
    /// <returns>The builder for chaining</returns>
    public ProjectSetupBuilder WithName(string name)
    {
        _projectName = name ?? throw new ArgumentNullException(nameof(name));
        return this;
    }

    /// <summary>
    /// Adds lists to the project.
    /// </summary>
    /// <param name="listNames">The list names to create</param>
    /// <returns>The builder for chaining</returns>
    public ProjectSetupBuilder WithLists(params string[] listNames)
    {
        _listNames.AddRange(listNames ?? throw new ArgumentNullException(nameof(listNames)));
        return this;
    }

    /// <summary>
    /// Adds standard development lists (Backlog, In Progress, Review, Done).
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public ProjectSetupBuilder WithDevelopmentLists()
    {
        return WithLists("Backlog", "In Progress", "Review", "Done");
    }

    /// <summary>
    /// Adds standard marketing lists (Ideas, Planning, In Progress, Review, Published).
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public ProjectSetupBuilder WithMarketingLists()
    {
        return WithLists("Ideas", "Planning", "In Progress", "Review", "Published");
    }

    /// <summary>
    /// Adds task templates to be created in the first list.
    /// </summary>
    /// <param name="templates">The task templates</param>
    /// <returns>The builder for chaining</returns>
    public ProjectSetupBuilder WithTaskTemplates(params TaskTemplate[] templates)
    {
        _taskTemplates.AddRange(templates ?? throw new ArgumentNullException(nameof(templates)));
        return this;
    }

    /// <summary>
    /// Enables time tracking for the project.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public ProjectSetupBuilder EnableTimeTracking()
    {
        _enableTimeTracking = true;
        return this;
    }

    /// <summary>
    /// Enables custom fields for the project.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public ProjectSetupBuilder EnableCustomFields()
    {
        _enableCustomFields = true;
        return this;
    }

    /// <summary>
    /// Enables GitHub integration for the project.
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public ProjectSetupBuilder EnableGitHubIntegration()
    {
        _enableGitHubIntegration = true;
        return this;
    }

    /// <summary>
    /// Creates the complete project setup.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The project setup result</returns>
    public async Task<ProjectSetupResult> CreateAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_projectName))
            throw new InvalidOperationException("Project name is required.");

        // Step 1: Create the space
        var spaceBuilder = new SpaceFluentCreateRequestEnhanced(_workspaceId, _spacesService)
            .WithName(_projectName);

        if (_enableTimeTracking) spaceBuilder.EnableTimeTracking();
        if (_enableCustomFields) spaceBuilder.EnableCustomFields();
        if (_enableGitHubIntegration) spaceBuilder.EnableGitHubIntegration();

        var space = await spaceBuilder.CreateAsync(cancellationToken);

        // Step 2: Create lists
        var lists = new List<ClickUp.Api.Client.Models.Entities.Lists.ClickUpList>();
        foreach (var listName in _listNames)
        {
            var list = await _listsService.CreateFolderlessListAsync(
                space.Id,
                new ClickUp.Api.Client.Models.RequestModels.Lists.CreateListRequest(
                    Name: listName,
                    Content: null,
                    MarkdownContent: null,
                    DueDate: null,
                    DueDateTime: null,
                    Priority: null,
                    Assignee: null,
                    Status: null
                ),
                cancellationToken
            );
            lists.Add(list);
        }

        // Step 3: Create initial tasks in the first list
        var tasks = new List<ClickUp.Api.Client.Models.Entities.Tasks.CuTask>();
        if (lists.Any() && _taskTemplates.Any())
        {
            var firstList = lists.First();
            foreach (var template in _taskTemplates)
            {
                var task = await _tasksService.CreateTaskAsync(
                    firstList.Id,
                    new ClickUp.Api.Client.Models.RequestModels.Tasks.CreateTaskRequest(
                        Name: template.Name,
                        Description: template.Description,
                        Assignees: null,
                        GroupAssignees: null,
                        Tags: null,
                        Status: null,
                        Priority: template.Priority,
                        DueDate: null,
                        DueDateTime: null,
                        TimeEstimate: null,
                        StartDate: null,
                        StartDateTime: null,
                        NotifyAll: null,
                        Parent: null,
                        LinksTo: null,
                        CheckRequiredCustomFields: null,
                        CustomFields: null,
                        CustomItemId: null,
                        ListId: null
                    ),
                    customTaskIds: null,
                    teamId: null,
                    cancellationToken: cancellationToken
                );
                tasks.Add(task);
            }
        }

        return new ProjectSetupResult(space, lists.AsReadOnly(), tasks.AsReadOnly());
    }
}

/// <summary>
/// Fluent builder for setting up a development team with predefined structure.
/// </summary>
public class DevelopmentTeamSetupBuilder
{
    private readonly ProjectSetupBuilder _projectBuilder;
    private string? _teamName;
    private string? _repositoryUrl;

    internal DevelopmentTeamSetupBuilder(
        string workspaceId,
        ISpacesService spacesService,
        IListsService listsService,
        ITasksService tasksService)
    {
        _projectBuilder = new ProjectSetupBuilder(workspaceId, spacesService, listsService, tasksService);
    }

    /// <summary>
    /// Sets the team name.
    /// </summary>
    /// <param name="name">The team name</param>
    /// <returns>The builder for chaining</returns>
    public DevelopmentTeamSetupBuilder WithTeamName(string name)
    {
        _teamName = name ?? throw new ArgumentNullException(nameof(name));
        _projectBuilder.WithName($"{name} Development");
        return this;
    }

    /// <summary>
    /// Sets the repository URL for GitHub integration.
    /// </summary>
    /// <param name="url">The repository URL</param>
    /// <returns>The builder for chaining</returns>
    public DevelopmentTeamSetupBuilder WithRepository(string url)
    {
        _repositoryUrl = url ?? throw new ArgumentNullException(nameof(url));
        return this;
    }

    /// <summary>
    /// Creates the development team setup with predefined configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The project setup result</returns>
    public async Task<ProjectSetupResult> CreateAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_teamName))
            throw new InvalidOperationException("Team name is required.");

        _projectBuilder
            .WithDevelopmentLists()
            .EnableTimeTracking()
            .EnableCustomFields()
            .EnableGitHubIntegration()
            .WithTaskTemplates(
                new TaskTemplate("Setup Development Environment", "Configure local development environment and tools", 2),
                new TaskTemplate("Code Review Guidelines", "Establish code review process and guidelines", 1),
                new TaskTemplate("CI/CD Pipeline", "Setup continuous integration and deployment pipeline", 3),
                new TaskTemplate("Documentation", "Create project documentation and README", 1)
            );

        return await _projectBuilder.CreateAsync(cancellationToken);
    }
}

/// <summary>
/// Fluent builder for setting up a marketing campaign with predefined structure.
/// </summary>
public class MarketingCampaignSetupBuilder
{
    private readonly ProjectSetupBuilder _projectBuilder;
    private string? _campaignName;
    private DateTime? _launchDate;

    internal MarketingCampaignSetupBuilder(
        string workspaceId,
        ISpacesService spacesService,
        IListsService listsService,
        ITasksService tasksService)
    {
        _projectBuilder = new ProjectSetupBuilder(workspaceId, spacesService, listsService, tasksService);
    }

    /// <summary>
    /// Sets the campaign name.
    /// </summary>
    /// <param name="name">The campaign name</param>
    /// <returns>The builder for chaining</returns>
    public MarketingCampaignSetupBuilder WithCampaignName(string name)
    {
        _campaignName = name ?? throw new ArgumentNullException(nameof(name));
        _projectBuilder.WithName($"{name} Campaign");
        return this;
    }

    /// <summary>
    /// Sets the campaign launch date.
    /// </summary>
    /// <param name="date">The launch date</param>
    /// <returns>The builder for chaining</returns>
    public MarketingCampaignSetupBuilder WithLaunchDate(DateTime date)
    {
        _launchDate = date;
        return this;
    }

    /// <summary>
    /// Creates the marketing campaign setup with predefined configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The project setup result</returns>
    public async Task<ProjectSetupResult> CreateAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_campaignName))
            throw new InvalidOperationException("Campaign name is required.");

        _projectBuilder
            .WithMarketingLists()
            .EnableCustomFields()
            .WithTaskTemplates(
                new TaskTemplate("Market Research", "Conduct target audience and competitor analysis", 2),
                new TaskTemplate("Content Strategy", "Develop content strategy and messaging framework", 3),
                new TaskTemplate("Creative Assets", "Design and create marketing materials", 2),
                new TaskTemplate("Campaign Launch", $"Launch campaign{(_launchDate.HasValue ? $" on {_launchDate.Value:yyyy-MM-dd}" : "")}", 3),
                new TaskTemplate("Performance Analysis", "Monitor and analyze campaign performance", 1)
            );

        return await _projectBuilder.CreateAsync(cancellationToken);
    }
}

/// <summary>
/// Represents a task template for initial project setup.
/// </summary>
public record TaskTemplate(string Name, string Description, int Priority);

/// <summary>
/// Represents the result of a project setup operation.
/// </summary>
public record ProjectSetupResult(
    Space Space,
    IReadOnlyList<ClickUp.Api.Client.Models.Entities.Lists.ClickUpList> Lists,
    IReadOnlyList<ClickUp.Api.Client.Models.Entities.Tasks.CuTask> Tasks
);

/// <summary>
/// Extension methods for fluent API composition.
/// </summary>
public static class FluentCompositionExtensions
{
    /// <summary>
    /// Creates a fluent API composition instance.
    /// </summary>
    /// <param name="spacesService">The spaces service</param>
    /// <param name="listsService">The lists service</param>
    /// <param name="tasksService">The tasks service</param>
    /// <returns>The fluent API composition instance</returns>
    public static FluentApiComposition CreateComposition(
        this ISpacesService spacesService,
        IListsService listsService,
        ITasksService tasksService)
    {
        return new FluentApiComposition(spacesService, listsService, tasksService);
    }
}