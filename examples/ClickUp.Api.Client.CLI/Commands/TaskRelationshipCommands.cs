using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class TaskRelationshipCommands : BaseCommand
{
    private readonly ITaskRelationshipsService _taskRelationshipsService;

    public TaskRelationshipCommands(ITaskRelationshipsService taskRelationshipsService, IOutputFormatter outputFormatter, ILogger<TaskRelationshipCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _taskRelationshipsService = taskRelationshipsService;

    public override Command CreateCommand()
    {
        var relationshipCommand = new Command("task-relationship", "Task relationship commands")
        {
            CreateListRelationshipsCommand()
        };
        return relationshipCommand;
    }

    private Command CreateListRelationshipsCommand()
    {
        var taskIdArgument = new Argument<string>("task-id", "The task ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listRelationshipsCommand = new Command("list", "List relationships for a task") { taskIdArgument, formatOption, propertiesOption };

        listRelationshipsCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving relationships for task {taskId}...");
                // Note: Task relationships (dependencies and links) can only be added/removed via ITaskRelationshipsService
                // To retrieve task relationships, use ITasksService.GetTaskAsync which includes relationship information
                Console.WriteLine(OutputFormatter.FormatWarning("Task relationships can be retrieved via the task get command"));
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listRelationshipsCommand;
    }
}