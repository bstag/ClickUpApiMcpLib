using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Serialization.Converters
{
    /// <summary>
    /// Converts a Unix epoch time (in milliseconds) to/from <see cref="DateTimeOffset"/>.
    /// ClickUp API often returns timestamps as strings or numbers representing milliseconds since epoch.
    /// </summary>
    public class UnixEpochDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        /// <inheritdoc />
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (long.TryParse(reader.GetString(), out long milliseconds))
                {
                    return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
                }
                // Fallback for date strings if necessary, though primary goal is Unix epoch
                if (DateTimeOffset.TryParse(reader.GetString(), out DateTimeOffset dateFromString))
                {
                    return dateFromString;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64());
            }

            throw new JsonException($"Unexpected token type {reader.TokenType} when parsing DateTimeOffset from Unix epoch time.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            long milliseconds = value.ToUnixTimeMilliseconds();
            writer.WriteStringValue(milliseconds.ToString());
        }
    }

    /// <summary>
    /// Converts a nullable Unix epoch time (in milliseconds) to/from <see cref="DateTimeOffset?"/>.
    /// Handles null values and cases where the timestamp might be "0" or an empty string for null-like values.
    /// </summary>
    public class NullableUnixEpochDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
    {
        /// <inheritdoc />
        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue) || stringValue == "0") // "0" often indicates a null or unset date in some APIs
                {
                    return null;
                }
                if (long.TryParse(stringValue, out long milliseconds))
                {
                    return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
                }
                // Fallback for date strings if necessary
                if (DateTimeOffset.TryParse(stringValue, out DateTimeOffset dateFromString))
                {
                    return dateFromString;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                long milliseconds = reader.GetInt64();
                if (milliseconds == 0) // Consider 0 as null for numeric timestamps too, if appropriate
                {
                    // This behavior depends on API contract. If 0 is a valid epoch time, this should be removed.
                    // For ClickUp, '0' usually means not set.
                    return null;
                }
                return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
            }

            throw new JsonException($"Unexpected token type {reader.TokenType} when parsing nullable DateTimeOffset from Unix epoch time.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                long milliseconds = value.Value.ToUnixTimeMilliseconds();
                writer.WriteStringValue(milliseconds.ToString());
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
