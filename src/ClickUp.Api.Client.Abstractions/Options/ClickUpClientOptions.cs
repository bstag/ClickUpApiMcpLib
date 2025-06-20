namespace ClickUp.Api.Client.Abstractions.Options
{
    /// <summary>
    /// Options for configuring the ClickUp API Client.
    /// </summary>
    public class ClickUpClientOptions
    {
        /// <summary>
        /// Gets or sets the Personal Access Token for ClickUp API authentication.
        /// </summary>
        public string? PersonalAccessToken { get; set; }

        /// <summary>
        /// Gets or sets the base address for the ClickUp API.
        /// Defaults to "https://api.clickup.com/api/v2/".
        /// </summary>
        public string BaseAddress { get; set; } = "https://api.clickup.com/api/v2/";

        // OAuth specific properties can be added later:
        // public string? OAuthClientId { get; set; }
        // public string? OAuthClientSecret { get; set; }
        // public string? OAuthRedirectUri { get; set; } // Optional, if needed for URL generation helpers
    }
}
