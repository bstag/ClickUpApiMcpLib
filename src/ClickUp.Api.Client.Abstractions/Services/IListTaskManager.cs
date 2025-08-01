using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines operations for managing tasks within Lists in ClickUp.
    /// </summary>
    public interface IListTaskManager
    {
        /// <summary>
        /// Adds an existing task to an additional List. This operation requires the "Tasks in Multiple Lists" ClickApp to be enabled for the Workspace.
        /// </summary>
        /// <param name="listId">The unique identifier of the List to which the task will be added.</param>
        /// <param name="taskId">The unique identifier of the task to add to the List.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List or task with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the "Tasks in Multiple Lists" ClickApp is not enabled, or if other validation rules fail.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task AddTaskToListAsync(
            string listId,
            string taskId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a task from one of its Lists. This operation requires the "Tasks in Multiple Lists" ClickApp to be enabled.
        /// A task must always belong to at least one List; it cannot be removed from its primary List if it's not in any other Lists.
        /// </summary>
        /// <param name="listId">The unique identifier of the List from which the task will be removed.</param>
        /// <param name="taskId">The unique identifier of the task to remove from the List.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List or task with the specified IDs does not exist, or if the task is not currently associated with the List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the "Tasks in Multiple Lists" ClickApp is not enabled, or if attempting to remove the task from its only List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task RemoveTaskFromListAsync(
            string listId,
            string taskId,
            CancellationToken cancellationToken = default);
    }
}