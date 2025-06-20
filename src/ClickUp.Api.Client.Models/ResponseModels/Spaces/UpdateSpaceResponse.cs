using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Features, MemberSummary, DefaultListSettings
using ClickUp.Api.Client.Models.Common; // For Status
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.ResponseModels.Spaces
{
    public class UpdateSpaceResponse
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

        [JsonPropertyName("members")]
        public List<MemberSummary>? Members { get; set; }

        [JsonPropertyName("statuses")]
        public List<Status>? Statuses { get; set; }

        [JsonPropertyName("multiple_assignees")]
        public bool MultipleAssignees { get; set; }

        [JsonPropertyName("features")]
        public Features? Features { get; set; } // Made nullable

        [JsonPropertyName("team_id")]
        public string? TeamId { get; set; }

        [JsonPropertyName("default_list_settings")]
        public DefaultListSettings? DefaultListSettings { get; set; }

        public UpdateSpaceResponse() {} // For deserializer
    }
}
