using System;
using System.Net.Http.Headers;

using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Http;
using ClickUp.Api.Client.Services;

using Microsoft.Extensions.DependencyInjection; // Required for IServiceCollection, AddHttpClient, etc.

namespace ClickUp.Api.Client
{
    /// <summary>
    /// Extension methods for setting up ClickUp API client services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds the ClickUp API client services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="apiToken">The ClickUp API token.</param>
        /// <param name="userAgent">Optional. The user agent string to use for requests. Defaults to "ClickUp.Net.Client".</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddClickUpApiClient(
            this IServiceCollection services,
            string apiToken,
            string userAgent = "ClickUp.Net.Client/1.0") // Default User-Agent
        {
            if (string.IsNullOrWhiteSpace(apiToken))
            {
                throw new ArgumentException("API token cannot be null or whitespace.", nameof(apiToken));
            }

            // Configure HttpClient for IApiConnection
            services.AddHttpClient<IApiConnection, ApiConnection>(client =>
            {
                client.BaseAddress = new Uri("https://api.clickup.com/api/v2/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiToken); // This is for Personal API Token. OAuth would be different.
                client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(Http.Handlers.HttpPolicyBuilders.GetRetryAfterPolicy()) // Handle Retry-After header specifically
            .AddPolicyHandler(Http.Handlers.HttpPolicyBuilders.GetRetryPolicy()) // General transient error retry
            .AddPolicyHandler(Http.Handlers.HttpPolicyBuilders.GetCircuitBreakerPolicy()); // Circuit breaker for repeated failures
            // Consider adding a TimeoutPolicy if long-running requests are an issue: .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)));


            // Register services
            services.AddScoped<IAttachmentsService, AttachmentsService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            // services.AddScoped<IChatsService, ChatsService>();
            // services.AddScoped<IChecklistsService, ChecklistsService>();
            // services.AddScoped<ICommentsService, CommentsService>();
            services.AddScoped<ICustomFieldsService, CustomFieldsService>();
            // services.AddScoped<ICustomTaskTypesService, CustomTaskTypesService>();
            services.AddScoped<IFoldersService, FoldersService>();
            services.AddScoped<IGoalsService, GoalsService>();
            // services.AddScoped<IGroupsService, GroupsService>();
            // services.AddScoped<IGuestsService, GuestsService>();
            services.AddScoped<IListsService, ListsService>();
            // services.AddScoped<IMembersService, MembersService>();
            // services.AddScoped<IRolesService, RolesService>();
            // services.AddScoped<ISharedHierarchyService, SharedHierarchyService>();
            services.AddScoped<ISpacesService, SpacesService>();
            services.AddScoped<ITagsService, TagsService>();
            // services.AddScoped<ITaskChecklistsService, TaskChecklistsService>();
            // services.AddScoped<ITaskRelationshipsService, TaskRelationshipsService>();
            services.AddScoped<ITasksService, TaskService>(); // Corrected from TasksService
            // services.AddScoped<ITeamsService, TeamsService>(); // Assuming ITeamsService is for User Groups / ClickUp "Teams"
            // services.AddScoped<ITemplatesService, TemplatesService>();
            services.AddScoped<ITimeTrackingService, TimeTrackingService>();
            // services.AddScoped<ITimeTrackingLegacyService, TimeTrackingLegacyService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IViewsService, ViewsService>();
            services.AddScoped<IWebhooksService, WebhooksService>();
            // Add other services as they are created/refactored

            return services;
        }
    }
}
