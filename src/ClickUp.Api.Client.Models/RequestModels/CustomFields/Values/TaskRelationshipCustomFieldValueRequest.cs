using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request model for setting a task relationship custom field value.
/// </summary>
public record class TaskRelationshipCustomFieldValueRequest
(
    [property: JsonPropertyName("value")]
    TaskRelationshipActionValue Value
);
