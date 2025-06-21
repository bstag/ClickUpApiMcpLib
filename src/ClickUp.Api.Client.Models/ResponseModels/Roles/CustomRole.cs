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
        /// <summary>
        /// Gets or sets the ID of the custom role.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the custom role.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ID of the role from which this custom role inherits permissions.
        /// Null if it does not inherit from a standard role.
        /// </summary>
        [JsonPropertyName("inherited_role")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? InheritedRole { get; set; }

        /// <summary>
        /// Gets or sets the date when this custom role was created, as a Unix timestamp (long).
        /// </summary>
        [JsonPropertyName("date_created")]
        public long DateCreated { get; set; }

        /// <summary>
        /// Gets or sets a list of user IDs who are members of this custom role.
        /// </summary>
        [JsonPropertyName("members")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<long>? Members { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRole"/> class.
        /// Required for deserialization.
        /// </summary>
        public CustomRole()
        {
            Name = string.Empty; // Initialize to avoid null warnings for non-nullable string
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRole"/> class with specified properties.
        /// </summary>
        /// <param name="id">The ID of the custom role.</param>
        /// <param name="name">The name of the custom role.</param>
        /// <param name="dateCreated">The creation date timestamp of the custom role.</param>
        public CustomRole(int id, string name, long dateCreated)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DateCreated = dateCreated;
        }
    }
}
