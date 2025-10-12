# Task 02: Blob Storage Integration Client

**Task ID:** openapi-update-02
**Priority:** High
**Dependencies:** openapi-update-01
**Approach:** Implementation (Models must exist first)

## Objective

Implement the client methods for Blob Storage Integration API endpoints in a new partial class file, following the existing partial class pattern used in the SDK.

## What Needs to Be Done

### Files to Create

1. **src/Langfuse/Client/LangfuseClient.BlobStorageIntegrations.cs**

Create a new partial class file implementing three endpoints:
- GET `/api/public/integrations/blob-storage`
- PUT `/api/public/integrations/blob-storage`
- DELETE `/api/public/integrations/blob-storage/{id}`

### Implementation Details

```csharp
using Langfuse.Models.BlobStorageIntegration;

namespace Langfuse.Client;

public partial class LangfuseClient
{
    /// <summary>
    /// Get all blob storage integrations for the organization.
    /// Requires organization-scoped API key.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of blob storage integrations</returns>
    public async Task<BlobStorageIntegrationsResponse> GetBlobStorageIntegrationsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            "integrations/blob-storage",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BlobStorageIntegrationsResponse>(
            _jsonOptions,
            cancellationToken);

        return result ?? throw new LangfuseApiException("Failed to deserialize response");
    }

    /// <summary>
    /// Create or update a blob storage integration for a specific project.
    /// The configuration is validated by performing a test upload to the bucket.
    /// Requires organization-scoped API key.
    /// </summary>
    /// <param name="request">Blob storage integration configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created or updated blob storage integration</returns>
    public async Task<BlobStorageIntegrationResponse> UpsertBlobStorageIntegrationAsync(
        CreateBlobStorageIntegrationRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            "integrations/blob-storage",
            request,
            _jsonOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BlobStorageIntegrationResponse>(
            _jsonOptions,
            cancellationToken);

        return result ?? throw new LangfuseApiException("Failed to deserialize response");
    }

    /// <summary>
    /// Delete a blob storage integration by ID.
    /// Requires organization-scoped API key.
    /// </summary>
    /// <param name="id">The unique identifier of the blob storage integration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    public async Task<BlobStorageIntegrationDeletionResponse> DeleteBlobStorageIntegrationAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var response = await _httpClient.DeleteAsync(
            $"integrations/blob-storage/{Uri.EscapeDataString(id)}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BlobStorageIntegrationDeletionResponse>(
            _jsonOptions,
            cancellationToken);

        return result ?? throw new LangfuseApiException("Failed to deserialize response");
    }
}
```

### Key Implementation Points

1. **URL Construction:** Use relative paths without leading slash (e.g., `"integrations/blob-storage"`)
2. **Path Parameters:** Escape using `Uri.EscapeDataString()` for the ID parameter
3. **Error Handling:** Use `response.EnsureSuccessStatusCode()` and throw `LangfuseApiException` for null results
4. **JSON Options:** Use existing `_jsonOptions` field for serialization
5. **Validation:** Validate required parameters with `ArgumentException.ThrowIfNullOrWhiteSpace()`
6. **XML Documentation:** Include organization-scoped key requirement in documentation

### Testing Requirements

Create test file: `tests/Langfuse.Tests/Client/LangfuseClientBlobStorageIntegrationsTests.cs`

Tests to write:
1. **GetBlobStorageIntegrationsAsync:**
   - Test successful GET request
   - Test correct URL is called
   - Test response deserialization
   - Test handles empty array

2. **UpsertBlobStorageIntegrationAsync:**
   - Test successful PUT request
   - Test request serialization
   - Test response deserialization
   - Test validation of required fields

3. **DeleteBlobStorageIntegrationAsync:**
   - Test successful DELETE request
   - Test ID parameter is URL encoded
   - Test throws on null/empty ID
   - Test response deserialization

4. **Error Cases:**
   - Test 401 Unauthorized response
   - Test 403 Forbidden response (wrong API key scope)
   - Test 404 Not Found response
   - Test network errors

### Mock Setup Example

```csharp
using NSubstitute;
using Xunit;

public class LangfuseClientBlobStorageIntegrationsTests
{
    private readonly HttpClient _httpClient;
    private readonly LangfuseClient _client;

    public LangfuseClientBlobStorageIntegrationsTests()
    {
        var handler = Substitute.For<HttpMessageHandler>();
        _httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        _client = new LangfuseClient(_httpClient);
    }

    [Fact]
    public async Task GetBlobStorageIntegrationsAsync_Success()
    {
        // Arrange
        var expected = new BlobStorageIntegrationsResponse
        {
            Data = new[] { /* test data */ }
        };

        // Mock HTTP response...

        // Act
        var result = await _client.GetBlobStorageIntegrationsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Data);
    }
}
```

## Acceptance Criteria

- [ ] `LangfuseClient.BlobStorageIntegrations.cs` partial class created
- [ ] All three methods implemented (Get, Upsert, Delete)
- [ ] Methods follow existing SDK patterns (check other LangfuseClient.*.cs files)
- [ ] Proper URL construction with path parameters
- [ ] ID parameter properly URL encoded in delete method
- [ ] All parameters validated (null checks)
- [ ] XML documentation comments on all public methods
- [ ] Organization-scoped key requirement documented
- [ ] Test file created with comprehensive coverage
- [ ] All HTTP methods tested (GET, PUT, DELETE)
- [ ] Error cases tested (401, 403, 404)
- [ ] All tests pass
- [ ] Code builds without warnings
- [ ] Follows existing client method patterns
