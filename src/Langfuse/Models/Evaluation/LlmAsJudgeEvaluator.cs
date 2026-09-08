using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     LLM-as-a-judge evaluator: scores data by prompting a model and parsing its structured output.
/// </summary>
public class LlmAsJudgeEvaluator : Evaluator
{
    /// <inheritdoc />
    [JsonPropertyName("type")]
    public override EvaluatorType Type => EvaluatorType.Llm_As_Judge;

    /// <summary>
    ///     Ordered chat messages used by the latest evaluator version.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required EvaluatorChatMessage[] Prompt { get; init; }

    /// <summary>
    ///     Variables extracted from the latest prompt and available for evaluation-rule mappings.
    /// </summary>
    [JsonPropertyName("variables")]
    public required string[] Variables { get; init; }

    /// <summary>
    ///     Default variable mapping for the latest version, or null when no default is configured.
    /// </summary>
    [JsonPropertyName("variableMapping")]
    public PromptVariableMapping[]? VariableMapping { get; init; }

    /// <summary>
    ///     Explicit model configuration for the latest version, or null when the project's default evaluation
    ///     model is used.
    /// </summary>
    [JsonPropertyName("modelConfig")]
    public EvaluatorModelConfig? ModelConfig { get; init; }

    /// <summary>
    ///     Structured output schema returned by the latest evaluator version.
    /// </summary>
    [JsonPropertyName("outputDefinition")]
    public required EvaluatorOutputDefinition OutputDefinition { get; init; }
}