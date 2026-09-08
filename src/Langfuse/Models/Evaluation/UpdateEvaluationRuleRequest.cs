using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Partial update for an evaluation rule. Provide only the fields to change; at least one is required.
///     Legacy trace and dataset rules can only be deactivated via <see cref="Enabled" />.
/// </summary>
public class UpdateEvaluationRuleRequest
{
    /// <summary>
    ///     New human-readable rule name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     New desired live-execution state. Setting true is rejected when the resulting assignment list is empty.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; init; }

    /// <summary>
    ///     New fraction of matching observations to evaluate. Omit to keep the current value.
    /// </summary>
    [JsonPropertyName("sampling")]
    public double? Sampling { get; init; }

    /// <summary>
    ///     Complete replacement filter list. An empty list matches every incoming observation.
    /// </summary>
    [JsonPropertyName("filter")]
    public EvaluationRuleFilter[]? Filter { get; init; }

    /// <summary>
    ///     Complete replacement assignment list. An empty list disables the rule.
    /// </summary>
    [JsonPropertyName("evaluatorAssignments")]
    public EvaluationRuleEvaluatorAssignmentInput[]? EvaluatorAssignments { get; init; }
}