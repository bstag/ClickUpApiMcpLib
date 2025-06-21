using System;
using System.IO; // Required for Stream
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities; // May not be needed if Attachment is not directly used
using ClickUp.Api.Client.Models.Responses.Attachments; // Changed to use the new response DTO

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Attachments operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - POST /v2/task/{task_id}/attachment
    /// (Full API spec might include GET, DELETE operations for attachments as well)
    /// </remarks>
    public interface IAttachmentsService
    {
        /// <summary>
        /// Uploads a file to a task as an attachment.
        /// </summary>
        /// <param name="taskId">The ID of the task to attach the file to.</param>
        /// <param name="fileStream">The stream containing the file content.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the created attachment response <see cref="CreateTaskAttachmentResponse"/>.</returns>
        Task<CreateTaskAttachmentResponse> CreateTaskAttachmentAsync(
            string taskId,
            Stream fileStream,
            string fileName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        // Placeholder for GetTaskAttachmentsAsync if the API supports it
        // /// <summary>
        // /// Gets all attachments for a specific task.
        // /// </summary>
        // /// <param name="taskId">The ID of the task.</param>
        // /// <param name="cancellationToken">Cancellation token.</param>
        // /// <returns>A list of attachments for the task.</returns>
        // CuTask<IEnumerable<Attachment>> GetTaskAttachmentsAsync(string taskId, CancellationToken cancellationToken = default);

        // Placeholder for DeleteTaskAttachmentAsync if the API supports it
        // /// <summary>
        // /// Deletes an attachment from a task.
        // /// </summary>
        // /// <param name="taskId">The ID of the task.</param>
        // /// <param name="attachmentId">The ID of the attachment to delete.</param>
        // /// <param name="cancellationToken">Cancellation token.</param>
        // /// <returns>An awaitable task representing the asynchronous operation.</returns>
        // System.Threading.Tasks.CuTask DeleteTaskAttachmentAsync(string taskId, string attachmentId, CancellationToken cancellationToken = default);
    }
}
