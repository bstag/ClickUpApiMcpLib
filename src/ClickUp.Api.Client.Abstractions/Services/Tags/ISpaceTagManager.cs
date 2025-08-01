using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tags;
using ClickUp.Api.Client.Models.RequestModels.Spaces; // Note: ModifyTagRequest might be better in a Tags specific namespace.

namespace ClickUp.Api.Client.Abstractions.Services.Tags
{
    /// <summary>
    /// Service interface for managing ClickUp Tags within Spaces.
    /// This interface handles CRUD operations for tags at the Space level.
    /// </summary>
    /// <remarks>
    /// This service provides methods for managing Tags within a Space.
    /// Covered API Endpoints:
    /// - Space Tags: `GET /space/{space_id}/tag`, `POST /space/{space_id}/tag`, `PUT /space/{space_id}/tag/{tag_name}`, `DELETE /space/{space_id}/tag/{tag_name}`
    /// </remarks>
    public interface ISpaceTagManager
    {
        /// <summary>
        /// Retrieves all task Tags available within a specific Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space from which to retrieve Tags.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Tag"/> objects defined in the Space.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Tags in this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<IEnumerable<Tag>> GetSpaceTagsAsync(
            string spaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new task Tag within a specific Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space where the new Tag will be created.</param>
        /// <param name="modifyTagRequest">An object containing the details for the new Tag, such as its name and color attributes.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The API typically returns the created Tag, but this method might be void or return a specific response object if needed.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> or <paramref name="modifyTagRequest"/> is null, or if the tag name within the request is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if a Tag with the same name already exists in the Space or if the request is otherwise invalid.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create Tags in this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task CreateSpaceTagAsync(
            string spaceId,
            ModifyTagRequest modifyTagRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing task Tag within a Space. The Tag to be updated is identified by its original name.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space containing the Tag.</param>
        /// <param name="tagName">The current name of the Tag to be edited. This name is case-sensitive.</param>
        /// <param name="modifyTagRequest">An object containing the new details for the Tag, such as a new name or updated color attributes.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Tag"/> object with its new details.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/>, <paramref name="tagName"/>, or <paramref name="modifyTagRequest"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space or the Tag with the specified name does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to edit Tags in this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Tag> EditSpaceTagAsync(
            string spaceId,
            string tagName,
            ModifyTagRequest modifyTagRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a task Tag from a specific Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space from which the Tag will be deleted.</param>
        /// <param name="tagName">The name of the Tag to delete. This name is case-sensitive.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> or <paramref name="tagName"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space or the Tag with the specified name does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete Tags in this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteSpaceTagAsync(
            string spaceId,
            string tagName,
            CancellationToken cancellationToken = default);
    }
}