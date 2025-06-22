# ClickUp.Api.Client.Worker Example

This project demonstrates usage of the `ClickUp.Api.Client` SDK in a .NET Worker Service. This type of application is suitable for background tasks, such as periodically polling the ClickUp API.

## Prerequisites

- .NET 9 SDK (or later)
- A ClickUp Personal Access Token

## Configuration

This worker service can be configured using `appsettings.json` and User Secrets. For sensitive information like the API token, User Secrets is recommended.

1.  **API Token (Recommended: User Secrets):**
    Initialize User Secrets for the project (if not already done):
    ```bash
    dotnet user-secrets init --project .
    ```
    Set your ClickUp Personal Access Token:
    ```bash
    dotnet user-secrets set "ClickUpApiOptions:PersonalAccessToken" "YOUR_ACTUAL_TOKEN_VALUE" --project .
    ```

2.  **Application Settings (`appsettings.json`):**
    You can copy `appsettings.template.json` to `appsettings.json` to customize other settings like logging levels or worker-specific configurations.
    ```bash
    cp appsettings.template.json appsettings.json
    ```
    The `appsettings.json` would look like this:
    ```json
    {
      "ClickUpApiOptions": {
        // Token can be here, but User Secrets is preferred for security
        "PersonalAccessToken": "YOUR_CLICKUP_PERSONAL_ACCESS_TOKEN_HERE_OR_IN_USER_SECRETS"
      },
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.Hosting.Lifetime": "Information"
        }
      },
      "WorkerSettings": {
        "PollingIntervalSeconds": 60
      }
    }
    ```
    If `ClickUpApiOptions:PersonalAccessToken` is present in both User Secrets and `appsettings.json`, User Secrets will take precedence by default in development.

## How to Run

1.  Navigate to the `examples/ClickUp.Api.Client.Worker` directory.
2.  Run the application using the .NET CLI:

    ```bash
    dotnet run
    ```

## Scenarios Demonstrated

This example will be developed to showcase:
- SDK Initialization and Configuration in a Worker Service.
- Periodic polling of the ClickUp API (e.g., checking for new tasks).
- (More scenarios to be added)

(This README will be updated as more features are added to the example.)
