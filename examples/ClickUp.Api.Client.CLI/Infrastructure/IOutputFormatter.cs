using System.Collections;

namespace ClickUp.Api.Client.CLI.Infrastructure;

/// <summary>
/// Interface for formatting output in different formats
/// </summary>
public interface IOutputFormatter
{
    /// <summary>
    /// Format data as a table
    /// </summary>
    /// <param name="data">Data to format</param>
    /// <param name="properties">Properties to include (null for all)</param>
    /// <returns>Formatted string</returns>
    string FormatAsTable<T>(IEnumerable<T> data, string[]? properties = null);

    /// <summary>
    /// Format data as JSON
    /// </summary>
    /// <param name="data">Data to format</param>
    /// <param name="indent">Whether to indent the JSON</param>
    /// <returns>Formatted JSON string</returns>
    string FormatAsJson<T>(T data, bool indent = true);

    /// <summary>
    /// Format data as CSV
    /// </summary>
    /// <param name="data">Data to format</param>
    /// <param name="properties">Properties to include (null for all)</param>
    /// <returns>Formatted CSV string</returns>
    string FormatAsCsv<T>(IEnumerable<T> data, string[]? properties = null);

    /// <summary>
    /// Format data based on the specified format
    /// </summary>
    /// <param name="data">Data to format</param>
    /// <param name="format">Output format (table, json, csv)</param>
    /// <param name="properties">Properties to include (null for all)</param>
    /// <returns>Formatted string</returns>
    string Format<T>(T data, string format, string[]? properties = null);

    /// <summary>
    /// Format a single object as a property list
    /// </summary>
    /// <param name="data">Object to format</param>
    /// <param name="properties">Properties to include (null for all)</param>
    /// <returns>Formatted string</returns>
    string FormatAsProperties<T>(T data, string[]? properties = null);

    /// <summary>
    /// Format a collection of objects as property lists
    /// </summary>
    /// <param name="data">Collection of objects to format</param>
    /// <param name="properties">Properties to include (null for all)</param>
    /// <returns>Formatted string</returns>
    string FormatAsPropertiesList<T>(IEnumerable<T> data, string[]? properties = null);

    /// <summary>
    /// Format error message with consistent styling
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="exception">Optional exception details</param>
    /// <returns>Formatted error string</returns>
    string FormatError(string message, Exception? exception = null);

    /// <summary>
    /// Format success message with consistent styling
    /// </summary>
    /// <param name="message">Success message</param>
    /// <returns>Formatted success string</returns>
    string FormatSuccess(string message);

    /// <summary>
    /// Format warning message with consistent styling
    /// </summary>
    /// <param name="message">Warning message</param>
    /// <returns>Formatted warning string</returns>
    string FormatWarning(string message);

    /// <summary>
    /// Format info message with consistent styling
    /// </summary>
    /// <param name="message">Info message</param>
    /// <returns>Formatted info string</returns>
    string FormatInfo(string message);
}