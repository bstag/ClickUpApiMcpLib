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
using ClickUp.Api.Client.Models.ResponseModels.Roles; // Assuming GetCustomRolesResponse exists

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IRolesService"/> for ClickUp Role operations.
    /// </summary>
    public class RolesService : IRolesService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public RolesService(IApiConnection apiConnection)
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
        public async Task<IEnumerable<CustomRole>> GetCustomRolesAsync(
            string workspaceId,
            bool? includeMembers = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/customroles"; // team_id is workspaceId
            var queryParams = new Dictionary<string, string?>();
            if (includeMembers.HasValue) queryParams["include_members"] = includeMembers.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            // API returns { "roles": [...] } in GetCustomRolesResponse
            var response = await _apiConnection.GetAsync<GetCustomRolesResponse>(endpoint, cancellationToken);
            return response?.Roles ?? Enumerable.Empty<CustomRole>();
        }
    }
}
