# LangfuseDotnet — API Client

> ILangfuseClient provides full access to the Langfuse REST API.
> All methods accept an optional `CancellationToken`.
> See [api-types.md](api-types.md) for request/response type definitions.

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
GetDatasetRunAsync(datasetName, runName) → DatasetRunWithItems
DeleteDatasetRunAsync(datasetName, runName) → DeleteDatasetRunResponse
GetDatasetRunsAsync(datasetName, DatasetRunListRequest) → PaginatedDatasetRuns
```

## Dataset Items

```
GetDatasetItemListAsync(datasetName, DatasetItemListRequest) → PaginatedDatasetItems
GetDatasetItemAsync(id) → DatasetItem
CreateDatasetItemAsync(CreateDatasetItemRequest) → DatasetItem
UpdateDatasetItemAsync(id, CreateDatasetItemRequest) → DatasetItem
DeleteDatasetItemAsync(id) → DeleteDatasetItemResponse
```

## Dataset Run Items

```
GetDatasetRunItemListAsync(datasetName, runName, DatasetRunItemListRequest) → PaginatedDatasetRunItems
CreateDatasetRunItemAsync(CreateDatasetRunItemRequest) → DatasetRunItemListRequest
```

## Scores

```
GetScoreListAsync(ScoreListRequest?) → ScoreListResponse
GetScoreAsync(id) → ScoreModel
CreateScoreAsync(ScoreCreateRequest) → CreateScoreResponse
DeleteScoreAsync(id)
```

Example:
```csharp
await _langfuseClient.CreateScoreAsync(new ScoreCreateRequest
{
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
GetTraceListAsync(TraceListRequest?) → TraceListResponse
GetTraceAsync(id) → TraceWithDetails
DeleteTraceAsync(id) → DeleteTraceResponse
DeleteTraceManyAsync(DeleteTraceManyRequest) → DeleteTraceResponse
```

## Observations

```
GetObservationListAsync(ObservationListRequest?) → ObservationListResponse
GetObservationAsync(id) → ObservationModel
GetObservationsV2Async(ObservationsV2Request) → ObservationsV2Response
```

## Sessions

```
GetSessionListAsync(SessionListRequest?) → SessionListResponse
GetSessionAsync(id) → Session
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
GetMetricsAsync(MetricsRequest) → MetricsResponse
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
GetLlmConnectionListAsync() → PaginatedLlmConnections
UpsertLlmConnectionAsync(id, UpsertLlmConnectionRequest) → LlmConnection
```

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
