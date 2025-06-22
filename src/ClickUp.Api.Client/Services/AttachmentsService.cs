using System.Text; // For StringBuilder query params
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Attachments;

// using ClickUp.Api.Client.Models.Entities; // May not be needed if Attachment model is not directly used here
// using ClickUp.Api.Client.Models.Entities.Attachments; // Replaced by specific response

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IAttachmentsService"/> for interacting with ClickUp Attachments.
    /// </summary>
    public class AttachmentsService : IAttachmentsService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<AttachmentsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public AttachmentsService(IApiConnection apiConnection, ILogger<AttachmentsService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<AttachmentsService>.Instance;
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
        public async Task<CreateTaskAttachmentResponse> CreateTaskAttachmentAsync(
            string taskId,
            Stream fileStream,
            string fileName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating task attachment for task ID: {TaskId}, FileName: {FileName}", taskId, fileName);
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

            // Add the filename as a separate form-data field as per ClickUp API docs
            multipartContent.Add(new StringContent(fileName), "filename");

            var fileContent = new StreamContent(fileStream);
            // It's good practice to set ContentType for file uploads, though HttpClient might infer it.
            // e.g., fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            // However, for "attachment" part, the API expects the raw file data.
            // The 'Name' in ContentDisposition is what links it to the form field name.
            fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = "attachment", // This is the field name expected by ClickUp for the file data
                FileName = fileName  // This sets the filename in the Content-Disposition, which is standard
            };
            multipartContent.Add(fileContent, "attachment"); // Explicitly provide field name here too

            // Ensure the IApiConnection interface and its implementation support PostMultipartAsync
            // For now, we assume it correctly handles the multipart request and deserializes the response.
            var response = await _apiConnection.PostMultipartAsync<CreateTaskAttachmentResponse>(endpoint, multipartContent, cancellationToken);

            if (response == null)
            {
                // This case should ideally not happen if the API successfully creates an attachment and returns it.
                // If it can happen (e.g. API returns 204 No Content on success for some reason, though unlikely for a create operation),
                // the interface IAttachmentsService might need to be Task<CreateTaskAttachmentResponse?> or handle it differently.
                // For now, adhering to the non-null interface contract and ClickUp's typical behavior of returning the created entity.
                // Consider specific exception handling (Step 6 of overall plan) here in future.
                throw new InvalidOperationException($"Failed to create attachment for task {taskId}, or the API returned an unexpected null or invalid response.");
            }

            return response;
        }
    }
}
