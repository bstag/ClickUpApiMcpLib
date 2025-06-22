using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Webhooks; // Added for Webhook and WebhookHealth
using ClickUp.Api.Client.Models.RequestModels.Webhooks;
using ClickUp.Api.Client.Models.ResponseModels.Webhooks; // Assuming GetWebhooksResponse, CreateWebhookResponse, UpdateWebhookResponse
using System.Linq; // For Enumerable.Empty
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IWebhooksService"/> for ClickUp Webhook operations.
    /// </summary>
    public class WebhooksService : IWebhooksService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<WebhooksService> _logger;
        private const string BaseWorkspaceEndpoint = "team"; // ClickUp v2 uses "team/{team_id}" for workspace context

        /// <summary>
        /// Initializes a new instance of the <see cref="WebhooksService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public WebhooksService(IApiConnection apiConnection, ILogger<WebhooksService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<WebhooksService>.Instance;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Webhook>> GetWebhooksAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting webhooks for workspace ID: {WorkspaceId}", workspaceId);
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/webhook";
            var response = await _apiConnection.GetAsync<GetWebhooksResponse>(endpoint, cancellationToken); // API returns {"webhooks": [...]}
            return response?.Webhooks ?? Enumerable.Empty<Webhook>();
        }

        /// <inheritdoc />
        public async Task<Webhook> CreateWebhookAsync(
            string workspaceId,
            CreateWebhookRequest createWebhookRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating webhook in workspace ID: {WorkspaceId}, Endpoint: {WebhookEndpoint}", workspaceId, createWebhookRequest.Endpoint);
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/webhook";
            var responseWrapper = await _apiConnection.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(endpoint, createWebhookRequest, cancellationToken);
            if (responseWrapper?.Webhook == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty webhook data when creating webhook in workspace {workspaceId}.");
            }
            return responseWrapper.Webhook;
        }

        /// <inheritdoc />
        public async Task<Webhook> UpdateWebhookAsync(
            string webhookId, // This is the webhook_id from the path, not workspaceId
            UpdateWebhookRequest updateWebhookRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating webhook ID: {WebhookId}, Endpoint: {WebhookEndpoint}", webhookId, updateWebhookRequest.Endpoint);
            var endpoint = $"webhook/{webhookId}";
            var responseWrapper = await _apiConnection.PutAsync<UpdateWebhookRequest, UpdateWebhookResponse>(endpoint, updateWebhookRequest, cancellationToken);
            if (responseWrapper?.Webhook == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty webhook data when updating webhook {webhookId}.");
            }
            return responseWrapper.Webhook;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteWebhookAsync(
            string webhookId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting webhook ID: {WebhookId}", webhookId);
            var endpoint = $"webhook/{webhookId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }
    }
}
