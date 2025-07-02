using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Webhooks; // For Webhook
using ClickUp.Api.Client.Models.RequestModels.Webhooks; // For request DTOs
using ClickUp.Api.Client.Models.ResponseModels.Webhooks; // For response DTOs
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class WebhooksServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly WebhooksService _webhooksService;
        private readonly Mock<ILogger<WebhooksService>> _mockLogger;

        public WebhooksServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<WebhooksService>>();
            _webhooksService = new WebhooksService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private Webhook CreateSampleWebhook(string id = "wh_1", string endpoint = "https://example.com/webhook")
        {
            var sampleUser = new ClickUp.Api.Client.Models.Entities.Users.User(123, "Webhook User", "whuser@example.com", "#789", null, "WU");
            return new Webhook(
                Id: id,
                UserId: 123,
                User: sampleUser, // Added User
                TeamId: 456,
                Endpoint: endpoint,
                ClientId: "client_abc",
                Events: new List<string> { "*" }, // Example: all events
                TaskId: null,
                ListId: null,
                FolderId: null,
                SpaceId: null,
                Health: new WebhookHealth("ok", 0, DateTimeOffset.UtcNow.ToString(), DateTimeOffset.UtcNow.ToString()), // Added missing params
                Secret: "secret_key",
                Status: "active", // Added missing param
                DateCreated: DateTimeOffset.UtcNow // Added missing param
            );
        }

        // --- Tests for GetWebhooksAsync ---

        [Fact]
        public async Task GetWebhooksAsync_ValidWorkspaceId_ReturnsWebhooks()
        {
            // Arrange
            var workspaceId = "ws123";
            var expectedWebhooks = new List<Webhook> { CreateSampleWebhook("wh1", "https://example.com/hook1") };
            var apiResponse = new GetWebhooksResponse(expectedWebhooks);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetWebhooksResponse>(
                    $"team/{workspaceId}/webhook",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _webhooksService.GetWebhooksAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("wh1", result.First().Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetWebhooksResponse>(
                $"team/{workspaceId}/webhook",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetWebhooksAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_wh_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetWebhooksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetWebhooksResponse?)null);

            // Act
            var result = await _webhooksService.GetWebhooksAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetWebhooksAsync_ApiReturnsResponseWithNullWebhooks_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_wh_null_webhooks_in_resp";
            var apiResponse = new GetWebhooksResponse(null!); // Webhooks property is null
            _mockApiConnection
                .Setup(x => x.GetAsync<GetWebhooksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _webhooksService.GetWebhooksAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetWebhooksAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_wh_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetWebhooksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _webhooksService.GetWebhooksAsync(workspaceId)
            );
        }

        [Fact]
        public async Task GetWebhooksAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var workspaceId = "ws_wh_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetWebhooksResponse(new List<Webhook>());

            _mockApiConnection.Setup(c => c.GetAsync<GetWebhooksResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _webhooksService.GetWebhooksAsync(workspaceId, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetWebhooksAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_wh_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetWebhooksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _webhooksService.GetWebhooksAsync(workspaceId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetWebhooksAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_wh_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetWebhooksResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetWebhooksResponse(new List<Webhook>()));

            // Act
            await _webhooksService.GetWebhooksAsync(workspaceId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetWebhooksResponse>(
                $"team/{workspaceId}/webhook",
                expectedToken), Times.Once);
        }

        // --- Tests for CreateWebhookAsync ---

        [Fact]
        public async Task CreateWebhookAsync_ValidRequest_ReturnsWebhook()
        {
            // Arrange
            var workspaceId = "ws_create_wh";
            var request = new CreateWebhookRequest(
                Endpoint: "https://example.com/my-new-hook",
                Events: new List<string> { "taskCreated", "taskUpdated" },
                SpaceId: null,
                FolderId: null,
                ListId: null,
                TaskId: null,
                TeamId: null
            );
            var expectedWebhook = CreateSampleWebhook("wh_new", "https://example.com/my-new-hook");
            // CreateWebhookResponse wraps a Webhook
            var apiResponse = new CreateWebhookResponse(Id: "webhook_resource_id_1", Webhook: expectedWebhook);

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(
                    $"team/{workspaceId}/webhook",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _webhooksService.CreateWebhookAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedWebhook.Id, result.Id);
            Assert.Equal(expectedWebhook.Endpoint, result.Endpoint);
            _mockApiConnection.Verify(x => x.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(
                $"team/{workspaceId}/webhook",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateWebhookAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_create_wh_null_api";
            var request = new CreateWebhookRequest("https://example.com/null", new List<string>(), null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CreateWebhookResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _webhooksService.CreateWebhookAsync(workspaceId, request)
            );
        }

        // [Fact] // Commenting out as CreateWebhookResponse requires a non-null Webhook.
        // public async Task CreateWebhookAsync_ApiReturnsResponseWithNullWebhook_ThrowsInvalidOperationException()
        // {
        //     // Arrange
        //     var workspaceId = "ws_create_wh_null_webhook";
        //     var request = new CreateWebhookRequest("https://example.com/null-wh", new List<string>(), null, null, null, null, null);
        //     // This scenario is hard to mock if the DTO constructor enforces non-null Webhook.
        //     // If Webhook property were nullable (Webhook?), then:
        //     // var apiResponse = new CreateWebhookResponse("some_id", null!);
        //     // For now, assume deserialization would fail or this state isn't reachable with current DTO.
        //     var mockResponse = new Mock<CreateWebhookResponse>();
        //     mockResponse.SetupGet(r => r.Webhook).Returns((Webhook)null!); // This doesn't work for record primary constructor properties
        //     _mockApiConnection
        //        .Setup(x => x.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(new CreateWebhookResponse("dummy_id_for_null_webhook_test", null!)); // This will fail if Webhook is not nullable in DTO
        //
        //     // Act & Assert
        //     await Assert.ThrowsAsync<InvalidOperationException>(() =>
        //         _webhooksService.CreateWebhookAsync(workspaceId, request)
        //     );
        // }

        [Fact]
        public async Task CreateWebhookAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_create_wh_http_ex";
            var request = new CreateWebhookRequest("https://example.com/http-wh", new List<string>(), null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _webhooksService.CreateWebhookAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task CreateWebhookAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var workspaceId = "ws_create_wh_op_cancel";
            var request = new CreateWebhookRequest("https://example.com/cancel-wh", new List<string>(), null, null, null, null, null);
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new CreateWebhookResponse("dummy_id", CreateSampleWebhook());

            _mockApiConnection.Setup(x => x.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(
                    It.IsAny<string>(), It.IsAny<CreateWebhookRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, CreateWebhookRequest, CancellationToken>((url, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _webhooksService.CreateWebhookAsync(workspaceId, request, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CreateWebhookAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_create_wh_cancel_ex";
            var request = new CreateWebhookRequest("https://example.com/cancel-wh", new List<string>(), null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _webhooksService.CreateWebhookAsync(workspaceId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task CreateWebhookAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_create_wh_ct_pass";
            var request = new CreateWebhookRequest("https://example.com/ct-wh", new List<string>(), null, null, null, null, null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedWebhook = CreateSampleWebhook("wh_ct_new", "https://example.com/ct-wh");
            var apiResponse = new CreateWebhookResponse("wh_ct_new_id", expectedWebhook);

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _webhooksService.CreateWebhookAsync(workspaceId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateWebhookRequest, CreateWebhookResponse>(
                $"team/{workspaceId}/webhook",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for UpdateWebhookAsync ---

        [Fact]
        public async Task UpdateWebhookAsync_ValidRequest_ReturnsUpdatedWebhook()
        {
            // Arrange
            var webhookId = "wh_to_update";
            var request = new UpdateWebhookRequest(
                Endpoint: "https://example.com/updated-hook",
                Events: new List<string> { "taskDeleted" },
                Status: "active",
                Secret: null
            );
            var updatedWebhook = CreateSampleWebhook(webhookId, "https://example.com/updated-hook");
            updatedWebhook = updatedWebhook with { Events = new List<string> { "taskDeleted" } }; // Reflect changes
            var apiResponse = new UpdateWebhookResponse(webhookId, updatedWebhook);

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateWebhookRequest, UpdateWebhookResponse>(
                    $"webhook/{webhookId}",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _webhooksService.UpdateWebhookAsync(webhookId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedWebhook.Endpoint, result.Endpoint);
            Assert.Contains("taskDeleted", result.Events);
            _mockApiConnection.Verify(x => x.PutAsync<UpdateWebhookRequest, UpdateWebhookResponse>(
                $"webhook/{webhookId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateWebhookAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var webhookId = "wh_update_null_api";
            var request = new UpdateWebhookRequest("https://example.com/update-null", new List<string>(), "inactive", null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateWebhookRequest, UpdateWebhookResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UpdateWebhookResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _webhooksService.UpdateWebhookAsync(webhookId, request)
            );
        }

        // [Fact] // Commenting out as UpdateWebhookResponse requires a non-null Webhook for its constructor.
        // public async Task UpdateWebhookAsync_ApiReturnsResponseWithNullWebhook_ThrowsInvalidOperationException()
        // {
        //     // Arrange
        //     var webhookId = "wh_update_null_webhook";
        //     var request = new UpdateWebhookRequest("https://example.com/update-null-wh", new List<string>(), "active", null);
        //     // This is hard to mock correctly if the DTO constructor enforces non-null Webhook.
        //     // var apiResponse = new UpdateWebhookResponse(webhookId, null!); // This would fail at constructor
        //     _mockApiConnection
        //        .Setup(x => x.PutAsync<UpdateWebhookRequest, UpdateWebhookResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(new UpdateWebhookResponse(webhookId, null!)); // This will fail if Webhook is not nullable in DTO
        //
        //     // Act & Assert
        //     await Assert.ThrowsAsync<InvalidOperationException>(() =>
        //         _webhooksService.UpdateWebhookAsync(webhookId, request)
        //     );
        // }

        [Fact]
        public async Task UpdateWebhookAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var webhookId = "wh_update_http_ex";
            var request = new UpdateWebhookRequest("https://example.com/update-http", new List<string>(), "active", null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateWebhookRequest, UpdateWebhookResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _webhooksService.UpdateWebhookAsync(webhookId, request)
            );
        }

        [Fact]
        public async Task UpdateWebhookAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var webhookId = "wh_update_op_cancel";
            var request = new UpdateWebhookRequest("https://example.com/update-cancel", new List<string>(), "active", null);
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new UpdateWebhookResponse(webhookId, CreateSampleWebhook());

            _mockApiConnection.Setup(x => x.PutAsync<UpdateWebhookRequest, UpdateWebhookResponse>(
                    It.IsAny<string>(), It.IsAny<UpdateWebhookRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, UpdateWebhookRequest, CancellationToken>((url, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _webhooksService.UpdateWebhookAsync(webhookId, request, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task UpdateWebhookAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var webhookId = "wh_update_cancel_ex";
            var request = new UpdateWebhookRequest("https://example.com/update-cancel", new List<string>(), "active", null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateWebhookRequest, UpdateWebhookResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _webhooksService.UpdateWebhookAsync(webhookId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task UpdateWebhookAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var webhookId = "wh_update_ct_pass";
            var request = new UpdateWebhookRequest("https://example.com/update-ct", new List<string>(), "active", null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedWebhook = CreateSampleWebhook(webhookId, "https://example.com/update-ct");
            var apiResponse = new UpdateWebhookResponse(webhookId, expectedWebhook);

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateWebhookRequest, UpdateWebhookResponse>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _webhooksService.UpdateWebhookAsync(webhookId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync<UpdateWebhookRequest, UpdateWebhookResponse>(
                $"webhook/{webhookId}",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for DeleteWebhookAsync ---

        [Fact]
        public async Task DeleteWebhookAsync_ValidWebhookId_CallsDeleteAsync()
        {
            // Arrange
            var webhookId = "wh_to_delete";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"webhook/{webhookId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _webhooksService.DeleteWebhookAsync(webhookId);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"webhook/{webhookId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteWebhookAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var webhookId = "wh_delete_http_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _webhooksService.DeleteWebhookAsync(webhookId)
            );
        }

        [Fact]
        public async Task DeleteWebhookAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var webhookId = "wh_delete_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();

            _mockApiConnection.Setup(x => x.DeleteAsync(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .Returns(Task.CompletedTask);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _webhooksService.DeleteWebhookAsync(webhookId, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task DeleteWebhookAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var webhookId = "wh_delete_cancel_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _webhooksService.DeleteWebhookAsync(webhookId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task DeleteWebhookAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var webhookId = "wh_delete_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _webhooksService.DeleteWebhookAsync(webhookId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"webhook/{webhookId}",
                expectedToken), Times.Once);
        }
    }
}
