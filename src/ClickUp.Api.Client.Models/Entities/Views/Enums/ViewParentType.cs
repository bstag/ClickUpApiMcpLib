using System.Text.Json.Serialization;
// Assuming a custom converter might be needed if values are non-standard,
// but for simple int mapping, it might not be if handled in logic or if API accepts int.
// For now, let's define it as a standard enum.

namespace ClickUp.Api.Client.Models.Entities.Views.Enums;

/// <summary>
/// Defines the type of a View's parent.
/// Values based on OpenAPI specification indications.
/// </summary>
public enum ViewParentType
{
    Unknown = 0, // Default

    /// <summary>
    /// Represents a Space. (Type ID: 4)
    /// </summary>
    Space = 4,

    /// <summary>
    /// Represents a Folder. (Type ID: 5)
    /// </summary>
    Folder = 5,

    /// <summary>
    /// Represents a List. (Type ID: 6)
    /// </summary>
    List = 6,

    /// <summary>
    /// Represents a Workspace/Team. (Type ID: 7)
    /// </summary>
    Workspace = 7,

    // Other types if identified in the schema.
    // For example, if a "User" or "My Views" (personal) can be a parent.
    // The spec mentions: "Parent type (7 for workspace, 4 for space, 5 for folder, 6 for list)"
    // So, these are the confirmed ones.
}
