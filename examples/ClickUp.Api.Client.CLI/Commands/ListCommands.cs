using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class ListCommands : BaseCommand
{
    private readonly IListsService _listsService;

    public ListCommands(IListsService listsService, IOutputFormatter outputFormatter, ILogger<ListCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _listsService = listsService;

    public override Command CreateCommand()
    {
        var listsCommand = new Command("lists", "List management commands")
        {
            CreateListListsCommand(),
            CreateGetListCommand()
        };
        
        return listsCommand;
    }

    private Command CreateListListsCommand()
    {
        var folderIdArgument = new Argument<string>("folder-id", "The folder ID");
        var archivedOption = new Option<bool>("--archived", "Include archived lists");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listListsCommand = new Command("list", "List lists in a folder") { folderIdArgument, archivedOption, formatOption, propertiesOption };

        listListsCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var folderId = context.ParseResult.GetValueForArgument(folderIdArgument);
                var archived = context.ParseResult.GetValueForOption(archivedOption);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (string.IsNullOrWhiteSpace(folderId))
                {
                    Console.WriteLine(OutputFormatter.FormatError("Folder ID is required"));
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving lists for folder {folderId}...");
                var lists = await _listsService.GetListsInFolderAsync(folderId, archived);

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

    private Command CreateGetListCommand()
    {
        var listIdArgument = new Argument<string>("list-id", "The list ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getListCommand = new Command("get", "Get list details") { listIdArgument, formatOption, propertiesOption };

        getListCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving list {listId}...");
                var list = await _listsService.GetListAsync(listId);

                if (list == null)
                {
                    Console.WriteLine(OutputFormatter.FormatError("List not found"));
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
}