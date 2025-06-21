using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    /// <summary>
    /// Represents basic information about a user, specifically in the context of comments or replies.
    /// Note: This model is very similar to <see cref="ChatSimpleUser"/>. Consider consolidation if appropriate.
    /// </summary>
    /// <param name="Email">The email address of the user. May not always be present.</param>
    /// <param name="Id">The unique identifier of the user.</param>
    /// <param name="Initials">The initials of the user.</param>
    /// <param name="Name">The full name of the user. May not always be present.</param>
    /// <param name="Username">The username of the user. May not always be present.</param>
    /// <param name="ProfilePicture">The URL to the user's profile picture, if available.</param>
    public record CommentSimpleUser
    (
        [property: JsonPropertyName("email")] string? Email,
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("initials")] string? Initials,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("username")] string? Username,
        [property: JsonPropertyName("profilePicture")] string? ProfilePicture
    );
}
