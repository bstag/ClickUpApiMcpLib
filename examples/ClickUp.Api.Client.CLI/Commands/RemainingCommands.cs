using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

// Folder Commands
public class FolderCommands : BaseCommand
{
    private readonly IFoldersService _foldersService;

    public FolderCommands(IFoldersService foldersService, IOutputFormatter outputFormatter, ILogger<FolderCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _foldersService = foldersService;

    public override Command CreateCommand()
    {
        var folderCommand = new Command("folder", "Folder management commands")
        {
            CreateGetFolderCommand(),
            CreateListFoldersCommand()
        };
        return folderCommand;
    }

    private Command CreateGetFolderCommand()
    {
        var folderIdArgument = new Argument<long>("folder-id", "The folder ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getFolderCommand = new Command("get", "Get a specific folder by ID") { folderIdArgument, formatOption, propertiesOption };

        getFolderCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var folderId = context.ParseResult.GetValueForArgument(folderIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateNumericParameters(("folder-id", folderId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving folder {folderId}...");
                var folder = await _foldersService.GetFolderAsync(folderId.ToString());

                if (folder == null)
                {
                    Console.WriteLine(OutputFormatter.FormatWarning($"Folder {folderId} not found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(folder, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return getFolderCommand;
    }

    private Command CreateListFoldersCommand()
    {
        var spaceIdArgument = new Argument<long>("space-id", "The space ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();
        var includeArchivedOption = CreateIncludeArchivedOption();

        var listFoldersCommand = new Command("list", "List folders in a space") { spaceIdArgument, formatOption, propertiesOption, includeArchivedOption };

        listFoldersCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var spaceId = context.ParseResult.GetValueForArgument(spaceIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));
                var includeArchived = context.ParseResult.GetValueForOption(includeArchivedOption);

                if (!ValidateNumericParameters(("space-id", spaceId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving folders from space {spaceId}...");
                var folders = await _foldersService.GetFoldersAsync(spaceId.ToString(), includeArchived);

                if (folders == null || !folders.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No folders found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(folders, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listFoldersCommand;
    }
}

// List Commands
public class ListCommands : BaseCommand
{
    private readonly IListsService _listsService;

    public ListCommands(IListsService listsService, IOutputFormatter outputFormatter, ILogger<ListCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _listsService = listsService;

    public override Command CreateCommand()
    {
        var listCommand = new Command("list", "List management commands")
        {
            CreateGetListCommand(),
            CreateListListsCommand()
        };
        return listCommand;
    }

    private Command CreateGetListCommand()
    {
        var listIdArgument = new Argument<long>("list-id", "The list ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getListCommand = new Command("get", "Get a specific list by ID") { listIdArgument, formatOption, propertiesOption };

        getListCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var listId = context.ParseResult.GetValueForArgument(listIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateNumericParameters(("list-id", listId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving list {listId}...");
                var list = await _listsService.GetListAsync(listId.ToString());

                if (list == null)
                {
                    Console.WriteLine(OutputFormatter.FormatWarning($"List {listId} not found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(list, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return getListCommand;
    }

    private Command CreateListListsCommand()
    {
        var folderIdArgument = new Argument<long>("folder-id", "The folder ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();
        var includeArchivedOption = CreateIncludeArchivedOption();

        var listListsCommand = new Command("list-all", "List all lists in a folder") { folderIdArgument, formatOption, propertiesOption, includeArchivedOption };

        listListsCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var folderId = context.ParseResult.GetValueForArgument(folderIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));
                var includeArchived = context.ParseResult.GetValueForOption(includeArchivedOption);

                if (!ValidateNumericParameters(("folder-id", folderId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving lists from folder {folderId}...");
                var lists = await _listsService.GetListsInFolderAsync(folderId.ToString(), includeArchived);

                if (lists == null || !lists.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No lists found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(lists, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listListsCommand;
    }
}

// Comment Commands
public class CommentCommands : BaseCommand
{
    private readonly ICommentsService _commentService;

    public CommentCommands(ICommentsService commentService, IOutputFormatter outputFormatter, ILogger<CommentCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _commentService = commentService;

    public override Command CreateCommand()
    {
        var commentCommand = new Command("comment", "Comment management commands")
        {
            CreateGetCommentsCommand()
        };
        return commentCommand;
    }

    private Command CreateGetCommentsCommand()
    {
        var taskIdArgument = new Argument<string>("task-id", "The task ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getCommentsCommand = new Command("list", "Get comments for a task") { taskIdArgument, formatOption, propertiesOption };

        getCommentsCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var taskId = context.ParseResult.GetValueForArgument(taskIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateRequiredParameters(("task-id", taskId)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving comments for task {taskId}...");
                var request = new GetTaskCommentsRequest(taskId);
                var comments = await _commentService.GetTaskCommentsAsync(request);

                if (comments == null || !comments.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No comments found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(comments, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return getCommentsCommand;
    }
}

// Placeholder Commands for remaining services
public class MemberCommands : BaseCommand
{
    public MemberCommands(IOutputFormatter outputFormatter, ILogger<MemberCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) { }

    public override Command CreateCommand() => new Command("member", "Member management commands (not implemented)");
}

public class CustomFieldCommands : BaseCommand
{
    private readonly ICustomFieldsService _customFieldsService;

    public CustomFieldCommands(ICustomFieldsService customFieldsService, IOutputFormatter outputFormatter, ILogger<CustomFieldCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _customFieldsService = customFieldsService;

    public override Command CreateCommand() => new Command("custom-field", "Custom field management commands (not implemented)");
}

public class TagCommands : BaseCommand
{
    private readonly ITagsService _tagsService;

    public TagCommands(ITagsService tagsService, IOutputFormatter outputFormatter, ILogger<TagCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _tagsService = tagsService;

    public override Command CreateCommand() => new Command("tag", "Tag management commands (not implemented)");
}

public class ViewCommands : BaseCommand
{
    private readonly IViewsService _viewsService;

    public ViewCommands(IViewsService viewsService, IOutputFormatter outputFormatter, ILogger<ViewCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _viewsService = viewsService;

    public override Command CreateCommand() => new Command("view", "View management commands (not implemented)");
}

public class GoalCommands : BaseCommand
{
    private readonly IGoalsService _goalsService;

    public GoalCommands(IGoalsService goalsService, IOutputFormatter outputFormatter, ILogger<GoalCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _goalsService = goalsService;

    public override Command CreateCommand() => new Command("goal", "Goal management commands (not implemented)");
}

public class TimeTrackingCommands : BaseCommand
{
    private readonly ITimeTrackingService _timeTrackingService;

    public TimeTrackingCommands(ITimeTrackingService timeTrackingService, IOutputFormatter outputFormatter, ILogger<TimeTrackingCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _timeTrackingService = timeTrackingService;

    public override Command CreateCommand() => new Command("time-tracking", "Time tracking commands (not implemented)");
}

public class TemplateCommands : BaseCommand
{
    private readonly ITemplatesService _templatesService;

    public TemplateCommands(ITemplatesService templatesService, IOutputFormatter outputFormatter, ILogger<TemplateCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _templatesService = templatesService;

    public override Command CreateCommand() => new Command("template", "Template management commands (not implemented)");
}

public class UserGroupCommands : BaseCommand
{
    private readonly IUserGroupsService _userGroupsService;

    public UserGroupCommands(IUserGroupsService userGroupsService, IOutputFormatter outputFormatter, ILogger<UserGroupCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _userGroupsService = userGroupsService;

    public override Command CreateCommand() => new Command("user-group", "User group management commands (not implemented)");
}

public class WebhookCommands : BaseCommand
{
    private readonly IWebhooksService _webhooksService;

    public WebhookCommands(IWebhooksService webhooksService, IOutputFormatter outputFormatter, ILogger<WebhookCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _webhooksService = webhooksService;

    public override Command CreateCommand() => new Command("webhook", "Webhook management commands (not implemented)");
}

public class AttachmentCommands : BaseCommand
{
    private readonly IAttachmentsService _attachmentsService;

    public AttachmentCommands(IAttachmentsService attachmentsService, IOutputFormatter outputFormatter, ILogger<AttachmentCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _attachmentsService = attachmentsService;

    public override Command CreateCommand() => new Command("attachment", "Attachment management commands (not implemented)");
}

public class DocsCommands : BaseCommand
{
    private readonly IDocsService _docsService;

    public DocsCommands(IDocsService docsService, IOutputFormatter outputFormatter, ILogger<DocsCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _docsService = docsService;

    public override Command CreateCommand() => new Command("docs", "Docs management commands (not implemented)");
}

public class GuestCommands : BaseCommand
{
    private readonly IGuestsService _guestsService;

    public GuestCommands(IGuestsService guestsService, IOutputFormatter outputFormatter, ILogger<GuestCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _guestsService = guestsService;

    public override Command CreateCommand() => new Command("guest", "Guest management commands (not implemented)");
}

public class RoleCommands : BaseCommand
{
    private readonly IRolesService _rolesService;

    public RoleCommands(IRolesService rolesService, IOutputFormatter outputFormatter, ILogger<RoleCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _rolesService = rolesService;

    public override Command CreateCommand() => new Command("role", "Role management commands (not implemented)");
}

public class SharedHierarchyCommands : BaseCommand
{
    private readonly ISharedHierarchyService _sharedHierarchyService;

    public SharedHierarchyCommands(ISharedHierarchyService sharedHierarchyService, IOutputFormatter outputFormatter, ILogger<SharedHierarchyCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _sharedHierarchyService = sharedHierarchyService;

    public override Command CreateCommand() => new Command("shared-hierarchy", "Shared hierarchy commands (not implemented)");
}

public class TaskChecklistCommands : BaseCommand
{
    private readonly ITaskChecklistsService _taskChecklistsService;

    public TaskChecklistCommands(ITaskChecklistsService taskChecklistsService, IOutputFormatter outputFormatter, ILogger<TaskChecklistCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _taskChecklistsService = taskChecklistsService;

    public override Command CreateCommand() => new Command("task-checklist", "Task checklist commands (not implemented)");
}

public class TaskRelationshipCommands : BaseCommand
{
    private readonly ITaskRelationshipsService _taskRelationshipsService;

    public TaskRelationshipCommands(ITaskRelationshipsService taskRelationshipsService, IOutputFormatter outputFormatter, ILogger<TaskRelationshipCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _taskRelationshipsService = taskRelationshipsService;

    public override Command CreateCommand() => new Command("task-relationship", "Task relationship commands (not implemented)");
}

public class UserCommands : BaseCommand
{
    private readonly IUsersService _usersService;

    public UserCommands(IUsersService usersService, IOutputFormatter outputFormatter, ILogger<UserCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _usersService = usersService;

    public override Command CreateCommand() => new Command("user", "User management commands (not implemented)");
}

public class ChatCommands : BaseCommand
{
    private readonly IChatService _chatService;

    public ChatCommands(IChatService chatService, IOutputFormatter outputFormatter, ILogger<ChatCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _chatService = chatService;

    public override Command CreateCommand() => new Command("chat", "Chat management commands (not implemented)");
}