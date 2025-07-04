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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClickUp.Api.Client.Helpers;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ITemplatesService"/> for ClickUp Template operations.
    /// </summary>
    public class TemplatesService : ITemplatesService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<TemplatesService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatesService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public TemplatesService(IApiConnection apiConnection, ILogger<TemplatesService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<TemplatesService>.Instance;
        }


        /// <inheritdoc />
        public async Task<GetTaskTemplatesResponse> GetTaskTemplatesAsync(
            string workspaceId,
            int page,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting task templates for workspace ID: {WorkspaceId}, Page: {Page}", workspaceId, page);
            var endpoint = $"team/{workspaceId}/taskTemplate"; // team_id is workspaceId
            var queryParams = new Dictionary<string, string?>
            {
                { "page", page.ToString() } // 'page' is a required query parameter for this endpoint
            };
            endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetTaskTemplatesResponse>(endpoint, cancellationToken);
            if (response == null)
            {
                _logger.LogWarning("API call to get templates for workspace {WorkspaceId} returned null.", workspaceId);
                throw new InvalidOperationException($"API call to get templates for workspace {workspaceId} returned null.");
            }
            if (response.Templates == null)
            {
                _logger.LogWarning("API call to get templates for workspace {WorkspaceId} returned a response with null Templates list. Normalizing to empty list.", workspaceId);
                return new GetTaskTemplatesResponse(new List<TaskTemplate>());
            }
            return response;
        }
    }
}
