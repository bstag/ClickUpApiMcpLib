using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ClickUp.Api.Client.Abstractions.Options;
using ClickUp.Api.Client.Extensions; // For AddClickUpClient
using System;

namespace ClickUp.Api.Client.Tests.Integration
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly IConfigurationRoot Configuration;
        protected readonly ServiceProvider ServiceProvider;
        protected readonly ClickUpClientOptions ClientOptions;

        protected IntegrationTestBase()
        {
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables() // Load from environment variables
                .AddUserSecrets<IntegrationTestBase>() // Load from User Secrets using the assembly containing this class
                .Build();

            ClientOptions = new ClickUpClientOptions();
            // Bind the "ClickUpApi" section from configuration to ClientOptions
            Configuration.GetSection("ClickUpApi").Bind(ClientOptions);

            if (string.IsNullOrWhiteSpace(ClientOptions.PersonalAccessToken))
            {
                throw new InvalidOperationException("ClickUp API PersonalAccessToken not configured for integration tests. "+
                                                    "Set via User Secrets (e.g., 'ClickUpApi:PersonalAccessToken') " +
                                                    "or Environment Variables (e.g., 'ClickUpApi__PersonalAccessToken').");
            }

            var services = new ServiceCollection();
            services.AddClickUpClient(opts =>
            {
                opts.PersonalAccessToken = ClientOptions.PersonalAccessToken;
                if (!string.IsNullOrWhiteSpace(ClientOptions.BaseAddress) &&
                    Uri.TryCreate(ClientOptions.BaseAddress, UriKind.Absolute, out _)) // Ensure BaseAddress from config is valid
                {
                    opts.BaseAddress = ClientOptions.BaseAddress;
                }
                // else it will use the default from ClickUpClientOptions
            });
            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServiceProvider?.Dispose();
            }
        }
    }
}
