using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

/// <summary>
/// Commands for authentication and user information
/// </summary>
public class AuthCommands : BaseCommand
{
    private readonly IAuthorizationService _authService;

    public AuthCommands(
        IAuthorizationService authService,
        IOutputFormatter outputFormatter,
        ILogger<AuthCommands> logger,
        IOptions<CliOptions> options)
        : base(outputFormatter, logger, options)
    {
        _authService = authService;
    }

    public override Command CreateCommand()
    {
        var authCommand = new Command("auth", "Authentication and user information commands")
        {
            CreateUserCommand(),
            CreateWorkspacesCommand()
        };

        return authCommand;
    }

    private Command CreateUserCommand()
    {
        var userCommand = new Command("user", "Get current user information")
        {
            CreateGetUserCommand()
        };

        return userCommand;
    }

    private Command CreateGetUserCommand()
    {
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getUserCommand = new Command("get", "Get current authenticated user information")
        {
            formatOption,
            propertiesOption
        };

        getUserCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                ShowProgress("Retrieving user information...");

                var user = await _authService.GetAuthorizedUserAsync();

                if (user == null)
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No user information found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(user, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                context.ExitCode = HandleException(ex, context);
            }
        });

        return getUserCommand;
    }

    private Command CreateWorkspacesCommand()
    {
        var workspacesCommand = new Command("workspaces", "Workspace management commands")
        {
            CreateListWorkspacesCommand()
        };

        return workspacesCommand;
    }

    private Command CreateListWorkspacesCommand()
    {
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var listWorkspacesCommand = new Command("list", "List all authorized workspaces")
        {
            formatOption,
            propertiesOption
        };

        listWorkspacesCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                ShowProgress("Retrieving workspaces...");

                var workspaces = await _authService.GetAuthorizedWorkspacesAsync();

                if (workspaces == null || !workspaces.Any())
                {
                    Console.WriteLine(OutputFormatter.FormatWarning("No workspaces found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(workspaces, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                context.ExitCode = HandleException(ex, context);
            }
        });

        return listWorkspacesCommand;
    }
}