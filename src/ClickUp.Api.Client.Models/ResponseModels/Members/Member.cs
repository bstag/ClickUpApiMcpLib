using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.ResponseModels.Users; // Using the newly created User class

namespace ClickUp.Api.Client.Models.ResponseModels.Members
{
    /// <summary>
    /// Represents a Member of a Task, List, etc.
    /// </summary>
    public class Member
    {
        /// <summary>
        /// Gets or sets the User details of the Member.
        /// </summary>
        [JsonPropertyName("user")]
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the role of the member if applicable.
        /// (This is hypothetical, depends on actual API response for members)
        /// </summary>
        [JsonPropertyName("role")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Role { get; set; }

        /// <summary>
        /// Parameterless constructor for deserialization.
        /// </summary>
        public Member() {}

        /// <summary>
        /// Constructor to initialize required fields.
        /// </summary>
        public Member(User user)
        {
            User = user ?? throw new System.ArgumentNullException(nameof(user));
        }
    }
}
