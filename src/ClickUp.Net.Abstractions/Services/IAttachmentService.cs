using System.Threading;
using System.Threading.Tasks;
// Assuming a placeholder for request/response models for now
// using ClickUp.Net.Models;

namespace ClickUp.Net.Abstractions.Services
{
    /// <summary>
    /// Interface for services interacting with ClickUp Attachments.
    /// </summary>
    public interface IAttachmentService
    {
        /// <summary>
        /// Uploads a file to a task as an attachment.
        /// </summary>
        /// <param name="taskId">The ID of the task to attach the file to.</param>
        /// <param name="customTaskIds">If referencing task by custom id, set to true.</param>
        /// <param name="teamId">The team ID if using custom task IDs.</param>
        /// <param name="attachment">The file attachment (details TBD, likely a stream or byte array).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the attachment creation response.</returns>
        Task<object> CreateTaskAttachmentAsync(string taskId, bool? customTaskIds = null, double? teamId = null, object attachment = null, CancellationToken cancellationToken = default);
    }
}
