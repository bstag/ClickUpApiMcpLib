using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class AttachmentCommands : BaseCommand
{
    private readonly IAttachmentsService _attachmentsService;

    public AttachmentCommands(IAttachmentsService attachmentsService, IOutputFormatter outputFormatter, ILogger<AttachmentCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _attachmentsService = attachmentsService;

    public override Command CreateCommand()
    {
        var attachmentCommand = new Command("attachment", "Attachment management commands")
        {
            CreateListAttachmentsCommand()
        };
        return attachmentCommand;
    }

    private Command CreateListAttachmentsCommand()
    {
        var taskIdArgument = new Argument<string>("task-id", "The task ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listAttachmentsCommand = new Command("list", "List attachments for a task") { taskIdArgument, formatOption, propertiesOption };

        listAttachmentsCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var taskId = context.ParseResult.GetValueForArgument(taskIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (string.IsNullOrWhiteSpace(taskId))
                {
                    Console.WriteLine(OutputFormatter.FormatError("Task ID is required"));
                    context.ExitCode = 1;
                    return;
                }

                // Note: IAttachmentsService only provides methods for creating attachments
                // There is no GetTaskAttachmentsAsync method available in the current API
                Console.WriteLine(OutputFormatter.FormatWarning("Getting task attachments is not supported. Use attachment creation operations instead."));
                context.ExitCode = 1;
                return;
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listAttachmentsCommand;
    }
}