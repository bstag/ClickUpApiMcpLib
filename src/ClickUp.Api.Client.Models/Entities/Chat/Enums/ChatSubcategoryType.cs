using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat.Enums
{
    // Note: The OpenAPI spec mentions values like 1, 2, 3 for this.
    // If the API sends these as integers in JSON, this enum will need a custom converter
    // or the properties using it should be of type int.
    // Assuming here that the API might send/accept these as strings due to JsonStringEnumConverter,
    // which is not typical for numeric enum values.
    // Or, these are symbolic names that happen to be digits.
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChatSubcategoryType
    {
        [EnumMember(Value = "1")]
        Type1, // Example: General Chat or Uncategorized

        [EnumMember(Value = "2")]
        Type2, // Example: Project-related Chat

        [EnumMember(Value = "3")]
        Type3  // Example: Task-specific Chat
    }
}
