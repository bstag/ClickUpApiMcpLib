using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class SharedHierarchyCommands : BaseCommand
{
    private readonly ISharedHierarchyService _sharedHierarchyService;

    public SharedHierarchyCommands(ISharedHierarchyService sharedHierarchyService, IOutputFormatter outputFormatter, ILogger<SharedHierarchyCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _sharedHierarchyService = sharedHierarchyService;

    public override Command CreateCommand()
    {
        var sharedCommand = new Command("shared-hierarchy", "Shared hierarchy commands")
        {
            CreateListSharedHierarchyCommand()
        };
        return sharedCommand;
    }

    private Command CreateListSharedHierarchyCommand()
    {
        var teamIdArgument = new Argument<long>("team-id", "The team ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listSharedCommand = new Command("list", "List shared hierarchy for a team") { teamIdArgument, formatOption, propertiesOption };

        listSharedCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var teamId = context.ParseResult.GetValueForArgument(teamIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateNumericParameters(("team-id", teamId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving shared hierarchy for team {teamId}...");
                var sharedHierarchy = await _sharedHierarchyService.GetSharedHierarchyAsync(teamId.ToString());

                if (sharedHierarchy == null)
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No shared hierarchy found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(sharedHierarchy, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listSharedCommand;
    }
}