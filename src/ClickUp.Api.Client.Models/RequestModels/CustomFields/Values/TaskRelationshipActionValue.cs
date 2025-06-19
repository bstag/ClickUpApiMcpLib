using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the value for adding or removing task relationships in a custom field.
/// </summary>
public record class TaskRelationshipActionValue
(
    [property: JsonPropertyName("add")]
    List<string>? Add,

    [property: JsonPropertyName("rem")]
    List<string>? Rem
);
