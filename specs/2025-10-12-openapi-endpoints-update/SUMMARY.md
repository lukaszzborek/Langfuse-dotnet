# Quick Summary: OpenAPI Endpoints Update

## What Changed in the OpenAPI Spec?

Based on the staged changes to `langfuse-openapi.yml`, the following updates need to be implemented:

## New Features (2)

### 1. Blob Storage Integrations (NEW DOMAIN)
- **3 endpoints:** GET, PUT, DELETE
- **Purpose:** Manage blob storage integrations for data export (S3, Azure, etc.)
- **Complexity:** Medium-High

### 2. LLM Connections (NEW DOMAIN)
- **2 endpoints:** GET (list), PUT (upsert)
- **Purpose:** Configure LLM API connections with custom adapters
- **Complexity:** Medium

## Enhanced Features (4)

### 3. Annotation Queues
- **3 new endpoints:** POST create queue, POST/DELETE assignments
- **Extends existing:** Annotation queue management

### 4. Organization Management
- **2 new endpoints:** DELETE organization membership, DELETE project membership
- **Extends existing:** Membership management

### 5. Score Configs
- **1 new endpoint:** PATCH update config
- **1 new parameter:** sessionId in GET scores

### 6. Observations
- **7 new types:** Agent, Tool, Chain, Retriever, Evaluator, Embedding, Guardrail

## Advanced Capabilities (1)

### 7. Trace Filtering
- **New parameter:** `filter` - JSON-based complex filtering
- **Replaces:** Legacy filter parameters (still supported)
- **Complexity:** High

## Implementation Stats

- **Total new endpoints:** 11
- **New model classes:** ~25
- **New enum values:** ~15
- **Updated existing endpoints:** 3
- **Estimated effort:** 3-5 days
- **Risk level:** Medium

## Files to Create

```
src/Langfuse/Client/
  - LangfuseClient.BlobStorageIntegrations.cs
  - LangfuseClient.LlmConnections.cs

src/Langfuse/Models/BlobStorageIntegration/
  - [8 new model files]

src/Langfuse/Models/LlmConnection/
  - [4 new model files]

src/Langfuse/Models/Membership/
  - [2 new model files]

src/Langfuse/Models/AnnotationQueue/
  - [4 new model files]

src/Langfuse/Models/ScoreConfig/
  - [1 new model file]

src/Langfuse/Models/Trace/
  - [1 new model file]

tests/Langfuse.Tests/
  - [4+ new test files]
```

## Files to Modify

```
src/Langfuse/Client/
  - LangfuseClient.AnnotationQueue.cs
  - LangfuseClient.Organization.cs
  - LangfuseClient.Score.cs
  - LangfuseClient.Observation.cs
  - LangfuseClient.Trace.cs

src/Langfuse/Models/
  - ObservationType.cs
  - GetTracesRequest.cs
  - GetScoresRequest.cs
```

## Key Decisions Needed

1. **Filter API Design:** Builder pattern vs strongly-typed classes?
2. **Authentication Scope:** How to document organization vs project-scoped keys?
3. **Implementation Order:** Blob storage first or LLM connections?
4. **Testing Strategy:** Mock tests only or also integration tests?

## Breaking Changes

**None!** All changes are additive and backwards compatible.

## Next Step

Review the full specification in `SPEC-2025-10-12-openapi-endpoints-update.md` and approve to begin implementation.
