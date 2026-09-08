using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Stored version of an LLM-as-a-judge evaluator.
/// </summary>
public class LlmAsJudgeEvaluatorVersion : EvaluatorVersion
{
    /// <inheritdoc />
    [JsonPropertyName("type")]
    public override EvaluatorType Type => EvaluatorType.Llm_As_Judge;

    /// <summary>
    ///     Ordered chat messages used during evaluation.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required EvaluatorChatMessage[] Prompt { get; init; }

    /// <summary>
    ///     Variables extracted from the prompt and available for evaluation-rule mappings.
    /// </summary>
    [JsonPropertyName("variables")]
    public required string[] Variables { get; init; }

    /// <summary>
    ///     Default variable mapping for this version, or null when no default is configured.
    /// </summary>
    [JsonPropertyName("variableMapping")]
    public PromptVariableMapping[]? VariableMapping { get; init; }

    /// <summary>
    ///     Explicit model configuration, or null when the project's default evaluation model is used.
    /// </summary>
    [JsonPropertyName("modelConfig")]
    public EvaluatorModelConfig? ModelConfig { get; init; }

    /// <summary>
    ///     Structured output schema returned by this evaluator version.
    /// </summary>
    [JsonPropertyName("outputDefinition")]
    public required EvaluatorOutputDefinition OutputDefinition { get; init; }
}