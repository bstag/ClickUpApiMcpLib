using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users; // Ensure this is the correct User model

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
        public required User User { get; set; } // Use the User record from Entities.Users

        /// <summary>
        /// Gets or sets the role of the member if applicable.
        /// (This is hypothetical, depends on actual API response for members)
        /// </summary>
        [JsonPropertyName("role")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Role { get; set; }

        // Parameterless constructor for deserialization can be removed if all essential properties are 'required'
        // or if a constructor is provided that initializes them.
        // For now, let's assume System.Text.Json can handle records with 'required' properties.
        // public Member() {} // Likely not needed if 'User' is required.

        // Constructor to initialize required fields.
        // This constructor might also not be strictly necessary if using 'required' properties
        // and relying on object initializers, but can be kept for convenience.
        public Member(User user)
        {
            User = user; // No null check needed if 'user' parameter itself is non-nullable
                         // and User property is 'required'
        }

        // Adding a parameterless constructor back for now to ensure no deserialization issues,
        // as 'required' primarily enforces assignment during object creation in code.
        // System.Text.Json might still need a parameterless constructor for complex scenarios,
        // especially if not all properties are part of a primary constructor (which isn't the case here, but good to be cautious).
        public Member() { }
    }
}
