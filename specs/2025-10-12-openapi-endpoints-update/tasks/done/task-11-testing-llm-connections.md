# Task 11: Testing - LLM Connections

**Task ID:** openapi-update-11
**Priority:** Medium
**Dependencies:** openapi-update-03, openapi-update-04
**Approach:** Testing

## Objective

Write comprehensive unit tests for LLM Connections models and client methods, with special focus on secret handling and provider uniqueness.

## What Needs to Be Done

### Test Files to Create/Update

**1. tests/Langfuse.Tests/Models/LlmConnectionTests.cs** (if not created in Task 03)
**2. tests/Langfuse.Tests/Client/LangfuseClientLlmConnectionsTests.cs** (if not created in Task 04)

### Test Coverage Requirements

#### Model Tests

**LlmAdapter Enum Tests:**
```csharp
[Theory]
[InlineData(LlmAdapter.Anthropic, "anthropic")]
[InlineData(LlmAdapter.OpenAi, "openai")]
[InlineData(LlmAdapter.Azure, "azure")]
[InlineData(LlmAdapter.Bedrock, "bedrock")]
[InlineData(LlmAdapter.GoogleVertexAi, "google-vertex-ai")]
[InlineData(LlmAdapter.GoogleAiStudio, "google-ai-studio")]
public void LlmAdapter_Serializes_Lowercase(LlmAdapter adapter, string expected)
{
    // Test lowercase serialization with hyphens
    var json = JsonSerializer.Serialize(adapter, _jsonOptions);
    Assert.Equal($"\"{expected}\"", json);
}

[Theory]
[InlineData("anthropic", LlmAdapter.Anthropic)]
[InlineData("google-vertex-ai", LlmAdapter.GoogleVertexAi)]
public void LlmAdapter_Deserializes_Correctly(string json, LlmAdapter expected)
{
    // Test deserialization
    var result = JsonSerializer.Deserialize<LlmAdapter>($"\"{json}\"", _jsonOptions);
    Assert.Equal(expected, result);
}
```

**Request Serialization Tests:**
```csharp
[Fact]
public void UpsertLlmConnectionRequest_Serializes_AllFields()
{
    var request = new UpsertLlmConnectionRequest
    {
        Provider = "my-openai-gateway",
        Adapter = LlmAdapter.OpenAi,
        SecretKey = "sk-1234567890abcdef",
        BaseURL = "https://api.custom-gateway.com",
        CustomModels = new[] { "gpt-4-custom", "gpt-3.5-custom" },
        WithDefaultModels = true,
        ExtraHeaders = new Dictionary<string, string>
        {
            { "X-Custom-Header", "value1" },
            { "X-Api-Version", "v2" }
        }
    };

    var json = JsonSerializer.Serialize(request, _jsonOptions);

    // Assertions
    Assert.Contains("provider", json);
    Assert.Contains("my-openai-gateway", json);
    Assert.Contains("secretKey", json);
    Assert.Contains("sk-1234567890abcdef", json);
    Assert.Contains("extraHeaders", json);
}

[Fact]
public void UpsertLlmConnectionRequest_Serializes_MinimalFields()
{
    var request = new UpsertLlmConnectionRequest
    {
        Provider = "openai",
        Adapter = LlmAdapter.OpenAi,
        SecretKey = "sk-1234567890abcdef"
        // BaseURL, CustomModels, WithDefaultModels, ExtraHeaders = null
    };

    var json = JsonSerializer.Serialize(request, _jsonOptions);

    // Verify required fields present, nullable fields handled
    Assert.Contains("provider", json);
    Assert.Contains("adapter", json);
    Assert.Contains("secretKey", json);
}
```

**Response Deserialization Tests:**
```csharp
[Fact]
public void LlmConnection_Deserializes_Correctly()
{
    var json = @"{
        ""id"": ""conn-123"",
        ""provider"": ""my-openai"",
        ""adapter"": ""openai"",
        ""displaySecretKey"": ""sk-***def"",
        ""baseURL"": ""https://api.custom.com"",
        ""customModels"": [""model-1"", ""model-2""],
        ""withDefaultModels"": true,
        ""extraHeaderKeys"": [""X-Custom-Header""],
        ""createdAt"": ""2025-10-12T00:00:00Z"",
        ""updatedAt"": ""2025-10-12T00:00:00Z""
    }";

    var connection = JsonSerializer.Deserialize<LlmConnection>(json, _jsonOptions);

    Assert.NotNull(connection);
    Assert.Equal("conn-123", connection.Id);
    Assert.Equal("my-openai", connection.Provider);
    Assert.Equal("sk-***def", connection.DisplaySecretKey);
    Assert.Equal(2, connection.CustomModels.Length);
    Assert.Single(connection.ExtraHeaderKeys);
}

[Fact]
public void LlmConnection_DoesNotContain_SecretKey()
{
    // Verify secretKey field doesn't exist in response model
    var properties = typeof(LlmConnection).GetProperties();
    Assert.DoesNotContain(properties, p => p.Name == "SecretKey");
    Assert.Contains(properties, p => p.Name == "DisplaySecretKey");
}
```

**Pagination Tests:**
```csharp
[Fact]
public void PaginatedLlmConnections_Deserializes_WithMeta()
{
    var json = @"{
        ""data"": [
            {
                ""id"": ""conn-1"",
                ""provider"": ""openai"",
                ""adapter"": ""openai"",
                ""displaySecretKey"": ""sk-***"",
                ""customModels"": [],
                ""withDefaultModels"": true,
                ""extraHeaderKeys"": [],
                ""createdAt"": ""2025-10-12T00:00:00Z"",
                ""updatedAt"": ""2025-10-12T00:00:00Z""
            }
        ],
        ""meta"": {
            ""page"": 1,
            ""limit"": 50,
            ""totalItems"": 1,
            ""totalPages"": 1
        }
    }";

    var result = JsonSerializer.Deserialize<PaginatedLlmConnections>(json, _jsonOptions);

    Assert.NotNull(result);
    Assert.Single(result.Data);
    Assert.NotNull(result.Meta);
    Assert.Equal(1, result.Meta.Page);
}
```

#### Client Method Tests

**GetLlmConnectionsAsync Tests:**
```csharp
[Fact]
public async Task GetLlmConnectionsAsync_NoPagination_Success()
{
    // Test without pagination parameters
    var result = await client.GetLlmConnectionsAsync();
    Assert.NotNull(result);
}

[Theory]
[InlineData(null, null, "llm-connections")]
[InlineData(1, null, "llm-connections?page=1")]
[InlineData(null, 50, "llm-connections?limit=50")]
[InlineData(2, 25, "llm-connections?page=2&limit=25")]
public async Task GetLlmConnectionsAsync_BuildsCorrectUrl(
    int? page, int? limit, string expectedUrl)
{
    // Test query string construction
    await client.GetLlmConnectionsAsync(page, limit);
    // Verify URL matches expected
}

[Fact]
public async Task GetLlmConnectionsAsync_EmptyResults_ReturnsEmptyArray()
{
    // Mock empty data array
    // Verify returns successfully with empty array
}
```

**UpsertLlmConnectionAsync Tests:**
```csharp
[Fact]
public async Task UpsertLlmConnectionAsync_Create_Success()
{
    var request = new UpsertLlmConnectionRequest
    {
        Provider = "new-provider",
        Adapter = LlmAdapter.OpenAi,
        SecretKey = "sk-new-key"
    };

    var result = await client.UpsertLlmConnectionAsync(request);

    Assert.NotNull(result);
    Assert.Equal("new-provider", result.Provider);
}

[Fact]
public async Task UpsertLlmConnectionAsync_Update_SameProvider()
{
    // Test updating existing provider
    // First create, then upsert with same provider name
}

[Fact]
public async Task UpsertLlmConnectionAsync_SecretInRequest_NotInResponse()
{
    var request = new UpsertLlmConnectionRequest
    {
        Provider = "test",
        Adapter = LlmAdapter.Anthropic,
        SecretKey = "sk-ant-1234567890"
    };

    // Mock response without secretKey but with displaySecretKey
    var result = await client.UpsertLlmConnectionAsync(request);

    // Verify response has displaySecretKey, not secretKey
    Assert.NotNull(result.DisplaySecretKey);
    Assert.Contains("***", result.DisplaySecretKey);
}

[Fact]
public async Task UpsertLlmConnectionAsync_NullRequest_ThrowsArgumentNullException()
{
    await Assert.ThrowsAsync<ArgumentNullException>(() =>
        client.UpsertLlmConnectionAsync(null!));
}
```

**Error Handling Tests:**
```csharp
[Fact]
public async Task LlmConnectionOperations_401Unauthorized_ThrowsException()
{
    // Test authentication error
}

[Fact]
public async Task UpsertLlmConnectionAsync_InvalidAdapter_Returns400()
{
    // Test invalid adapter value
}

[Fact]
public async Task UpsertLlmConnectionAsync_DuplicateProvider_HandlesCorrectly()
{
    // Test behavior with duplicate provider
    // Should update, not error
}
```

### Edge Cases to Test

1. **Provider Uniqueness:**
   - Test upserting with same provider name updates existing
   - Test provider name case sensitivity

2. **Secret Key Handling:**
   - Test secretKey in request is sent
   - Test secretKey never in response
   - Test displaySecretKey format (masked)

3. **Custom Models:**
   - Test empty customModels array
   - Test null customModels
   - Test customModels with withDefaultModels=true

4. **Extra Headers:**
   - Test extraHeaders serialization (full dictionary)
   - Test extraHeaderKeys deserialization (keys only)
   - Test empty extraHeaders

5. **BaseURL:**
   - Test with custom baseURL
   - Test without baseURL (null)
   - Test baseURL validation (if any)

## Acceptance Criteria

- [x] All enum serialization tests pass (lowercase, hyphens)
- [x] All request serialization tests pass
- [x] All response deserialization tests pass
- [x] Secret handling tested (in request, not in response)
- [x] DisplaySecretKey vs SecretKey distinction verified
- [x] Pagination tests pass
- [x] GetLlmConnectionsAsync fully tested with all parameter combinations
- [x] UpsertLlmConnectionAsync fully tested (create and update)
- [x] Provider uniqueness tested
- [x] Error cases tested (401, 400)
- [x] Edge cases tested (customModels, extraHeaders, baseURL)
- [x] Test coverage > 80% for LLM connections code
- [x] All tests pass
- [x] Tests follow existing patterns
