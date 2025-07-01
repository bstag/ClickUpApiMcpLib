using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.CustomFields;

namespace ClickUp.Api.Client.Models.ResponseModels.CustomFields;

/// <summary>
/// Represents the response model for getting accessible custom fields.
/// </summary>
public record class GetAccessibleCustomFieldsResponse
(
    [property: JsonPropertyName("fields")]
    List<CustomFieldDefinition> Fields
);
