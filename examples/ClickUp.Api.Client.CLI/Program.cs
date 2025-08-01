using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.CommandLine;
using System.CommandLine.Invocation;
using ClickUp.Api.Client.Extensions;
using ClickUp.Api.Client.CLI.Commands;
using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using static ClickUp.Api.Client.CLI.Commands.ConfigCommands;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Caching.Memory;
// Enhanced SDK imports for latest patterns
using ClickUp.Api.Client.Abstractions.Strategies;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Services.Caching;
using ClickUp.Api.Client.Strategies.Authentication;
using ClickUp.Api.Client.Strategies.Caching;
using ClickUp.Api.Client.Strategies.Retry;
using ClickUp.Api.Client.Plugins;
using ClickUp.Api.Client.Fluent.Enhanced;
using ClickUp.Api.Client.Fluent.Composition;

namespace ClickUp.Api.Client.CLI;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Build configuration with user config directory support
        var userConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".clickup");
        var userConfigFile = Path.Combine(userConfigDir, "config.json");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            // Add user config first (highest priority after env vars)
            .AddJsonFile(userConfigFile, optional: true, reloadOnChange: true)
            // Add project config as fallback
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            // Environment variables have highest priority
            .AddEnvironmentVariables("CLICKUP_")
            .Build();

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        try
        {
            Log.Information("ClickUp CLI starting...");

            // Build host
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Clear default configuration sources
                    config.Sources.Clear();
                    
                    // Add our custom configuration with user config support
                    config.SetBasePath(Directory.GetCurrentDirectory())
                        // Add user config first (highest priority after env vars)
                        .AddJsonFile(userConfigFile, optional: true, reloadOnChange: true)
                        // Add project config as fallback
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        // Environment variables have highest priority
                        .AddEnvironmentVariables("CLICKUP_");
                })
                .ConfigureServices((context, services) =>
                {
                    // Add HTTP logging handler
                    services.AddTransient<HttpLoggingHandler>();
                    
                    // Configure enhanced caching strategies
                    services.AddMemoryCache();
                    services.AddSingleton<ICachingStrategy, MemoryCachingStrategy>();
                    services.AddSingleton<ICacheService, MemoryCacheService>();
                    
                    // Configure authentication strategies
                    services.AddSingleton<IAuthenticationStrategy>(provider =>
                    {
                        var logger = provider.GetService<ILogger<ApiKeyAuthenticationStrategy>>();
                        var strategy = new ApiKeyAuthenticationStrategy(logger);
                        var apiToken = context.Configuration["ClickUpApiOptions:PersonalAccessToken"];
                        if (!string.IsNullOrWhiteSpace(apiToken))
                        {
                            strategy.SetApiKey(apiToken);
                        }
                        return strategy;
                    });
                    
                    // Note: Retry strategies are configured within the ClickUp SDK
                    
                    // Add ClickUp SDK with enhanced configuration
                    var httpClientBuilder = services.AddClickUpClient(options =>
                    {
                        // Try to get from user config first, then fallback to appsettings
                        var apiOptions = new ClickUpApiOptions();
                        
                        // Check if we have user config values
                        var userConfigToken = context.Configuration["ClickUpApiOptions:PersonalAccessToken"];
                        var userConfigBaseUrl = context.Configuration["ClickUpApiOptions:BaseUrl"];
                        
                        if (!string.IsNullOrEmpty(userConfigToken))
                        {
                            apiOptions.PersonalAccessToken = userConfigToken;
                            apiOptions.BaseUrl = userConfigBaseUrl;
                        }
                        else
                        {
                            // Fallback to section binding for appsettings
                            context.Configuration.GetSection("ClickUpApiOptions").Bind(apiOptions);
                        }
                        
                        // Map CLI ClickUpApiOptions to SDK ClickUpClientOptions
                        options.PersonalAccessToken = apiOptions.PersonalAccessToken;
                        options.BaseAddress = apiOptions.BaseUrl ?? "https://api.clickup.com/api/v2/";
                    });
                    
                    // Add HTTP logging handler (will only log when debug is enabled)
                    httpClientBuilder.AddHttpMessageHandler<HttpLoggingHandler>();

                    // Add CLI services
                    services.Configure<CliOptions>(context.Configuration.GetSection("CLI"));
                    services.AddSingleton<IOutputFormatter, OutputFormatter>();
                    services.AddSingleton<IConfigurationValidator, ConfigurationValidator>();
                    
                    // Add debug state service
                    services.AddSingleton<IDebugStateService, DebugStateService>();
                    
                    // Add command handlers
                    services.AddTransient<AuthCommands>();
                    services.AddTransient<WorkspaceCommands>();
                    services.AddTransient<SpaceCommands>();
                    services.AddTransient<FolderCommands>();
                    services.AddTransient<ListCommands>();
                    services.AddTransient<TaskCommands>();
                    services.AddTransient<CommentCommands>();
                    services.AddTransient<MemberCommands>();
                    services.AddTransient<CustomFieldCommands>();
                    services.AddTransient<TagCommands>();
                    services.AddTransient<ViewCommands>();
                    services.AddTransient<GoalCommands>();
                    services.AddTransient<TimeTrackingCommands>();
                    services.AddTransient<TemplateCommands>();
                    services.AddTransient<UserGroupCommands>();
                    services.AddTransient<WebhookCommands>();
                    services.AddTransient<AttachmentCommands>();
                    services.AddTransient<DocsCommands>();
                    services.AddTransient<GuestCommands>();
                    services.AddTransient<RoleCommands>();
                    services.AddTransient<SharedHierarchyCommands>();
                    services.AddTransient<TaskChecklistCommands>();
                    services.AddTransient<TaskRelationshipCommands>();
                    services.AddTransient<UserCommands>();
                    services.AddTransient<ChatCommands>();
                    services.AddTransient<ConfigCommands>();
                })
                .UseSerilog()
                .Build();

            // Create root command
            var rootCommand = new RootCommand("ClickUp CLI - Command line interface for ClickUp API")
            {
                Name = "clickup-cli"
            };

            // Add global options
            var formatOption = new Option<string>(
                aliases: new[] { "--format", "-f" },
                description: "Output format (table, json, csv)",
                getDefaultValue: () => "table");
            
            var verboseOption = new Option<bool>(
                aliases: new[] { "--verbose", "-v" },
                description: "Enable verbose output");
            
            var debugOption = new Option<bool>(
                aliases: new[] { "--debug", "-d" },
                description: "Enable debug output including HTTP request/response logging");
            
            var configOption = new Option<string?>(
                aliases: new[] { "--config", "-c" },
                description: "Path to configuration file");

            rootCommand.AddGlobalOption(formatOption);
            rootCommand.AddGlobalOption(verboseOption);
            rootCommand.AddGlobalOption(debugOption);
            rootCommand.AddGlobalOption(configOption);

            // Get service provider
            using var serviceScope = host.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            // Add commands
            var authCommands = serviceProvider.GetRequiredService<AuthCommands>();
            var workspaceCommands = serviceProvider.GetRequiredService<WorkspaceCommands>();
            var spaceCommands = serviceProvider.GetRequiredService<SpaceCommands>();
            var folderCommands = serviceProvider.GetRequiredService<FolderCommands>();
            var listCommands = serviceProvider.GetRequiredService<ListCommands>();
            var taskCommands = serviceProvider.GetRequiredService<TaskCommands>();
            var commentCommands = serviceProvider.GetRequiredService<CommentCommands>();
            var memberCommands = serviceProvider.GetRequiredService<MemberCommands>();
            var customFieldCommands = serviceProvider.GetRequiredService<CustomFieldCommands>();
            var tagCommands = serviceProvider.GetRequiredService<TagCommands>();
            var viewCommands = serviceProvider.GetRequiredService<ViewCommands>();
            var goalCommands = serviceProvider.GetRequiredService<GoalCommands>();
            var timeTrackingCommands = serviceProvider.GetRequiredService<TimeTrackingCommands>();
            var templateCommands = serviceProvider.GetRequiredService<TemplateCommands>();
            var userGroupCommands = serviceProvider.GetRequiredService<UserGroupCommands>();
            var webhookCommands = serviceProvider.GetRequiredService<WebhookCommands>();
            var attachmentCommands = serviceProvider.GetRequiredService<AttachmentCommands>();
            var docsCommands = serviceProvider.GetRequiredService<DocsCommands>();
            var guestCommands = serviceProvider.GetRequiredService<GuestCommands>();
            var roleCommands = serviceProvider.GetRequiredService<RoleCommands>();
            var sharedHierarchyCommands = serviceProvider.GetRequiredService<SharedHierarchyCommands>();
            var taskChecklistCommands = serviceProvider.GetRequiredService<TaskChecklistCommands>();
            var taskRelationshipCommands = serviceProvider.GetRequiredService<TaskRelationshipCommands>();
            var userCommands = serviceProvider.GetRequiredService<UserCommands>();
            var chatCommands = serviceProvider.GetRequiredService<ChatCommands>();

            // Register commands
            rootCommand.AddCommand(authCommands.CreateCommand());
            rootCommand.AddCommand(workspaceCommands.CreateCommand());
            rootCommand.AddCommand(spaceCommands.CreateCommand());
            rootCommand.AddCommand(folderCommands.CreateCommand());
            rootCommand.AddCommand(listCommands.CreateCommand());
            rootCommand.AddCommand(taskCommands.CreateCommand());
            rootCommand.AddCommand(commentCommands.CreateCommand());
            rootCommand.AddCommand(memberCommands.CreateCommand());
            rootCommand.AddCommand(customFieldCommands.CreateCommand());
            rootCommand.AddCommand(tagCommands.CreateCommand());
            rootCommand.AddCommand(viewCommands.CreateCommand());
            rootCommand.AddCommand(goalCommands.CreateCommand());
            rootCommand.AddCommand(timeTrackingCommands.CreateCommand());
            rootCommand.AddCommand(templateCommands.CreateCommand());
            rootCommand.AddCommand(userGroupCommands.CreateCommand());
            rootCommand.AddCommand(webhookCommands.CreateCommand());
            rootCommand.AddCommand(attachmentCommands.CreateCommand());
            rootCommand.AddCommand(docsCommands.CreateCommand());
            rootCommand.AddCommand(guestCommands.CreateCommand());
            rootCommand.AddCommand(roleCommands.CreateCommand());
            rootCommand.AddCommand(sharedHierarchyCommands.CreateCommand());
            rootCommand.AddCommand(taskChecklistCommands.CreateCommand());
            rootCommand.AddCommand(taskRelationshipCommands.CreateCommand());
            rootCommand.AddCommand(userCommands.CreateCommand());
            rootCommand.AddCommand(chatCommands.CreateCommand());

            // Add utility commands
            rootCommand.AddCommand(CreateHealthCommand(serviceProvider));
            var configCommands = serviceProvider.GetRequiredService<ConfigCommands>();
            rootCommand.AddCommand(configCommands.CreateCommand());

            // Parse arguments to get debug option value before invoking
            var parseResult = rootCommand.Parse(args);
            var debugValue = parseResult.GetValueForOption(debugOption);
            var debugStateService = serviceProvider.GetRequiredService<IDebugStateService>();
            debugStateService.SetDebugEnabled(debugValue);
            
            // Check if this is a config or help command - skip validation for these
            var isConfigCommand = args.Length > 0 && (args[0] == "config" || args[0] == "--help" || args[0] == "-h" || args[0] == "help");
            var isHealthCommand = args.Length > 0 && args[0] == "health";
            
            // Validate configuration before executing commands (except config and help commands)
            if (!isConfigCommand && !isHealthCommand)
            {
                var validator = serviceProvider.GetRequiredService<IConfigurationValidator>();
                var validationResult = await validator.ValidateAsync();
                
                if (!validationResult.IsValid)
                {
                    Console.WriteLine("❌ Configuration Error:");
                    foreach (var error in validationResult.Errors)
                    {
                        Console.WriteLine($"   {error}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("💡 To fix this issue:");
                    Console.WriteLine("   1. Copy 'appsettings.template.json' to 'appsettings.json'");
                    Console.WriteLine("   2. Edit 'appsettings.json' and replace 'YOUR_CLICKUP_API_TOKEN_HERE' with your actual ClickUp API token");
                    Console.WriteLine("   3. Get your API token from: https://app.clickup.com/settings/apps");
                    Console.WriteLine();
                    Console.WriteLine("   Or use the config command to set up your configuration:");
                    Console.WriteLine("   clickup-cli config init");
                    return 1;
                }
                
                // Show warnings if any
                if (validationResult.Warnings.Count > 0)
                {
                    foreach (var warning in validationResult.Warnings)
                    {
                        Console.WriteLine($"⚠️  {warning}");
                    }
                    Console.WriteLine();
                }
            }
            
            // Parse and invoke
            return await rootCommand.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static Command CreateHealthCommand(IServiceProvider serviceProvider)
    {
        var healthCommand = new Command("health", "Health check commands")
        {
            CreateHealthCheckCommand(serviceProvider)
        };

        return healthCommand;
    }

    private static Command CreateHealthCheckCommand(IServiceProvider serviceProvider)
    {
        var checkCommand = new Command("check", "Check API connectivity and token validity");
        
        checkCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var validator = serviceProvider.GetRequiredService<IConfigurationValidator>();
                var result = await validator.ValidateAsync();
                
                if (result.IsValid)
                {
                    Console.WriteLine("✅ Health check passed");
                    Console.WriteLine($"✅ API Token: Valid");
                    Console.WriteLine($"✅ API Connectivity: OK");
                    context.ExitCode = 0;
                }
                else
                {
                    Console.WriteLine("❌ Health check failed");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"❌ {error}");
                    }
                    context.ExitCode = 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Health check failed: {ex.Message}");
                context.ExitCode = 1;
            }
        });
        
        return checkCommand;
    }
}