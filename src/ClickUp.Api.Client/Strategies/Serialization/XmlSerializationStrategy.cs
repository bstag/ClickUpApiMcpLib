using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ClickUp.Api.Client.Abstractions.Strategies;
using Microsoft.Extensions.Logging;

namespace ClickUp.Api.Client.Strategies.Serialization
{
    /// <summary>
    /// XML serialization strategy implementation using System.Xml.Serialization.
    /// </summary>
    public class XmlSerializationStrategy : ISerializationStrategy
    {
        private readonly ILogger<XmlSerializationStrategy>? _logger;
        private readonly XmlWriterSettings _writerSettings;
        private readonly XmlReaderSettings _readerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationStrategy"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="writerSettings">The XML writer settings.</param>
        /// <param name="readerSettings">The XML reader settings.</param>
        public XmlSerializationStrategy(
            ILogger<XmlSerializationStrategy>? logger = null,
            XmlWriterSettings? writerSettings = null,
            XmlReaderSettings? readerSettings = null)
        {
            _logger = logger;
            _writerSettings = writerSettings ?? CreateDefaultWriterSettings();
            _readerSettings = readerSettings ?? CreateDefaultReaderSettings();
        }

        /// <inheritdoc />
        public string Name => "Xml";

        /// <inheritdoc />
        public bool IsEnabled => true;

        /// <inheritdoc />
        public string ContentType => "application/xml";

        /// <inheritdoc />
        public Encoding Encoding => Encoding.UTF8;

        /// <inheritdoc />
        public string Serialize<T>(T obj, object? options = null)
        {
            if (obj == null)
                return "<null />";

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using var stringWriter = new StringWriter();
                var writerSettings = (options as XmlWriterSettings) ?? _writerSettings;
                using var xmlWriter = XmlWriter.Create(stringWriter, writerSettings);
                
                serializer.Serialize(xmlWriter, obj);
                var xml = stringWriter.ToString();
                
                _logger?.LogDebug("Serialized object of type {Type} to XML ({Length} characters)", 
                    typeof(T).Name, xml.Length);
                return xml;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to serialize object of type {Type} to XML", typeof(T).Name);
                throw new InvalidOperationException($"Failed to serialize object of type {typeof(T).Name} to XML", ex);
            }
        }

        /// <inheritdoc />
        public byte[] SerializeToBytes<T>(T obj, object? options = null)
        {
            if (obj == null)
                return Encoding.GetBytes("<null />");

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using var memoryStream = new MemoryStream();
                var writerSettings = (options as XmlWriterSettings) ?? _writerSettings;
                using var xmlWriter = XmlWriter.Create(memoryStream, writerSettings);
                
                serializer.Serialize(xmlWriter, obj);
                xmlWriter.Flush();
                
                var bytes = memoryStream.ToArray();
                _logger?.LogDebug("Serialized object of type {Type} to XML bytes ({Length} bytes)", 
                    typeof(T).Name, bytes.Length);
                return bytes;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to serialize object of type {Type} to XML bytes", typeof(T).Name);
                throw new InvalidOperationException($"Failed to serialize object of type {typeof(T).Name} to XML bytes", ex);
            }
        }

        /// <inheritdoc />
        public async Task SerializeAsync<T>(Stream stream, T obj, object? options = null, CancellationToken cancellationToken = default)
        {
            if (obj == null)
            {
                var nullBytes = Encoding.GetBytes("<null />");
                await stream.WriteAsync(nullBytes, 0, nullBytes.Length, cancellationToken).ConfigureAwait(false);
                return;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                var writerSettings = (options as XmlWriterSettings) ?? _writerSettings;
                using var xmlWriter = XmlWriter.Create(stream, writerSettings);
                
                // XML serialization is inherently synchronous, but we can wrap it in a task
                await Task.Run(() => serializer.Serialize(xmlWriter, obj), cancellationToken).ConfigureAwait(false);
                
                _logger?.LogDebug("Async serialized object of type {Type} to XML stream", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to async serialize object of type {Type} to XML stream", typeof(T).Name);
                throw new InvalidOperationException($"Failed to async serialize object of type {typeof(T).Name} to XML stream", ex);
            }
        }

        /// <inheritdoc />
        public T? Deserialize<T>(string data, object? options = null)
        {
            if (string.IsNullOrEmpty(data))
                return default;

            if (data.Trim() == "<null />")
                return default;

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using var stringReader = new StringReader(data);
                var readerSettings = (options as XmlReaderSettings) ?? _readerSettings;
                using var xmlReader = XmlReader.Create(stringReader, readerSettings);
                
                var result = (T?)serializer.Deserialize(xmlReader);
                _logger?.LogDebug("Deserialized XML ({Length} characters) to object of type {Type}", 
                    data.Length, typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to deserialize XML to object of type {Type}. Data: {Data}", 
                    typeof(T).Name, data.Length > 200 ? data.Substring(0, 200) + "..." : data);
                throw new InvalidOperationException($"Failed to deserialize XML to object of type {typeof(T).Name}", ex);
            }
        }

        /// <inheritdoc />
        public T? DeserializeFromBytes<T>(byte[] data, object? options = null)
        {
            if (data == null || data.Length == 0)
                return default;

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using var memoryStream = new MemoryStream(data);
                var readerSettings = (options as XmlReaderSettings) ?? _readerSettings;
                using var xmlReader = XmlReader.Create(memoryStream, readerSettings);
                
                var result = (T?)serializer.Deserialize(xmlReader);
                _logger?.LogDebug("Deserialized XML bytes ({Length} bytes) to object of type {Type}", 
                    data.Length, typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to deserialize XML bytes to object of type {Type}. Data length: {Length}", 
                    typeof(T).Name, data.Length);
                throw new InvalidOperationException($"Failed to deserialize XML bytes to object of type {typeof(T).Name}", ex);
            }
        }

        /// <inheritdoc />
        public async Task<T?> DeserializeAsync<T>(Stream stream, object? options = null, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                return default;

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                var readerSettings = (options as XmlReaderSettings) ?? _readerSettings;
                
                // XML deserialization is inherently synchronous, but we can wrap it in a task
                var result = await Task.Run(() =>
                {
                    using var xmlReader = XmlReader.Create(stream, readerSettings);
                    return (T?)serializer.Deserialize(xmlReader);
                }, cancellationToken).ConfigureAwait(false);
                
                _logger?.LogDebug("Async deserialized XML stream to object of type {Type}", typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to async deserialize XML stream to object of type {Type}", typeof(T).Name);
                throw new InvalidOperationException($"Failed to async deserialize XML stream to object of type {typeof(T).Name}", ex);
            }
        }

        /// <inheritdoc />
        public bool CanDeserialize(string data)
        {
            if (string.IsNullOrEmpty(data))
                return false;

            try
            {
                using var stringReader = new StringReader(data);
                using var xmlReader = XmlReader.Create(stringReader, _readerSettings);
                
                // Just try to parse the XML structure without deserializing to a specific type
                while (xmlReader.Read()) { }
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
                using var memoryStream = new MemoryStream(bytes);
                using var xmlReader = XmlReader.Create(memoryStream, _readerSettings);
                
                // Just try to parse the XML structure without deserializing to a specific type
                while (xmlReader.Read()) { }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static XmlWriterSettings CreateDefaultWriterSettings()
        {
            return new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = false,
                WriteEndDocumentOnClose = true
            };
        }

        private static XmlReaderSettings CreateDefaultReaderSettings()
        {
            return new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true
            };
        }
    }
}