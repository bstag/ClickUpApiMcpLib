using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;

namespace ClickUp.Api.Client.Abstractions.Services.Folders
{
    /// <summary>
    /// Interface for ClickUp Folder template management operations.
    /// This interface follows the Interface Segregation Principle by focusing solely on template-related operations.
    /// </summary>
    public interface IFolderTemplateManager
    {
        /// <summary>
        /// Creates a new Folder from a specified Folder template within a Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space where the new Folder will be created.</param>
        /// <param name="templateId">The unique identifier of the Folder template to use.</param>
        /// <param name="createFolderFromTemplateRequest">An object containing details for creating the Folder from the template, such as the new Folder's name.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="Folder"/> object. Note: The API might return only an ID if specific 'return_immediately' options are used in the request, though this SDK aims to return the full object where possible.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/>, <paramref name="templateId"/>, or <paramref name="createFolderFromTemplateRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space or Folder template with the specified IDs does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action in the Space or use the template.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Folder> CreateFolderFromTemplateAsync(
            string spaceId,
            string templateId,
            CreateFolderFromTemplateRequest createFolderFromTemplateRequest,
            CancellationToken cancellationToken = default);
    }
}