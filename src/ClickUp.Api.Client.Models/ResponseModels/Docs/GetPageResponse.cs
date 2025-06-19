using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Page entity

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    public record GetPageResponse
    (
        // Similar to CreatePageResponse, assuming it returns the full Page entity.
        [property: JsonPropertyName("page")] Page Page
        // Could also include additional metadata or permissions specific to the get response.
    );
}
