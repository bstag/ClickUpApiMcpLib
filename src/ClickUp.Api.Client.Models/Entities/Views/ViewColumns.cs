using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the column settings for a View (typically for List or Table views).
/// </summary>
public record ViewColumns
{
    /// <summary>
    /// List of fields defining the columns to display, their order, and settings.
    /// </summary>
    [JsonPropertyName("fields")]
    public List<ViewColumnField>? Fields { get; init; }

    // Example from GetView response:
    // "columns": {
    //   "fields": [
    //     { "id": "name", "field": "name", "title": "Task name", "type": "string", "width": 300, "orderindex": 0, "hidden": false },
    //     { "id": "assignees", "field": "assignees", "title": "Assignees", "type": "user", "width": 100, "orderindex": 1, "hidden": false },
    //     // ... other column field objects ...
    //   ]
    // }
    // The structure seems to be primarily the list of fields.
}
