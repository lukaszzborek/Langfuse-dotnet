# Task 03: LLM Connection Models

**Task ID:** openapi-update-03
**Priority:** High
**Dependencies:** None
**Approach:** TDD (Test-driven development)

## Objective

Create all model classes for the LLM Connections feature, including the LlmAdapter enum, request/response DTOs, and paginated response, following existing SDK patterns.

## What Needs to Be Done

### Files to Create

Create the following files in `src/Langfuse/Models/LlmConnection/`:

1. **LlmAdapter.cs**
   - Enum with values: Anthropic, OpenAi, Azure, Bedrock, GoogleVertexAi, GoogleAiStudio
   - Use lowercase serialization (e.g., "anthropic", "openai", "azure")
   - Special cases: "google-vertex-ai", "google-ai-studio"

2. **LlmConnection.cs**
   - Record type with properties:
     - `string Id` (required)
     - `string Provider` (required) - unique in project, used for upserting
     - `LlmAdapter Adapter` (required)
     - `string DisplaySecretKey` (required) - masked version for display
     - `string? BaseURL` (nullable) - custom base URL
     - `string[] CustomModels` (required) - list of custom model names
     - `bool WithDefaultModels` (required) - include default models
     - `string[] ExtraHeaderKeys` (required) - keys only, values excluded for security
     - `DateTime CreatedAt` (required)
     - `DateTime UpdatedAt` (required)
   - Note: secretKey never returned in responses
   - Add XML documentation

3. **UpsertLlmConnectionRequest.cs**
   - Record type with properties:
     - `string Provider` (required) - unique identifier
     - `LlmAdapter Adapter` (required)
     - `string SecretKey` (required) - API key/secret
     - `string? BaseURL` (nullable)
     - `string[]? CustomModels` (nullable)
     - `bool? WithDefaultModels` (nullable)
     - `Dictionary<string, string>? ExtraHeaders` (nullable) - full headers for request
   - Add XML documentation

4. **PaginatedLlmConnections.cs**
   - Record type with properties:
     - `LlmConnection[] Data` (required)
     - `utilsMetaResponse Meta` (required) - use existing pagination meta type
   - Add XML documentation

### Implementation Details

**LlmAdapter Enum:**
```csharp
using System.Text.Json.Serialization;

namespace Langfuse.Models.LlmConnection;

/// <summary>
/// The adapter used to interface with the LLM
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<LlmAdapter>))]
public enum LlmAdapter
{
    /// <summary>
    /// Anthropic adapter
    /// </summary>
    [JsonPropertyName("anthropic")]
    Anthropic,

    /// <summary>
    /// OpenAI adapter
    /// </summary>
    [JsonPropertyName("openai")]
    OpenAi,

    /// <summary>
    /// Azure OpenAI adapter
    /// </summary>
    [JsonPropertyName("azure")]
    Azure,

    /// <summary>
    /// AWS Bedrock adapter
    /// </summary>
    [JsonPropertyName("bedrock")]
    Bedrock,

    /// <summary>
    /// Google Vertex AI adapter
    /// </summary>
    [JsonPropertyName("google-vertex-ai")]
    GoogleVertexAi,

    /// <summary>
    /// Google AI Studio adapter
    /// </summary>
    [JsonPropertyName("google-ai-studio")]
    GoogleAiStudio
}
```

**LlmConnection Record:**
```csharp
using System.Text.Json.Serialization;

namespace Langfuse.Models.LlmConnection;

/// <summary>
/// LLM API connection configuration (secrets excluded)
/// </summary>
public record LlmConnection
{
    /// <summary>
    /// Unique identifier for the connection
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Provider name (e.g., 'openai', 'my-gateway').
    /// Must be unique in project, used for upserting.
    /// </summary>
    [JsonPropertyName("provider")]
    public required string Provider { get; init; }

    /// <summary>
    /// The adapter used to interface with the LLM
    /// </summary>
    [JsonPropertyName("adapter")]
    public required LlmAdapter Adapter { get; init; }

    /// <summary>
    /// Masked version of the secret key for display purposes
    /// </summary>
    [JsonPropertyName("displaySecretKey")]
    public required string DisplaySecretKey { get; init; }

    /// <summary>
    /// Custom base URL for the LLM API
    /// </summary>
    [JsonPropertyName("baseURL")]
    public string? BaseURL { get; init; }

    /// <summary>
    /// List of custom model names available for this connection
    /// </summary>
    [JsonPropertyName("customModels")]
    public required string[] CustomModels { get; init; }

    /// <summary>
    /// Whether to include default models for this adapter
    /// </summary>
    [JsonPropertyName("withDefaultModels")]
    public required bool WithDefaultModels { get; init; }

    /// <summary>
    /// Keys of extra headers sent with requests (values excluded for security)
    /// </summary>
    [JsonPropertyName("extraHeaderKeys")]
    public required string[] ExtraHeaderKeys { get; init; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public required DateTime UpdatedAt { get; init; }
}
```

**UpsertLlmConnectionRequest Record:**
```csharp
using System.Text.Json.Serialization;

namespace Langfuse.Models.LlmConnection;

/// <summary>
/// Request to create or update an LLM connection (upsert)
/// </summary>
public record UpsertLlmConnectionRequest
{
    /// <summary>
    /// Provider name (e.g., 'openai', 'my-gateway').
    /// Must be unique in project, used for upserting.
    /// </summary>
    [JsonPropertyName("provider")]
    public required string Provider { get; init; }

    /// <summary>
    /// The adapter used to interface with the LLM
    /// </summary>
    [JsonPropertyName("adapter")]
    public required LlmAdapter Adapter { get; init; }

    /// <summary>
    /// Secret key or API key for authentication
    /// </summary>
    [JsonPropertyName("secretKey")]
    public required string SecretKey { get; init; }

    /// <summary>
    /// Custom base URL for the LLM API
    /// </summary>
    [JsonPropertyName("baseURL")]
    public string? BaseURL { get; init; }

    /// <summary>
    /// List of custom model names to make available
    /// </summary>
    [JsonPropertyName("customModels")]
    public string[]? CustomModels { get; init; }

    /// <summary>
    /// Whether to include default models for this adapter
    /// </summary>
    [JsonPropertyName("withDefaultModels")]
    public bool? WithDefaultModels { get; init; }

    /// <summary>
    /// Extra headers to send with requests
    /// </summary>
    [JsonPropertyName("extraHeaders")]
    public Dictionary<string, string>? ExtraHeaders { get; init; }
}
```

### Testing Requirements

Create test file: `tests/Langfuse.Tests/Models/LlmConnectionTests.cs`

Tests to write:
1. Test LlmAdapter enum serialization (verify lowercase and hyphenated values)
2. Test LlmAdapter enum deserialization
3. Test UpsertLlmConnectionRequest serialization with all fields
4. Test UpsertLlmConnectionRequest with nullable fields as null
5. Test LlmConnection response deserialization
6. Test PaginatedLlmConnections deserialization with array and meta
7. Test secretKey is in request but not in response
8. Test DisplaySecretKey vs SecretKey naming distinction

## Acceptance Criteria

- [x] All 4 model files created in `src/Langfuse/Models/LlmConnection/`
- [x] LlmAdapter enum uses lowercase serialization
- [x] Special enum values use hyphens (google-vertex-ai, google-ai-studio)
- [x] All record types use `[JsonPropertyName]` for camelCase
- [x] LlmConnection excludes secretKey (only displaySecretKey)
- [x] UpsertLlmConnectionRequest includes secretKey
- [x] PaginatedLlmConnections uses existing utilsMetaResponse type
- [x] All properties have correct nullability annotations
- [x] All public types have XML documentation comments
- [x] Security note added about secretKey handling
- [x] Test file created with comprehensive serialization tests
- [x] All tests pass
- [x] Code builds without warnings
- [x] No nullable reference type warnings
- [x] Follows existing SDK patterns
