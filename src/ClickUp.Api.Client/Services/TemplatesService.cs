using System;
using System.Collections.Generic;
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Templates; // For TaskTemplate if it were used directly
using ClickUp.Api.Client.Models.ResponseModels.Templates;
using System.Linq; // For Enumerable.Empty, though not directly used here but good for consistency

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ITemplatesService"/> for ClickUp Template operations.
    /// </summary>
    public class TemplatesService : ITemplatesService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatesService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public TemplatesService(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        }

        private string BuildQueryString(Dictionary<string, string?> queryParams)
        {
            if (queryParams == null || !queryParams.Any(kvp => kvp.Value != null))
            {
                return string.Empty;
            }

            var sb = new StringBuilder("?");
            foreach (var kvp in queryParams)
            {
                if (kvp.Value != null)
                {
                    if (sb.Length > 1) sb.Append('&');
                    sb.Append($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
                }
            }
            return sb.ToString();
        }

        /// <inheritdoc />
        public async Task<GetTaskTemplatesResponse> GetTaskTemplatesAsync(
            string workspaceId,
            int page,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/taskTemplate"; // team_id is workspaceId
            var queryParams = new Dictionary<string, string?>
            {
                { "page", page.ToString() } // 'page' is a required query parameter for this endpoint
            };
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetTaskTemplatesResponse>(endpoint, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting task templates for workspace {workspaceId}.");
            }
            // The GetTaskTemplatesResponse contains the List<TaskTemplate> as "Templates"
            return response;
        }
    }
}
