using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Templates;

namespace ClickUp.Api.Client.Models.ResponseModels.Templates;

/// <summary>
/// Represents the response model for getting task templates.
/// </summary>
public record class GetTaskTemplatesResponse
(
    [property: JsonPropertyName("templates")]
    List<TaskTemplate> Templates
);
