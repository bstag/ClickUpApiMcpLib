using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class RoleCommands : BaseCommand
{
    private readonly IRolesService _rolesService;

    public RoleCommands(IRolesService rolesService, IOutputFormatter outputFormatter, ILogger<RoleCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _rolesService = rolesService;

    public override Command CreateCommand()
    {
        var roleCommand = new Command("role", "Role management commands")
        {
            CreateListRolesCommand()
        };
        return roleCommand;
    }

    private Command CreateListRolesCommand()
    {
        var teamIdArgument = new Argument<long>("team-id", "The team ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listRolesCommand = new Command("list", "List roles for a team") { teamIdArgument, formatOption, propertiesOption };

        listRolesCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving roles for team {teamId}...");
                var roles = await _rolesService.GetCustomRolesAsync(teamId.ToString());

                if (roles == null || !roles.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No roles found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(roles, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listRolesCommand;
    }
}