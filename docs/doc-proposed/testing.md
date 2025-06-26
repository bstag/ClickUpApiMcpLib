# Testing

This document provides an overview of the testing strategy for the ClickUp API Client SDK, covering both unit and integration tests.

## Philosophy

The testing strategy is designed to ensure the SDK is reliable, correct, and easy to maintain. It is based on two main types of tests:

- **Unit Tests**: These tests are fast, isolated, and verify the logic of individual components without making any network calls.
- **Integration Tests**: These tests verify the SDK's interaction with the live ClickUp API, ensuring that requests and responses are handled correctly.

## Unit Tests

The unit tests are located in the `ClickUp.Api.Client.Tests` project. They use mocking frameworks like Moq to isolate the code under test from its dependencies, particularly the `IApiConnection`.

**Goals of Unit Tests:**

- Verify the business logic of the service classes.
- Ensure that request models are correctly serialized.
- Check that response models are correctly deserialized.
- Test error handling and exception throwing.

To run the unit tests, use the following command:

```bash
dotnet test src/ClickUp.Api.Client.Tests/ClickUp.Api.Client.Tests.csproj
```

## Integration Tests

The integration tests are in the `ClickUp.Api.Client.IntegrationTests` project. These tests are designed to be run against the real ClickUp API, but they use a recording and playback mechanism to avoid making live API calls on every test run.

### Recording and Playback

The integration tests can be run in two modes:

- **Record Mode**: In this mode, the tests make live API calls to the ClickUp API and save the JSON responses to files in the `test-data/recorded-responses` directory. This mode is used to generate or update the test recordings.
- **Playback Mode**: In this mode, the tests do not make any network calls. Instead, they use the previously recorded JSON responses to simulate the API. This is the default mode for running the tests and is ideal for CI/CD environments.

### Running Integration Tests

To run the integration tests in playback mode, use the following command:

```bash
dotnet test src/ClickUp.Api.Client.IntegrationTests/ClickUp.Api.Client.IntegrationTests.csproj
```

To run the tests in record mode, you need to set the `CLICKUP_SDK_TEST_MODE` environment variable to `Record` and provide a valid `ClickUpApi__PersonalAccessToken`.

For detailed instructions on generating test recordings, see the `GENERATING_TEST_RECORDINGS.md` file in the root of the repository.