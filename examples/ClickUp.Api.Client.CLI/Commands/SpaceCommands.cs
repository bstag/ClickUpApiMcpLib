using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

/// <summary>
/// Commands for space operations
/// </summary>
public class SpaceCommands : BaseCommand
{
    private readonly ISpacesService _spacesService;

    public SpaceCommands(
        ISpacesService spacesService,
        IOutputFormatter outputFormatter,
        ILogger<SpaceCommands> logger,
        IOptions<CliOptions> options)
        : base(outputFormatter, logger, options)
    {
        _spacesService = spacesService;
    }

    public override Command CreateCommand()
    {
        var spaceCommand = new Command("space", "Space management commands")
        {
            CreateGetSpaceCommand(),
            CreateListSpacesCommand()
        };

        return spaceCommand;
    }

    private Command CreateGetSpaceCommand()
    {
        var spaceIdArgument = new Argument<long>("space-id", "The space ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getSpaceCommand = new Command("get", "Get a specific space by ID")
        {
            spaceIdArgument,
            formatOption,
            propertiesOption
        };

        getSpaceCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving space {spaceId}...");

                var space = await _spacesService.GetSpaceAsync(spaceId.ToString());

                if (space == null)
                {
                    Console.WriteLine(OutputFormatter.FormatWarning($"Space {spaceId} not found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(space, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                context.ExitCode = HandleException(ex, context);
            }
        });

        return getSpaceCommand;
    }

    private Command CreateListSpacesCommand()
    {
        var workspaceIdArgument = new Argument<long>("workspace-id", "The workspace ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();
        var includeArchivedOption = CreateIncludeArchivedOption();

        var listSpacesCommand = new Command("list", "List spaces in a workspace")
        {
            workspaceIdArgument,
            formatOption,
            propertiesOption,
            includeArchivedOption
        };

        listSpacesCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var workspaceId = context.ParseResult.GetValueForArgument(workspaceIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));
                var includeArchived = context.ParseResult.GetValueForOption(includeArchivedOption);

                if (!ValidateNumericParameters(("workspace-id", workspaceId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving spaces from workspace {workspaceId}...");

                var spaces = await _spacesService.GetSpacesAsync(workspaceId.ToString(), includeArchived);

                if (spaces == null || !spaces.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No spaces found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(spaces, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                context.ExitCode = HandleException(ex, context);
            }
        });

        return listSpacesCommand;
    }
}