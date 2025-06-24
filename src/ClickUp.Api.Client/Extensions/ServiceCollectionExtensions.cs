using System;
using System.Net.Http; // Required for HttpResponseMessage in logging
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging; // Added for ILogger
using Microsoft.Extensions.Options;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Options;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Http;
using ClickUp.Api.Client.Http.Handlers;
using ClickUp.Api.Client.Services;
using Polly;
using Polly.CircuitBreaker; // Required for BrokenCircuitException
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
        /// <param name="configureClientOptions">An <see cref="Action{ClickUpClientOptions}"/> to configure the provided <see cref="ClickUpClientOptions"/>.</param>
        /// <param name="configurePollyOptions">An optional <see cref="Action{ClickUpPollyOptions}"/> to configure Polly resilience policies.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">Thrown if services or configureClientOptions is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if PersonalAccessToken or BaseAddress is not configured in ClickUpClientOptions.</exception>
        public static IServiceCollection AddClickUpClient(
            this IServiceCollection services,
            Action<ClickUpClientOptions> configureClientOptions,
            Action<ClickUpPollyOptions>? configurePollyOptions = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configureClientOptions == null) throw new ArgumentNullException(nameof(configureClientOptions));

            services.AddOptions(); // Ensure IOptions system is available

            services.Configure(configureClientOptions);

            // Configure Polly options. If configurePollyOptions is null, IOptions will provide defaults from ClickUpPollyOptions constructor or appsettings.
            if (configurePollyOptions != null)
            {
                services.Configure(configurePollyOptions);
            }
            else
            {
                // Ensure ClickUpPollyOptions is registered so IOptions<ClickUpPollyOptions> can be resolved
                // This allows it to be bound from configuration (e.g. appsettings) or use default values if not specified.
                services.Configure<ClickUpPollyOptions>(_ => { });
            }


            services.AddTransient<AuthenticationDelegatingHandler>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ClickUpClientOptions>>().Value;
                // PersonalAccessToken and OAuthAccessToken will be checked within the handler itself.
                return new AuthenticationDelegatingHandler(sp.GetRequiredService<IOptions<ClickUpClientOptions>>());
            });

            var jitterer = new Random(); // For jitter in retry policy

            services.AddHttpClient<IApiConnection, ApiConnection>((sp, client) =>
            {
                var clientOptions = sp.GetRequiredService<IOptions<ClickUpClientOptions>>().Value;
                if (string.IsNullOrWhiteSpace(clientOptions.BaseAddress))
                {
                    // Default to ClickUp's v2 API base address if not specified
                    clientOptions.BaseAddress = "https://api.clickup.com/api/v2/";
                }
                client.BaseAddress = new Uri(clientOptions.BaseAddress);
                client.DefaultRequestHeaders.Add("User-Agent", "ClickUp.Api.Client.Net");
            })
                .AddHttpMessageHandler<AuthenticationDelegatingHandler>()
                .AddPolicyHandler((sp, request) => GetRetryPolicy(sp, jitterer))
                .AddPolicyHandler((sp, request) => GetCircuitBreakerPolicy(sp));

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

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider serviceProvider, Random jitterer)
        {
            var pollyOptions = serviceProvider.GetRequiredService<IOptions<ClickUpPollyOptions>>().Value;
            var logger = serviceProvider.GetRequiredService<ILogger<ApiConnection>>(); // Changed to ILogger<ApiConnection>

            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .Or<BrokenCircuitException>() // Retry if the circuit breaker allows it
                .WaitAndRetryAsync(
                    retryCount: pollyOptions.RetryCount,
                    sleepDurationProvider: (retryAttempt, response, context) =>
                    {
                        var retryAfterDelay = TimeSpan.Zero;
                        bool isRetryAfter = false;

                        if (response?.Result?.Headers?.RetryAfter != null)
                        {
                            if (response.Result.Headers.RetryAfter.Delta.HasValue)
                            {
                                retryAfterDelay = response.Result.Headers.RetryAfter.Delta.Value;
                                isRetryAfter = true;
                                logger.LogInformation(
                                    "[PollyRetry] HTTP 429 with Retry-After (delta) received. Delaying for {RetryAfterDelay}. CorrelationId: {CorrelationId}, Request: {RequestUri}",
                                    retryAfterDelay, context.CorrelationId, response.Result.RequestMessage?.RequestUri);
                            }
                            else if (response.Result.Headers.RetryAfter.Date.HasValue)
                            {
                                var delay = response.Result.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
                                if (delay > TimeSpan.Zero)
                                {
                                    retryAfterDelay = delay;
                                    isRetryAfter = true;
                                    logger.LogInformation(
                                        "[PollyRetry] HTTP 429 with Retry-After (date) received. Delaying for {RetryAfterDelay}. CorrelationId: {CorrelationId}, Request: {RequestUri}",
                                        retryAfterDelay, context.CorrelationId, response.Result.RequestMessage?.RequestUri);
                                }
                            }
                        }

                        if (isRetryAfter)
                        {
                            return retryAfterDelay;
                        }

                        // Default exponential backoff with jitter
                        return TimeSpan.FromSeconds(Math.Pow(pollyOptions.RetryBaseDelay.TotalSeconds, retryAttempt))
                               + TimeSpan.FromMilliseconds(jitterer.Next(0, 500));
                    },
                    onRetryAsync: (outcome, timespan, retryAttempt, context) =>
                    {
                        logger.LogWarning(
                            outcome.Exception, // Log the exception if any
                            "[PollyRetry] Request to {RequestUri} failed with {StatusCode}. Delaying for {Timespan}ms, then making retry {RetryAttempt}. CorrelationId: {CorrelationId}",
                            outcome.Result?.RequestMessage?.RequestUri,
                            outcome.Result?.StatusCode,
                            timespan.TotalMilliseconds,
                            retryAttempt,
                            context.CorrelationId);
                        return Task.CompletedTask; // onRetryAsync expects a Task
                    }
                );
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(IServiceProvider serviceProvider)
        {
            var pollyOptions = serviceProvider.GetRequiredService<IOptions<ClickUpPollyOptions>>().Value;
            var logger = serviceProvider.GetRequiredService<ILogger<ApiConnection>>(); // Changed to ILogger<ApiConnection>

            return HttpPolicyExtensions
                .HandleTransientHttpError() // Catches HttpRequestException, 5XX and 408
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: pollyOptions.CircuitBreakerFailureThreshold,
                    durationOfBreak: pollyOptions.CircuitBreakerBreakDuration,
                    onBreak: (outcome, breakDelay, context) =>
                    {
                        logger.LogError(
                            outcome.Exception, // Log the exception if any
                            "[PollyCircuitBreaker] Circuit broken for {BreakDelay} for request to {RequestUri} due to {StatusCode}. CorrelationId: {CorrelationId}",
                            breakDelay,
                            outcome.Result?.RequestMessage?.RequestUri,
                            outcome.Result?.StatusCode,
                            context.CorrelationId);
                    },
                    onReset: (context) =>
                    {
                        logger.LogInformation(
                            "[PollyCircuitBreaker] Circuit reset. CorrelationId: {CorrelationId}",
                            context.CorrelationId);  // Context in onReset has CorrelationId
                    },
                    onHalfOpen: () => // This delegate does not have context
                    {
                        logger.LogInformation("[PollyCircuitBreaker] Circuit is now half-open; next request is a trial.");
                    }
                );
        }
    }
}
