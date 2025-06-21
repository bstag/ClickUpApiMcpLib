using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Page entity

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    /// <summary>
    /// Represents the response after creating a new Page within a Document.
    /// </summary>
    /// <param name="Page">The newly created <see cref="Entities.Docs.Page"/> object.
    /// The API might return the full page object or a subset of its properties.
    /// </param>
    public record CreatePageResponse
    (
        [property: JsonPropertyName("page")] Page Page
    );
}
