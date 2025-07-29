using ConsoleTables;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections;
using System.Reflection;
using System.Text;
using ClickUp.Api.Client.CLI.Models;

namespace ClickUp.Api.Client.CLI.Infrastructure;

/// <summary>
/// Implementation of output formatter for different formats
/// </summary>
public class OutputFormatter : IOutputFormatter
{
    private readonly CliOptions _options;

    public OutputFormatter(IOptions<CliOptions> options)
    {
        _options = options.Value;
    }

    public string FormatAsTable<T>(IEnumerable<T> data, string[]? properties = null)
    {
        var items = data.ToList();
        if (!items.Any())
        {
            return "No data found.";
        }

        var type = typeof(T);
        var props = GetPropertiesToDisplay(type, properties);

        if (!props.Any())
        {
            return "No displayable properties found.";
        }

        var table = new ConsoleTable(props.Select(p => p.Name).ToArray());

        foreach (var item in items)
        {
            var values = props.Select(prop => FormatPropertyValue(prop.GetValue(item))).ToArray();
            table.AddRow(values);
        }

        return table.ToString();
    }

    public string FormatAsJson<T>(T data, bool indent = true)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = indent ? Formatting.Indented : Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };

        return JsonConvert.SerializeObject(data, settings);
    }

    public string FormatAsCsv<T>(IEnumerable<T> data, string[]? properties = null)
    {
        var items = data.ToList();
        if (!items.Any())
        {
            return "No data found.";
        }

        var type = typeof(T);
        var props = GetPropertiesToDisplay(type, properties);

        if (!props.Any())
        {
            return "No displayable properties found.";
        }

        var csv = new StringBuilder();
        
        // Header
        csv.AppendLine(string.Join(",", props.Select(p => EscapeCsvValue(p.Name))));
        
        // Data rows
        foreach (var item in items)
        {
            var values = props.Select(prop => EscapeCsvValue(FormatPropertyValue(prop.GetValue(item))));
            csv.AppendLine(string.Join(",", values));
        }

        return csv.ToString();
    }

    public string Format<T>(T data, string format, string[]? properties = null)
    {
        return format.ToLowerInvariant() switch
        {
            "json" => FormatAsJson(data),
            "csv" when data is IEnumerable enumerable => FormatAsCsv(enumerable.Cast<object>(), properties),
            "table" when data is IEnumerable enumerable => FormatAsTable(enumerable.Cast<object>(), properties),
            "properties" => FormatAsProperties(data, properties),
            _ when data is IEnumerable enumerable => FormatAsTable(enumerable.Cast<object>(), properties),
            _ => FormatAsProperties(data, properties)
        };
    }

    public string FormatAsProperties<T>(T data, string[]? properties = null)
    {
        if (data == null)
        {
            return "No data found.";
        }

        var type = typeof(T);
        var props = GetPropertiesToDisplay(type, properties);

        if (!props.Any())
        {
            return "No displayable properties found.";
        }

        var sb = new StringBuilder();
        var maxNameLength = props.Max(p => p.Name.Length);

        foreach (var prop in props)
        {
            var value = FormatPropertyValue(prop.GetValue(data));
            sb.AppendLine($"{prop.Name.PadRight(maxNameLength)}: {value}");
        }

        return sb.ToString();
    }

    public string FormatError(string message, Exception? exception = null)
    {
        var sb = new StringBuilder();
        
        if (_options.EnableColors)
        {
            sb.Append("\u001b[31m"); // Red color
        }
        
        sb.Append("❌ Error: ");
        sb.Append(message);
        
        if (_options.EnableColors)
        {
            sb.Append("\u001b[0m"); // Reset color
        }

        if (exception != null && _options.VerboseMode)
        {
            sb.AppendLine();
            sb.AppendLine($"Exception: {exception.GetType().Name}");
            sb.AppendLine($"Message: {exception.Message}");
            
            if (exception.StackTrace != null)
            {
                sb.AppendLine($"Stack Trace: {exception.StackTrace}");
            }
        }

        return sb.ToString();
    }

    public string FormatSuccess(string message)
    {
        var sb = new StringBuilder();
        
        if (_options.EnableColors)
        {
            sb.Append("\u001b[32m"); // Green color
        }
        
        sb.Append("✅ ");
        sb.Append(message);
        
        if (_options.EnableColors)
        {
            sb.Append("\u001b[0m"); // Reset color
        }

        return sb.ToString();
    }

    public string FormatWarning(string message)
    {
        var sb = new StringBuilder();
        
        if (_options.EnableColors)
        {
            sb.Append("\u001b[33m"); // Yellow color
        }
        
        sb.Append("⚠️  ");
        sb.Append(message);
        
        if (_options.EnableColors)
        {
            sb.Append("\u001b[0m"); // Reset color
        }

        return sb.ToString();
    }

    public string FormatInfo(string message)
    {
        var sb = new StringBuilder();
        
        if (_options.EnableColors)
        {
            sb.Append("\u001b[36m"); // Cyan color
        }
        
        sb.Append("ℹ️  ");
        sb.Append(message);
        
        if (_options.EnableColors)
        {
            sb.Append("\u001b[0m"); // Reset color
        }

        return sb.ToString();
    }

    private static PropertyInfo[] GetPropertiesToDisplay(Type type, string[]? properties)
    {
        var allProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && !p.GetIndexParameters().Any())
            .ToArray();

        if (properties == null || !properties.Any())
        {
            return allProps;
        }

        return allProps
            .Where(p => properties.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
            .ToArray();
    }

    private static string FormatPropertyValue(object? value)
    {
        return value switch
        {
            null => "<null>",
            string str => str,
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
            DateTimeOffset dto => dto.ToString("yyyy-MM-dd HH:mm:ss zzz"),
            bool b => b ? "Yes" : "No",
            IEnumerable enumerable when !(value is string) => $"[{string.Join(", ", enumerable.Cast<object>().Take(3))}]{(enumerable.Cast<object>().Count() > 3 ? "..." : "")}",
            _ => value.ToString() ?? "<null>"
        };
    }

    private static string EscapeCsvValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        // Escape quotes and wrap in quotes if contains comma, quote, or newline
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\";";
        }

        return value;
    }
}