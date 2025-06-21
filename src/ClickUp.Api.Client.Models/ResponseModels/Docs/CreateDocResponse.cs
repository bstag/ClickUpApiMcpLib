using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Doc entity and PageDefaults

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    /// <summary>
    /// Represents the response after creating a new Document.
    /// </summary>
    /// <param name="Doc">The newly created <see cref="Entities.Docs.Doc"/> object.</param>
    /// <param name="PageDefaults">Default settings for pages created within this document.</param>
    /// <param name="DefaultPageId">The ID of the default page created within the document, if applicable.</param>
    public record CreateDocResponse
    (
        [property: JsonPropertyName("doc")] Doc Doc,
        [property: JsonPropertyName("page_defaults")] PageDefaults? PageDefaults,
        [property: JsonPropertyName("default_page_id")] string? DefaultPageId
    );
}
