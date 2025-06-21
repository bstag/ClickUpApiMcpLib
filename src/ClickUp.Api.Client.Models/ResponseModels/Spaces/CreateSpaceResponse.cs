using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Features, MemberSummary
using ClickUp.Api.Client.Models.Common; // For Status
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.ResponseModels.Spaces
{
    /// <summary>
    /// Represents the response after creating a new Space.
    /// This typically includes the full details of the newly created Space.
    /// </summary>
    public class CreateSpaceResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the newly created Space.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the Space.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets a value indicating whether the Space is private.
        /// </summary>
        [JsonPropertyName("private")]
        public bool Private { get; set; }

        /// <summary>
        /// Gets or sets the color associated with the Space.
        /// </summary>
        [JsonPropertyName("color")]
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the URL of the avatar for the Space.
        /// </summary>
        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether admins can manage this private Space.
        /// </summary>
        [JsonPropertyName("admin_can_manage")]
        public bool? AdminCanManage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Space is archived.
        /// </summary>
        [JsonPropertyName("archived")]
        public bool? Archived { get; set; }

        /// <summary>
        /// Gets or sets the list of statuses available within this Space.
        /// </summary>
        [JsonPropertyName("statuses")]
        public List<Status>? Statuses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tasks within this Space can have multiple assignees.
        /// </summary>
        [JsonPropertyName("multiple_assignees")]
        public bool MultipleAssignees { get; set; }

        /// <summary>
        /// Gets or sets the configuration of features enabled for this Space.
        /// </summary>
        [JsonPropertyName("features")]
        public Features? Features { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the team (workspace) this Space belongs to.
        /// </summary>
        [JsonPropertyName("team_id")]
        public string? TeamId { get; set; }

        /// <summary>
        /// Gets or sets a list of members in this Space (summary view).
        /// </summary>
        [JsonPropertyName("members")]
        public List<MemberSummary>? Members { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSpaceResponse"/> class.
        /// Required for deserialization.
        /// </summary>
        public CreateSpaceResponse() {}
    }
}
