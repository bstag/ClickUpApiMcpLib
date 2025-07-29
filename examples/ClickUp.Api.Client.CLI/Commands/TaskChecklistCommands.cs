using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class TaskChecklistCommands : BaseCommand
{
    private readonly ITaskChecklistsService _taskChecklistsService;

    public TaskChecklistCommands(ITaskChecklistsService taskChecklistsService, IOutputFormatter outputFormatter, ILogger<TaskChecklistCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _taskChecklistsService = taskChecklistsService;

    public override Command CreateCommand()
    {
        var checklistCommand = new Command("task-checklist", "Task checklist commands")
        {
            CreateListChecklistsCommand()
        };
        return checklistCommand;
    }

    private Command CreateListChecklistsCommand()
    {
        var taskIdArgument = new Argument<string>("task-id", "The task ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listChecklistsCommand = new Command("list", "List checklists for a task") { taskIdArgument, formatOption, propertiesOption };

        listChecklistsCommand.SetHandler(async (InvocationContext context) =>
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

                ShowProgress($"Retrieving checklists for task {taskId}...");
                // Note: Task checklists are retrieved as part of the task details via ITasksService.GetTaskAsync
                // ITaskChecklistsService only provides CRUD operations for checklists, not retrieval
                Console.WriteLine(OutputFormatter.FormatWarning("Task checklists can be retrieved via the task get command"));
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return listChecklistsCommand;
    }
}