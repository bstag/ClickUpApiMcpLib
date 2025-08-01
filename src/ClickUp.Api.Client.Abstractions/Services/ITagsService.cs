using ClickUp.Api.Client.Abstractions.Services.Tags;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Comprehensive service interface for ClickUp Tags operations.
    /// This interface follows the Interface Segregation Principle by composing smaller, focused interfaces.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive methods for managing Tags within Spaces and for applying or removing Tags from Tasks.
    /// The interface is composed of:
    /// - ISpaceTagManager: For managing tags within spaces (CRUD operations)
    /// - ITaskTagManager: For managing tag assignments to tasks
    /// Covered API Endpoints:
    /// - Space Tags: `GET /space/{space_id}/tag`, `POST /space/{space_id}/tag`, `PUT /space/{space_id}/tag/{tag_name}`, `DELETE /space/{space_id}/tag/{tag_name}`
    /// - Task Tags: `POST /task/{task_id}/tag/{tag_name}`, `DELETE /task/{task_id}/tag/{tag_name}`
    /// </remarks>
    public interface ITagsService : ISpaceTagManager, ITaskTagManager
    {
        // All methods are now inherited from the composed interfaces:
        // - ISpaceTagManager: For managing tags within spaces (CRUD operations)
        // - ITaskTagManager: For managing tag assignments to tasks
    }
}
