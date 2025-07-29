using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.CLI.Infrastructure
{
    /// <summary>
    /// A delegating handler that logs HTTP requests and responses for debugging purposes.
    /// </summary>
    public class HttpLoggingHandler : DelegatingHandler
    {
        private readonly IDebugStateService _debugStateService;
        private readonly bool _logRequestHeaders;
        private readonly bool _logResponseHeaders;
        private readonly bool _logRequestBody;
        private readonly bool _logResponseBody;

        public HttpLoggingHandler(
            IDebugStateService debugStateService,
            bool logRequestHeaders = true,
            bool logResponseHeaders = true,
            bool logRequestBody = true,
            bool logResponseBody = true)
        {
            _debugStateService = debugStateService ?? throw new ArgumentNullException(nameof(debugStateService));
            _logRequestHeaders = logRequestHeaders;
            _logResponseHeaders = logResponseHeaders;
            _logRequestBody = logRequestBody;
            _logResponseBody = logResponseBody;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Only log if debug is enabled
            if (_debugStateService.IsDebugEnabled)
            {
                // Log the request
                await LogRequestAsync(request);
            }

            // Send the request and get the response
            var response = await base.SendAsync(request, cancellationToken);

            // Only log if debug is enabled
            if (_debugStateService.IsDebugEnabled)
            {
                // Log the response
                await LogResponseAsync(response);
            }

            return response;
        }

        private async Task LogRequestAsync(HttpRequestMessage request)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\n=== HTTP REQUEST ===");
            sb.AppendLine($"{request.Method} {request.RequestUri}");

            if (_logRequestHeaders && request.Headers != null)
            {
                sb.AppendLine("Headers:");
                foreach (var header in request.Headers)
                {
                    sb.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (request.Content?.Headers != null)
                {
                    foreach (var header in request.Content.Headers)
                    {
                        sb.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                    }
                }
            }

            if (_logRequestBody && request.Content != null)
            {
                try
                {
                    var content = await request.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        sb.AppendLine("Body:");
                        sb.AppendLine(content);
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"Error reading request body: {ex.Message}");
                }
            }

            Console.WriteLine(sb.ToString());
        }

        private async Task LogResponseAsync(HttpResponseMessage response)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\n=== HTTP RESPONSE ===");
            sb.AppendLine($"Status: {(int)response.StatusCode} {response.StatusCode}");

            if (_logResponseHeaders && response.Headers != null)
            {
                sb.AppendLine("Headers:");
                foreach (var header in response.Headers)
                {
                    sb.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (response.Content?.Headers != null)
                {
                    foreach (var header in response.Content.Headers)
                    {
                        sb.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                    }
                }
            }

            if (_logResponseBody && response.Content != null)
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        sb.AppendLine("Body:");
                        sb.AppendLine(content);
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"Error reading response body: {ex.Message}");
                }
            }

            sb.AppendLine("=== END HTTP RESPONSE ===");
            Console.WriteLine(sb.ToString());
        }
    }
}