using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Abstractions.Services.CustomFields
{
    /// <summary>
    /// Interface for managing ClickUp Custom Field values on Tasks.
    /// This interface focuses on setting and removing Custom Field values from tasks.
    /// </summary>
    public interface ITaskCustomFieldManager
    {
        /// <summary>
        /// Sets or updates the value of a Custom Field on a specific task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task.</param>
        /// <param name="fieldId">The unique identifier (UUID) of the Custom Field whose value is to be set.</param>
        /// <param name="setFieldValueRequest">A <see cref="SetCustomFieldValueRequest"/> object containing the value to set for the Custom Field. The structure of the 'value' property within this request depends on the type of the Custom Field (e.g., string, number, array of user IDs).</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/>, <paramref name="fieldId"/>, or <paramref name="setFieldValueRequest"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or Custom Field with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the provided value in <paramref name="setFieldValueRequest"/> is invalid for the type of the Custom Field.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify this Custom Field on the task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task SetCustomFieldValueAsync(
            string taskId,
            string fieldId,
            SetCustomFieldValueRequest setFieldValueRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the value from a Custom Field on a specific task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task.</param>
        /// <param name="fieldId">The unique identifier (UUID) of the Custom Field from which the value will be removed.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="fieldId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or Custom Field with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify this Custom Field on the task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task RemoveCustomFieldValueAsync(
            string taskId,
            string fieldId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
    }
}