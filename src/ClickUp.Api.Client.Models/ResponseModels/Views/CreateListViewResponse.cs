using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.ResponseModels.Views;

/// <summary>
/// Represents the response model for creating a list view.
/// </summary>
public record class CreateListViewResponse
(
    [property: JsonPropertyName("view")]
    View View
);
