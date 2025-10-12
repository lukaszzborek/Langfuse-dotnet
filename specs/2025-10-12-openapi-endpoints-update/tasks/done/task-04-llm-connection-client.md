# Task 04: LLM Connection Client

**Task ID:** openapi-update-04
**Priority:** High
**Dependencies:** openapi-update-03
**Approach:** Implementation (Models must exist first)

## Objective

Implement the client methods for LLM Connections API endpoints in a new partial class file, including pagination support for the list endpoint.

## What Needs to Be Done

### Files to Create

1. **src/Langfuse/Client/LangfuseClient.LlmConnections.cs**

Create a new partial class file implementing two endpoints:
- GET `/api/public/llm-connections` (with pagination)
- PUT `/api/public/llm-connections` (upsert)

### Implementation Details

```csharp
using Langfuse.Models.LlmConnection;

namespace Langfuse.Client;

public partial class LangfuseClient
{
    /// <summary>
    /// Get all LLM connections in the project.
    /// </summary>
    /// <param name="page">Page number, starts at 1</param>
    /// <param name="limit">Limit of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of LLM connections</returns>
    public async Task<PaginatedLlmConnections> GetLlmConnectionsAsync(
        int? page = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();

        if (page.HasValue)
            queryParams.Add($"page={page.Value}");

        if (limit.HasValue)
            queryParams.Add($"limit={limit.Value}");

        var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

        var response = await _httpClient.GetAsync(
            $"llm-connections{query}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PaginatedLlmConnections>(
            _jsonOptions,
            cancellationToken);

        return result ?? throw new LangfuseApiException("Failed to deserialize response");
    }

    /// <summary>
    /// Create or update an LLM connection.
    /// The connection is upserted based on the provider name.
    /// </summary>
    /// <param name="request">LLM connection configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created or updated LLM connection</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public async Task<LlmConnection> UpsertLlmConnectionAsync(
        UpsertLlmConnectionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await _httpClient.PutAsJsonAsync(
            "llm-connections",
            request,
            _jsonOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LlmConnection>(
            _jsonOptions,
            cancellationToken);

        return result ?? throw new LangfuseApiException("Failed to deserialize response");
    }
}
```

### Key Implementation Points

1. **Pagination:** Build query string manually for optional page/limit parameters
2. **Upsert Logic:** Provider field in request determines which connection to update
3. **URL Construction:** Use relative paths (e.g., `"llm-connections"`)
4. **Validation:** Validate request object with `ArgumentNullException.ThrowIfNull()`
5. **XML Documentation:** Document the upsert behavior (updates based on provider)
6. **Error Handling:** Use `response.EnsureSuccessStatusCode()` and throw `LangfuseApiException`

### Testing Requirements

Create test file: `tests/Langfuse.Tests/Client/LangfuseClientLlmConnectionsTests.cs`

Tests to write:

1. **GetLlmConnectionsAsync:**
   - Test without pagination parameters (no query string)
   - Test with page parameter only
   - Test with limit parameter only
   - Test with both page and limit parameters
   - Test correct URL construction with query string
   - Test response deserialization with data and meta
   - Test empty results (empty data array)

2. **UpsertLlmConnectionAsync:**
   - Test successful PUT request (create)
   - Test successful PUT request (update same provider)
   - Test request serialization (verify secretKey is sent)
   - Test response deserialization (verify secretKey not in response, displaySecretKey present)
   - Test throws on null request
   - Test all required fields are validated

3. **Error Cases:**
   - Test 400 Bad Request (invalid adapter)
   - Test 401 Unauthorized
   - Test 403 Forbidden
   - Test 409 Conflict (if provider uniqueness violated)

### Mock Setup Example

```csharp
using NSubstitute;
using Xunit;

public class LangfuseClientLlmConnectionsTests
{
    [Fact]
    public async Task GetLlmConnectionsAsync_WithPagination_BuildsCorrectUrl()
    {
        // Arrange
        var client = CreateClient();

        // Act
        await client.GetLlmConnectionsAsync(page: 2, limit: 50);

        // Assert
        // Verify URL contains "?page=2&limit=50"
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_SecretKeyNotInResponse()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-secret123"
        };

        var response = new LlmConnection
        {
            Id = "conn-123",
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            DisplaySecretKey = "sk-sec***123",
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Mock HTTP response...

        // Act
        var result = await client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.Equal("sk-sec***123", result.DisplaySecretKey);
        // Verify secretKey not in response
    }
}
```

## Acceptance Criteria

- [x] `LangfuseClient.LlmConnections.cs` partial class created
- [x] Both methods implemented (Get with pagination, Upsert)
- [x] Pagination query string built correctly (only include non-null params)
- [x] Request validation implemented (null checks)
- [x] XML documentation comments on all public methods
- [x] Upsert behavior documented (updates based on provider)
- [x] Test file created with comprehensive coverage
- [x] Pagination scenarios tested (no params, one param, both params)
- [x] Upsert behavior tested (create and update)
- [x] Secret key handling tested (in request, not in response)
- [x] Error cases tested (400, 401, 403)
- [x] All tests pass
- [x] Code builds without warnings
- [x] Follows existing client method patterns
