using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For DocPageListingItem

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    public record GetDocPageListingResponse
    (
        // The structure might be a direct list or nested under a "pages" property.
        // Based on spec examples, it's often a direct list of the top-level items.
        [property: JsonPropertyName("pages")] List<DocPageListingItem> Pages
        // Or if it's a direct list: public List<DocPageListingItem> PageListing { get; init; }
        // For now, assuming it's wrapped in a "pages" property as per common patterns.
    );
}
