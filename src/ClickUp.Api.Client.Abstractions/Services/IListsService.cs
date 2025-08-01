using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.RequestModels.Lists;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp List operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for managing Lists within Folders or directly within Spaces (Folderless Lists).
    /// It supports creating, retrieving, updating, and deleting Lists, as well as managing tasks within Lists
    /// (e.g., adding/removing tasks from additional Lists if the "Tasks in Multiple Lists" ClickApp is enabled)
    /// and creating Lists from templates.
    /// Covered API Endpoints:
    /// - Lists in Folders: `GET /folder/{folder_id}/list`, `POST /folder/{folder_id}/list`
    /// - Folderless Lists: `GET /space/{space_id}/list`, `POST /space/{space_id}/list`
    /// - General List Operations: `GET /list/{list_id}`, `PUT /list/{list_id}`, `DELETE /list/{list_id}`
    /// - Tasks in Multiple Lists: `POST /list/{list_id}/task/{task_id}`, `DELETE /list/{list_id}/task/{task_id}`
    /// - Lists from Templates: `POST /folder/{folder_id}/list_template/{template_id}`, `POST /space/{space_id}/list_template/{template_id}`
    /// </remarks>
    public interface IListsService : IListReader, IListWriter, IListTaskManager
    {
        // All methods are now inherited from the composed interfaces
    }

}
