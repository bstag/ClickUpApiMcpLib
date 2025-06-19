using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.ResponseModels.Views;

/// <summary>
/// Represents the response model for getting folder views.
/// </summary>
public record class GetFolderViewsResponse
(
    [property: JsonPropertyName("views")]
    List<View> Views
);
