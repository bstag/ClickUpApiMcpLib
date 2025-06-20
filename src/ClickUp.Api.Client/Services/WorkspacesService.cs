using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces;
// Potentially ClickUp.Api.Client.Models.Entities if Workspace, UserGroup etc. were directly used.

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IWorkspacesService"/> for ClickUp Workspace (Team) operations.
    /// </summary>
    public class WorkspacesService : IWorkspacesService
    {
        private readonly IApiConnection _apiConnection;
        private const string BaseWorkspaceEndpoint = "team"; // ClickUp v2 uses "team/{team_id}" for workspace context

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspacesService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public WorkspacesService(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        }

        /// <inheritdoc />
        public async Task<WorkspaceSeats?> GetWorkspaceSeatsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            // Endpoint: GET /api/v2/team/{team_id}/seats
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/seats";
            // Assuming WorkspaceSeats is the direct response DTO
            return await _apiConnection.GetAsync<WorkspaceSeats>(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<WorkspacePlan?> GetWorkspacePlanAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            // Endpoint: GET /api/v2/team/{team_id}/plan
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/plan";
            // Assuming WorkspacePlan is the direct response DTO
            return await _apiConnection.GetAsync<WorkspacePlan>(endpoint, cancellationToken);
        }
    }
}
