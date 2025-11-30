namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for Gen AI embeddings operations.
/// </summary>
public class GenAiEmbeddingsConfig
{
    public required string Provider { get; init; }
    public required string Model { get; init; }
    public int? Dimensions { get; init; }
    public string[]? EncodingFormats { get; init; }
    public string? ServerAddress { get; init; }
    public int? ServerPort { get; init; }
}