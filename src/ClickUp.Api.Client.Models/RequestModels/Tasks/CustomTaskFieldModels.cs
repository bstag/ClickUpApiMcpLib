using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks
{
    // Represents a custom field value to be set when creating/updating a task.
    public record CustomTaskFieldToSet
    (
        [property: JsonPropertyName("id")] string Id, // Custom Field ID
        [property: JsonPropertyName("value")] object? Value,
        [property: JsonPropertyName("value_options")] CustomTaskFieldValueOptions? ValueOptions = null
    );

    // Used for Users custom fields to specify add/remove operations
    public record CustomTaskFieldValueOptions
    (
        [property: JsonPropertyName("add")] List<int>? Add, // User IDs to add
        [property: JsonPropertyName("rem")] List<int>? Remove // User IDs to remove
    );
}
