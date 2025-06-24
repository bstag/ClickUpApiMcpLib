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
                    new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower), // Assumes enums are also snake_case in API
                    new Models.Serialization.Converters.UnixEpochDateTimeOffsetConverter(),
                    new Models.Serialization.Converters.NullableUnixEpochDateTimeOffsetConverter(),
                    new Models.Serialization.Converters.JsonStringOrNumberConverter() // Added the new converter
                }
            };

            // Further custom converters or settings can be added here.
        }
    }
}