using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

public class TaskRelationshipCustomFieldValueRequest : SetCustomFieldValueRequest
{
    [JsonPropertyName("value")]
    public TaskRelationshipActionValue Value { get; set; } // Assuming TaskRelationshipActionValue is a defined type

    public TaskRelationshipCustomFieldValueRequest(TaskRelationshipActionValue value)
    {
        Value = value;
    }
    public TaskRelationshipCustomFieldValueRequest() { /* Value will be initialized by deserializer or needs default if used directly */ }
}
