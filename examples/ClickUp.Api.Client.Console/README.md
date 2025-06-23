# ClickUp.Api.Client.Console Example

This project demonstrates various uses of the `ClickUp.Api.Client` SDK in a .NET Console Application. It showcases common API interactions like fetching workspace data, managing tasks, and handling comments.

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
    Open `appsettings.json` and:
    *   Replace `"YOUR_CLICKUP_PERSONAL_ACCESS_TOKEN_HERE"` with your actual ClickUp Personal Access Token in the `ClickUpApiOptions` section.
    *   **Optional but Recommended:** To run all examples fully, replace placeholder values in the `ExampleSettings` section with actual IDs from your ClickUp account:
        *   `WorkspaceIdForExamples`: A Workspace (Team) ID. If left as default, the first workspace of the authorized user will be used.
        *   `SpaceIdForExamples`: A Space ID. If left as default, the first space in the target workspace will be used.
        *   `FolderIdForExamples`: A Folder ID. If left as default, the first folder in the target space will be used.
        *   `ListIdForExamples`: A List ID. This is **important** for task creation examples. If left as default, the application will attempt to find the first available list.
        *   `TaskIdForComments`: A specific Task ID to run comment examples against. If a new task is successfully created by the example, that new task's ID will be used for comment examples, overriding this setting.

    Example `appsettings.json` structure:
    ```json
    {
      "ClickUpApiOptions": {
        "PersonalAccessToken": "pkat_your_actual_token_value"
      },
      "ExampleSettings": {
        "WorkspaceIdForExamples": "123456",
        "SpaceIdForExamples": "789012",
        "FolderIdForExamples": "345678",
        "ListIdForExamples": "901234",
        "TaskIdForComments": "abc123xyz"
      },
      "Logging": {
        // ... logging settings ...
      }
    }
    ```
    **Important:** The `appsettings.json` file is included in the `.gitignore` file (standard practice) to prevent accidental commitment of your sensitive API token and specific IDs.

## How to Run

1.  Navigate to the `examples/ClickUp.Api.Client.Console` directory.
2.  Run the application using the .NET CLI:

    ```bash
    dotnet run
    ```

## Scenarios Demonstrated

This example showcases:
- **SDK Initialization:** Configuring and initializing the ClickUp API client via Dependency Injection.
- **Authentication:** Fetching the authorized user and their workspaces.
- **Workspace Navigation:**
    - Listing Workspaces (Teams) and their plans.
    - Listing Spaces within a Workspace.
    - Getting details for a specific Space.
    - Listing Folders within a Space.
    - Listing Lists within a Folder.
    - Listing Folderless Lists within a Space.
- **Task Management:**
    - Creating a new task in a specified (or auto-detected) list.
    - Retrieving details of the created task.
    - Updating the created task's name and description.
    - Listing tasks within a list.
- **Comment Management:**
    - Creating a comment on a task (either newly created or specified in settings).
    - Retrieving comments for that task.
- **Error Handling:** Basic try-catch blocks for `ClickUpApiException` and other exceptions.
- **Cleanup:** Attempting to delete the task created during the example run.

The console output will log the actions being performed and the results from the API. Check the output for details on each step.
