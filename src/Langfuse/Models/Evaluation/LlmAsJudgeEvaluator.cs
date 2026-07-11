using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     LLM-as-a-judge evaluator: scores data by prompting a model with a prompt template and parsing its
///     structured output.
/// </summary>
public class LlmAsJudgeEvaluator : Evaluator
{
    /// <inheritdoc />
    [JsonPropertyName("type")]
    public override EvaluatorType Type => EvaluatorType.Llm_As_Judge;

    /// <summary>
    ///     Prompt template used during evaluation.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    /// <summary>
    ///     Structured output schema returned by this evaluator.
    /// </summary>
    [JsonPropertyName("outputDefinition")]
    public required EvaluatorOutputDefinition OutputDefinition { get; init; }

    /// <summary>
    ///     Explicit model configuration, or null when the project default evaluation model is used.
    /// </summary>
    [JsonPropertyName("modelConfig")]
    public EvaluatorModelConfig? ModelConfig { get; init; }
}