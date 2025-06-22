using System;
using System.Net.Http; // Required for HttpResponseMessage in logging
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Options;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Http;
using ClickUp.Api.Client.Http.Handlers;
using ClickUp.Api.Client.Services;
using Polly;
using Polly.Extensions.Http;

namespace ClickUp.Api.Client.Extensions
{
    /// <summary>
    /// Extension methods for setting up ClickUp API client services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the ClickUp API client services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configureOptions">An <see cref="Action{ClickUpClientOptions}"/> to configure the provided <see cref="ClickUpClientOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">Thrown if services or configureOptions is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if PersonalAccessToken or BaseAddress is not configured.</exception>
        public static IServiceCollection AddClickUpClient(this IServiceCollection services, Action<ClickUpClientOptions> configureOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            services.Configure(configureOptions);

            services.AddTransient<AuthenticationDelegatingHandler>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ClickUpClientOptions>>().Value;
                if (string.IsNullOrWhiteSpace(options.PersonalAccessToken))
                {
                    throw new InvalidOperationException("Personal Access Token is not configured in ClickUpClientOptions. Please provide a PersonalAccessToken.");
                }
                return new AuthenticationDelegatingHandler(options.PersonalAccessToken);
            });

            var jitterer = new Random(); // For jitter in retry policy

            services.AddHttpClient<IApiConnection, ApiConnection>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ClickUpClientOptions>>().Value;
                if (string.IsNullOrWhiteSpace(options.BaseAddress))
                {
                    throw new InvalidOperationException("BaseAddress is not configured in ClickUpClientOptions.");
                }
                client.BaseAddress = new Uri(options.BaseAddress);
                client.DefaultRequestHeaders.Add("User-Agent", "ClickUp.Api.Client.Net");
            })
                .AddHttpMessageHandler<AuthenticationDelegatingHandler>()
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                                         + TimeSpan.FromMilliseconds(jitterer.Next(0, 500)), // Jitter
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        // Using Console.WriteLine for now. In a real app, use ILogger.
                        Console.WriteLine($"[PollyRetry] Request to {outcome.Result?.RequestMessage?.RequestUri} failed with {outcome.Result?.StatusCode}. Delaying for {timespan.TotalMilliseconds}ms, then making retry {retryAttempt}. CorrelationId: {context.CorrelationId}");
                    }
                ))
                .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, breakDelay, context) =>
                    {
                        Console.WriteLine($"[PollyCircuitBreaker] Circuit broken for {breakDelay.TotalSeconds}s for request to {outcome.Result?.RequestMessage?.RequestUri} due to {outcome.Result?.StatusCode}. CorrelationId: {context.CorrelationId}");
                    },
                    onReset: (context) =>
                    {
                        Console.WriteLine($"[PollyCircuitBreaker] Circuit reset. CorrelationId: {context.OperationKey}");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine("[PollyCircuitBreaker] Circuit is now half-open; next request is a trial.");
                    }
                ));

            // Register all the services
            services.AddTransient<ITasksService, TaskService>();
            services.AddTransient<IAttachmentsService, AttachmentsService>();
            services.AddTransient<IAuthorizationService, AuthorizationService>();
            services.AddTransient<ITaskChecklistsService, TaskChecklistsService>();
            services.AddTransient<ITaskRelationshipsService, TaskRelationshipsService>();
            services.AddTransient<ITemplatesService, TemplatesService>();
            services.AddTransient<IRolesService, RolesService>();
            services.AddTransient<ISharedHierarchyService, SharedHierarchyService>();
            services.AddTransient<ICommentsService, CommentService>();
            services.AddTransient<ICustomFieldsService, CustomFieldsService>();
            services.AddTransient<IFoldersService, FoldersService>();
            services.AddTransient<IGoalsService, GoalsService>();
            services.AddTransient<IGuestsService, GuestsService>();
            services.AddTransient<IListsService, ListsService>();
            services.AddTransient<IMembersService, MembersService>();
            services.AddTransient<ISpacesService, SpacesService>();
            services.AddTransient<ITagsService, TagsService>();
            services.AddTransient<IDocsService, DocsService>();
            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<ITimeTrackingService, TimeTrackingService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IViewsService, ViewsService>();
            services.AddTransient<IWebhooksService, WebhooksService>();
            services.AddTransient<IWorkspacesService, WorkspacesService>();
            services.AddTransient<IUserGroupsService, UserGroupsService>();

            return services;
        }
    }
}
