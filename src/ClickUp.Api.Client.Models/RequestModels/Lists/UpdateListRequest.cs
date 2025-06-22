using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Lists;

/// <summary>
/// Represents the request model for updating a list.
/// </summary>
public record class UpdateListRequest
(
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("content")]
    string? Content,

    [property: JsonPropertyName("markdown_content")]
    string? MarkdownContent,

    [property: JsonPropertyName("due_date")]
    System.DateTimeOffset? DueDate,

    [property: JsonPropertyName("due_date_time")]
    bool? DueDateTime,

    [property: JsonPropertyName("priority")]
    int? Priority,

    [property: JsonPropertyName("assignee")]
    string? Assignee,

    [property: JsonPropertyName("status")]
    string? Status,

    [property: JsonPropertyName("unset_status")]
    bool? UnsetStatus
);