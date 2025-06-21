using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User type

namespace ClickUp.Api.Client.Models.Entities.Docs
{
    /// <summary>
    /// Represents a Document in ClickUp.
    /// </summary>
    /// <param name="Id">The unique identifier of the document.</param>
    /// <param name="Name">The name of the document.</param>
    /// <param name="WorkspaceId">The identifier of the workspace this document belongs to.</param>
    /// <param name="Creator">The user who created the document. Can be null in some contexts.</param>
    public record Doc
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("workspace_id")] string WorkspaceId,
        [property: JsonPropertyName("creator")] User? Creator
    );
}
