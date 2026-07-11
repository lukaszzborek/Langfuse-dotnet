using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Request body for creating an LLM-as-a-judge evaluator.
/// </summary>
public class CreateLlmAsJudgeEvaluatorRequest : CreateEvaluatorRequest
{
    /// <inheritdoc />
    [JsonPropertyName("type")]
    public override EvaluatorType Type => EvaluatorType.Llm_As_Judge;

    /// <summary>
    ///     Prompt template used by the evaluator.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    /// <summary>
    ///     Structured output schema the evaluator must return. Always send DataType.
    /// </summary>
    [JsonPropertyName("outputDefinition")]
    public required EvaluatorOutputDefinition OutputDefinition { get; init; }

    /// <summary>
    ///     Optional explicit model configuration. Omit or set to null to use the project default evaluation model.
    /// </summary>
    [JsonPropertyName("modelConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EvaluatorModelConfig? ModelConfig { get; init; }
}