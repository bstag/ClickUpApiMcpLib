using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.ResponseModels.Views;

/// <summary>
/// Represents the response for getting multiple views.
/// </summary>
public record GetViewsResponse
{
    /// <summary>
    /// List of views.
    /// </summary>
    [JsonPropertyName("views")]
    public List<View> Views { get; init; } = new();
}
