using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Helpers
{
    /// <summary>
    /// Provides extension methods for <see cref="HttpContent"/>.
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Reads the <see cref="HttpContent"/> and deserializes the JSON content as the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the JSON content into.</typeparam>
        /// <param name="content">The <see cref="HttpContent"/> to read from.</param>
        /// <param name="options">Optional <see cref="JsonSerializerOptions"/>. If null, <see cref="JsonSerializerOptionsHelper.Options"/> will be used.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the deserialized object.</returns>
        public static async Task<T?> ReadFromJsonAsync<T>(
            this HttpContent content,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            options ??= JsonSerializerOptionsHelper.Options;
            var stream = await content.ReadAsStreamAsync(cancellationToken);
            // Consider adding a check for null or empty stream if necessary, though DeserializeAsync might handle it.
            return await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
        }
    }
}
