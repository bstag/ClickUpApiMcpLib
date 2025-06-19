using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Docs
{
    public record ParentDocIdentifier
    (
        [property: JsonPropertyName("id")] string Id, // ID of the parent doc or view
        [property: JsonPropertyName("type")] int Type // 0 for doc, 1 for view
    );

    public record CreateDocRequest
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("parent")] ParentDocIdentifier? Parent,
        [property: JsonPropertyName("visibility")] string? Visibility, // e.g., "private", "public_template", "workspace"
        [property: JsonPropertyName("create_page")] bool? CreatePage, // Whether to create a default page
        [property: JsonPropertyName("template_id")] string? TemplateId, // Optional template to create from
        [property: JsonPropertyName("workspace_id")] long? WorkspaceId // Usually inferred from token, but can be specified
    );
}
