# LangfuseDotnet — API Client

> ILangfuseClient provides full access to the Langfuse REST API.
> All methods accept an optional `CancellationToken`.
> See [api-types.md](api-types.md) for request/response type definitions.
> The client targets the Langfuse v4 API. The v3 read and ingestion endpoints (traces, sessions, v1 observations and metrics, v2 scores, dataset runs) are not available. Send traces through OpenTelemetry (see [tracing.md](tracing.md)) and read data with the Observations V2, Scores V3, Metrics V2 and Experiments APIs.

## Prompts

```
GetPromptListAsync(PromptListRequest?) → PromptMetaListResponse
GetPromptAsync(name, version?) → Prompt
CreatePromptAsync(CreatePromptRequest) → Prompt
UpdatePromptAsync(name, CreatePromptRequest) → Prompt
DeletePromptAsync(name)
DeletePromptVersionAsync(name, version)
```

Example:
```csharp
var prompt = await _langfuseClient.GetPromptAsync("my-prompt", label: "production");
```

## Datasets

```
GetDatasetListAsync(DatasetListRequest) → PaginatedDatasets
GetDatasetAsync(name) → DatasetModel
CreateDatasetAsync(CreateDatasetRequest) → DatasetModel
```

## Dataset Items

```
GetDatasetItemListAsync(DatasetItemListRequest) → PaginatedDatasetItems
GetDatasetItemAsync(id) → DatasetItem
CreateDatasetItemAsync(CreateDatasetItemRequest) → DatasetItem
DeleteDatasetItemAsync(id) → DeleteDatasetItemResponse
```

`DatasetItem.Version` is the time this version of the item became current. Pass a timestamp in `DatasetItemListRequest.Version` to list items as they were at that time.

## Experiments

Experiments replace the v3 dataset runs and dataset run items.

```
GetExperimentsAsync(ExperimentListRequest) → ExperimentsResponse
GetExperimentItemsAsync(ExperimentItemListRequest) → ExperimentItemsResponse
```

## Scores

```
CreateScoreAsync(ScoreCreateRequest) → CreateScoreBatchResponse
CreateScoresAsync(IReadOnlyCollection<ScoreCreateRequest>) → CreateScoreBatchResponse
GetScoresV3Async(ScoreV3ListRequest?) → GetScoresV3Response
DeleteScoreAsync(id)
```

Scores are created asynchronously. The response has no score id: `Message` is `"Accepted"` on HTTP 202, and `Accepted`/`Rejected`/`Errors` are set on partial acceptance (HTTP 207). `CreateScoreAsync` throws `LangfuseApiException` when the server rejects its score. When `ScoreCreateRequest.Id` is null, the client sets it to a new GUID before sending, so read `request.Id` afterwards to find or delete the score. To update a score, send it again with the same `Id` and `Timestamp`.

Example:
```csharp
await _langfuseClient.CreateScoreAsync(new ScoreCreateRequest
{
    Id = "score-123",
    TraceId = "trace-123",
    Name = "accuracy",
    Value = 0.95
});
```

## Score Configs

```
GetScoreConfigListAsync(ScoreConfigListRequest?) → ScoreConfigListResponse
GetScoreConfigAsync(id) → ScoreConfig
CreateScoreConfigAsync(CreateScoreConfigRequest) → ScoreConfig
UpdateScoreConfigAsync(id, UpdateScoreConfigRequest) → ScoreConfig
DeleteScoreConfigAsync(id)
```

## Traces

```
DeleteTraceAsync(id) → DeleteTraceResponse
DeleteTraceManyAsync(DeleteTraceManyRequest) → DeleteTraceResponse
```

## Observations

Read traces, spans, generations and events through Observations V2. Filter by `TraceId` or `SessionId` to read one trace or session.

```
GetObservationsV2Async(ObservationsV2Request?) → ObservationsV2Response
```

## Comments

```
GetCommentListAsync(objectId) → GetCommentsResponse
CreateCommentAsync(CreateCommentRequest) → CreateCommentResponse
DeleteCommentAsync(id)
```

## Media

```
UploadMediaAsync(MediaUploadRequest) → MediaUploadResponse
GetMediaAsync(id) → GetMediaResponse
UpdateMediaAsync(id, MediaUpdateRequest) → MediaModel
DeleteMediaAsync(id)
```

## Models

```
GetModelListAsync(ModelListRequest?) → PaginatedModels
GetModelAsync(id) → Model
CreateModelAsync(CreateModelRequest) → Model
```

## Metrics

```
GetMetricsV2Async(MetricsV2Request) → MetricsV2Response
```

## Annotation Queues

```
GetAnnotationQueueListAsync(AnnotationQueueListRequest?) → AnnotationQueueListResponse
CreateAnnotationQueueAsync(CreateAnnotationQueueRequest) → AnnotationQueueModel
GetAnnotationQueueItemsAsync(queueId, AnnotationQueueItemListRequest) → AnnotationQueueItemListResponse
CreateAnnotationQueueItemAsync(queueId, CreateAnnotationQueueItemRequest) → AnnotationQueueItem
UpdateAnnotationQueueItemAsync(itemId, UpdateAnnotationQueueItemRequest) → AnnotationQueueItem
DeleteAnnotationQueueItemAsync(itemId) → DeleteAnnotationQueueItemResponse
AssignAnnotationQueueItemAsync(itemId, AnnotationQueueAssignmentRequest) → CreateAnnotationQueueAssignmentResponse
DeleteAnnotationQueueAssignmentAsync(assignmentId) → DeleteAnnotationQueueAssignmentResponse
```

## Blob Storage Integrations

```
GetBlobStorageIntegrationsAsync() → BlobStorageIntegrationsResponse
CreateBlobStorageIntegrationAsync(CreateBlobStorageIntegrationRequest) → BlobStorageIntegrationResponse
DeleteBlobStorageIntegrationAsync(id) → BlobStorageIntegrationDeletionResponse
```

## LLM Connections

```
GetLlmConnectionsAsync(page?, limit?) → PaginatedLlmConnections
UpsertLlmConnectionAsync(UpsertLlmConnectionRequest) → LlmConnection
DeleteLlmConnectionAsync(id) → DeleteLlmConnectionResponse
```

Organization-level connections need an organization API key:

```
GetOrganizationLlmConnectionsAsync(page?, limit?) → PaginatedLlmConnections
UpsertOrganizationLlmConnectionAsync(UpsertLlmConnectionRequest) → LlmConnection
DeleteOrganizationLlmConnectionAsync(id) → DeleteLlmConnectionResponse
```

## Skills

```
GetSkillListAsync(SkillListRequest?) → SkillMetaListResponse
GetSkillAsync(skillName, version?, label?, includeContent?) → SkillVersion
CreateSkillVersionAsync(CreateSkillVersionRequest) → SkillVersion
UpdateSkillAsync(skillName, UpdateSkillRequest) → SkillVersion
GetSkillFileContentsAsync(sha256Hashes) → SkillFileContentsResponse
UpdateSkillVersionLabelsAsync(skillName, version, UpdateSkillLabelsRequest) → SkillVersion
DeleteSkillVersionAsync(skillName, version) → DeleteSkillVersionResponse
```

`GetSkillAsync` returns the `production` label when no version or label is given. Set `includeContent` to get file text in `SkillFile.Content`.

## Organization

```
GetOrganizationAsync() → Organization
GetOrganizationProjectsAsync() → OrganizationProjectsResponse
GetApiKeysAsync() → OrganizationApiKeysResponse
CreateApiKeyAsync(CreateApiKeyRequest) → ApiKeyResponse
DeleteApiKeyAsync(id) → ApiKeyDeletionResponse
GetMembershipsAsync() → MembershipsResponse
CreateMembershipAsync(CreateMembershipRequest) → MembershipResponse
DeleteMembershipAsync(userId, DeleteMembershipRequest) → MembershipDeletionResponse
```

## Project

```
GetProjectAsync() → Project
GetProjectListAsync() → PaginatedProjects
CreateProjectAsync(CreateProjectRequest) → Project
UpdateProjectAsync(id, UpdateProjectRequest) → Project
DeleteProjectAsync(id) → ProjectDeletionResponse
```

## SCIM (User Provisioning)

```
GetScimServiceProviderConfigAsync() → ScimServiceProviderConfig
GetScimSchemasAsync() → ScimSchemasResponse
GetScimResourceTypesAsync() → ScimResourceTypesResponse
GetScimUsersAsync() → PaginatedScimUsers
CreateScimUserAsync(CreateScimUserRequest) → ScimUser
```

## Health

```
GetHealthAsync() → HealthResponse
```
