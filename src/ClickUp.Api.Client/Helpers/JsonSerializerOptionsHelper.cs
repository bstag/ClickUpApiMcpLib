using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Helpers
{
    /// <summary>
    /// Provides shared <see cref="JsonSerializerOptions"/> for the ClickUp API client.
    /// </summary>
    public static class JsonSerializerOptionsHelper
    {
        /// <summary>
        /// Gets the default JSON serializer options configured for the ClickUp API.
        /// </summary>
        public static JsonSerializerOptions Options { get; }

        static JsonSerializerOptionsHelper()
        {
            Options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, // For serializing C# PascalCase to snake_case
                PropertyNameCaseInsensitive = true, // For deserializing snake_case from API to C# PascalCase
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) // Assumes enums are also snake_case in API
                }
            };

            // Add any other custom converters or settings here if identified from:
            // docs/plans/04-httpclient-helpers-conceptual.md
            // For example, if there are custom date formats or specific type converters needed.
        }
    }
}
