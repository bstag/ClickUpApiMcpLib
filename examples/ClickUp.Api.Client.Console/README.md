# ClickUp.Api.Client.Console Example

This project demonstrates basic usage of the `ClickUp.Api.Client` SDK in a .NET Console Application.

## Prerequisites

- .NET 9 SDK (or later)
- A ClickUp Personal Access Token

## Configuration

1.  **Copy Configuration File:**
    Make a copy of `appsettings.template.json` and name it `appsettings.json`.

    ```bash
    cp appsettings.template.json appsettings.json
    ```

2.  **Edit `appsettings.json`:**
    Open `appsettings.json` and replace `"YOUR_CLICKUP_PERSONAL_ACCESS_TOKEN_HERE"` with your actual ClickUp Personal Access Token.

    ```json
    {
      "ClickUpApiOptions": {
        "PersonalAccessToken": "your_actual_token_value"
      },
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.Hosting.Lifetime": "Information"
        }
      }
    }
    ```
    **Important:** The `appsettings.json` file is included in the `.gitignore` file (standard practice) to prevent accidental commitment of your sensitive API token.

## How to Run

1.  Navigate to the `examples/ClickUp.Api.Client.Console` directory.
2.  Run the application using the .NET CLI:

    ```bash
    dotnet run
    ```

## Scenarios Demonstrated

This example will showcase:
- SDK Initialization and Authentication.
- Fetching the authorized user.
- (More scenarios to be added)

(This README will be updated as more features are added to the example.)
