using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Templates;

/// <summary>
/// Represents the response model for getting task templates.
/// </summary>
public record class GetTaskTemplatesResponse
(
    [property: JsonPropertyName("templates")]
    List<string> Templates // Assuming template details might be more complex later, but string for now as per schema
);
