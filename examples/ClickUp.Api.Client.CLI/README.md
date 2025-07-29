# ClickUp API CLI Tool

A comprehensive command-line interface for the ClickUp API SDK that provides access to all GET operations across ClickUp modules.

## Features

- **Complete API Coverage**: Access to all ClickUp API GET endpoints through 26 organized command modules
- **Modular Command Structure**: Well-organized commands separated into logical modules for better maintainability
- **Multiple Output Formats**: Table, JSON, CSV, and properties formats
- **Flexible Filtering**: Filter results by date ranges, status, assignees, and more
- **Pagination Support**: Handle large datasets with built-in pagination
- **Error Handling**: Comprehensive error handling with detailed messages
- **Configuration Management**: Easy setup and validation of API credentials
- **Colored Output**: Enhanced readability with colored console output
- **Verbose Logging**: Detailed logging for troubleshooting
- **Extensible Architecture**: Clean separation of concerns with individual command files

## Installation

1. Clone the repository and navigate to the CLI project:
   ```bash
   cd examples/ClickUp.Api.Client.CLI
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Initialize configuration:
   ```bash
   dotnet run -- config init
   ```

4. Set your ClickUp API token using one of these methods:

   **Method 1: Using config commands (Recommended)**
   ```bash
   dotnet run -- config set token YOUR_CLICKUP_API_TOKEN
   ```

   **Method 2: Manual configuration file**
   Edit the `appsettings.json` file and add your ClickUp API token:
   ```json
   {
     "ClickUpApiOptions": {
       "PersonalAccessToken": "your_actual_api_token_here"
     }
   }
   ```

## Configuration

### Getting Your ClickUp API Token

1. Go to your ClickUp settings
2. Navigate to "Apps" section
3. Generate a Personal Access Token
4. Copy the token to your `appsettings.json` file

### Configuration Validation

Validate your configuration:
```bash
dotnet run -- config validate
```

Check API connectivity:
```bash
dotnet run -- health check
```

## Usage

### Basic Command Structure

```bash
dotnet run -- [command] [subcommand] [arguments] [options]
```

### Global Options

- `--format, -f`: Output format (table, json, csv, properties)
- `--verbose, -v`: Enable verbose output
- `--config, -c`: Path to configuration file
- `--debug, -d`: Enable HTTP request/response debugging

### Available Commands

The CLI provides comprehensive access to all ClickUp API operations through well-organized command modules:

#### Authentication & User Management

```bash
# Authentication and user information
dotnet run -- auth user get
dotnet run -- auth workspaces list

# User management
dotnet run -- user get [user-id]
dotnet run -- user list [workspace-id]

# User groups
dotnet run -- user-group list [workspace-id]
dotnet run -- user-group get [group-id]
```

> **Note**: In ClickUp API v2, "workspaces" and "teams" refer to the same entity. The CLI uses "workspaces" terminology for consistency.

#### Workspace & Organization

```bash
# Workspace operations
dotnet run -- workspace seat-usage [workspace-id]
dotnet run -- workspace plan [workspace-id]

# Member management
dotnet run -- member list [workspace-id]
dotnet run -- member get [member-id]

# Guest management
dotnet run -- guest list [workspace-id]
dotnet run -- guest get [guest-id]

# Role management
dotnet run -- role list [workspace-id]
dotnet run -- role get [role-id]
```

#### Project Structure

```bash
# Space operations
dotnet run -- space get [space-id]
dotnet run -- space list [workspace-id] --include-archived

# Folder operations
dotnet run -- folder get [folder-id]
dotnet run -- folder list [space-id] --include-archived

# List operations
dotnet run -- list get [list-id]
dotnet run -- list list [folder-id] --include-archived

# View management
dotnet run -- view list-space [space-id]
dotnet run -- view list-folder [folder-id]
dotnet run -- view list-list [list-id]
dotnet run -- view get [view-id]
```

#### Task Management

```bash
# Core task operations
dotnet run -- task get [task-id] --include-subtasks
dotnet run -- task list [list-id] --page-size 50 --statuses "Open,In Progress"

# Task checklists
dotnet run -- task-checklist list [task-id]
dotnet run -- task-checklist get [checklist-id]

# Task relationships
dotnet run -- task-relationship list [task-id]
dotnet run -- task-relationship get [relationship-id]

# Time tracking
dotnet run -- time-tracking get [entry-id]
dotnet run -- time-tracking list [workspace-id]
dotnet run -- time-tracking start [task-id]
dotnet run -- time-tracking stop [entry-id]

# Task time in status
dotnet run -- task time-in-status [task-id]
dotnet run -- task bulk-time-in-status [task-id1,task-id2,task-id3]
```

#### Communication & Collaboration

```bash
# Comments
dotnet run -- comment list [task-id]
dotnet run -- comment get [comment-id]

# Chat messages
dotnet run -- chat list [view-id]
dotnet run -- chat get [message-id]

# Attachments
dotnet run -- attachment list [task-id]
dotnet run -- attachment get [attachment-id]
```

#### Advanced Features

```bash
# Custom fields
dotnet run -- custom-field list [list-id]
dotnet run -- custom-field get [field-id]

# Tags
dotnet run -- tag list [space-id]
dotnet run -- tag get [tag-id]

# Goals
dotnet run -- goal list [workspace-id]
dotnet run -- goal get [goal-id]

# Templates
dotnet run -- template list [workspace-id]
dotnet run -- template get [template-id]

# Documents
dotnet run -- docs list [workspace-id]
dotnet run -- docs search [workspace-id] --query "search term"

# Webhooks
dotnet run -- webhook list [workspace-id]
dotnet run -- webhook get [webhook-id]

# Shared hierarchy
dotnet run -- shared-hierarchy list [workspace-id]
dotnet run -- shared-hierarchy get [shared-id]
```

### Advanced Filtering

#### Date Range Filtering

```bash
# Filter tasks by date range
dotnet run -- task list [list-id] --from 2024-01-01 --to 2024-12-31
```

#### Status and Assignee Filtering

```bash
# Filter by specific statuses
dotnet run -- task list [list-id] --statuses "Open,In Progress,Review"

# Filter by assignees
dotnet run -- task list [list-id] --assignees 123,456,789

# Filter by tags
dotnet run -- task list [list-id] --tags "urgent,bug,feature"
```

#### Pagination

```bash
# Get specific page with custom page size
dotnet run -- task list [list-id] --page 2 --page-size 25
```

### Output Formatting

#### Different Output Formats

```bash
# Table format (default)
dotnet run -- auth user get --format table

# JSON format
dotnet run -- auth user get --format json

# CSV format
dotnet run -- task list [list-id] --format csv

# Properties format (for single objects)
dotnet run -- space get [space-id] --format properties
```

#### Selecting Specific Properties

```bash
# Show only specific properties
dotnet run -- task list [list-id] --properties "id,name,status,assignees"
```

### Examples

#### Complete Workflow Example

ClickUp data follows a hierarchical structure: **Workspace → Space → Folder → List → Task**

```bash
# 1. Check configuration and connectivity
dotnet run -- config validate

# 2. Get your workspaces (teams)
dotnet run -- auth workspaces list
# Note the workspace ID from the output

# 3. Get spaces in a workspace
dotnet run -- space list 12345
# Note the space ID from the output

# 4. Get folders in a space
dotnet run -- folder list 67890
# Note the folder ID from the output

# 5. Get lists in a folder
dotnet run -- list list-all 11111
# Note the list ID from the output

# 6. Get tasks in a list
dotnet run -- task list 22222 --format json --page-size 10

# 7. Get specific task details
dotnet run -- task get abc123 --include-subtasks
```

#### Understanding the Data Hierarchy

1. **Workspaces/Teams**: Top-level containers for your organization
2. **Spaces**: Major project areas within a workspace
3. **Folders**: Organizational containers within spaces (optional)
4. **Lists**: Task containers within folders or spaces
5. **Tasks**: Individual work items within lists

Each level requires the ID from the parent level to access its children.

#### Export Data Example

```bash
# Export tasks to CSV
dotnet run -- task list [list-id] --format csv > tasks.csv

# Export user info to JSON
dotnet run -- auth user get --format json > user.json
```

## Error Handling

The CLI provides detailed error messages for common issues:

- **Authentication Errors**: Invalid or expired API tokens
- **Permission Errors**: Insufficient permissions for requested operations
- **Not Found Errors**: Invalid IDs or inaccessible resources
- **Rate Limiting**: Automatic retry with exponential backoff
- **Network Errors**: Connection and timeout issues

## Troubleshooting

### Common Issues

1. **"Authentication failed"**
   - Verify your API token: `dotnet run -- config get token`
   - Set your token: `dotnet run -- config set token YOUR_TOKEN`
   - Check token permissions in ClickUp settings
   - Validate configuration: `dotnet run -- config validate`

2. **"Resource not found"**
   - Verify the ID is correct
   - Ensure you have access to the resource
   - Check the hierarchical path (workspace → space → folder → list)

3. **"Rate limit exceeded"**
   - Wait a few minutes and try again
   - Reduce the frequency of requests

### Debug Output

Enable HTTP request/response debugging to see API calls:
```bash
dotnet run -- [command] --debug
```

Enable verbose output for detailed application logging:
```bash
dotnet run -- [command] --verbose
```

### Configuration Commands

```bash
# Initialize configuration
dotnet run -- config init

# Set API token
dotnet run -- config set token YOUR_TOKEN

# View current token (masked)
dotnet run -- config get token

# List all configuration
dotnet run -- config list

# Validate configuration and test connectivity
dotnet run -- config validate

# Reset configuration
dotnet run -- config reset
```

### Logging

Logs are written to:
- Console (configurable level)
- File: `logs/clickup-cli-.log` (daily rolling)

## Configuration Reference

### appsettings.json Structure

```json
{
  "ClickUpApiOptions": {
    "PersonalAccessToken": "your_token_here",
    "BaseUrl": "https://api.clickup.com/api/v2"
  },
  "CLI": {
    "DefaultFormat": "table",
    "DefaultPageSize": 25,
    "MaxPageSize": 100,
    "EnableColors": true,
    "VerboseMode": false,
    "RequestTimeoutSeconds": 30,
    "RetryAttempts": 3,
    "RetryDelayMs": 1000,
    "EnableAutoPaging": true,
    "ShowProgress": true,
    "EnableCaching": false,
    "CacheDurationMinutes": 5
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/clickup-cli-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

## Development

### Command Architecture

The CLI uses a modular command architecture with 26 separate command modules, each focusing on a specific area of the ClickUp API:

- **Authentication**: `AuthCommands.cs` - User authentication and workspace access
- **Project Structure**: `SpaceCommands.cs`, `FolderCommands.cs`, `ListCommands.cs`, `ViewCommands.cs`
- **Task Management**: `TaskCommands.cs`, `TaskChecklistCommands.cs`, `TaskRelationshipCommands.cs`
- **User Management**: `UserCommands.cs`, `MemberCommands.cs`, `GuestCommands.cs`, `RoleCommands.cs`
- **Communication**: `CommentCommands.cs`, `ChatCommands.cs`, `AttachmentCommands.cs`
- **Advanced Features**: `CustomFieldCommands.cs`, `TagCommands.cs`, `GoalCommands.cs`, `TemplateCommands.cs`
- **Integration**: `WebhookCommands.cs`, `SharedHierarchyCommands.cs`, `DocsCommands.cs`
- **Time Tracking**: `TimeTrackingCommands.cs`
- **Workspace**: `WorkspaceCommands.cs`, `UserGroupCommands.cs`

### Project Structure

```
ClickUp.Api.Client.CLI/
├── Commands/           # 26 modular command implementations
│   ├── AuthCommands.cs
│   ├── TaskCommands.cs
│   ├── UserCommands.cs
│   └── ... (23 more modules)
├── Infrastructure/     # Core services and utilities
├── Models/            # Configuration and data models
├── Program.cs         # Application entry point
├── appsettings.json   # Configuration file
└── README.md          # This file
```

### Adding New Commands

1. Create a new command class inheriting from `BaseCommand`
2. Implement the `CreateCommand()` method
3. Register the command in `Program.cs`
4. Add the service dependency if needed
5. Follow the established naming conventions and module organization

### Recent Improvements

- **Modular Refactoring**: Separated all commands from a single large file into 26 focused modules
- **Improved Maintainability**: Each command module handles a specific domain area
- **Better Navigation**: Developers can quickly find and modify specific functionality
- **Enhanced Readability**: Smaller, focused files are easier to understand and maintain

### Building and Running

```bash
# Development build
dotnet build

# Run with arguments
dotnet run -- [arguments]

# Publish for distribution
dotnet publish -c Release -o publish
```

## License

This project is part of the ClickUp API SDK and follows the same