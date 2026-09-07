# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.13.0] - 2026-09-07

### Added

- Stable Evaluators API (`/api/public/v2/evaluators`): `CreateEvaluatorAsync`, `GetEvaluatorsAsync`, `GetEvaluatorAsync`, `UpdateEvaluatorAsync`, `DeleteEvaluatorAsync`, `GetEvaluatorVersionsAsync`. Evaluators carry `Status`/`PausedReason`, `CreatedBy`, flattened latest-version fields (`VersionId`, `Version`, `VersionCreatedAt`, `VersionCreatedBy`) and `EvaluationRuleAssignments`. LLM-as-a-judge prompts are chat message lists (`EvaluatorChatMessage`); requests accept a plain string shortcut via `EvaluatorChatPromptInput`. Updates use `UpdateEvaluatorMetadataRequest` (no new version) or `UpdateLlmAsJudgeEvaluatorRequest`/`UpdateCodeEvaluatorRequest` (full definition replacement); assigning `Description = null` explicitly clears the description, omitting it keeps the current one.
- Stable Evaluation Rules API (`/api/public/v2/evaluation-rules`): rules reference evaluators by stable id through `EvaluatorAssignments` (`EvaluationRuleEvaluatorAssignmentInput` with optional per-rule `PromptVariableMappingInput` override). Responses include legacy trace/dataset rules; legacy mappings are exposed on `PromptVariableMapping` (`IsLegacy`, `LangfuseObject`, `ObjectName`).
- Cursor pagination for evaluators and rules: `EvaluatorsPage`, `EvaluatorVersionsPage`, `EvaluationRulesPage` with `CursorMeta`.
- `UpsertModelAsync` (`PUT /api/public/models/{id}`) to create or replace a project-owned model by id.
- `ModelTokenizerId` enum (`openai`, `claude`).

### Changed

- **Breaking:** the unstable evaluator/evaluation-rule endpoints (`/api/public/unstable/...`) were replaced by the stable v2 API (Langfuse removed them from Cloud on 2026-09-04). Removed types: `EvaluatorScope`, `EvaluationRuleTarget`, `EvaluationRuleStatus`, `EvaluationRuleMapping`, `EvaluationRuleMappingSource` (now `PromptVariableMappingSource`), `EvaluationRuleEvaluator`, `*EvaluationRuleEvaluatorReference`, `Create*EvaluationRuleRequest` subclasses (now a single `CreateEvaluationRuleRequest`), `EvaluatorOutputDataType` (now `EvaluatorOutputScoreType`), `EvaluatorOutputFieldDefinition`/`EvaluatorOutputScoreDefinition` (flattened into `EvaluatorOutputDefinition`), `PaginatedEvaluators`/`PaginatedEvaluationRules`, `DeleteEvaluatorResponse`/`DeleteEvaluationRuleResponse` (now `DeletedEvaluator`/`DeletedEvaluationRule` with `Id`). `GetEvaluatorsAsync`/`GetEvaluationRulesAsync` take `(limit, cursor)` instead of `(page, limit)`. Member changes on kept types: `Evaluator.Scope`/`EvaluationRuleCount` removed and `Evaluator.Variables` moved to `LlmAsJudgeEvaluator`; `LlmAsJudgeEvaluator.Prompt` is `EvaluatorChatMessage[]` instead of `string`; `EvaluationRule.Evaluator`/`Target`/`Status`/`PausedReason`/`PausedMessage`/`Mapping` replaced by `CreatedBy`/`EvaluatorAssignments`; `CreateEvaluationRuleRequest` is no longer abstract and has no `Target`.
- **Breaking:** `CreateModelRequest.Unit` is now required (non-nullable) and `CreateModelRequest.TokenizerId` is a `ModelTokenizerId?` enum instead of `string?`, per spec.
- **Breaking:** `PricingTierInput.IsDefault` is now `bool?` instead of `bool`, and `PricingTierUsageCondition.CaseSensitive` is now `bool?` instead of `bool`. Both are optional on input and default to false when omitted; `caseSensitive` is required in the response schema, so conditions read back from the API always carry a value. Code reading these properties directly (`if (tier.IsDefault)`) must handle null, e.g. `tier.IsDefault == true`.
- `CreateModelRequest` flat price fields documented as deprecated in favour of `PricingTiers`.
- Observations V2 field group docs synced with the spec: `isRootObservation` (basic), `usagePricingTierName` (usage), the new `trace_context` group (tags, release, traceName), metadata truncation note, and `providedModelName` renamed to `model`.

## [0.12.0] - 2026-08-19

### Changed

- **Breaking:** `PricingTierCondition` is now an abstract base of `PricingTierUsageCondition` (the previous regex/threshold shape) and new `PricingTierAttributeCondition` (`Source`: `PricingTierAttributeSource.ModelParameters`/`Metadata`, `Key`, `Operator` = `in`, `Values`). Replace `new PricingTierCondition { ... }` with `new PricingTierUsageCondition { ... }`. Deserialization picks the concrete type by shape.
- Marked Langfuse v3 endpoints as `[Obsolete]` per OpenAPI spec (removed on Langfuse Cloud 2026-11-16, on self-hosted when upgrading to v4): `GetDatasetRunAsync`, `DeleteDatasetRunAsync`, `GetDatasetRunsAsync`, `CreateDataSetRunAsync`, `GetDatasetRunListAsync`, `GetScoreListAsync`, `GetScoreAsync`, `GetTraceListAsync`, `GetTraceAsync`, `GetObservationAsync`, `GetSessionListAsync`, `GetSessionAsync`. Existing obsolete messages for `IngestAsync`, `GetMetricsAsync`, `GetObservationListAsync` updated with removal date and replacements.
- Doc comments synced with spec: `CreateCommentRequest.AuthorUserId` must be an org member, `CreateScimUserRequest.Password` is ignored, `BlobStorageExportSource`/`ExportSource` default and availability notes.

## [0.11.0] - 2026-08-09

### Added

- Feedback API: `SubmitFeedbackAsync` (`POST /api/public/feedback`) with `SubmitFeedbackRequest`, `SubmitFeedbackResponse`, and `FeedbackTargetType` (#38)
- `Deprecation` model surfaced as `_deprecation` on responses from deprecated endpoints (dataset runs, observations, scores, sessions, traces) (#38)
- Observations V2 filters: `SessionId` and `IsRootObservation` on `ObservationsV2Request` (#38)
- `ScoreCreateRequest.Source` with new `CreateScoreSource` enum (`API`, `ANNOTATION`) (#38)
- Metrics views: `scores-boolean` supported alongside numeric and categorical (#38)

## [0.10.0] - 2026-07-11

### Added

- Experiments API: `GetExperimentsAsync` and `GetExperimentItemsAsync` (`GET /api/public/experiments`, `GET /api/public/experiment-items`) with cursor-based pagination and field groups ([Langfuse changelog](https://langfuse.com/changelog/2026-07-07-experiments-public-api-and-mcp))
- Scores V3 API: `GetScoresV3Async` (`GET /api/public/v3/scores`) with polymorphic `ScoreV3` models (numeric, boolean, categorical, text, correction) and `ScoreSubjectV3` (trace, observation, session, experiment) ([Langfuse changelog](https://langfuse.com/changelog/2026-06-10-scores-v3-api))
- Dashboard widgets (unstable API): `CreateDashboardWidgetAsync` (`POST /api/public/unstable/dashboard-widgets`)
- Evaluator delete endpoint: `DeleteEvaluatorAsync` (`DELETE /api/public/unstable/evaluators/{evaluatorId}`) ([Langfuse changelog](https://langfuse.com/changelog/2026-06-15-delete-evaluator-templates))
- Code evaluators: `CodeEvaluator`, `CreateCodeEvaluatorRequest`, `CreateCodeEvaluationRuleRequest`, `EvaluatorType.Code` ([Langfuse changelog](https://langfuse.com/changelog/2026-05-28-code-evaluators))
- Media uploads for dataset items: `MediaUploadRequest.DatasetId` / `DatasetItemId` ([Langfuse changelog](https://langfuse.com/changelog/2026-06-23-multi-modal-datasets))
- `DatasetItem.MediaReferences` with resolved media records ([Langfuse changelog](https://langfuse.com/changelog/2026-06-23-multi-modal-datasets))
- Enum values: `BlobStorageIntegrationFileType.Parquet` ([Langfuse changelog](https://langfuse.com/changelog/2026-07-08-parquet-blob-storage-exports)), `BlobStorageSyncStatus.Running`, `EvaluationRuleMappingSource.Tool_Calls` ([Langfuse changelog](https://langfuse.com/changelog/2026-07-10-evaluator-tool-calls))
- `EvaluationRuleEvaluator.Type` property
- Environment support for traces (#34)

### Changed

- **Breaking:** `Evaluator`, `CreateEvaluatorRequest`, and `CreateEvaluationRuleRequest` are now abstract bases of discriminated unions (`llm_as_judge` / `code`). Use `LlmAsJudgeEvaluator`/`CodeEvaluator`, `CreateLlmAsJudgeEvaluatorRequest`/`CreateCodeEvaluatorRequest`, and `CreateLlmAsJudgeEvaluationRuleRequest`/`CreateCodeEvaluationRuleRequest`
- **Breaking:** `MediaUploadRequest.TraceId` is now optional (`string?`); exactly one context is required — trace or dataset item

## [0.9.0] - 2026-06-04

### Added

- Evaluator and evaluation rule models and endpoints (#31) ([Langfuse changelog](https://langfuse.com/changelog/2026-04-15-llm-as-a-judge-api))
- API client and types documentation, context7 integration (#28, #29)

### Changed

- Updated package dependencies across projects (#32)

## [0.8.0] - 2026-03-18

### Changed

- Improved response handling performance with `ReadFromJsonAsync`

## [0.7.0] - 2026-03-16

### Added

- .NET 10 target framework support (#25)
- Cache input token attributes and methods in `GenAiActivityHelper` (#24)

## [0.6.1] - 2026-03-07

### Added

- Methods for setting trace and request parameters in `GenAiActivityHelper`

## [0.6.0] - 2026-03-02

### Added

- Agent Framework example with OpenTelemetry integration

### Changed

- Models enforce non-nullable properties; new fields added (#22)

## [0.5.2] - 2026-01-19

### Added

- Symbol package (snupkg) support (#21)

## [0.5.1] - 2025-12-29

### Changed

- **Breaking:** reworked OpenTelemetry settings (#20)

## [0.5.0] - 2025-12-29

### Added

- OpenTelemetry tracing support (#13, #16)
- Multi-service tracing with `IOtelLangfuseTraceContext` (#15)
- Missing API endpoints (#17)
- Observation metadata deserialization (#19)

### Changed

- Renamed `BaseAddress` to `Endpoint` in `LangfuseOtlpExporterOptions` (#14)

## [0.4.0] - 2025-10-12

### Added

- New API endpoints (#12)

## [0.3.0] - 2025-08-09

### Added

- Full API endpoint coverage (#7)
- Roslyn analyzers (#8, #9)

## [0.2.1] - 2025-06-08

### Fixed

- `CreateEventBody` metadata parameter

## [0.2.0] - 2025-03-03

### Added

- Ingestion batch splitting

## [0.1.0] - 2025-02-16

Initial release.

[0.13.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.12.0...v0.13.0
[0.12.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.11.0...v0.12.0
[0.11.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.10.0...v0.11.0
[0.10.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.9.0...v0.10.0
[0.9.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.8.0...v0.9.0
[0.8.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.7.0...v0.8.0
[0.7.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.6.1...v0.7.0
[0.6.1]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.6.0...v0.6.1
[0.6.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.5.2...v0.6.0
[0.5.2]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.5.1...v0.5.2
[0.5.1]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.5.0...v0.5.1
[0.5.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.4.0...v0.5.0
[0.4.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.3.0...v0.4.0
[0.3.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.2.1...v0.3.0
[0.2.1]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.2.0...v0.2.1
[0.2.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/lukaszzborek/Langfuse-dotnet/releases/tag/v0.1.0