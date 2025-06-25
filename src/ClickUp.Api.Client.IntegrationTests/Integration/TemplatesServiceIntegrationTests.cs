using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure;
using ClickUp.Api.Client.Models.ResponseModels.Templates;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using Xunit;
using Xunit.Abstractions;

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class TemplatesServiceIntegrationTests : IntegrationTestBase, IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private readonly ITemplatesService _templatesService;
        private readonly string _testWorkspaceId;

        public TemplatesServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _templatesService = ServiceProvider.GetRequiredService<ITemplatesService>();
            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"]!;

            if (string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured. Test setup will fail for tests requiring a workspace ID.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for TemplatesServiceIntegrationTests.");
            }
        }

        public Task InitializeAsync()
        {
            _output.LogInformation($"[TemplatesServiceIntegrationTests] Initializing for Workspace: {_testWorkspaceId}. Test Mode: {CurrentTestMode}");
            // No specific setup needed for GetTaskTemplatesAsync as it's a read-only operation.
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _output.LogInformation($"[TemplatesServiceIntegrationTests] Disposing.");
            // No resources to clean up for GetTaskTemplatesAsync.
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetTaskTemplatesAsync_WhenCalled_ShouldReturnTemplates()
        {
            // Arrange
            Assert.False(string.IsNullOrWhiteSpace(_testWorkspaceId), "TestWorkspaceId must be configured.");
            _output.LogInformation($"Getting task templates for workspace: {_testWorkspaceId}, Page: 0. Test Mode: {CurrentTestMode}");

            var expectedUrl = $"https://api.clickup.com/api/v2/team/{_testWorkspaceId}/taskTemplate?page=0";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "TemplatesService", "GETGetTaskTemplates", "Success_querypage_0.json");

                _output.LogInformation($"[Playback] Looking for mock response file: {responsePath}");
                Assert.True(File.Exists(responsePath), $"Mock data file not found: {responsePath}");

                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When(expectedUrl)
                               .Respond("application/json", responseContent);
                _output.LogInformation($"[Playback] Mocking GET task templates. Url: {expectedUrl}. Response from: {responsePath}");
            }

            // Act
            GetTaskTemplatesResponse? result = null;
            try
            {
                result = await _templatesService.GetTaskTemplatesAsync(_testWorkspaceId, 0);
            }
            catch (Exception ex)
            {
                _output.LogError($"GetTaskTemplatesAsync threw an exception: {ex.Message}. URL: {expectedUrl}", ex);
                if (CurrentTestMode == TestMode.Record) _output.LogWarning("Ensure you are in Record mode if you intend to capture this request.");
                Assert.Fail($"GetTaskTemplatesAsync threw an exception: {ex.Message}");
            }

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Templates);

            if (CurrentTestMode != TestMode.Playback)
            {
                // In live modes, we expect the test workspace to have at least one task template.
                // This might need adjustment based on actual test workspace setup.
                // For now, we'll be flexible and just log if it's empty.
                if (!result.Templates.Any())
                {
                    _output.LogWarning($"[Record/Passthrough] No task templates found in workspace {_testWorkspaceId}. This might be unexpected for a live test.");
                }
            }
            else
            {
                 // In Playback mode, the number of templates depends on the mock JSON.
                 // If the mock JSON is empty, this is fine.
                 if (!result.Templates.Any())
                 {
                    _output.LogInformation("[Playback] Retrieved 0 task templates as per mock data.");
                 }
            }

            _output.LogInformation($"Successfully retrieved {result.Templates.Count()} task templates.");
            foreach (var template in result.Templates)
            {
                Assert.False(string.IsNullOrWhiteSpace(template.Id));
                Assert.False(string.IsNullOrWhiteSpace(template.Name));
                _output.LogInformation($"Template - ID: {template.Id}, Name: {template.Name}");
            }
        }
    }
}
