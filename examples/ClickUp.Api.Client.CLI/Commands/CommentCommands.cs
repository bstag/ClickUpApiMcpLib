using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class CommentCommands : BaseCommand
{
    private readonly ICommentsService _commentsService;

    public CommentCommands(ICommentsService commentsService, IOutputFormatter outputFormatter, ILogger<CommentCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _commentsService = commentsService;

    public override Command CreateCommand()
    {
        var commentCommand = new Command("comment", "Comment management commands")
        {
            CreateListCommentsCommand()
        };
        return commentCommand;
    }

    private Command CreateListCommentsCommand()
    {
        var taskIdArgument = new Argument<string>("task-id", "The task ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listCommentsCommand = new Command("list", "List comments for a task") { taskIdArgument, formatOption, propertiesOption };

        listCommentsCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving comments for task {taskId}...");
                var comments = await _commentsService.GetTaskCommentsAsync(new GetTaskCommentsRequest(taskId));

                if (comments == null || !comments.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No comments found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(comments, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listCommentsCommand;
    }
}