using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tags;
using ClickUp.Api.Client.Models.RequestModels.Spaces; // Note: ModifyTagRequest might be better in a Tags specific namespace.

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Tags operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for managing Tags within a Space and for applying or removing Tags from Tasks.
    /// Covered API Endpoints:
    /// - Space Tags: `GET /space/{space_id}/tag`, `POST /space/{space_id}/tag`, `PUT /space/{space_id}/tag/{tag_name}`, `DELETE /space/{space_id}/tag/{tag_name}`
    /// - Task Tags: `POST /task/{task_id}/tag/{tag_name}`, `DELETE /task/{task_id}/tag/{tag_name}`
    /// </remarks>
    public interface ITagsService
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

        /// <summary>
        /// Adds an existing Tag to a specific task. The Tag must already exist within the Space of the task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task to which the Tag will be added.</param>
        /// <param name="tagName">The name of the Tag to add. The Tag must be predefined in the task's Space.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="tagName"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or the Tag (within the task's Space) with the specified ID/name does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to add Tags to this task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task AddTagToTaskAsync(
            string taskId,
            string tagName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an existing Tag from a specific task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task from which the Tag will be removed.</param>
        /// <param name="tagName">The name of the Tag to remove from the task.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="tagName"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task does not exist, or if the Tag is not currently applied to the task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove Tags from this task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task RemoveTagFromTaskAsync(
            string taskId,
            string tagName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
    }
}
