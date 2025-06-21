using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.CustomFields
{
    /// <summary>
    /// Represents the type-specific configuration for a Custom Field.
    /// Note: This is similar to <see cref="ClickUp.Api.Client.Models.CustomFields.CustomFieldTypeConfig"/>. Consider consolidation.
    /// </summary>
    /// <param name="Options">List of options, e.g., for 'Dropdown' or 'Labels' types.</param>
    /// <param name="DefaultValue">Default value for the field. API name is "default".</param>
    /// <param name="Precision">Precision for 'Number' or 'Currency' types.</param>
    /// <param name="CurrencyType">Currency symbol/code for 'Currency' type.</param>
    /// <param name="Placeholder">Placeholder text for input fields.</param>
    /// <param name="End">End date/time for 'Date' type (Unix timestamp).</param>
    /// <param name="Start">Start date/time for 'Date' type (Unix timestamp).</param>
    /// <param name="Count">Maximum count for 'Users' type, or item count for 'Rating' (emoji) type.</param>
    /// <param name="CodePoint">Unicode code point for 'Rating' (emoji) type.</param>
    /// <param name="Tracking">Tracking configuration for 'Progress (Automatic)' type.</param>
    /// <param name="CompleteOn">Target value for completion for 'Progress (Manual)' type.</param>
    /// <param name="NewDropDown">Indicates if it's a new style dropdown.</param>
    /// <param name="IncludeGuests">For 'Users' type, whether to include guests.</param>
    /// <param name="IncludeTeamMembers">For 'Users' type, whether to include team members.</param>
    /// <param name="SingleUser">For 'Users' type, whether only a single user can be selected.</param>
    /// <param name="IsRelation">For 'Relationship' type, indicates if it's a relationship field.</param>
    /// <param name="Direction">For 'Relationship' type, the direction of the relationship (e.g., "inward", "outward").</param>
    /// <param name="RelatedListId">For 'Relationship' type, the ID of the related list.</param>
    /// <param name="RelatedTaskId">For 'Relationship' type, the ID of the related task (less common).</param>
    /// <param name="RelatedCustomFieldId">For 'Relationship' type, the ID of the related custom field.</param>
    public record TypeConfig
    (
        [property: JsonPropertyName("options")] List<Option>? Options,
        [property: JsonPropertyName("default")] object? DefaultValue,
        [property: JsonPropertyName("precision")] int? Precision,
        [property: JsonPropertyName("currency_type")] string? CurrencyType,
        [property: JsonPropertyName("placeholder")] string? Placeholder,
        [property: JsonPropertyName("end")] long? End,
        [property: JsonPropertyName("start")] long? Start,
        [property: JsonPropertyName("count")] int? Count,
        [property: JsonPropertyName("code_point")] string? CodePoint,
        [property: JsonPropertyName("tracking")] Tracking? Tracking,
        [property: JsonPropertyName("complete_on")] int? CompleteOn,
        [property: JsonPropertyName("new_drop_down")] bool? NewDropDown,
        [property: JsonPropertyName("include_guests")] bool? IncludeGuests,
        [property: JsonPropertyName("include_team_members")] bool? IncludeTeamMembers,
        [property: JsonPropertyName("single_user")] bool? SingleUser,
        [property: JsonPropertyName("is_relation")] bool? IsRelation,
        [property: JsonPropertyName("direction")] string? Direction,
        [property: JsonPropertyName("related_list_id")] string? RelatedListId,
        [property: JsonPropertyName("related_task_id")] string? RelatedTaskId,
        [property: JsonPropertyName("related_custom_field_id")] string? RelatedCustomFieldId
    );
}
