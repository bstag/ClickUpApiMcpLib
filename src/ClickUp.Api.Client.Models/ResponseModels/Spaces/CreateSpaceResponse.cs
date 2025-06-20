using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Features, MemberSummary
using ClickUp.Api.Client.Models.Common; // For Status
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.ResponseModels.Spaces
{
    public class CreateSpaceResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("private")]
        public bool Private { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

        [JsonPropertyName("admin_can_manage")]
        public bool? AdminCanManage { get; set; }

        [JsonPropertyName("archived")]
        public bool? Archived { get; set; }

        [JsonPropertyName("statuses")]
        public List<Status>? Statuses { get; set; }

        [JsonPropertyName("multiple_assignees")]
        public bool MultipleAssignees { get; set; }

        [JsonPropertyName("features")]
        public Features? Features { get; set; } // Made nullable

        [JsonPropertyName("team_id")]
        public string? TeamId { get; set; }

        [JsonPropertyName("members")]
        public List<MemberSummary>? Members { get; set; }

        // DefaultListSettings is not typically part of a create response for a space,
        // and was null in the base call of the previous record, so omitting it here.

        public CreateSpaceResponse() {} // For deserializer
    }
}
