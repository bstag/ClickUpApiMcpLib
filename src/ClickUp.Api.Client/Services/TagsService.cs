using System;
using System.Collections.Generic;
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tags;
using ClickUp.Api.Client.Models.RequestModels.Tags; // For ModifyTagRequest
using ClickUp.Api.Client.Models.ResponseModels; // Assuming GetTagsResponse exists
using System.Linq;
using ClickUp.Api.Client.Models.ResponseModels.Spaces; // For Enumerable.Empty
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ITagsService"/> for ClickUp Tag operations.
    /// </summary>
    public class TagsService : ITagsService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<TagsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public TagsService(IApiConnection apiConnection, ILogger<TagsService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<TagsService>.Instance;
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
        public async Task<IEnumerable<Tag>> GetSpaceTagsAsync(
            string spaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting space tags for space ID: {SpaceId}", spaceId);
            var endpoint = $"space/{spaceId}/tag";
            var response = await _apiConnection.GetAsync<GetSpaceTagsResponse>(endpoint, cancellationToken); // API returns {"tags": [...]}
            return response?.Tags ?? Enumerable.Empty<Tag>();
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task CreateSpaceTagAsync(
            string spaceId,
            SaveTagRequest modifyTagRequest, // Changed from CreateTagRequest
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating space tag in space ID: {SpaceId}, Tag Name: {TagName}", spaceId, modifyTagRequest.Tag.Name);
            var endpoint = $"space/{spaceId}/tag";
            // Interface is void. The API might return the created tag, but we discard it.
            // If API requires specific response handling (even if just for success/fail), this might need adjustment.
            // Using the PostAsync overload that doesn't expect a specific typed response body.
            await _apiConnection.PostAsync(endpoint, modifyTagRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Tag> EditSpaceTagAsync(
            string spaceId,
            string tagName, // This tagName is for identifying the tag to edit, part of the path
            SaveTagRequest modifyTagRequest, // Changed from UpdateTagRequest, this contains the new tag details
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Editing space tag in space ID: {SpaceId}, Original Tag Name: {OriginalTagName}, New Tag Name: {NewTagName}", spaceId, tagName, modifyTagRequest.Tag.Name);
            // Ensure tagName is URL encoded if it can contain special characters
            var encodedTagName = Uri.EscapeDataString(tagName);
            var endpoint = $"space/{spaceId}/tag/{encodedTagName}";
            // Assuming API returns the updated Tag directly or a wrapper containing the tag.
            // The interface expects Tag, so we aim for _apiConnection.PutAsync<ModifyTagRequest, Tag>
            // The API example for "Edit Space Tag" shows response: {"tag": {"name": "Updated Tag", ...}}
            // This implies a wrapper. Let's assume GetTagResponse or similar that has a Tag property.
            // If EditTagResponse was intended for this, it should wrap a Tag.
            // For now, to match interface, let's assume direct Tag response or a simple wrapper.
            // Let's assume the API returns the Tag object directly for simplicity matching the interface.
            var tag = await _apiConnection.PutAsync<SaveTagRequest, Tag>(endpoint, modifyTagRequest, cancellationToken);
            if (tag == null)
            {
                throw new InvalidOperationException($"API connection returned null response when editing tag '{tagName}' in space {spaceId}.");
            }
            return tag;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteSpaceTagAsync(
            string spaceId,
            string tagName, // This tagName is for identifying the tag to delete, part of the path
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting space tag in space ID: {SpaceId}, Tag Name: {TagName}", spaceId, tagName);
            var encodedTagName = Uri.EscapeDataString(tagName);
            var endpoint = $"space/{spaceId}/tag/{encodedTagName}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task AddTagToTaskAsync(
            string taskId,
            string tagName, // This tagName is for identifying the tag to add, part of the path
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding tag {TagName} to task ID: {TaskId}", tagName, taskId);
            var encodedTagName = Uri.EscapeDataString(tagName);
            var endpoint = $"task/{taskId}/tag/{encodedTagName}";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            // This POST request does not have a body.
            await _apiConnection.PostAsync<object>(endpoint, new object(), cancellationToken);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task RemoveTagFromTaskAsync(
            string taskId,
            string tagName, // This tagName is for identifying the tag to remove, part of the path
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing tag {TagName} from task ID: {TaskId}", tagName, taskId);
            var encodedTagName = Uri.EscapeDataString(tagName);
            var endpoint = $"task/{taskId}/tag/{encodedTagName}";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        public Task CreateSpaceTagAsync(string spaceId, Models.RequestModels.Spaces.ModifyTagRequest modifyTagRequest, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Tag> EditSpaceTagAsync(string spaceId, string tagName, Models.RequestModels.Spaces.ModifyTagRequest modifyTagRequest, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
