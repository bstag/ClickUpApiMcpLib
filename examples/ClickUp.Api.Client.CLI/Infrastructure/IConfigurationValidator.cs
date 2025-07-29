namespace ClickUp.Api.Client.CLI.Infrastructure;

/// <summary>
/// Interface for validating configuration settings
/// </summary>
public interface IConfigurationValidator
{
    /// <summary>
    /// Validate configuration settings
    /// </summary>
    /// <returns>Validation result</returns>
    Task<ValidationResult> ValidateAsync();
}

/// <summary>
/// Result of configuration validation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Whether the validation passed
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of validation warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Additional validation information
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}