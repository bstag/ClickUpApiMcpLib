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
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using ClickUp.Api.Client.Models.ResponseModels.Spaces; // Assuming GetSpacesResponse exists
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ISpacesService"/> for ClickUp Space operations.
    /// </summary>
    public class SpacesService : ISpacesService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<SpacesService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpacesService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public SpacesService(IApiConnection apiConnection, ILogger<SpacesService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<SpacesService>.Instance;
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
        public async Task<IEnumerable<Space>> GetSpacesAsync(
            string workspaceId,
            bool? archived = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting spaces for workspace ID: {WorkspaceId}, Archived: {Archived}", workspaceId, archived);
            var endpoint = $"team/{workspaceId}/space"; // team_id is workspaceId
            var queryParams = new Dictionary<string, string?>();
            if (archived.HasValue) queryParams["archived"] = archived.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetSpacesResponse>(endpoint, cancellationToken); // API returns {"spaces": [...]}
            return response?.Spaces ?? Enumerable.Empty<Space>();
        }

        /// <inheritdoc />
        public async Task<Space> CreateSpaceAsync(
            string workspaceId,
            CreateSpaceRequest createSpaceRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating space in workspace ID: {WorkspaceId}, Name: {SpaceName}", workspaceId, createSpaceRequest.Name);
            var endpoint = $"team/{workspaceId}/space";
            var space = await _apiConnection.PostAsync<CreateSpaceRequest, Space>(endpoint, createSpaceRequest, cancellationToken);
            if (space == null)
            {
                throw new InvalidOperationException($"Failed to create space in workspace {workspaceId} or API returned an unexpected null response.");
            }
            return space;
        }

        /// <inheritdoc />
        public async Task<Space> GetSpaceAsync(
            string spaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting space ID: {SpaceId}", spaceId);
            var endpoint = $"space/{spaceId}";
            var space = await _apiConnection.GetAsync<Space>(endpoint, cancellationToken);
            if (space == null)
            {
                throw new InvalidOperationException($"Failed to get space {spaceId} or API returned an unexpected null response.");
            }
            return space;
        }

        /// <inheritdoc />
        public async Task<Space> UpdateSpaceAsync(
            string spaceId,
            UpdateSpaceRequest updateSpaceRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating space ID: {SpaceId}, Name: {SpaceName}", spaceId, updateSpaceRequest.Name);
            var endpoint = $"space/{spaceId}";
            var space = await _apiConnection.PutAsync<UpdateSpaceRequest, Space>(endpoint, updateSpaceRequest, cancellationToken);
            if (space == null)
            {
                throw new InvalidOperationException($"Failed to update space {spaceId} or API returned an unexpected null response.");
            }
            return space;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteSpaceAsync(
            string spaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting space ID: {SpaceId}", spaceId);
            var endpoint = $"space/{spaceId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }
    }
}
