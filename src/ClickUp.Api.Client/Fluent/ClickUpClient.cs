using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Http;
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace ClickUp.Api.Client.Fluent;

public class ClickUpClient
{
    private readonly IApiConnection _apiConnection;
    private readonly IAttachmentsService _attachmentsService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IChatService _chatService;
    private readonly ICommentsService _commentService;
    private readonly ICustomFieldsService _customFieldsService;
    private readonly IDocsService _docsService;
    private readonly IFoldersService _foldersService;
    private readonly IGoalsService _goalsService;
    private readonly IGuestsService _guestsService;
    private readonly IListsService _listsService;
    private readonly IMembersService _membersService;
    private readonly IRolesService _rolesService;
    private readonly ISharedHierarchyService _sharedHierarchyService;
    private readonly ISpacesService _spacesService;
    private readonly ITagsService _tagsService;
    private readonly ITaskChecklistsService _taskChecklistsService;
    private readonly ITaskRelationshipsService _taskRelationshipsService;
    private readonly ITasksService _tasksService;
    private readonly ITemplatesService _templatesService;
    private readonly ITimeTrackingService _timeTrackingService;
    private readonly IUserGroupsService _userGroupsService;
    private readonly IUsersService _usersService;
    private readonly IViewsService _viewsService;
    private readonly IWebhooksService _webhooksService;
    private readonly IWorkspacesService _workspacesService;

    public ClickUpClient(IApiConnection apiConnection, ILoggerFactory loggerFactory)
    {
        _apiConnection = apiConnection;
        _attachmentsService = new AttachmentsService(_apiConnection, loggerFactory.CreateLogger<AttachmentsService>());
        _authorizationService = new AuthorizationService(_apiConnection, loggerFactory.CreateLogger<AuthorizationService>());
        _chatService = new ChatService(_apiConnection, loggerFactory.CreateLogger<ChatService>());
        _commentService = new CommentService(_apiConnection, loggerFactory.CreateLogger<CommentService>());
        _customFieldsService = new CustomFieldsService(_apiConnection, loggerFactory.CreateLogger<CustomFieldsService>());
        _docsService = new DocsService(_apiConnection, loggerFactory.CreateLogger<DocsService>());
        _foldersService = new FoldersService(_apiConnection, loggerFactory.CreateLogger<FoldersService>());
        _goalsService = new GoalsService(_apiConnection, loggerFactory.CreateLogger<GoalsService>());
        _guestsService = new GuestsService(_apiConnection, loggerFactory.CreateLogger<GuestsService>());
        _listsService = new ListsService(_apiConnection, loggerFactory.CreateLogger<ListsService>());
        _membersService = new MembersService(_apiConnection, loggerFactory.CreateLogger<MembersService>());
        _rolesService = new RolesService(_apiConnection, loggerFactory.CreateLogger<RolesService>());
        _sharedHierarchyService = new SharedHierarchyService(_apiConnection, loggerFactory.CreateLogger<SharedHierarchyService>());
        _spacesService = new SpacesService(_apiConnection, loggerFactory.CreateLogger<SpacesService>());
        _tagsService = new TagsService(_apiConnection, loggerFactory.CreateLogger<TagsService>());
        _taskChecklistsService = new TaskChecklistsService(_apiConnection, loggerFactory.CreateLogger<TaskChecklistsService>());
        _taskRelationshipsService = new TaskRelationshipsService(_apiConnection, loggerFactory.CreateLogger<TaskRelationshipsService>());
        _tasksService = new TaskService(_apiConnection, loggerFactory.CreateLogger<TaskService>());
        _templatesService = new TemplatesService(_apiConnection, loggerFactory.CreateLogger<TemplatesService>());
        _timeTrackingService = new TimeTrackingService(_apiConnection, loggerFactory.CreateLogger<TimeTrackingService>());
        _userGroupsService = new UserGroupsService(_apiConnection, loggerFactory.CreateLogger<UserGroupsService>());
        _usersService = new UsersService(_apiConnection, loggerFactory.CreateLogger<UsersService>());
        _viewsService = new ViewsService(_apiConnection, loggerFactory.CreateLogger<ViewsService>());
        _webhooksService = new WebhooksService(_apiConnection, loggerFactory.CreateLogger<WebhooksService>());
        _workspacesService = new WorkspacesService(_apiConnection, loggerFactory.CreateLogger<WorkspacesService>());
    }

    public AttachmentsFluentApi Attachments => new AttachmentsFluentApi(_attachmentsService);
    public AuthorizationFluentApi Authorization => new AuthorizationFluentApi(_authorizationService);
    public ChatFluentApi Chat => new ChatFluentApi(_chatService);
    public CommentFluentApi Comments => new CommentFluentApi(_commentService);
    public CustomFieldsFluentApi CustomFields => new CustomFieldsFluentApi(_customFieldsService);
    public DocsFluentApi Docs => new DocsFluentApi(_docsService);
    public FoldersFluentApi Folders => new FoldersFluentApi(_foldersService);
    public GoalsFluentApi Goals => new GoalsFluentApi(_goalsService);
    public GuestsFluentApi Guests => new GuestsFluentApi(_guestsService);
    public ListsFluentApi Lists => new ListsFluentApi(_listsService);
    public MembersFluentApi Members => new MembersFluentApi(_membersService);
    public RolesFluentApi Roles => new RolesFluentApi(_rolesService);
    public SharedHierarchyFluentApi SharedHierarchy => new SharedHierarchyFluentApi(_sharedHierarchyService);
    public SpacesFluentApi Spaces => new SpacesFluentApi(_spacesService);
    public TagsFluentApi Tags => new TagsFluentApi(_tagsService);
    public TaskChecklistsFluentApi TaskChecklists => new TaskChecklistsFluentApi(_taskChecklistsService);
    public TaskRelationshipsFluentApi TaskRelationships => new TaskRelationshipsFluentApi(_taskRelationshipsService);
    public TasksFluentApi Tasks => new TasksFluentApi(_tasksService);
    public TemplatesFluentApi Templates => new TemplatesFluentApi(_templatesService);
    public TimeTrackingFluentApi TimeTracking => new TimeTrackingFluentApi(_timeTrackingService);
    public UserGroupsFluentApi UserGroupsApi => new UserGroupsFluentApi(_userGroupsService);
    public UsersFluentApi Users => new UsersFluentApi(_usersService);
    public ViewsFluentApi Views => new ViewsFluentApi(_viewsService);
    public WebhooksFluentApi Webhooks => new WebhooksFluentApi(_webhooksService);
    public WorkspacesFluentApi Workspaces => new WorkspacesFluentApi(_workspacesService);

    public static ClickUpClient Create(string apiToken, ILoggerFactory loggerFactory)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(apiToken);
        var apiConnection = new ApiConnection(httpClient);
        return new ClickUpClient(apiConnection, loggerFactory);
    }
}
