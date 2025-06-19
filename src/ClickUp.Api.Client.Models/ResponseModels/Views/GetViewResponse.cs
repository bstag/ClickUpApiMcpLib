using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.ResponseModels.Views;

/// <summary>
/// Represents the response model for getting a view.
/// </summary>
public record class GetViewResponse
(
    [property: JsonPropertyName("view")]
    View View
);
