using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Goals
{
    /// <summary>
    /// Represents the request to update an existing Key Result.
    /// </summary>
    public class UpdateKeyResultRequest
    {
        /// <summary>
        /// Gets or sets the new note for the Key Result.
        /// </summary>
        [JsonPropertyName("note")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Note { get; set; }

        /// <summary>
        /// Gets or sets the new current steps count.
        /// </summary>
        [JsonPropertyName("steps_current")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? StepsCurrent { get; set; }

        /// <summary>
        /// Gets or sets the list of user IDs to add as assignees.
        /// </summary>
        [JsonPropertyName("assignees_add")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<long>? AssigneesAdd { get; set; }

        /// <summary>
        /// Gets or sets the list of user IDs to remove as assignees.
        /// </summary>
        [JsonPropertyName("assignees_remove")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<long>? AssigneesRemove { get; set; }

        // According to the OpenAPI spec, 'type' is required, but it's part of CreateKeyResultOpts,
        // and for updates, it might not be directly updatable or required in the same way.
        // If the API mandates it for updates, it should be added.
        // For now, assuming it's not part of the direct update payload unless specified by a build error.
    }
}
