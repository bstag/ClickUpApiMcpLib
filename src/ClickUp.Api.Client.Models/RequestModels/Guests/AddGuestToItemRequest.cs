using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Guests;

/// <summary>
/// Represents the request model for adding a guest to a generic item (e.g., Folder, List, CuTask).
/// The specific item type is usually determined by the endpoint used.
/// </summary>
public class AddGuestToItemRequest
{
    /// <summary>
    /// Gets or sets the permission level for the guest on the item.
    /// The meaning of the integer value can vary based on the item type and API version.
    /// Refer to ClickUp API documentation for specific permission level codes.
    /// </summary>
    /// <example>1 (view), 2 (comment), 3 (edit), 4 (create/full) - these are examples and may not be accurate.</example>
    [JsonPropertyName("permission_level")]
    public int PermissionLevel { get; set; }
}
