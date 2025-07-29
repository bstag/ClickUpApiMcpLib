using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.CLI.Commands;

public class ConfigCommands : BaseCommand
{
    private readonly IConfigurationValidator _configValidator;
    private static readonly string UserConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".clickup");
    private static readonly string UserConfigFile = Path.Combine(UserConfigDir, "config.json");

    public ConfigCommands(IConfigurationValidator configValidator, IOutputFormatter outputFormatter, ILogger<ConfigCommands> logger, IOptions<CliOptions> options)
        : base(outputFormatter, logger, options)
    {
        _configValidator = configValidator;
    }

    public override Command CreateCommand()
    {
        var configCommand = new Command("config", "Configuration management commands")
        {
            CreateInitCommand(),
            CreateSetCommand(),
            CreateGetCommand(),
            CreateListCommand(),
            CreateValidateCommand(),
            CreateResetCommand()
        };
        return configCommand;
    }

    private Command CreateInitCommand()
    {
        var forceOption = new Option<bool>("--force", "Overwrite existing configuration");
        var initCommand = new Command("init", "Initialize user configuration directory") { forceOption };

        initCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var force = context.ParseResult.GetValueForOption(forceOption);

                // Create user config directory
                if (!Directory.Exists(UserConfigDir))
                {
                    Directory.CreateDirectory(UserConfigDir);
                    Console.WriteLine($"✅ Created config directory: {UserConfigDir}");
                }

                // Check if config file exists
                if (File.Exists(UserConfigFile) && !force)
                {
                    Console.WriteLine($"Configuration file already exists: {UserConfigFile}");
                    Console.WriteLine("Use --force to overwrite or 'config set' to update individual settings.");
                    context.ExitCode = 1;
                    return;
                }

                // Create default config
                var defaultConfig = new UserConfig
                {
                    ClickUpApiOptions = new ClickUpApiOptions
                    {
                        PersonalAccessToken = "YOUR_CLICKUP_API_TOKEN_HERE",
                        BaseUrl = "https://api.clickup.com/api/v2"
                    },
                    CLI = new CliOptions
                    {
                        DefaultFormat = "table",
                        DefaultPageSize = 25,
                        MaxPageSize = 100,
                        EnableColors = true,
                        VerboseMode = false,
                        RequestTimeoutSeconds = 30,
                        RetryAttempts = 3,
                        RetryDelayMs = 1000,
                        EnableAutoPaging = true,
                        ShowProgress = true,
                        CacheDurationMinutes = 5,
                        EnableCaching = false
                    }
                };

                var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await File.WriteAllTextAsync(UserConfigFile, json);
                Console.WriteLine($"✅ Configuration file created: {UserConfigFile}");
                Console.WriteLine("\nNext steps:");
                Console.WriteLine($"1. Set your API token: clickup-cli config set token YOUR_TOKEN");
                Console.WriteLine($"2. Validate configuration: clickup-cli config validate");
                Console.WriteLine($"3. Test connectivity: clickup-cli health check");
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to initialize configuration: {ex.Message}");
                context.ExitCode = 1;
            }
        });

        return initCommand;
    }

    private Command CreateSetCommand()
    {
        var keyArgument = new Argument<string>("key", "Configuration key (token, format, pagesize, colors, verbose, timeout, retries, caching)");
        var valueArgument = new Argument<string>("value", "Configuration value");
        var setCommand = new Command("set", "Set a configuration value") { keyArgument, valueArgument };

        setCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var key = context.ParseResult.GetValueForArgument(keyArgument);
                var value = context.ParseResult.GetValueForArgument(valueArgument);

                if (!File.Exists(UserConfigFile))
                {
                    Console.WriteLine("Configuration file not found. Run 'config init' first.");
                    context.ExitCode = 1;
                    return;
                }

                var json = await File.ReadAllTextAsync(UserConfigFile);
                var config = JsonSerializer.Deserialize<UserConfig>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }) ?? new UserConfig();

                // Set the value based on key
                switch (key.ToLowerInvariant())
                {
                    case "token":
                        config.ClickUpApiOptions ??= new ClickUpApiOptions();
                        config.ClickUpApiOptions.PersonalAccessToken = value;
                        Console.WriteLine("✅ API token updated");
                        break;
                    case "baseurl":
                        config.ClickUpApiOptions ??= new ClickUpApiOptions();
                        config.ClickUpApiOptions.BaseUrl = value;
                        Console.WriteLine($"✅ Base URL updated to: {value}");
                        break;
                    case "format":
                        config.CLI ??= new CliOptions();
                        if (new[] { "table", "json", "csv", "properties" }.Contains(value.ToLowerInvariant()))
                        {
                            config.CLI.DefaultFormat = value.ToLowerInvariant();
                            Console.WriteLine($"✅ Default format updated to: {value}");
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid format. Use: table, json, csv, or properties");
                            context.ExitCode = 1;
                            return;
                        }
                        break;
                    case "pagesize":
                        config.CLI ??= new CliOptions();
                        if (int.TryParse(value, out var pageSize) && pageSize > 0 && pageSize <= 1000)
                        {
                            config.CLI.DefaultPageSize = pageSize;
                            Console.WriteLine($"✅ Default page size updated to: {pageSize}");
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid page size. Use a number between 1 and 1000");
                            context.ExitCode = 1;
                            return;
                        }
                        break;
                    case "colors":
                        config.CLI ??= new CliOptions();
                        if (bool.TryParse(value, out var enableColors))
                        {
                            config.CLI.EnableColors = enableColors;
                            Console.WriteLine($"✅ Colors {(enableColors ? "enabled" : "disabled")}");
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid value. Use: true or false");
                            context.ExitCode = 1;
                            return;
                        }
                        break;
                    case "verbose":
                        config.CLI ??= new CliOptions();
                        if (bool.TryParse(value, out var verboseMode))
                        {
                            config.CLI.VerboseMode = verboseMode;
                            Console.WriteLine($"✅ Verbose mode {(verboseMode ? "enabled" : "disabled")}");
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid value. Use: true or false");
                            context.ExitCode = 1;
                            return;
                        }
                        break;
                    case "timeout":
                        config.CLI ??= new CliOptions();
                        if (int.TryParse(value, out var timeout) && timeout > 0)
                        {
                            config.CLI.RequestTimeoutSeconds = timeout;
                            Console.WriteLine($"✅ Request timeout updated to: {timeout} seconds");
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid timeout. Use a positive number");
                            context.ExitCode = 1;
                            return;
                        }
                        break;
                    case "retries":
                        config.CLI ??= new CliOptions();
                        if (int.TryParse(value, out var retries) && retries >= 0)
                        {
                            config.CLI.RetryAttempts = retries;
                            Console.WriteLine($"✅ Retry attempts updated to: {retries}");
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid retry count. Use a non-negative number");
                            context.ExitCode = 1;
                            return;
                        }
                        break;
                    case "caching":
                        config.CLI ??= new CliOptions();
                        if (bool.TryParse(value, out var enableCaching))
                        {
                            config.CLI.EnableCaching = enableCaching;
                            Console.WriteLine($"✅ Caching {(enableCaching ? "enabled" : "disabled")}");
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid value. Use: true or false");
                            context.ExitCode = 1;
                            return;
                        }
                        break;
                    default:
                        Console.WriteLine($"❌ Unknown configuration key: {key}");
                        Console.WriteLine("Available keys: token, baseurl, format, pagesize, colors, verbose, timeout, retries, caching");
                        context.ExitCode = 1;
                        return;
                }

                // Save updated config
                var updatedJson = JsonSerializer.Serialize(config, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await File.WriteAllTextAsync(UserConfigFile, updatedJson);
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to set configuration: {ex.Message}");
                context.ExitCode = 1;
            }
        });

        return setCommand;
    }

    private Command CreateGetCommand()
    {
        var keyArgument = new Argument<string?>("key", () => null, "Configuration key to retrieve (optional - shows all if not specified)");
        keyArgument.Arity = ArgumentArity.ZeroOrOne;
        var getCommand = new Command("get", "Get configuration value(s)") { keyArgument };

        getCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var key = context.ParseResult.GetValueForArgument(keyArgument);

                if (!File.Exists(UserConfigFile))
                {
                    Console.WriteLine("Configuration file not found. Run 'config init' first.");
                    context.ExitCode = 1;
                    return;
                }

                var json = await File.ReadAllTextAsync(UserConfigFile);
                var config = JsonSerializer.Deserialize<UserConfig>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }) ?? new UserConfig();

                if (string.IsNullOrEmpty(key))
                {
                    // Show all configuration
                    Console.WriteLine("Current Configuration:");
                    Console.WriteLine($"  Token: {(string.IsNullOrEmpty(config.ClickUpApiOptions?.PersonalAccessToken) ? "Not set" : "***" + config.ClickUpApiOptions.PersonalAccessToken[^4..])}");
                    Console.WriteLine($"  Base URL: {config.ClickUpApiOptions?.BaseUrl ?? "Not set"}");
                    Console.WriteLine($"  Default Format: {config.CLI?.DefaultFormat ?? "table"}");
                    Console.WriteLine($"  Page Size: {config.CLI?.DefaultPageSize ?? 25}");
                    Console.WriteLine($"  Colors: {config.CLI?.EnableColors ?? true}");
                    Console.WriteLine($"  Verbose: {config.CLI?.VerboseMode ?? false}");
                    Console.WriteLine($"  Timeout: {config.CLI?.RequestTimeoutSeconds ?? 30}s");
                    Console.WriteLine($"  Retries: {config.CLI?.RetryAttempts ?? 3}");
                    Console.WriteLine($"  Caching: {config.CLI?.EnableCaching ?? false}");
                }
                else
                {
                    // Show specific key
                    switch (key.ToLowerInvariant())
                    {
                        case "token":
                            var token = config.ClickUpApiOptions?.PersonalAccessToken;
                            Console.WriteLine(string.IsNullOrEmpty(token) ? "Not set" : "***" + token[^4..]);
                            break;
                        case "baseurl":
                            Console.WriteLine(config.ClickUpApiOptions?.BaseUrl ?? "Not set");
                            break;
                        case "format":
                            Console.WriteLine(config.CLI?.DefaultFormat ?? "table");
                            break;
                        case "pagesize":
                            Console.WriteLine(config.CLI?.DefaultPageSize ?? 25);
                            break;
                        case "colors":
                            Console.WriteLine(config.CLI?.EnableColors ?? true);
                            break;
                        case "verbose":
                            Console.WriteLine(config.CLI?.VerboseMode ?? false);
                            break;
                        case "timeout":
                            Console.WriteLine(config.CLI?.RequestTimeoutSeconds ?? 30);
                            break;
                        case "retries":
                            Console.WriteLine(config.CLI?.RetryAttempts ?? 3);
                            break;
                        case "caching":
                            Console.WriteLine(config.CLI?.EnableCaching ?? false);
                            break;
                        default:
                            Console.WriteLine($"❌ Unknown configuration key: {key}");
                            context.ExitCode = 1;
                            return;
                    }
                }

                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to get configuration: {ex.Message}");
                context.ExitCode = 1;
            }
        });

        return getCommand;
    }

    private Command CreateListCommand()
    {
        var listCommand = new Command("list", "List all available configuration keys");

        listCommand.SetHandler((InvocationContext context) =>
        {
            Console.WriteLine("Available configuration keys:");
            Console.WriteLine("  token     - ClickUp API personal access token");
            Console.WriteLine("  baseurl   - ClickUp API base URL");
            Console.WriteLine("  format    - Default output format (table, json, csv, properties)");
            Console.WriteLine("  pagesize  - Default page size for paginated results");
            Console.WriteLine("  colors    - Enable colored output (true/false)");
            Console.WriteLine("  verbose   - Enable verbose mode (true/false)");
            Console.WriteLine("  timeout   - Request timeout in seconds");
            Console.WriteLine("  retries   - Number of retry attempts");
            Console.WriteLine("  caching   - Enable result caching (true/false)");
            context.ExitCode = 0;
        });

        return listCommand;
    }

    private Command CreateValidateCommand()
    {
        var validateCommand = new Command("validate", "Validate configuration settings");

        validateCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                if (!File.Exists(UserConfigFile))
                {
                    Console.WriteLine("❌ Configuration file not found. Run 'config init' first.");
                    context.ExitCode = 1;
                    return;
                }

                var json = await File.ReadAllTextAsync(UserConfigFile);
                var config = JsonSerializer.Deserialize<UserConfig>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var errors = new List<string>();

                if (config?.ClickUpApiOptions == null)
                {
                    errors.Add("ClickUp API options are missing");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(config.ClickUpApiOptions.PersonalAccessToken))
                    {
                        errors.Add("Personal access token is required");
                    }
                    if (string.IsNullOrWhiteSpace(config.ClickUpApiOptions.BaseUrl))
                    {
                        errors.Add("Base URL is required");
                    }
                    else if (!Uri.TryCreate(config.ClickUpApiOptions.BaseUrl, UriKind.Absolute, out _))
                    {
                        errors.Add("Base URL is not a valid URL");
                    }
                }

                if (config?.CLI != null)
                {
                    if (config.CLI.DefaultPageSize <= 0 || config.CLI.DefaultPageSize > 1000)
                    {
                        errors.Add("Default page size must be between 1 and 1000");
                    }
                    if (config.CLI.RequestTimeoutSeconds <= 0)
                    {
                        errors.Add("Request timeout must be greater than 0");
                    }
                    if (config.CLI.RetryAttempts < 0)
                    {
                        errors.Add("Retry attempts cannot be negative");
                    }
                }

                if (errors.Count == 0)
                {
                    Console.WriteLine("✅ Configuration is valid");
                    Console.WriteLine($"   Token: {(string.IsNullOrEmpty(config?.ClickUpApiOptions?.PersonalAccessToken) ? "Not set" : "***" + config.ClickUpApiOptions.PersonalAccessToken[^4..])}");
                    Console.WriteLine($"   Base URL: {config?.ClickUpApiOptions?.BaseUrl}");
                    context.ExitCode = 0;
                }
                else
                {
                    Console.WriteLine("❌ Configuration validation failed:");
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"  - {error}");
                    }
                    context.ExitCode = 1;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"❌ Configuration file is not valid JSON: {ex.Message}");
                context.ExitCode = 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Configuration validation failed: {ex.Message}");
                context.ExitCode = 1;
            }
        });

        return validateCommand;
    }

    private Command CreateResetCommand()
    {
        var confirmOption = new Option<bool>("--confirm", "Confirm reset without prompting");
        var resetCommand = new Command("reset", "Reset configuration to defaults") { confirmOption };

        resetCommand.SetHandler(async (InvocationContext context) =>
        {
            try
            {
                var confirm = context.ParseResult.GetValueForOption(confirmOption);

                if (!confirm)
                {
                    Console.Write("Are you sure you want to reset configuration to defaults? (y/N): ");
                    var response = Console.ReadLine();
                    if (!string.Equals(response?.Trim(), "y", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Reset cancelled.");
                        context.ExitCode = 0;
                        return;
                    }
                }

                if (File.Exists(UserConfigFile))
                {
                    File.Delete(UserConfigFile);
                }

                // Recreate with defaults
                await CreateInitCommand().InvokeAsync(new[] { "--force" });
                Console.WriteLine("✅ Configuration reset to defaults");
                context.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to reset configuration: {ex.Message}");
                context.ExitCode = 1;
            }
        });

        return resetCommand;
    }
}

// Configuration models
public class UserConfig
{
    public ClickUpApiOptions? ClickUpApiOptions { get; set; }
    public CliOptions? CLI { get; set; }
}

public class ClickUpApiOptions
{
    public string? PersonalAccessToken { get; set; }
    public string? BaseUrl { get; set; }
}