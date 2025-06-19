using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{

    // Represents the Spaces operations in the ClickUp API
    // Based on endpoints like:
    // - GET /v2/team/{team_id}/space
    // - POST /v2/team/{team_id}/space
    // - GET /v2/space/{space_id}
    // - PUT /v2/space/{space_id}
    // - DELETE /v2/space/{space_id}

    public interface ISpacesService
    {
        /// <summary>
        /// Retrieves Spaces available in a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="archived">Optional. Whether to include archived Spaces.</param>
        /// <returns>A list of Spaces in the Workspace.</returns>
        Task<IEnumerable<object>> GetSpacesAsync(double workspaceId, bool? archived = null);
        // Note: Return type should be IEnumerable<SpaceDto>.

        /// <summary>
        /// Creates a new Space in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="createSpaceRequest">Details of the Space to create.</param>
        /// <returns>The created Space.</returns>
        Task<object> CreateSpaceAsync(double workspaceId, object createSpaceRequest);
        // Note: createSpaceRequest should be CreateSpaceRequest, return type should be SpaceDto.

        /// <summary>
        /// Retrieves details of a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <returns>Details of the Space.</returns>
        Task<object> GetSpaceAsync(double spaceId);
        // Note: Return type should be SpaceDto.

        /// <summary>
        /// Updates a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="updateSpaceRequest">Details for updating the Space.</param>
        /// <returns>The updated Space.</returns>
        Task<object> UpdateSpaceAsync(double spaceId, object updateSpaceRequest);
        // Note: updateSpaceRequest should be UpdateSpaceRequest, return type should be SpaceDto.

        /// <summary>
        /// Deletes a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteSpaceAsync(double spaceId);
        // Note: API returns 200 with an empty object.
    }
}
