namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for Gen AI text completion operations.
/// </summary>
public class GenAiTextCompletionConfig
{
    /// <summary>
    ///     The AI provider name (e.g., "openai", "anthropic").
    /// </summary>
    public required string Provider { get; init; }

    /// <summary>
    ///     The model identifier to use for the completion.
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    ///     Controls randomness in the output. Higher values make output more random.
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    ///     Maximum number of tokens to generate in the completion.
    /// </summary>
    public int? MaxTokens { get; init; }
}