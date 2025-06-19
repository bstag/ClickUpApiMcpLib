using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.ResponseModels.Views;

/// <summary>
/// Represents the response model for creating a folder view.
/// </summary>
public record class CreateFolderViewResponse
(
    [property: JsonPropertyName("view")]
    View View
);
