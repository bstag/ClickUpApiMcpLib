using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.ResponseModels.Views;

/// <summary>
/// Represents the response model for updating a view.
/// </summary>
public record class UpdateViewResponse
(
    [property: JsonPropertyName("view")]
    View View
);
