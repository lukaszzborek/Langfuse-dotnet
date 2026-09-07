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
    ///     User prompt string shortcut or an ordered list of chat messages. Variables use <c>{{variable}}</c> syntax.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required EvaluatorChatPromptInput Prompt { get; init; }

    /// <summary>
    ///     Explicit model configuration. Omit to use the project's default evaluation model.
    /// </summary>
    [JsonPropertyName("modelConfig")]
    public EvaluatorModelConfig? ModelConfig { get; init; }

    /// <summary>
    ///     Default prompt-variable mapping, or null when no default is configured.
    /// </summary>
    [JsonPropertyName("variableMapping")]
    public PromptVariableMappingInput[]? VariableMapping { get; init; }

    /// <summary>
    ///     Structured output schema returned by this evaluator.
    /// </summary>
    [JsonPropertyName("outputDefinition")]
    public required EvaluatorOutputDefinition OutputDefinition { get; init; }
}