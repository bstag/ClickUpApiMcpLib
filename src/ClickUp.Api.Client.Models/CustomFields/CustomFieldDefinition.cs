namespace ClickUp.Api.Client.Models.CustomFields;

/// <summary>
/// Defines the structure and properties of a Custom Field.
/// Maps to the 'Field' object in the ClickUp API specification.
/// </summary>
public record CustomFieldDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // e.g., "text", "drop_down", "number"
    public CustomFieldTypeConfig TypeConfig { get; init; } = new();
    public string DateCreated { get; init; } = string.Empty; // Timestamp
    public bool HideFromGuests { get; init; }
    public bool? Required { get; init; } // Is this field required? (Often seen in task types or specific contexts)
    // Add other properties if they become apparent from further spec review or usage
}
