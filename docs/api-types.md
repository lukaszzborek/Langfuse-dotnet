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
DeleteTraceManyRequest
  TraceIds: string[]         // Trace IDs to delete
```

---

## Observation V2 Types

```
ObservationsV2Request
  Fields: string?            // Comma-separated field groups to return
  Limit: int?
  Cursor: string?            // Cursor from the previous page
  ExpandMetadata: string?    // Metadata keys to return untruncated
  Name: string?, UserId: string?, Type: string?
  TraceId: string?           // Filter by trace
  SessionId: string?         // Filter by session
  Level: LangfuseLogLevel?
  ParentObservationId: string?, IsRootObservation: bool?
  Environment: List<string>
  FromStartTime: DateTime?, ToStartTime: DateTime?
  Version: string?
  Filter: string?            // JSON filter conditions

ObservationsV2Response
  Data: List<Dictionary<string, object?>>  // Observations with the requested field groups
  Meta: ObservationsV2Meta   // Cursor: string? for the next page
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
  Version: DateTime?         // When this item version became current

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

DatasetListRequest
  Page: int?, Limit: int?

DatasetItemListRequest
  DatasetName: string?, SourceTraceId: string?, SourceObservationId: string?
  Page: int?, Limit: int?, Version: DateTime?
```

---

## Score Types

```
GetScoresV3Response
  Data: ScoreV3[]            // Scores (polymorphic on data type)
  Meta: GetScoresV3Meta      // Cursor pagination

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

CreateScoreBatchResponse    // Returned by CreateScoreAsync and CreateScoresAsync
  Message: string?           // "Accepted" when all scores were accepted (HTTP 202)
  Accepted: int?             // Accepted count on partial acceptance (HTTP 207)
  Rejected: int?             // Rejected count on partial acceptance (HTTP 207)
  Errors: List<CreateScoreBatchError>  // Message per rejected score
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
  Source: CreateScoreSource?  // Api or Annotation
  Timestamp: DateTimeOffset?  // Defaults to receive time; same Id + Timestamp updates a score

ScoreV3ListRequest
  Limit: int?, Cursor: string?, Fields: string?
  Id: string?, Name: string?, Source: string?, DataType: string?
  Environment: string?, ConfigId: string?, QueueId: string?, AuthorUserId: string?
  Value: string?, ValueMin: double?, ValueMax: double?
  TraceId: string?, SessionId: string?, ObservationId: string?, ExperimentId: string?
  FromTimestamp: DateTime?, ToTimestamp: DateTime?

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

## Skill Types

```
SkillMeta
  Name: string, Description: string, Tags: List<string>
  LatestVersion: int, LatestVersionCreatedAt: DateTime, LatestVersionLastUpdatedAt: DateTime
  ProductionVersion: int?    // Version that has the production label

SkillMetaListResponse
  Data: List<SkillMeta>, Meta: SkillListPagination  // ApiMetadata + HasNextPage

SkillVersion
  Id: string, Name: string, Description: string, Version: int
  Frontmatter: object?       // Parsed SKILL.md frontmatter
  Tags: List<string>         // Shared across all versions
  Labels: List<string>       // Labels on this version (e.g. production)
  CommitMessage: string?
  Files: List<SkillFile>
  ProjectId: string, CreatedBy: string, CreatedAt: DateTime, UpdatedAt: DateTime

SkillFile
  Id: string, Path: string, BlobId: string, Sha256Hash: string
  ContentType: string, ContentLength: long
  Content: string?           // Only set when GetSkillAsync is called with includeContent
```

### Skill Request Types

```
SkillListRequest
  Name: string?, Search: string?, Tag: string?
  Page: int?, Limit: int?, FromUpdatedAt: DateTimeOffset?, ToUpdatedAt: DateTimeOffset?

CreateSkillVersionRequest
  Files: List<SkillVersionFileCreateInput>
  CommitMessage: string?

SkillVersionFileCreateInput
  Path: string               // Required
  Content: string?           // New file text, or
  Sha256Hash: string?        // reference to an existing file by hash

UpdateSkillRequest
  Tags: List<string>         // Replaces tags on all versions; empty list clears them

UpdateSkillLabelsRequest
  Labels: List<string>       // Replaces labels on the version and moves them from other versions
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
