using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.ResponseModels.Views;

/// <summary>
/// Represents the response model for getting team views.
/// </summary>
public record class GetTeamViewsResponse
(
    [property: JsonPropertyName("views")]
    List<View> Views
);
