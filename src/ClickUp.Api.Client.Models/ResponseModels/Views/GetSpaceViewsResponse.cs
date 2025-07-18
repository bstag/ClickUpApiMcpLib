using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.ResponseModels.Views;

/// <summary>
/// Represents the response model for getting space views.
/// </summary>
public record class GetSpaceViewsResponse
(
    [property: JsonPropertyName("views")]
    List<View> Views
);
