using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

namespace ClickUp.Api.Client.CLI.Commands;

public class TimeTrackingCommands : BaseCommand
{
    private readonly ITimeTrackingService _timeTrackingService;

    public TimeTrackingCommands(ITimeTrackingService timeTrackingService, IOutputFormatter outputFormatter, ILogger<TimeTrackingCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _timeTrackingService = timeTrackingService;

    public override Command CreateCommand()
    {
        var timeTrackingCommand = new Command("time-tracking", "Time tracking commands")
        {
            CreateGetTimeEntriesCommand(),
            CreateGetSingleTimeEntryCommand()
        };
        return timeTrackingCommand;
    }

    private Command CreateGetTimeEntriesCommand()
    {
        var teamIdArgument = new Argument<long>("team-id", "The team ID");
        var startDateOption = new Option<DateTime?>("--start-date", "Start date for time entries");
        var endDateOption = new Option<DateTime?>("--end-date", "End date for time entries");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getTimeEntriesCommand = new Command("list", "Get time entries for a team") { teamIdArgument, startDateOption, endDateOption, formatOption, propertiesOption };

        getTimeEntriesCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var teamId = context.ParseResult.GetValueForArgument(teamIdArgument);
                var startDate = context.ParseResult.GetValueForOption(startDateOption);
                var endDate = context.ParseResult.GetValueForOption(endDateOption);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateNumericParameters(("team-id", teamId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving time entries for team {teamId}...");
                var timeEntries = await _timeTrackingService.GetTimeEntriesAsync(teamId.ToString());

                if (timeEntries == null || !timeEntries.Items.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No time entries found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(timeEntries, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return getTimeEntriesCommand;
    }

    private Command CreateGetSingleTimeEntryCommand()
    {
        var teamIdArgument = new Argument<long>("team-id", "The team ID");
        var timerIdArgument = new Argument<string>("timer-id", "The timer ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getSingleTimeEntryCommand = new Command("get", "Get a single time entry") { teamIdArgument, timerIdArgument, formatOption, propertiesOption };

        getSingleTimeEntryCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var teamId = context.ParseResult.GetValueForArgument(teamIdArgument);
                var timerId = context.ParseResult.GetValueForArgument(timerIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateNumericParameters(("team-id", teamId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                if (string.IsNullOrWhiteSpace(timerId))
                {
                    Console.WriteLine(OutputFormatter.FormatError("Timer ID is required"));
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving time entry {timerId} for team {teamId}...");
                var timeEntry = await _timeTrackingService.GetTimeEntryAsync(teamId.ToString(), timerId);

                if (timeEntry == null)
                {
                    Console.WriteLine(OutputFormatter.FormatError("Time entry not found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(timeEntry, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return getSingleTimeEntryCommand;
    }
}