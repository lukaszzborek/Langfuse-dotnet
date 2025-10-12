# Task 10: Testing - Blob Storage Integration

**Task ID:** openapi-update-10
**Priority:** Medium
**Dependencies:** openapi-update-01, openapi-update-02
**Approach:** Testing

## Objective

Write comprehensive unit tests for Blob Storage Integration models and client methods, ensuring all scenarios including edge cases and error handling are covered.

## What Needs to Be Done

### Test Files to Create/Update

**1. tests/Langfuse.Tests/Models/BlobStorageIntegrationTests.cs** (if not created in Task 01)
**2. tests/Langfuse.Tests/Client/LangfuseClientBlobStorageIntegrationsTests.cs** (if not created in Task 02)

### Test Coverage Requirements

#### Model Tests

**Enum Serialization Tests:**
```csharp
[Theory]
[InlineData(BlobStorageIntegrationType.S3, "S3")]
[InlineData(BlobStorageIntegrationType.S3Compatible, "S3_COMPATIBLE")]
[InlineData(BlobStorageIntegrationType.AzureBlobStorage, "AZURE_BLOB_STORAGE")]
public void BlobStorageIntegrationType_Serializes_Correctly(
    BlobStorageIntegrationType type, string expected)
{
    // Test enum serialization to kebab-case-upper
}

[Theory]
[InlineData(BlobStorageExportFrequency.Hourly, "hourly")]
[InlineData(BlobStorageExportFrequency.Daily, "daily")]
[InlineData(BlobStorageExportFrequency.Weekly, "weekly")]
public void BlobStorageExportFrequency_Serializes_Lowercase(
    BlobStorageExportFrequency freq, string expected)
{
    // Test lowercase serialization
}
```

**Request Serialization Tests:**
```csharp
[Fact]
public void CreateBlobStorageIntegrationRequest_Serializes_AllFields()
{
    // Test complete request with all fields
    var request = new CreateBlobStorageIntegrationRequest
    {
        ProjectId = "project-123",
        Type = BlobStorageIntegrationType.S3,
        BucketName = "my-bucket",
        Region = "us-east-1",
        AccessKeyId = "AKIAIOSFODNN7EXAMPLE",
        SecretAccessKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY",
        Prefix = "exports/",
        ExportFrequency = BlobStorageExportFrequency.Daily,
        Enabled = true,
        ForcePathStyle = false,
        FileType = BlobStorageIntegrationFileType.Json,
        ExportMode = BlobStorageExportMode.FullHistory
    };

    var json = JsonSerializer.Serialize(request);

    // Assert all fields present and camelCase
    Assert.Contains("projectId", json);
    Assert.Contains("bucketName", json);
    // ... more assertions
}

[Fact]
public void CreateBlobStorageIntegrationRequest_Serializes_NullableFields()
{
    // Test request with nullable fields as null
    var request = new CreateBlobStorageIntegrationRequest
    {
        ProjectId = "project-123",
        Type = BlobStorageIntegrationType.AzureBlobStorage,
        BucketName = "my-container",
        Region = "eastus",
        // Endpoint, AccessKeyId, SecretAccessKey, Prefix = null
        ExportFrequency = BlobStorageExportFrequency.Weekly,
        Enabled = true,
        ForcePathStyle = false,
        FileType = BlobStorageIntegrationFileType.Csv,
        ExportMode = BlobStorageExportMode.FromToday
    };

    var json = JsonSerializer.Serialize(request);

    // Verify nullable fields handled correctly
}
```

**Response Deserialization Tests:**
```csharp
[Fact]
public void BlobStorageIntegrationResponse_Deserializes_Correctly()
{
    var json = @"{
        ""id"": ""int-123"",
        ""projectId"": ""project-123"",
        ""type"": ""S3"",
        ""bucketName"": ""my-bucket"",
        ""region"": ""us-east-1"",
        ""prefix"": ""exports/"",
        ""exportFrequency"": ""daily"",
        ""enabled"": true,
        ""forcePathStyle"": false,
        ""fileType"": ""JSON"",
        ""exportMode"": ""FULL_HISTORY"",
        ""createdAt"": ""2025-10-12T00:00:00Z"",
        ""updatedAt"": ""2025-10-12T00:00:00Z""
    }";

    var response = JsonSerializer.Deserialize<BlobStorageIntegrationResponse>(json);

    Assert.NotNull(response);
    Assert.Equal("int-123", response.Id);
    // ... more assertions
}
```

#### Client Method Tests

**GetBlobStorageIntegrationsAsync Tests:**
```csharp
[Fact]
public async Task GetBlobStorageIntegrationsAsync_Success_ReturnsIntegrations()
{
    // Arrange: Mock successful response
    // Act: Call method
    // Assert: Verify result
}

[Fact]
public async Task GetBlobStorageIntegrationsAsync_EmptyArray_ReturnsEmpty()
{
    // Test empty data array
}

[Fact]
public async Task GetBlobStorageIntegrationsAsync_CallsCorrectUrl()
{
    // Verify "integrations/blob-storage" is called
}
```

**UpsertBlobStorageIntegrationAsync Tests:**
```csharp
[Fact]
public async Task UpsertBlobStorageIntegrationAsync_Success_ReturnsIntegration()
{
    // Test successful creation/update
}

[Fact]
public async Task UpsertBlobStorageIntegrationAsync_NullRequest_ThrowsArgumentNullException()
{
    // Test validation
}

[Fact]
public async Task UpsertBlobStorageIntegrationAsync_SerializesRequest_Correctly()
{
    // Verify request body serialization
}
```

**DeleteBlobStorageIntegrationAsync Tests:**
```csharp
[Fact]
public async Task DeleteBlobStorageIntegrationAsync_Success_ReturnsConfirmation()
{
    // Test successful deletion
}

[Fact]
public async Task DeleteBlobStorageIntegrationAsync_EncodesId_InUrl()
{
    // Test URL encoding of ID with special characters
    var id = "int-123/test";
    await client.DeleteBlobStorageIntegrationAsync(id);
    // Verify URL contains encoded version
}

[Fact]
public async Task DeleteBlobStorageIntegrationAsync_NullId_ThrowsArgumentException()
{
    // Test validation
}
```

**Error Handling Tests:**
```csharp
[Fact]
public async Task BlobStorageOperations_401Unauthorized_ThrowsException()
{
    // Test authentication error
}

[Fact]
public async Task BlobStorageOperations_403Forbidden_ThrowsException()
{
    // Test wrong API key scope (project instead of org)
}

[Fact]
public async Task BlobStorageOperations_404NotFound_ThrowsException()
{
    // Test not found error
}
```

### Edge Cases to Test

1. **Prefix Validation:**
   - Test prefix without trailing "/" (should this be validated?)
   - Test empty prefix vs null prefix

2. **Export Mode Validation:**
   - Test FROM_CUSTOM_DATE with null exportStartDate
   - Test FROM_CUSTOM_DATE with exportStartDate in the past

3. **S3 Compatible:**
   - Test S3_COMPATIBLE type requires endpoint
   - Test S3 type with endpoint (should work)

4. **Secrets:**
   - Test secretAccessKey in request
   - Test secretAccessKey NOT in response
   - Test displaySecretKey in response (if applicable)

5. **Timestamps:**
   - Test nextSyncAt and lastSyncAt nullable
   - Test createdAt and updatedAt required

## Acceptance Criteria

- [x] All enum serialization tests pass
- [x] All request serialization tests pass (with and without nulls)
- [x] All response deserialization tests pass
- [x] GetBlobStorageIntegrationsAsync fully tested
- [x] UpsertBlobStorageIntegrationAsync fully tested
- [x] DeleteBlobStorageIntegrationAsync fully tested
- [x] URL encoding tested for ID parameter
- [x] Error cases tested (401, 403, 404)
- [x] Edge cases tested (prefix, export mode, S3 compatible)
- [x] Secret handling tested (in request, not in response)
- [x] Test coverage > 80% for blob storage code
- [x] All tests pass
- [x] No flaky tests
- [x] Tests follow existing patterns in test suite
