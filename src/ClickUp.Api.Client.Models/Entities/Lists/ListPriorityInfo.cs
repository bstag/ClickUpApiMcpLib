using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Lists
{
    public record ListPriorityInfo
    (
        [property: JsonPropertyName("priority")] string Priority, // Assuming priority is a string, adjust if it's an int
        [property: JsonPropertyName("color")] string? Color // Assuming color can be nullable
    );
}
