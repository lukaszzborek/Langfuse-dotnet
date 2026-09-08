using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Live evaluation rule for incoming observations. A rule determines which evaluators should be used,
///     which observations should trigger scoring, how often scoring should run, and which observation fields
///     should populate prompt variables.
/// </summary>
public class EvaluationRule
{
    /// <summary>
    ///     Stable evaluation-rule identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Human-readable rule name. Independent from evaluator names and not required to be unique.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     User who created this rule, or null when no user can be resolved.
    /// </summary>
    [JsonPropertyName("createdBy")]
    public Creator? CreatedBy { get; init; }

    /// <summary>
    ///     Whether live execution is enabled for this rule.
    /// </summary>
    [JsonPropertyName("enabled")]
    public required bool Enabled { get; init; }

    /// <summary>
    ///     Fraction of matching observations that should be evaluated, between 0 and 1. 1 evaluates every match.
    /// </summary>
    [JsonPropertyName("sampling")]
    public required double Sampling { get; init; }

    /// <summary>
    ///     Stored filter conditions returned verbatim. An empty list matches every incoming object.
    /// </summary>
    [JsonPropertyName("filter")]
    public required EvaluationRuleReadFilter[] Filter { get; init; }

    /// <summary>
    ///     Evaluators attached to this rule in deterministic assignment order.
    /// </summary>
    [JsonPropertyName("evaluatorAssignments")]
    public required EvaluatorAssignment[] EvaluatorAssignments { get; init; }

    /// <summary>
    ///     Timestamp when the evaluation rule was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Timestamp when the evaluation rule was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public required DateTime UpdatedAt { get; init; }
}