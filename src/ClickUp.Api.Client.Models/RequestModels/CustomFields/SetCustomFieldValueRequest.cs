namespace ClickUp.Api.Client.Models.RequestModels.CustomFields;

/// <summary>
/// Base class for setting the value of a Custom Field.
/// Specific field types will have their own derived request DTOs.
/// </summary>
public abstract class SetCustomFieldValueRequest
{
    // This class can be empty for now, serving as a polymorphic base type.
    // It could later include common properties if any are identified (e.g., 'value_options').
}
