using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Strategies
{
    /// <summary>
    /// Interface for serialization strategies that can be used to serialize and deserialize data.
    /// </summary>
    public interface ISerializationStrategy
    {
        /// <summary>
        /// Gets the name of the serialization strategy.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the content type associated with this serialization strategy.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets a value indicating whether this serialization strategy is enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Serializes an object to a string.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <param name="options">Optional serialization options.</param>
        /// <returns>The serialized string representation of the object.</returns>
        string Serialize<T>(T value, object? options = null);

        /// <summary>
        /// Serializes an object to a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <param name="options">Optional serialization options.</param>
        /// <returns>The serialized byte array representation of the object.</returns>
        byte[] SerializeToBytes<T>(T value, object? options = null);

        /// <summary>
        /// Serializes an object to a stream asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="options">Optional serialization options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SerializeAsync<T>(Stream stream, T value, object? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deserializes a string to an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="options">Optional deserialization options.</param>
        /// <returns>The deserialized object.</returns>
        T? Deserialize<T>(string json, object? options = null);

        /// <summary>
        /// Deserializes a byte array to an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="bytes">The byte array to deserialize.</param>
        /// <param name="options">Optional deserialization options.</param>
        /// <returns>The deserialized object.</returns>
        T? DeserializeFromBytes<T>(byte[] bytes, object? options = null);

        /// <summary>
        /// Deserializes a stream to an object asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="options">Optional deserialization options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The deserialized object.</returns>
        Task<T?> DeserializeAsync<T>(Stream stream, object? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates whether the provided data can be deserialized by this strategy.
        /// </summary>
        /// <param name="data">The data to validate.</param>
        /// <returns>True if the data can be deserialized; otherwise, false.</returns>
        bool CanDeserialize(string data);

        /// <summary>
        /// Validates whether the provided byte array can be deserialized by this strategy.
        /// </summary>
        /// <param name="bytes">The byte array to validate.</param>
        /// <returns>True if the data can be deserialized; otherwise, false.</returns>
        bool CanDeserialize(byte[] bytes);
    }
}