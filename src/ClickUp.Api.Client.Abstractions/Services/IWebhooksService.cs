using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Webhooks; // Assuming Webhook DTO is here
using ClickUp.Api.Client.Models.RequestModels.Webhooks; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Webhooks operations in the ClickUp API.
    /// This service allows for creating, retrieving, updating, and deleting webhooks for a Workspace.
    /// </summary>
    /// <remarks>
    /// Webhooks are used to receive notifications about events that happen in ClickUp.
    /// Based on ClickUp API endpoints like:
    /// <list type="bullet">
    /// <item><description>GET /v2/team/{team_id}/webhook</description></item>
    /// <item><description>POST /v2/team/{team_id}/webhook</description></item>
    /// <item><description>PUT /v2/webhook/{webhook_id}</description></item>
    /// <item><description>DELETE /v2/webhook/{webhook_id}</description></item>
    /// </list>
    /// </remarks>
    public interface IWebhooksService
    {
        /// <summary>
        /// Retrieves all Webhooks created by the authenticated user for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team) for which to retrieve webhooks.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="Webhook"/> objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid workspace ID or authentication issues.</exception>
        Task<IEnumerable<Webhook>> GetWebhooksAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Webhook for a specific Workspace to listen for specified events.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team) where the webhook will be created.</param>
        /// <param name="createWebhookRequest">A <see cref="CreateWebhookRequest"/> object containing details for the new webhook, such as the endpoint URL and events to subscribe to.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="Webhook"/> details, including its ID and secret key.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createWebhookRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid endpoint URL or event types.</exception>
        Task<Webhook> CreateWebhookAsync(
            string workspaceId,
            CreateWebhookRequest createWebhookRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing Webhook's endpoint URL, subscribed events, or status.
        /// </summary>
        /// <param name="webhookId">The UUID of the Webhook to update.</param>
        /// <param name="updateWebhookRequest">An <see cref="UpdateWebhookRequest"/> object containing the updated details for the webhook.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Webhook"/> details.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="webhookId"/> or <paramref name="updateWebhookRequest"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as webhook not found or invalid update parameters.</exception>
        Task<Webhook> UpdateWebhookAsync(
            string webhookId,
            UpdateWebhookRequest updateWebhookRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an existing Webhook.
        /// </summary>
        /// <param name="webhookId">The UUID of the Webhook to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="webhookId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as webhook not found or insufficient permissions.</exception>
        System.Threading.Tasks.Task DeleteWebhookAsync(
            string webhookId,
            CancellationToken cancellationToken = default);
    }
}
