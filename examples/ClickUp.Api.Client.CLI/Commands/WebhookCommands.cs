using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class WebhookCommands : BaseCommand
{
    private readonly IWebhooksService _webhooksService;

    public WebhookCommands(IWebhooksService webhooksService, IOutputFormatter outputFormatter, ILogger<WebhookCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _webhooksService = webhooksService;

    public override Command CreateCommand()
    {
        var webhookCommand = new Command("webhook", "Webhook management commands")
        {
            CreateListWebhooksCommand()
        };
        return webhookCommand;
    }

    private Command CreateListWebhooksCommand()
    {
        var teamIdArgument = new Argument<long>("team-id", "The team ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listWebhooksCommand = new Command("list", "List webhooks for a team") { teamIdArgument, formatOption, propertiesOption };

        listWebhooksCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving webhooks for team {teamId}...");
                var webhooks = await _webhooksService.GetWebhooksAsync(teamId.ToString());

                if (webhooks == null || !webhooks.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No webhooks found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(webhooks, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listWebhooksCommand;
    }
}