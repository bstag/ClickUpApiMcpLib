using System;
using System.Collections.Generic;
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.ResponseModels.Templates; // Assuming GetTemplatesResponse exists

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
        public async Task<IEnumerable<Template>?> GetTaskTemplatesAsync(
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

            // API returns { "templates": [...] }
            var response = await _apiConnection.GetAsync<GetTemplatesResponse>(endpoint, cancellationToken);
            return response?.Templates;
        }
    }
}
