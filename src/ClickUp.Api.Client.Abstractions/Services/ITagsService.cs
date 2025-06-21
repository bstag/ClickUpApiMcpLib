using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Tags; // Assuming Tag DTO is here
using ClickUp.Api.Client.Models.RequestModels.Spaces; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Tags operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/space/{space_id}/tag
    /// - POST /v2/space/{space_id}/tag
    /// - PUT /v2/space/{space_id}/tag/{tag_name}
    /// - DELETE /v2/space/{space_id}/tag/{tag_name}
    /// - POST /v2/task/{task_id}/tag/{tag_name}
    /// - DELETE /v2/task/{task_id}/tag/{tag_name}
    /// </remarks>
    public interface ITagsService
    {
        /// <summary>
        /// Retrieves task Tags available in a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Tag"/> objects in the Space.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access tags in this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<IEnumerable<Tag>> GetSpaceTagsAsync(
            string spaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new task Tag in a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="modifyTagRequest">Details of the Tag to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> or <paramref name="modifyTagRequest"/> is null, or if tag name in request is null/empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiValidationException">Thrown if a tag with the same name already exists in the space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create tags in this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task CreateSpaceTagAsync(
            string spaceId,
            ModifyTagRequest modifyTagRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a task Tag in a Space. The tag is identified by its original name.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="tagName">The original name of the Tag to edit.</param>
        /// <param name="modifyTagRequest">Details for updating the Tag (e.g., new name, colors).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="Tag"/> details.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/>, <paramref name="tagName"/>, or <paramref name="modifyTagRequest"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space or the tag with the specified name is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to edit tags in this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Tag> EditSpaceTagAsync(
            string spaceId,
            string tagName,
            ModifyTagRequest modifyTagRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a task Tag from a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="tagName">The name of the Tag to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> or <paramref name="tagName"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space or the tag with the specified name is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete tags in this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task DeleteSpaceTagAsync(
            string spaceId,
            string tagName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a Tag to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="tagName">The name of the Tag to add. The tag must already exist in the Space.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="tagName"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or the tag (in the task's space) with the specified ID/name is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to add tags to this task.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task AddTagToTaskAsync(
            string taskId,
            string tagName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a Tag from a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="tagName">The name of the Tag to remove.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="tagName"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or the tag (on the task) with the specified ID/name is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove tags from this task.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task RemoveTagFromTaskAsync(
            string taskId,
            string tagName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
    }
}
