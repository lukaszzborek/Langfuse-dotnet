using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Evaluator attached to an evaluation rule.
/// </summary>
public class EvaluatorAssignment
{
    /// <summary>
    ///     Stable identifier of the evaluator associated with this rule.
    /// </summary>
    [JsonPropertyName("evaluatorId")]
    public required string EvaluatorId { get; init; }

    /// <summary>
    ///     Stored rule-specific override, or null when the evaluator's latest default mapping is inherited.
    ///     Legacy mappings are flagged via <see cref="PromptVariableMapping.IsLegacy" />.
    /// </summary>
    [JsonPropertyName("variableMapping")]
    public PromptVariableMapping[]? VariableMapping { get; init; }
}