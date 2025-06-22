using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
// using ClickUp.Api.Client.Models.ResponseModels; // No longer needed directly here
using ClickUp.Api.Client.Models.ResponseModels.Sharing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ISharedHierarchyService"/> for ClickUp Shared Hierarchy operations.
    /// </summary>
    public class SharedHierarchyService : ISharedHierarchyService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<SharedHierarchyService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedHierarchyService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public SharedHierarchyService(IApiConnection apiConnection, ILogger<SharedHierarchyService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<SharedHierarchyService>.Instance;
        }

        /// <inheritdoc />
        public async Task<SharedHierarchyResponse> GetSharedHierarchyAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting shared hierarchy for workspace ID: {WorkspaceId}", workspaceId);
            var endpoint = $"team/{workspaceId}/shared"; // team_id is workspaceId
            var response = await _apiConnection.GetAsync<SharedHierarchyResponse>(endpoint, cancellationToken);
            return response ?? throw new InvalidOperationException("API returned null response for SharedHierarchy.");
        }
    }
}
