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

## Current Task (as per prompt on 2024-07-10):
1.  **Documentation Update:** Detail out steps 4, 5, and 6 of `NEW_OVERALL_PLAN.md`.
    *   This is a documentation-only task.
    *   Involves reviewing existing conceptual plans and the main `NEW_OVERALL_PLAN.md`.
    *   The goal is to add detailed checklists and mark accomplished sub-tasks for these sections.
    *   Status: In progress. `NEW_OVERALL_PLAN.md` has been updated with the detailed breakdown. `jules.md` and `prompts.md` are being updated.

## Previous Task (as per prompt on 2024-07-09):
1.  **Build and Test:** Build the solution and run tests to ensure the last set of changes were successful and the environment is stable.
    *   **Build Status (2024-07-09):** Successful (0 errors, 3 warnings - CS8424 regarding `EnumeratorCancellationAttribute` misuse).
    *   **Test Status (2024-07-09):** Pending.
2.  **Continue with `NEW_OVERALL_PLAN.md`:** Proceed with the next steps outlined in `docs/plans/NEW_OVERALL_PLAN.md`.
    *   The (then) immediate next step according to the plan was **Phase 2, Step 4: Implement Service Layer in `ClickUp.Api.Client`**, focusing on implementing the actual HTTP call logic. This is now superseded by the documentation task.

## Key Files & Directories:

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
- **Build Status (2024-07-09):** Successful (0 errors, 3 warnings - CS8424 regarding `EnumeratorCancellationAttribute` misuse). Warnings persist (CS8618, CS8613) from previous builds but were not present in the latest.
