using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Lists;

namespace ClickUp.Api.Client.Models.ResponseModels.Lists;

/// <summary>
/// Represents the response model for getting lists.
/// </summary>
public record class GetListsResponse
(
    [property: JsonPropertyName("lists")]
    List<List> Lists
);
