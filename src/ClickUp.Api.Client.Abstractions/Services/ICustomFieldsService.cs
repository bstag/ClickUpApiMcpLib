using ClickUp.Api.Client.Abstractions.Services.CustomFields;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Comprehensive service interface for ClickUp Custom Fields operations.
    /// This interface follows the Interface Segregation Principle by composing smaller, focused interfaces.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive methods for managing Custom Fields, including retrieving
    /// Custom Field definitions from various hierarchy levels and managing Custom Field values on tasks.
    /// The interface is composed of:
    /// - ICustomFieldReader: For retrieving Custom Field definitions from various levels
    /// - ITaskCustomFieldManager: For managing Custom Field values on tasks
    /// 
    /// Covered API Endpoints:
    /// - `GET /list/{list_id}/field`: Retrieves accessible Custom Fields for a List.
    /// - `GET /folder/{folder_id}/field`: Retrieves Custom Fields defined at the Folder level.
    /// - `GET /space/{space_id}/field`: Retrieves Custom Fields defined at the Space level.
    /// - `GET /team/{team_id}/field`: Retrieves Custom Fields defined at the Workspace (Team) level.
    /// - `POST /task/{task_id}/field/{field_id}`: Sets or updates the value of a Custom Field on a task.
    /// - `DELETE /task/{task_id}/field/{field_id}`: Removes the value of a Custom Field from a task.
    /// </remarks>
    public interface ICustomFieldsService : ICustomFieldReader, ITaskCustomFieldManager
    {
        // All methods are now inherited from the composed interfaces:
        // - ICustomFieldReader: For retrieving Custom Field definitions from various levels
        // - ITaskCustomFieldManager: For managing Custom Field values on tasks
    }
}
