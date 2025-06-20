using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities; // Assuming Webhook DTO is here
using ClickUp.Api.Client.Models.RequestModels.Webhooks; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Webhooks operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/team/{team_id}/webhook
    /// - POST /v2/team/{team_id}/webhook
    /// - PUT /v2/webhook/{webhook_id}
    /// - DELETE /v2/webhook/{webhook_id}
    /// </remarks>
    public interface IWebhooksService
    {
        /// <summary>
        /// Retrieves Webhooks created by the authenticated user for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Webhook"/> objects for the Workspace.</returns>
        Task<IEnumerable<Webhook>> GetWebhooksAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Webhook.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team) where the webhook will be created.</param>
        /// <param name="createWebhookRequest">Details of the Webhook to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="Webhook"/> details, including its ID and secret.</returns>
        Task<Webhook> CreateWebhookAsync(
            string workspaceId,
            CreateWebhookRequest createWebhookRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a Webhook.
        /// </summary>
        /// <param name="webhookId">The UUID of the Webhook to update.</param>
        /// <param name="updateWebhookRequest">Details for updating the Webhook (endpoint, events, status).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="Webhook"/> details.</returns>
        Task<Webhook> UpdateWebhookAsync(
            string webhookId,
            UpdateWebhookRequest updateWebhookRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a Webhook.
        /// </summary>
        /// <param name="webhookId">The UUID of the Webhook to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task DeleteWebhookAsync(
            string webhookId,
            CancellationToken cancellationToken = default);
    }
}
