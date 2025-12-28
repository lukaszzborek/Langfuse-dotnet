using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.LlmConnection;

/// <summary>
///     Request to create or update an LLM connection (upsert)
/// </summary>
public record UpsertLlmConnectionRequest
{
    /// <summary>
    ///     Provider name (e.g., 'openai', 'my-gateway').
    ///     Must be unique in project, used for upserting.
    /// </summary>
    [JsonPropertyName("provider")]
    public required string Provider { get; init; }

    /// <summary>
    ///     The adapter used to interface with the LLM
    /// </summary>
    [JsonPropertyName("adapter")]
    public required LlmAdapter Adapter { get; init; }

    /// <summary>
    ///     Secret key or API key for authentication
    /// </summary>
    [JsonPropertyName("secretKey")]
    public required string SecretKey { get; init; }

    /// <summary>
    ///     Custom base URL for the LLM API
    /// </summary>
    [JsonPropertyName("baseURL")]
    public string? BaseURL { get; init; }

    /// <summary>
    ///     List of custom model names to make available
    /// </summary>
    [JsonPropertyName("customModels")]
    public string[]? CustomModels { get; init; }

    /// <summary>
    ///     Whether to include default models for this adapter
    /// </summary>
    [JsonPropertyName("withDefaultModels")]
    public bool? WithDefaultModels { get; init; }

    /// <summary>
    ///     Extra headers to send with requests
    /// </summary>
    [JsonPropertyName("extraHeaders")]
    public Dictionary<string, string>? ExtraHeaders { get; init; }

    /// <summary>
    ///     Adapter-specific configuration. Validation rules:
    ///     - Bedrock: Required. Must be {"region": "aws-region"} (e.g., {"region":"us-east-1"})
    ///     - VertexAI: Optional. If provided, must be {"location": "gcp-location"} (e.g., {"location":"us-central1"})
    ///     - Other adapters: Not supported. Omit this field or set to null.
    /// </summary>
    [JsonPropertyName("config")]
    public Dictionary<string, object>? Config { get; init; }
}