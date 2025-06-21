using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the column settings for a View (typically for List or Table views).
/// </summary>
public record ViewColumns
{
    /// <summary>
    /// Gets the list of fields that define the columns to be displayed, including their order and individual settings.
    /// </summary>
    [JsonPropertyName("fields")]
    public List<ViewColumnField>? Fields { get; init; }
}
