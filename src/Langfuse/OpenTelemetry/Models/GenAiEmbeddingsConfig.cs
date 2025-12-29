namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for Gen AI embeddings operations.
/// </summary>
public class GenAiEmbeddingsConfig
{
    /// <summary>
    ///     The AI provider name (e.g., "openai", "cohere").
    /// </summary>
    public required string Provider { get; init; }

    /// <summary>
    ///     The model identifier to use for generating embeddings.
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    ///     The number of dimensions for the embedding vectors.
    /// </summary>
    public int? Dimensions { get; init; }

    /// <summary>
    ///     The encoding formats to use for the embeddings.
    /// </summary>
    public string[]? EncodingFormats { get; init; }

    /// <summary>
    ///     The server address for custom API endpoints.
    /// </summary>
    public string? ServerAddress { get; init; }

    /// <summary>
    ///     The server port for custom API endpoints.
    /// </summary>
    public int? ServerPort { get; init; }
}