using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Responses.Attachments;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for operations related to task attachments.
    /// </summary>
    /// <remarks>
    /// This service allows for managing attachments linked to tasks within ClickUp.
    /// Endpoints covered:
    /// - `POST /task/{task_id}/attachment`: Uploads an attachment to a task.
    /// Additional operations like listing or deleting attachments might be added in future versions if supported by the API.
    /// </remarks>
    public interface IAttachmentsService
    {
        /// <summary>
        /// Uploads a file to a specified task as an attachment.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task to which the file will be attached.</param>
        /// <param name="fileStream">The stream containing the content of the file to be uploaded.</param>
        /// <param name="fileName">The desired name for the file once uploaded.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateTaskAttachmentResponse"/> object with details of the uploaded attachment.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="taskId"/>, <paramref name="fileStream"/>, or <paramref name="fileName"/> is null or empty/whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown if the API call fails. Common derived types include <see cref="Models.Exceptions.ClickUpApiNotFoundException"/> (if the task is not found), <see cref="Models.Exceptions.ClickUpApiRateLimitException"/> (if rate limits are exceeded), or <see cref="Models.Exceptions.ClickUpApiRequestException"/> (for other client-side request issues).</exception>
        Task<CreateTaskAttachmentResponse> CreateTaskAttachmentAsync(
            string taskId,
            Stream fileStream,
            string fileName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        // Future considerations (if API supports):
        // GetTaskAttachmentsAsync(string taskId, CancellationToken cancellationToken = default);
        // DeleteTaskAttachmentAsync(string taskId, string attachmentId, CancellationToken cancellationToken = default);
    }
}
