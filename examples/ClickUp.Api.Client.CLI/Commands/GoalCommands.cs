using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

namespace ClickUp.Api.Client.CLI.Commands;

public class GoalCommands : BaseCommand
{
    private readonly IGoalsService _goalsService;

    public GoalCommands(IGoalsService goalsService, IOutputFormatter outputFormatter, ILogger<GoalCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _goalsService = goalsService;

    public override Command CreateCommand()
    {
        var goalCommand = new Command("goal", "Goal management commands")
        {
            CreateListGoalsCommand(),
            CreateGetGoalCommand()
        };
        return goalCommand;
    }

    private Command CreateListGoalsCommand()
    {
        var teamIdArgument = new Argument<long>("team-id", "The team ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listGoalsCommand = new Command("list", "List goals for a team") { teamIdArgument, formatOption, propertiesOption };

        listGoalsCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving goals for team {teamId}...");
                var goals = await _goalsService.GetGoalsAsync(teamId.ToString());

                if (goals == null || !goals.Goals.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No goals found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(goals, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listGoalsCommand;
    }

    private Command CreateGetGoalCommand()
    {
        var goalIdArgument = new Argument<string>("goal-id", "The goal ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getGoalCommand = new Command("get", "Get goal details") { goalIdArgument, formatOption, propertiesOption };

        getGoalCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var goalId = context.ParseResult.GetValueForArgument(goalIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (string.IsNullOrWhiteSpace(goalId))
                {
                    Console.WriteLine(OutputFormatter.FormatError("Goal ID is required"));
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving goal {goalId}...");
                var goal = await _goalsService.GetGoalAsync(goalId);

                if (goal == null)
                {
                    Console.WriteLine(OutputFormatter.FormatError("Goal not found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(goal, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return getGoalCommand;
    }
}