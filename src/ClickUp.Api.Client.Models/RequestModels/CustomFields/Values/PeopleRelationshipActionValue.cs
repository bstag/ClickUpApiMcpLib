using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the value for adding or removing people relationships in a custom field.
/// </summary>
public record class PeopleRelationshipActionValue
(
    [property: JsonPropertyName("add")]
    List<int>? Add,

    [property: JsonPropertyName("rem")]
    List<int>? Rem
);
