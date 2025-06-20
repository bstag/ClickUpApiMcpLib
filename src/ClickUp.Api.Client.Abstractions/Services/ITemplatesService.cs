using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Templates; // For GetTaskTemplatesResponse

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Templates operations in the ClickUp API, focusing on retrieving CuTask Templates.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/team/{team_id}/taskTemplate
    /// </remarks>
    public interface ITemplatesService
    {
        /// <summary>
        /// Retrieves the task templates available in a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="page">The page number to retrieve (0-indexed). This is a required parameter by the ClickUp API for this endpoint.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="GetTaskTemplatesResponse"/> object which contains a list of task templates available in the Workspace for the specified page.</returns>
        /// <remarks>The ClickUp API returns templates in a {"templates": []} structure within the response object.</remarks>
        Task<GetTaskTemplatesResponse> GetTaskTemplatesAsync(
            string workspaceId,
            int page,
            CancellationToken cancellationToken = default);
    }
}
