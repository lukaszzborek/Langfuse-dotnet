using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Evaluator to attach to an evaluation rule.
/// </summary>
public class EvaluationRuleEvaluatorAssignmentInput
{
    /// <summary>
    ///     Stable evaluator identifier. The rule automatically uses that evaluator's latest version.
    /// </summary>
    [JsonPropertyName("evaluatorId")]
    public required string EvaluatorId { get; init; }

    /// <summary>
    ///     Rule-specific prompt-variable mapping. Omit to inherit the evaluator's latest default mapping.
    ///     Code evaluators use the fixed runtime mapping and should omit this.
    /// </summary>
    [JsonPropertyName("variableMapping")]
    public PromptVariableMappingInput[]? VariableMapping { get; init; }
}