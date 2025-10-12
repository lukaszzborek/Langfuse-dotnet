# Task 01: Blob Storage Integration Models

**Task ID:** openapi-update-01
**Priority:** High
**Dependencies:** None
**Approach:** TDD (Test-driven development)

## Objective

Create all model classes for the Blob Storage Integration feature, including enums, request/response DTOs, following the existing SDK patterns for immutable records, JSON serialization, and XML documentation.

## What Needs to Be Done

### Files to Create

Create the following files in `src/Langfuse/Models/BlobStorageIntegration/`:

1. **BlobStorageIntegrationType.cs**
   - Enum with values: S3, S3Compatible, AzureBlobStorage
   - Use `[JsonConverter(typeof(JsonStringEnumConverter))]` attribute
   - Follow kebab-case-upper serialization (e.g., "S3_COMPATIBLE")

2. **BlobStorageIntegrationFileType.cs**
   - Enum with values: Json, Csv, Jsonl
   - JSON serialization attributes

3. **BlobStorageExportMode.cs**
   - Enum with values: FullHistory, FromToday, FromCustomDate
   - JSON serialization attributes

4. **BlobStorageExportFrequency.cs**
   - Enum with values: Hourly, Daily, Weekly
   - JSON serialization attributes (lowercase: "hourly", "daily", "weekly")

5. **CreateBlobStorageIntegrationRequest.cs**
   - Record type with properties:
     - `string ProjectId` (required)
     - `BlobStorageIntegrationType Type` (required)
     - `string BucketName` (required)
     - `string? Endpoint` (nullable, required for S3_COMPATIBLE)
     - `string Region` (required)
     - `string? AccessKeyId` (nullable)
     - `string? SecretAccessKey` (nullable, will be encrypted)
     - `string? Prefix` (nullable, must end with "/" if provided)
     - `BlobStorageExportFrequency ExportFrequency` (required)
     - `bool Enabled` (required)
     - `bool ForcePathStyle` (required)
     - `BlobStorageIntegrationFileType FileType` (required)
     - `BlobStorageExportMode ExportMode` (required)
     - `DateTime? ExportStartDate` (nullable, required when ExportMode is FROM_CUSTOM_DATE)
   - Use `[JsonPropertyName("propertyName")]` for camelCase serialization
   - Add XML documentation comments

6. **BlobStorageIntegrationResponse.cs**
   - Record type with all properties from request plus:
     - `string Id` (required)
     - `DateTime? NextSyncAt` (nullable)
     - `DateTime? LastSyncAt` (nullable)
     - `DateTime CreatedAt` (required)
     - `DateTime UpdatedAt` (required)
   - Note: `SecretAccessKey` not included in response
   - Add XML documentation comments

7. **BlobStorageIntegrationsResponse.cs**
   - Record type with property:
     - `BlobStorageIntegrationResponse[] Data` (required)
   - Add XML documentation comments

8. **BlobStorageIntegrationDeletionResponse.cs**
   - Record type with property:
     - `string Message` (required)
   - Add XML documentation comments

### Implementation Details

**Enum Serialization Pattern:**
```csharp
using System.Text.Json.Serialization;

namespace Langfuse.Models.BlobStorageIntegration;

/// <summary>
/// Blob storage integration type
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<BlobStorageIntegrationType>))]
public enum BlobStorageIntegrationType
{
    /// <summary>
    /// Amazon S3
    /// </summary>
    [JsonPropertyName("S3")]
    S3,

    /// <summary>
    /// S3-compatible storage
    /// </summary>
    [JsonPropertyName("S3_COMPATIBLE")]
    S3Compatible,

    /// <summary>
    /// Azure Blob Storage
    /// </summary>
    [JsonPropertyName("AZURE_BLOB_STORAGE")]
    AzureBlobStorage
}
```

**Record Type Pattern:**
```csharp
using System.Text.Json.Serialization;

namespace Langfuse.Models.BlobStorageIntegration;

/// <summary>
/// Request to create or update a blob storage integration
/// </summary>
public record CreateBlobStorageIntegrationRequest
{
    /// <summary>
    /// ID of the project in which to configure the blob storage integration
    /// </summary>
    [JsonPropertyName("projectId")]
    public required string ProjectId { get; init; }

    // ... other properties
}
```

### Testing Requirements

Create test file: `tests/Langfuse.Tests/Models/BlobStorageIntegrationTests.cs`

Tests to write:
1. Test enum serialization to JSON (verify kebab-case-upper)
2. Test enum deserialization from JSON
3. Test CreateBlobStorageIntegrationRequest serialization with all fields
4. Test CreateBlobStorageIntegrationRequest serialization with nullable fields as null
5. Test BlobStorageIntegrationResponse deserialization
6. Test BlobStorageIntegrationsResponse deserialization with array
7. Test null handling for optional fields

## Acceptance Criteria

- [ ] All 8 model files created in `src/Langfuse/Models/BlobStorageIntegration/`
- [ ] All enums use proper JSON serialization attributes
- [ ] BlobStorageIntegrationType uses kebab-case-upper (S3_COMPATIBLE)
- [ ] BlobStorageExportFrequency uses lowercase (hourly, daily, weekly)
- [ ] All record types use `[JsonPropertyName]` for camelCase
- [ ] All properties have correct nullability annotations
- [ ] All public types have XML documentation comments
- [ ] Test file created with comprehensive serialization tests
- [ ] All tests pass
- [ ] Code builds without warnings
- [ ] No nullable reference type warnings
- [ ] Follows existing SDK patterns (check similar models in Models/ directory)
