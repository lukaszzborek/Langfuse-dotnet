namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for Gen AI text completion operations.
/// </summary>
public class GenAiTextCompletionConfig
{
    public required string Provider { get; init; }
    public required string Model { get; init; }
    public double? Temperature { get; init; }
    public int? MaxTokens { get; init; }
}