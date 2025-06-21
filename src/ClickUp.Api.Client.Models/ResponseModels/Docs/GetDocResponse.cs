using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Doc entity

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    /// <summary>
    /// Represents the response when retrieving a single Document.
    /// </summary>
    /// <param name="Doc">The retrieved <see cref="Entities.Docs.Doc"/> object.
    /// This typically includes details about the document itself. Page content or listings are usually fetched via separate endpoints.
    /// </param>
    public record GetDocResponse
    (
        [property: JsonPropertyName("doc")] Doc Doc
    );
}
