using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Doc entity

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    // PageDefaults is defined in CreateDocResponse.cs. If it's used here,
    // it should ideally be moved to a common location or Entities/Docs if it's a standalone concept.
    // For now, assuming it's accessible or will be redefined if necessary.
    // For simplicity, let's assume GetDocResponse might not include PageDefaults or it's part of the Doc entity itself.
    // If PageDefaults is indeed part of GetDocResponse separately, we'd add:
    // [property: JsonPropertyName("page_defaults")] PageDefaults? PageDefaults,

    public record GetDocResponse
    (
        [property: JsonPropertyName("doc")] Doc Doc // The full Doc entity
        // Potentially other metadata related to the doc or user's access to it.
    );
}
