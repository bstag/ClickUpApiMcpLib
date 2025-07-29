using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

namespace ClickUp.Api.Client.CLI.Commands;

public class ViewCommands : BaseCommand
{
    private readonly IViewsService _viewsService;

    public ViewCommands(IViewsService viewsService, IOutputFormatter outputFormatter, ILogger<ViewCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _viewsService = viewsService;

    public override Command CreateCommand()
    {
        var viewCommand = new Command("view", "View management commands")
        {
            CreateListSpaceViewsCommand(),
            CreateListFolderViewsCommand(),
            CreateListListViewsCommand(),
            CreateGetViewCommand()
        };
        return viewCommand;
    }

    private Command CreateListSpaceViewsCommand()
    {
        var spaceIdArgument = new Argument<long>("space-id", "The space ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listSpaceViewsCommand = new Command("list-space", "List views for a space") { spaceIdArgument, formatOption, propertiesOption };

        listSpaceViewsCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var spaceId = context.ParseResult.GetValueForArgument(spaceIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateNumericParameters(("space-id", spaceId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving views for space {spaceId}...");
                var views = await _viewsService.GetSpaceViewsAsync(spaceId.ToString());

                if (views == null || !views.Views.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No views found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(views, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listSpaceViewsCommand;
    }

    private Command CreateListFolderViewsCommand()
    {
        var folderIdArgument = new Argument<string>("folder-id", "The folder ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listFolderViewsCommand = new Command("list-folder", "List views for a folder") { folderIdArgument, formatOption, propertiesOption };

        listFolderViewsCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var folderId = context.ParseResult.GetValueForArgument(folderIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (string.IsNullOrWhiteSpace(folderId))
                {
                    Console.WriteLine(OutputFormatter.FormatError("Folder ID is required"));
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving views for folder {folderId}...");
                var views = await _viewsService.GetFolderViewsAsync(folderId);

                if (views == null || !views.Views.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No views found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(views, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listFolderViewsCommand;
    }

    private Command CreateListListViewsCommand()
    {
        var listIdArgument = new Argument<string>("list-id", "The list ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listListViewsCommand = new Command("list-list", "List views for a list") { listIdArgument, formatOption, propertiesOption };

        listListViewsCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var listId = context.ParseResult.GetValueForArgument(listIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (string.IsNullOrWhiteSpace(listId))
                {
                    Console.WriteLine(OutputFormatter.FormatError("List ID is required"));
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving views for list {listId}...");
                var views = await _viewsService.GetListViewsAsync(listId);

                if (views == null || !views.Views.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No views found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(views, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listListViewsCommand;
    }

    private Command CreateGetViewCommand()
    {
        var viewIdArgument = new Argument<string>("view-id", "The view ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getViewCommand = new Command("get", "Get view details") { viewIdArgument, formatOption, propertiesOption };

        getViewCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var viewId = context.ParseResult.GetValueForArgument(viewIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (string.IsNullOrWhiteSpace(viewId))
                {
                    Console.WriteLine(OutputFormatter.FormatError("View ID is required"));
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving view {viewId}...");
                var view = await _viewsService.GetViewAsync(viewId);

                if (view == null)
                {
                    Console.WriteLine(OutputFormatter.FormatError("View not found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(view, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return getViewCommand;
    }
}