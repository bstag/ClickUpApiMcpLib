# Jules's Knowledge Base

This file contains specific notes, conventions, and information Jules needs to remember about this repository and the ongoing tasks.

## Project Overview & Goals
- Objective: Develop a professional-grade .NET client library for the ClickUp API, incorporating best practices for design, resilience, developer experience, and AI integration.
- Project Type: .NET 9 Core
- Master Conceptual Plan: `docs/plans/geminiPlan.md` (provides comprehensive architectural guidance).
- Operational Guidance: Derived from the latest user instructions, which supersede any previous specific operational plan documents.
- OpenAPI Spec: `docs/OpenApiSpec/ClickUp-6-17-25.json`
- API Reference Website: `https://developer.clickup.com/reference/`

## Current Project State:
- **Models (`src/ClickUp.Api.Client.Models`):** Some models exist, but many are missing. The task is to identify and implement these.
- **Service Interfaces (`src/ClickUp.Api.Client.Abstractions`):** Defined, but require refinement to use concrete DTOs instead of generic `object` types and to ensure parameter correctness. `docs/plans/02-abstractions-interfaces-actual.md` tracks their existence.
- **Service Implementations (`src/ClickUp.Api.Client`):** Implementation is in progress. Focus is on resolving build errors stemming from interface implementations and missing types.
- **`ClickUp.Net.Abstractions`:** This path appears to be deprecated or non-existent. The focus is on `ClickUp.Api.Client.Abstractions`.

## Current Task (as per prompt on 2024-07-08 and subsequent interactions):
1.  **Primary Goal:** Review the existing codebase (`src/`) and the OpenAPI specification (`docs/OpenApiSpec/ClickUp-6-17-25.json`), guided by `docs/plans/geminiPlan.md`, to identify and implement all missing request objects, response objects, and other entities.
2.  **Error Resolution:** This effort is primarily aimed at resolving CS0246 (type not found), CS0738 (incorrect interface implementation), and CS0535 (missing interface member implementation) errors that occur during the build process.
3.  **Build Status (as of 2024-07-09):** Last build resulted in **59 errors**. Recent fixes in `ITaskRelationshipsService.cs` (changing return types to `Task<CuTask?>`) and `TaskRelationshipsService.cs` seem to have caused these, likely due to signature mismatches or related issues.
4.  **Immediate Next Steps (2024-07-09):**
    *   Re-examine the latest build output (59 errors).
    *   Focus heavily on `TaskRelationshipsService.cs` to ensure its method signatures and implementations correctly align with `ITaskRelationshipsService.cs`, especially concerning `CuTask?`.
    *   Investigate the `DeleteAsync` method usage in `TaskRelationshipsService.DeleteTaskLinkAsync`. Check if `IApiConnection` needs a generic `DeleteAsync<TResponse>` or if the ClickUp API returns no content on this specific delete operation (which would mean `Task` is the correct return type for the service method, and the interface should match).
    *   Systematically go through other errors if the above doesn't resolve the majority.

## Key Files & Directories:
- OpenAPI Spec: `docs/OpenApiSpec/ClickUp-6-17-25.json`
- API Reference: `https://developer.clickup.com/reference/`
- Master Conceptual Plan: `docs/plans/geminiPlan.md`
- Model Implementation Tracking: `docs/plans/01-core-models-actual.md` (May need review and updates based on current task)
- Interface Implementation Tracking: `docs/plans/02-abstractions-interfaces-actual.md`
- This file (Project Notes): `jules.md`
- Prompt history: `docs/prompts.md`
- Models: `src/ClickUp.Api.Client.Models/`
- Abstractions: `src/ClickUp.Api.Client.Abstractions/`
- Client Implementation: `src/ClickUp.Api.Client/`
- Tests: `src/ClickUp.Api.Client.Tests/`
- Examples: `examples/`
- .NET install script: `utilities/dotnet-install.sh`

## Build and Test Commands:
- Install .NET 9: `utilities/dotnet-install.sh --channel 9.0` (run from the repository root)
- Build: `dotnet build src/ClickUp.Api.sln --nologo` (run from the repository root, or `dotnet build ClickUp.Api.sln --nologo` from the `src` folder)
- Test: `dotnet test src/ClickUp.Api.sln` (run from the repository root, or `dotnet test` from the `src` folder if the context is already there)


## Notes on `geminiPlan.md`:
The `geminiPlan.md` is the primary source for architectural principles, including:
- SOLID principles, Facade, Strategy, Builder, Observer patterns.
- Fluent interfaces for usability.
- Clean Architecture (Abstractions, Client, Examples projects).
- `IHttpClientFactory`, Polly for resilience.
- Custom exception hierarchy.
- Authentication (Personal Token, OAuth 2.0).
- Pagination, async streaming.
- Webhook helpers.
- DocFX for documentation.
- AI integration (Semantic Kernel, MCP).
The current tasks aim to implement these aspects systematically, guided by user prompts.

## General Instructions from User:
- Save all prompts to `/docs/prompts.md` with date and time.
- Keep this file (`jules.md`) updated with relevant repository/task information.

## Task History Excerpt:
- **2024-07-08 (Initial Phase):** Focus on creating missing models like `GetChatChannelsResponse`, `CreateDirectMessageChatChannelRequest`, etc., to resolve CS0246. Initial build had 90 errors.
- **Progressive Fixes:**
    - Corrected type names in `ChatService.cs` (e.g., `CreateDirectMessageChatChannelRequest` -> `ChatCreateDirectMessageChatChannelRequest`).
    - Fixed `GetChatChannelsAsync` in `ChatService.cs` to use `ChatChannelPaginatedResponse`.
    - Created `UpdateKeyResultRequest.cs`.
    - Corrected `GoalsService.EditKeyResultAsync` to use `EditKeyResultRequest`.
    - Created `User.cs`, `Member.cs`, `GetMembersResponse.cs` and fixed related using statements and qualifications in `IMembersService.cs`.
    - Created `CustomRole.cs`, `GetCustomRolesResponse.cs` and fixed `RolesService.cs` / `IRolesService.cs`.
    - Corrected `SharedHierarchyService.cs` using statements and return types.
    - Created `AddDependencyRequest.cs`.
    - Updated `ITaskRelationshipsService.cs` and `TaskRelationshipsService.cs` method signatures to use `Task<CuTask?>`. Fully qualified `CuTask` in `ITaskRelationshipsService.cs`.
- **Current Build Status:** 59 errors. Warnings persist (CS8618, CS8613).
