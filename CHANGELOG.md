# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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