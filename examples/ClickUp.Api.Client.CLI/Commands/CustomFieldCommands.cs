using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class CustomFieldCommands : BaseCommand
{
    private readonly ICustomFieldsService _customFieldsService;

    public CustomFieldCommands(ICustomFieldsService customFieldsService, IOutputFormatter outputFormatter, ILogger<CustomFieldCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _customFieldsService = customFieldsService;

    public override Command CreateCommand()
    {
        var customFieldCommand = new Command("custom-field", "Custom field management commands")
        {
            CreateListCustomFieldsCommand()
        };
        return customFieldCommand;
    }

    private Command CreateListCustomFieldsCommand()
    {
        var listIdArgument = new Argument<string>("list-id", "The list ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listCustomFieldsCommand = new Command("list", "List custom fields for a list") { listIdArgument, formatOption, propertiesOption };

        listCustomFieldsCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var listId = context.ParseResult.GetValueForArgument(listIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (string.IsNullOrWhiteSpace(listId))
                {
                    Console.WriteLine(OutputFormatter.FormatError("List ID is required"));
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving custom fields for list {listId}...");
                var customFields = await _customFieldsService.GetAccessibleCustomFieldsAsync(listId);

                if (customFields == null || !customFields.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No custom fields found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(customFields, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listCustomFieldsCommand;
    }
}