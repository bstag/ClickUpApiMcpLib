using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
// using ClickUp.Api.Client.Models.ResponseModels; // No longer needed directly here
using ClickUp.Api.Client.Models.ResponseModels.Sharing;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ISharedHierarchyService"/> for ClickUp Shared Hierarchy operations.
    /// </summary>
    public class SharedHierarchyService : ISharedHierarchyService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedHierarchyService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public SharedHierarchyService(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        }

        /// <inheritdoc />
        public async Task<SharedHierarchyResponse> GetSharedHierarchyAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/shared"; // team_id is workspaceId
            var response = await _apiConnection.GetAsync<SharedHierarchyResponse>(endpoint, cancellationToken);
            return response ?? throw new InvalidOperationException("API returned null response for SharedHierarchy.");
        }
    }
}
