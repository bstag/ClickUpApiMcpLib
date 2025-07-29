using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Options;
using ClickUp.Api.Client.Http.Handlers;
using Microsoft.Extensions.Options;
using Xunit;

namespace ClickUp.Api.Client.Tests.Http;

public class AuthenticationHeaderTest
{
    [Fact]
    public async Task PersonalAccessToken_ShouldNotIncludeBearerPrefix()
    {
        // Arrange
        var testToken = "pk_test_token_123";
        var options = Options.Create(new ClickUpClientOptions
        {
            PersonalAccessToken = testToken
        });
        
        var handler = new AuthenticationDelegatingHandler(options)
        {
            InnerHandler = new TestHttpMessageHandler()
        };
        
        var httpClient = new HttpClient(handler);
        
        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");
        await httpClient.SendAsync(request);
        
        // Assert
        Assert.NotNull(request.Headers.Authorization);
        // For PAT, the entire token becomes the scheme when using Parse()
        Assert.Equal(testToken, request.Headers.Authorization.Scheme);
        Assert.Null(request.Headers.Authorization.Parameter); // Should be null for PAT
    }
    
    [Fact]
    public async Task OAuthAccessToken_ShouldIncludeBearerPrefix()
    {
        // Arrange
        var testToken = "oauth_test_token_123";
        var options = Options.Create(new ClickUpClientOptions
        {
            OAuthAccessToken = testToken
        });
        
        var handler = new AuthenticationDelegatingHandler(options)
        {
            InnerHandler = new TestHttpMessageHandler()
        };
        
        var httpClient = new HttpClient(handler);
        
        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");
        await httpClient.SendAsync(request);
        
        // Assert
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("Bearer", request.Headers.Authorization.Scheme);
        Assert.Equal(testToken, request.Headers.Authorization.Parameter);
    }
    
    private class TestHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
        }
    }
}