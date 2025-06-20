using System;
using System.Collections.Generic;
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Tags;
using ClickUp.Api.Client.Models.ResponseModels.Tags; // Assuming GetTagsResponse and EditTagResponse exist

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ITagsService"/> for ClickUp Tag operations.
    /// </summary>
    public class TagsService : ITagsService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public TagsService(IApiConnection apiConnection)
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
        public async Task<IEnumerable<Tag>?> GetSpaceTagsAsync(
            string spaceId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"space/{spaceId}/tag";
            var response = await _apiConnection.GetAsync<GetTagsResponse>(endpoint, cancellationToken); // API returns {"tags": [...]}
            return response?.Tags;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task CreateSpaceTagAsync(
            string spaceId,
            CreateTagRequest createTagRequest,
            CancellationToken cancellationToken = default)
        {
            // Note: The API POST /v2/space/{space_id}/tag expects the tag name directly in the path for creation,
            // along with the body. This is unusual. The CreateTagRequest usually contains the name.
            // Let's assume the endpoint is just /space/{space_id}/tag and the body contains the name and other details.
            // If the tag name MUST be in the path, this interface/method is slightly misaligned with that specific API design.
            // Standard REST would be POST to /space/{space_id}/tag with tag details in body.
            // ClickUp documentation for "Create Space Tag": POST /space/{space_id}/tag with JSON body for tag.
            // It returns the created tag object, but the interface is void.
            var endpoint = $"space/{spaceId}/tag";
            await _apiConnection.PostAsync<CreateTagRequest, Tag>(endpoint, createTagRequest, cancellationToken);
            // Interface is void, so we don't return the result from PostAsync<TRequest, TResponse>
        }

        /// <inheritdoc />
        public async Task<Tag?> EditSpaceTagAsync(
            string spaceId,
            string tagName, // This tagName is for identifying the tag to edit, part of the path
            UpdateTagRequest updateTagRequest, // This contains the new tag details
            CancellationToken cancellationToken = default)
        {
            // Ensure tagName is URL encoded if it can contain special characters
            var encodedTagName = Uri.EscapeDataString(tagName);
            var endpoint = $"space/{spaceId}/tag/{encodedTagName}";
            // API returns {"tag": {...}}
            var response = await _apiConnection.PutAsync<UpdateTagRequest, EditTagResponse>(endpoint, updateTagRequest, cancellationToken);
            return response?.Tag;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteSpaceTagAsync(
            string spaceId,
            string tagName, // This tagName is for identifying the tag to delete, part of the path
            CancellationToken cancellationToken = default)
        {
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
            var encodedTagName = Uri.EscapeDataString(tagName);
            var endpoint = $"task/{taskId}/tag/{encodedTagName}";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }
    }
}
