# GEMINI.md - ClickUpApiMcpLib Repository Information

This file contains essential information about the `ClickUpApiMcpLib` repository for the Gemini model's future reference.

## 1. Project Overview
This repository contains a C#/.NET library designed to interact with the ClickUp API. It includes core models, abstractions, a main client implementation (with a Fluent API), and various test projects.

## 2. Key Directories and Their Contents

*   **`src/ClickUp.Api.Client.Models/`**: Contains the C# data models (POCOs) that represent the data structures used in the ClickUp API (e.g., tasks, users, workspaces).
*   **`src/ClickUp.Api.Client.Abstractions/`**: Defines interfaces and abstract classes for the API client, promoting loose coupling and testability.
*   **`src/ClickUp.Api.Client/`**: The main implementation of the ClickUp API client, including the Fluent API for a more readable and chainable interface.
*   **`src/ClickUp.Api.Client.Tests/`**: Contains unit tests for the library's components. This is where `FluentApiTests.cs` is located.
*   **`src/ClickUp.Api.Client.IntegrationTests/`**: Contains integration tests that interact with the actual ClickUp API. These tests require specific API credentials to run successfully.
*   **`examples/`**: Contains example applications demonstrating how to use the ClickUp API client library.
*   **`docs/`**: Stores project documentation, including conceptual guides and API specifications.

## 3. Build and Test Commands

To build the solution:
```bash
dotnet build
```
(Execute this command from the `src/` directory: `C:/Source/ClickUpApiMcpLib/src/`)

To run all tests (unit and integration):
```bash
dotnet test
```
(Execute this command from the `src/` directory: `C:/Source/ClickUpApiMcpLib/src/`)

To run only unit tests (excluding integration tests):
```bash
dotnet test --filter "FullyQualifiedName!~IntegrationTest"
```
(Execute this command from the `src/` directory: `C:/Source/ClickUpApiMcpLib/src/`)

## 4. Integration Test Specifics

Integration tests located in `src/ClickUp.Api.Client.IntegrationTests/` require a `ClickUp API PersonalAccessToken` to be configured. Without this, these tests are expected to fail with an `InvalidOperationException` indicating that the token is not configured. This token can be set via .NET User Secrets or Environment Variables (e.g., `ClickUpApi__PersonalAccessToken`).

## 5. Technology Stack

*   **Language**: C#
*   **Framework**: .NET (currently targeting .NET 9.0)
*   **Testing Frameworks**: XUnit, Moq (for mocking in unit tests)
