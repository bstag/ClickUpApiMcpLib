using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Spaces; // Assuming Space DTO is here
using ClickUp.Api.Client.Models.RequestModels.Spaces; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Spaces operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/team/{team_id}/space
    /// - POST /v2/team/{team_id}/space
    /// - GET /v2/space/{space_id}
    /// - PUT /v2/space/{space_id}
    /// - DELETE /v2/space/{space_id}
    /// </remarks>
    public interface ISpacesService
    {
        /// <summary>
        /// Retrieves Spaces available in a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="archived">Optional. Whether to include archived Spaces.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Space"/> objects in the Workspace.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the workspace with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access spaces in this workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<IEnumerable<Space>> GetSpacesAsync(
            string workspaceId,
            bool? archived = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Space in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="createSpaceRequest">Details of the Space to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="Space"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createSpaceRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the workspace with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create spaces in this workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Space> CreateSpaceAsync(
            string workspaceId,
            CreateSpaceRequest createSpaceRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves details of a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="Space"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Space> GetSpaceAsync(
            string spaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="updateSpaceRequest">Details for updating the Space.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="Space"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> or <paramref name="updateSpaceRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Space> UpdateSpaceAsync(
            string spaceId,
            UpdateSpaceRequest updateSpaceRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task DeleteSpaceAsync(
            string spaceId,
            CancellationToken cancellationToken = default);
    }
}
