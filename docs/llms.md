# LangfuseDotnet

> A .NET SDK (v0.8.0) for Langfuse LLM observability. Uses OpenTelemetry with
> Gen AI semantic conventions to trace LLM calls, embeddings, tool calls, and
> agent workflows. Also provides a full HTTP client for the Langfuse REST API.
> Targets .NET 8.0, 9.0, and 10.0. NuGet: zborek.LangfuseDotnet

## Quick Start
- [Installation & DI Setup](setup.md#installation): Register via AddLangfuseExporter and AddLangfuseTracing
- [Configuration](setup.md#configuration-reference): appsettings.json or programmatic setup for both OTel exporter and API client
- [Testing](setup.md#testing): AddLangfuseTracingNoOp() for unit tests
- [Error Handling](setup.md#error-handling): LangfuseApiException on API errors
- [Architecture](setup.md#architecture-notes): Namespaces, scoping, batch processing

## Tracing (OpenTelemetry)
- [IOtelLangfuseTrace](tracing.md#iotellangfusetrace--scoped-trace-service-di): Scoped trace service — StartTrace, CreateGeneration, CreateSpan, CreateToolCall, CreateEmbedding, CreateAgent, CreateEvent
- [OtelLangfuseTrace Direct](tracing.md#otellangfusetrace--direct-instantiation-no-di): Direct instantiation without DI, including detached traces for parallel work
- [Observation Types](tracing.md#observation-types): OtelSpan, OtelGeneration, OtelEmbedding, OtelToolCall, OtelAgent, OtelEvent
- [Generation Parameters](tracing.md#otelgeneration--llm-calls): SetTemperature, SetMaxTokens, SetInputMessages, SetResponse, SetUsageDetails, SetCostDetails
- [Skip Pattern](tracing.md#skip-pattern): Skip individual observations or entire traces (e.g. cache hits)
- [Key Types](tracing.md#key-types): GenAiResponse, GenAiMessage, GenAiToolDefinition, LangfuseObservationLevel

## API Client
- [ILangfuseClient](api-client.md): Full Langfuse REST API — 22 domain areas
- [Prompts](api-client.md#prompts): GetPromptAsync, CreatePromptAsync — version-controlled prompt templates
- [Datasets](api-client.md#datasets): CRUD for datasets, dataset items, and dataset run items
- [Scores](api-client.md#scores): CreateScoreAsync, GetScoreListAsync — evaluation scoring
- [Traces & Observations](api-client.md#traces): Query and manage traces and observations
- [Sessions](api-client.md#sessions): Group traces into user sessions
- [All API Domains](api-client.md#annotation-queues): Comments, Models, Media, Metrics, AnnotationQueues, Organization, Project, SCIM, Health, BlobStorage, LlmConnections

## API Types Reference
- [Pagination](api-types.md#pagination): PaginatedResponse<T>, ApiMetadata
- [Prompt Types](api-types.md#prompt-types): Prompt (TextPrompt/ChatPrompt), PromptMeta, CreatePromptRequest
- [Trace Types](api-types.md#trace-types): TraceModel, TraceWithDetails (with observations, scores, cost, latency), TraceListRequest
- [Observation Types](api-types.md#observation-types): ObservationModel with usage, cost, model details, TTFT
- [Dataset Types](api-types.md#dataset-types): DatasetModel, DatasetItem, DatasetRun, DatasetRunItem, DatasetRunWithItems
- [Score Types](api-types.md#score-types): ScoreModel (Numeric/Boolean/Categorical/Correction), ScoreConfig, ScoreCreateRequest
- [Session Types](api-types.md#session-types): SessionModel with traces
- [Comment Types](api-types.md#comment-types): CommentModel for traces, observations, sessions, prompts
- [Media Types](api-types.md#media-types): MediaModel, presigned upload/download URLs
- [Model Types](api-types.md#model-types-llm-model-definitions): LLM model definitions with pricing and tokenizer config
- [Annotation Queue Types](api-types.md#annotation-queue-types): AnnotationQueueModel, AnnotationQueueItem
- [Organization & Project](api-types.md#organization--project-types): Organization, ProjectModel, MembershipResponse, ApiKeyResponse
- [LLM Connection Types](api-types.md#llm-connection-types): LlmConnection, LlmAdapter (Anthropic/OpenAI/Azure/Bedrock/Google)
- [Blob Storage](api-types.md#blob-storage-integration-types): BlobStorageIntegrationResponse, export configuration
- [Health](api-types.md#health-types): HealthResponse with version and status
