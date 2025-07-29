using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

namespace ClickUp.Api.Client.CLI.Commands;

public class DocsCommands : BaseCommand
{
    private readonly IDocsService _docsService;

    public DocsCommands(IDocsService docsService, IOutputFormatter outputFormatter, ILogger<DocsCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _docsService = docsService;

    public override Command CreateCommand()
    {
        var docsCommand = new Command("docs", "Docs management commands")
        {
            CreateListDocsCommand()
        };
        return docsCommand;
    }

    private Command CreateListDocsCommand()
    {
        var workspaceIdArgument = new Argument<long>("workspace-id", "The workspace ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listDocsCommand = new Command("list", "List docs for a workspace") { workspaceIdArgument, formatOption, propertiesOption };

        listDocsCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving docs for workspace {workspaceId}...");
                var docs = await _docsService.SearchDocsAsync(workspaceId.ToString(), new SearchDocsRequest());

                if (docs == null || docs.Items == null || !docs.Items.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No docs found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(docs.Items, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listDocsCommand;
    }
}