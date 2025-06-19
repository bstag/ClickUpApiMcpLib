using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Lists;

/// <summary>
/// Represents the request model for creating a list.
/// </summary>
public record class CreateListRequest
(
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("content")]
    string? Content,

    [property: JsonPropertyName("markdown_content")]
    string? MarkdownContent,

    [property: JsonPropertyName("due_date")]
    long? DueDate,

    [property: JsonPropertyName("due_date_time")]
    bool? DueDateTime,

    [property: JsonPropertyName("priority")]
    int? Priority,

    [property: JsonPropertyName("assignee")]
    int? Assignee,

    [property: JsonPropertyName("status")]
    string? Status
);
