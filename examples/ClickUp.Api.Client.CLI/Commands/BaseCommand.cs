using ClickUp.Api.Client.CLI.Infrastructure;
using ClickUp.Api.Client.CLI.Models;
using ClickUp.Api.Client.Models.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace ClickUp.Api.Client.CLI.Commands;

/// <summary>
/// Base class for all CLI commands providing common functionality
/// </summary>
public abstract class BaseCommand
{
    protected readonly IOutputFormatter OutputFormatter;
    protected readonly ILogger Logger;
    protected readonly CliOptions Options;

    protected BaseCommand(
        IOutputFormatter outputFormatter,
        ILogger logger,
        IOptions<CliOptions> options)
    {
        OutputFormatter = outputFormatter;
        Logger = logger;
        Options = options.Value;
    }

    /// <summary>
    /// Create the command with all subcommands
    /// </summary>
    /// <returns>The configured command</returns>
    public abstract Command CreateCommand();

    /// <summary>
    /// Handle exceptions consistently across all commands
    /// </summary>
    /// <param name="ex">The exception to handle</param>
    /// <param name="context">The invocation context</param>
    /// <returns>Exit code</returns>
    protected virtual int HandleException(Exception ex, InvocationContext context)
    {
        Logger.LogError(ex, "Command execution failed");

        var errorMessage = ex switch
        {
            ClickUpApiException apiEx => FormatApiException(apiEx),
            ArgumentException argEx => $"Invalid argument: {argEx.Message}",
            InvalidOperationException opEx => $"Invalid operation: {opEx.Message}",
            HttpRequestException httpEx => $"Network error: {httpEx.Message}",
            TaskCanceledException => "Operation timed out",
            _ => $"Unexpected error: {ex.Message}"
        };

        Console.WriteLine(OutputFormatter.FormatError(errorMessage, Options.VerboseMode ? ex : null));
        return 1;
    }

    /// <summary>
    /// Format ClickUp API exceptions with specific error details
    /// </summary>
    /// <param name="apiEx">The API exception</param>
    /// <returns>Formatted error message</returns>
    protected virtual string FormatApiException(ClickUpApiException apiEx)
    {
        return apiEx.HttpStatus switch
        {
            System.Net.HttpStatusCode.Unauthorized => "Authentication failed. Please check your API token.",
            System.Net.HttpStatusCode.Forbidden => "Access denied. You don't have permission to perform this operation.",
            System.Net.HttpStatusCode.NotFound => "Resource not found. Please check the provided IDs.",
            System.Net.HttpStatusCode.TooManyRequests => "Rate limit exceeded. Please try again later.",
            System.Net.HttpStatusCode.BadRequest => $"Bad request: {apiEx.Message}",
            System.Net.HttpStatusCode.InternalServerError => "ClickUp API internal error. Please try again later.",
            _ => $"API error ({apiEx.HttpStatus}): {apiEx.Message}"
        };
    }

    /// <summary>
    /// Create a standard option for output format
    /// </summary>
    /// <returns>Format option</returns>
    protected static Option<string> CreateFormatOption()
    {
        return new Option<string>(
            aliases: new[] { "--format", "-f" },
            description: "Output format (table, json, csv, properties)",
            getDefaultValue: () => "table");
    }

    /// <summary>
    /// Create a standard option for page size
    /// </summary>
    /// <returns>Page size option</returns>
    protected Option<int> CreatePageSizeOption()
    {
        return new Option<int>(
            aliases: new[] { "--page-size", "-s" },
            description: "Number of items per page",
            getDefaultValue: () => Options.DefaultPageSize);
    }

    /// <summary>
    /// Create a standard option for page number
    /// </summary>
    /// <returns>Page option</returns>
    protected static Option<int> CreatePageOption()
    {
        return new Option<int>(
            aliases: new[] { "--page", "-p" },
            description: "Page number (0-based)",
            getDefaultValue: () => 0);
    }

    /// <summary>
    /// Create a standard option for including archived items
    /// </summary>
    /// <returns>Include archived option</returns>
    protected static Option<bool> CreateIncludeArchivedOption()
    {
        return new Option<bool>(
            aliases: new[] { "--include-archived", "-a" },
            description: "Include archived items in results");
    }

    /// <summary>
    /// Create a standard option for filtering by date range
    /// </summary>
    /// <returns>Date filter options</returns>
    protected static (Option<DateTime?> from, Option<DateTime?> to) CreateDateRangeOptions()
    {
        var fromOption = new Option<DateTime?>(
            aliases: new[] { "--from", "--start-date" },
            description: "Start date for filtering (ISO format: yyyy-MM-dd)");

        var toOption = new Option<DateTime?>(
            aliases: new[] { "--to", "--end-date" },
            description: "End date for filtering (ISO format: yyyy-MM-dd)");

        return (fromOption, toOption);
    }

    /// <summary>
    /// Create a standard option for properties to display
    /// </summary>
    /// <returns>Properties option</returns>
    protected static Option<string[]> CreatePropertiesOption()
    {
        return new Option<string[]>(
            aliases: new[] { "--properties", "--props" },
            description: "Specific properties to display (comma-separated)");
    }

    /// <summary>
    /// Validate required parameters
    /// </summary>
    /// <param name="parameters">Parameters to validate</param>
    /// <returns>True if all parameters are valid</returns>
    protected bool ValidateRequiredParameters(params (string name, object? value)[] parameters)
    {
        var missingParams = parameters
            .Where(p => p.value == null || (p.value is string str && string.IsNullOrWhiteSpace(str)))
            .Select(p => p.name)
            .ToList();

        if (missingParams.Any())
        {
            Console.WriteLine(OutputFormatter.FormatError($"Missing required parameters: {string.Join(", ", missingParams)}"));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validate numeric parameters
    /// </summary>
    /// <param name="parameters">Parameters to validate</param>
    /// <returns>True if all parameters are valid</returns>
    protected bool ValidateNumericParameters(params (string name, long value, long min, long? max)[] parameters)
    {
        var invalidParams = new List<string>();

        foreach (var (name, value, min, max) in parameters)
        {
            if (value < min)
            {
                invalidParams.Add($"{name} must be >= {min}");
            }
            else if (max.HasValue && value > max.Value)
            {
                invalidParams.Add($"{name} must be <= {max.Value}");
            }
        }

        if (invalidParams.Any())
        {
            Console.WriteLine(OutputFormatter.FormatError($"Invalid parameters: {string.Join(", ", invalidParams)}"));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Output data using the specified format
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <param name="data">Data to output</param>
    /// <param name="format">Output format</param>
    /// <param name="properties">Properties to include</param>
    protected void OutputData<T>(T data, string format, string[]? properties = null)
    {
        try
        {
            var output = OutputFormatter.Format(data, format, properties);
            Console.WriteLine(output);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to format output");
            Console.WriteLine(OutputFormatter.FormatError("Failed to format output", ex));
        }
    }

    /// <summary>
    /// Show progress indicator for long-running operations
    /// </summary>
    /// <param name="message">Progress message</param>
    protected void ShowProgress(string message)
    {
        if (Options.ShowProgress)
        {
            Console.WriteLine(OutputFormatter.FormatInfo(message));
        }
    }

    /// <summary>
    /// Parse comma-separated properties string
    /// </summary>
    /// <param name="propertiesString">Comma-separated properties</param>
    /// <returns>Array of property names</returns>
    protected static string[]? ParseProperties(string[]? propertiesArray)
    {
        if (propertiesArray == null || !propertiesArray.Any())
        {
            return null;
        }

        return propertiesArray
            .SelectMany(p => p.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToArray();
    }
}