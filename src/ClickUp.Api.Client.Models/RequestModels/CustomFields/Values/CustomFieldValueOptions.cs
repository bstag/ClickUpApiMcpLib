using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values
{
    /// <summary>
    /// Represents additional options for a custom field value, such as specifying if time is included for a date field.
    /// </summary>
    public class CustomFieldValueOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether time information is included or relevant for the custom field value.
        /// Typically used with Date custom fields.
        /// </summary>
        [JsonPropertyName("time")]
        public bool Time { get; set; }
    }
}
