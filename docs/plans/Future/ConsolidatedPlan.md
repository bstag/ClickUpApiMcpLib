# Consolidated ClickUp SDK Future Plan
### 10. Implement Webhook Helpers
- [ ] **Task:** Thoroughly research and document ClickUp's webhook payload structures for common events and the exact signature validation mechanism.
    - *Why:* Foundational knowledge for building reliable helpers.
    - *Ref:* `docs/plans/updatedPlans/webhooks/10-WebhookHelpers.md` (Conceptual)
- [ ] **Task:** Define C# DTOs in `src/ClickUp.Api.Client.Models/Webhooks/` for common incoming webhook payloads.
- [ ] **Task:** Implement `WebhookSignatureValidator.cs` in `src/ClickUp.Api.Client/Webhooks/` to validate incoming webhook signatures.
- [ ] **Task:** Write unit tests for payload DTO deserialization and the `WebhookSignatureValidator`.
- [ ] **Task:** Add conceptual documentation (`articles/webhooks.md`) explaining setup and usage.
- *Files:* New files in `src/ClickUp.Api.Client.Models/Webhooks/`, `src/ClickUp.Api.Client/Webhooks/`, `src/ClickUp.Api.Client.Tests/Webhooks/`, `docs/docfx/articles/webhooks.md`.
- *Why:* Assists users in securely consuming ClickUp webhooks.
- *Ref:* `docs/plans/updatedPlans/webhooks/10-WebhookHelpers.md`

### 11. Implement AI Integration (Semantic Kernel)
- [ ] **Task:** Create the new project `src/ClickUp.Api.Client.SemanticKernel/`.
- [ ] **Task:** Design and implement Semantic Kernel plugins wrapping key SDK services (e.g., `TaskPlugin`, `ListPlugin`). Focus on methods identified as high-value for AI.
- [ ] **Task:** Implement DI registration for these plugins.
- [ ] **Task:** Write unit tests for the Semantic Kernel plugins, mocking SDK services.
- [ ] **Task:** Add conceptual documentation (`articles/semantic-kernel-integration.md`) on using these plugins.
- *Files:* New project and files under `src/ClickUp.Api.Client.SemanticKernel/`, new tests, `docs/docfx/articles/semantic-kernel-integration.md`.
- *Why:* Enables AI agent interaction with the ClickUp API via the SDK.
- *Ref:* `docs/plans/updatedPlans/ai_integration/11-SemanticKernelIntegration.md`

## Ongoing Tasks (Implicit across phases)
*   Continuously review models and interfaces against OpenAPI spec for deviations or missing elements (`01-CoreModelsAndAbstractions.md`).
*   Maintain high test coverage as new features are added (`07-TestingStrategy.md`).
