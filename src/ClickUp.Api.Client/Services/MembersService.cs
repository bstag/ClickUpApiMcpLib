using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.ResponseModels.Members; // Assuming GetMembersResponse exists

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IMembersService"/> for ClickUp Member operations.
    /// </summary>
    public class MembersService : IMembersService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MembersService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public MembersService(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ClickUp.Api.Client.Models.ResponseModels.Members.Member>> GetTaskMembersAsync(
            string taskId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"task/{taskId}/member";
            var response = await _apiConnection.GetAsync<GetMembersResponse>(endpoint, cancellationToken); // API returns {"members": [...]}
            return response?.Members ?? Enumerable.Empty<ClickUp.Api.Client.Models.ResponseModels.Members.Member>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ClickUp.Api.Client.Models.ResponseModels.Members.Member>> GetListMembersAsync(
            string listId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"list/{listId}/member";
            var response = await _apiConnection.GetAsync<GetMembersResponse>(endpoint, cancellationToken); // API returns {"members": [...]}
            return response?.Members ?? Enumerable.Empty<ClickUp.Api.Client.Models.ResponseModels.Members.Member>();
        }
    }
}
