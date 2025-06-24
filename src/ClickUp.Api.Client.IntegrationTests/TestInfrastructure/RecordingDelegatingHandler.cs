using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.IntegrationTests.TestInfrastructure
{
    public class RecordingDelegatingHandler : DelegatingHandler
    {
        private readonly string _basePath;
        // HttpClientHandler is a concrete HttpMessageHandler.
        // It's often used as the default final handler in the chain.
        private static readonly HttpMessageHandler _defaultInnerHandler = new HttpClientHandler();


        // Used to ensure that the handler is added only once per HttpClient.
        public bool IsAdded { get; internal set; }

        public RecordingDelegatingHandler(string basePath)
        {
            _basePath = basePath;
            // DelegatingHandler.InnerHandler expects an HttpMessageHandler.
            // If this handler is the last custom one before the actual transport,
            // it might be set by the HttpClient pipeline itself.
            // However, setting a default can be useful if it's used standalone or if the pipeline doesn't set one.
            // For now, let's rely on the pipeline to set it if this handler is added via AddHttpMessageHandler.
            // If constructing manually and it's the final delegating handler, then:
            // InnerHandler = new HttpClientHandler(); // or a shared instance
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            // Check if recording is enabled (e.g., via a toggle or if the response is successful)
            // For now, let's assume we always try to record if this handler is in the pipeline
            // and the request is a GET request (simplification for PoC)
            if (request.Method == HttpMethod.Get && response.IsSuccessStatusCode)
            {
                var path = GenerateFilePath(request);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    await File.WriteAllTextAsync(path, content, cancellationToken);
                    Console.WriteLine($"[RecordingDelegatingHandler] Recorded response for {request.Method} {request.RequestUri} to {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RecordingDelegatingHandler] Failed to record response for {request.Method} {request.RequestUri} to {path}. Error: {ex.Message}");
                    // Optionally re-throw or handle as needed
                }
            }

            return response;
        }

        private string GenerateFilePath(HttpRequestMessage request)
        {
            var uri = request.RequestUri;
            var method = request.Method.Method; // GET, POST, etc.

            // Simplified naming: method_host_pathsegments_queryhash.json
            // Example: GET_api_clickup_com_api_v2_user.json
            // Example: GET_api_clickup_com_api_v2_team_teamId_space_spaceId_folder.json
            // Using a hash for the query string to keep filenames manageable and somewhat deterministic for identical queries.

            var pathSegments = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var sanitizedPath = string.Join("_", pathSegments).Replace(".", "_"); // Replace dots to avoid issues with filenames/extensions

            string fileName;
            if (!string.IsNullOrWhiteSpace(uri.Query))
            {
                using var sha256 = SHA256.Create();
                var queryBytes = Encoding.UTF8.GetBytes(uri.Query);
                var queryHashBytes = sha256.ComputeHash(queryBytes);
                var queryHash = BitConverter.ToString(queryHashBytes).Replace("-", "").ToLowerInvariant().Substring(0, 8);
                fileName = $"{method}_{sanitizedPath}_{queryHash}.json";
            }
            else
            {
                fileName = $"{method}_{sanitizedPath}.json";
            }

            // This is a very generic path. Tests will need to organize these files later
            // or the recording mechanism will need to be more sophisticated in where it saves them
            // based on service/method.
            // For PoC, save directly into _basePath, making them easier to find and move.
            // Later, a more sophisticated mechanism will place them in service/method folders.
            return Path.Combine(_basePath, fileName);
        }
    }
}
