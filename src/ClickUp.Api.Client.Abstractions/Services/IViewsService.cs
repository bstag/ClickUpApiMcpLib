using ClickUp.Api.Client.Abstractions.Services.Views;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Comprehensive service interface for ClickUp View operations.
    /// This interface follows the Interface Segregation Principle by composing smaller, focused interfaces.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive methods for managing Views across different hierarchy levels,
    /// individual view management, and task operations within views.
    /// The interface is composed of:
    /// - IViewHierarchyService: For view operations across different hierarchy levels (workspace, space, folder, list)
    /// - IViewManagementService: For individual view CRUD operations
    /// - IViewTaskService: For task operations within views
    /// Views can be associated with Workspaces (Teams), Spaces, Folders, or Lists.
    /// Based on ClickUp API endpoints like:
    /// <list type="bullet">
    /// <item><description>GET /v2/team/{team_id}/view</description></item>
    /// <item><description>POST /v2/team/{team_id}/view</description></item>
    /// <item><description>GET /v2/space/{space_id}/view</description></item>
    /// <item><description>POST /v2/space/{space_id}/view</description></item>
    /// <item><description>GET /v2/folder/{folder_id}/view</description></item>
    /// <item><description>POST /v2/folder/{folder_id}/view</description></item>
    /// <item><description>GET /v2/list/{list_id}/view</description></item>
    /// <item><description>POST /v2/list/{list_id}/view</description></item>
    /// <item><description>GET /v2/view/{view_id}</description></item>
    /// <item><description>PUT /v2/view/{view_id}</description></item>
    /// <item><description>DELETE /v2/view/{view_id}</description></item>
    /// <item><description>GET /v2/view/{view_id}/task</description></item>
    /// </list>
    /// </remarks>
    public interface IViewsService : IViewHierarchyService, IViewManagementService, IViewTaskService
    {
        // All methods are now inherited from the composed interfaces:
        // - IViewHierarchyService: View operations across different hierarchy levels (workspace, space, folder, list)
        // - IViewManagementService: Individual view CRUD operations
        // - IViewTaskService: Task operations within views
    }
}
