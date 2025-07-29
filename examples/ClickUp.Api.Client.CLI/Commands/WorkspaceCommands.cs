using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

/// <summary>
/// Commands for workspace operations
/// </summary>
public class WorkspaceCommands : BaseCommand
{
    private readonly IWorkspacesService _workspacesService;

    public WorkspaceCommands(
        IWorkspacesService workspacesService,
        IOutputFormatter outputFormatter,
        ILogger<WorkspaceCommands> logger,
        IOptions<CliOptions> options)
        : base(outputFormatter, logger, options)
    {
        _workspacesService = workspacesService;
    }

    public override Command CreateCommand()
    {
        var workspaceCommand = new Command("workspace", "Workspace management commands")
        {
            CreateGetSeatUsageCommand(),
            CreateGetPlanCommand()
        };

        return workspaceCommand;
    }

    private Command CreateGetSeatUsageCommand()
    {
        var workspaceIdArgument = new Argument<long>("workspace-id", "The workspace ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getSeatUsageCommand = new Command("seat-usage", "Get workspace seat usage information")
        {
            workspaceIdArgument,
            formatOption,
            propertiesOption
        };

        getSeatUsageCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var workspaceId = context.ParseResult.GetValueForArgument(workspaceIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateNumericParameters(("workspace-id", workspaceId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving seat usage for workspace {workspaceId}...");

                var seatUsage = await _workspacesService.GetWorkspaceSeatsAsync(workspaceId.ToString());

                if (seatUsage == null)
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No seat usage information found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(seatUsage, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                context.ExitCode = HandleException(ex, context);
            }
        });

        return getSeatUsageCommand;
    }

    private Command CreateGetPlanCommand()
    {
        var workspaceIdArgument = new Argument<long>("workspace-id", "The workspace ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getPlanCommand = new Command("plan", "Get workspace plan information")
        {
            workspaceIdArgument,
            formatOption,
            propertiesOption
        };

        getPlanCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var workspaceId = context.ParseResult.GetValueForArgument(workspaceIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateNumericParameters(("workspace-id", workspaceId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving plan information for workspace {workspaceId}...");

                var plan = await _workspacesService.GetWorkspacePlanAsync(workspaceId.ToString());

                if (plan == null)
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No plan information found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(plan, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                context.ExitCode = HandleException(ex, context);
            }
        });

        return getPlanCommand;
    }
}