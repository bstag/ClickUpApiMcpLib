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

    public FluentAttachmentsApi Attachments => new FluentAttachmentsApi(_attachmentsService);
    public FluentAuthorizationApi Authorization => new FluentAuthorizationApi(_authorizationService);
    public FluentChatApi Chat => new FluentChatApi(_chatService);
    public FluentCommentApi Comments => new FluentCommentApi(_commentService);
    public FluentCustomFieldsApi CustomFields => new FluentCustomFieldsApi(_customFieldsService);
    public FluentDocsApi Docs => new FluentDocsApi(_docsService);
    public FluentFoldersApi Folders => new FluentFoldersApi(_foldersService);
    public FluentGoalsApi Goals => new FluentGoalsApi(_goalsService);
    public FluentGuestsApi Guests => new FluentGuestsApi(_guestsService);
    public FluentListsApi Lists => new FluentListsApi(_listsService);
    public FluentMembersApi Members => new FluentMembersApi(_membersService);
    public FluentRolesApi Roles => new FluentRolesApi(_rolesService);
    public FluentSharedHierarchyApi SharedHierarchy => new FluentSharedHierarchyApi(_sharedHierarchyService);
    public FluentSpacesApi Spaces => new FluentSpacesApi(_spacesService);
    public FluentTagsApi Tags => new FluentTagsApi(_tagsService);
    public FluentTaskChecklistsApi TaskChecklists => new FluentTaskChecklistsApi(_taskChecklistsService);
    public FluentTaskRelationshipsApi TaskRelationships => new FluentTaskRelationshipsApi(_taskRelationshipsService);
    public FluentTasksApi Tasks => new FluentTasksApi(_tasksService);
    public FluentTemplatesApi Templates => new FluentTemplatesApi(_templatesService);
    public FluentTimeTrackingApi TimeTracking => new FluentTimeTrackingApi(_timeTrackingService);
    public FluentUserGroupsApi UserGroupsApi => new FluentUserGroupsApi(_userGroupsService);
    public FluentUsersApi Users => new FluentUsersApi(_usersService);
    public FluentViewsApi Views => new FluentViewsApi(_viewsService);
    public FluentWebhooksApi Webhooks => new FluentWebhooksApi(_webhooksService);
    public FluentWorkspacesApi Workspaces => new FluentWorkspacesApi(_workspacesService);

    public static ClickUpClient Create(string apiToken, ILoggerFactory loggerFactory)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(apiToken);
        var apiConnection = new ApiConnection(httpClient);
        return new ClickUpClient(apiConnection, loggerFactory);
    }
}
