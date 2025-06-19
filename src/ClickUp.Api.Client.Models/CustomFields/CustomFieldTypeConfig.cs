using System.Collections.Generic;
using System.Text.Json.Serialization; // For JsonExtensionData

namespace ClickUp.Api.Client.Models.CustomFields;

/// <summary>
/// Represents the type-specific configuration for a Custom Field.
/// This object's properties can vary greatly depending on the Custom Field type.
/// </summary>
public record CustomFieldTypeConfig
{
    // For Dropdown, Labels
    public List<CustomFieldOption>? Options { get; init; }

    // For various types like Text, Number, Date, etc.
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; init; }

    // Specific known properties (can be expanded as more types are analyzed)
    // These might also be covered by AdditionalProperties but are common enough to list
    public object? DefaultValue { get; init; } // Name used 'default' in spec, which is a C# keyword
    public int? Precision { get; init; } // For Number, Currency
    public string? CurrencyType { get; init; } // For Currency
    public string? Placeholder { get; init; }
    public int? End { get; init; } // For Manual Progress
    public int? Start { get; init; } // For Manual Progress
    public int? Count { get; init; } // For Emoji (Rating)
    public string? CodePoint { get; init; } // For Emoji (Rating) - the emoji itself
    public CustomFieldTracking? Tracking { get; init; } // For Progress (Automatic), Tasks relationship
    public int? CompleteOn { get; init; } // For Progress (Automatic) - e.g., status to mark complete
    public bool? SingleUser { get; init; } // For Users custom field
    public bool? IncludeGroups { get; init; } // For Users custom field
    public bool? IncludeGuests { get; init; } // For Users custom field
    public bool? IncludeTeamMembers { get; init; } // For Users custom field
    public bool? IsRelationship { get; init; } // For Relationship custom field
    public string? RelatedEntityType { get; init; } // For Relationship (e.g., "task")
    public bool? AllowMultipleValues { get; init; } // For some relationship types
}
