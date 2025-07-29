using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class UserGroupCommands : BaseCommand
{
    private readonly IUserGroupsService _userGroupsService;

    public UserGroupCommands(IUserGroupsService userGroupsService, IOutputFormatter outputFormatter, ILogger<UserGroupCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _userGroupsService = userGroupsService;

    public override Command CreateCommand()
    {
        var userGroupCommand = new Command("user-group", "User group management commands")
        {
            CreateListUserGroupsCommand()
        };
        return userGroupCommand;
    }

    private Command CreateListUserGroupsCommand()
    {
        var teamIdArgument = new Argument<long>("team-id", "The team ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listUserGroupsCommand = new Command("list", "List user groups for a team") { teamIdArgument, formatOption, propertiesOption };

        listUserGroupsCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving user groups for team {teamId}...");
                var userGroups = await _userGroupsService.GetUserGroupsAsync(teamId.ToString());

                if (userGroups == null || !userGroups.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No user groups found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(userGroups, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listUserGroupsCommand;
    }
}