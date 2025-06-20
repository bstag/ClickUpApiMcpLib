using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Webhooks;
using ClickUp.Api.Client.Models.ResponseModels.Webhooks; // Assuming GetWebhooksResponse, CreateWebhookResponse, UpdateWebhookResponse

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IWebhooksService"/> for ClickUp Webhook operations.
    /// </summary>
    public class WebhooksService : IWebhooksService
    {
        private readonly IApiConnection _apiConnection;
        private const string BaseWorkspaceEndpoint = "team"; // ClickUp v2 uses "team/{team_id}" for workspace context

        /// <summary>
        /// Initializes a new instance of the <see cref="WebhooksService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public WebhooksService(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Webhook>?> GetWebhooksAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/webhook";
            var response = await _apiConnection.GetAsync<GetWebhooksResponse>(endpoint, cancellationToken); // API returns {"webhooks": [...]}
            return response?.Webhooks;
        }

        /// <inheritdoc />
        public async Task<Webhook?> CreateWebhookAsync(
            string workspaceId,
            CreateWebhookRequest createWebhookRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/webhook";
            // API returns {"id": "...", "webhook": {...}}
            var response = await _apiConnection.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(endpoint, createWebhookRequest, cancellationToken);
            // The interface expects Webhook, so we return response.Webhook (assuming CreateWebhookResponse has Id and Webhook properties)
            // If the response directly IS the webhook with its ID, then PostAsync<..., Webhook> would be used.
            // The typical ClickUp response includes the "webhook" object and its "id" at the top level of the response.
            // So, CreateWebhookResponse should model this structure.
            return response?.Webhook;
        }

        /// <inheritdoc />
        public async Task<Webhook?> UpdateWebhookAsync(
            string webhookId, // This is the webhook_id from the path, not workspaceId
            UpdateWebhookRequest updateWebhookRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"webhook/{webhookId}";
            // API returns {"id": "...", "webhook": {...}}
            var response = await _apiConnection.PutAsync<UpdateWebhookRequest, UpdateWebhookResponse>(endpoint, updateWebhookRequest, cancellationToken);
            return response?.Webhook;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteWebhookAsync(
            string webhookId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"webhook/{webhookId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }
    }
}
