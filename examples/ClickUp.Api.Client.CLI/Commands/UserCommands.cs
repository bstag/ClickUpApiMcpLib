using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

public class UserCommands : BaseCommand
{
    private readonly IAuthorizationService _authorizationService;

    public UserCommands(IAuthorizationService authorizationService, IOutputFormatter outputFormatter, ILogger<UserCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options) => _authorizationService = authorizationService;

    public override Command CreateCommand()
    {
        var userCommand = new Command("user", "User management commands")
        {
            CreateGetUserCommand()
        };
        return userCommand;
    }

    private Command CreateGetUserCommand()
    {
        var formatOption = CreateFormatOption();
        var propertiesOption = CreatePropertiesOption();

        var getUserCommand = new Command("get", "Get current user information") { formatOption, propertiesOption };

        getUserCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var format = context.ParseResult.GetValueForOption(formatOption)!;
                var properties = ParseProperties(context.ParseResult.GetValueForOption(propertiesOption));

                ShowProgress("Retrieving current user information...");
                var user = await _authorizationService.GetAuthorizedUserAsync();

                if (user == null)
                {
                    Console.WriteLine(OutputFormatter.FormatError("User information not found"));
                    context.ExitCode = 1;
                    return;
                }

                OutputData(user, format, properties);
                context.ExitCode = 0;
            }
            catch (Exception ex) { context.ExitCode = HandleException(ex, context); }
        });

        return getUserCommand;
    }
}