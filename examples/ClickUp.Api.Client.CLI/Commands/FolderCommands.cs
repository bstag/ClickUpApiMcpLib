using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class FolderCommands : BaseCommand
{
    private readonly IFoldersService _foldersService;

    public FolderCommands(IFoldersService foldersService, IOutputFormatter outputFormatter, ILogger<FolderCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _foldersService = foldersService;

    public override Command CreateCommand()
    {
        var folderCommand = new Command("folder", "Folder management commands")
        {
            CreateListFoldersCommand(),
            CreateGetFolderCommand()
        };
        return folderCommand;
    }

    private Command CreateListFoldersCommand()
    {
        var spaceIdArgument = new Argument<long>("space-id", "The space ID");
        var archivedOption = new Option<bool>("--archived", "Include archived folders");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listFoldersCommand = new Command("list", "List folders in a space") { spaceIdArgument, archivedOption, formatOption, propertiesOption };

        listFoldersCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var spaceId = context.ParseResult.GetValueForArgument(spaceIdArgument);
                var archived = context.ParseResult.GetValueForOption(archivedOption);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateNumericParameters(("space-id", spaceId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving folders for space {spaceId}...");
                var folders = await _foldersService.GetFoldersAsync(spaceId.ToString(), archived);

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

    private Command CreateGetFolderCommand()
    {
        var folderIdArgument = new Argument<string>("folder-id", "The folder ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getFolderCommand = new Command("get", "Get folder details") { folderIdArgument, formatOption, propertiesOption };

        getFolderCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving folder {folderId}...");
                var folder = await _foldersService.GetFolderAsync(folderId);

                if (folder == null)
                {
                    Console.WriteLine(OutputFormatter.FormatError("Folder not found"));
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
}