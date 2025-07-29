# ClickUp API CLI Tool

A comprehensive command-line interface for the ClickUp API SDK that provides access to all GET operations across ClickUp modules.

## Features

- **Complete API Coverage**: Access to all ClickUp API GET endpoints
- **Multiple Output Formats**: Table, JSON, CSV, and properties formats
- **Flexible Filtering**: Filter results by date ranges, status, assignees, and more
- **Pagination Support**: Handle large datasets with built-in pagination
- **Error Handling**: Comprehensive error handling with detailed messages
- **Configuration Management**: Easy setup and validation of API credentials
- **Colored Output**: Enhanced readability with colored console output
- **Verbose Logging**: Detailed logging for troubleshooting

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

4. Edit the `appsettings.json` file and add your ClickUp API token:
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

### Available Commands

#### Authentication & User Info

```bash
# Get current user information
dotnet run -- auth user get

# List authorized workspaces
dotnet run -- auth workspaces list
```

#### Workspace Operations

```bash
# Get workspace seat usage
dotnet run -- workspace seat-usage [workspace-id]

# Get workspace plan information
dotnet run -- workspace plan [workspace-id]
```

#### Space Operations

```bash
# Get specific space
dotnet run -- space get [space-id]

# List spaces in workspace
dotnet run -- space list [workspace-id] --include-archived
```

#### Folder Operations

```bash
# Get specific folder
dotnet run -- folder get [folder-id]

# List folders in space
dotnet run -- folder list [space-id] --include-archived
```

#### List Operations

```bash
# Get specific list
dotnet run -- list get [list-id]

# List all lists in folder
dotnet run -- list list-all [folder-id] --include-archived
```

#### Task Operations

```bash
# Get specific task
dotnet run -- task get [task-id] --include-subtasks

# List tasks in a list
dotnet run -- task list [list-id] --page-size 50 --statuses "Open,In Progress"

# Get time tracking for task
dotnet run -- task time-in-status [task-id]

# Get bulk time tracking
dotnet run -- task bulk-time-in-status [task-id1,task-id2,task-id3]
```

#### Comment Operations

```bash
# Get comments for a task
dotnet run -- comment list [task-id]
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

```bash
# 1. Check configuration
dotnet run -- health check

# 2. Get your workspaces
dotnet run -- auth workspaces list

# 3. Get spaces in a workspace
dotnet run -- space list 12345

# 4. Get folders in a space
dotnet run -- folder list 67890

# 5. Get lists in a folder
dotnet run -- list list-all 11111

# 6. Get tasks in a list
dotnet run -- task list 22222 --format json --page-size 10

# 7. Get specific task details
dotnet run -- task get abc123 --include-subtasks
```

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
   - Verify your API token in `appsettings.json`
   - Check token permissions in ClickUp settings

2. **"Resource not found"**
   - Verify the ID is correct
   - Ensure you have access to the resource

3. **"Rate limit exceeded"**
   - Wait a few minutes and try again
   - Reduce the frequency of requests

### Verbose Output

Enable verbose output for detailed debugging:
```bash
dotnet run -- [command] --verbose
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

### Project Structure

```
ClickUp.Api.Client.CLI/
├── Commands/           # Command implementations
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