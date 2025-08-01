using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ClickUp.Api.Client.CLI.Models;

namespace ClickUp.Api.Client.CLI.Infrastructure;

/// <summary>
/// Implementation of configuration validator
/// </summary>
public class ConfigurationValidator : IConfigurationValidator
{
    private readonly IConfiguration _configuration;
    private readonly IAuthorizationService _authService;
    private readonly ILogger<ConfigurationValidator> _logger;
    private readonly CliOptions _cliOptions;

    public ConfigurationValidator(
        IConfiguration configuration,
        IAuthorizationService authService,
        ILogger<ConfigurationValidator> logger,
        IOptions<CliOptions> cliOptions)
    {
        _configuration = configuration;
        _authService = authService;
        _logger = logger;
        _cliOptions = cliOptions.Value;
    }

    public async Task<ValidationResult> ValidateAsync()
    {
        var result = new ValidationResult();

        try
        {
            // Validate basic configuration
            ValidateBasicConfiguration(result);

            // If basic validation fails, don't proceed with API validation
            if (!result.IsValid)
            {
                return result;
            }

            // Validate API connectivity
            await ValidateApiConnectivityAsync(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during configuration validation");
            result.Errors.Add($"Unexpected validation error: {ex.Message}");
            result.IsValid = false;
        }

        return result;
    }

    private void ValidateBasicConfiguration(ValidationResult result)
    {
        // Check if ClickUp API options are configured
        var apiSection = _configuration.GetSection("ClickUpApiOptions");
        if (!apiSection.Exists())
        {
            result.Errors.Add("ClickUpApiOptions section is missing from configuration");
            result.IsValid = false;
            return;
        }

        // Check personal access token
        var personalAccessToken = apiSection["PersonalAccessToken"];
        if (string.IsNullOrWhiteSpace(personalAccessToken))
        {
            result.Errors.Add("PersonalAccessToken is required in ClickUpApiOptions");
            result.IsValid = false;
        }
        else if (personalAccessToken == "your_personal_access_token_here")
        {
            result.Errors.Add("PersonalAccessToken appears to be a placeholder value. Please set your actual ClickUp API token.");
            result.IsValid = false;
        }
        else
        {
            // Basic token format validation
            if (personalAccessToken.Length < 10)
            {
                result.Warnings.Add("PersonalAccessToken appears to be too short. Please verify it's correct.");
            }
            
            result.Metadata["TokenLength"] = personalAccessToken.Length;
        }

        // Check base URL
        var baseUrl = apiSection["BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            result.Warnings.Add("BaseUrl is not configured, using default ClickUp API URL");
        }
        else if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
        {
            result.Errors.Add($"BaseUrl '{baseUrl}' is not a valid URL");
            result.IsValid = false;
        }
        else
        {
            result.Metadata["BaseUrl"] = baseUrl;
        }

        // Validate CLI options
        ValidateCliOptions(result);

        // If no errors so far, mark as valid
        if (result.Errors.Count == 0)
        {
            result.IsValid = true;
        }
    }

    private void ValidateCliOptions(ValidationResult result)
    {
        // Validate page size settings
        if (_cliOptions.DefaultPageSize <= 0)
        {
            result.Errors.Add("DefaultPageSize must be greater than 0");
        }
        else if (_cliOptions.DefaultPageSize > _cliOptions.MaxPageSize)
        {
            result.Errors.Add("DefaultPageSize cannot be greater than MaxPageSize");
        }

        if (_cliOptions.MaxPageSize <= 0)
        {
            result.Errors.Add("MaxPageSize must be greater than 0");
        }
        else if (_cliOptions.MaxPageSize > 1000)
        {
            result.Warnings.Add("MaxPageSize is very large (>1000), this may cause performance issues");
        }

        // Validate timeout settings
        if (_cliOptions.RequestTimeoutSeconds <= 0)
        {
            result.Errors.Add("RequestTimeoutSeconds must be greater than 0");
        }
        else if (_cliOptions.RequestTimeoutSeconds > 300)
        {
            result.Warnings.Add("RequestTimeoutSeconds is very large (>5 minutes), this may cause poor user experience");
        }

        // Validate retry settings
        if (_cliOptions.RetryAttempts < 0)
        {
            result.Errors.Add("RetryAttempts cannot be negative");
        }
        else if (_cliOptions.RetryAttempts > 10)
        {
            result.Warnings.Add("RetryAttempts is very high (>10), this may cause long delays");
        }

        if (_cliOptions.RetryDelayMs < 0)
        {
            result.Errors.Add("RetryDelayMs cannot be negative");
        }

        // Validate output format
        var validFormats = new[] { "table", "json", "csv", "properties" };
        if (!validFormats.Contains(_cliOptions.DefaultFormat.ToLowerInvariant()))
        {
            result.Errors.Add($"DefaultFormat '{_cliOptions.DefaultFormat}' is not valid. Valid formats: {string.Join(", ", validFormats)}");
        }

        // Validate cache settings
        if (_cliOptions.EnableCaching && _cliOptions.CacheDurationMinutes <= 0)
        {
            result.Errors.Add("CacheDurationMinutes must be greater than 0 when caching is enabled");
        }
    }

    private async Task ValidateApiConnectivityAsync(ValidationResult result)
    {
        try
        {
            _logger.LogDebug("Testing API connectivity...");
            
            // Try to get user information to validate token and connectivity
            var user = await _authService.GetAuthorizedUserAsync();
            
            if (user != null)
            {
                result.Metadata["UserId"] = user.Id;
                result.Metadata["Username"] = user.Username;
                result.Metadata["UserEmail"] = user.Email;
                
                _logger.LogDebug("API connectivity test successful. User: {Username} ({Email})", user.Username, user.Email);
            }
            else
            {
                result.Warnings.Add("API connectivity test returned null user information");
            }

            // Try to get workspaces to further validate permissions
            try
            {
                var workspaces = await _authService.GetAuthorizedWorkspacesAsync();
                var workspaceCount = workspaces?.Count() ?? 0;
                
                result.Metadata["WorkspaceCount"] = workspaceCount;
                
                if (workspaceCount == 0)
                {
                    result.Warnings.Add("No workspaces found. The API token may have limited permissions.");
                }
                else
                {
                    _logger.LogDebug("Found {WorkspaceCount} accessible workspaces", workspaceCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not retrieve workspaces during validation");
                result.Warnings.Add($"Could not retrieve workspaces: {ex.Message}");
            }
        }
        catch (ClickUpApiException apiEx)
        {
            _logger.LogError(apiEx, "ClickUp API error during validation");
            
            result.IsValid = false;
            
            switch (apiEx.StatusCode)
            {
                case System.Net.HttpStatusCode.Unauthorized:
                    result.Errors.Add("API token is invalid or expired. Please check your PersonalAccessToken in the configuration.");
                    break;
                case System.Net.HttpStatusCode.Forbidden:
                    result.Errors.Add("API token does not have sufficient permissions. Please check your token permissions in ClickUp.");
                    break;
                case System.Net.HttpStatusCode.TooManyRequests:
                    result.Errors.Add("API rate limit exceeded. Please try again later.");
                    break;
                default:
                    result.Errors.Add($"API error ({apiEx.StatusCode}): {apiEx.Message}");
                    break;
            }
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP error during API validation");
            result.IsValid = false;
            result.Errors.Add($"Network connectivity error: {httpEx.Message}. Please check your internet connection and API base URL.");
        }
        catch (TaskCanceledException timeoutEx)
        {
            _logger.LogError(timeoutEx, "Timeout during API validation");
            result.IsValid = false;
            result.Errors.Add("API request timed out. Please check your network connection or increase the timeout setting.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during API validation");
            result.IsValid = false;
            result.Errors.Add($"Unexpected API validation error: {ex.Message}");
        }
    }
}