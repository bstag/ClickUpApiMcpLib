using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For User type

namespace ClickUp.Api.Client.Models.Entities.Docs
{
    public record Doc
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("workspace_id")] string WorkspaceId,
        [property: JsonPropertyName("creator")] ComUser? Creator // Assuming Creator can be nullable
    );
}
