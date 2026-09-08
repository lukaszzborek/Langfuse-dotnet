using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Complete definition replacement for an LLM-as-a-judge evaluator. Definition fields are replaced as a
///     unit rather than merged. Omitting <see cref="ModelConfig" /> selects the project's default evaluation model.
/// </summary>
public class UpdateLlmAsJudgeEvaluatorRequest : UpdateEvaluatorRequest
{
    /// <summary>
    ///     Evaluator type. The type of an existing evaluator cannot change.
    /// </summary>
    [JsonPropertyName("type")]
    public EvaluatorType Type => EvaluatorType.Llm_As_Judge;

    /// <summary>
    ///     Complete replacement user prompt string or ordered list of chat messages.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required EvaluatorChatPromptInput Prompt { get; init; }

    /// <summary>
    ///     Explicit model configuration. Omit to use the project's default evaluation model.
    /// </summary>
    [JsonPropertyName("modelConfig")]
    public EvaluatorModelConfig? ModelConfig { get; init; }

    /// <summary>
    ///     Complete replacement default variable mapping, or null when no default is configured.
    /// </summary>
    [JsonPropertyName("variableMapping")]
    public PromptVariableMappingInput[]? VariableMapping { get; init; }

    /// <summary>
    ///     Complete replacement output schema.
    /// </summary>
    [JsonPropertyName("outputDefinition")]
    public required EvaluatorOutputDefinition OutputDefinition { get; init; }
}