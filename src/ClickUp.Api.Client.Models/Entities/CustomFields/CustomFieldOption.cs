namespace ClickUp.Api.Client.Models.Entities.CustomFields;

/// <summary>
/// Represents an option within a Custom Field, typically used for 'Dropdown', 'Labels', or 'Rating' types.
/// </summary>
public record CustomFieldOption
{
    /// <summary>
    /// Gets the unique identifier of the custom field option.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the option, primarily used for 'Dropdown' type custom fields.
    /// For 'Labels' type, the <see cref="Label"/> property is typically used.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the label of the option, primarily used for 'Labels' type custom fields.
    /// </summary>
    public string? Label { get; init; }

    /// <summary>
    /// Gets the color associated with the option, often used for 'Dropdown' or 'Labels'.
    /// </summary>
    /// <example>"#FF0000"</example>
    public string? Color { get; init; }

    /// <summary>
    /// Gets an optional integer value associated with the option.
    /// This is sometimes used in specific configurations, like options for an emoji rating custom field.
    /// </summary>
    public int? Value { get; init; }

    /// <summary>
    /// Gets the type of the option, if specified. This is not commonly used but available.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets the order index of the option, determining its position within the list of options.
    /// </summary>
    public int? OrderIndex { get; init; }
}
