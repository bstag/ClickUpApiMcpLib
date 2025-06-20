using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Webhooks;
using ClickUp.Api.Client.Models.ResponseModels.Webhooks;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ClickUp.Api.Client.Tests.Services
{
    public class WebhooksServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly WebhooksService _webhooksService;
        private const string BaseWorkspaceEndpoint = "team";


        public WebhooksServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _webhooksService = new WebhooksService(_mockApiConnection.Object);
        }

        private Webhook CreateSampleWebhook(string id, string endpointUrl)
        {
            var webhook = (Webhook)Activator.CreateInstance(typeof(Webhook), nonPublic: true)!;
            var props = typeof(Webhook).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(webhook, id);
            props.FirstOrDefault(p => p.Name == "Endpoint")?.SetValue(webhook, endpointUrl);
            // Set other necessary properties...
            return webhook;
        }

        private CreateWebhookResponse CreateSampleCreateWebhookResponse(Webhook webhook)
        {
             // Assuming CreateWebhookResponse has Id and Webhook properties,
             // or it directly is the Webhook DTO if Id is part of Webhook DTO.
             // For this test, let's assume it has a Webhook property.
            var responseType = typeof(CreateWebhookResponse);
            var constructor = responseType.GetConstructors().FirstOrDefault(c =>
                c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(Webhook)); // Or could be (string id, Webhook webhook)
            if (constructor != null && constructor.GetParameters().Length == 1)
            {
                 // This might not be correct if the constructor expects id and webhook separately.
                 // For simplicity, assuming it can be constructed or properties set.
                 // return (CreateWebhookResponse)constructor.Invoke(new object[] { webhook });
            }
            var instance = (CreateWebhookResponse)Activator.CreateInstance(responseType, nonPublic: true)!;
            responseType.GetProperty("Webhook")?.SetValue(instance, webhook);
            responseType.GetProperty("Id")?.SetValue(instance, webhook.Id); // Assuming Id is also top level
            return instance;
        }


        [Fact]
        public async Task GetWebhooksAsync_WhenWebhooksExist_ReturnsWebhooks()
        {
            // Arrange
            var workspaceId = "ws-id";
            var expectedWebhooks = new List<Webhook> { CreateSampleWebhook("wh-1", "http://example.com/hook1") };
            var expectedResponse = new GetWebhooksResponse(expectedWebhooks); // Assuming GetWebhooksResponse wraps List<Webhook>

            _mockApiConnection.Setup(c => c.GetAsync<GetWebhooksResponse>(
                $"{BaseWorkspaceEndpoint}/{workspaceId}/webhook",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _webhooksService.GetWebhooksAsync(workspaceId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedWebhooks);
        }

        [Fact]
        public async Task CreateWebhookAsync_ValidRequest_ReturnsCreatedWebhook()
        {
            // Arrange
            var workspaceId = "ws-id";
            var requestDto = new CreateWebhookRequest("http://example.com/newhook", new List<string> { "*" });
            var sampleWebhook = CreateSampleWebhook("new-wh-id", "http://example.com/newhook");
            // The service method implementation for CreateWebhookAsync returns response.Webhook
            // So the mock should return a DTO that has a 'Webhook' property.
            var expectedResponsePayload = new CreateWebhookResponse(sampleWebhook.Id, sampleWebhook);


            _mockApiConnection.Setup(c => c.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(
                $"{BaseWorkspaceEndpoint}/{workspaceId}/webhook",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponsePayload);

            // Act
            var result = await _webhooksService.CreateWebhookAsync(workspaceId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleWebhook);
        }
    }
}
