using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Evaluation-rule assignment associated with an evaluator.
/// </summary>
public class EvaluationRuleAssignment
{
    /// <summary>
    ///     Stable identifier of the assigned evaluation rule.
    /// </summary>
    [JsonPropertyName("evaluationRuleId")]
    public required string EvaluationRuleId { get; init; }

    /// <summary>
    ///     Rule-specific variable mapping override. Null when the evaluator's latest default mapping is inherited.
    ///     Legacy mappings are flagged via <see cref="PromptVariableMapping.IsLegacy" />.
    /// </summary>
    [JsonPropertyName("variableMappingOverride")]
    public PromptVariableMapping[]? VariableMappingOverride { get; init; }
}