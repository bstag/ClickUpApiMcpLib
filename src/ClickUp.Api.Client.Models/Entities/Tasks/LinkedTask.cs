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
    DateTimeOffset DateCreated,

    [property: JsonPropertyName("userid")]
    string UserId
)
{
    [JsonConverter(typeof(UnixEpochDateTimeOffsetConverter))]
    public DateTimeOffset DateCreated { get; init; } = DateCreated;
}