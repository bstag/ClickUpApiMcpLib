using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Serialization.Converters;

namespace ClickUp.Api.Client.Models.Entities.Tasks;

/// <summary>
/// Represents a linked task.
/// </summary>
public record class LinkedTask
(
    [property: JsonPropertyName("task_id")]
    string TaskId,

    [property: JsonPropertyName("link_id")]
    string LinkId,

    [property: JsonPropertyName("date_created")]
    [property: JsonConverter(typeof(UnixEpochDateTimeOffsetConverter))]
    DateTimeOffset DateCreated,

    [property: JsonPropertyName("userid")]
    string UserId
);