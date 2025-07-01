using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.CustomFields;

/// <summary>
/// Represents the type-specific configuration for a Custom Field.
/// The properties within this record vary greatly depending on the <see cref="CustomFieldDefinition.Type"/> of the custom field.
/// </summary>
public record CustomFieldTypeConfig
{
    /// <summary>
    /// Gets the list of options for 'Dropdown' or 'Labels' type custom fields.
    /// </summary>
    public List<CustomFieldOption>? Options { get; init; }

    /// <summary>
    /// Gets or sets additional properties that are not explicitly defined.
    /// This is used to capture any type-specific configurations not covered by named properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; init; }

    /// <summary>
    /// Gets the default value for the custom field. The type of this object depends on the custom field type.
    /// Note: In the API, this property might be named "default", which is a C# keyword.
    /// </summary>
    public object? DefaultValue { get; init; }

    /// <summary>
    /// Gets the precision for 'Number' or 'Currency' type custom fields (number of decimal places).
    /// </summary>
    public int? Precision { get; init; }

    /// <summary>
    /// Gets the currency symbol or code for 'Currency' type custom fields.
    /// </summary>
    /// <example>"USD", "$"</example>
    public string? CurrencyType { get; init; }

    /// <summary>
    /// Gets the placeholder text for input-based custom fields like 'Text'.
    /// </summary>
    public string? Placeholder { get; init; }

    /// <summary>
    /// Gets the end value for 'Progress (Manual)' type custom fields.
    /// </summary>
    public int? End { get; init; }

    /// <summary>
    /// Gets the start value for 'Progress (Manual)' type custom fields.
    /// </summary>
    public int? Start { get; init; }

    /// <summary>
    /// Gets the count of items for 'Rating' (emoji) type custom fields (e.g., number of stars).
    /// </summary>
    public int? Count { get; init; }

    /// <summary>
    /// Gets the Unicode code point for the emoji used in 'Rating' type custom fields.
    /// </summary>
    public string? CodePoint { get; init; }

    /// <summary>
    /// Gets the tracking configuration for 'Progress (Automatic)' or 'Tasks' (relationship) type custom fields.
    /// </summary>
    public CustomFieldTracking? Tracking { get; init; }

    /// <summary>
    /// Gets the status type or ID that marks a task as complete, relevant for 'Progress (Automatic)' custom fields.
    /// </summary>
    public int? CompleteOn { get; init; } // API docs suggest this can be a status type string or integer ID. Using int for now.

    /// <summary>
    /// Gets a value indicating whether only a single user can be selected for 'Users' type custom fields.
    /// </summary>
    public bool? SingleUser { get; init; }

    /// <summary>
    /// Gets a value indicating whether user groups can be selected for 'Users' type custom fields.
    /// </summary>
    public bool? IncludeGroups { get; init; }

    /// <summary>
    /// Gets a value indicating whether guests can be selected for 'Users' type custom fields.
    /// </summary>
    public bool? IncludeGuests { get; init; }

    /// <summary>
    /// Gets a value indicating whether team members can be selected for 'Users' type custom fields.
    /// </summary>
    public bool? IncludeTeamMembers { get; init; }

    /// <summary>
    /// Gets a value indicating whether this custom field represents a relationship to another entity.
    /// </summary>
    public bool? IsRelationship { get; init; }

    /// <summary>
    /// Gets the type of entity this custom field is related to (e.g., "task").
    /// Relevant for 'Relationship' or 'Tasks' type custom fields.
    /// </summary>
    public string? RelatedEntityType { get; init; }

    /// <summary>
    /// Gets a value indicating whether multiple values can be selected.
    /// Relevant for some relationship types or 'Labels' custom fields.
    /// </summary>
    public bool? AllowMultipleValues { get; init; }
}
