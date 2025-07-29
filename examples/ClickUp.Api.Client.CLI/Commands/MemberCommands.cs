using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class MemberCommands : BaseCommand
{
    private readonly IMembersService _membersService;

    public MemberCommands(IMembersService membersService, IOutputFormatter outputFormatter, ILogger<MemberCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _membersService = membersService;

    public override Command CreateCommand()
    {
        var memberCommand = new Command("member", "Member management commands")
        {
            CreateListMembersCommand()
        };
        return memberCommand;
    }

    private Command CreateListMembersCommand()
    {
        var teamIdArgument = new Argument<long>("team-id", "The team ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listMembersCommand = new Command("list", "List members for a team") { teamIdArgument, formatOption, propertiesOption };

        listMembersCommand.SetHandler(async (InvocationContext context) =>
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

                Console.WriteLine(OutputFormatter.FormatWarning("Getting team members is not supported. Use task or list member commands instead."));
                Console.WriteLine("Available commands:");
                Console.WriteLine("  - Use 'task member <task-id>' to get task members");
                Console.WriteLine("  - Use 'list member <list-id>' to get list members");
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listMembersCommand;
    }
}