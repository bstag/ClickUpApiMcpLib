using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class TagCommands : BaseCommand
{
    private readonly ITagsService _tagsService;

    public TagCommands(ITagsService tagsService, IOutputFormatter outputFormatter, ILogger<TagCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _tagsService = tagsService;

    public override Command CreateCommand()
    {
        var tagCommand = new Command("tag", "Tag management commands")
        {
            CreateListTagsCommand()
        };
        return tagCommand;
    }

    private Command CreateListTagsCommand()
    {
        var spaceIdArgument = new Argument<long>("space-id", "The space ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listTagsCommand = new Command("list", "List tags for a space") { spaceIdArgument, formatOption, propertiesOption };

        listTagsCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var spaceId = context.ParseResult.GetValueForArgument(spaceIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateNumericParameters(("space-id", spaceId, 1, null)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving tags for space {spaceId}...");
                var tags = await _tagsService.GetSpaceTagsAsync(spaceId.ToString());

                if (tags == null || !tags.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No tags found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(tags, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listTagsCommand;
    }
}