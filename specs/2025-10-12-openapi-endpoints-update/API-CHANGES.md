# API Changes Overview

## New Endpoints

### Annotation Queues (3 new)
```
POST   /api/public/annotation-queues
       └─ Create an annotation queue

POST   /api/public/annotation-queues/{queueId}/assignments
       └─ Create assignment for user to queue

DELETE /api/public/annotation-queues/{queueId}/assignments
       └─ Delete assignment for user from queue
```

### Blob Storage Integrations (3 new) ⭐ NEW DOMAIN
```
GET    /api/public/integrations/blob-storage
       └─ Get all blob storage integrations (org-scoped key required)

PUT    /api/public/integrations/blob-storage
       └─ Create or update blob storage integration (org-scoped key required)

DELETE /api/public/integrations/blob-storage/{id}
       └─ Delete blob storage integration by ID (org-scoped key required)
```

### LLM Connections (2 new) ⭐ NEW DOMAIN
```
GET    /api/public/llm-connections?page=1&limit=50
       └─ List all LLM connections in project

PUT    /api/public/llm-connections
       └─ Create or update LLM connection (upsert on provider)
```

### Organization Management (2 new)
```
DELETE /api/public/organizations/memberships
       └─ Delete membership from organization (org-scoped key required)

DELETE /api/public/projects/{projectId}/memberships
       └─ Delete membership from project (org-scoped key required)
```

### Score Configs (1 new)
```
PATCH  /api/public/score-configs/{configId}
       └─ Update score config (partial update)
```

## Updated Endpoints

### Ingestion API
```
POST   /api/public/ingestion
       ⚠️  Marked as LEGACY - use OpenTelemetry endpoint instead
       └─ /api/public/otel (recommended)
```

### Observations
```
New observation types added:
  - AGENT
  - TOOL
  - CHAIN
  - RETRIEVER
  - EVALUATOR
  - EMBEDDING
  - GUARDRAIL
```

### Traces
```
GET    /api/public/traces
       ✨ New parameter: filter (JSON-based filtering)
       ✨ Updated parameter: fields (better documentation)

Example filter:
{
  "filter": [
    {
      "type": "string",
      "column": "userId",
      "operator": "=",
      "value": "user123"
    },
    {
      "type": "datetime",
      "column": "timestamp",
      "operator": ">",
      "value": "2025-10-01T00:00:00Z"
    }
  ]
}
```

### Scores
```
GET    /api/public/v2/scores
       ✨ New parameter: sessionId
       └─ Filter scores by session ID
```

## Summary by Impact

### High Impact (New Capabilities)
- **Blob Storage Integrations** - Export data to S3/Azure
- **LLM Connections** - Configure custom LLM providers
- **Trace Filtering** - Advanced query capabilities

### Medium Impact (Enhancements)
- **Annotation Queue Management** - Create and assign queues
- **Membership Management** - Delete org/project members
- **Score Config Updates** - Modify existing configs

### Low Impact (Extensions)
- **Observation Types** - Support for more event types
- **Score Filtering** - Filter by session
- **API Documentation** - Legacy marking, better docs

## API Key Requirements

### Project-Scoped Keys (Most Endpoints)
- Annotation Queues
- LLM Connections
- Score Configs
- Traces
- Scores

### Organization-Scoped Keys (Required)
- Blob Storage Integrations (all operations)
- Organization Membership deletion
- Project Membership deletion

## Backwards Compatibility

✅ **All changes are backwards compatible:**
- New endpoints don't affect existing code
- New parameters are optional
- Enum additions are binary compatible
- Legacy endpoints still functional
- No breaking changes to existing APIs

## Migration Notes

### If using Ingestion API
```csharp
// Old (still works, but legacy)
await client.IngestAsync(events);

// New (recommended)
// Use OpenTelemetry endpoint instead
// See: https://langfuse.com/integrations/native/opentelemetry
```

### If filtering traces
```csharp
// Old (still works)
await client.GetTracesAsync(userId: "user123", fromTimestamp: date);

// New (more powerful)
var filter = new TraceFilter
{
    Conditions = new[]
    {
        new StringFilter("userId", "=", "user123"),
        new DateTimeFilter("timestamp", ">", date)
    }
};
await client.GetTracesAsync(filter: filter);
```

## Implementation Priority

1. **Phase 1** (Critical)
   - Blob Storage Integrations
   - LLM Connections

2. **Phase 2** (Important)
   - Annotation Queue enhancements
   - Membership deletion
   - Score config updates

3. **Phase 3** (Enhancement)
   - Observation types
   - Trace filtering
   - Score session filtering
