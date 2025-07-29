using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class GuestCommands : BaseCommand
{
    private readonly IGuestsService _guestsService;

    public GuestCommands(IGuestsService guestsService, IOutputFormatter outputFormatter, ILogger<GuestCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _guestsService = guestsService;

    public override Command CreateCommand()
    {
        var guestCommand = new Command("guest", "Guest management commands")
        {
            CreateListGuestsCommand()
        };
        return guestCommand;
    }

    private Command CreateListGuestsCommand()
    {
        var teamIdArgument = new Argument<long>("team-id", "The team ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listGuestsCommand = new Command("list", "List guests for a team") { teamIdArgument, formatOption, propertiesOption };

        listGuestsCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving guests for team {teamId}...");
                // Note: IGuestsService only provides methods for individual guest operations
                // There is no GetGuestsAsync method available
                Console.WriteLine(OutputFormatter.FormatWarning("Getting multiple guests is not supported. Use individual guest operations instead."));
                context.ExitCode = 1;
                return;
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listGuestsCommand;
    }
}