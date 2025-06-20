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
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) // Or CamelCase if API prefers that for enums
                }
            };

            // Add any other custom converters or settings here if identified from:
            // docs/plans/04-httpclient-helpers-conceptual.md
            // For example, if there are custom date formats or specific type converters needed.
        }
    }
}
