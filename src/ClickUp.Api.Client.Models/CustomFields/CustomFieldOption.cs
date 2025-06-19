namespace ClickUp.Api.Client.Models.CustomFields;

/// <summary>
/// Represents an option within a Custom Field (e.g., for Dropdown or Labels type).
/// </summary>
public record CustomFieldOption
{
    public string Id { get; init; } = string.Empty;
    public string? Name { get; init; } // Used for Dropdown options (label is also there)
    public string? Label { get; init; } // Used for Label options
    public string? Color { get; init; }
    public int? Value { get; init; } // Seen in some type_config options (e.g. for emoji rating options, though less common here)
    public string? Type { get; init; } // Potentially indicates option type if varied
    public int? OrderIndex { get; init; }
}
