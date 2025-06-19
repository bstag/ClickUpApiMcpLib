using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Templates operations in the ClickUp API, focusing on retrieving Task Templates.
    // Based on endpoints like:
    // - GET /v2/team/{team_id}/taskTemplate

    public interface ITemplatesService
    {
        /// <summary>
        /// Retrieves the task templates available in a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="page">The page number to retrieve. API is 0-indexed. This is a required parameter in the ClickUp API for this endpoint.</param>
        /// <returns>A list of task templates available in the Workspace.</returns>
        Task<IEnumerable<object>> GetTaskTemplatesAsync(double workspaceId, int page);
        // Note: Return type should be IEnumerable<TaskTemplateDto>. The response example { "templates": [] } suggests it's a list.
        // The 'page' parameter is marked as required in the OpenAPI spec for this endpoint.
    }
}
