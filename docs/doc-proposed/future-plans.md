
# Future Plans

This document outlines the future work planned for the ClickUp API Client SDK. It has been updated to reflect the completion of the initial service implementations and a review of the authentication system, test coverage, resilience policies, and documentation.

## Testing Enhancements

- **Expand Integration Test Coverage**:
    - **Increase Service Coverage**: Create integration test files for all services that currently lack them.
    - **Complex Scenarios**: Add tests for more complex scenarios, including error handling, complex filtering, and pagination.
    - **File Operations**: Implement integration tests for file attachments.

- **Expand Unit Test Coverage**: 
    - **Improve Mocking**: Refactor existing tests to use specific object matching.
    - **Test for Specific `ClickUpApiException`s**: Add tests to simulate different HTTP error codes.

## Authentication Enhancements

- **OAuth 2.0 Token Refresh**: Implement logic to automatically refresh expired OAuth 2.0 access tokens.
- **OAuth 2.0 Authorization Flow Helpers**: Provide helper methods to simplify the initial OAuth 2.0 authorization process.

## Core Functionality

- **Integrate Polly for Resilience**: 
    - **Idempotent Retries**: Configure the retry policy to only apply to idempotent HTTP methods.
    - **Advanced Circuit Breaker Logic**: Enhance the circuit breaker policy with features like dynamic break durations.
    - **Bulkhead Isolation**: Implement a Bulkhead policy to limit concurrent requests.
    - **Timeout Policy**: Add a Timeout policy to prevent long-running requests.

## Developer Experience Enhancements

- **API Documentation with DocFX**: While the DocFX project is well-structured, the following work is needed to make it a complete and valuable resource for developers:
    - **Complete Conceptual Articles**: Fill in the placeholder content for the `webhooks.md` and `semantic-kernel-integration.md` articles.
    - **Generate and Publish API Reference**: Ensure that the XML comments in the source code are complete and accurate, and then build and publish the API reference documentation to a live URL.
    - **Add Visuals**: Enhance the documentation with diagrams, screenshots, and other visuals to improve clarity.
    - **Implement Versioning**: Establish a strategy for versioning the documentation to correspond with different versions of the SDK.

- **Showcase Example Projects**: The existing examples provide a good starting point, but they should be enhanced to cover more advanced and real-world scenarios.

- **Implement Pagination Helpers**: While a generic helper for cursor-based pagination exists, it is only used in the `DocsService`. The following work is needed to provide a complete and consistent pagination experience:
    - **Wider Adoption**: Implement the existing `IAsyncEnumerable<T>` helper for all cursor-paginated endpoints across all relevant services (e.g., `ChatService`).
    - **Page-Based Pagination Helper**: Create a new `IAsyncEnumerable<T>` helper to handle page-based pagination, which is common throughout the ClickUp API (e.g., in `TasksService`, `TemplatesService`, etc.).

## Advanced Features

- **Webhook Helpers**: Provide utilities for consuming ClickUp webhooks.

- **Semantic Kernel Integration**: Create Semantic Kernel plugins to make the SDK easily consumable by AI agents.
