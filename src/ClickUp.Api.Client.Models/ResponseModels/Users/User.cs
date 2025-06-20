using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Users
{
    /// <summary>
    /// Represents a ClickUp User.
    /// </summary>
    public class User
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Color { get; set; }

        [JsonPropertyName("profilePicture")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ProfilePicture { get; set; }

        [JsonPropertyName("initials")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Initials { get; set; }

        // Parameterless constructor for deserialization
        public User() { }

        // Constructor to initialize required fields (if any identified as strictly required by API for all User contexts)
        // For now, assuming Username and Email are generally essential.
        public User(long id, string username, string email)
        {
            Id = id;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
    }
}
