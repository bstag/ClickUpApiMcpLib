using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Attachments operations in the ClickUp API
    // Based on endpoints like:
    // - POST /v2/task/{task_id}/attachment
    // (Assuming there might be GET, DELETE operations for attachments as well, though not explicitly listed in the immediate previous context,
    // a full-fledged service would typically include them if they exist in the full API spec)
    public interface IAttachmentsService
    {
        /// <summary>
        /// Uploads a file to a task as an attachment.
        /// </summary>
        /// <param name="taskId">The ID of the task to attach the file to.</param>
        /// <param name="attachmentContent">The content of the file to upload. This would typically be a stream or byte array and filename.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the created attachment.</returns>
        Task<object> CreateTaskAttachmentAsync(string taskId, object attachmentContent, bool? customTaskIds = null, double? teamId = null);
        // Note: 'object attachmentContent' should be a more specific type representing the file to be uploaded (e.g., Stream, byte[], or a custom DTO).
        // Note: The return type 'object' should be replaced with a specific DTO (e.g., TaskAttachmentDto) representing the attachment details from the API response.

        // Placeholder for GetTaskAttachmentsAsync if the API supports it
        // /// <summary>
        // /// Gets all attachments for a specific task.
        // /// </summary>
        // /// <param name="taskId">The ID of the task.</param>
        // /// <returns>A list of attachments for the task.</returns>
        // Task<IEnumerable<object>> GetTaskAttachmentsAsync(string taskId);

        // Placeholder for DeleteTaskAttachmentAsync if the API supports it
        // /// <summary>
        // /// Deletes an attachment from a task.
        // /// </summary>
        // /// <param name="taskId">The ID of the task.</param>
        // /// <param name="attachmentId">The ID of the attachment to delete.</param>
        // /// <returns>An awaitable task representing the asynchronous operation.</returns>
        // Task DeleteTaskAttachmentAsync(string taskId, string attachmentId);
    }
}
