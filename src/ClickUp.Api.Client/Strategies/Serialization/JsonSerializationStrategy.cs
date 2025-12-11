using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Strategies;
using Microsoft.Extensions.Logging;

namespace ClickUp.Api.Client.Strategies.Serialization
{
    /// <summary>
    /// JSON serialization strategy implementation using System.Text.Json.
    /// </summary>
    public class JsonSerializationStrategy : ISerializationStrategy
    {
        private readonly ILogger<JsonSerializationStrategy>? _logger;
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializationStrategy"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="options">The JSON serializer options.</param>
        public JsonSerializationStrategy(
            ILogger<JsonSerializationStrategy>? logger = null,
            JsonSerializerOptions? options = null)
        {
            _logger = logger;
            _options = options ?? CreateDefaultOptions();
        }

        /// <inheritdoc />
        public string Name => "Json";

        /// <inheritdoc />
        public bool IsEnabled => true;

        /// <inheritdoc />
        public string ContentType => "application/json";

        /// <inheritdoc />
        public Encoding Encoding => Encoding.UTF8;

        /// <inheritdoc />
        public string Serialize<T>(T obj, object? options = null)
        {
            if (obj == null)
                return "null";

            try
            {
                var serializerOptions = (options as JsonSerializerOptions) ?? _options;
                var json = JsonSerializer.Serialize(obj, serializerOptions);
                _logger?.LogDebug("Serialized object of type {Type} to JSON ({Length} characters)", 
                    typeof(T).Name, json.Length);
                return json;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to serialize object of type {Type} to JSON", typeof(T).Name);
                throw new InvalidOperationException($"Failed to serialize object of type {typeof(T).Name} to JSON", ex);
            }
        }

        /// <inheritdoc />
        public byte[] SerializeToBytes<T>(T obj, object? options = null)
        {
            if (obj == null)
                return Encoding.GetBytes("null");

            try
            {
                var serializerOptions = (options as JsonSerializerOptions) ?? _options;
                var bytes = JsonSerializer.SerializeToUtf8Bytes(obj, serializerOptions);
                _logger?.LogDebug("Serialized object of type {Type} to JSON bytes ({Length} bytes)", 
                    typeof(T).Name, bytes.Length);
                return bytes;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to serialize object of type {Type} to JSON bytes", typeof(T).Name);
                throw new InvalidOperationException($"Failed to serialize object of type {typeof(T).Name} to JSON bytes", ex);
            }
        }

        /// <inheritdoc />
        public async Task SerializeAsync<T>(Stream stream, T obj, object? options = null, CancellationToken cancellationToken = default)
        {
            if (obj == null)
            {
                var nullBytes = Encoding.GetBytes("null");
                await stream.WriteAsync(nullBytes, 0, nullBytes.Length, cancellationToken).ConfigureAwait(false);
                return;
            }

            try
            {
                var serializerOptions = (options as JsonSerializerOptions) ?? _options;
                await JsonSerializer.SerializeAsync(stream, obj, serializerOptions, cancellationToken).ConfigureAwait(false);
                
                _logger?.LogDebug("Async serialized object of type {Type} to JSON stream", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to async serialize object of type {Type} to JSON stream", typeof(T).Name);
                throw new InvalidOperationException($"Failed to async serialize object of type {typeof(T).Name} to JSON stream", ex);
            }
        }

        /// <inheritdoc />
        public T? Deserialize<T>(string data, object? options = null)
        {
            if (string.IsNullOrEmpty(data))
                return default;

            if (data.Trim() == "null")
                return default;

            try
            {
                var serializerOptions = (options as JsonSerializerOptions) ?? _options;
                var result = JsonSerializer.Deserialize<T>(data, serializerOptions);
                _logger?.LogDebug("Deserialized JSON ({Length} characters) to object of type {Type}", 
                    data.Length, typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to deserialize JSON to object of type {Type}. Data: {Data}", 
                    typeof(T).Name, data.Length > 200 ? data.Substring(0, 200) + "..." : data);
                throw new InvalidOperationException($"Failed to deserialize JSON to object of type {typeof(T).Name}", ex);
            }
        }

        /// <inheritdoc />
        public T? DeserializeFromBytes<T>(byte[] data, object? options = null)
        {
            if (data == null || data.Length == 0)
                return default;

            try
            {
                var serializerOptions = (options as JsonSerializerOptions) ?? _options;
                var result = JsonSerializer.Deserialize<T>(data, serializerOptions);
                _logger?.LogDebug("Deserialized JSON bytes ({Length} bytes) to object of type {Type}", 
                    data.Length, typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to deserialize JSON bytes to object of type {Type}. Data length: {Length}", 
                    typeof(T).Name, data.Length);
                throw new InvalidOperationException($"Failed to deserialize JSON bytes to object of type {typeof(T).Name}", ex);
            }
        }

        /// <inheritdoc />
        public async Task<T?> DeserializeAsync<T>(Stream stream, object? options = null, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                return default;

            try
            {
                var serializerOptions = (options as JsonSerializerOptions) ?? _options;
                var result = await JsonSerializer.DeserializeAsync<T>(stream, serializerOptions, cancellationToken).ConfigureAwait(false);
                _logger?.LogDebug("Async deserialized JSON stream to object of type {Type}", typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to async deserialize JSON stream to object of type {Type}", typeof(T).Name);
                throw new InvalidOperationException($"Failed to async deserialize JSON stream to object of type {typeof(T).Name}", ex);
            }
        }

        /// <inheritdoc />
        public bool CanDeserialize(string data)
        {
            if (string.IsNullOrEmpty(data))
                return false;

            try
            {
                using var document = JsonDocument.Parse(data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc />
        public bool CanDeserialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return false;

            try
            {
                using var document = JsonDocument.Parse(bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static JsonSerializerOptions CreateDefaultOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }
    }
}