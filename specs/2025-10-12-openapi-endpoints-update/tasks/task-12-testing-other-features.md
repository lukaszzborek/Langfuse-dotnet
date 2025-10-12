# Task 12: Testing - Other Features

**Task ID:** openapi-update-12
**Priority:** Low
**Dependencies:** openapi-update-05, openapi-update-06, openapi-update-07, openapi-update-08
**Approach:** Testing

## Objective

Write unit tests for the smaller enhancements: annotation queue enhancements, organization membership deletion, score config updates, and observation type updates.

## What Needs to Be Done

### Test Coverage Requirements

#### Annotation Queue Enhancement Tests

**Model Tests:**
```csharp
[Fact]
public void CreateAnnotationQueueRequest_Serializes_Correctly()
{
    var request = new CreateAnnotationQueueRequest
    {
        Name = "Quality Review Queue",
        Description = "Queue for quality reviews",
        ScoreConfigIds = new[] { "config-1", "config-2" }
    };

    var json = JsonSerializer.Serialize(request, _jsonOptions);

    Assert.Contains("name", json);
    Assert.Contains("Quality Review Queue", json);
    Assert.Contains("scoreConfigIds", json);
}

[Fact]
public void AnnotationQueueAssignmentRequest_Serializes_Correctly()
{
    var request = new AnnotationQueueAssignmentRequest
    {
        UserId = "user-123"
    };

    var json = JsonSerializer.Serialize(request, _jsonOptions);
    Assert.Contains("userId", json);
}
```

**Client Method Tests:**
```csharp
[Fact]
public async Task CreateAnnotationQueueAsync_Success()
{
    var request = new CreateAnnotationQueueRequest
    {
        Name = "Test Queue",
        ScoreConfigIds = new[] { "config-1" }
    };

    var result = await client.CreateAnnotationQueueAsync(request);

    Assert.NotNull(result);
    Assert.Equal("Test Queue", result.Name);
}

[Fact]
public async Task CreateQueueAssignmentAsync_EncodesQueueId()
{
    var queueId = "queue-123/test";
    var request = new AnnotationQueueAssignmentRequest { UserId = "user-1" };

    await client.CreateQueueAssignmentAsync(queueId, request);

    // Verify URL encoding
}

[Fact]
public async Task DeleteQueueAssignmentAsync_DeleteWithBody()
{
    // Test DELETE with request body
    var result = await client.DeleteQueueAssignmentAsync("queue-1",
        new AnnotationQueueAssignmentRequest { UserId = "user-1" });

    Assert.True(result.Success);
}
```

#### Organization Management Tests

**Model Tests:**
```csharp
[Fact]
public void DeleteMembershipRequest_Serializes_Correctly()
{
    var request = new DeleteMembershipRequest
    {
        UserId = "user-123"
    };

    var json = JsonSerializer.Serialize(request, _jsonOptions);
    Assert.Contains("userId", json);
    Assert.Contains("user-123", json);
}

[Fact]
public void MembershipDeletionResponse_Deserializes_Correctly()
{
    var json = @"{
        ""message"": ""Membership deleted successfully"",
        ""userId"": ""user-123""
    }";

    var response = JsonSerializer.Deserialize<MembershipDeletionResponse>(json, _jsonOptions);

    Assert.NotNull(response);
    Assert.Equal("user-123", response.UserId);
    Assert.Contains("success", response.Message);
}
```

**Client Method Tests:**
```csharp
[Fact]
public async Task DeleteOrganizationMembershipAsync_Success()
{
    var request = new DeleteMembershipRequest { UserId = "user-123" };

    var result = await client.DeleteOrganizationMembershipAsync(request);

    Assert.NotNull(result);
    Assert.Equal("user-123", result.UserId);
}

[Fact]
public async Task DeleteProjectMembershipAsync_EncodesProjectId()
{
    var projectId = "project-123/test";
    var request = new DeleteMembershipRequest { UserId = "user-1" };

    await client.DeleteProjectMembershipAsync(projectId, request);

    // Verify URL encoding
}

[Fact]
public async Task DeleteMembership_RequiresOrgScopedKey()
{
    // Test 403 error with project-scoped key
    // Mock 403 response
    await Assert.ThrowsAsync<HttpRequestException>(() =>
        client.DeleteOrganizationMembershipAsync(
            new DeleteMembershipRequest { UserId = "user-1" }));
}
```

#### Score Config Update Tests

**Model Tests:**
```csharp
[Fact]
public void UpdateScoreConfigRequest_Serializes_AllFields()
{
    var request = new UpdateScoreConfigRequest
    {
        Name = "Updated Score",
        Description = "New description",
        IsArchived = true,
        MinValue = 0.0,
        MaxValue = 100.0,
        Categories = new[]
        {
            new ConfigCategory { Label = "Good", Value = 1 },
            new ConfigCategory { Label = "Bad", Value = 0 }
        }
    };

    var json = JsonSerializer.Serialize(request, _jsonOptions);

    Assert.Contains("name", json);
    Assert.Contains("isArchived", json);
    Assert.Contains("minValue", json);
}

[Fact]
public void UpdateScoreConfigRequest_Serializes_PartialUpdate()
{
    var request = new UpdateScoreConfigRequest
    {
        Name = "Updated Name"
        // All other fields null
    };

    var json = JsonSerializer.Serialize(request, _jsonOptions);

    Assert.Contains("name", json);
    // Verify only name is present or nulls handled correctly
}
```

**Client Method Tests:**
```csharp
[Fact]
public async Task UpdateScoreConfigAsync_Success()
{
    var request = new UpdateScoreConfigRequest
    {
        Name = "Updated Score"
    };

    var result = await client.UpdateScoreConfigAsync("config-123", request);

    Assert.NotNull(result);
    Assert.Equal("Updated Score", result.Name);
}

[Fact]
public async Task UpdateScoreConfigAsync_UsesPatchMethod()
{
    // Verify HTTP PATCH is used
    var request = new UpdateScoreConfigRequest { Name = "Test" };

    await client.UpdateScoreConfigAsync("config-1", request);

    // Verify PATCH method was used
}

[Fact]
public async Task UpdateScoreConfigAsync_EncodesConfigId()
{
    var configId = "config-123/test";
    var request = new UpdateScoreConfigRequest { Name = "Test" };

    await client.UpdateScoreConfigAsync(configId, request);

    // Verify URL encoding
}

[Fact]
public async Task GetScoresAsync_WithSessionId()
{
    var request = new GetScoresRequest
    {
        SessionId = "session-123"
    };

    await client.GetScoresAsync(request);

    // Verify sessionId in query string
}
```

#### Observation Type Tests

**Enum Tests:**
```csharp
[Theory]
[InlineData(ObservationType.Agent, "AGENT")]
[InlineData(ObservationType.Tool, "TOOL")]
[InlineData(ObservationType.Chain, "CHAIN")]
[InlineData(ObservationType.Retriever, "RETRIEVER")]
[InlineData(ObservationType.Evaluator, "EVALUATOR")]
[InlineData(ObservationType.Embedding, "EMBEDDING")]
[InlineData(ObservationType.Guardrail, "GUARDRAIL")]
public void NewObservationTypes_Serialize_Uppercase(
    ObservationType type, string expected)
{
    var json = JsonSerializer.Serialize(type, _jsonOptions);
    Assert.Equal($"\"{expected}\"", json);
}

[Theory]
[InlineData("AGENT", ObservationType.Agent)]
[InlineData("TOOL", ObservationType.Tool)]
[InlineData("CHAIN", ObservationType.Chain)]
public void NewObservationTypes_Deserialize_Correctly(
    string json, ObservationType expected)
{
    var result = JsonSerializer.Deserialize<ObservationType>(
        $"\"{json}\"", _jsonOptions);
    Assert.Equal(expected, result);
}

[Fact]
public void ExistingObservationTypes_StillWork()
{
    // Test backwards compatibility
    var types = new[]
    {
        ObservationType.Span,
        ObservationType.Generation,
        ObservationType.Event
    };

    foreach (var type in types)
    {
        var json = JsonSerializer.Serialize(type, _jsonOptions);
        var deserialized = JsonSerializer.Deserialize<ObservationType>(json, _jsonOptions);
        Assert.Equal(type, deserialized);
    }
}
```

### Edge Cases to Test

1. **Annotation Queues:**
   - Empty scoreConfigIds array
   - Multiple assignments to same queue
   - Delete non-existent assignment

2. **Organization Membership:**
   - Delete last admin
   - Delete own membership
   - User not in organization

3. **Score Config:**
   - MinValue > MaxValue
   - Update archived config
   - Partial update with all nulls

4. **Observation Types:**
   - Use new types in CreateObservationRequest
   - Filter observations by new types

## Acceptance Criteria

- [ ] All annotation queue enhancement tests pass
- [ ] All organization membership tests pass
- [ ] All score config update tests pass
- [ ] All observation type tests pass
- [ ] DELETE with body tested for relevant endpoints
- [ ] PATCH method tested for score config
- [ ] URL encoding tested for all path parameters
- [ ] SessionId parameter tested in GetScoresAsync
- [ ] Backwards compatibility tested for observation types
- [ ] Edge cases tested
- [ ] Test coverage > 80% for all affected code
- [ ] All tests pass
- [ ] Tests follow existing patterns
