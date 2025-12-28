using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.LlmConnection;

/// <summary>
///     LLM API connection configuration (secrets excluded)
/// </summary>
public record LlmConnection
{
    /// <summary>
    ///     Unique identifier for the connection
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

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
    ///     Masked version of the secret key for display purposes
    /// </summary>
    [JsonPropertyName("displaySecretKey")]
    public required string DisplaySecretKey { get; init; }

    /// <summary>
    ///     Custom base URL for the LLM API
    /// </summary>
    [JsonPropertyName("baseURL")]
    public string? BaseURL { get; init; }

    /// <summary>
    ///     List of custom model names available for this connection
    /// </summary>
    [JsonPropertyName("customModels")]
    public required string[] CustomModels { get; init; }

    /// <summary>
    ///     Whether to include default models for this adapter
    /// </summary>
    [JsonPropertyName("withDefaultModels")]
    public required bool WithDefaultModels { get; init; }

    /// <summary>
    ///     Keys of extra headers sent with requests (values excluded for security)
    /// </summary>
    [JsonPropertyName("extraHeaderKeys")]
    public required string[] ExtraHeaderKeys { get; init; }

    /// <summary>
    ///     Adapter-specific configuration.
    ///     Required for Bedrock (e.g., {"region":"us-east-1"}),
    ///     optional for VertexAI (e.g., {"location":"us-central1"}),
    ///     not used by other adapters.
    /// </summary>
    [JsonPropertyName("config")]
    public Dictionary<string, object>? Config { get; init; }

    /// <summary>
    ///     Creation timestamp
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Last update timestamp
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public required DateTime UpdatedAt { get; init; }
}