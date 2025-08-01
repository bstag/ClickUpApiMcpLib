using ClickUp.Api.Client.Abstractions.Services.Folders;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Comprehensive service interface for ClickUp Folder operations.
    /// This interface follows the Interface Segregation Principle by composing smaller, focused interfaces.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive methods for managing Folders within Spaces,
    /// including creating, retrieving, updating, deleting Folders, and creating Folders from templates.
    /// The interface is composed of:
    /// - IFolderReader: For folder read operations (Get operations)
    /// - IFolderWriter: For folder write operations (Create, Update, Delete)
    /// - IFolderTemplateManager: For template-related operations
    /// Covered API Endpoints:
    /// - `GET /space/{space_id}/folder`: Retrieves Folders in a Space.
    /// - `POST /space/{space_id}/folder`: Creates a new Folder in a Space.
    /// - `GET /folder/{folder_id}`: Retrieves details of a specific Folder.
    /// - `PUT /folder/{folder_id}`: Updates an existing Folder.
    /// - `DELETE /folder/{folder_id}`: Deletes a Folder.
    /// - `POST /space/{space_id}/folder_template/{template_id}`: Creates a Folder from a template.
    /// </remarks>
    public interface IFoldersService : IFolderReader, IFolderWriter, IFolderTemplateManager
    {
        // All methods are now inherited from the composed interfaces:
        // - IFolderReader: Folder read operations (Get operations)
        // - IFolderWriter: Folder write operations (Create, Update, Delete)
        // - IFolderTemplateManager: Template-related operations
    }
}
