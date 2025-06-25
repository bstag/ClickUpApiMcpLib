using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ClickUp.Api.Client.Abstractions.Options;
using ClickUp.Api.Client.Extensions; // For AddClickUpClient
using System;
using System.IO;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure; // Required for RecordingDelegatingHandler
using RichardSzalay.MockHttp; // Required for MockHttpMessageHandler

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    public enum TestMode
    {
        Passthrough, // Actual API calls, no recording, no mocking
        Record,      // Actual API calls, records responses
        Playback     // Uses mocked responses, no actual API calls
    }

    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly IConfigurationRoot Configuration;
        protected readonly ServiceProvider ServiceProvider;
        protected readonly ClickUpClientOptions ClientOptions;
        protected readonly TestMode CurrentTestMode;
        protected readonly string RecordedResponsesBasePath;
        protected MockHttpMessageHandler? MockHttpHandler { get; private set; } // Only used in Playback mode


        protected IntegrationTestBase()
        {
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables() // Load from environment variables
                .AddUserSecrets<IntegrationTestBase>() // Load from User Secrets using the assembly containing this class
                .Build();

            ClientOptions = new ClickUpClientOptions();
            Configuration.GetSection("ClickUpApi").Bind(ClientOptions);

            // Validate essential configuration based on TestMode
            if (CurrentTestMode != TestMode.Playback)
            {
                if (string.IsNullOrWhiteSpace(ClientOptions.PersonalAccessToken))
                {
                    throw new InvalidOperationException("ClickUp API PersonalAccessToken not configured for integration tests. " +
                                                        "Set via User Secrets (e.g., 'ClickUpApi:PersonalAccessToken') " +
                                                        "or Environment Variables (e.g., 'ClickUpApi__PersonalAccessToken'). " +
                                                        "Required for Record and Passthrough modes.");
                }
                if (string.IsNullOrWhiteSpace(ClientOptions.TestWorkspaceId))
                {
                    // This ID is often part of the URL path, so it's needed even for recording.
                    throw new InvalidOperationException("ClickUp API TestWorkspaceId not configured for integration tests. " +
                                                        "Set via User Secrets (e.g., 'ClickUpApi:TestWorkspaceId') " +
                                                        "or Environment Variables (e.g., 'ClickUpApi__TestWorkspaceId'). " +
                                                        "Required for Record and Passthrough modes.");
                }
            }
            // For Playback mode, PersonalAccessToken and TestWorkspaceId might not be strictly necessary if all requests are mocked
            // and mock paths don't rely on them. However, they are loaded if present.

            var testModeEnv = Environment.GetEnvironmentVariable("CLICKUP_SDK_TEST_MODE");
            Enum.TryParse(testModeEnv, true, out TestMode mode); // Defaults to Passthrough (0) if parsing fails
            CurrentTestMode = mode;

            // Define base path for recorded responses - relative to project output directory.
            // Adjust if tests run from a different working directory.
            // For a structure like: <test_project_root>/bin/Debug/net9.0/test-data/recorded-responses
            // We need to go up a few levels from AppContext.BaseDirectory to reach test project root typically.
            // This might need adjustment depending on the test runner's working directory.
            // For simplicity, let's assume a path that can be created from AppContext.BaseDirectory for now.
            string projectRoot = GetProjectDirectory();
            RecordedResponsesBasePath = Path.Combine(projectRoot, "test-data", "recorded-responses");

            Console.WriteLine($"[IntegrationTestBase] Test Mode: {CurrentTestMode}");
            Console.WriteLine($"[IntegrationTestBase] Recorded Responses Base Path: {RecordedResponsesBasePath}");


            var services = new ServiceCollection();
            var clientBuilder = services.AddClickUpClient(opts =>
            {
                opts.PersonalAccessToken = ClientOptions.PersonalAccessToken; // Still set it, might be used by some passthrough scenarios or if recording needs auth
                if (!string.IsNullOrWhiteSpace(ClientOptions.BaseAddress) &&
                    Uri.TryCreate(ClientOptions.BaseAddress, UriKind.Absolute, out _))
                {
                    opts.BaseAddress = ClientOptions.BaseAddress;
                }
            });

            if (CurrentTestMode == TestMode.Record)
            {
                services.AddTransient<RecordingDelegatingHandler>(sp => new RecordingDelegatingHandler(RecordedResponsesBasePath));
                clientBuilder.AddHttpMessageHandler<RecordingDelegatingHandler>();
                Console.WriteLine("[IntegrationTestBase] RecordingDelegatingHandler added to pipeline for Record mode.");
            }
            else if (CurrentTestMode == TestMode.Playback)
            {
                MockHttpHandler = new MockHttpMessageHandler();
                // The primary handler for MockHttp is configured via clientBuilder.ConfigurePrimaryHttpMessageHandler
                // or by passing the mock handler when creating an HttpClient directly.
                // For IHttpClientFactory, it's usually:
                clientBuilder.ConfigurePrimaryHttpMessageHandler(() => MockHttpHandler);
                Console.WriteLine("[IntegrationTestBase] MockHttpMessageHandler configured as primary for Playback mode.");
            }
            // In Passthrough mode, no additional handlers are added here; it uses the default setup.

            ServiceProvider = services.BuildServiceProvider();
        }

        private static string GetProjectDirectory()
        {
            // AppContext.BaseDirectory is typically <project_folder>/bin/Debug/netX.Y/
            var assemblyLocation = typeof(IntegrationTestBase).Assembly.Location; // Path to the DLL, e.g., /app/src/ClickUp.Api.Client.IntegrationTests/bin/Debug/net9.0/ClickUp.Api.Client.IntegrationTests.dll
            var assemblyDir = Path.GetDirectoryName(assemblyLocation); // e.g., /app/src/ClickUp.Api.Client.IntegrationTests/bin/Debug/net9.0/

            if (string.IsNullOrEmpty(assemblyDir))
            {
                 Console.WriteLine($"[IntegrationTestBase] Warning: Could not get directory from assembly location '{assemblyLocation}'. Using AppContext.BaseDirectory as fallback for project root.");
                return AppContext.BaseDirectory;
            }

            // Try to go up to the project directory (assuming standard bin/Debug/netX.Y or bin/Release/netX.Y structure)
            var projectDirInfo = new DirectoryInfo(assemblyDir).Parent?.Parent?.Parent;

            if (projectDirInfo != null && projectDirInfo.Exists && File.Exists(Path.Combine(projectDirInfo.FullName, "ClickUp.Api.Client.IntegrationTests.csproj")))
            {
                return projectDirInfo.FullName;
            }

            // Fallback if the above heuristic doesn't find the csproj, could be a custom build output path
            Console.WriteLine($"[IntegrationTestBase] Warning: Could not reliably determine test project root from assembly location '{assemblyLocation}'. Using directory of assembly as fallback: {assemblyDir}");
            // This fallback might place test-data inside bin/Debug/net9.0/test-data, which is not ideal but better than /app/test-data
            return assemblyDir;
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
