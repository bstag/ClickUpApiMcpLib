using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels; // As per interface for SharedHierarchy
using ClickUp.Api.Client.Models.ResponseModels.Shared; // Assuming SharedHierarchy DTO is here or in a more specific namespace

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
        public async Task<SharedHierarchy?> GetSharedHierarchyAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/shared"; // team_id is workspaceId
            // This API directly returns the SharedHierarchy object, not nested under a "shared" key usually.
            return await _apiConnection.GetAsync<SharedHierarchy>(endpoint, cancellationToken);
        }
    }
}
