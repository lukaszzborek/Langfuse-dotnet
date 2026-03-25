# LangfuseDotnet — API Types Reference

> All request, response, and model types used by ILangfuseClient.
> See [api-client.md](api-client.md) for method signatures.

## Pagination

All paginated responses inherit from `PaginatedResponse<T>`:
```
PaginatedResponse<T>
  Data: T[]              // Items in this page
  Meta: ApiMetadata      // Pagination info

ApiMetadata
  Page: int              // Current page (1-based)
  Limit: int             // Items per page
  TotalItems: int        // Total across all pages
  TotalPages: int        // Total pages
```

---

## Prompt Types

```
Prompt (abstract, polymorphic on Type: "text" or "chat")
  Name: string               // Prompt name
  Version: int               // Version number
  Config: object?             // Configuration object
  Labels: List<string>        // Categorization labels
  Tags: List<string>          // Metadata tags
  CommitMessage: string?      // Version description

TextPrompt : Prompt
  Type: "text"
  PromptText: string          // Text content of prompt

ChatPrompt : Prompt
  Type: "chat"
  PromptMessages: List<ChatMessageWithPlaceholders>  // Chat messages

ChatMessage : ChatMessageWithPlaceholders
  Role: string               // system, user, assistant, etc.
  Content: string            // Message content

PlaceholderMessage : ChatMessageWithPlaceholders
  Name: string               // Placeholder variable name

PromptMeta
  Name: string               // Prompt name
  Type: PromptType           // Text or Chat
  Versions: List<int>        // Available version numbers
  Tags: List<string>
  Labels: List<string>
  LastUpdatedAt: DateTime
  LastConfig: object?

PromptType (enum): Text, Chat
```

### Prompt Request Types

```
CreatePromptRequest (abstract)
  Type: PromptType?          // Text or Chat
  Name: string               // Prompt name (required)
  Config: object?             // Configuration
  Labels: List<string>        // Labels
  Tags: List<string>          // Tags
  CommitMessage: string?      // Commit message

PromptListRequest
  Page: int?
  Limit: int?
  Name: string?              // Filter by name
  Tag: string?               // Filter by tag
  Label: string?             // Filter by label
```

---

## Trace Types

```
TraceModel
  Id: string                 // Trace ID
  Timestamp: DateTime        // Creation timestamp
  Name: string?              // Trace name
  Input: object?             // Input data
  Output: object?            // Output data
  SessionId: string?         // Associated session
  Release: string?           // Application release
  Version: string?           // Version info
  UserId: string?            // Associated user
  Metadata: object?          // Custom metadata
  Tags: string[]             // Categorization tags
  Public: bool               // Public visibility
  Environment: string        // Environment (prod/staging/dev)

TraceWithDetails : TraceModel
  Observations: ObservationModel[]?  // Nested observations
  Scores: object[]?                  // Associated scores
  Cost: decimal?                     // Cost info
  Usage: TraceUsage?                 // Token usage
  Latency: double?                   // Execution latency (seconds)
  TotalCost: double?                 // Total cost in USD

TraceUsage
  PromptTokens: int?         // Input tokens
  CompletionTokens: int?     // Output tokens
  TotalTokens: int?          // Total tokens
```

### Trace Request Types

```
TraceListRequest
  Page: int?
  Limit: int?
  UserId: string?            // Filter by user
  Name: string?              // Filter by name
  SessionId: string?         // Filter by session
  FromTimestamp: DateTime?    // Min timestamp
  ToTimestamp: DateTime?      // Max timestamp
  OrderBy: string?           // Sort field and direction
  Tags: string[]?            // Filter by tags
  Version: string?           // Filter by version
  Release: string?           // Filter by release
  Environment: string?       // Filter by environment

DeleteTraceManyRequest
  TraceIds: string[]         // Trace IDs to delete
```

---

## Observation Types

```
ObservationModel
  Id: string                 // Observation ID
  Name: string?              // Observation name
  Type: string               // SPAN, GENERATION, EVENT, etc.
  TraceId: string?           // Parent trace
  ParentObservationId: string?  // Parent observation
  StartTime: DateTime        // Start timestamp
  EndTime: DateTime?         // End timestamp
  CompletionStartTime: DateTime?  // Streaming start (TTFT)
  Input: object?             // Input data
  Output: object?            // Output data
  Metadata: object?          // Custom metadata
  Level: LangfuseLogLevel    // Log level
  StatusMessage: string?     // Status description
  Version: string?           // Version info
  Model: string?             // AI model name (generations)
  ModelParameters: object?   // Model config
  Usage: Usage?              // Token usage
  UsageDetails: Dictionary<string, int>   // Detailed usage
  CostDetails: Dictionary<string, double> // Detailed costs
  PromptId: string?          // Associated prompt ID
  PromptName: string?        // Associated prompt name
  PromptVersion: int?        // Prompt version
  InputPrice: double?        // Input price per unit
  OutputPrice: double?       // Output price per unit
  TotalPrice: double?        // Total price per unit
  Latency: double?           // Execution latency (seconds)
  TimeToFirstToken: double?  // TTFT (streaming)
  Environment: string        // Environment context
  CreatedAt: DateTime
  UpdatedAt: DateTime

Usage
  Input: int                 // Input tokens
  Output: int                // Output tokens
  Total: int                 // Total tokens
  Unit: string?              // Unit of measurement
  InputCost: double?         // Input cost USD
  OutputCost: double?        // Output cost USD
  TotalCost: double?         // Total cost USD

ObservationType (enum): Span, Generation, Event, Agent, Tool, Chain,
                         Retriever, Evaluator, Embedding, Guardrail
```

### Observation Request Types

```
ObservationListRequest
  Page: int?
  Limit: int?
  Name: string?              // Filter by name
  UserId: string?            // Filter by user
  Type: string?              // Filter by type
  TraceId: string?           // Filter by trace
  Level: LangfuseLogLevel?   // Filter by level
  ParentObservationId: string?  // Filter by parent
  Environment: string[]?     // Filter by environment
  FromStartTime: DateTime?   // Min start time
  ToStartTime: DateTime?     // Max start time
  Version: string?           // Filter by version
```

---

## Dataset Types

```
DatasetModel
  Id: string                 // Dataset ID
  Name: string               // Dataset name
  Description: string?       // Optional description
  ProjectId: string          // Associated project
  Metadata: object?          // Custom metadata
  InputSchema: object?       // JSON schema for inputs
  ExpectedOutputSchema: object?  // JSON schema for outputs
  CreatedAt: DateTime
  UpdatedAt: DateTime

DatasetItem
  Id: string                 // Item ID
  Status: DatasetStatus      // Active or Archived
  Input: object?             // Input data
  ExpectedOutput: object?    // Expected output
  Metadata: object?          // Custom metadata
  SourceTraceId: string?     // Original trace (if derived)
  SourceObservationId: string?  // Original observation
  DatasetId: string          // Parent dataset ID
  DatasetName: string        // Parent dataset name
  CreatedAt: DateTime
  UpdatedAt: DateTime

DatasetRun
  Id: string                 // Run ID
  Name: string               // Run name
  Description: string?
  Metadata: object?
  DatasetId: string          // Associated dataset ID
  DatasetName: string        // Associated dataset name
  CreatedAt: DateTime
  UpdatedAt: DateTime

DatasetRunItem
  Id: string                 // Run item ID
  DatasetRunId: string       // Parent run ID
  DatasetRunName: string     // Parent run name
  DatasetItemId: string      // Associated dataset item
  TraceId: string            // Execution trace
  ObservationId: string?     // Specific observation
  CreatedAt: DateTime
  UpdatedAt: DateTime

DatasetRunWithItems : DatasetRun
  DatasetRunItems: List<DatasetRunItem>  // Execution items

DatasetStatus (enum): Active, Archived
```

### Dataset Request Types

```
CreateDatasetRequest
  Name: string               // Dataset name (required)
  Description: string?
  Metadata: object?
  InputSchema: object?       // JSON schema for inputs
  ExpectedOutputSchema: object?  // JSON schema for outputs

CreateDatasetItemRequest
  DatasetName: string        // Target dataset (required)
  Input: object?             // Input data
  ExpectedOutput: object?    // Expected output
  Metadata: object?
  SourceTraceId: string?     // Source trace ID
  SourceObservationId: string?  // Source observation ID
  Id: string?                // Custom item ID (upsert key)
  Status: DatasetStatus?     // Active or Archived

CreateDatasetRunItemRequest
  RunName: string            // Dataset run name (required)
  RunDescription: string?
  Metadata: object?
  DatasetItemId: string      // Item ID (required)
  ObservationId: string?     // Observation from execution
  TraceId: string?           // Trace from execution

DatasetListRequest
  Page: int?, Limit: int?

DatasetItemListRequest
  DatasetName: string?, SourceTraceId: string?, SourceObservationId: string?
  Page: int?, Limit: int?, Version: DateTime?

DatasetRunListRequest
  Page: int?, Limit: int?
```

---

## Score Types

```
ScoreModel
  Id: string                 // Score ID
  TraceId: string?           // Associated trace
  SessionId: string?         // Associated session
  ObservationId: string?     // Associated observation
  Name: string               // Score name/type
  Value: object?             // Score value
  StringValue: string?       // String representation
  DataType: ScoreDataType    // Numeric, Boolean, Categorical, Correction
  Source: ScoreSource?       // Annotation, Api, Eval
  Comment: string?           // Optional comment
  Timestamp: DateTime
  AuthorUserId: string?      // Creator user ID
  ConfigId: string?          // Score config ID
  Metadata: object?
  DatasetRunId: string?
  QueueId: string?           // Annotation queue ID
  Environment: string
  CreatedAt: DateTime
  UpdatedAt: DateTime

ScoreDataType (enum): Numeric, Boolean, Categorical, Correction
ScoreSource (enum): Annotation, Api, Eval

ScoreConfig
  Id: string                 // Config ID
  Name: string               // Config name
  DataType: ScoreConfigDataType  // Numeric, Boolean, Categorical
  IsArchived: bool
  Description: string?
  MinValue: double?          // Min value (numeric)
  MaxValue: double?          // Max value (numeric)
  Categories: ConfigCategory[]?  // Categorical options
  CreatedAt: DateTimeOffset
  UpdatedAt: DateTimeOffset
  ProjectId: string

ConfigCategory
  Value: double              // Numeric value
  Label: string              // Human-readable label

CreateScoreResponse
  Id: string                 // Created score ID
```

### Score Request Types

```
ScoreCreateRequest
  Id: string?                // Custom score ID
  TraceId: string?           // Associated trace
  ObservationId: string?     // Associated observation
  SessionId: string?
  DatasetRunId: string?
  Name: string               // Score name (required)
  Value: object              // Score value (required)
  Comment: string?
  Metadata: object?
  Environment: string?
  DataType: ScoreDataType?   // Inferred if not provided
  ConfigId: string?
  QueueId: string?

ScoreListRequest
  Page: int?, Limit: int?
  UserId: string?, Name: string?, TraceId: string?, ObservationId: string?
  SessionId: string?, DatasetRunId: string?, ConfigId: string?, QueueId: string?
  FromTimestamp: DateTime?, ToTimestamp: DateTime?
  Environment: string[]?, Source: ScoreSource?, DataType: ScoreDataType?
  TraceTags: string[]?
  Operator: string?, Value: double?  // Comparison filter

CreateScoreConfigRequest
  Name: string               // Config name (required)
  DataType: ScoreConfigDataType  // (required)
  Description: string?
  MinValue: double?, MaxValue: double?
  Categories: ConfigCategory[]?

UpdateScoreConfigRequest
  Description: string?
  IsArchived: bool?
```

---

## Session Types

```
SessionModel
  Id: string                 // Session ID
  CreatedAt: DateTime
  ProjectId: string
  Traces: TraceModel[]?      // Session traces
  Environment: string

SessionListRequest
  Page: int?, Limit: int?
  FromTimestamp: DateTime?, ToTimestamp: DateTime?
  Environment: string[]?
```

---

## Comment Types

```
CommentModel
  Id: string                 // Comment ID
  ProjectId: string
  ObjectType: CommentObjectType  // Trace, Observation, Session, Prompt
  ObjectId: string           // ID of commented object
  Content: string            // Comment text (Markdown, max 3000 chars)
  AuthorUserId: string?
  CreatedAt: DateTime
  UpdatedAt: DateTime

CommentObjectType (enum): Trace, Observation, Session, Prompt

CreateCommentRequest
  ProjectId: string          // (required)
  ObjectType: CommentObjectType  // (required)
  ObjectId: string           // (required)
  Content: string            // (required)
  AuthorUserId: string?

CreateCommentResponse
  Id: string                 // Created comment ID
```

---

## Media Types

```
MediaModel
  Id: string                 // Media ID
  Filename: string           // Original filename
  ContentType: string        // MIME type
  ContentLength: long        // File size (bytes)
  Url: string?               // Access URL
  Sha256Hash: string?        // SHA-256 hash
  UploadStatus: MediaUploadStatus
  CreatedAt: DateTimeOffset
  UpdatedAt: DateTimeOffset
  ProjectId: string
  UploadedByUserId: string?

MediaUploadRequest
  TraceId: string            // Associated trace (required)
  ObservationId: string?     // Associated observation
  ContentType: string        // MIME type (required)
  ContentLength: int         // File size (required)
  Sha256Hash: string         // SHA-256 hash (required)
  Field: string              // input, output, or metadata (required)

MediaUploadResponse
  UploadUrl: string?         // Presigned URL (null if already uploaded)
  MediaId: string            // Media record ID

GetMediaResponse
  MediaId: string
  ContentType: string
  ContentLength: int
  UploadedAt: DateTimeOffset
  Url: string                // Download URL
  UrlExpiry: string          // URL expiration time
```

---

## Model Types (LLM model definitions)

```
Model
  Id: string                 // Model definition ID
  ModelName: string          // AI model name
  MatchPattern: string       // Pattern to match model names
  StartDate: DateTime?       // Effective start date
  Unit: ModelUsageUnit?      // Usage unit
  InputPrice: double?        // Input price per unit USD
  OutputPrice: double?       // Output price per unit USD
  TotalPrice: double?        // Total price per unit USD
  TokenizerId: string?
  TokenizerConfig: TokenizerConfig
  IsLangfuseManaged: bool
  CreatedAt: DateTime

ModelUsageUnit (enum): Characters, Tokens, Milliseconds, Seconds, Images, Requests

TokenizerConfig
  TokensPerName: int?        // Overhead per function name
  TokenizerModel: string?    // Tokenizer model name
  TokensPerMessage: int?     // Overhead per message

CreateModelRequest
  ModelName: string          // (required)
  MatchPattern: string       // (required)
  StartDate: DateTime?
  InputPrice: double?, OutputPrice: double?, TotalPrice: double?
  Unit: ModelUsageUnit?
  TokenizerId: string?
  TokenizerConfig: TokenizerConfig?
```

---

## Annotation Queue Types

```
AnnotationQueueModel
  Id: string                 // Queue ID
  Name: string               // Queue name
  Description: string?
  ScoreConfigIds: string[]   // Associated score configs
  CreatedAt: DateTimeOffset
  UpdatedAt: DateTimeOffset

AnnotationQueueItem
  Id: string                 // Item ID
  QueueId: string            // Parent queue ID
  ObjectId: string           // Object being annotated
  ObjectType: AnnotationObjectType  // Trace, Observation, Session
  Status: AnnotationQueueStatus     // Pending, Completed
  CompletedAt: DateTimeOffset?
  CreatedAt: DateTimeOffset
  UpdatedAt: DateTimeOffset

AnnotationObjectType (enum): Trace, Observation, Session
AnnotationQueueStatus (enum): Pending, Completed
```

---

## Organization & Project Types

```
Organization
  Id: string
  Name: string

ProjectModel
  Id: string
  Name: string
  Organization: Organization
  Metadata: object?
  RetentionDays: int?        // Data retention period

MembershipResponse
  UserId: string
  Role: MembershipRole       // Owner, Admin, Member, Viewer
  Email: string
  Name: string

MembershipRole (enum): Owner, Admin, Member, Viewer

ApiKeyResponse
  Id: string                 // API key ID
  CreatedAt: DateTime
  PublicKey: string           // Public key
  SecretKey: string           // Secret key (returned once on creation)
  DisplaySecretKey: string    // Masked secret key
  Note: string?
```

---

## LLM Connection Types

```
LlmConnection
  Id: string                 // Connection ID
  Provider: string           // Provider name
  Adapter: LlmAdapter        // Anthropic, OpenAi, Azure, etc.
  DisplaySecretKey: string   // Masked secret key
  BaseURL: string?           // Custom base URL
  CustomModels: string[]     // Custom model names
  WithDefaultModels: bool    // Include default models
  CreatedAt: DateTime
  UpdatedAt: DateTime

LlmAdapter (enum): Anthropic, OpenAi, Azure, Bedrock, GoogleVertexAi, GoogleAiStudio

UpsertLlmConnectionRequest
  Provider: string           // (required)
  Adapter: LlmAdapter        // (required)
  SecretKey: string          // (required)
  BaseURL: string?
  CustomModels: string[]?
  WithDefaultModels: bool?
  ExtraHeaders: Dictionary<string, string>?
```

---

## Blob Storage Integration Types

```
BlobStorageIntegrationResponse
  Id: string                 // Integration ID
  ProjectId: string
  Type: BlobStorageIntegrationType  // S3, GCS, Azure, etc.
  BucketName: string
  Endpoint: string?          // Custom endpoint
  Region: string
  Prefix: string             // File prefix
  ExportFrequency: BlobStorageExportFrequency
  Enabled: bool
  FileType: BlobStorageIntegrationFileType   // JSONL, Parquet, etc.
  ExportMode: BlobStorageExportMode
  ExportStartDate: DateTime?
  NextSyncAt: DateTime?
  LastSyncAt: DateTime?
  CreatedAt: DateTime
  UpdatedAt: DateTime

CreateBlobStorageIntegrationRequest
  ProjectId: string          // (required)
  Type: BlobStorageIntegrationType  // (required)
  BucketName: string         // (required)
  Region: string             // (required)
  AccessKeyId: string?
  SecretAccessKey: string?
  Endpoint: string?
  Prefix: string?
  ExportFrequency: BlobStorageExportFrequency  // (required)
  Enabled: bool              // (required)
  FileType: BlobStorageIntegrationFileType     // (required)
  ExportMode: BlobStorageExportMode            // (required)
  ExportStartDate: DateTime?
```

---

## Health Types

```
HealthResponse
  Version: string            // Langfuse server version
  Status: string             // Service status
```
