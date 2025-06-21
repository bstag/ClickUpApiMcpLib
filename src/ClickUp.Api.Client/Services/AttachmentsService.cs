using System;
using System.IO;
using System.Net.Http; // Required for MultipartFormDataContent and StreamContent
using System.Text; // For StringBuilder query params
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using System.Collections.Generic; // For Dictionary
using System.Linq;
using ClickUp.Api.Client.Models.Entities.Attachments; // For Linq Any

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IAttachmentsService"/> for interacting with ClickUp Attachments.
    /// </summary>
    public class AttachmentsService : IAttachmentsService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public AttachmentsService(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        }

        private string BuildQueryString(Dictionary<string, string?> queryParams)
        {
            if (queryParams == null || !queryParams.Any(kvp => kvp.Value != null))
            {
                return string.Empty;
            }

            var sb = new StringBuilder("?");
            foreach (var kvp in queryParams)
            {
                if (kvp.Value != null)
                {
                    if (sb.Length > 1) sb.Append('&');
                    sb.Append($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
                }
            }
            return sb.ToString();
        }

        /// <inheritdoc />
        public async Task<Attachment> CreateTaskAttachmentAsync(
            string taskId,
            Stream fileStream,
            string fileName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"task/{taskId}/attachment";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            using var multipartContent = new MultipartFormDataContent();

            // Add file stream content
            var streamContent = new StreamContent(fileStream);
            // The API typically expects the file field to be named "attachment" or "file"
            // and also expects a filename.
            // For ClickUp, the file itself is the "attachment" part, and filename is separate.
            // The ClickUp API for task attachments expects 'filename' as a form field and the file data as 'attachment'.
            // However, when using StreamContent with MultipartFormDataContent,
            // the Content-Disposition header usually handles the filename.
            // Let's ensure we set the filename in the Content-Disposition header of the StreamContent.
            // streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            // {
            //     Name = "attachment", // This is the field name expected by ClickUp
            //     FileName = fileName
            // };
            // multipartContent.Add(streamContent);

            // The ClickUp API documentation specifies:
            // - A form field named `filename` (e.g. `screenshot.jpg`)
            // - The file contents under the field name `attachment`
            // Let's try setting the filename as a separate StringContent part first, as often required.
            // And then add the file stream as "attachment".

            // It's more common that the filename is part of the StreamContent's headers.
            // If the API expects 'filename' as a separate field, then it would be:
            // multipartContent.Add(new StringContent(fileName), "filename");
            // And the file part:
            // multipartContent.Add(new StreamContent(fileStream), "attachment");

            // Let's use the more standard way of providing filename via ContentDisposition
            // and ensure the field name for the file is "attachment".
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = "attachment", // This is the field name expected by ClickUp for the file data
                FileName = fileName  // This sets the filename in the Content-Disposition
            };
            multipartContent.Add(fileContent);

            // ClickUp documentation also shows an optional 'filename' field for when 'attachment' field is a URL.
            // When 'attachment' is the file content, the filename in Content-Disposition should suffice.
            // If the API strictly requires a separate 'filename' field even with file upload, it would be:
            // multipartContent.Add(new StringContent(fileName), "\"filename\""); // Quotes might be needed for form field names by some servers

            // Ensure the IApiConnection interface and its implementation support PostMultipartAsync
            // For now, we assume it correctly handles the multipart request and deserializes the response.
            var createdAttachment = await _apiConnection.PostMultipartAsync<Attachment>(endpoint, multipartContent, cancellationToken);

            if (createdAttachment == null)
            {
                // This case should ideally not happen if the API successfully creates an attachment and returns it.
                // If it can happen (e.g. API returns 204 No Content on success for some reason, though unlikely for a create operation),
                // the interface IAttachmentsService might need to be Task<Attachment?> or handle it differently.
                // For now, adhering to the non-null interface contract and ClickUp's typical behavior of returning the created entity.
                throw new InvalidOperationException($"Failed to create attachment for task {taskId}, or the API returned an unexpected null response.");
            }

            return createdAttachment;
        }
    }
}
