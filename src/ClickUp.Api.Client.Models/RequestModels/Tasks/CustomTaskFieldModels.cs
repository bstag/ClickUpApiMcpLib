using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks
{
    /// <summary>
    /// Represents a custom field and its value to be set when creating or updating a task.
    /// </summary>
    /// <param name="Id">The ID of the Custom Field.</param>
    /// <param name="Value">The value to set for the Custom Field. The type of this object depends on the Custom Field's type (e.g., string, number, array of user IDs).</param>
    /// <param name="ValueOptions">Optional: Additional options for setting the value, typically used for 'Users' custom fields to specify add/remove operations.</param>
    public record CustomTaskFieldToSet
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("value")] object? Value,
        [property: JsonPropertyName("value_options")] CustomTaskFieldValueOptions? ValueOptions = null
    );

    /// <summary>
    /// Represents options for modifying a 'Users' (people) custom field value, allowing for adding or removing users.
    /// </summary>
    /// <param name="Add">A list of user IDs to add to the custom field.</param>
    /// <param name="Remove">A list of user IDs to remove from the custom field.</param>
    public record CustomTaskFieldValueOptions
    (
        [property: JsonPropertyName("add")] List<int>? Add,
        [property: JsonPropertyName("rem")] List<int>? Remove
    );
}
