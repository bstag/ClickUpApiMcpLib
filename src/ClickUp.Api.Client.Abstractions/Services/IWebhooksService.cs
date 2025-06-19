using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Webhooks operations in the ClickUp API
    // Based on endpoints like:
    // - GET /v2/team/{team_id}/webhook
    // - POST /v2/team/{team_id}/webhook
    // - PUT /v2/webhook/{webhook_id}
    // - DELETE /v2/webhook/{webhook_id}

    public interface IWebhooksService
    {
        /// <summary>
        /// Retrieves Webhooks created by the authenticated user for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <returns>A list of Webhooks for the Workspace.</returns>
        Task<IEnumerable<object>> GetWebhooksAsync(double workspaceId);
        // Note: Return type should be IEnumerable<WebhookDto>. Response is { "webhooks": [...] }.

        /// <summary>
        /// Creates a new Webhook.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id) where the webhook will be created.</param>
        /// <param name="createWebhookRequest">Details of the Webhook to create.</param>
        /// <returns>The created Webhook details, including its ID and secret.</returns>
        Task<object> CreateWebhookAsync(double workspaceId, object createWebhookRequest);
        // Note: createWebhookRequest should be CreateWebhookRequest. Return type should be a DTO containing "id" and "webhook" (WebhookDto).

        /// <summary>
        /// Updates a Webhook.
        /// </summary>
        /// <param name="webhookId">The UUID of the Webhook to update.</param>
        /// <param name="updateWebhookRequest">Details for updating the Webhook (endpoint, events, status).</param>
        /// <returns>The updated Webhook details.</returns>
        Task<object> UpdateWebhookAsync(string webhookId, object updateWebhookRequest);
        // Note: updateWebhookRequest should be UpdateWebhookRequest. Return type should be a DTO containing "id" and "webhook" (WebhookDto).

        /// <summary>
        /// Deletes a Webhook.
        /// </summary>
        /// <param name="webhookId">The UUID of the Webhook to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteWebhookAsync(string webhookId);
        // Note: API returns 200 with an empty object.
    }
}
