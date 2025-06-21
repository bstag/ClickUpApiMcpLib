using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Docs
{
    /// <summary>
    /// Represents an identifier for a parent entity, which can be a Document or a View.
    /// </summary>
    /// <param name="Id">The ID of the parent document or view.</param>
    /// <param name="Type">The type of the parent: 0 for a Document, 1 for a View.</param>
    public record ParentDocIdentifier
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("type")] int Type
    );

    /// <summary>
    /// Represents the request to create a new Document.
    /// </summary>
    /// <param name="Name">The name of the new document.</param>
    /// <param name="Parent">Optional: Identifier for a parent document or view under which this document should be created.</param>
    /// <param name="Visibility">Optional: The visibility of the document (e.g., "private", "public_template", "workspace").</param>
    /// <param name="CreatePage">Optional: Whether to create a default page within the new document.</param>
    /// <param name="TemplateId">Optional: The ID of a template to use for creating this document.</param>
    /// <param name="WorkspaceId">Optional: The ID of the workspace where the document will be created. Usually inferred from the API token.</param>
    public record CreateDocRequest
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("parent")] ParentDocIdentifier? Parent,
        [property: JsonPropertyName("visibility")] string? Visibility,
        [property: JsonPropertyName("create_page")] bool? CreatePage,
        [property: JsonPropertyName("template_id")] string? TemplateId,
        [property: JsonPropertyName("workspace_id")] long? WorkspaceId
    );
}
