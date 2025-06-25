// Copyright (c) AI General. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace ClickUp.Api.Client.Abstractions.Options;

/// <summary>
/// Options for configuring the ClickUp API client.
/// </summary>
public class ClickUpClientOptions
{
    /// <summary>
    /// Gets or sets the ClickUp Personal Access Token.
    /// This is used if <see cref="OAuthAccessToken"/> is not provided.
    /// </summary>
    public string? PersonalAccessToken { get; set; }

    /// <summary>
    /// Gets or sets the ClickUp OAuth 2.0 Access Token.
    /// If provided, this token will be used for authorization, taking precedence over <see cref="PersonalAccessToken"/>.
    /// The SDK does not handle the OAuth flow to obtain this token; it must be acquired externally.
    /// </summary>
    public string? OAuthAccessToken { get; set; }

    /// <summary>
    /// Gets or sets the base address for the ClickUp API.
    /// Defaults to "https://api.clickup.com/api/v2/".
    /// </summary>
    public string BaseAddress { get; set; } = "https://api.clickup.com/api/v2/";

    /// <summary>
    /// Gets or sets the Workspace ID (Team ID) to be used in integration tests.
    /// This is often required as part of the API URL path.
    /// </summary>
    public string? TestWorkspaceId { get; set; }

    // Other OAuth properties like ClientId, ClientSecret, RefreshToken, TokenExpiresAt
    // are not included here as this library focuses on consuming a pre-acquired access token.
    // Handling the OAuth flow itself or token refresh mechanisms are outside the current scope.
}
