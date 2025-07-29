using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.Common.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

namespace ClickUp.Api.Client.CLI.Commands;

/// <summary>
/// Commands for task operations
/// </summary>
public class TaskCommands : BaseCommand
{
    private readonly ITasksService _tasksService;

    public TaskCommands(
        ITasksService tasksService,
        IOutputFormatter outputFormatter,
        ILogger<TaskCommands> logger,
        IOptions<CliOptions> options)
        : base(outputFormatter, logger, options)
    {
        _tasksService = tasksService;
    }

    public override Command CreateCommand()
    {
        var taskCommand = new Command("task", "Task management commands")
        {
            CreateGetTaskCommand(),
            CreateListTasksCommand(),
            CreateGetTimeInStatusCommand(),
            CreateGetBulkTimeInStatusCommand()
        };

        return taskCommand;
    }

    private Command CreateGetTaskCommand()
    {
        var taskIdArgument = new Argument<string>("task-id", "The task ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();
        var includeSubtasksOption = new Option<bool>(
            aliases: new[] { "--include-subtasks" },
            description: "Include subtasks in the response");

        var getTaskCommand = new Command("get", "Get a specific task by ID")
        {
            taskIdArgument,
            formatOption,
            propertiesOption,
            includeSubtasksOption
        };

        getTaskCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var taskId = context.ParseResult.GetValueForArgument(taskIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));
                var includeSubtasks = context.ParseResult.GetValueForOption(includeSubtasksOption);

                if (!ValidateRequiredParameters(("task-id", taskId)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving task {taskId}...");

                var task = await _tasksService.GetTaskAsync(taskId, new GetTaskRequest { IncludeSubtasks = includeSubtasks, CustomTaskIds = false });

                if (task == null)
                {
                    Console.WriteLine(OutputFormatter.FormatWarning($"Task {taskId} not found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(task, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                context.ExitCode = HandleException(ex, context);
            }
        });

        return getTaskCommand;
    }

    private Command CreateListTasksCommand()
    {
        var listIdArgument = new Argument<long>("list-id", "The list ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();
        var pageOption = CreatePageOption();
        var pageSizeOption = CreatePageSizeOption();
        var includeArchivedOption = CreateIncludeArchivedOption();
        var (fromDateOption, toDateOption) = CreateDateRangeOptions();
        
        var orderByOption = new Option<string>(
            aliases: new[] { "--order-by" },
            description: "Order by field (id, created, updated, due_date)");
        
        var reverseOption = new Option<bool>(
            aliases: new[] { "--reverse" },
            description: "Reverse the order");
        
        var subtasksOption = new Option<bool>(
            aliases: new[] { "--subtasks" },
            description: "Include subtasks");
        
        var statusesOption = new Option<string[]>(
            aliases: new[] { "--statuses" },
            description: "Filter by status names (comma-separated)");
        
        var assigneesOption = new Option<int[]>(
            aliases: new[] { "--assignees" },
            description: "Filter by assignee IDs (comma-separated)");
        
        var tagsOption = new Option<string[]>(
            aliases: new[] { "--tags" },
            description: "Filter by tag names (comma-separated)");

        var listTasksCommand = new Command("list", "List tasks in a list")
        {
            listIdArgument,
            formatOption,
            propertiesOption,
            pageOption,
            pageSizeOption,
            includeArchivedOption,
            fromDateOption,
            toDateOption,
            orderByOption,
            reverseOption,
            subtasksOption,
            statusesOption,
            assigneesOption,
            tagsOption
        };

        listTasksCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var listId = context.ParseResult.GetValueForArgument(listIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));
                var page = context.ParseResult.GetValueForOption(pageOption);
                var pageSize = context.ParseResult.GetValueForOption(pageSizeOption);
                var includeArchived = context.ParseResult.GetValueForOption(includeArchivedOption);
                var fromDate = context.ParseResult.GetValueForOption(fromDateOption);
                var toDate = context.ParseResult.GetValueForOption(toDateOption);
                var orderBy = context.ParseResult.GetValueForOption(orderByOption);
                var reverse = context.ParseResult.GetValueForOption(reverseOption);
                var subtasks = context.ParseResult.GetValueForOption(subtasksOption);
                var statuses = context.ParseResult.GetValueForOption(statusesOption);
                var assignees = context.ParseResult.GetValueForOption(assigneesOption);
                var tags = context.ParseResult.GetValueForOption(tagsOption);

                if (!ValidateNumericParameters(
                    ("list-id", listId, 1, null),
                    ("page", page, 0, null),
                    ("page-size", pageSize, 1, Options.MaxPageSize)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving tasks from list {listId}...");

                var tasks = await _tasksService.GetTasksAsync(listId.ToString(), parameters => {
                    parameters.Archived = includeArchived;
                    parameters.Page = page;
                    parameters.Subtasks = subtasks;
                    parameters.Statuses = statuses;
                    parameters.IncludeClosed = true;
                    parameters.AssigneeIds = assignees;
                    parameters.Tags = tags;
                    if (!string.IsNullOrEmpty(orderBy)) {
                        var sortDirection = reverse ? SortDirection.Descending : SortDirection.Ascending;
                        parameters.SortBy = new SortOption(orderBy, sortDirection);
                    }
                    if (fromDate.HasValue && toDate.HasValue) {
                        parameters.DueDateRange = new TimeRange(fromDate.Value, toDate.Value);
                        parameters.DateCreatedRange = new TimeRange(fromDate.Value, toDate.Value);
                        parameters.DateUpdatedRange = new TimeRange(fromDate.Value, toDate.Value);
                    }
                });

                if (tasks == null || tasks.Items == null || !tasks.Items.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No tasks found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(tasks, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                context.ExitCode = HandleException(ex, context);
            }
        });

        return listTasksCommand;
    }

    private Command CreateGetTimeInStatusCommand()
    {
        var taskIdArgument = new Argument<string>("task-id", "The task ID");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getTimeInStatusCommand = new Command("time-in-status", "Get time tracking information for a task")
        {
            taskIdArgument,
            formatOption,
            propertiesOption
        };

        getTimeInStatusCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var taskId = context.ParseResult.GetValueForArgument(taskIdArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (!ValidateRequiredParameters(("task-id", taskId)))
                {
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving time in status for task {taskId}...");

                var timeInStatus = await _tasksService.GetTaskTimeInStatusAsync(taskId, new GetTaskTimeInStatusRequest { CustomTaskIds = false });

                if (timeInStatus == null)
                {
                    Console.WriteLine(OutputFormatter.FormatWarning($"Time in status for task {taskId} not found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(timeInStatus, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                context.ExitCode = HandleException(ex, context);
            }
        });

        return getTimeInStatusCommand;
    }

    private Command CreateGetBulkTimeInStatusCommand()
    {
        var taskIdsArgument = new Argument<string[]>("task-ids", "The task IDs (comma-separated)");
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getBulkTimeInStatusCommand = new Command("bulk-time-in-status", "Get time tracking information for multiple tasks")
        {
            taskIdsArgument,
            formatOption,
            propertiesOption
        };

        getBulkTimeInStatusCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var taskIds = context.ParseResult.GetValueForArgument(taskIdsArgument);
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                if (taskIds == null || !taskIds.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatError("At least one task ID is required"));
                    context.ExitCode = 1;
                    return;
                }

                // Parse comma-separated task IDs
                var parsedTaskIds = taskIds
                    .SelectMany(id => id.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .Select(id => id.Trim())
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .ToArray();

                if (!parsedTaskIds.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatError("No valid task IDs provided"));
                    context.ExitCode = 1;
                    return;
                }

                ShowProgress($"Retrieving time in status for {parsedTaskIds.Length} tasks...");

                var bulkTimeInStatus = await _tasksService.GetBulkTasksTimeInStatusAsync(new GetBulkTasksTimeInStatusRequest(parsedTaskIds, customTaskIds: false));

                if (bulkTimeInStatus == null || !bulkTimeInStatus.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No time tracking data found for the specified tasks"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(bulkTimeInStatus, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                context.ExitCode = HandleException(ex, context);
            }
        });

        return getBulkTimeInStatusCommand;
    }
}