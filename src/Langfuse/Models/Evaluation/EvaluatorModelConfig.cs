using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Explicit model configuration for an evaluator. If omitted, the project's default evaluation model is used.
/// </summary>
public class EvaluatorModelConfig
{
    /// <summary>
    ///     Provider identifier to use for this evaluator (for example "openai" or "anthropic").
    ///     Must match one of the providers configured via the LLM connections endpoint.
    /// </summary>
    [JsonPropertyName("provider")]
    public required string Provider { get; init; }

    /// <summary>
    ///     Model identifier exposed by the provider (for example "gpt-4.1-mini").
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }
}
