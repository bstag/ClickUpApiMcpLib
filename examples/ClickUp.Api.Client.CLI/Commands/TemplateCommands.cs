using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

namespace ClickUp.Api.Client.CLI.Commands;

public class TemplateCommands : BaseCommand
{
    private readonly ITemplatesService _templatesService;

    public TemplateCommands(ITemplatesService templatesService, IOutputFormatter outputFormatter, ILogger<TemplateCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _templatesService = templatesService;

    public override Command CreateCommand()
    {
        var templateCommand = new Command("template", "Template management commands")
        {
            CreateListTemplatesCommand()
        };
        return templateCommand;
    }

    private Command CreateListTemplatesCommand()
    {
        var teamIdArgument = new Argument<long>("team-id", "The team ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listTemplatesCommand = new Command("list", "List templates for a team") { teamIdArgument, formatOption, propertiesOption };

        listTemplatesCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving templates for team {teamId}...");
                var templates = await _templatesService.GetTaskTemplatesAsync(teamId.ToString(), 0);

                if (templates == null || !templates.Templates.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No templates found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(templates, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listTemplatesCommand;
    }
}