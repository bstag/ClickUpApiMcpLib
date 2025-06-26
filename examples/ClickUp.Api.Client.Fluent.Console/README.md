# ClickUp Fluent API Console Example

This example demonstrates how to use the new fluent API to interact with the ClickUp API.

## How to Run

1.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```

2.  **Update Placeholders:**
    Open the `Program.cs` file and replace the following placeholders with your actual data:
    *   `YOUR_API_TOKEN`: Your ClickUp API token.
    *   `YOUR_LIST_ID`: The ID of a list in your ClickUp workspace.
    *   `YOUR_WORKSPACE_ID`: The ID of your ClickUp workspace.
    *   `YOUR_TASK_ID`: The ID of a task in your ClickUp workspace.
    *   `PATH_TO_YOUR_FILE.txt`: The path to a local file you want to attach to a task.
    *   `YOUR_GUEST_ID`: The ID of a guest in your ClickUp workspace.
    *   `YOUR_FOLDER_ID`: The ID of a folder in your ClickUp workspace.
    *   `YOUR_CHECKLIST_ID`: The ID of a checklist in your ClickUp workspace.
    *   `YOUR_TASK_ID_1`: The ID of the first task for dependency example.
    *   `YOUR_TASK_ID_2`: The ID of the second task for dependency example.

3.  **Run the application:**
    ```bash
    dotnet run
    ```