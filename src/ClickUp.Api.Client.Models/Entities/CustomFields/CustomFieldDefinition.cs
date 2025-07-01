namespace ClickUp.Api.Client.Models.Entities.CustomFields;

/// <summary>
/// Defines the structure and properties of a Custom Field.
/// This record maps to the 'Field' object in the ClickUp API specification when referring to a custom field's definition.
/// </summary>
public record CustomFieldDefinition
{
    /// <summary>
    /// Gets the unique identifier of the custom field.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the custom field.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of the custom field.
    /// </summary>
    /// <example>"text", "drop_down", "number", "currency", "date", "email", "phone", "labels", "users", "checkbox", "url", "rating", "progress_auto", "progress_manual", "tasks", "formula"</example>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type-specific configuration for this custom field.
    /// </summary>
    public CustomFieldTypeConfig TypeConfig { get; init; } = new();

    /// <summary>
    /// Gets the date when the custom field was created, as a string (Unix timestamp in milliseconds).
    /// </summary>
    public DateTimeOffset? DateCreated { get; init; }

    /// <summary>
    /// Gets a value indicating whether this custom field is hidden from guests.
    /// </summary>
    public bool HideFromGuests { get; init; }

    /// <summary>
    /// Gets a value indicating whether this custom field is required.
    /// This is often relevant in specific contexts like task types.
    /// </summary>
    public bool? Required { get; init; }
}