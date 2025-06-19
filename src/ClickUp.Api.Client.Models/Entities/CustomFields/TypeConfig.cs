using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.CustomFields
{
    public record TypeConfig
    (
        [property: JsonPropertyName("options")] List<Option>? Options, // For "drop_down", "labels" types
        [property: JsonPropertyName("default")] object? DefaultValue, // Can be string, number, bool, etc. Changed name from "Default"
        [property: JsonPropertyName("precision")] int? Precision, // For "number", "currency" types
        [property: JsonPropertyName("currency_type")] string? CurrencyType, // For "currency" type
        [property: JsonPropertyName("placeholder")] string? Placeholder, // For "text", "url", "email" types
        [property: JsonPropertyName("end")] long? End, // For "date" type (timestamp)
        [property: JsonPropertyName("start")] long? Start, // For "date" type (timestamp)
        [property: JsonPropertyName("count")] int? Count, // For "users" type (max number of users)
        [property: JsonPropertyName("code_point")] string? CodePoint, // For "emoji" type
        [property: JsonPropertyName("tracking")] Tracking? Tracking, // For "progress" (auto) type
        [property: JsonPropertyName("complete_on")] int? CompleteOn, // For "progress" (manual) type (target value)
        [property: JsonPropertyName("new_drop_down")] bool? NewDropDown, // Specific to dropdowns
        [property: JsonPropertyName("include_guests")] bool? IncludeGuests, // For "users" type
        [property: JsonPropertyName("include_team_members")] bool? IncludeTeamMembers, // For "users" type
        [property: JsonPropertyName("single_user")] bool? SingleUser, // For "users" type
        [property: JsonPropertyName("is_relation")] bool? IsRelation, // For "relationship" type
        [property: JsonPropertyName("direction")] string? Direction, // For "relationship" type (e.g. "inward", "outward")
        [property: JsonPropertyName("related_list_id")] string? RelatedListId, // For "relationship" type
        [property: JsonPropertyName("related_task_id")] string? RelatedTaskId, // For "relationship" type (less common)
        [property: JsonPropertyName("related_custom_field_id")] string? RelatedCustomFieldId // For "relationship" type
    );
}
