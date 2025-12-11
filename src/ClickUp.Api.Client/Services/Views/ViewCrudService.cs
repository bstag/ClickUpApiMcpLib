using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Views;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;
using ClickUp.Api.Client.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services.Views
{
    /// <summary>
    /// Provides basic CRUD operations for ClickUp Views.
    /// Implements the Single Responsibility Principle by focusing only on view creation, reading, updating, and deletion.
    /// </summary>
    public class ViewCrudService : IViewCrudService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<ViewCrudService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCrudService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        public ViewCrudService(IApiConnection apiConnection, ILogger<ViewCrudService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<ViewCrudService>.Instance;
        }

        /// <inheritdoc />
        public async Task<GetViewResponse> GetViewAsync(string viewId, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(viewId, nameof(viewId));
            
            _logger.LogInformation("Getting view with ID: {ViewId}", viewId);
            
            var endpoint = UrlBuilderHelper.CreateBuilder()
                .WithPathSegments("view", viewId)
                .Build();

            var view = await _apiConnection.GetAsync<GetViewResponse>(endpoint, cancellationToken);
            if (view == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting view {viewId}.");
            }
            
            _logger.LogDebug("Successfully retrieved view {ViewId}", viewId);
            return view;
        }

        /// <inheritdoc />
        public async Task<UpdateViewResponse> UpdateViewAsync(string viewId, UpdateViewRequest updateViewRequest, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(viewId, nameof(viewId));
            if (updateViewRequest == null) throw new ArgumentNullException(nameof(updateViewRequest));
            
            _logger.LogInformation("Updating view with ID: {ViewId}", viewId);
            
            var endpoint = UrlBuilderHelper.CreateBuilder()
                .WithPathSegments("view", viewId)
                .Build();

            var view = await _apiConnection.PutAsync<UpdateViewRequest, UpdateViewResponse>(endpoint, updateViewRequest, cancellationToken);
            if (view == null)
            {
                throw new InvalidOperationException($"API connection returned null response when updating view {viewId}.");
            }
            
            _logger.LogInformation("Successfully updated view {ViewId}", viewId);
            return view;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteViewAsync(string viewId, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(viewId, nameof(viewId));
            
            _logger.LogInformation("Deleting view with ID: {ViewId}", viewId);
            
            var endpoint = UrlBuilderHelper.CreateBuilder()
                .WithPathSegments("view", viewId)
                .Build();

            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
            _logger.LogInformation("Successfully deleted view {ViewId}", viewId);
        }
    }
}