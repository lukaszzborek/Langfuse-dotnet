using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Request body for creating an evaluator. If the same name already exists in your project, Langfuse
///     creates the next version and returns it.
/// </summary>
public class CreateEvaluatorRequest
{
    /// <summary>
    ///     Evaluator name within the authenticated project.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

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
