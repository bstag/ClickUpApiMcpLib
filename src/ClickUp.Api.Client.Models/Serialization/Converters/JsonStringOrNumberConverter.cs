using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Serialization.Converters
{
    /// <summary>
    /// Converts a JSON token to a string. If the token is a number, it converts the number to its string representation.
    /// </summary>
    public class JsonStringOrNumberConverter : JsonConverter<string>
    {
        /// <inheritdoc />
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                // Read the number and convert it to a string
                if (reader.TryGetDouble(out double doubleValue))
                {
                    return doubleValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                if (reader.TryGetInt64(out long longValue))
                {
                    return longValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                // Fallback for other number types: get the raw text of the token.
                // This requires parsing the current token as a JsonElement.
                using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                {
                    return doc.RootElement.GetRawText();
                }
            }

            // Handle null tokens if the property is nullable, otherwise, it might be an error
            // For a JsonConverter<string>, returning null for a null token is standard.
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            throw new JsonException($"Unexpected token type {reader.TokenType} when expecting a string or number.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
