# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.15.0] - 2026-10-22

Langfuse removed the v3 ingestion and read endpoints from its OpenAPI spec. Langfuse Cloud shuts them down on 2026-11-16; self-hosted deployments lose them when upgrading to Langfuse v4 (see https://langfuse.com/self-hosting/upgrade/upgrade-guides/upgrade-v3-to-v4). This release removes the client code for them. Send traces through OpenTelemetry (`AddLangfuseExporter` + `IOtelLangfuseTrace` / `OtelLangfuseTrace`) and read data with `GetObservationsV2Async`, `GetScoresV3Async`, `GetMetricsV2Async`, `GetExperimentsAsync` and `GetExperimentItemsAsync`.

### Added

- Skills API (`/api/public/skills`, stable): `GetSkillListAsync`, `GetSkillAsync` (by version or label, defaults to `production`; `includeContent` returns file text in `SkillFile.Content`), `CreateSkillVersionAsync`, `UpdateSkillAsync` (shared tags), `GetSkillFileContentsAsync` (by SHA-256 hash), `UpdateSkillVersionLabelsAsync`, `DeleteSkillVersionAsync`, with models in `zborek.Langfuse.Models.Skill`.
- Organization-level LLM connections (`/api/public/organizations/llm-connections`, organization-scoped API key): `GetOrganizationLlmConnectionsAsync`, `UpsertOrganizationLlmConnectionAsync`, `DeleteOrganizationLlmConnectionAsync`.
- `ScoreCreateRequest.Timestamp` (`DateTimeOffset?`). Defaults to the time the request is received; sending a score again with the same `Id` and `Timestamp` updates it.
- `DatasetItem.Version`: when this version of the item became current.

### Changed

- **Breaking:** `CreateScoreAsync` now posts a one-element array to `POST /api/public/scores` and returns `CreateScoreBatchResponse` instead of `CreateScoreResponse`, because the endpoint no longer accepts a single score object. Scores are created asynchronously and the response carries no score id, so `CreateScoreAsync` and `CreateScoresAsync` set a null `ScoreCreateRequest.Id` to a new GUID before sending. When the server rejects the score (HTTP 207 with `Rejected > 0`), `CreateScoreAsync` throws `LangfuseApiException` with status code 207 and the response in `ResponseBody`.
- **Breaking:** `LangfuseConfig.DefaultTimeout` is now applied to the API client's `HttpClient.Timeout`. Before, it was ignored and requests used the 100-second `HttpClient` default, so the effective timeout drops to 30 seconds unless you set it. A timed-out request throws `LangfuseApiException` with status code 408. `AddLangfuse` throws `ArgumentOutOfRangeException` when `DefaultTimeout` is zero or negative (except `Timeout.InfiniteTimeSpan`).
- `GetSkillFileContentsAsync` throws `ArgumentException` for more than 50 hashes, and `CreateSkillVersionAsync` throws `ArgumentException` when `Files` is empty or a file does not set exactly one of `Content` or `Sha256Hash`.
- `OpenTelemetry`, `OpenTelemetry.Api` and `OpenTelemetry.Exporter.OpenTelemetryProtocol` dependencies raised from 1.9.0 to 1.15.3 to fix security advisories GHSA-g94r-2vxg-569j and GHSA-4625-4j76-fww9. OpenTelemetry 1.15.3 requires Microsoft.Extensions 10, so the `Microsoft.Extensions.*` dependencies are raised from 9.0.1 to 10.0.0 on all target frameworks.

### Removed

- **Breaking:** `LangfuseConfig.DefaultPageSize`, `EnableRetry`, `MaxRetryAttempts` and `RetryDelay`. The client never read them.

- **Breaking:** legacy ingestion: `ILangfuseClient.IngestAsync` (both overloads), `LangfuseTrace` and its scoped/non-scoped creation methods, `LangfuseBackgroundService`, the ingestion event types (`IIngestionEvent`, `CreateTraceEvent`, `CreateSpanEvent`, `CreateGenerationEvent`, `CreateEvent`, `CreateScoreEvent`, `UpdateSpanEvent`, `UpdateGenerationEvent` and their `*Body` types, `IngestionRequest`, `IngestionResponse`, `IngestionSuccessResponse`, `IngestionErrorResponse`), `ScopedMethodAttribute` / `NonScopedMethodAttribute`, and `LangfuseConfig.BatchMode` / `LangfuseConfig.BatchWaitTime`. `AddLangfuse` no longer registers `LangfuseTrace` or the background service.
- **Breaking:** the Roslyn analyzers (`LANG001`, `LANG002`) are no longer shipped in the package; they only checked `LangfuseTrace` usage.
- **Breaking:** trace reads `GetTraceListAsync`, `GetTraceAsync` (`DeleteTraceAsync` and `DeleteTraceManyAsync` remain) and the types `TraceListRequest`, `TraceListResponse`, `TraceModel`, `TraceWithDetails`, `TraceUsage`, `TraceFilter`, `TraceFilterCondition` (and its subclasses), `TraceFilterType`.
- **Breaking:** sessions API `GetSessionListAsync`, `GetSessionAsync` and `SessionModel`, `SessionListRequest`, `SessionListResponse`.
- **Breaking:** v1 observations `GetObservationListAsync`, `GetObservationAsync` and `ObservationModel`, `ObservationOtelMetadata`, `ObservationListRequest`, `ObservationListResponse`, `ObservationType`, `Usage`.
- **Breaking:** v1 metrics `GetMetricsAsync` and `MetricsRequest`, `MetricsResponse`, `Metric`, `MetricsDimension`, `MetricsFilter`.
- **Breaking:** v2 score reads `GetScoreListAsync`, `GetScoreAsync` and `ScoreModel`, `ScoreListRequest`, `ScoreListResponse`, `CreateScoreResponse`.
- **Breaking:** dataset runs and run items `GetDatasetRunAsync`, `DeleteDatasetRunAsync`, `GetDatasetRunsAsync`, `CreateDataSetRunAsync`, `GetDatasetRunListAsync` and `DatasetRun`, `DatasetRunItem`, `DatasetRunWithItems`, `PaginatedDatasetRuns`, `DatasetRunListRequest`, `DeleteDatasetRunResponse`, `CreateDatasetRunItemRequest`, `DatasetRunItemListRequest`, `PaginatedDatasetRunItems`. Use experiments instead.
- `Deprecation` model (only returned by the removed endpoints).
- Package dependency on `Microsoft.Extensions.Hosting.Abstractions` (only the removed background service used it).
- `Langfuse.Example.WebApi` example; `Langfuse.Example.OpenTelemetry` covers the same OpenAI scenario.

## [0.14.0] - 2026-10-12

### Added

- Batch score creation: `CreateScoresAsync` (`POST /api/public/scores` with an array body) returning `CreateScoreBatchResponse` — `Message` ("Accepted") on HTTP 202, or `Accepted`/`Rejected` counts with `Errors` on partial acceptance (HTTP 207). Do not retry a partially accepted batch automatically; accepted scores would be duplicated.
- API keys: `Name` and `ExpiresAt` on `CreateApiKeyRequest`, `ApiKeyResponse`; `Name` on `ApiKeySummary` and `OrganizationApiKey`.
- `PromptListRequest.Filter` (JSON filter conditions, takes precedence over the other list parameters).
- `BlobStorageIntegrationType.GoogleCloudStorage`.
- `EvaluationRuleStringObjectFilterOperator` with `IsSet` / `IsNotSet` (`is set` / `is not set`) for key-presence filters on `StringObjectEvaluationRuleFilter`.

### Changed

- **Breaking:** `StringObjectEvaluationRuleFilter.Operator` is now `EvaluationRuleStringObjectFilterOperator` instead of `EvaluationRuleStringFilterOperator`, and `Value` is no longer required (ignored for `is set` / `is not set`).
- API key `Note` properties marked `[Obsolete]` in favour of `Name` (same value; `Name` and `Note` cannot both be set on create).
- Legacy ingestion (`IngestAsync`, `LangfuseTrace.IngestAsync`) obsolete messages updated: from November 16, 2026 Langfuse Cloud keeps accepting `score-create` events on the legacy endpoint and rejects all other event types.

## [0.13.0] - 2026-09-08

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
- Build/test infrastructure moved to the .NET 10 SDK (`global.json`), xunit v3 and Microsoft.Testing.Platform; test projects now run as executables. The library itself still targets the same frameworks.

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

[Unreleased]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.15.0...HEAD
[0.15.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.14.0...v0.15.0
[0.14.0]: https://github.com/lukaszzborek/Langfuse-dotnet/compare/v0.13.0...v0.14.0
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