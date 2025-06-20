using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Roles
{
    /// <summary>
    /// Represents a Custom Role in ClickUp.
    /// </summary>
    public class CustomRole
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("inherited_role")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? InheritedRole { get; set; }

        [JsonPropertyName("date_created")]
        public long DateCreated { get; set; } // Assuming Unix timestamp (long)

        [JsonPropertyName("members")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<long>? Members { get; set; }

        // Parameterless constructor for deserialization
        public CustomRole()
        {
            Name = string.Empty; // Initialize to avoid null warnings for non-nullable string
        }

        public CustomRole(int id, string name, long dateCreated)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DateCreated = dateCreated;
        }
    }
}
