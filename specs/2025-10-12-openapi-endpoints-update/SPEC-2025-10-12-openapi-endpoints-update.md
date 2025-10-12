# Specification: OpenAPI Endpoints Update - New API Support

**Date:** 2025-10-12
**Status:** Pending Approval
**Author:** Claude Code (Automated Analysis)

---

## 1. Overview

### Task Name
Implement New Langfuse API Endpoints based on OpenAPI Specification Updates

### Purpose and Goals
The OpenAPI specification file (`langfuse-openapi.yml`) has been updated with several new endpoints and schemas. This specification outlines the implementation plan for adding support for these new endpoints in the C# Langfuse SDK.

The primary goals are:
- Implement missing API endpoints to achieve feature parity with the Langfuse API
- Add support for new resource types (BlobStorageIntegrations, LlmConnections, etc.)
- Update existing endpoint implementations with new parameters and functionality
- Maintain consistency with the existing SDK architecture and patterns

### Related Issues
- Based on staged changes in `langfuse-openapi.yml`
- Relates to ongoing API coverage tracking mentioned in CLAUDE.md

---

## 2. Requirements

### Functional Requirements

#### New Endpoints to Implement:

**Annotation Queues:**
1. `POST /api/public/annotation-queues` - Create annotation queue
2. `POST /api/public/annotation-queues/{queueId}/assignments` - Create queue assignment
3. `DELETE /api/public/annotation-queues/{queueId}/assignments` - Delete queue assignment

**Blob Storage Integrations (New Feature):**
4. `GET /api/public/integrations/blob-storage` - Get all blob storage integrations
5. `PUT /api/public/integrations/blob-storage` - Create/update blob storage integration
6. `DELETE /api/public/integrations/blob-storage/{id}` - Delete blob storage integration

**LLM Connections (New Feature):**
7. `GET /api/public/llm-connections` - List LLM connections
8. `PUT /api/public/llm-connections` - Upsert LLM connection

**Organization Management:**
9. `DELETE /api/public/organizations/memberships` - Delete organization membership
10. `DELETE /api/public/projects/{projectId}/memberships` - Delete project membership

**Score Configs:**
11. `PATCH /api/public/score-configs/{configId}` - Update score config

#### Updated Endpoints:

**Ingestion API:**
- Update description to mark as legacy endpoint
- Add reference to OpenTelemetry endpoint

**Observation Types:**
- Add new observation types: AGENT, TOOL, CHAIN, RETRIEVER, EVALUATOR, EMBEDDING, GUARDRAIL

**Traces API:**
- Add new `filter` query parameter with JSON-based filtering
- Add new `sessionId` parameter to scores endpoint
- Update `fields` parameter documentation

### Non-Functional Requirements

1. **Consistency:** Follow existing partial class pattern (`LangfuseClient.*.cs`)
2. **Authentication:** Use existing BasicAuth security handler
3. **Error Handling:** Implement consistent error handling with `LangfuseApiException`
4. **Serialization:** Use camelCase JSON serialization with kebab-case-upper enums
5. **Nullability:** Follow nullable reference types pattern (.NET 9.0)
6. **Testing:** Unit tests with xUnit and NSubstitute
7. **Documentation:** XML documentation comments for public APIs

---

## 3. Proposed Changes

### Files to be Created

**New Partial Client Classes:**
1. `src/Langfuse/Client/LangfuseClient.BlobStorageIntegrations.cs` - New domain
2. `src/Langfuse/Client/LangfuseClient.LlmConnections.cs` - New domain

**New Model Classes:**
1. `src/Langfuse/Models/BlobStorageIntegration/`
   - `BlobStorageIntegrationType.cs`
   - `BlobStorageIntegrationFileType.cs`
   - `BlobStorageExportMode.cs`
   - `BlobStorageExportFrequency.cs`
   - `CreateBlobStorageIntegrationRequest.cs`
   - `BlobStorageIntegrationResponse.cs`
   - `BlobStorageIntegrationsResponse.cs`
   - `BlobStorageIntegrationDeletionResponse.cs`

2. `src/Langfuse/Models/LlmConnection/`
   - `LlmConnection.cs`
   - `PaginatedLlmConnections.cs`
   - `UpsertLlmConnectionRequest.cs`
   - `LlmAdapter.cs`

3. `src/Langfuse/Models/Membership/`
   - `DeleteMembershipRequest.cs`
   - `MembershipDeletionResponse.cs`

4. `src/Langfuse/Models/AnnotationQueue/`
   - `CreateAnnotationQueueRequest.cs`
   - `AnnotationQueueAssignmentRequest.cs`
   - `DeleteAnnotationQueueAssignmentResponse.cs`
   - `CreateAnnotationQueueAssignmentResponse.cs`

5. `src/Langfuse/Models/ScoreConfig/`
   - `UpdateScoreConfigRequest.cs`

6. `src/Langfuse/Models/Trace/`
   - `TraceFilterCondition.cs` (for new filter parameter)

**New Test Files:**
1. `tests/Langfuse.Tests/Client/LangfuseClientBlobStorageIntegrationsTests.cs`
2. `tests/Langfuse.Tests/Client/LangfuseClientLlmConnectionsTests.cs`
3. `tests/Langfuse.Tests/Models/BlobStorageIntegrationTests.cs`
4. `tests/Langfuse.Tests/Models/LlmConnectionTests.cs`

### Files to be Modified

**Existing Client Files:**
1. `src/Langfuse/Client/LangfuseClient.AnnotationQueue.cs` - Add create, assignment methods
2. `src/Langfuse/Client/LangfuseClient.Organization.cs` - Add delete membership methods
3. `src/Langfuse/Client/LangfuseClient.Score.cs` - Add update score config method, add sessionId parameter
4. `src/Langfuse/Client/LangfuseClient.Observation.cs` - Update observation types enum
5. `src/Langfuse/Client/LangfuseClient.Trace.cs` - Add filter parameter support

**Existing Model Files:**
1. `src/Langfuse/Models/Observation/ObservationType.cs` - Add new types
2. `src/Langfuse/Models/Trace/GetTracesRequest.cs` - Add filter parameter
3. `src/Langfuse/Models/Score/GetScoresRequest.cs` - Add sessionId parameter

### Components Affected
- LangfuseClient (core HTTP client)
- Model classes for request/response DTOs
- Unit tests for new functionality
- Potentially background service if new endpoints use batch mode

---

## 4. Technical Approach

### Implementation Strategy

**Phase 1: Model Classes**
Create all new model classes following existing patterns:
- Use `record` types for immutable DTOs where appropriate
- Implement proper JSON serialization attributes
- Add XML documentation comments
- Ensure nullable reference type annotations are correct

**Phase 2: Client Methods**
Implement client methods following the partial class pattern:
- Each domain gets its own partial class file
- Use existing HTTP methods (`GetAsync`, `PostAsync`, `PutAsync`, `DeleteAsync`)
- Return appropriate response types
- Handle pagination for list endpoints

**Phase 3: Enhancements**
Update existing endpoints with new parameters and functionality:
- Add filter parameter support to Traces API
- Update observation types enum
- Add sessionId parameter to scores endpoint

**Phase 4: Testing**
Write comprehensive unit tests:
- Test request serialization
- Test response deserialization
- Test error handling
- Mock HTTP responses using NSubstitute

### Technology Stack
- .NET 9.0
- System.Text.Json for serialization
- HttpClient for HTTP communication
- xUnit for testing
- NSubstitute for mocking

### API Endpoints Detail

#### Blob Storage Integrations
```csharp
// GET /api/public/integrations/blob-storage
Task<BlobStorageIntegrationsResponse> GetBlobStorageIntegrationsAsync(CancellationToken cancellationToken = default);

// PUT /api/public/integrations/blob-storage
Task<BlobStorageIntegrationResponse> UpsertBlobStorageIntegrationAsync(
    CreateBlobStorageIntegrationRequest request,
    CancellationToken cancellationToken = default);

// DELETE /api/public/integrations/blob-storage/{id}
Task<BlobStorageIntegrationDeletionResponse> DeleteBlobStorageIntegrationAsync(
    string id,
    CancellationToken cancellationToken = default);
```

#### LLM Connections
```csharp
// GET /api/public/llm-connections
Task<PaginatedLlmConnections> GetLlmConnectionsAsync(
    int? page = null,
    int? limit = null,
    CancellationToken cancellationToken = default);

// PUT /api/public/llm-connections
Task<LlmConnection> UpsertLlmConnectionAsync(
    UpsertLlmConnectionRequest request,
    CancellationToken cancellationToken = default);
```

#### Annotation Queue Enhancements
```csharp
// POST /api/public/annotation-queues
Task<AnnotationQueue> CreateAnnotationQueueAsync(
    CreateAnnotationQueueRequest request,
    CancellationToken cancellationToken = default);

// POST /api/public/annotation-queues/{queueId}/assignments
Task<CreateAnnotationQueueAssignmentResponse> CreateQueueAssignmentAsync(
    string queueId,
    AnnotationQueueAssignmentRequest request,
    CancellationToken cancellationToken = default);

// DELETE /api/public/annotation-queues/{queueId}/assignments
Task<DeleteAnnotationQueueAssignmentResponse> DeleteQueueAssignmentAsync(
    string queueId,
    AnnotationQueueAssignmentRequest request,
    CancellationToken cancellationToken = default);
```

#### Organization Management
```csharp
// DELETE /api/public/organizations/memberships
Task<MembershipDeletionResponse> DeleteOrganizationMembershipAsync(
    DeleteMembershipRequest request,
    CancellationToken cancellationToken = default);

// DELETE /api/public/projects/{projectId}/memberships
Task<MembershipDeletionResponse> DeleteProjectMembershipAsync(
    string projectId,
    DeleteMembershipRequest request,
    CancellationToken cancellationToken = default);
```

#### Score Configs
```csharp
// PATCH /api/public/score-configs/{configId}
Task<ScoreConfig> UpdateScoreConfigAsync(
    string configId,
    UpdateScoreConfigRequest request,
    CancellationToken cancellationToken = default);
```

### Data Models

#### Key Enums
```csharp
public enum BlobStorageIntegrationType
{
    S3,
    S3Compatible,
    AzureBlobStorage
}

public enum BlobStorageIntegrationFileType
{
    Json,
    Csv,
    Jsonl
}

public enum BlobStorageExportMode
{
    FullHistory,
    FromToday,
    FromCustomDate
}

public enum BlobStorageExportFrequency
{
    Hourly,
    Daily,
    Weekly
}

public enum LlmAdapter
{
    Anthropic,
    OpenAi,
    Azure,
    Bedrock,
    GoogleVertexAi,
    GoogleAiStudio
}
```

#### Observation Type Update
```csharp
public enum ObservationType
{
    Span,
    Generation,
    Event,
    Agent,      // NEW
    Tool,       // NEW
    Chain,      // NEW
    Retriever,  // NEW
    Evaluator,  // NEW
    Embedding,  // NEW
    Guardrail   // NEW
}
```

---

## 5. Implementation Plan

### Step-by-step Breakdown

**Step 1: Blob Storage Integration Models** (Medium complexity)
- Create enum types
- Create request/response models
- Add JSON serialization attributes
- Add XML documentation

**Step 2: Blob Storage Integration Client** (Medium complexity)
- Create `LangfuseClient.BlobStorageIntegrations.cs`
- Implement GET, PUT, DELETE methods
- Handle query parameters and path parameters

**Step 3: LLM Connection Models** (Medium complexity)
- Create LlmAdapter enum
- Create request/response models
- Implement pagination response model

**Step 4: LLM Connection Client** (Medium complexity)
- Create `LangfuseClient.LlmConnections.cs`
- Implement GET (with pagination) and PUT methods

**Step 5: Annotation Queue Enhancements** (Low complexity)
- Add CreateAnnotationQueueRequest model
- Add assignment-related models
- Update existing client with new methods

**Step 6: Organization Management** (Low complexity)
- Add DeleteMembershipRequest model
- Add MembershipDeletionResponse model
- Update organization client with delete methods

**Step 7: Score Config Updates** (Low complexity)
- Add UpdateScoreConfigRequest model
- Update score client with PATCH method
- Add sessionId parameter to GetScoresAsync

**Step 8: Observation Type Updates** (Low complexity)
- Update ObservationType enum with new values
- Ensure serialization works correctly

**Step 9: Trace Filter Support** (High complexity)
- Design TraceFilterCondition model
- Support JSON serialization of complex filter structure
- Update GetTracesAsync to accept filter parameter
- Update documentation

**Step 10: Testing - Blob Storage** (Medium complexity)
- Write unit tests for blob storage models
- Write unit tests for blob storage client methods
- Test all HTTP methods and error cases

**Step 11: Testing - LLM Connections** (Medium complexity)
- Write unit tests for LLM connection models
- Write unit tests for LLM connection client methods

**Step 12: Testing - Other Features** (Low complexity)
- Test annotation queue enhancements
- Test organization membership deletion
- Test score config updates
- Test observation type updates

**Step 13: Integration Testing** (Medium complexity)
- Create example application demonstrating new features
- Test against staging/test Langfuse instance if available
- Verify serialization/deserialization works end-to-end

**Step 14: Documentation** (Low complexity)
- Update CLAUDE.md if needed
- Add XML documentation for all public APIs
- Consider adding examples to Example projects

### Order of Implementation
1. Blob Storage (Steps 1-2) - Most complex new feature
2. LLM Connections (Steps 3-4) - Second most complex
3. Simple enhancements (Steps 5-8) - Quick wins
4. Complex filter support (Step 9) - Most challenging
5. Testing (Steps 10-12) - Ensure quality
6. Integration & docs (Steps 13-14) - Final polish

---

## 6. Testing Strategy

### Unit Tests Needed

**Blob Storage Integration:**
- Test CreateBlobStorageIntegrationRequest serialization with all fields
- Test BlobStorageIntegrationResponse deserialization
- Test enum serialization (kebab-case-upper pattern)
- Test null handling for optional fields
- Mock GET/PUT/DELETE HTTP calls
- Test error responses (400, 401, 403, 404)

**LLM Connections:**
- Test UpsertLlmConnectionRequest with secretKey handling
- Test LlmConnection response with display fields
- Test pagination parameters
- Test enum values for LlmAdapter
- Mock HTTP responses

**Annotation Queues:**
- Test CreateAnnotationQueueRequest validation
- Test assignment creation/deletion
- Test queueId path parameter

**Organization:**
- Test membership deletion request/response
- Test both organization and project-level deletion

**Score Configs:**
- Test partial update with UpdateScoreConfigRequest
- Test nullable field handling
- Test sessionId parameter in GetScoresAsync

**Traces:**
- Test filter parameter JSON serialization
- Test complex filter conditions with different operators
- Test filter precedence over legacy parameters

### Integration Tests Needed
- End-to-end test creating and managing blob storage integration
- End-to-end test creating and listing LLM connections
- Test authentication with organization-scoped API keys
- Test pagination behavior

### Edge Cases to Consider
1. **Blob Storage:**
   - Prefix must end with "/" if provided
   - exportStartDate required when exportMode is FROM_CUSTOM_DATE
   - secretAccessKey encryption/display
   - S3_COMPATIBLE requires endpoint

2. **LLM Connections:**
   - Provider must be unique per project
   - secretKey should never be returned in responses
   - customModels vs withDefaultModels interaction

3. **Filters:**
   - Empty filter array
   - Invalid operator for type
   - Missing required key for stringObject/numberObject
   - Filter superseding legacy parameters

4. **Pagination:**
   - Page starts at 1, not 0
   - Handle empty results
   - Handle invalid page numbers

### Manual Testing Steps
1. Create organization-scoped API key
2. Test blob storage integration CRUD operations
3. Test LLM connection upsert and list
4. Test annotation queue creation and assignment
5. Test organization membership deletion
6. Test trace filtering with various conditions
7. Verify error messages are clear and helpful

---

## 7. Dependencies

### External Libraries/Packages
- No new external dependencies required
- Uses existing System.Text.Json for serialization
- Uses existing HttpClient infrastructure

### Other Features/Modules
- Depends on existing `LangfuseClient` base class
- Depends on existing authentication handler (`AuthorizationDelegatingHandler`)
- Depends on existing error handling (`LangfuseApiException`)
- Depends on existing pagination utilities (`utilsMetaResponse`)

### Backwards Compatibility Considerations
1. **Additive Changes:** All new endpoints are additions, no breaking changes to existing APIs
2. **Enum Extensions:** New observation types added to existing enum - binary compatible
3. **New Parameters:** Filter parameter added to traces - optional, maintains backwards compatibility
4. **Legacy Marking:** Ingestion endpoint marked as legacy but still functional
5. **Version Compatibility:** Should target .NET 9.0 as specified in project

**Breaking Change Analysis:**
- ✅ No method signature changes to existing methods
- ✅ All new parameters are optional
- ✅ Enum additions are backwards compatible
- ✅ New model classes don't affect existing code
- ✅ Safe to deploy as minor version update (0.3.x → 0.4.0)

---

## 8. Potential Risks/Challenges

### Known Challenges

1. **Complex Filter Structure**
   - The trace filter parameter has a complex JSON structure with type-dependent operators
   - Need to design intuitive C# API for building filter conditions
   - Risk: Poor API design could make filters hard to use
   - Mitigation: Consider fluent builder pattern or strongly-typed filter classes

2. **Organization-Scoped Authentication**
   - Some endpoints require organization-scoped API keys vs project-scoped
   - Current SDK may not differentiate between these
   - Risk: Confusing errors if wrong key type is used
   - Mitigation: Clear documentation and helpful error messages

3. **Sensitive Data Handling**
   - Blob storage secretAccessKey must be encrypted when stored
   - LLM connection secretKey should never be returned in responses
   - Risk: Accidental exposure of secrets
   - Mitigation: Clear naming (displaySecretKey vs secretKey), documentation

4. **Enum Serialization**
   - Project uses kebab-case-upper for enums (e.g., "S3_COMPATIBLE")
   - Must ensure new enums follow this pattern
   - Risk: Inconsistent serialization breaks API compatibility
   - Mitigation: Unit tests for enum serialization, use consistent JSON attributes

5. **API Endpoint Validation**
   - No access to live Langfuse API for testing
   - Relying solely on OpenAPI spec
   - Risk: Spec may not match actual API behavior
   - Mitigation: Comprehensive unit tests, rely on community feedback

6. **Large Diff Complexity**
   - 1141 lines of changes to process
   - Risk: Missing subtle requirements or relationships
   - Mitigation: Systematic review of each endpoint, careful testing

### Mitigation Strategies

1. **Filter API Design:**
   ```csharp
   // Option 1: Fluent builder
   var filter = new TraceFilterBuilder()
       .Where("userId", StringOperator.Equals, "user123")
       .And("timestamp", DateTimeOperator.GreaterThan, DateTime.Now.AddDays(-7))
       .Build();

   // Option 2: Strongly-typed classes
   var filter = new TraceFilter
   {
       Conditions = new[]
       {
           new StringFilter("userId", StringOperator.Equals, "user123"),
           new DateTimeFilter("timestamp", DateTimeOperator.GreaterThan, DateTime.Now.AddDays(-7))
       }
   };
   ```

2. **Authentication Clarity:**
   - Add XML comments noting which endpoints require organization vs project scope
   - Consider adding organizationScoped parameter or separate client methods

3. **Secret Handling:**
   - Use clear naming: `SecretAccessKey` for input, `DisplaySecretKey` for output
   - Add XML comments warning about sensitive data
   - Never log or serialize secrets

4. **Testing Rigor:**
   - Write tests before implementation where possible
   - Test all enum values serialize correctly
   - Test null handling extensively
   - Use OpenAPI spec as source of truth

---

## 9. Acceptance Criteria

### Definition of Done

**Code Complete:**
- [ ] All 11 new endpoints implemented
- [ ] All new model classes created
- [ ] All existing endpoints updated with new parameters
- [ ] ObservationType enum updated with 7 new types
- [ ] Filter parameter support added to Traces API
- [ ] Code follows existing patterns and conventions
- [ ] All public APIs have XML documentation comments

**Testing Complete:**
- [ ] Unit tests written for all new client methods
- [ ] Unit tests written for all new model classes
- [ ] Enum serialization tests pass
- [ ] Pagination tests pass
- [ ] Error handling tests pass
- [ ] All tests pass with > 80% code coverage for new code

**Quality Assurance:**
- [ ] Code builds without warnings
- [ ] No nullable reference type warnings
- [ ] JSON serialization follows camelCase convention
- [ ] Enums follow kebab-case-upper convention
- [ ] Error messages are clear and helpful
- [ ] No secrets logged or exposed

**Documentation:**
- [ ] XML comments for all public APIs
- [ ] CLAUDE.md updated if needed
- [ ] Breaking changes documented (none expected)
- [ ] Examples added for complex features (filters, blob storage)

**Integration:**
- [ ] New code integrates with existing client architecture
- [ ] Partial class pattern maintained
- [ ] Authentication handler works with new endpoints
- [ ] Background service compatible (if applicable)
- [ ] No breaking changes to existing APIs

### Clear Criteria for Completion

**Must Have:**
1. All 11 new endpoints functional and tested
2. All model classes properly serializing/deserializing
3. Unit tests passing
4. No breaking changes
5. Code review approved

**Should Have:**
1. Integration test or example application
2. Comprehensive XML documentation
3. Filter builder pattern for trace filters
4. Clear organization vs project scope documentation

**Nice to Have:**
1. Example console application demonstrating new features
2. Migration guide from legacy ingestion to OpenTelemetry
3. Performance benchmarks for filter queries
4. Visual Studio IntelliSense improvements

---

## 10. Rollback Plan

### How to Revert if Something Goes Wrong

**Pre-Implementation Safety:**
1. All work done in feature branch: `feature/update` (already exists)
2. Staged changes reviewed before commit
3. No changes to existing method signatures (only additions)

**During Implementation:**
1. Commit after each logical unit (e.g., blob storage models, then client)
2. Run tests before committing
3. Keep main branch stable - only merge when complete

**Post-Implementation Rollback:**

**If Issues Found Before Release:**
```bash
# Option 1: Revert specific commits
git revert <commit-hash>

# Option 2: Reset feature branch
git reset --hard <last-good-commit>
```

**If Issues Found After Release:**
1. **Minor Issues:** Patch release (0.4.1) with fixes
2. **Major Issues:** Revert to previous version (0.3.x)
   - Remove new endpoints from code
   - Mark version as deprecated on NuGet
   - Publish rollback version (0.4.1 or 0.5.0)

**Database/State Considerations:**
- SDK is stateless - no database migrations needed
- Only HTTP calls affected
- No user data at risk

**Communication Plan:**
- If rollback needed: Update GitHub releases with notice
- Post issue on GitHub explaining problem
- Provide timeline for fix
- Recommend users stay on 0.3.x until fixed

**Testing Before Rollback:**
- Verify previous version (0.3.x) still works
- Test that removing new code doesn't break existing functionality
- Ensure NuGet package can be republished

**Minimal Risk Strategy:**
Since all changes are additive:
- Existing users won't be affected (they won't use new endpoints)
- New users can simply not use problematic endpoints
- Can deploy incrementally: core features first, complex filters later

---

## Summary

This specification outlines the implementation of 11 new API endpoints and several enhancements to existing endpoints in the Langfuse C# SDK. The changes introduce two entirely new API domains (Blob Storage Integrations and LLM Connections), enhance existing domains (Annotation Queues, Organizations, Score Configs), and add advanced filtering capabilities to the Traces API.

The implementation follows established patterns in the codebase, maintains backwards compatibility, and introduces no breaking changes. The work is structured into 14 logical steps with a clear testing strategy and acceptance criteria.

**Key Highlights:**
- 2 new API domains: Blob Storage Integrations, LLM Connections
- 11 new endpoints total
- 7 new observation types
- Advanced JSON-based filtering for traces
- Comprehensive model classes with proper serialization
- Full unit test coverage
- Maintains backwards compatibility

**Estimated Effort:** Medium-Large (3-5 days of development + testing)

**Risk Level:** Medium (complex filter API design, no live API for testing)

---

## Next Steps

Please review this specification and provide feedback on:
1. Overall approach and architecture
2. Filter API design preference (builder vs strongly-typed)
3. Testing strategy adequacy
4. Any missing requirements or edge cases
5. Priority/ordering of implementation steps

Once approved, implementation can begin following the step-by-step plan outlined in Section 5.
