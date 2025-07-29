namespace ClickUp.Api.Client.CLI.Models;

/// <summary>
/// Configuration options for the CLI application
/// </summary>
public class CliOptions
{
    /// <summary>
    /// Default output format for commands
    /// </summary>
    public string DefaultFormat { get; set; } = "table";

    /// <summary>
    /// Default page size for paginated results
    /// </summary>
    public int DefaultPageSize { get; set; } = 25;

    /// <summary>
    /// Maximum page size allowed
    /// </summary>
    public int MaxPageSize { get; set; } = 100;

    /// <summary>
    /// Enable colored output
    /// </summary>
    public bool EnableColors { get; set; } = true;

    /// <summary>
    /// Enable verbose output by default
    /// </summary>
    public bool VerboseMode { get; set; } = false;

    /// <summary>
    /// Timeout for API requests in seconds
    /// </summary>
    public int RequestTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Number of retry attempts for failed requests
    /// </summary>
    public int RetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay between retry attempts in milliseconds
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// Enable automatic paging for large result sets
    /// </summary>
    public bool EnableAutoPaging { get; set; } = true;

    /// <summary>
    /// Show progress indicators for long-running operations
    /// </summary>
    public bool ShowProgress { get; set; } = true;

    /// <summary>
    /// Cache results for repeated queries (in minutes)
    /// </summary>
    public int CacheDurationMinutes { get; set; } = 5;

    /// <summary>
    /// Enable caching
    /// </summary>
    public bool EnableCaching { get; set; } = false;
}