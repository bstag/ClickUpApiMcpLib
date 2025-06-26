
# Examples

This repository includes two example projects to demonstrate how to use the ClickUp API Client SDK in different scenarios: a console application and a background worker service.

## Console Application (`ClickUp.Api.Client.Console`)

The console application provides a set of examples for common SDK operations. It demonstrates how to:

- Configure the SDK using `appsettings.json` and dependency injection.
- Authenticate with the ClickUp API.
- Perform CRUD (Create, Read, Update, Delete) operations on tasks.
- Create and retrieve comments.
- Handle API exceptions.

### How to Run

1.  Navigate to the `examples/ClickUp.Api.Client.Console` directory.
2.  Configure your ClickUp API token and other settings in `appsettings.json`.
3.  Run the application using `dotnet run`.

## Worker Service (`ClickUp.Api.Client.Worker`)

The worker service example shows how to use the SDK in a long-running background process. It demonstrates:

- How to set up a hosted service that uses the SDK.
- How to periodically poll a ClickUp list for new or updated tasks.

### How to Run

1.  Navigate to the `examples/ClickUp.Api.Client.Worker` directory.
2.  Configure your ClickUp API token and the list to be polled in `appsettings.json`.
3.  Run the application using `dotnet run`.
