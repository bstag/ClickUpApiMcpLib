using ClickUp.Api.Client.Abstractions.Services.Spaces;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Comprehensive service interface for ClickUp Space operations.
    /// This interface follows the Interface Segregation Principle by composing smaller, focused interfaces.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive methods for managing Spaces within a Workspace (Team),
    /// including creating, retrieving, updating, and deleting Spaces.
    /// The interface is composed of:
    /// - ISpaceReader: For space read operations (Get operations)
    /// - ISpaceWriter: For space write operations (Create, Update, Delete)
    /// Covered API Endpoints:
    /// - `GET /team/{team_id}/space`: Retrieves Spaces in a Workspace.
    /// - `POST /team/{team_id}/space`: Creates a new Space in a Workspace.
    /// - `GET /space/{space_id}`: Retrieves details of a specific Space.
    /// - `PUT /space/{space_id}`: Updates an existing Space.
    /// - `DELETE /space/{space_id}`: Deletes a Space.
    /// </remarks>
    public interface ISpacesService : ISpaceReader, ISpaceWriter
    {
        // All methods are now inherited from the composed interfaces:
        // - ISpaceReader: Space read operations (Get operations)
        // - ISpaceWriter: Space write operations (Create, Update, Delete)
    }
}
