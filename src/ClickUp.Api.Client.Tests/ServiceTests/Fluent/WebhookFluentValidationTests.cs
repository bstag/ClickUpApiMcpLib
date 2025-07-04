using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Webhooks;
using ClickUp.Api.Client.Models.RequestModels.Webhooks;
using System.Collections.Generic; // Required for List
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class WebhookFluentValidationTests
{
    // --- WebhookFluentCreateRequest Tests ---

    [Fact]
    public void Create_Validate_MissingWorkspaceId_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentCreateRequest(string.Empty, webhooksServiceMock.Object) // Changed null to string.Empty
            .WithEndpoint("https://example.com/hook")
            .WithEvents(new List<string> { "taskCreated" });
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("WorkspaceId (TeamId for webhook registration) is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_MissingEndpoint_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentCreateRequest("ws123", webhooksServiceMock.Object)
            .WithEvents(new List<string> { "taskCreated" });
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Endpoint URL is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_InvalidEndpointUrl_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentCreateRequest("ws123", webhooksServiceMock.Object)
            .WithEndpoint("not-a-url")
            .WithEvents(new List<string> { "taskCreated" });
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Endpoint URL must be a valid absolute URI.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_MissingEvents_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentCreateRequest("ws123", webhooksServiceMock.Object)
            .WithEndpoint("https://example.com/hook");
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("At least one event must be specified.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_WildcardWithOtherEvents_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentCreateRequest("ws123", webhooksServiceMock.Object)
            .WithEndpoint("https://example.com/hook")
            .WithEvents(new List<string> { "*", "taskCreated" });
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("If '*' (all events) is specified, no other events should be listed.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_ValidRequest_DoesNotThrow()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentCreateRequest("ws123", webhooksServiceMock.Object)
            .WithEndpoint("https://example.com/hook")
            .WithEvents(new List<string> { "taskCreated" });
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentCreateRequest(string.Empty, webhooksServiceMock.Object); // Invalid, changed null to string.Empty for workspaceId
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync());
        Assert.Contains("WorkspaceId (TeamId for webhook registration) is required.", ex.ValidationErrors);
        Assert.Contains("Endpoint URL is required.", ex.ValidationErrors);
        Assert.Contains("At least one event must be specified.", ex.ValidationErrors);
    }

    // --- WebhookFluentUpdateRequest Tests ---

    [Fact]
    public void Update_Validate_MissingWebhookId_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentUpdateRequest(string.Empty, webhooksServiceMock.Object) // Changed null to string.Empty
            .WithStatus("active");
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("WebhookId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_NoFieldsSet_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentUpdateRequest("hook123", webhooksServiceMock.Object); // No fields set
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("At least one property (Endpoint, Events, Status, or Secret) must be set for updating a Webhook.", ex.ValidationErrors); // Updated message
    }

    [Fact]
    public void Update_Validate_InvalidEndpointUrl_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentUpdateRequest("hook123", webhooksServiceMock.Object)
            .WithEndpoint("not-a-url");
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("If Endpoint URL is provided, it must be a valid absolute URI.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_WildcardWithOtherEvents_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentUpdateRequest("hook123", webhooksServiceMock.Object)
            .WithEvents(new List<string> { "*", "taskCreated" });
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("If '*' (all events) is specified, no other events should be listed.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_InvalidStatus_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentUpdateRequest("hook123", webhooksServiceMock.Object)
            .WithStatus("pending"); // Invalid status
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("Status must be 'active' or 'disabled' if provided.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_ValidRequest_WithStatus_DoesNotThrow()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentUpdateRequest("hook123", webhooksServiceMock.Object)
            .WithStatus("active");
        request.Validate(); // Should not throw
    }

    [Fact]
    public void Update_Validate_ValidRequest_WithSecretOnly_DoesNotThrow()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentUpdateRequest("hook123", webhooksServiceMock.Object)
            .WithSecret("mysecret"); // Updating only secret is allowed
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task UpdateAsync_InvalidRequest_ThrowsException()
    {
        var webhooksServiceMock = new Mock<IWebhooksService>();
        var request = new WebhookFluentUpdateRequest(string.Empty, webhooksServiceMock.Object); // Invalid, changed null to string.Empty for webhookId
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.UpdateAsync());
        Assert.Contains("WebhookId is required.", ex.ValidationErrors);
        Assert.Contains("At least one property (Endpoint, Events, Status, or Secret) must be set for updating a Webhook.", ex.ValidationErrors); // Updated message
    }
}
