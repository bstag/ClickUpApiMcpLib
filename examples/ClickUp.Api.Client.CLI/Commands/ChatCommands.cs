using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class ChatCommands : BaseCommand
{
    private readonly IChatService _chatService;

    public ChatCommands(IChatService chatService, IOutputFormatter outputFormatter, ILogger<ChatCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _chatService = chatService;

    public override Command CreateCommand()
    {
        var chatCommand = new Command("chat", "Chat management commands")
        {
            CreateListChatViewsCommand()
        };
        return chatCommand;
    }

    private Command CreateListChatViewsCommand()
    {
        var teamIdArgument = new Argument<long>("team-id", "The team ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listChatViewsCommand = new Command("list", "List chat views for a team") { teamIdArgument, formatOption, propertiesOption };

        listChatViewsCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving chat channels for team {teamId}...");
                // Note: IChatService provides methods for chat channels, messages, and reactions
                // Use GetChatChannelsAsync with GetChatChannelsRequest for full functionality
                Console.WriteLine(OutputFormatter.FormatWarning("Chat channels retrieval requires GetChatChannelsRequest parameters. Use IChatService.GetChatChannelsAsync for full functionality."));
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listChatViewsCommand;
    }
}