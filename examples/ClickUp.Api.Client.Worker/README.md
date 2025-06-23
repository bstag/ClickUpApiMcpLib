# ClickUp.Api.Client.Worker Example

This project demonstrates usage of the `ClickUp.Api.Client` SDK in a .NET Worker Service. This type of application is suitable for background tasks, such as periodically polling the ClickUp API for new or updated tasks.

## Prerequisites

- .NET 9 SDK (or later)
- A ClickUp Personal Access Token

## Configuration

This worker service is configured using `appsettings.json` (and `appsettings.Development.json`) or User Secrets. For sensitive information like the API token, User Secrets is recommended, especially during development.

1.  **Copy Configuration File:**
    Make a copy of `appsettings.template.json` and name it `appsettings.json`.
    ```bash
    cp appsettings.template.json appsettings.json
    ```
    You might also want to create `appsettings.Development.json` for development-specific overrides.

2.  **API Token (User Secrets - Recommended for Development):**
    Initialize User Secrets for the project (if not already done):
    ```bash
    dotnet user-secrets init --project .
    ```
    Set your ClickUp Personal Access Token:
    ```bash
    dotnet user-secrets set "ClickUpApiOptions:PersonalAccessToken" "pkat_YOUR_ACTUAL_TOKEN_VALUE" --project .
    ```
    Alternatively, you can place the `PersonalAccessToken` directly in `appsettings.json` or `appsettings.Development.json`, but this is less secure.

3.  **Worker Settings (`appsettings.json` or `appsettings.Development.json`):**
    Open your `appsettings.json` (or `appsettings.Development.json`) and configure the `WorkerExampleSettings`:
    *   `ListIdForPolling`: **Required.** Replace `"YOUR_LIST_ID_FOR_POLLING_TASKS"` with the ID of the ClickUp List you want the worker to monitor for new or updated tasks.
    *   `PollingIntervalSeconds`: Optional. The interval (in seconds) at which the worker will poll the API. Defaults to 60 seconds if not specified.

    Example `appsettings.json` structure:
    ```json
    {
      "ClickUpApiOptions": {
        // Token can be here if not using User Secrets, but not recommended for VCS
        "PersonalAccessToken": "YOUR_CLICKUP_PERSONAL_ACCESS_TOKEN_HERE_OR_IN_USER_SECRETS"
      },
      "WorkerExampleSettings": {
        "ListIdForPolling": "123456789", // <-- Replace with your actual List ID
        "PollingIntervalSeconds": 30
      },
      "Logging": {
        // ... logging settings ...
        // "ClickUp.Api.Client.Worker": "Debug" // For more verbose worker logs
      }
    }
    ```
    If `ClickUpApiOptions:PersonalAccessToken` is present in both User Secrets and an `appsettings.json` file, User Secrets will typically take precedence during development.

## How to Run

1.  Ensure you have configured your API token and `ListIdForPolling` as described above.
2.  Navigate to the `examples/ClickUp.Api.Client.Worker` directory.
3.  Run the application using the .NET CLI:

    ```bash
    dotnet run
    ```
    The worker will start, and you should see log messages in the console indicating its activity, including when it polls for tasks and if it finds any new or updated ones.

## Scenarios Demonstrated

This example showcases:
- **SDK Initialization:** Configuring and initializing the ClickUp API client via Dependency Injection in a Worker Service.
- **Configuration:** Using `IOptions<T>` to inject worker-specific settings (`ListIdForPolling`, `PollingIntervalSeconds`) from `appsettings.json`.
- **Periodic Polling:** The worker periodically queries a specified ClickUp List for tasks.
- **Change Detection (Basic):** It logs tasks that have been updated since the last poll by comparing their `DateUpdatedUnix` timestamp.
    *Note: This is a basic form of change detection. For robust production systems, more sophisticated state management or webhook usage might be preferred.*
- **`CancellationToken` Usage:** Demonstrates how the `CancellationToken` is passed to SDK methods and used to gracefully stop the worker.
- **Logging:** Utilizes Serilog for structured logging of worker activities.

The worker will continue to run and poll the API at the configured interval until the application is stopped (e.g., by pressing `Ctrl+C` in the console).
