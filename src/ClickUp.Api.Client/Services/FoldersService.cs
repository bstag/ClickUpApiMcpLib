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
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using ClickUp.Api.Client.Models.ResponseModels.Folders; // Assuming GetFoldersResponse exists
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClickUp.Api.Client.Helpers;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IFoldersService"/> for ClickUp Folder operations.
    /// </summary>
    public class FoldersService : IFoldersService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<FoldersService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FoldersService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public FoldersService(IApiConnection apiConnection, ILogger<FoldersService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<FoldersService>.Instance;
        }


        /// <inheritdoc />
        public async Task<IEnumerable<Folder>> GetFoldersAsync(
            string spaceId,
            bool? archived = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting folders for space ID: {SpaceId}, Archived: {Archived}", spaceId, archived);
            var endpoint = $"space/{spaceId}/folder";
            var queryParams = new Dictionary<string, string?>();
            if (archived.HasValue) queryParams["archived"] = archived.Value.ToString().ToLower();
            endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetFoldersResponse>(endpoint, cancellationToken); // API returns {"folders": [...]}
            return response?.Folders ?? Enumerable.Empty<Folder>();
        }

        /// <inheritdoc />
        public async Task<Folder> CreateFolderAsync(
            string spaceId,
            CreateFolderRequest createFolderRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating folder in space ID: {SpaceId}, Name: {FolderName}", spaceId, createFolderRequest.Name);
            var endpoint = $"space/{spaceId}/folder";
            var folder = await _apiConnection.PostAsync<CreateFolderRequest, Folder>(endpoint, createFolderRequest, cancellationToken);
            if (folder == null)
            {
                throw new InvalidOperationException($"Failed to create folder in space {spaceId} or API returned an unexpected null response.");
            }
            return folder;
        }

        /// <inheritdoc />
        public async Task<Folder> GetFolderAsync(
            string folderId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting folder ID: {FolderId}", folderId);
            var endpoint = $"folder/{folderId}";
            var folder = await _apiConnection.GetAsync<Folder>(endpoint, cancellationToken);
            if (folder == null)
            {
                throw new InvalidOperationException($"Failed to get folder {folderId} or API returned an unexpected null response.");
            }
            return folder;
        }

        /// <inheritdoc />
        public async Task<Folder> UpdateFolderAsync(
            string folderId,
            UpdateFolderRequest updateFolderRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating folder ID: {FolderId}, Name: {FolderName}", folderId, updateFolderRequest.Name);
            var endpoint = $"folder/{folderId}";
            var folder = await _apiConnection.PutAsync<UpdateFolderRequest, Folder>(endpoint, updateFolderRequest, cancellationToken);
            if (folder == null)
            {
                throw new InvalidOperationException($"Failed to update folder {folderId} or API returned an unexpected null response.");
            }
            return folder;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteFolderAsync(
            string folderId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting folder ID: {FolderId}", folderId);
            var endpoint = $"folder/{folderId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Folder> CreateFolderFromTemplateAsync(
            string spaceId,
            string templateId,
            CreateFolderFromTemplateRequest createFolderFromTemplateRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating folder from template ID: {TemplateId} in space ID: {SpaceId}, Name: {FolderName}", templateId, spaceId, createFolderFromTemplateRequest.Name);
            var endpoint = $"space/{spaceId}/folderTemplate/{templateId}"; // Corrected endpoint
            var folder = await _apiConnection.PostAsync<CreateFolderFromTemplateRequest, Folder>(endpoint, createFolderFromTemplateRequest, cancellationToken);
            if (folder == null)
            {
                throw new InvalidOperationException($"Failed to create folder from template {templateId} in space {spaceId} or API returned an unexpected null response.");
            }
            return folder;
        }
    }
}
