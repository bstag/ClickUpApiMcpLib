using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Page entity

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    /// <summary>
    /// Represents the response when retrieving a single Page from a Document.
    /// </summary>
    /// <param name="Page">The retrieved <see cref="Entities.Docs.Page"/> object, typically including its content.</param>
    public record GetPageResponse
    (
        [property: JsonPropertyName("page")] Page Page
    );
}
