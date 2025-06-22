using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces;
// Potentially ClickUp.Api.Client.Models.Entities if Workspace, UserGroup etc. were directly used.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IWorkspacesService"/> for ClickUp Workspace (Team) operations.
    /// </summary>
    public class WorkspacesService : IWorkspacesService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<WorkspacesService> _logger;
        private const string BaseWorkspaceEndpoint = "team"; // ClickUp v2 uses "team/{team_id}" for workspace context

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspacesService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public WorkspacesService(IApiConnection apiConnection, ILogger<WorkspacesService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<WorkspacesService>.Instance;
        }

        /// <inheritdoc />
        public async Task<GetWorkspaceSeatsResponse> GetWorkspaceSeatsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting workspace seats for workspace ID: {WorkspaceId}", workspaceId);
            // Endpoint: GET /api/v2/team/{team_id}/seats
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/seats";
            var response = await _apiConnection.GetAsync<GetWorkspaceSeatsResponse>(endpoint, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting workspace seats for workspace {workspaceId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<GetWorkspacePlanResponse> GetWorkspacePlanAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting workspace plan for workspace ID: {WorkspaceId}", workspaceId);
            // Endpoint: GET /api/v2/team/{team_id}/plan
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/plan";
            var response = await _apiConnection.GetAsync<GetWorkspacePlanResponse>(endpoint, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting workspace plan for workspace {workspaceId}.");
            }
            return response;
        }
    }
}
